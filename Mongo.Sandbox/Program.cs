using System;
using System.Collections.Generic;
using System.Configuration;
using MongoDB.Driver;

namespace Mongo.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            var client = new MongoClient(ConfigurationManager.ConnectionStrings["mongodb"].ConnectionString);
            var database = client.GetDatabase("Test");
            var collection = database.GetCollection<Person>("Persons");

            SetupTestPersons(collection);
            collection.DeleteMany(FilterDefinition<Person>.Empty);

            SetupTestPersons(collection);
            UpdateTestPerson(collection);

            Console.WriteLine("Done..");
            Console.ReadLine();
        }

        private static void UpdateTestPerson(IMongoCollection<Person> collection)
        {
            var update = Builders<Person>.Update.Set(s => s.BirthDate, new DateTime(2010, 11, 11));
            collection.UpdateMany(x => x.Name == "Kees", update, new UpdateOptions { IsUpsert = true });
        }

        private static void SetupTestPersons(IMongoCollection<Person> collection)
        {
            var persons = new List<Person>
            {
                new Person{ Name = "Kees", Address = "Voorstraat 14, Den-Haag", BirthDate = new DateTime(1989, 9, 11)},
                new Person{ Name = "Koos", Address = "Voorstraat 115, Utrecht", BirthDate = new DateTime(1990, 10, 7)},
                new Person{ Name = "Nico", Address = "Voorstraat 16, Den-Haag", BirthDate = new DateTime(1995, 8, 1)},
                new Person{ Name = "Jan", Address = "Voorstraat 17, Amsterdam", BirthDate = new DateTime(1991, 6, 5)},
                new Person{ Name = "Nicole", Address = "Voorstraat 22, Amsterdam", BirthDate = new DateTime(1991, 6, 5)},
                new Person{ Name = "Klaas", Address = "Voorstraat 18, Den-Haag", BirthDate = new DateTime(1992, 2, 7)},
                new Person{ Name = "Jen", Address = "Voorstraat 1, Rotterdam", BirthDate = new DateTime(1993, 4, 12)},
                new Person{ Name = "Henk", Address = "Voorstraat 19, Rotterdam", BirthDate = new DateTime(1993, 4, 12)},
                new Person{ Name = "Ronald", Address = "Voorstraat 88, Rotterdam", BirthDate = new DateTime(1993, 4, 12)},
                new Person{ Name = "Nico", Address = "Voorstraat 20, Eindhoven", BirthDate = new DateTime(1994, 1, 11)},
                new Person{ Name = "Roos", Address = "Voorstraat 26, Eindhoven", BirthDate = new DateTime(1994, 1, 11)},
                new Person{ Name = "Kees", Address = "Voorstraat 22, Ridderkerk", BirthDate = new DateTime(1988, 4, 11)},
            };

            collection.InsertMany(persons);
        }
    }
}