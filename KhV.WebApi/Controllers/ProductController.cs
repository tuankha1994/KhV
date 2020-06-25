using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KhV.WebApi.Models;
using KhV.MongoDb.Repos;
using KhV.MongoDb.Entities;
using Serilog;

namespace KhV.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo ProductRepo;

        public ProductController(IProductRepo productRepo)
        {
            ProductRepo = productRepo;
        }
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<ListProductResponse>> Get([FromQuery] ProductParamRequest param = null)
        {
            try
            {
                var pageIndex = param?.Page ?? 0;
                var limit = param?.Limit > 0 ? param.Limit : 100;
                var prods = await ProductRepo.GetAllProducts(param.Sku, param.Name, param.PriceFrom, param.PriceTo, param.Description, pageIndex * limit, limit);
                var data = prods.Data.Select(x => new ProductDto
                {
                    Id = x.Id,
                    Sku = x.Sku,
                    Name = x.Name,
                    Price = x.Price,
                    Description = x.Description,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.ModifiedDate
                });
                var res = new ListProductResponse
                {
                    Data = data.ToList(),
                    Paging = new Paging
                    {
                        Page = pageIndex,
                        Limit = limit,
                        TotalItem = prods.Total,
                        TotalPage = prods.Total / limit + 1
                    }
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> Get(int id)
        {
            try
            {
                var x = await ProductRepo.GetById(id);
                if (x == null)
                {
                    return Ok(new ErrorResponse
                    {
                        Errors = new List<string> { "not fount id = " + id }
                    });
                }
                var res = new ProductDto
                {
                    Id = x.Id,
                    Sku = x.Sku,
                    Name = x.Name,
                    Price = x.Price,
                    Description = x.Description,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.ModifiedDate
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<object>> Post([FromBody] ProductDto req)
        {
            try
            {
                var errors = new List<string>();
                if (string.IsNullOrEmpty(req.Sku))
                {
                    errors.Add("Sku must be not null");
                }
                if (string.IsNullOrEmpty(req.Name))
                {
                    errors.Add("Name must be not null");
                }
                if (req.Price <= 0)
                {
                    errors.Add("Price must be > 0");
                }

                if (errors.Any())
                {
                    return Ok(new ErrorResponse
                    {
                        Errors = errors
                    });
                }

                var existed = await ProductRepo.GetBySku(req.Sku);
                if (existed != null)
                {
                    return Ok(new ErrorResponse
                    {
                        Errors = new List<string> { $"sku {req.Sku} existed" }
                    });
                }

                var idMax = await ProductRepo.GetMaxId();
                var pr = new Product
                {
                    Id = idMax + 1,
                    Sku = req.Sku,
                    Name = req.Name,
                    Description = req.Description,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };
                var p = await ProductRepo.AddSync(pr);
                return Ok(pr);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> Put(int id, [FromBody] Product req)
        {
            try
            {
                if (id <= 0)
                {
                    return Ok(new ErrorResponse
                    {
                        Errors = new List<string> { $"id <= 0" }
                    });
                }

                var p = await ProductRepo.GetById(id);
                if (p == null)
                {
                    return Ok(new ErrorResponse
                    {
                        Errors = new List<string> { $"id {id} is not exist" }
                    });
                }

                if (!string.IsNullOrEmpty(req.Sku) && p.Sku != req.Sku)
                {
                    p.Sku = req.Sku;
                }

                if (!string.IsNullOrEmpty(req.Name) && p.Name != req.Name)
                {
                    p.Name = req.Name;
                }

                if (req.Price >= 0 && p.Price != req.Price)
                {
                    p.Price = req.Price;
                }

                if (!string.IsNullOrEmpty(req.Description) && p.Description != req.Description)
                {
                    p.Description = req.Description;
                }
                p.ModifiedDate = DateTime.Now;
                await ProductRepo.UpdateAsync(p.DocId, p);
                var res = new ProductDto
                {
                    Id = p.Id,
                    Sku = p.Sku,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.ModifiedDate
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Ok(new ErrorResponse
                    {
                        Errors = new List<string> { $"id <= 0" }
                    });
                }
                var p = await ProductRepo.GetById(id);
                if (p == null)
                {
                    return Ok(new ErrorResponse
                    {
                        Errors = new List<string> { $"id {id} is not exist" }
                    });
                }
                await ProductRepo.RemoveAsync(p.DocId);
                var res = new ProductDto
                {
                    Id = p.Id,
                    Sku = p.Sku,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.ModifiedDate
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }
    }
}
