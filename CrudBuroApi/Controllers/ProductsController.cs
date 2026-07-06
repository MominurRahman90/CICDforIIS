using System;
using System.Collections.Generic;
using System.Web.Http;
using CrudBuroApi.Data;
using CrudBuroApi.Models;

namespace CrudBuroApi.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly ProductRepository _repository = new ProductRepository();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var products = _repository.GetAll();
                return Ok(ApiResponse<List<Product>>.Ok(products, "Products retrieved successfully"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var product = _repository.GetById(id);
                if (product == null)
                {
                    return Ok(ApiResponse<Product>.Fail("Product not found"));
                }
                return Ok(ApiResponse<Product>.Ok(product, "Product retrieved successfully"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(CreateProductDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var product = _repository.Create(dto);
                return Ok(ApiResponse<Product>.Ok(product, "Product created successfully"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Update(int id, UpdateProductDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var existing = _repository.GetById(id);
                if (existing == null)
                {
                    return Ok(ApiResponse<Product>.Fail("Product not found"));
                }
                var product = _repository.Update(id, dto);
                return Ok(ApiResponse<Product>.Ok(product, "Product updated successfully"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var existing = _repository.GetById(id);
                if (existing == null)
                {
                    return Ok(ApiResponse<bool>.Fail("Product not found"));
                }
                var result = _repository.Delete(id);
                return Ok(ApiResponse<bool>.Ok(result, "Product deleted successfully"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
