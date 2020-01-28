﻿using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DimStock.UserForms
{
    public partial class ReportViewForm : Form
    {
        #region Constructors
        private ReportViewForm(string path, string reportName, bool isEmbeddedResource, Dictionary<string,
        object> dataSources, Dictionary<string, object> reportParameters = null)
        {

            InitializeComponent();

            RpvReportViewer.LocalReport.DisplayName = reportName;

            if (isEmbeddedResource)
            {
                RpvReportViewer.LocalReport.ReportEmbeddedResource = path;
            }
            else
            {
                RpvReportViewer.LocalReport.ReportPath = path;
            }

            foreach (var dataSource in dataSources)
            {
                var reportDataSource = new ReportDataSource(dataSource.Key, dataSource.Value);
                this.RpvReportViewer.LocalReport.DataSources.Add(reportDataSource);
            }

            if (reportParameters != null)
            {
                var reportParameterCollection = new List<ReportParameter>();

                foreach (var parameter in reportParameters)
                {
                    var reportParameter = new ReportParameter(parameter.Key, parameter.Value.ToString());
                    reportParameterCollection.Add(reportParameter);
                }

                RpvReportViewer.LocalReport.SetParameters(reportParameterCollection);
            }
        }
        #endregion

        #region UserForm

        private void FrmReportView_Load(object sender, EventArgs e)
        {

            RpvReportViewer.RefreshReport();
        }

        #endregion

        #region Methods

        public static void ShowReport(string path, string reportName, bool isEmbeddedResource,
        Dictionary<string, object> dataSources, Dictionary<string, object> reportParameters = null)
        {
            var form = new ReportViewForm(path, reportName, isEmbeddedResource,
            dataSources, reportParameters)
            {
                ShowInTaskbar = false
            };

            form.Show();
        }

        #endregion
    }
}
