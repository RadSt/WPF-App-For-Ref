using System;
using System.Threading;
using System.Windows;
using VisualPacker.Properties;

namespace VisualPacker.Views
{
    /// <summary>
    /// Interaction logic for PreferenceWindow.xaml
    /// </summary>
    public partial class PreferenceWindow : Window
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
            this.Resources.MergedDictionaries.Add(dict);
        }
        private void LoadSavedSettings()
        {
            this.weightRestriction_CheckBox.IsChecked = Settings.Default.PreferenceWindow_weightRestrictionCheckBoxEnabled;
            //this.datePicker1 = (Date) Properties.Settings.Default.PreferenceWindow_FirstDateOfWeightRestriction;
        }
        private void calendar_SelectedDatesChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //Properties.Settings.Default.PreferenceWindow_weightRestrictionCheckBoxEnabled = this.weightRestriction_CheckBox.IsChecked;
            Settings.Default.Save();

        }
       
    }
}
