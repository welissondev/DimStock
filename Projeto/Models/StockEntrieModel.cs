﻿using DimStock.AuxilyTools.AuxilyClasses;
using System;
using System.Data;

namespace DimStock.Models
{
    public class StockEntrieModel
    {
        ConnectionTransactionModel dataBaseTransaction;

        public int Id { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime RegisterHour { get; set; }
        public string NFE { get; set; }
        public SupplierModel Supplier { get; set; }
        public MovementModel Movement { get; set; }

        public StockEntrieModel()
        {
            Supplier = new SupplierModel();
            Movement = new MovementModel();
        }

        public bool Insert()
        {
            var sql = string.Empty;
            var actionResult = false;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"INSET INTO stockEntrie(supplierId, movementId, registerDate, registerHour, nfe)
                VALUES(@supplierId, @movementId, @registerDate, @registerHour, @nfe)";

                dataBase.ClearParameter();
                dataBase.AddParameter("@supplierId", Supplier.Id);
                dataBase.AddParameter("@movementId", Movement.Id);
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

            using (var dataBase = new ConnectionModel())
            {
                sql = @"UPDATE stockEntrie SET supplierId = @supplierId, movementId = @movementId,
                registerDate = @registerDate, registerHour = @registerHour, nfe = @nfe WHERE id = @id";

                dataBase.ClearParameter();
                dataBase.AddParameter("@supplierId", Supplier.Id);
                dataBase.AddParameter("@movementId", Movement.Id);
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

            if (MessageNotifier.Reply("Confirma essa operação?",
            "IMPORTANTE") == false) return actionResult;

            using (dataBaseTransaction = new ConnectionTransactionModel())
            {
                if (RemovePosting() == true)
                {
                    if (new MovementModel(dataBaseTransaction) { Id = Movement.Id }.Delete() == true)
                    {
                        actionResult = true;
                        dataBaseTransaction.Commit();
                    }
                }

            }

            return actionResult;
        }

        public bool Finisy()
        {
            var actionResult = false;

            if (MessageNotifier.Reply("Confirma essa operação?",
            "IMPORTANTE") == false) return actionResult;

            using (dataBaseTransaction = new ConnectionTransactionModel())
            {
                if (InsertPosting() == true)
                {
                    if (new MovementModel(dataBaseTransaction) { Id = Movement.Id }.FinishOperation() == true)
                    {
                        actionResult = true;
                        dataBaseTransaction.Commit();
                    }
                }
            }

            return actionResult;
        }

        private bool InsertPosting()
        {
            return new StockModel(dataBaseTransaction).InsertPostingOfEntries(GetItems());
        }
        private bool RemovePosting()
        {
            return new StockModel(dataBaseTransaction).RemovePostingOfEntries(GetItems());
        }

        public DataTable GetItems()
        {
            return new StockEntrieItemModel(this).ListItems();
        }

        public int GetLastId()
        {
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"SELECT MAX(id) FROM stockEntrie";
                Id = dataBase.ExecuteScalar(sql);
            }

            return Id;
        }
    }

}
