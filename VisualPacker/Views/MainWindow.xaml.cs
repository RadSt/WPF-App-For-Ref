using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using VisualPacker.Models;
using VisualPacker.ViewModels;
using VisualPacker.ViewModels.Helpers;

namespace VisualPacker.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Calculation calculation = new Calculation();
        private readonly ObservableCollection<Container> containers = new ObservableCollection<Container>();
        private readonly ObservableCollection<Vehicle> selectedVehicles = new ObservableCollection<Vehicle>();
        private readonly ObservableCollection<Vehicle> vehicles = new ObservableCollection<Vehicle>();
        private readonly List<Container> wasteContainers = new List<Container>();
        public MainWindow()
        {
            InitializeComponent();
            SetLanguageDictionary();
            //Загружаем список автомобилей   
            try
            {
                vehicles =
                    XmlHelper.LoadVehiclesFromXML(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) +
                                                  "\\vehicles.xml");
            }
            catch
            {
                MessageBox.Show("Ошибка открытия файла" + Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) +
                                "\\vehicles.xml");
            }

            dataGrid2.ItemsSource = selectedVehicles;
            vehicleTree.ItemsSource = vehicles;

            try
            {
                containers =
                    XmlHelper.LoadItemFromXML(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\items.xml");
            }
            catch
            {
                MessageBox.Show("Ошибка открытия файла " +
                                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                "\\items.xml");
            }
            dataGrid1.ItemsSource = containers;
            WriteLog("Вход в программу");
        }
        private void WriteLog(string logString)
        {
            if (Directory.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\logs") == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\logs");
            }

            var logFile =
                new StreamWriter(
                    new FileStream(
                        Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\logs\\" + DateTime.Today.Year +
                        DateTime.Today.Month + DateTime.Today.Day + ".log",
                        FileMode.Append));
            logFile.WriteLine(DateTime.Now + ": user " + Environment.UserName + ":" + logString);
            logFile.Flush();
            logFile.Close();//:TODO Дублирование кода
        }
        private void SaveProtocolToFile()
        {
            if (Directory.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData") ==
                false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData");
            }

            var destination = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData\\" +
                              DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + " " +
                              DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + " " +
                              Environment.UserName + " protocol";

            var file = new StreamWriter(new FileStream(destination + ".txt",
                FileMode.Append));
            file.WriteLine(textBox.Text);
            file.Flush();
            file.Close(); //:TODO Дублирование кода
        }
        private void SetLanguageDictionary()
        {
            var dict = new ResourceDictionary();
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
            Resources.MergedDictionaries.Add(dict); //TODO дублирование кода
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Report_Click(object sender, RoutedEventArgs e)
        {
            var winReport = new LoadScheme(selectedVehicles);

            winReport.Show();
        }
        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            var winPreferences = new PreferenceWindow();
            winPreferences.ShowDialog();
        }
        private void ButtonView3D_Click(object sender, RoutedEventArgs e)
        {
            var winView3D = new View3D(selectedVehicles);

            winView3D.Show();
        }
        private void AboutProgramm_Click(object sender, RoutedEventArgs e)
        {
            var winAbout = new AboutWindow();

            winAbout.Show();
        }
        private void ButtonView2D_Click(object sender, RoutedEventArgs e)
        {
            var winView2D = new View2D(selectedVehicles);

            winView2D.Show();
        }
        private void ButtonContainerList_Click(object sender, RoutedEventArgs e)
        {
            var winContainerList = new ContainerList
            {
                vehicles = selectedVehicles.ToList(),
                containers = wasteContainers.ToList()
            };
            winContainerList.ShowContainers();
            winContainerList.Show();
        }
        private void ButtonCalculate_Click(object sender, RoutedEventArgs e)
        {
            const int maxTonnage = 0;
            dataGrid1.CommitEdit();
            if (containers.Count == 0)
            {
                MessageBox.Show("Нет груза для расчета.");
            }
            else if (selectedVehicles.Count == 0) MessageBox.Show("Не выбран автомобиль.");
            else
            {
                var wasteContainersList = calculation.CalculateLoadScheme(containers.Where(s => s.IsChecked).ToList(),
                    selectedVehicles, textBox, maxTonnage);
                UpdateCheckProperty(wasteContainersList);
            }
            dataGrid1.ItemsSource = null;
            dataGrid1.ItemsSource = containers;
            WriteLog("Выполнен расчет схемы загрузки контейнеров");

            SaveProtocolToFile();
        }
        private void selectAllContainers_Click(object sender, RoutedEventArgs e)
        {
            foreach (var c in containers)
            {
                c.IsChecked = true;
            }
            dataGrid1.ItemsSource = null;
            dataGrid1.ItemsSource = containers;
        }
        private void deselectAllContainers_Click(object sender, RoutedEventArgs e)
        {
            foreach (var c in containers)
            {
                c.IsChecked = false;
            }
            dataGrid1.ItemsSource = null;
            dataGrid1.ItemsSource = containers;
        }
        private void UpdateCheckProperty(List<Container> containersList)
        {
            foreach (var c in containersList)
            {
                var list = containers.Where(s => s.Name == c.Name).ToList();
                if (list.Any())
                {
                    list[0].IsChecked = false;
                }
                else
                {
                    MessageBox.Show("Ошибка при изменении поля IsCheked");
                }
            }
        }
        private void MenuDeleteContainer_Click(object sender, RoutedEventArgs e)
        {
            containers.Remove((Container) dataGrid1.SelectedItem);
        }
        private void MenuMoveContainers_Click(object sender, RoutedEventArgs e)
        {
            dataGrid1.CommitEdit();
            var list = new List<string>();
            foreach (var c in containers)
            {
                if (c.IsChecked) list.Add(c.Name);
            }
            textBox.Text = "Протокол переноса контейнеров...";
            SendRequestToService(list, "Новый");
            WriteLog("Перенос контейнеров в новое ГО");
        }
        private void MenuMoveContainersToExistingLoad_Click(object sender, RoutedEventArgs e)
        {
            dataGrid1.CommitEdit();
            var list = new List<string>();
            foreach (var c in containers)
            {
                if (c.IsChecked) list.Add(c.Name);
            }
            var inputDialog = new InputDialog();
            if (inputDialog.ShowDialog() == true)
            {
                var loadId = inputDialog.Answer;
                textBox.Text = "Протокол переноса контейнеров...";
                SendRequestToService(list, loadId);
            }
            WriteLog("Перенос контейнеров в существующее ГО");
        }
        private void SendRequestToService(List<string> list, string loadId)
        {
            var reqString =
                "http://localhost/ILSIntegrationServices/ShipmentContainerTransferResource/MoveContainers?containerId=" +
                string.Join(",", list.ToArray()) + "&load=" + loadId;
            var r = WebRequest.Create(reqString) as HttpWebRequest;
            r.Method = "GET";
            r.Accept = "application/json";
            r.ContentType = "application/json";
            r.Headers.Add("UserName:user199");
            try
            {
                var response = r.GetResponse() as HttpWebResponse;
                var responseBody = "";
                using (var rspStm = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(rspStm))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
                textBox.Text = textBox.Text + "\r\n";
                textBox.Text = textBox.Text + "Статус выполнения запроса: " + response.StatusCode;
                textBox.Text = textBox.Text + "\r\n";
                textBox.Text = textBox.Text + responseBody;
            }
            catch (WebException ex)
            {
                textBox.Text = textBox.Text + "Ошибка переноса контейнеров: " + ex.Message;
                textBox.Text = textBox.Text + "\n";
                textBox.Text = textBox.Text + "Текст запроса: " + reqString;
                textBox.Text = textBox.Text + "\r\n";
                var reader = new StreamReader(ex.Response.GetResponseStream());
                textBox.Text = textBox.Text + reader.ReadToEnd();
            }
        }
        private void MenuVehicle_Click(object sender, RoutedEventArgs e)
        {
            selectedVehicles.Remove((Vehicle) dataGrid2.SelectedItem);
        }
        private void vehicleTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var clonableVehicle = (Vehicle) vehicleTree.SelectedItem;
            selectedVehicles.Add(clonableVehicle.Clone() as Vehicle);
        }
    }
}