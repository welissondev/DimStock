﻿using System;
using System.Data;

namespace DimStock.Models
{
    public class StockInModel
    {
        private ConnectionTransactionModel dataBaseTransaction;

        public int Id { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime RegisterHour { get; set; }
        public string NFE { get; set; }
        public SupplierModel Supplier { get; set; }
        public StockMoveModel StockMove { get; set; }
        public DataTable Items { get; set; }

        public StockInModel()
        {
            Supplier = new SupplierModel();
            StockMove = new StockMoveModel();
        }
        public StockInModel(StockMoveModel stockMove)
        {
            StockMove = stockMove;
        }

        public bool Insert()
        {
            var sql = string.Empty;
            var actionResult = false;

            if (StockInValidationModel.ValidateToInsert(this) == false)
                return actionResult;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"INSERT INTO stockIn(supplierId, stockMoveId, registerDate, registerHour, nfe)
                VALUES(@supplierId, @stockMoveId, @registerDate, @registerHour, @nfe)";

                dataBase.ClearParameter();
                dataBase.AddParameter("@supplierId", Supplier.Id);
                dataBase.AddParameter("@stockMoveId", StockMove.Id);
                dataBase.AddParameter("@registerDate", RegisterDate);
                dataBase.AddParameter("@registerHour", RegisterHour);
                dataBase.AddParameter("@nfe", NFE);

                if (dataBase.ExecuteNonQuery(sql) > 0)
                {
                    actionResult = true;
                }
            }

            return actionResult;
        }
        public bool Update()
        {
            var sql = string.Empty;
            var actionResult = false;

            if (StockInValidationModel.ValidateToUpdate(this) == false)
                return actionResult;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"UPDATE stockIn SET supplierId = @supplierId, stockMoveId = @stockMoveId,
                registerDate = @registerDate, registerHour = @registerHour, nfe = @nfe WHERE id = @id";

                dataBase.ClearParameter();
                dataBase.AddParameter("@supplierId", Supplier.Id);
                dataBase.AddParameter("@stockMoveId", StockMove.Id);
                dataBase.AddParameter("@registerDate", RegisterDate);
                dataBase.AddParameter("@registerHour", RegisterHour);
                dataBase.AddParameter("@nfe", NFE);
                dataBase.AddParameter("@id", Id);

                if (dataBase.ExecuteNonQuery(sql) > 0)
                {
                    actionResult = true;
                }
            }

            return actionResult;
        }
        public bool Delete()
        {
            var actionResult = false;
            var sql = string.Empty;

            if (StockInValidationModel.ValidateToDelete(this) == false)
                return actionResult;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"DELETE FROM stockIn WHERE id = @id";

                dataBase.ClearParameter();
                dataBase.AddParameter("@id", Id);

                actionResult = dataBase.ExecuteNonQuery(sql) > 0;
            }

            return actionResult;
        }

        public bool Finish()
        {
            var actionResult = false;

            if (StockInValidationModel.ValidateToFinisy(this) == false)
                return actionResult;

            using (dataBaseTransaction = new ConnectionTransactionModel())
            {
                if (InsertPostingStock() == true)
                {
                    if (new StockMoveModel(dataBaseTransaction) { Id = StockMove.Id }.Finish() == true)
                    {
                        actionResult = true;
                        dataBaseTransaction.Commit();
                    }
                }
            }

            return actionResult;
        }
        public bool Cancel()
        {
            var actionResult = false;

            if (StockInValidationModel.ValidateToCancel(this) == false)
                return actionResult;

            using (dataBaseTransaction = new ConnectionTransactionModel())
            {
                if (RemovePostingStock() == true)
                {
                    if (new StockMoveModel(dataBaseTransaction) { Id = StockMove.Id }.Cancel() == true)
                    {
                        actionResult = true;
                        dataBaseTransaction.Commit();
                    }
                }
            }

            return actionResult;
        }

        public bool CheckRelationWithStockMove()
        {
            var relatedStatus = false;
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"SELECT * FROM stockIn WHERE 
                stockMoveId = @stockMoveId";

                dataBase.ClearParameter();
                dataBase.AddParameter("stockMoveId", StockMove.Id);

                if (dataBase.ExecuteScalar(sql) > 0)
                {
                    relatedStatus = true;
                }
            }

            return relatedStatus;

        }

        private bool InsertPostingStock()
        {
            var actionResult = false;

            Items = GetItems();

            if (StockInValidationModel.ValidatePostingItems(this) == false)
                return actionResult;

            if (new StockModel(dataBaseTransaction).InsertPostingOfEntries(Items) == true)
            {
                actionResult = true;
            }

            return actionResult;
        }
        private bool RemovePostingStock()
        {
            var actionResult = false;

            Items = GetItems();

            if (StockInValidationModel.ValidatePostingItems(this) == false)
                return actionResult;

            if (new StockModel(dataBaseTransaction).RemovePostingOfEntries(Items) == true)
            {
                actionResult = true;
            }

            return actionResult;
        }

        public DataTable GetItems()
        {
            return new StockInItemModel(this).ListItems();
        }

        public int GetLastId()
        {
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"SELECT MAX(id) FROM stockIn";
                Id = dataBase.ExecuteScalar(sql);
            }

            return Id;
        }
    }

}
