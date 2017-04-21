using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Performance
{
    class Program
    {
        private static IMongoCollection<DocumentDateTime> dateTimecollection;
        private static IMongoCollection<DocumentBool> boolsCollection;
        private static int amountToTest = 100000;

        static void Main(string[] args)
        {
            var client = new MongoClient(ConfigurationManager.ConnectionStrings["mongodb"].ConnectionString);
            var database = client.GetDatabase("Performance");

            dateTimecollection = database.GetCollection<DocumentDateTime>(nameof(DocumentDateTime));
            boolsCollection = database.GetCollection<DocumentBool>(nameof(DocumentBool));

            InsertData();
            FindDocuments();

            Console.WriteLine("Done...");
            Console.ReadLine();
        }

        private static void FindDocuments()
        {
            long avgDateTime = 0;
            long avgBools = 0;
            int limit = 10;

            for (int i = 0; i < limit; i++)
            {
                var stopWatch = Stopwatch.StartNew();
                var aggDt = dateTimecollection.Aggregate()
                    .Match(new BsonDocument
                    {
                        { "Finished", (DateTime?)null}
                    })
                    .Group(new BsonDocument
                    {
                        {"_id", 0},
                        {
                            "total", new BsonDocument
                            {
                                {
                                    "$sum", "$Index"
                                }
                            }
                        }
                    })
                    .Project(new BsonDocument
                    {
                        {"_id", 0},
                        {"total", 1},
                    });

                aggDt.Single();
                stopWatch.Stop();
                avgDateTime += stopWatch.ElapsedMilliseconds;
                Console.WriteLine($"DateTime: {stopWatch.ElapsedMilliseconds}ms");

                stopWatch = Stopwatch.StartNew();

                var aggBool = boolsCollection.Aggregate()
                    .Match(new BsonDocument
                    {
                        { "Deleted", false}
                    })
                    .Group(new BsonDocument
                    {
                        {"_id", 0},
                        {
                            "total", new BsonDocument
                            {
                                {
                                    "$sum", "$Index"
                                }
                            }
                        }
                    })
                    .Project(new BsonDocument
                    {
                        {"_id", 0},
                        {"total", 1},
                    });

                aggBool.Single();

                stopWatch.Stop();
                avgBools += stopWatch.ElapsedMilliseconds;
                Console.WriteLine($"Booleans: {stopWatch.ElapsedMilliseconds}ms");
            }

            Console.WriteLine();
            Console.WriteLine($"DateTime: {avgDateTime / limit}ms");
            Console.WriteLine($"Bools: {avgBools / limit}ms");
        }

        private static void InsertData()
        {
            var stopWatch = Stopwatch.StartNew();

            var dDocs = new List<DocumentDateTime>();
            var bDocs = new List<DocumentBool>();

            for (int i = 0; i < amountToTest; i++)
            {
                var notInCollection = i % 2 == 0;

                dDocs.Add(new DocumentDateTime
                {
                    Name = "Testing date times",
                    Data = new BsonDocument(),
                    Created = DateTime.Now.AddMinutes(-i),
                    Finished = notInCollection ? (DateTime?) null : DateTime.Now.AddMinutes(-i + 5),
                    Index = i
                });

                bDocs.Add(new DocumentBool
                {
                    Name = "Testing booleans",
                    Data = new BsonDocument(),
                    Deleted = !notInCollection,
                    Index = i
                });
            }

            stopWatch.Stop();
            Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");

            stopWatch.Start();
            Console.WriteLine("Starting...");

            dateTimecollection.InsertMany(dDocs);
            boolsCollection.InsertMany(bDocs);

            stopWatch.Stop();
            Console.WriteLine($"Done inserting in: {stopWatch.ElapsedMilliseconds}");

            Console.WriteLine("Creating indexes for DateTimeCollection");
            stopWatch.Start();
            dateTimecollection.Indexes.CreateOne(Builders<DocumentDateTime>.IndexKeys.Ascending(key => key.Finished));
            stopWatch.Stop();
            Console.WriteLine($"Done creating indexes: {stopWatch.ElapsedMilliseconds}ms");

            stopWatch.Start();
            Console.WriteLine("Creating indexes for BoolCollection");
            boolsCollection.Indexes.CreateOne(Builders<DocumentBool>.IndexKeys.Ascending(key => key.Deleted));
            stopWatch.Stop();
            Console.WriteLine($"Done creating indexes: {stopWatch.ElapsedMilliseconds}ms");
        }
    }
}
