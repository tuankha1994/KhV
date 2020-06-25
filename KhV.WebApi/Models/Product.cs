using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KhV.WebApi.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class ErrorResponse
    {
        public List<string> Errors { get; set; }

        public ErrorResponse()
        {
            Errors = new List<string>();
        }
    }

    public class Paging
    {
        public long TotalItem { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public long TotalPage { get; set; }
    }

    public class ListProductResponse
    {
        public List<ProductDto> Data { get; set; }
        public Paging Paging { get; set; }
    }

    public class BaseParamRequest
    {
        public int TotalItem { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalPage { get; set; }
    }

    public class ProductParamRequest: BaseParamRequest
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public string Description { get; set; }
    }
}
