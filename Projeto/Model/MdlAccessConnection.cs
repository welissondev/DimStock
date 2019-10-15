﻿using System;
using System.Data.OleDb;
using System.Data;

namespace SysEstoque.Model
{
    public class MdlAccessConnection : IDisposable
    {
        #region Propriedades 
        private readonly OleDbConnection connection = new OleDbConnection(@"Provider = Microsoft.jet.oledb.4.0; Data Source =" + AppDomain.CurrentDomain.BaseDirectory.ToString() + @"Padrao\dbestoque.mdb;jet oledb:database password=#admin#");
        public OleDbCommand Command = new OleDbCommand();
        public OleDbParameter Parameter = new OleDbParameter();
        public OleDbTransaction Transaction;
        private bool disposed = false;
        #endregion

        #region Obter OledbConnection
        public OleDbConnection OpenConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection;
            }
            catch (OleDbException)
            {
                throw;
            }

        }
        #endregion

        #region AddParameter()
        public void AddParameter(string name, OleDbType type, object value)
        {
            OleDbParameter parametro = new OleDbParameter
            {
                ParameterName = name,
                OleDbType = type,
                Value = value
            };
            Command.Parameters.Add(parametro);
        }
        #endregion

        #region ExecuteNonQuery()
        public int ExecuteNonQuery(string sql)
        {
            try
            {
                Command.CommandText = sql;
                Command.Connection = OpenConnection();
                return Command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region ExecuteScalar()
        public string ExecuteScalar(string sql)
        {
            try
            {
                Command.CommandText = sql;
                Command.Connection = OpenConnection();
                return  Command.ExecuteScalar().ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region ExecuteQuery()
        public OleDbDataReader ExecuteQuery(string sql)
        {
            try
            {
                Command.CommandText = sql;
                Command.Connection = OpenConnection();

                return Command.ExecuteReader();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region ExecuteParameterQuery()
        public OleDbDataReader ExecuteParameterQuery(string sql)
        {
            try
            {
                Command.CommandText = sql;
                Command.Connection = OpenConnection();
                return Command.ExecuteReader();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region ExecuteTransaction()
        public int ExecuteTransaction(string commandSQL)
        {
            try
            {
                Command.CommandText = commandSQL;
                Command.Connection = Transaction.Connection;
                Command.Transaction = Transaction;
                return Command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region ParameterClear()
        public void ParameterClear()
        {
            Command.Parameters.Clear();
        }
        #endregion 

        #region Disposed
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region VirtualDispose()
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                connection.Dispose();
                Command.Dispose();
            }

            disposed = true;
        }
        #endregion 
    }
}