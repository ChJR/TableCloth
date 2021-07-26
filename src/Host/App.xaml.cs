﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml;
using TableCloth.Helpers;
using TableCloth.Models.Catalog;
using TableCloth.Resources;

namespace Host
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            using WebClient webClient = new();
            CatalogDocument catalog = null;

            try
            {
                using Stream catalogStream = webClient.OpenRead(StringResources.CatalogUrl);
                catalog = XmlHelpers.DeserializeFromXml<CatalogDocument>(catalogStream);

                if (catalog == null)
                {
                    throw new XmlException(StringResources.HostError_CatalogDeserilizationFailure);
                }

                Current.InitCatalogDocument(catalog);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(StringResources.HostError_CatalogLoadFailure(ex), StringResources.TitleText_Error,
                    MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                Current.Shutdown(0);
                return;
            }

            string[] targetSites = e.Args.ToArray();

            if (!targetSites.Any())
            {
                _ = MessageBox.Show(StringResources.Host_No_Targets, StringResources.TitleText_Error,
                    MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                Current.Shutdown(0);
                return;
            }

            Current.InitInstallSites(targetSites);
        }
    }
}