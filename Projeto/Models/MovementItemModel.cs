﻿using System.Data;

namespace DimStock.Models
{
    /// <summary>
    /// Representa o modelo de items da movimentação
    /// </summary>
    public partial class MovementItemModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double UnitaryValue { get; set; }
        public double TotalValue { get; set; }
        public double SubTotal { get; set; }
        public StockModel Stock { get; set; }
        public ProductModel Product { get; set; }
        public MovementModel Movement { get; set; }
    }

    public partial class MovementItemModel
    {
        public MovementItemModel()
        {
            Stock = new StockModel();
            Product = new ProductModel();
            Movement = new MovementModel();
        }
        public MovementItemModel(MovementModel movement)
        {
            Movement = movement;
        }

        public bool Insert()
        {
            var actionResult = false;
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"INSERT INTO MovementItem(MovementId, ProductId, StockId, 
                Quantity, UnitaryValue, TotalValue)VALUES(@MovementId, @ProductId, @StockId, 
                @Quantity, @UnitaryValue, @TotalValue)";

                ParameterModel.Clear();
                ParameterModel.Add("@MovementId", Movement.Id);
                ParameterModel.Add("@ProductId", Product.Id);
                ParameterModel.Add("@StockId", Stock.Id);
                ParameterModel.Add("@Quantity", Quantity);
                ParameterModel.Add("@UnitaryValue", UnitaryValue);
                ParameterModel.Add("@TotalValue", TotalValue);

                if (dataBase.ExecuteNonQuery(sql) > 0)
                    actionResult = true;
            }

            return actionResult;
        }

        public bool Delete()
        {
            var actionResult = false;
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"DELETE FROM MovementItem Where Id = @Id";

                ParameterModel.Clear();
                ParameterModel.Add("Id", Id);

                if (dataBase.ExecuteNonQuery(sql) > 0)
                {
                    actionResult = true;
                }
            }

            return actionResult;
        }

        public DataTable ListItems()
        {
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"SELECT MovementItem.*, Product.Description, Product.InternalCode 
                FROM MovementItem INNER JOIN Product ON MovementItem.ProductId = Product.Id WHERE 
                MovementItem.MovementId = @MovementId ORDER BY InternalCode";

                ParameterModel.Clear();
                ParameterModel.Add("@MovementId", Movement.Id);

                return dataBase.ExecuteDataAdapter(sql);
            }
        }
    }
}