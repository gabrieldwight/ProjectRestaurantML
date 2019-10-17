using Microsoft.ML;
using Microsoft.ML.Trainers;
using RestaurantRecommender.MLCommon;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace RestaurantRecommender.MLTrainer
{
    class Program
    {
        private const float predictionuserId = 4;
        private const int predictionrestaurantId = 4;

        private static readonly string fileName = "Restaurant_RecommenderModel.zip";
        private static readonly string training_fileName = "Restaurant_Ratings-train.csv";
        private static readonly string test_fileName = "Restaurant_Ratings-test.csv";
        private static readonly string restaurant_base_fileName = "SampleRestaurantList.csv";

        private static readonly List<RestaurantTest> restaurant_Test_List = new List<RestaurantTest>();
        private static readonly Random random = new Random();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // TODO Assign csv files here
            var training_Data_Location = Path.Combine(Environment.CurrentDirectory, "Data", training_fileName);
            var test_Data_Location = Path.Combine(Environment.CurrentDirectory, "Data", test_fileName);

            // Checking if the training and test file exists
            if (!File.Exists(training_Data_Location) && !File.Exists(test_Data_Location))
            {
                try
                {
                    Generate_Training_CSV_File();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("=============== Exception {0} ===============", ex.Message);
                }
            }
            else
            {
                var context = new MLContext();

                // Read the training data which will be used to train the restaurant recommendation model
                (IDataView training_Data_View, IDataView test_Data_View) = Load_DataSet(context, training_Data_Location, test_Data_Location);

                // Calling the build and train method
                ITransformer model = Build_And_Train_Data_Model(context, training_Data_View);

                // Evaluating dataset model after training
                Evaluate_Dataset_Model(context, test_Data_View, model);

                // Testing prediction for creating a recommendation
                Use_Model_For_Single_Prediction(context, model);

                // TODO Save model Logic
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
            var data_processing_pipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: nameof(RestaurantRating.userId))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "restaurantIdEncoded", inputColumnName: nameof(RestaurantRating.restaurantId)));

            // Specify options for matrixfactorization trainer
            MatrixFactorizationTrainer.Options options = new MatrixFactorizationTrainer.Options();
            options.MatrixColumnIndexColumnName = "userIdEncoded";
            options.MatrixRowIndexColumnName = "restaurantIdEncoded";
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
            Console.WriteLine("The model evaluation metrics RootMeanSquaredError: " + metrics.RootMeanSquaredError);
            Console.WriteLine("The model evaluation metrics RSquared: " + metrics.RSquared);
        }

        public static void Use_Model_For_Single_Prediction(MLContext mlContext, ITransformer model)
        {
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
            Console.WriteLine("For userId:" + predictionuserId + " restaurant rating prediction (1 - 5 stars) for restaurant name:" + predictionrestaurantId + " is:" + Math.Round(restaurant_prediction.Score, 1));
        }

        public static void SaveModel(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            var model_path = Path.Combine(Environment.CurrentDirectory, "Data", fileName);
            Console.WriteLine("=============== Saving the model to a file ===============");
            mlContext.Model.Save(model, trainingDataViewSchema, model_path);
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadLine();
        }

        public static void Generate_Training_CSV_File()
        {
            Console.WriteLine("=============== Please wait reading and writing process for csv files ===============");
            // Reading the base csv file
            List<string> Restaurant_ID = new List<string>();

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

                        Restaurant_ID.Add(values[0]);
                    }
                }

                for (int i = 0; i < 10000; i++)
                {
                    Generate_RestaurantRating_List(Generate_Training_Data(Restaurant_ID));
                }
            }

            // Saving the csv file for training data for ML
            var model_path = Path.Combine(Environment.CurrentDirectory, "Data", training_fileName);
            if (!File.Exists(model_path))
            {
                Console.WriteLine("=============== Restaurant Training data started writing to a file process ===============");
                var csv_content = new StringBuilder();
                // Add csv headers
                csv_content.AppendLine("userId,restaurantId,rating");
                foreach (var item in restaurant_Test_List.OrderBy(x => x.userid))
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
            // Generating test data for ML
            var test_list = Generate_Test_Data(Restaurant_ID);

            var test_model_path = Path.Combine(Environment.CurrentDirectory, "Data", test_fileName);
            if (!File.Exists(test_model_path))
            {
                Console.WriteLine("=============== Restaurant Test data started writing to a file process ===============");
                var csv_content = new StringBuilder();
                // Add csv headers
                csv_content.AppendLine("userId,restaurantId,rating");
                foreach (var item in test_list)
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

        public static RestaurantTest Generate_Training_Data(List<string> id)
        {
            int index = random.Next(id.Count);
            float r_id = float.Parse(id[index], CultureInfo.InvariantCulture.NumberFormat);
            return new RestaurantTest()
            {
                restaurantid = r_id,
                userid = random.Next(1, 7),
                rating = random.Next(1, 6)
            };
        }

        public static List<RestaurantTest> Generate_Test_Data(List<string> id)
        {
            IEnumerable<int> ratings_test = Enumerable.Empty<int>();
            IEnumerable<int> users_test = Enumerable.Empty<int>();
            List<RestaurantTest> r_tests = new List<RestaurantTest>();
            ratings_test = Enumerable.Range(1, 5);
            users_test = Enumerable.Range(1, 6);
            for (int i = 0; i < 30; i++)
            {
                int index = random.Next(id.Count);
                int rate_index = random.Next(ratings_test.ToList().Count);
                int user_index = random.Next(users_test.ToList().Count);

                r_tests.Add(new RestaurantTest()
                {
                    rating = ratings_test.ToList().ElementAt(rate_index),
                    userid = users_test.ToList().ElementAt(user_index),
                    restaurantid = float.Parse(id[index], CultureInfo.InvariantCulture.NumberFormat)
                });
            }
            return r_tests.OrderBy(x => x.userid).ToList();
        }

        public static List<RestaurantTest> Generate_RestaurantRating_List(RestaurantTest restaurantTest)
        {
            if (restaurant_Test_List != null && restaurantTest != null)
            {
                restaurant_Test_List.Add(restaurantTest);
            }
            return restaurant_Test_List;
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
