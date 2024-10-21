using Microsoft.Data.Sqlite;

namespace PriceSpy.Web.Models
{
    public static class DataBaseHandler
    {
        private static readonly string _dbFileName = "Products.db";
        private static readonly string _dbFile = Path.Combine(DataFromLocalFiles.pathData, _dbFileName);
        private static readonly string _dbConnection = "DataSource=" + _dbFile;
        public static void GetResultsFromDB(Card card)
        {
            using var connection = new SqliteConnection(_dbFile);
            connection.OpenAsync();
            SqliteCommand command = new();
            command.Connection = connection;
            command.CommandText = @"";

            Console.WriteLine(command.ExecuteNonQuery());
        }

        public static void OpenAsync(Card card)
        {
            using var connection = new SqliteConnection(_dbFile);
            {

                connection.Open();

                // Проверка, есть ли товар с таким ID
                string id = card.CardUrl;

                string selectQuery = "SELECT * FROM @dbFileName WHERE ID = @id";
                using (SqliteCommand command = new (selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        //if (!reader.HasRows)
                        //{
                        //    string insertQuery = @"INSERT INTO @dbFileName (ID, CurrentPrice, MinPrice, MaxPrice, CurrentDate, MinPriceDate, MaxPriceDate)
                        //               VALUES (@id, @currentPrice, @minPrice, @maxPrice, @currentDate, @minPriceDate, @maxPriceDate)";

                        //    using var insertCommand = new SqliteCommand(insertQuery, connection);
                        //    insertCommand.Parameters.AddWithValue("@id", productID);
                        //    insertCommand.Parameters.AddWithValue("@currentPrice", currentPrice);
                        //    insertCommand.Parameters.AddWithValue("@minPrice", minPrice);
                        //    insertCommand.Parameters.AddWithValue("@maxPrice", maxPrice);
                        //    insertCommand.Parameters.AddWithValue("@currentDate", currentDate);
                        //    insertCommand.Parameters.AddWithValue("@minPriceDate", minPriceDate);
                        //    insertCommand.Parameters.AddWithValue("@maxPriceDate", maxPriceDate);
                        //    Console.WriteLine(insertCommand.ExecuteNonQuery());
                        //}
                        //if (existingPrice > currentPrice)
                        //{

                        //    string updateMinPriceQuery = "UPDATE Products SET MinPrice = @minPrice, MinPriceDate = @minPriceDate WHERE ID = @id";

                        //}
                        //else if (existingPrice < currentPrice)
                        //{

                        //    string updateMaxPriceQuery = "UPDATE Products SET MaxPrice = @maxPrice, MaxPriceDate = @maxPriceDate WHERE ID = @id";

                        //}
                    }
                }
            }
        }
        public static void LoadAsync()
        {
            if (!CheckAvailable()) CreateTable();
            using var connection = new SqliteConnection(_dbConnection);
            //connection.Open();
        }
        private static bool CheckAvailable()
        {
            FileInfo fileInf = new(_dbFile);
            return fileInf.Exists;
        }
        private static async void CreateTable()
        {

            try
            {
                using var connection = new SqliteConnection(_dbConnection);
                await connection.OpenAsync();

                string createTableQuery = $@"
                    CREATE TABLE IF NOT EXISTS Products (
                        ID TEXT PRIMARY KEY,
                        CurrentPrice REAL NOT NULL,
                        MinPrice REAL NOT NULL,
                        MaxPrice REAL NOT NULL,
                        CurrentDate TEXT NOT NULL,
                        MinPriceDate TEXT NOT NULL,
                        MaxPriceDate TEXT NOT NULL
                    );";

                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }

                Console.WriteLine("DB created");
            }
            catch (Exception)
            {
                Console.WriteLine($"DB file error");
            }
        }
    }
    
}
