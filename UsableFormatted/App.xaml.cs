using UsableFormatted.Controller;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Text.RegularExpressions;

namespace UsableFormatted
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            FileOperations.CheckAppParameters(e.Args);
        }

        public void ChangeAppLanguage(string language)
        {
            this.Resources.MergedDictionaries.Clear();

            var styleDict = new ResourceDictionary();
            styleDict.Source = new Uri("Resources\\Styles.xaml", UriKind.Relative);
            this.Resources.MergedDictionaries.Add(styleDict);

            ResourceDictionary dict = new ResourceDictionary();
            string path;
            switch (language)
            {
                case "lv":
                    path = "Resources\\StringResources.lv.xaml";
                    break;

                case "en":
                default:
                    path = "Resources\\StringResources.en.xaml";
                    break;
            }
            dict.Source = new Uri(path, UriKind.Relative);
            this.Resources.MergedDictionaries.Add(dict);

        }
    }
}
