﻿using Microsoft.Data.Sqlite;
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
                float? actualPriceDb = reader.IsDBNull(1) ? null : reader.GetFloat(1);
                float? minPriceDb = reader.IsDBNull(2) ? null : reader.GetFloat(2);
                float? maxPriceDb = reader.IsDBNull(3) ? null : reader.GetFloat(3);
                DateTime? currentDate = DateTime.Now;
                DateTime? actualPriceDateDb = reader.IsDBNull(4) ? null : reader.GetDateTime (4);
                DateTime? minPriceDateDb = reader.IsDBNull(5) ? null : reader.GetDateTime(5);
                DateTime? maxPriceDateDb = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
                if (minPriceDb == 0) minPriceDb = null;
                if (maxPriceDb == 0) maxPriceDb = null;
                if (currentPrice == actualPriceDb) return;
                if (currentPrice > actualPriceDb)
                {
                    if (currentPrice >= maxPriceDb || maxPriceDb == null)
                    {
                        maxPriceDb = currentPrice;
                        maxPriceDateDb = currentDate;
                    }

                    if (minPriceDb == null) 
                    {
                        minPriceDb = actualPriceDb;
                        minPriceDateDb = actualPriceDateDb;
                    }
                    
                }
                if (currentPrice < actualPriceDb)
                {
                    if (currentPrice <= minPriceDb || minPriceDb == null)
                    {
                        minPriceDb = currentPrice;
                        minPriceDateDb = currentDate;
                    }

                    if (maxPriceDb == null)
                    {
                        maxPriceDb = actualPriceDb;
                        maxPriceDateDb = actualPriceDateDb;
                    }
                }
                actualPriceDb = currentPrice;
                actualPriceDateDb = currentDate;

                string updateQuery = @"UPDATE Products 
                                          SET ActualPrice = round(@actualPriceDb, 2),
                                              MinPrice = round(@minPriceDb, 2),
                                              MaxPrice = round(@maxPriceDb, 2),
                                              ActualDate = @actualPriceDateDb,
                                              MinPriceDate = @minPriceDateDb,
                                              MaxPriceDate = @maxPriceDateDb
                                        WHERE ID = @id";

                using var updateCommand = new SqliteCommand(updateQuery, connection);
                var productID = card.UrlPrefix + card.CardUrl;
                updateCommand.Parameters.AddWithValue("@id", productID);
                updateCommand.Parameters.AddWithValue("@actualPriceDb", actualPriceDb);
                updateCommand.Parameters.AddWithValue("@minPriceDb", minPriceDb);
                updateCommand.Parameters.AddWithValue("@maxPriceDb", maxPriceDb);
                updateCommand.Parameters.AddWithValue("@actualPriceDateDb", actualPriceDateDb);
                updateCommand.Parameters.AddWithValue("@minPriceDateDb", minPriceDateDb);
                updateCommand.Parameters.AddWithValue("@maxPriceDateDb", maxPriceDateDb);
                updateCommand.ExecuteNonQuery();
            }
        }
    }
}
