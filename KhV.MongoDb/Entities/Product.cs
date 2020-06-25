using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KhV.MongoDb.Entities
{
    public class Product : ICreateDate, IModifiedDate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DocId { get; set; }
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
