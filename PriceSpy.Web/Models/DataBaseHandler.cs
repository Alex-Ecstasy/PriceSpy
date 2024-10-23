using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;

namespace PriceSpy.Web.Models
{
    public static class DataBaseHandler
    {
        private static readonly string _dbFileName = "Products.db";
        private static readonly string _dbFile = Path.Combine(DataFromLocalFiles.pathData, _dbFileName);
        private static readonly string _dbConnection = "DataSource=" + _dbFile;
        public static void FindElementInDb(Card card, SqliteConnection connection)
        {
            var productID = card.UrlPrefix + card.CardUrl;
            string selectQuery = "SELECT * FROM Products WHERE ID = @id";
            using SqliteCommand command = new(selectQuery, connection);
            command.Parameters.AddWithValue("@id", productID);
            var reader = command.ExecuteReader();
            if (reader.HasRows) SetPrices(card, reader, connection); 
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
                        ActualPrice REAL,
                        MinPrice REAL,
                        MaxPrice REAL,
                        ActualDate TEXT,
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
                string insertQuery = @"INSERT INTO Products (ID, ActualPrice, ActualDate)
                               VALUES (@id, round(@actualPrice, 2), @actualDate)";
                var actualDate = DateTime.Now;
                var productID = card.UrlPrefix + card.CardUrl;
                using var insertCommand = new SqliteCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@id", productID);
                insertCommand.Parameters.AddWithValue("@actualPrice", card.Price);
                insertCommand.Parameters.AddWithValue("@actualDate", actualDate);
                insertCommand.ExecuteNonQuery();
            }
        }
        private static void SetPrices(Card card, SqliteDataReader reader, SqliteConnection connection)
        {
            if (reader.Read())
            {
                float? currentPrice = card.Price;
                float? actualPriceDb = reader.GetFloat(1);
                float? minPriceDb = reader.GetFloat(2);
                float? maxPriceDb = reader.GetFloat(3);
                if (currentPrice == actualPriceDb)
                {
                    //Console.WriteLine($@"Parse {actualPriceDb} " + card.Name);
                    return;
                }

                //if (currentPrice > actualPriceDb)
                //{
                //    string updateMinPriceQuery = "UPDATE Products SET MinPrice = @minPrice, MinPriceDate = @minPriceDate WHERE ID = @id";
                //    using var updateCommand = new SqliteCommand(updateMinPriceQuery, connection);
                //    var productID = card.UrlPrefix + card.CardUrl;
                //    updateCommand.Parameters.AddWithValue("@id", productID);
                //    updateCommand.Parameters.AddWithValue("@actualPrice", card.Price);
                //    updateCommand.Parameters.AddWithValue("@actualDate", actualDate);
                //}
                //else
                //{
                //    string updateMaxPriceQuery = "UPDATE Products SET MaxPrice = @maxPrice, MaxPriceDate = @maxPriceDate WHERE ID = @id";
                //}

                if (currentPrice > actualPriceDb)
                {//     10              1
                    currentPrice = actualPriceDb; //currentPrice < maxPriceDb
                    //       10             9
                    if (currentPrice >= maxPriceDb)
                    {
                        maxPriceDb = currentPrice;
                    }

                }

                if (currentPrice < actualPriceDb)
                {//          1             10
                    currentPrice = actualPriceDb; //currentPrice > minPriceDb
                    //       1              10
                    if (currentPrice <= minPriceDb)
                    {
                        minPriceDb = currentPrice;
                    }
                }

            }

        }
    }
    
}
