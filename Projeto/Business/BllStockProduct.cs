﻿using DimStock.Model;
using System.Collections.Generic;
using DimStock.Report;
using DimStock.Auxiliary;

namespace DimStock.Business
{
    public class BllStockProduct
    {
        #region Constructs

        public BllStockProduct() { }

        public BllStockProduct(AxlDataPagination dataPagination)
        {
            DataPagination = dataPagination;
        }

        #endregion

        #region BussinesProperties
        public int StockId { get; set; }
        public string Supplier { get; set; }
        public int StockQuantity { get; set; }
        public double StockValue { get; set; }
        public int MinStock { get; set; }
        public int MaxStock { get; set; }
        public string StockResume { get; set; }
        public string StockResult { get; set; }
        public int ProductId { get; set; }
        public int ProductCode { get; set; }
        public int ProductSize { get; set; }
        public string ProductDescription { get; set; }
        public int ProductReference { get; set; }
        public double ProductCostPrice { get; set; }
        public string ProductPhoto { get; set; }
        public List<BllStockProduct> ListOfRecords { get; set; }
        #endregion 

        #region QueryProperties

        private string queryByResume = "All";

        public string QueryByCode { get; set; }
        public string QueryBySize { get; set; }
        public string QueryByReference { get; set; }
        public string QueryByDescription { get; set; }
        public string QueryByResume { get => queryByResume; set => queryByResume = value; }
        public AxlDataPagination DataPagination { get; set; }
       
        #endregion

        #region Methods

        #region Filter()
        public void Filter(string code, string size, string reference,
        string description, int numberOfRecords = 20)
        {
            var stockProduct = new MdlStockProduct();

            ListOfRecords = stockProduct.Filter(code, size, reference,
            description, numberOfRecords);
        }
        #endregion

        #region FetchData()
        public void FetchData()
        {
            var stockProduct = new MdlStockProduct(this);
            stockProduct.FetchData();
        }
        #endregion

        #region ListAll()
        public void ListAll()
        {
            var stockProduct = new MdlStockProduct(this);
            stockProduct.ListAll();
        }
        #endregion

        #region ViewDetails()
        public void ViewDetails(int id)
        {
            var stockProduct = new MdlStockProduct(this);
            stockProduct.ViewDetails(id);
        }
        #endregion

        #region GenerateReport()
        public void GenerateReport(List<BllStockProduct> listOfRecords)
        {
            var reportStockProduct = new ReportStockProduct();
            reportStockProduct.GenerateReport(listOfRecords);
        }
        #endregion

        #endregion

    }
}
