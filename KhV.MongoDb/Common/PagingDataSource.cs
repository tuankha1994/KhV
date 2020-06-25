using System;
using System.Collections.Generic;
using System.Text;

namespace KhV.MongoDb.Common
{
    public class PagingDataSource<T>
    {
        public List<T> Data { get; set; }
        public long Total {get;set;}
    }
}
