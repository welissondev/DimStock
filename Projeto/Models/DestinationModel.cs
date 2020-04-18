﻿using DimStock.AuxilyTools.AuxilyClasses;
using System;
using System.Collections.Generic;

namespace DimStock.Models
{
    /// <summary>
    /// Representa o modelo de destino do estoque
    /// </summary>
    public partial class DestinationModel
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public List<DestinationModel> List { get; set; }
    }

    public partial class DestinationModel
    {
        public DestinationModel()
        {
            List = new List<DestinationModel>();
        }

        public bool Insert()
        {
            var actionResult = false;
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"INSERT INTO StockDestination
                (Location)VALUES(@Location)";

                dataBase.ClearParameter();
                dataBase.AddParameter("@Location", Location);

                if (dataBase.ExecuteNonQuery(sql) > 0)
                {
                    MessageNotifier.Set("Destino cadastrado " +
                    "com sucesso!", "Sucesso");

                    actionResult = true;
                }
            }

            return actionResult;
        }

        public bool Update(int id)
        {
            var actionResult = false;
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"UPDATE StockDestination SET 
                Location = @Location WHERE Id = @Id";

                dataBase.ClearParameter();
                dataBase.AddParameter("@Location", Location);
                dataBase.AddParameter("@Id", id);

                if (dataBase.ExecuteNonQuery(sql) > 0)
                {
                    MessageNotifier.Set("Destino editado " +
                    "com sucesso!", "Sucesso");

                    actionResult = true;
                }
            }

            return actionResult;
        }

        public bool Delete(int id)
        {
            var actionResult = false;
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"DELETE FROM Destination WHERE Id = @Id";

                dataBase.ClearParameter();
                dataBase.AddParameter("@Id", id);

                if (dataBase.ExecuteNonQuery(sql) > 0)
                {
                    MessageNotifier.Set("Destino deletado " +
                    "com sucesso!", "Sucesso");
                    
                    actionResult = true;
                }
                else
                {
                    MessageNotifier.Set("Esse registro já foi " +
                    "deletado, atualize a lista de dados!", "");
                }
            }

            return actionResult;
        }

        public void ListData()
        {
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"SELECT * From StockDestination";

                using (var reader = dataBase.ExecuteReader(sql))
                {
                    while (reader.Read())
                    {
                        var destination = new DestinationModel()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Location = Convert.ToString(reader["Location"])
                        };

                        List.Add(destination);
                    }
                }
            }
        }

        public void GetDetail(int id)
        {
            var sql = string.Empty;

            using (var dataBase = new ConnectionModel())
            {
                sql = @"SELECT * FROM StockDestination 
                WHERE Id = @Id";

                dataBase.ClearParameter();
                dataBase.AddParameter("@Id", id);

                using (var reader = dataBase.ExecuteReader(sql))
                {
                    while (reader.Read())
                    {
                        Id = Convert.ToInt32(reader["Id"]);
                        Location = Convert.ToString(reader["Location"]);
                    }
                }
            }
        }
    }
}
