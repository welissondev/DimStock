﻿using DimStock.Auxiliarys;
using DimStock.Business;
using DimStock.Properties;
using Syncfusion.Windows.Forms.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DimStock.UserForms
{
    public partial class ProductRegistrationForm : Form
    {
        #region Get & Set

        public int Id { get; set; }

        public int CategoryId { get; set; }

        #endregion

        #region Variables

        private ProductPhoto productPhoto = new ProductPhoto();

        private AxlDataPagination dataPagination = new AxlDataPagination();

        #endregion 

        #region Constructs

        public ProductRegistrationForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Button

        private void Save_Click(object sender, EventArgs e)
        {
            var user = new User();
            user.ViewDetails(AxlLogin.Id);

            if (Id == 0)
            {
                if (user.PermissionToRegister == false)
                {
                    MessageBox.Show("Você não tem permissão para cadastrar!", "NÃO PERMITIDO",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Register();
            }

            if (Id > 0)
            {
                if (user.PermissionToEdit == false)
                {
                    MessageBox.Show("Você não tem permissão para editar!", "NÃO PERMITIDO",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Edit();
            };
        }

        private void ClearFields_Click_1(object sender, EventArgs e)
        {
            CallAllResets();
        }

        #endregion

        #region ListBox

        private void ListviewCategory_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                CategoryId = Convert.ToInt32(ListviewCategory.
                SelectedItems[0].SubItems[0].Text);

                BoxProductCategoryList.Text = ListviewCategory.
                SelectedItems[0].SubItems[1].Text;

                ListviewCategory.Visible = false;
            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
        }

        #endregion

        #region ComboBox

        private void BoxProductCategoryList_Click(object sender, EventArgs e)
        {
            if (ListviewCategory.Visible == false)
            {
                if (BoxProductCategoryList.Text == string.Empty)
                {
                    ListviewCategory.Visible = true;
                    BoxProductCategoryList.DroppedDown = false;
                    StartSearchTimer();
                }
            }
            else
            {
                ListviewCategory.Visible = false;
            }
        }

        private void BoxProductCategoryList_KeyPress(object sender, KeyPressEventArgs e)
        {
            StartSearchTimer();
        }

        #endregion

        #region LabelLink

        private void AddNewProductCategory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var categoryForm = new ProductCategoryRegistrationForm()
            {
                ShowInTaskbar = false,
                MaximizeBox = false,
                MinimizeBox = false
            };

            categoryForm.ShowDialog();
        }

        #endregion

        #region PictureBox

        private void ImageProduct_Click(object sender, EventArgs e)
        {
            try
            {
                if (UploadPhoto() == false)
                {
                    if (productPhoto.CheckIfExtits(productPhoto.GetDirectoryPeth() +
                        ImageProduct.IndentificationPhotoNumber).Equals(false))
                    {
                        ImageProduct.IndentificationPhotoNumber = "";
                        ImageProduct.SelectedDirectory = "";
                        ImageProduct.Image = Resources.FotoNothing;
                    }
                }
            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
        }

        #endregion

        #region Timer

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            FillBoxCategory();
        }

        #endregion

        #region MethodsAuxiliarys

        private void Register()
        {
            try
            {
                if (ValidateData() == false)
                {
                    return;
                }

                var product = new Product
                {
                    Code = Convert.ToInt32(ProductCode.Text),
                    Size = Convert.ToInt32(ProductSize.Text),
                    Reference = Convert.ToInt32(ProductReference.Text),
                    Description = Description.Text,
                    CostPrice = Convert.ToDouble(CostPrice.DecimalValue),
                    SalePrice = Convert.ToDouble(SalePrice.DecimalValue),
                    BarCode = BarCode.Text,
                    PhotoPath = ImageProduct.IndentificationPhotoNumber
                };

                product.Category.Id = CategoryId;

                if (product.Register() == false)
                {
                    MessageBox.Show(AxlMessageNotifier.Message, "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }

                productPhoto.CopyFromDirectory(ImageProduct.SelectedDirectory,
                productPhoto.GetDirectoryPeth() + product.PhotoPath);

                MessageBox.Show(AxlMessageNotifier.Message, "SUCESSO",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

                CallAllResets();
            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
        }

        private void Edit()
        {
            try
            {
                if (ValidateData() == false)
                {
                    return;
                }

                var product = new Product()
                {
                    Code = Convert.ToInt32(ProductCode.Text),
                    Size = Convert.ToInt32(ProductSize.Text),
                    Reference = Convert.ToInt32(ProductReference.Text),
                    Description = Description.Text,
                    CostPrice = Convert.ToDouble(CostPrice.DecimalValue),
                    SalePrice = Convert.ToDouble(SalePrice.DecimalValue),
                    BarCode = BarCode.Text,
                    PhotoPath = ImageProduct.IndentificationPhotoNumber,
                };
                product.Category.Id  = CategoryId;

                if (product.Edit(Id) == false)
                {
                    MessageBox.Show(AxlMessageNotifier.Message, "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }

                var photoPath = productPhoto.GetDirectoryPeth()
                + product.PhotoPath;

                //Apaga a foto atual do diretório, caso a foto do produto
                //seja alterada
                if (productPhoto.CheckIfExtits(photoPath) == false)
                    productPhoto.DeleteFromDirectory(
                    ImageProduct.PathOfLastSelectedPhoto);

                productPhoto.CopyFromDirectory(ImageProduct.SelectedDirectory,
                photoPath);

                MessageBox.Show(AxlMessageNotifier.Message, "SUCESSO",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
        }

        private bool ValidateData()
        {

            if (ProductCode.Text == string.Empty)
            {
                MessageBox.Show("Campo código produto é obrigatório!",
                "OBRIGATÓRIO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                ProductCode.Select();

                return false;
            }

            if (ProductSize.Text == string.Empty)
            {
                MessageBox.Show("Campo tamanho é obrigatório!", "OBRIGATÓRIO",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                ProductSize.Select();

                return false;
            }

            if (ProductReference.Text == string.Empty)
            {
                MessageBox.Show("Campo código estampa é obrigatório!", "OBRIGATÓRIO",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                ProductReference.Select();

                return false;
            }

            if (CategoryId == 0)
            {
                MessageBox.Show("Informe a categoria do produto!", "OBRIGATÓRIO",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                BoxProductCategoryList.Select();

                return false;
            }

            if (Description.Text == string.Empty)
            {
                MessageBox.Show("Campo descrição é obrigatório!", "OBRIGATÓRIO",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                Description.Select();

                return false;
            }

            return true;
        }

        private bool UploadPhoto()
        {
            var picture = new AxlFile();
            var uploadState = false;

            picture.SelectPath();

            if (picture.DirectoryPath != string.Empty)
            {
                using (var fileStream = new FileStream(picture.DirectoryPath,
                FileMode.Open, FileAccess.Read))
                {
                    ImageProduct.Image = Image.FromStream(fileStream);
                    ImageProduct.IndentificationPhotoNumber = productPhoto.GetNumberId() + ".jpg";
                    ImageProduct.SelectedDirectory = picture.DirectoryPath;

                    uploadState = true;
                }
            }

            return uploadState;
        }

        public void ReloadPhoto(string photoIdNumber, bool newIdNumber = false)
        {
            var photoPath = productPhoto.GetDirectoryPeth() + photoIdNumber;

            if (productPhoto.CheckIfExtits(photoPath) == false)
            {
                ImageProduct.Image = Resources.FotoNothing;
                return;
            }

            using (var fileStream = new FileStream(photoPath, FileMode.Open, FileAccess.Read))
            {
                ImageProduct.Image = Image.FromStream(fileStream);
                ImageProduct.SelectedDirectory = photoPath;
                ImageProduct.PathOfLastSelectedPhoto = photoPath;
                ImageProduct.IndentificationPhotoNumber = photoIdNumber;

                if (newIdNumber == true)
                {
                    photoIdNumber = productPhoto.GetNumberId() + ".jpg";
                    ImageProduct.IndentificationPhotoNumber = photoIdNumber;
                }

            }
        }

        private void ResetControls()
        {
            try
            {
                foreach (Control ctl in Controls)
                {
                    if (ctl.GetType() == typeof(TextBoxExt))
                    {
                        ctl.Text = string.Empty;
                    }
                    else if (ctl.GetType() == typeof(CurrencyTextBox))
                    {
                        ctl.Text = string.Empty;
                    }
                    else if (ctl.GetType() == typeof(IntegerTextBox))
                    {
                        ctl.Text = string.Empty;
                    }
                }

                ImageProduct.Image = Resources.FotoNothing;
                ImageProduct.IndentificationPhotoNumber = string.Empty;
                ImageProduct.SelectedDirectory = string.Empty;
                ImageProduct.PathOfLastSelectedPhoto = string.Empty;

                ProductCode.Select();
            }
            catch (Exception ex)
            {
                AxlException.Message.Show(ex);
            }
        }

        private void ResetVariables()
        {
            Id = 0;
            CategoryId = 0;
        }

        private void CallAllResets()
        {
            ResetControls();
            ResetVariables();
        }

        public void FillBoxCategory()
        {
            try
            {
                var category = new ProductCategory(dataPagination)
                {
                    Description = BoxProductCategoryList.Text
                };
                category.SearchData();

                ListviewCategory.Items.Clear();
                ListviewCategory.Height = 250;
                ListviewCategory.Visible = true;

                foreach (var item in category.ListOfRecords)
                {
                    ListviewCategory.Items.Add(new ListViewItem(
                    new string[] { item.Id.ToString(),
                    item.Description}));
                }

                PauseSearchTimer();
            }
            catch (Exception ex)
            {
                PauseSearchTimer();
                AxlException.Message.Show(ex);
            }
        }

        private void StartSearchTimer()
        {
            SearchTimer.Enabled = false;
            SearchTimer.Enabled = true;
        }

        private void PauseSearchTimer()
        {
            SearchTimer.Enabled = false;
        }

        #endregion
    }
}

