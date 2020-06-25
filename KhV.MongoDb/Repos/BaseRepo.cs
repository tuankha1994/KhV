using KhV.MongoDb.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KhV.MongoDb.Repos
{
    public interface IBaseRepo<TEntity> where TEntity : class
    {
        Task<TEntity> AddSync(TEntity obj);
        Task<TEntity> GetByIdAsync(string id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> UpdateAsync(string id, TEntity obj);
        Task<bool> RemoveAsync(string id);
        Task<bool> Remove(FilterDefinition<TEntity> filter);
        Task BatchUpdateAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update);
        Task BatchAddAsync(List<TEntity> l);
        Task DeleteManyAsync(FilterDefinition<TEntity> filter);
        Task<long> CountAsync(FilterDefinition<TEntity> filter);
    }

    public abstract class BaseRepo<TEntity> : IBaseRepo<TEntity> where TEntity : class

    {
        protected readonly IMongoDatabase Database;
        protected readonly IMongoCollection<TEntity> DbSet;
        //public ILog Log { get; set; }
        protected BaseRepo(IMongoDatabase database)
        {
            Database = database;
            DbSet = database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public virtual async Task<TEntity> AddSync(TEntity obj)
        {
            if (obj is ICreateDate createdOn)
            {
                var created = createdOn.CreatedDate;
                if (created == default(DateTime))
                {
                    createdOn.CreatedDate = DateTime.Now;
                }
            }
            await DbSet.InsertOneAsync(obj);
            return obj;
        }

        public virtual async Task<TEntity> GetByIdAsync(string id)
        {
            var data = await DbSet.Find(FilterId(id)).SingleOrDefaultAsync();
            return data;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            var all = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);
            return all.ToList();
        }

        public virtual async Task<TEntity> UpdateAsync(string id, TEntity obj)
        {
            if (obj is IModifiedDate modifiedOn)
            {
                modifiedOn.ModifiedDate = DateTime.Now;
            }

            await DbSet.ReplaceOneAsync(FilterId(id), obj);
            return obj;
        }

        public virtual async Task<bool> RemoveAsync(string id)
        {
            var result = await DbSet.DeleteOneAsync(FilterId(id));
            return result.IsAcknowledged;
        }

        public virtual async Task<bool> Remove(FilterDefinition<TEntity> filter)
        {
            var result = await DbSet.DeleteOneAsync(filter);
            return result.IsAcknowledged;
        }

        public async Task BatchUpdateAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update)
        {
            var updateManyAsync = DbSet?.UpdateManyAsync(filter, update);
            if (updateManyAsync != null) await updateManyAsync;
        }

        public async Task BatchAddAsync(List<TEntity> l)
        {
            foreach (var t in l)
            {
                if (t is ICreateDate createdOn)
                {
                    var created = createdOn.CreatedDate;
                    if (created == default(DateTime))
                    {
                        createdOn.CreatedDate = DateTime.Now;
                    }
                }
            }
            var insertManyAsync = DbSet?.InsertManyAsync(l);
            if (insertManyAsync != null) await insertManyAsync;
        }

        public async Task DeleteManyAsync(FilterDefinition<TEntity> filter)
        {
            var deliveryManyAsync = DbSet?.DeleteManyAsync(filter);
            if (deliveryManyAsync != null) await deliveryManyAsync;
        }

        public async Task<long> CountAsync(FilterDefinition<TEntity> filter)
        {
            return await DbSet.CountDocumentsAsync(filter);
        }


        private static FilterDefinition<TEntity> FilterId(string key)
        {
            return Builders<TEntity>.Filter.Eq("_id", key);
        }

    }
}
