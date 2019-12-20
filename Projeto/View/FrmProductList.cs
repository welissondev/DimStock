﻿using System;
using Syncfusion.Windows.Forms.Tools;
using System.Collections.Generic;
using System.Windows.Forms;
using DimStock.Business;
using DimStock.Auxiliary;
using DimStock.Properties;

namespace DimStock.View
{
    public partial class FrmProductList : Form
    {

        #region Variables
        public int ProductId = 0;
        private AxlDataPagination dataPagination = new AxlDataPagination();
        #endregion

        #region Constructs
        
        public FrmProductList()
        {
            InitializeComponent();
            DefineColumnsInTheDataGrid();
            ConfigureDataPagination();

            LblTodayIsDay.Text = DateTime.Now.ToLongDateString();

            AxlDataGridViewLealt.DefaultLayoutDarkblue(GridProductList);
        }

        #endregion

        #region Form

        private void FrmProductList_Load(object sender, EventArgs e)
        {
            FetchData();
            ListPageSize();
            WindowState = FormWindowState.Maximized;
        }
      
        private void FrmProductList_Resize(object sender, EventArgs e)
        {
            try
            {
                // Centraliza ImgGifLoading no formulário
                ImgGifLoading.Left = this.Width / 2 - ImgGifLoading.Width / 2;
            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
        }
  
        #endregion

        #region Button

        private void BtnNew_Click(object sender, EventArgs e)
        {
            using (var frmProductRegister = new FrmProductRegister())
            {
                frmProductRegister.ShowDialog();
            }
        }
                
        private void BtnUpdateList_Click(object sender, EventArgs e)
        { 
            CallAllResets();
            dataPagination.CurrentPage = 1;
            dataPagination.OffSetValue = 0;
            TimerStartQuery();
        }
        
        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                var product = new BllProduct(dataPagination)
                {
                    QueryByCode = TxtQueryByCode.Text,
                    QueryBySize = TxtQueryBySize.Text,
                    QueryByReference = TxtQueryByReference.Text,
                    QueryByDescription = TxtQueryByDescription.Text
                };

                product.ListAll();

                var path = "DimStock.Report.RpvProduct.rdlc";
                var name = "Relatório de Produtos";
                var dataset = "DataSetProduct";

                product.GenerateReport(product.ListOfRecords);

                FrmReportView.ShowReport(path, name, true,
                new Dictionary<string, object>() {{dataset,
                product.ListOfRecords}});

            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
        }
        
        #endregion

        #region TextBox

        private void StartTheQuery_ForTheSearchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            dataPagination.OffSetValue = 0;
            dataPagination.CurrentPage = 1;
            TimerStartQuery();
        }

        #endregion

        #region ComboBox

        private void CboPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var itemSelected = CboPageSize.SelectedIndex;

                switch (itemSelected)
                {
                    case 0:
                        dataPagination.PageSize = 20;
                        break;
                    case 1:
                        dataPagination.PageSize = 30;
                        break;
                    case 2:
                        dataPagination.PageSize = 70;
                        break;
                    case 3:
                        dataPagination.PageSize = 100;
                        break;
                }

