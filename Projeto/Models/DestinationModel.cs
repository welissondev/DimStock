﻿using DimStock.AuxilyTools.AuxilyClasses;
using System;
using System.Data;

namespace DimStock.Models
{
    /// <summary>
    /// Representa o modelo de destino do estoque
    /// </summary>
    public partial class DestinationModel
    {
        public int Id { get; set; }
        public string Location { get; set; }
    }

    public partial class DestinationModel
    {
        public DestinationModel()
        {
        }

        public bool Insert()
        {
            var actionResult = false;
            var sql = string.Empty;

            if (DestinationValidationModel.ValidateToInsert(this) == false)
                return actionResult;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"INSERT INTO StockDestination
                (Location)VALUES(@Location)";

                dataBase.ClearParameter();
                dataBase.AddParameter("@Location", Location);

                if (dataBase.ExecuteNonQuery(sql) > 0)
                {
                    MessageNotifier.Show("Destino cadastrado " +
                    "com sucesso!", "Sucesso");

                    actionResult = true;
                }
            }

            return actionResult;
        }

        public bool Update()
        {
            var actionResult = false;
            var sql = string.Empty;

            if (DestinationValidationModel.ValidateToUpdate(this) == false)
                return actionResult;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"UPDATE StockDestination SET 
                Location = @Location WHERE Id = @Id";

                dataBase.ClearParameter();
                dataBase.AddParameter("@Location", Location);
                dataBase.AddParameter("@Id", Id);

                if (dataBase.ExecuteNonQuery(sql) > 0)
                {
                    MessageNotifier.Show("Destino editado " +
                    "com sucesso!", "Sucesso");

                    actionResult = true;
                }
            }

            return actionResult;
        }

        public bool Delete()
        {
            var actionResult = false;
            var sql = string.Empty;

            if (DestinationValidationModel.ValidateToDelete(this) == false)
                return actionResult;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"DELETE FROM Destination WHERE Id = @Id";

                dataBase.ClearParameter();
                dataBase.AddParameter("@Id", Id);

                if (dataBase.ExecuteNonQuery(sql) > 0)
                {
                    MessageNotifier.Show("Destino deletado " +
                    "com sucesso!", "Sucesso", "!");

                    actionResult = true;
                }
            }

            return actionResult;
        }

        public bool GetDetails()
        {
            var sql = string.Empty;
            var actionResult = false;

            if (DestinationValidationModel.ValidateToGetDetails(this) == false)
                return actionResult;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"SELECT * FROM StockDestination 
                WHERE Id = @Id";

                dataBase.ClearParameter();
                dataBase.AddParameter("@Id", Id);

                using (var reader = dataBase.ExecuteReader(sql))
                {
                    if (reader.FieldCount > 0)
                    {
                        while (reader.Read())
                        {
                            Id = Convert.ToInt32(reader["Id"]);
                            Location = Convert.ToString(reader["Location"]);
                        }

                        actionResult = true;
                    }
                }
            }

            return actionResult;
        }

        public bool CheckIfRegister()
        {
            /*Essa verificação também precisou ser feita pelo 
             nome do local de destino, porque a regra de negócio não 
             permiti dois destinos com o mesmo nome*/

            var registrationStatus = false;
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"SELECT * FROM Destination WHERE Id = @Id
                OR Location = @Location";

                dataBase.ClearParameter();
                dataBase.AddParameter("@Id", Id);
                dataBase.AddParameter("@Location", Location);

                if (dataBase.ExecuteScalar(sql) == 0)
                {
                    return registrationStatus;
                }
            }

            return registrationStatus = true;
        }

        public DataTable SearchData()
        {
            var sql = string.Empty;
            var searchResult = new DataTable();

            using (var dataBase = new ConnectionModel())
            {
                sql = @"SELECT * From StockDestination";
                searchResult = dataBase.ExecuteDataAdapter(sql);
            }

            return searchResult;
        }
    }
}
