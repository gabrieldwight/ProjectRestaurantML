using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using RestaurantRecommender.MLCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace RestaurantRecommender.MLTrainer
{
    class Program
    {
        private const float predictionuserId = 1;
        private const int predictionrestaurantId = 9;

        private static readonly string fileName = "Restaurant_RecommenderModel.zip";
        private static readonly string training_fileName = "Restaurant_Ratings-train.csv";
        private static readonly string test_fileName = "Restaurant_Ratings-test.csv";
        private static readonly string restaurant_base_fileName = "SampleRestaurantList.csv";

        private static readonly List<RestaurantTest> restaurant_Test_List = new List<RestaurantTest>();
        private static readonly List<Restaurant> restaurant_Detail_List = new List<Restaurant>();
        private static readonly Random random = new Random();
        private static readonly MLContext context = new MLContext();
        private static IEnumerable<RestaurantTest> train_dataset = Enumerable.Empty<RestaurantTest>();
        private static IEnumerable<RestaurantTest> test_dataset = Enumerable.Empty<RestaurantTest>();
        private static readonly Stopwatch stopwatch = new Stopwatch();
        static void Main(string[] args)
        {
            Console.WriteLine("=============== Applying Matrix Factorization machine learning ===============");
            // TODO Assign csv files here
            var training_Data_Location = Path.Combine(Environment.CurrentDirectory, "Data", training_fileName);
            var test_Data_Location = Path.Combine(Environment.CurrentDirectory, "Data", test_fileName);

            // Checking if the training and test file exists
            if (!File.Exists(training_Data_Location) && !File.Exists(test_Data_Location))
            {
                try
                {
                    Generate_Training_And_Testing_CSV_File(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("=============== Exception {0} ===============", ex.Message);
                }
            }
            else
            {
                stopwatch.Start();
                // Read the training data which will be used to train the restaurant recommendation model
                (IDataView training_Data_View, IDataView test_Data_View) = Load_DataSet(context, training_Data_Location, test_Data_Location);

                // Calling the build and train method
                ITransformer model = Build_And_Train_Data_Model(context, training_Data_View);

                // Evaluating dataset model after training
                Evaluate_Dataset_Model(context, test_Data_View, model);
                stopwatch.Stop();

                // Testing prediction for creating a recommendation
                Use_Model_For_Single_Prediction(context, model);

                Console.WriteLine($"Time taken for matrix factorization algorithm - {stopwatch.ElapsedMilliseconds} ms");

                //DataViewSchema modelSchema;
                //ITransformer trained_model = context.Model.Load(Path.Combine(Environment.CurrentDirectory, "Data", fileName), out modelSchema);
                //Use_Model_For_Single_Prediction(context, trained_model);

                // Auto ML running the best algorithm for the dataset model
                Best_Machine_Learning_Algorithm(context, training_Data_View);

                // TODO Save model Logic
                SaveModel(context, training_Data_View.Schema, model);
            }
        }

        // Method to load sample dataset for training and testing 
        public static (IDataView training, IDataView test) Load_DataSet(MLContext mlContext, string training_Data_Location, string test_Data_Location)
        {
            // Read the training data which will be used to train the restaurant recommendation model
            IDataView training_data_view = mlContext.Data.LoadFromTextFile<RestaurantRating>(training_Data_Location, hasHeader: true, separatorChar: ',');
            IDataView test_data_view = mlContext.Data.LoadFromTextFile<RestaurantRating>(test_Data_Location, hasHeader: true, separatorChar: ',');
            return (training_data_view, test_data_view);
        }

        // Setting up the machine learning algorithm for training
        public static ITransformer Build_And_Train_Data_Model(MLContext mlContext, IDataView training_data_view)
        {
            // Data transformation by encoding the features for training
            string userEncoded = nameof(RestaurantRating.userId) + "Encoded";
            string restaurantEncoded = nameof(RestaurantRating.restaurantId) + "Encoded";
            var data_processing_pipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: userEncoded, inputColumnName: nameof(RestaurantRating.userId))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: restaurantEncoded, inputColumnName: nameof(RestaurantRating.restaurantId)));

            // Specify options for matrixfactorization trainer
            MatrixFactorizationTrainer.Options options = new MatrixFactorizationTrainer.Options();
            options.MatrixColumnIndexColumnName = userEncoded;
            options.MatrixRowIndexColumnName = restaurantEncoded;
            options.LabelColumnName = "Label";
            options.NumberOfIterations = 20;
            options.ApproximationRank = 100;

            // Design the training pipeline
            var training_pipeline = data_processing_pipeline.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));

            // training the model to match the dataset
            Console.WriteLine("=============== Training the model ===============");
            ITransformer model = training_pipeline.Fit(training_data_view);
            return model;
        }

        public static void Evaluate_Dataset_Model(MLContext mlContext, IDataView testDataView, ITransformer model)
        {
            // Evaluating the model performance
            Console.WriteLine("=============== Evaluating the model ===============");

            var prediction = model.Transform(testDataView);
            var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");
            Console.WriteLine($"The model evaluation metrics RootMeanSquaredError: {metrics.RootMeanSquaredError:0.##}");
            Console.WriteLine($"The model evaluation metrics RSquared: {metrics.RSquared:0.##}");
            Console.WriteLine($"The model evaluation metrics Absolute loss: {metrics.MeanAbsoluteError:0.##}");
            Console.WriteLine($"The model evaluation metrics Squared loss: {metrics.MeanSquaredError:0.##}");
        }

        public static void Use_Model_For_Single_Prediction(MLContext mlContext, ITransformer model)
        {
            Console.WriteLine("=============== Making a prediction ===============");
            // Testing the possible predictions
            var prediction_engine = mlContext.Model.CreatePredictionEngine<RestaurantRating, RestaurantPrediction>(model);
            // To predict which restaurant will be used as a suggestion for the user
            var restaurant_prediction = prediction_engine.Predict(
                new RestaurantRating()
                {
                    userId = predictionuserId,
                    restaurantId = predictionrestaurantId
                });
            Restaurant restaurantService = new Restaurant();
            Console.WriteLine($"For userId: {predictionuserId} restaurant rating prediction (1 - 5 stars) for restaurant name: {predictionrestaurantId} is: {Math.Round(restaurant_prediction.PredictedRating, 1)}");

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", restaurant_base_fileName)))
            {
                string headerLine = reader.ReadLine();
                string line;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    var values = line.Split(",");

                    restaurant_Detail_List.Add(new Restaurant()
                    {
                        RestaurantId = Convert.ToInt32(values[0]),
                        RestaurantName = values[1],
                        RestaurantType = values[2]
                    });
                }
            }

            var top5 = (from r in restaurant_Detail_List
                        let p = prediction_engine.Predict(
                            new RestaurantRating()
                            {
                                userId = predictionuserId,
                                restaurantId = r.RestaurantId
                            })
                        orderby p.PredictedRating descending
                        select (Restaurant: r.RestaurantId, Score: p.PredictedRating)).Take(5);

            foreach (var t in top5)
            {
                Console.WriteLine($"  Score:{t.Score}\tRestaurant: {t.Restaurant}");
            }
        }

        public static void SaveModel(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            var model_path = Path.Combine(Environment.CurrentDirectory, "Data", fileName);
            Console.WriteLine("=============== Saving the model to a file ===============");
            mlContext.Model.Save(model, trainingDataViewSchema, model_path);
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadLine();
        }

        public static void Generate_Training_And_Testing_CSV_File(MLContext mlContext)
        {
            Console.WriteLine("=============== Please wait reading and writing process for csv files ===============");
            // Reading the base csv file
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "Data", restaurant_base_fileName)))
            {
                throw new FileNotFoundException();
            }
            else
            {
                using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Data", restaurant_base_fileName)))
                {
                    string headerLine = reader.ReadLine();
                    string line;
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        var values = line.Split(",");

                        restaurant_Detail_List.Add(new Restaurant()
                        {
                            RestaurantId = Convert.ToInt32(values[0]),
                            RestaurantName = values[1],
                            RestaurantType = values[2]
                        });
                    }
                }
                // Generating sample training dataset
                Generate_Training_Data(restaurant_Detail_List);
            }

            // Saving the csv file for training data for ML
            var model_path = Path.Combine(Environment.CurrentDirectory, "Data", training_fileName);
            if (!File.Exists(model_path))
            {
                Console.WriteLine("=============== Restaurant Training data started writing to a file process ===============");
                // Loading the generated restaurant list from memory
                var dataview = mlContext.Data.LoadFromEnumerable(restaurant_Test_List);
                // Configuration settings for dataset splitting 
                var split = mlContext.Data.TrainTestSplit(dataview, testFraction: 0.2);
                train_dataset = mlContext.Data.CreateEnumerable<RestaurantTest>(split.TrainSet, reuseRowObject: false);
                test_dataset = mlContext.Data.CreateEnumerable<RestaurantTest>(split.TestSet, reuseRowObject: false);
                
                var csv_content = new StringBuilder();
                // Add csv headers
                csv_content.AppendLine("userId,restaurantId,rating");
                foreach (var item in train_dataset.OrderBy(x => x.userid))
                {
                    //csv_content.AppendLine(string.Join(",", item));
                    csv_content.AppendLine($"{item.userid},{item.restaurantid},{item.rating}");
                }
                File.WriteAllText(model_path, csv_content.ToString());
                Console.WriteLine("=============== Restaurant Training data csv saved to a file complete ===============");
            }
            else
            {
                Console.WriteLine("=============== File already exists ===============");
            }

            // Saving the csv file for test data for ML
            var test_model_path = Path.Combine(Environment.CurrentDirectory, "Data", test_fileName);
            if (!File.Exists(test_model_path))
            {
                Console.WriteLine("=============== Restaurant Test data started writing to a file process ===============");
                var csv_content = new StringBuilder();
                // Add csv headers
                csv_content.AppendLine("userId,restaurantId,rating");
                foreach (var item in test_dataset.OrderBy(x => x.userid))
                {
                    //csv_content.AppendLine(string.Join(",", item));
                    csv_content.AppendLine($"{item.userid},{item.restaurantid},{item.rating}");
                }
                File.WriteAllText(test_model_path, csv_content.ToString());
                Console.WriteLine("=============== Restaurant Test data csv saved to a file ===============");
            }
            else
            {
                Console.WriteLine("=============== File already exists ===============");
            }
        }

        public static List<RestaurantTest> Generate_Training_Data(List<Restaurant> restaurant)
        {
            IEnumerable<int> users_test = Enumerable.Empty<int>();
            users_test = Enumerable.Range(1, 6);
            restaurant.GroupBy(r => r.RestaurantId)
                .ToList()
                .ForEach(r =>
                {
                    // Grouping the restaurant based on restaurant ID
                    foreach (var item in r)
                    {
                        // Looping against the number of users visiting each restaurant group
                        foreach (var user in users_test)
                        {
                            restaurant_Test_List.Add(new RestaurantTest()
                            {
                                restaurantid = (float)item.RestaurantId,
                                userid = (float)user,
                                rating = random.Next(1, 6)
                            });
                        }
                    }
                });
            return restaurant_Test_List;
        }

        // Setting up auto ML
        private static void Best_Machine_Learning_Algorithm(MLContext mlContext, IDataView training_data_view)
        {
            Console.WriteLine("=============== Performing Regression Analysis for the best algorithm for the dataset model ===============");
            var settings = new RegressionExperimentSettings
            {
                MaxExperimentTimeInSeconds = 10
            };

            var experiment = mlContext.Auto().CreateRegressionExperiment(settings);

            var progress = new Progress<RunDetail<RegressionMetrics>>(x =>
            {;
                if (x.ValidationMetrics != null)
                {
                    stopwatch.Start();
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine($"Current result:");
                    Console.WriteLine($"Metrics for Trainer name - {x.TrainerName}");
                    Console.WriteLine($"RSquared - {x.ValidationMetrics.RSquared:0.##}");
                    Console.WriteLine($"RootMeanSquaredError - {x.ValidationMetrics.RootMeanSquaredError:0.##}");
                    Console.WriteLine($"Absolute Loss - {x.ValidationMetrics.MeanAbsoluteError:0.##}");
                    Console.WriteLine($"Squared Loss - {x.ValidationMetrics.MeanSquaredError:0.##}");
                    stopwatch.Stop();
                    Console.WriteLine($"Time taken - {stopwatch.ElapsedMilliseconds} ms");
                }
            });

            var result = experiment.Execute(training_data_view, labelColumnName: "Label", progressHandler: progress);

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Best run:");
            Console.WriteLine($"Trainer name - {result.BestRun.TrainerName}");
            Console.WriteLine($"RSquared - {result.BestRun.ValidationMetrics.RSquared:0.##}");
            Console.WriteLine($"RootMeanSquaredError - {result.BestRun.ValidationMetrics.RootMeanSquaredError:0.##}");
            Console.WriteLine($"Absolute Loss - {result.BestRun.ValidationMetrics.MeanAbsoluteError:0.##}");
            Console.WriteLine($"Squared Loss - {result.BestRun.ValidationMetrics.MeanSquaredError:0.##}");
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

    }
}
