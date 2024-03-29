﻿using DimStock.AuxilyTools.AuxilyClasses;
using System;
using System.Data;

namespace DimStock.Models
{
    public class ProductModel
    {
        private ConnectionTransactionModel dataBaseTransaction;

        public int Id { get; set; }
        public string InternalCode { get; set; }
        public string Description { get; set; }
        public double CostPrice { get; set; }
        public double SalePrice { get; set; }
        public string BarCode { get; set; }
        public CategoryModel Category { get; set; }
        public StockModel Stock { get; set; }

        public ProductModel()
        {
            Category = new CategoryModel();
            Stock = new StockModel(this);
        }

        public ProductModel(CategoryModel category)
        {
            Category = category;
        }

        public bool Insert()
        {
            var actionResult = false;
            var sql = string.Empty;

            if (ProductValidationModel.ValidateToInsert(this) == false)
                return actionResult;

            using (dataBaseTransaction = new ConnectionTransactionModel())
            {
                sql = @"INSERT INTO Product (CategoryId, InternalCode, Description, 
                CostPrice, SalePrice, BarCode) VALUES (@CategoryId, @Code, @InternalCode, 
                @CostPrice, @SalePrice, @BarCode)";

                dataBaseTransaction.ClearParameter();
                dataBaseTransaction.AddParameter("@CategoryId", Category.Id);
                dataBaseTransaction.AddParameter("@InternalCode", InternalCode);
                dataBaseTransaction.AddParameter("@Description", Description);
                dataBaseTransaction.AddParameter("@CostPrice", CostPrice);
                dataBaseTransaction.AddParameter("@SalePrice", SalePrice);
                dataBaseTransaction.AddParameter("@BarCode", BarCode);

                if (dataBaseTransaction.ExecuteNonQuery(sql) > 0)
                {
                    Id = GetLastId();

                    if (new StockModel(dataBaseTransaction, this).RelateProduct() == true)
                    {
                        actionResult = true;
                        dataBaseTransaction.Commit();

                        MessageNotifier.Show("Produto cadastrado " +
                        "com sucesso!", "Sucesso", "!");
                    }
                }
            }

            return actionResult;
        }

        public bool Update()
        {
            var actionResult = false;
            var sql = string.Empty;

            if (ProductValidationModel.ValidateToUpdate(this) == false)
                return actionResult;

            using (dataBaseTransaction = new ConnectionTransactionModel())
            {
                sql = @"UPDATE Product SET CategoryId = @CategoryId, InternalCode = @InternalCode, 
                Description = @Description, CostPrice = @CostPrice, SalePrice = @SalePrice, 
                BarCode = @BarCode WHERE Id = @Id";

                dataBaseTransaction.ClearParameter();
                dataBaseTransaction.AddParameter("@CategoryId", Category.Id);
                dataBaseTransaction.AddParameter("@InternalCode", InternalCode);
                dataBaseTransaction.AddParameter("@Description", Description);
                dataBaseTransaction.AddParameter("@CostPrice", CostPrice);
                dataBaseTransaction.AddParameter("@SalePrice", SalePrice);
                dataBaseTransaction.AddParameter("@BarCode", BarCode);
                dataBaseTransaction.AddParameter("@Id", Id);

                if (dataBaseTransaction.ExecuteNonQuery(sql) > 0)
                {
                    if (new StockModel(dataBaseTransaction, this).Update() == true)
                    {
                        dataBaseTransaction.Commit();
                        MessageNotifier.Show("Produto atualizado " +
                        "com sucesso!", "Sucesso", "!");

                        actionResult = true;
                    }
                }
            }

            return actionResult;
        }

        public bool Delete()
        {
            var actionResult = false;
            var sql = string.Empty;

            if (ProductValidationModel.ValidateToDelete(this) == false)
                return actionResult;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"DELETE FROM Product WHERE Id = @Id";

                dataBase.ClearParameter();
                dataBase.AddParameter("@Id", Id);

                if (dataBase.ExecuteNonQuery(sql) > 0)
                {
                    MessageNotifier.Show("Produto excluido " +
                    "com sucesso!", "Sucesso", "!");

                    actionResult = true;
                }
            }

