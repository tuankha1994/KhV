using KhV.MongoDb.Common;
using KhV.MongoDb.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KhV.MongoDb.Repos
{
    public interface IProductRepo : IBaseRepo<Product>
    {
        Task<PagingDataSource<Product>> GetAllProducts(string sku, string name, decimal? priceFrom, decimal? priceTo, string description, int skip, int limit);
        Task<Product> GetById(int id);
        Task<Product> GetBySku(string sku);
        Task<int> GetMaxId();

    }

    public class ProductRepo : BaseRepo<Product>, IProductRepo
    {
        public ProductRepo(IMongoDatabase database) : base(database)
        {
        }

        public async Task<PagingDataSource<Product>> GetAllProducts(string sku, string name, decimal? priceFrom, decimal? priceTo, string description, int skip, int limit)
        {
            var filter = Builders<Product>.Filter.Gt(x => x.Id, 0);
            if (!string.IsNullOrEmpty(sku))
            {
                filter = Builders<Product>.Filter.Regex(x => x.Sku, new BsonRegularExpression(sku, "i"));
            }
            if (!string.IsNullOrEmpty(name))
            {
                filter = filter & Builders<Product>.Filter.Regex(x => x.Name, new BsonRegularExpression(name, "i"));
            }
            if (priceFrom.HasValue)
            {
                filter = filter & Builders<Product>.Filter.Gte(x => x.Price, priceFrom.Value);
            }
            if (priceTo.HasValue)
            {
                filter = filter & Builders<Product>.Filter.Lte(x => x.Price, priceTo.Value);
            }
            if (!string.IsNullOrEmpty(description))
            {
                filter = filter & Builders<Product>.Filter.Regex(x => x.Description, new BsonRegularExpression(description, "i"));
            }
            return new PagingDataSource<Product>
            {
                Data = await DbSet.Find(filter).Skip(skip).Limit(limit).ToListAsync(),
                Total = await DbSet.CountDocumentsAsync(filter)
            };
        }

        public async Task<Product> GetById(int id)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.Id, id);
            return await DbSet.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Product> GetBySku(string sku)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.Sku, sku);
            return await DbSet.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<int> GetMaxId()
        {
            var p = await DbSet.Find<Product>(x => true).SortByDescending(d => d.Id).Limit(1).FirstOrDefaultAsync();
            return p != null ? p.Id : 0;
        }
    }
}
