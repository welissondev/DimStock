﻿using System;
using DimStock.Properties;
using DimStock.Auxiliarys;
using System.Collections.Generic;

namespace DimStock.Business
{
    public class AppConfig
    {
        public void TransferDataBaseToMainDirectory()
        {
            var sourcePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + 
            @"Padrao\dimstock-database.mdb";
            
            var destPath = Settings.Default.MainAppDirectory + 
            @"\dimstock-database.mdb";

            var dataBase = new AxlFile();

            if (dataBase.CheckIfExists(destPath) == false)
                dataBase.CopyFromDirectory(sourcePath, destPath);
        }

        public void TransferCompanyLogoToMainDirectory(string sourcePath)
        {
            var destPath = GetMainAppDirectory() + @"\CompanyLogo\CompanyLogo.jpg";

            var logoImage = new AxlFile();

            if (logoImage.CheckIfExists(destPath) == false)
                logoImage.CopyFromDirectory(sourcePath, destPath);
        }

        public void CreateFoldersInTheMainDirectory()
        {
            var directory = new AxlDirectory();

            var listFolders = new List<string>()
            {
                "DataBaseBackUp",
                "ProductPhotos",
                "CompanyLogo"
            };

            var rootDirectory = GetMainAppDirectory();

            directory.CreateFoldersList(
            rootDirectory, listFolders);
        }

        public static bool GetAppSettingsState()
        {
            return Settings.Default.AppSettingsState;
        }

        public static string GetMainAppDirectory()
        {
            return Settings.Default.MainAppDirectory;
        }

        public void SaveAsMainDirectory(string path)
        {
            Settings.Default.MainAppDirectory = path;
            Settings.Default.Save();
        }

        public static void FinalizeSettings()
        {
            Settings.Default.AppSettingsState = true;
            Settings.Default.Save();
        }
    }
}