            return actionResult;
        }

        public bool CheckRegisterStatus()
        {
            var registrationStatus = false;
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"SELECT * FROM product WHERE id = @id";

                dataBase.ClearParameter();
                dataBase.AddParameter("@Id", Id);

                if (dataBase.ExecuteScalar(sql) == 0)
                {
                    return registrationStatus;
                }
            }

            return registrationStatus = true;
        }

        public bool CheckRelationWithStock()
        {
            return new StockModel(this).CheckRelationWithProduct();
        }

        public bool CheckRelationWithCategory()
        {
            var relatedStatus = false;
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"SELECT * FROM Product WHERE 
                CategoryId = @CategoryId";

                dataBase.ClearParameter();
                dataBase.AddParameter("CategoryId", Category.Id);

                if (dataBase.ExecuteScalar(sql) > 0)
                {
                    relatedStatus = true;
                }
            }

            return relatedStatus;
        }

        public bool GetDetails()
        {
            var sql = string.Empty;
            var actionResult = false;

            if (ProductValidationModel.ValidateToGetDetails(this) == false)
                return actionResult;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"SELECT product.* , category.*, stock.min, stock.max FROM (product
                LEFT JOIN category ON product.categoryId = category.id) LEFT JOIN 
                stock ON stock.productId = product.id WHERE product.id = @id ";

                dataBase.ClearParameter();
                dataBase.AddParameter("@id", Id);

                using (var reader = dataBase.ExecuteReader(sql))
                {
                    while (reader.Read())
                    {
                        Id = int.Parse(reader["product.Id"].ToString());
                        InternalCode = reader["internalCode"].ToString();
                        Description = reader["product.description"].ToString();
                        CostPrice = double.Parse(reader["costPrice"].ToString());
                        SalePrice = double.Parse(reader["salePrice"].ToString());
                        BarCode = reader["barCode"].ToString();

                        if (reader["category.id"].ToString() != "0" && reader["category.id"].ToString() != string.Empty)
                            Category.Id = int.Parse(reader["category.id"].ToString());

                        if (reader["category.description"] != DBNull.Value)
                            Category.Description = reader["category.description"].ToString();

                        if (reader["min"].ToString() != "0" && reader["min"].ToString() != string.Empty)
                            Stock.Min = int.Parse(reader["min"].ToString());

                        if (reader["max"].ToString() != "0" && reader["min"].ToString() != string.Empty)
                            Stock.Max = int.Parse(reader["max"].ToString());

                        actionResult = true;
                    }
                }
            }

            return actionResult;
        }

        public int GetQuantityInStock()
        {
            return new StockModel(this).GetQuantity();
        }

        public int GetLastId()
        {
            var sql = string.Empty;
            int lastId;

            sql = @"SELECT MAX(Id) FROM product";

            if (dataBaseTransaction != null)
            {
                dataBaseTransaction.ClearParameter();
                lastId = dataBaseTransaction.ExecuteScalar(sql);
            }
            else
            {
                using (var dataBase = new ConnectionModel())
                {
                    dataBase.ClearParameter();
                    lastId = dataBase.ExecuteScalar(sql);
                }
            }

            return lastId;
        }

        public double GetCostPrice()
        {
            double costPrice = CostPrice;

            using (var dataBase = new ConnectionModel())
            {
                var sql = @"SELECT CostPrice FROM 
                Product WHERE Id = @Id";

                dataBase.ClearParameter();
                dataBase.AddParameter("@Id", Id);

                using (var reader = dataBase.ExecuteReader(sql))
                {
                    while (reader.Read())
                    {
                        costPrice = double.Parse(reader["CostPrice"].ToString());
                    }
                }
            }

            return costPrice;
        }

        public DataTable SearchData()
        {
            var sqlSelect = string.Empty;
            var sqlParameter = string.Empty;
            var sqlOderBy = string.Empty;
            var query = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sqlSelect = @"SELECT Id, InternalCode, Description, 
                CostPrice, SalePrice FROM Product WHERE Id > 0 ";

                sqlOderBy = "Order By Id, InternalCode Desc";

                if (InternalCode != string.Empty)
                {
                    sqlParameter += "AND InternalCode LIKE @InternalCode ";

                    dataBase.AddParameter("@InternalCode", string.
                    Format("{0}%", InternalCode));
                }

                if (Description != string.Empty)
                {
                    sqlParameter += "AND Description LIKE @Description ";

                    dataBase.AddParameter("@Description", string.
                    Format("%{0}%", Description));
                }

                query += sqlSelect + sqlParameter + sqlOderBy;

                return dataBase.ExecuteDataAdapter(query);
            }
        }
    }
}
