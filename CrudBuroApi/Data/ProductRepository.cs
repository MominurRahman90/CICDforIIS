using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using CrudBuroApi.Models;

namespace CrudBuroApi.Data
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
        }

        private OracleConnection GetConnection()
        {
            return new OracleConnection(_connectionString);
        }

        public List<Product> GetAll()
        {
            var products = new List<Product>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT ID, NAME, DESCRIPTION, PRICE, STOCK_QUANTITY, IS_ACTIVE, CREATED_AT, UPDATED_AT FROM CICDTest_PRODUCTS WHERE IS_ACTIVE = 1 ORDER BY CREATED_AT DESC";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(MapProduct(reader));
                        }
                    }
                }
            }
            return products;
        }

        public Product GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT ID, NAME, DESCRIPTION, PRICE, STOCK_QUANTITY, IS_ACTIVE, CREATED_AT, UPDATED_AT FROM CICDTest_PRODUCTS WHERE ID = :id";
                    cmd.Parameters.Add(new OracleParameter("id", id));
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapProduct(reader);
                        }
                    }
                }
            }
            return null;
        }

        public Product Create(CreateProductDto dto)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO CICDTest_PRODUCTS (NAME, DESCRIPTION, PRICE, STOCK_QUANTITY, IS_ACTIVE, CREATED_AT) 
                                       VALUES (:name, :description, :price, :stockQuantity, 1, SYSTIMESTAMP) 
                                       RETURNING ID INTO :id";
                    cmd.Parameters.Add(new OracleParameter("name", dto.Name));
                    cmd.Parameters.Add(new OracleParameter("description", (object)dto.Description ?? DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("price", dto.Price));
                    cmd.Parameters.Add(new OracleParameter("stockQuantity", dto.StockQuantity));
                    var idParam = new OracleParameter("id", OracleDbType.Int32, ParameterDirection.Output);
                    cmd.Parameters.Add(idParam);
                    cmd.ExecuteNonQuery();

                    int newId = ((Oracle.ManagedDataAccess.Types.OracleDecimal)idParam.Value).ToInt32();
                    return GetById(newId);
                }
            }
        }

        public Product Update(int id, UpdateProductDto dto)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE CICDTest_PRODUCTS 
                                       SET NAME = :name, DESCRIPTION = :description, PRICE = :price, 
                                           STOCK_QUANTITY = :stockQuantity, IS_ACTIVE = :isActive, UPDATED_AT = SYSTIMESTAMP 
                                       WHERE ID = :id";
                    cmd.Parameters.Add(new OracleParameter("name", dto.Name));
                    cmd.Parameters.Add(new OracleParameter("description", (object)dto.Description ?? DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("price", dto.Price));
                    cmd.Parameters.Add(new OracleParameter("stockQuantity", dto.StockQuantity));
                    cmd.Parameters.Add(new OracleParameter("isActive", dto.IsActive ? 1 : 0));
                    cmd.Parameters.Add(new OracleParameter("id", id));
                    cmd.ExecuteNonQuery();

                    return GetById(id);
                }
            }
        }

        public bool Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM CICDTest_PRODUCTS WHERE ID = :id";
                    cmd.Parameters.Add(new OracleParameter("id", id));
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        private Product MapProduct(OracleDataReader reader)
        {
            return new Product
            {
                Id = Convert.ToInt32(reader["ID"]),
                Name = reader["NAME"].ToString(),
                Description = reader["DESCRIPTION"] == DBNull.Value ? null : reader["DESCRIPTION"].ToString(),
                Price = Convert.ToDecimal(reader["PRICE"]),
                StockQuantity = Convert.ToInt32(reader["STOCK_QUANTITY"]),
                IsActive = Convert.ToInt32(reader["IS_ACTIVE"]) == 1,
                CreatedAt = Convert.ToDateTime(reader["CREATED_AT"]),
                UpdatedAt = reader["UPDATED_AT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["UPDATED_AT"])
            };
        }
    }
}
