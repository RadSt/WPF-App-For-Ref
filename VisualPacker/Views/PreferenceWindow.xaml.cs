using System;
using System.Threading;
using System.Windows;
using VisualPacker.Properties;

namespace VisualPacker.Views
{
    /// <summary>
    /// Interaction logic for PreferenceWindow.xaml
    /// </summary>
    public partial class PreferenceWindow
    {
        public PreferenceWindow()
        {
            InitializeComponent();
            SetLanguageDictionary();
            LoadSavedSettings();
        }
        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":
                    dict.Source = new Uri("..\\Resources\\StringResources.xaml",
                                  UriKind.Relative);
                    break;
                case "ru-RU":
                    dict.Source = new Uri("..\\Resources\\StringResources.ru-RU.xaml",
                                       UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Resources\\StringResources.xaml",
                                      UriKind.Relative);
                    break;
            }
            Resources.MergedDictionaries.Add(dict); //:TODO дублирование кода
        }
        private void LoadSavedSettings()
        {
            weightRestrictionCheckBox.IsChecked = Settings.Default.PreferenceWindowWeightRestrictionCheckBoxEnabled;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
