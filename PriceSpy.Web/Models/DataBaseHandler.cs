using Microsoft.Data.Sqlite;

namespace PriceSpy.Web.Models
{
    public static class DataBaseHandler
    {
        private static readonly string _dbFileName = "Products.db";
        private static readonly string _dbFile = Path.Combine(DataFromLocalFiles.pathData, _dbFileName);
        private static readonly string _dbConnection = "DataSource=" + _dbFile;
        public static void FindElementInDb(Card card, SqliteConnection connection)
        {
            string id = card.CardUrl;
            var productID = card.UrlPrefix + card.CardUrl;
            string selectQuery = "SELECT * FROM Products WHERE ID = @id";
            using SqliteCommand command = new(selectQuery, connection);
            command.Parameters.AddWithValue("@id", productID);
            using var reader = command.ExecuteReader();
            if (reader.HasRows) SetPrices(card, connection); 
            else InsertElement(card, connection);
        }
        public static async Task<SqliteConnection> LoadAsync()
        {
            if (!CheckAvailable()) CreateTable();
            var connection = new SqliteConnection(_dbConnection);
            await connection.OpenAsync();
            return connection;
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
                        CurrentPrice REAL,
                        MinPrice REAL,
                        MaxPrice REAL,
                        CurrentDate TEXT,
                        MinPriceDate TEXT,
                        MaxPriceDate TEXT
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
        private static void InsertElement(Card card, SqliteConnection connection)
        {
            if (card.IsAvailable && card.Price != 0)
            {
                string insertQuery = @"INSERT INTO Products (ID, CurrentPrice, CurrentDate)
                               VALUES (@id, @currentPrice, @currentDate)";
                var currentDate = DateTime.Now;
                var productID = card.UrlPrefix + card.CardUrl;
                //var price = card.Price.ToString();
                using var insertCommand = new SqliteCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@id", productID);
                insertCommand.Parameters.AddWithValue("@currentPrice", card.Price);
                insertCommand.Parameters.AddWithValue("@currentDate", currentDate);
                insertCommand.ExecuteNonQuery();
            }
        }
        private static void SetPrices(Card card, SqliteConnection connection)
        {
            //if (existingPrice > currentPrice)
            {
                string updateMinPriceQuery = "UPDATE Products SET MinPrice = @minPrice, MinPriceDate = @minPriceDate WHERE ID = @id";
            }
            //else if (existingPrice < currentPrice)
            {
                string updateMaxPriceQuery = "UPDATE Products SET MaxPrice = @maxPrice, MaxPriceDate = @maxPriceDate WHERE ID = @id";
            }
        }
    }
    
}