                dataPagination.OffSetValue = 0;
                dataPagination.CurrentPage = 1;
                TimerStartQuery();
            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
        }
        
        #endregion

        #region Timer

        private void TimerExecuteQuery_Tick(object sender, EventArgs e)
        {
            FetchData();
        }
        
        #endregion

        #region DataGridView

        private void GridProductList_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            //Remove o focus do controle datagriview
            e.PaintParts = DataGridViewPaintParts.All ^ DataGridViewPaintParts.Focus;
        }
        
        private void GridProductList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (GridProductList.Rows.Count != 0)
            {
                GetDataFromGridItems();
            }

            var columnName = GridProductList.Columns[e.ColumnIndex].Name;

            if (columnName == "buttonEdit" && e.RowIndex != -1)
            {
                ViewData();
            }
            else if (columnName == "buttonDelete" && e.RowIndex != -1)
            {
                Delete();
            }
            else if (columnName == "buttonReplicate")
            {
                ReplicateRecord();
            }
        }
        
        private void GridProductList_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                var columnIndexName = GridProductList.Columns[e.ColumnIndex].Name;
                if (columnIndexName == "buttonEdit" || columnIndexName == "buttonDelete" || columnIndexName == "buttonReplicate")
                {
                    GridProductList.Cursor = Cursors.Hand;
                }
                else
                {
                    GridProductList.Cursor = Cursors.Arrow;
                }
            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
        }
        
        private void GridProductList_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ViewData();
        }
       
        #endregion

        #region BadingNavigator

        private void NextPage_Click(object sender, EventArgs e)
        {
            if (dataPagination.CurrentPage < dataPagination.NumberOfPages)
            {
                dataPagination.CurrentPage += 1;
                dataPagination.OffSetValue += dataPagination.PageSize;
                TimerStartQuery(); 
            }
        }
        
        private void BackPage_Click(object sender, EventArgs e)
        {
            if (dataPagination.CurrentPage > 1)
            {
                dataPagination.CurrentPage -= 1;
                dataPagination.OffSetValue -= dataPagination.PageSize;
                TimerStartQuery(); 
            }
        }

        #endregion

        #region MethodsAxiliarys

        private void FetchData()
        {
            try
            {
                var product = new BllProduct(dataPagination)
                {
                    QueryByCode = TxtQueryByCode.Text,
                    QueryBySize = TxtQueryBySize.Text,
                    QueryByReference = TxtQueryByReference.Text,
                    QueryByDescription = TxtQueryByDescription.Text,
                };

                product.FetchData();

                GridProductList.Rows.Clear();

                for (int i = 0; i < product.ListOfRecords.Count; i++)
                {
                    GridProductList.Rows.Add(
                    product.ListOfRecords[i].Id,
                    product.ListOfRecords[i].Code,
                    product.ListOfRecords[i].Reference,
                    product.ListOfRecords[i].Size,
                    product.ListOfRecords[i].Supplier,
                    product.ListOfRecords[i].Description,
                    product.ListOfRecords[i].CostPrice,
                    product.ListOfRecords[i].SalePrice);
                }

                SetInBadingNavigator(dataPagination);
    
            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
            finally
            {
                TimerStopQuery();
            }
        }

        private void ViewData()
        {
            var frmProductRegister = new FrmProductRegister();

            try
            {
                if (ProductId > 0)
                {
                    var product = new BllProduct();

                    product.ViewData(ProductId);

                    frmProductRegister.Id = ProductId;
                    frmProductRegister.TxtCode.Text = product.Code.ToString();
                    frmProductRegister.TxtSize.Text = product.Size.ToString();
                    frmProductRegister.TxtReference.Text = product.Reference.ToString();
                    frmProductRegister.TxtSupplier.Text = product.Supplier;
                    frmProductRegister.TxtDescription.Text = product.Description;
                    frmProductRegister.TxtMinStock.Text = product.MinStock.ToString();
                    frmProductRegister.TxtMaxStock.Text = product.MaxStock.ToString();
                    frmProductRegister.TxtCostPrice.Text = product.CostPrice.ToString();
                    frmProductRegister.TxtSalePrice.Text = product.SalePrice.ToString();
                    frmProductRegister.TxtBarCode.Text = product.BarCode;
                    frmProductRegister.ReloadPhoto(product.PhotoName);
                    frmProductRegister.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Selecione um item para visualizar!", "SELECIONE",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
            finally
            {
                frmProductRegister.Dispose();
            }
        }
        
        private void ListPageSize()
        {

            var itens = new List<string>
            {
                "20 Registros",
                "30 Registros",
                "70 Registros",
                "100 Registros"
            };

            CboPageSize.DataSource = itens;
            CboPageSize.Text = "20 Registros";
        }
        
        private void Delete()
        {
            try
            {
                if (AxlLogin.PermissionToDelete == true)
                {
                    if (ProductId > 0)
                    {
                        if (MessageBox.Show("ATENÇÃO! Exluindo esse registro todas os dados como: Estoque, Entradas, Saidas e etc... " +
                        "também serão deletados, você confirma essa ação?", "IMPORTANTE", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            var product = new BllProduct();

                            if (product.Delete(ProductId) == true)
                            {
                                var photoDirectoryPath = BllProductPhotho.GetPeth() +
                                Convert.ToString(GridProductList.CurrentRow.Cells["NomeFotoColumn"].Value);

                                BllProductPhotho.DeleteFile(photoDirectoryPath);

                                MessageBox.Show(BllNotification.Message, "SUCESSO",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                                CallAllResets();
                            }
                            else
                            {
                                MessageBox.Show(BllNotification.Message, "ATENÇÃO",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para deletar produtos!", "NÃO PERMITIDO",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERRO",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ReplicateRecord()
        {

            var frmProductRegister = new FrmProductRegister();

            try
            {
                var product = new BllProduct();

                product.ViewData(ProductId);

                frmProductRegister.TxtCode.Text = product.Code.ToString();
                frmProductRegister.TxtSize.Text = product.Size.ToString();
                frmProductRegister.TxtReference.Text = product.Reference.ToString();
                frmProductRegister.TxtSupplier.Text = product.Supplier;
                frmProductRegister.TxtDescription.Text = product.Description;
                frmProductRegister.TxtMinStock.Text = product.MinStock.ToString();
                frmProductRegister.TxtMaxStock.Text = product.MaxStock.ToString();
                frmProductRegister.TxtCostPrice.Text = product.CostPrice.ToString();
                frmProductRegister.TxtSalePrice.Text = product.SalePrice.ToString();
                frmProductRegister.TxtBarCode.Text = product.BarCode;
                frmProductRegister.ReloadPhoto(product.PhotoName, true);
                frmProductRegister.ShowDialog();
            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
            finally
            {
                frmProductRegister.Dispose();
            }
        }
        
        private void GetDataFromGridItems()
        {
            try
            {
                ProductId = Convert.ToInt32(GridProductList.CurrentRow.Cells["id"].Value);
            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
        }
        
        private void ResetVariables()
        {
            ProductId = 0;
        }
        
        private void ResetControls()
        {
            try
            {
                foreach (Control ctl in Controls)
                {
                    if (ctl.GetType() == typeof(TextBoxExt))
                        ctl.Text = string.Empty;
                }

                TxtQueryByCode.Select();
            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
        }
        
        private void CallAllResets()
        {
            ResetVariables();
            ResetControls();
        }
       
        private void DefineColumnsInTheDataGrid()
        {
            try
            {
                var id = new DataGridViewTextBoxColumn();
                var code = new DataGridViewTextBoxColumn();
                var reference = new DataGridViewTextBoxColumn();
                var size = new DataGridViewTextBoxColumn();
                var supplier = new DataGridViewTextBoxColumn();
                var description = new DataGridViewTextBoxColumn();
                var costPrice = new DataGridViewTextBoxColumn();
                var salePrice = new DataGridViewTextBoxColumn();
                var photoName = new DataGridViewTextBoxColumn();
                var buttonEdit = new DataGridViewImageColumn();
                var buttonDelete = new DataGridViewImageColumn();
                var buttonReplicate = new DataGridViewImageColumn();

                var dataGrid = GridProductList;

                dataGrid.Columns.Add(id);
                dataGrid.Columns[0].Name = "id";
                dataGrid.Columns[0].HeaderText = "Id";
                dataGrid.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[0].Visible = false;
                dataGrid.Columns[0].ReadOnly = true;

                dataGrid.Columns.Add(code);
                dataGrid.Columns[1].Width = 80;
                dataGrid.Columns[1].Name = "code";
                dataGrid.Columns[1].HeaderText = "Código";
                dataGrid.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[1].ReadOnly = true;

                dataGrid.Columns.Add(reference);
                dataGrid.Columns[2].Width = 80;
                dataGrid.Columns[2].Name = "reference";
                dataGrid.Columns[2].HeaderText = "Referência";
                dataGrid.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[2].ReadOnly = true;

                dataGrid.Columns.Add(size);
                dataGrid.Columns[3].Name = "size";
                dataGrid.Columns[3].HeaderText = "Tamanho";
                dataGrid.Columns[3].Width = 80;
                dataGrid.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[3].DisplayIndex = 2;
                dataGrid.Columns[3].ReadOnly = true;
                dataGrid.Columns[3].Visible = true;

                dataGrid.Columns.Add(supplier);
                dataGrid.Columns[4].Width = 250;
                dataGrid.Columns[4].Name = "supplier";
                dataGrid.Columns[4].HeaderText = "Fornecedor";
                dataGrid.Columns[4].ReadOnly = true;
                dataGrid.Columns[4].Visible = true;
                dataGrid.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dataGrid.Columns.Add(description);
                dataGrid.Columns[5].Name = "description";
                dataGrid.Columns[5].HeaderText = "Descrição";
                dataGrid.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGrid.Columns[5].ReadOnly = true;

                dataGrid.Columns.Add(costPrice);
                dataGrid.Columns[6].Name = "costPrice";
                dataGrid.Columns[6].HeaderText = "Preço Custo";
                dataGrid.Columns[6].Width = 80;
                dataGrid.Columns[6].DefaultCellStyle.Format = "c2";
                dataGrid.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGrid.Columns[6].ReadOnly = true;

                dataGrid.Columns.Add(salePrice);
                dataGrid.Columns[7].Name = "salePrice";
                dataGrid.Columns[7].HeaderText = "Preço Venda";
                dataGrid.Columns[7].Width = 80;
                dataGrid.Columns[7].DefaultCellStyle.Format = "c2";
                dataGrid.Columns[7].ReadOnly = true;

                dataGrid.Columns.Add(photoName);
                dataGrid.Columns[8].Name = "photoName";
                dataGrid.Columns[8].HeaderText = "Nome da Foto";
                dataGrid.Columns[8].ReadOnly = true;
                dataGrid.Columns[8].Visible = false;

                dataGrid.Columns.Add(buttonReplicate);
                buttonReplicate.Image = Resources.Duplicar;
                buttonReplicate.ImageLayout = DataGridViewImageCellLayout.Normal;
                dataGrid.Columns[9].Name = "buttonReplicate";
                dataGrid.Columns[9].HeaderText = "";
                dataGrid.Columns[9].Width = 70;
                dataGrid.Columns[9].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[9].ReadOnly = true;

                dataGrid.Columns.Add(buttonEdit);
                dataGrid.Columns[10].Name = "buttonEdit";
                dataGrid.Columns[10].HeaderText = "";
                dataGrid.Columns[10].Width = 70;
                dataGrid.Columns[10].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[10].ReadOnly = true;
                buttonEdit.ImageLayout = DataGridViewImageCellLayout.Normal;
                buttonEdit.Image = Resources.Editar;

                dataGrid.Columns.Add(buttonDelete);
                dataGrid.Columns[11].Name = "buttonDelete";
                dataGrid.Columns[11].HeaderText = "";
                dataGrid.Columns[11].Width = 70;
                dataGrid.Columns[11].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.Columns[11].ReadOnly = true;
                buttonDelete.ImageLayout = DataGridViewImageCellLayout.Normal;
                buttonDelete.Image = Resources.Deletar;

            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
        }
        
        private void ConfigureDataPagination()
        {
            dataPagination.OffSetValue = 0;
            dataPagination.PageSize = 20;
            dataPagination.CurrentPage = 1;
        }
        
        private void SetInBadingNavigator(AxlDataPagination dataPagination)
        {
            if(dataPagination.RecordCount == 0)
                dataPagination.CurrentPage = 0;

            var legend = " Página " + dataPagination.CurrentPage + " de " + dataPagination.NumberOfPages;
            BindingPagination.Items[2].Text = legend;

            legend = " Total de " + dataPagination.RecordCount + " registro(s)";
            BindingPagination.Items[6].Text = legend;
        }
  
        private void TimerStartQuery()
        {
            ImgGifLoading.Visible = true;
            TimerExecuteQuery.Enabled = false;
            TimerExecuteQuery.Enabled = true;
        }
        
        public void TimerStopQuery()
        {
            ImgGifLoading.Visible = false;
            TimerExecuteQuery.Enabled = false;
        }
        
        #endregion

    }
}


