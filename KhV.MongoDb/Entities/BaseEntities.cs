using System;
using System.Collections.Generic;
using System.Text;

namespace KhV.MongoDb.Entities
{
    public interface ICreateDate
    {
        DateTime CreatedDate { get; set; }
    }

    public interface IModifiedDate
    {
        DateTime? ModifiedDate { get; set; }
    }
}
