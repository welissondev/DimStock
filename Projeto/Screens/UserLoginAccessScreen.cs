﻿using System.Windows.Forms;
using DimStock.Views;
using DimStock.Presenters;
using System;
using DimStock.AuxilyTools.AuxilyClasses;
using System.Drawing;

namespace DimStock.Screens
{
    /// <summary>
    /// Representa a tela de acesso do usuário
    /// </summary>
    public partial class UserLoginAccessScreen : Form, IUserLoginAccessView
    {
        private UserLoginAccessPresenter presenter;

        public int Id { get; set; }
        public string YourName { get; set; }
        public string Email { get; set; }
        public string Login { get => TextLogin.Text; set => TextLogin.Text = value; }
        public string AccessPassWord { get => TextPassWord.Text; set => TextPassWord.Text = value; }
        public string PassWordCheck { get; set; }
        public bool InsertAllowed { get; set; }
        public bool UpdateAllowed { get; set; }
        public bool DeleteAllowed { get; set; }
        public bool AcessStatus { get; set;}
    }
}

namespace DimStock.Screens
{
    public partial class UserLoginAccessScreen
    {
        public UserLoginAccessScreen()
        {
            InitializeComponent();
            InitializePresenter();
            InitializeEvents();
            MaximizeScreen();
        }

        private void ScreenLoad(object sender, EventArgs e)
        {
        }
        private void ScreenClose(object sender, EventArgs e)
        {
            try
            {
                var homeScreen = HomeScreen.GetScreen();
                if (homeScreen != null)
                    homeScreen.Close();

                Close();
            }
            catch (Exception ex)
            {
                ExceptionNotifier.ShowMessage(ex);
            }
        }
        private void ShowRelatedScreen(object sender, EventArgs e)
        {
            try
            {
                if (sender.Equals(ButtonEnter))
                {
                    if (AcessStatus == true)
                    {
                        var homeScreen = HomeScreen.GetScreen();
                        if (homeScreen != null)
                            homeScreen.Show();

                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionNotifier.ShowMessage(ex);
            }
        }

        private void MaximizeScreen()
        {
            try
            {
                Width = Screen.PrimaryScreen.Bounds.Width;
                Height = Screen.PrimaryScreen.Bounds.Height - 40;
            }
            catch (Exception ex)
            {
                ExceptionNotifier.ShowMessage(ex);
            }
        }

        private void InitializePresenter()
        {
            try
            {
                presenter = new UserLoginAccessPresenter(this);
            }
            catch (Exception ex)
            {
                ExceptionNotifier.ShowMessage(ex);
            }
        }
        private void InitializeEvents()
        {
            try
            {
                Load += new EventHandler(ScreenLoad);
                ButtonEnter.Click += new EventHandler(presenter.ResquestAccess);
                ButtonEnter.Click += new EventHandler(ShowRelatedScreen);
                ButtonExit.Click += new EventHandler(ScreenClose);

            }
            catch (Exception ex)
            {
                ExceptionNotifier.ShowMessage(ex);
            }
        }

        public static void ShowScreen()
        {
            try
            {
                var homeScreen = HomeScreen.GetScreen();
                if (homeScreen != null)
                    homeScreen.Hide();

                var screen = new UserLoginAccessScreen();
                screen.Show();
            }
            catch (Exception ex)
            {
                ExceptionNotifier.ShowMessage(ex);
            }
        }
    }
}
