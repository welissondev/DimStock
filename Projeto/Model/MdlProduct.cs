﻿using DimStock.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace DimStock.Model
{
    public class MdlProduct
    {
        #region Properties
        private readonly BllProduct product;
        #endregion 

        #region Constructs
        public MdlProduct(BllProduct product)
        {
            this.product = product;
        }
        #endregion 

        #region Register()
        public bool Register()
        {
            using (var connection = new MdlConnection())
            {
                using (connection.Transaction = connection.Open().BeginTransaction())
                {
                    var commandSQL = @"INSERT INTO TBProduto(Fornecedor, Descricao, Codigo, Tamanho, Referencia, PrecoCusto, 
                        PrecoVenda, EstoqueMin, EstoqueMax, CodigoBarras, FotoNome)VALUES(@Fornecedor, @Descricao, @Codigo, @Tamanho, 
                        @Referencia, @PrecoCusto, @PrecoVenda, @EstoqueMin, @EstoqueMax, @CodigoBarras, @FotoNome)";

                    connection.AddParameter("@Fornecedor", OleDbType.VarChar, product.Supplier);
                    connection.AddParameter("@Descricao", OleDbType.VarChar, product.Description);
                    connection.AddParameter("@Codigo", OleDbType.VarChar, product.Code);
                    connection.AddParameter("@Tamanho", OleDbType.VarChar, product.Size);
                    connection.AddParameter("@Referencia", OleDbType.VarChar, product.Reference);
                    connection.AddParameter("@PrecoCusto", OleDbType.Double, product.CostPrice);
                    connection.AddParameter("@PrecoVenda", OleDbType.Double, product.SalePrice);
                    connection.AddParameter("@EstoqueMin", OleDbType.Integer, product.MinStock);
                    connection.AddParameter("@EstoqueMax", OleDbType.Integer, product.MaxStock);
                    connection.AddParameter("@CodigoBarras", OleDbType.VarChar, product.BarCode);
                    connection.AddParameter("@FotoNome", OleDbType.VarChar, product.PhotoName);

                    bool transaction = false;

                    if (connection.ExecuteTransaction(commandSQL) > 0)
                    {
                        product.Id = Convert.ToInt32(connection.ExecuteScalar("SELECT MAX(Id) From TBProduto"));

                        var stock = new MdlStock();

                        if (stock.RelateProduct(connection, product.Id) == true)
                        {
                            connection.Transaction.Commit();
                            transaction = true;
                            BllNotification.Message = "Produto cadastrado com sucesso!";
                        }
                    }

                    return transaction;
                }
            }
        }
        #endregion

        #region Edit()
        public bool Edit(int id)
        {
            using (var con = new MdlConnection())
            {
                using (con.Transaction = con.Open().BeginTransaction())
                {

                    var commandSQL = @"UPDATE TBProduto Set Fornecedor = @Fornecedor, Descricao = @Descricao, Codigo = @Codigo, Tamanho = @Tamanho,
                    Referencia = @Referencia , PrecoCusto = @PrecoCusto, PrecoVenda = @PrecoVenda, EstoqueMin = @EstoqueMin, EstoqueMax = @EstoqueMax, 
                    CodigoBarras = @CodigoBarras , FotoNome = @FotoNome Where Id = " + id;

                    con.AddParameter("@Fornecedor", OleDbType.VarChar, product.Supplier);
                    con.AddParameter("@Descricao", OleDbType.VarChar, product.Description);
                    con.AddParameter("@Codigo", OleDbType.VarChar, product.Code);
                    con.AddParameter("@Tamanho", OleDbType.VarChar, product.Size);
                    con.AddParameter("@Referecia", OleDbType.VarChar, product.Reference);
                    con.AddParameter("@PrecoCusto", OleDbType.Double, product.CostPrice);
                    con.AddParameter("@PrecoVenda", OleDbType.Double, product.SalePrice);
                    con.AddParameter("@EstoqueMin", OleDbType.Integer, product.MinStock);
                    con.AddParameter("@EstoqueMax", OleDbType.Integer, product.MaxStock);
                    con.AddParameter("@CodigoBarras", OleDbType.VarChar, product.BarCode);
                    con.AddParameter("@FotoNome", OleDbType.VarChar, product.PhotoName);

                    bool transaction = false;

                    if (con.ExecuteTransaction(commandSQL) > 0)
                    {
                        var costPrice = Convert.ToDouble(con.ExecuteScalar(
                        "SELECT precoCusto FROM TBProduto WHERE Id = " + id));

                        MdlStock estoque = new MdlStock();

                        if (estoque.UpdateValue(con, costPrice, id) == true)
                        {
                            con.Transaction.Commit();
                            transaction = true;
                            BllNotification.Message = "Produto alterado com sucesso!";
                        }
                    };

                    return transaction;
                }
            }
        }
        #endregion

        #region Delete()
        public bool Delete(int id)
        {
            var exclusionState = false;

            using (var connection = new MdlConnection())
            {
                if (connection.ExecuteNonQuery("DELETE From TBProduto WHERE Id = " + id) > 0)
                {
                    BllNotification.Message = "Produto deletado com sucesso!";
                    exclusionState = true;
                }
                else
                {
                    BllNotification.Message = @"Esse produto já foi deletado, 
                    atualize a lista de dados!";
                }
            }

            return exclusionState;
        }
        #endregion

        #region ListAll()
        public void ListAll()
        {
            using (var connection = new MdlConnection())
            {
                #region Variáveis
                var parameter = connection.Command.Parameters;
                var criterion = string.Empty;
                var commandSQL = string.Empty;
                var productList = new List<BllProduct>();
                #endregion

                #region ConsultaPadrão
                commandSQL = @"SELECT Id, Codigo, Fornecedor, Referencia, Descricao, 
                Tamanho, PrecoCusto, PrecoVenda, FotoNome From TBProduto WHERE Id > 0 ";
                #endregion

                #region Critério + Código
                if (product.Code != string.Empty)
                {
                    criterion += " AND Codigo = @Codigo ";

                    parameter.AddWithValue("@Codigo", string.Format("{0}",
                    product.Code));
                }
                #endregion    

                #region Critério + Tamanho
                if (product.Size != string.Empty)
                {
                    criterion += " AND Tamanho = @Tamanho ";

                    parameter.AddWithValue("@Tamanho", string.Format("{0}",
                    product.Size));
                }
                #endregion     

                #region Critério + Referência
                if (product.Reference != string.Empty)
                {
                    criterion += " AND Referencia = @Referencia";

                    parameter.AddWithValue("@Referencia", string.Format("{0}",
                    product.Reference));
                }
                #endregion     

                #region Critério + Descrição
                if (product.Description != string.Empty)
                {
                    criterion += " AND Descricao = @Descricao";

                    parameter.AddWithValue("@Descricao", string.Format("%{0}%",
                    product.Description));
                }
                #endregion

                #region SQLCommand + Critério
                commandSQL += criterion + " Order By Codigo,Tamanho, Referencia Asc";
                #endregion 

                #region PopularList
                using (var dataReader = connection.QueryWithDataReader(commandSQL))
                {
                    while (dataReader.Read())
                    {
                        var product = new BllProduct
                        {
                            Id = Convert.ToInt32(dataReader["Id"]),
                            Code = Convert.ToString(dataReader["Codigo"]),
                            Reference = Convert.ToString(dataReader["Referencia"]),
                            Supplier = Convert.ToString(dataReader["Fornecedor"]),
                            Description = Convert.ToString(dataReader["Descricao"]),
                            Size = Convert.ToString(dataReader["Tamanho"]),
                            CostPrice = Convert.ToDouble(dataReader["PrecoCusto"]),
                            SalePrice = Convert.ToDouble(dataReader["PrecoVenda"]),
                            PhotoName = Convert.ToString(dataReader["FotoNome"])
                        };

                        productList.Add(product);
                    }

                    product.ListOfRecords = productList;

                }
                #endregion
            }
        }
        #endregion

        #region FetchData()
        public void FetchData()
        {
            var commandSQL = string.Empty;
            var sqlCount = string.Empty;
            var criterion = string.Empty;

            using (var connection = new MdlConnection())
            {
                var paramerter = connection.Command.Parameters;

                #region ConsultaPadrão
                commandSQL = @"SELECT Id, Codigo, Fornecedor, Referencia, Descricao, 
                Tamanho, PrecoCusto, PrecoVenda, FotoNome From TBProduto WHERE Id > 0";

                sqlCount = "SELECT COUNT(*) From TBProduto";

                #endregion

                #region Critério + Codigo
                if (product.QueryByCode != string.Empty)
                {
                    criterion += " AND Codigo LIKE @Codigo";

                    paramerter.AddWithValue("@Codigo", string.Format("{0}",
                    product.QueryByCode));
                }
                #endregion

                #region Critério + Tamanho
                if (product.QueryBySize != string.Empty)
                {
                    criterion += " AND Tamanho LIKE @Tamanho";

                    paramerter.AddWithValue("@Tamanho", string.Format("{0}",
                    product.QueryBySize));
                }
                #endregion

                #region Critério + Referência
                if (product.QueryByReference != string.Empty)
                {
                    criterion += " AND Referencia LIKE @Referencia";

                    paramerter.AddWithValue("@Referencia", string.Format("{0}",
                    product.QueryByReference));
                }
                #endregion

                #region Critério + Descrição
                if (product.QueryByDescription != string.Empty)
                {
                    criterion += " AND Descricao LIKE @Descricao";

                    paramerter.AddWithValue("@Descricao", string.Format("%{0}%",
                    product.QueryByDescription));
                }
                #endregion     

                #region SQLCommand + critério
                commandSQL += criterion + " Order By Codigo, Tamanho, Referencia Asc";
                #endregion

                #region RecordCount
                product.DataPagination.RecordCount = Convert.ToInt32(
                connection.ExecuteScalar(sqlCount));
                #endregion

                #region Popular DataTable
                var dataTable = connection.QueryWithDataTable(commandSQL,
                product.DataPagination.OffSetValue,
                product.DataPagination.PageSize);

                PassDataTableToList(dataTable);
                #endregion 
            }
        }
        #endregion

        #region ViewData()
        public void ViewData(int id)
        {
            var commandSQL = @"SELECT Id, Codigo, Referencia, CodigoBarras, Fornecedor, Descricao, Tamanho, PrecoCusto,
                PrecoVenda, EstoqueMin, EstoqueMax, FotoNome From TBProduto Where Id = " + id;

            using (var connection = new MdlConnection())
            {
                using (var dataReader = connection.QueryWithDataReader(commandSQL))
                {
                    while (dataReader.Read())
                    {
                        product.Id = Convert.ToInt32(dataReader["Id"]);
                        product.Code = dataReader["Codigo"].ToString();
                        product.Reference = dataReader["Referencia"].ToString();
                        product.BarCode = dataReader["CodigoBarras"].ToString();
                        product.Supplier = dataReader["Fornecedor"].ToString();
                        product.Description = dataReader["Descricao"].ToString();
                        product.Size = dataReader["Tamanho"].ToString();
                        product.CostPrice = Convert.ToDouble(dataReader["PrecoCusto"]);
                        product.SalePrice = Convert.ToDouble(dataReader["PrecoVenda"]);
                        product.MinStock = Convert.ToInt32(dataReader["EstoqueMin"]);
                        product.MaxStock = Convert.ToInt32(dataReader["EstoqueMax"]);
                        product.PhotoName = dataReader["FotoNome"].ToString();
                    }
                }
            }
        }
        #endregion

        #region GetDataFromAffectedRecord()
        public string GetDataFromAffectedRecord(int id)
        {
            using (var connection = new MdlConnection())
            {
                var commandSQL = @"SELECT * From TBProduto Where Id = " + id;

                var affectedFieldList = new List<string>();

                using (var dataReader = connection.QueryWithDataReader(commandSQL))
                {
                    while (dataReader.Read())
                    {
                        affectedFieldList.Add("Id:" + dataReader["Id"].ToString());
                        affectedFieldList.Add("Código:" + dataReader["Codigo"].ToString());
                        affectedFieldList.Add("Referência:" + dataReader["Referencia"].ToString());
                        affectedFieldList.Add("Fornecedor:" + dataReader["Fornecedor"].ToString());
                        affectedFieldList.Add("Descrição:" + dataReader["Descricao"].ToString());
                        affectedFieldList.Add("Tamanho:" + dataReader["Tamanho"].ToString());
                        affectedFieldList.Add("PreçoCusto:" + dataReader["PrecoCusto"].ToString());
                        affectedFieldList.Add("PreçoVenda:" + dataReader["PrecoVenda"].ToString());
                        affectedFieldList.Add("CódigoBarras:" + dataReader["CodigoBarras"].ToString());
                        affectedFieldList.Add("FotoNome:" + dataReader["FotoNome"].ToString());
                    }
                }

                return string.Join(" | ", affectedFieldList.Select(x => x.ToString()));
            }
        }
        #endregion

        #region CheckIfCodeProductExists()
        public bool CheckIfCodeProductExists()
        {
            using (var connection = new MdlConnection())
            {
                var commandSQL = @"SELECT Codigo FROM TBProduto WHERE Codigo LIKE '" + product.Code + "'";

                var recordsFounds = 0;

                using (var dataReader = connection.QueryWithDataReader(commandSQL))
                {
                    while (dataReader.Read())
                        recordsFounds = +1;
                }
                return recordsFounds > 0;
            }
        }
        #endregion 

        #region PassDataTableToList()
        private void PassDataTableToList(DataTable dataTable)
        {
            var productList = new List<BllProduct>();

            foreach (DataRow row in dataTable.Rows)
            {
                var product = new BllProduct()
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Code = Convert.ToString(row["Codigo"]),
                    Size = Convert.ToString(row["Tamanho"]),
                    Reference = Convert.ToString(row["Referencia"]),
                    Supplier = Convert.ToString(row["Fornecedor"]),
                    Description = Convert.ToString(row["Descricao"]),
                    CostPrice = Convert.ToDouble(row["PrecoCusto"]),
                    SalePrice = Convert.ToDouble(row["PrecoVenda"]),
                    PhotoName = Convert.ToString(row["FotoNome"])
                };

                productList.Add(product);
            }

            product.ListOfRecords = productList;
        }
        #endregion 
    }
}

