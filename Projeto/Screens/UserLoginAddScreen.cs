﻿using System;
using MetroFramework.Forms;
using DimStock.Views;
using DimStock.Presenters;
using System.Windows.Forms;
using MetroFramework;
using DimStock.AuxilyTools.AuxilyClasses;

namespace DimStock.Screens
{
    public partial class UserLoginAddScreen : IUserLoginAddView
    {
        private UserLoginAddPresenter presenter;
        private static MetroForm thisScreen;

        public int Id { get; set; }
        public string YourName { get => TextYourName.Text; set => TextYourName.Text = value; }
        public string Email { get => TextEmail.Text; set => TextEmail.Text = value; }
        public string Login { get => TextLogin.Text; set => TextLogin.Text = value; }
        public string AccessPassWord { get => TextAccessPassWord.Text; set => TextAccessPassWord.Text = value; }
        public string PassWordCheck { get => TextPassWordCheck.Text; set => TextPassWordCheck.Text = value; }
        public bool InsertAllowed { get => true; set => value = true; }
        public bool UpdateAllowed { get => true; set => value = true; }
        public bool DeleteAllowed { get => true; set => value = true; }
    }
}

namespace DimStock.Screens
{
    public partial class UserLoginAddScreen : MetroForm
    {
        public UserLoginAddScreen()
        {
            InitializeComponent();
            InitializePresenter();
            InitializeEvents();
            SetScreen();
        }

        private void ScreenClose(object sender, EventArgs e)
        {
            try
            {
                Close();
                thisScreen = null;
            }
            catch (Exception ex)
            {
                ExceptionNotifier.ShowMessage(ex);
            }
        }
        private void ScreenResize(object sender, EventArgs e)
        {
            try
            {
                Refresh();
            }
            catch (Exception ex)
            {
                ExceptionNotifier.ShowMessage(ex);
            }
        }

        private void InitializePresenter()
        {
            presenter = new UserLoginAddPresenter(this);
        }
        private void InitializeEvents()
        {
            try
            {
                ButtonClose.Click += new EventHandler(ScreenClose);
                ButtonSave.Click += new EventHandler(presenter.InsertUpdate);
                ButtonDelete.Click += new EventHandler(presenter.Delete);
                ButtonClearView.Click += new EventHandler(presenter.ClearView);
                Resize += new EventHandler(ScreenResize);
            }
            catch (Exception ex)
            {
                ExceptionNotifier.ShowMessage(ex);
            }
        }

        private void SetScreen()
        {
            thisScreen = this;
        }
        public static MetroForm GetScreen()
        {
            return thisScreen;
        }

        public static void SetDetails(IUserLoginAddView view)
        {
            var screen = new UserLoginAddScreen()
            {
                Id = view.Id,
                YourName = view.YourName,
                Email = view.Email,
                Login = view.Login,
                AccessPassWord = view.AccessPassWord,
                PassWordCheck = view.AccessPassWord,
                InsertAllowed = view.InsertAllowed,
                UpdateAllowed = view.UpdateAllowed,
                DeleteAllowed = view.DeleteAllowed,
                ShowInTaskbar = false,
                ControlBox = false,
                ShowIcon = false,
                ShadowType = MetroFormShadowType.DropShadow,
                Style = MetroColorStyle.Blue

            };

            var listingScreen = UserListingScreen.GetScreen();
            if (listingScreen != null)
                screen.Owner = listingScreen;

            screen.ShowDialog();
            screen.Dispose();
        }

        public static void ShowScreen(Form fatherScreen = null)
        {
            try
            {
                var screen = new UserLoginAddScreen();

                if (fatherScreen != null)
                {
                    screen.MdiParent = fatherScreen;
                    screen.ShowInTaskbar = false;
                    screen.ControlBox = false;
                    screen.Dock = DockStyle.Fill;
                    screen.Style = MetroColorStyle.White;
                    screen.Movable = false;
                    screen.ShadowType = MetroFormShadowType.DropShadow;
                    screen.Show();
                }
                else
                {
                    screen.ShowInTaskbar = false;
                    screen.ControlBox = false;
                    screen.ShowIcon = false;
                    screen.Style = MetroColorStyle.Blue;
                    screen.ShadowType = MetroFormShadowType.DropShadow;

                    var listingScreen = UserListingScreen.GetScreen();
                    if (listingScreen != null)
                        screen.Owner = listingScreen;

                    screen.ShowDialog();
                    screen.Dispose();
                }
            }
            catch (Exception ex)
            {
                ExceptionNotifier.ShowMessage(ex);
            }
        }
    }
}
