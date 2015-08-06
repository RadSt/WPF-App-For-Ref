using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisualPacker.Models;
using VisualPacker.ViewModels;
using VisualPacker.ViewModels.Helpers;

namespace VisualPacker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
     
    public partial class MainWindow
    {
        ObservableCollection<Vehicle> vehicles = new ObservableCollection<Vehicle>();
        ObservableCollection<Vehicle> selectedVehicles = new ObservableCollection<Vehicle>();
        ObservableCollection<Container> containers = new ObservableCollection<Container>();
        List<Container> wasteContainers = new List<Container>();
        [STAThread]
        public static void CreateResult(List<RowBlock> rowBlocks,Vehicle vehicle)
        {
            TextBlock txt = new TextBlock();
            List<String> notIncluded=new List<String>();
            double volume = 0;
            double weight = 0;
            foreach (RowBlock r in rowBlocks)
            {
                foreach (VerticalBlock v in r.Blocks)
                {
                    foreach (Container c in v.Blocks)
                    {
                        if (r.Included)
                        {
                            volume = volume + c.Volume;
                            weight = weight + c.Mass;
                        }
                        else
                        {
                            notIncluded.Add(c.Name);
                        }
                    }
                  
                }
              
            }
            double volUsage=100*volume/(vehicle.Volume());
            String notIncludedString;
            if (notIncluded.Count==0)  notIncludedString="Все контейнеры вмещаются в машину.";
            else notIncludedString="Не поместились следующие контейнеры"+notIncluded;
            //MessageBox.Show("Заполнение машины - "+volUsage.ToString()+"%. Вес груза - "+weight.ToString()+" кг. " + notIncludedString);
        }
        public MainWindow()
        {
            InitializeComponent();
            SetLanguageDictionary();
            //Загружаем список автомобилей   
            try { vehicles = XmlHelper.LoadVehiclesFromXML(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\vehicles.xml"); }
            catch { MessageBox.Show("Ошибка открытия файла" + Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\vehicles.xml"); }

            dataGrid2.ItemsSource = selectedVehicles;
            vehicleTree.ItemsSource = vehicles;

            try { containers = XmlHelper.LoadItemFromXML(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\items.xml"); }
            catch { MessageBox.Show("Ошибка открытия файла " + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\items.xml"); }


           dataGrid1.ItemsSource = containers;
           //MessageBox.Show(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\logs\\" + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day + ".log");
           WriteLog("Вход в программу");
        }
        public void WriteLog(string logString)
        {
            if (Directory.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\logs") == false)
            { Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\logs"); }
            
            StreamWriter file = new StreamWriter(new FileStream(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\logs\\" + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day + ".log",
            FileMode.Append));
            file.WriteLine(DateTime.Now + ": user " + Environment.UserName +":"+ logString);
            file.Flush();
            file.Close();
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
            Resources.MergedDictionaries.Add(dict);
        } 
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void report_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow winReport = new ReportWindow(selectedVehicles);
            
            winReport.Show(); 
        }
        private void preferences_Click(object sender, RoutedEventArgs e)
        {
            PreferenceWindow winPreferences = new PreferenceWindow();
            winPreferences.ShowDialog();
        }

        private void Button_view3d_Click(object sender, RoutedEventArgs e)
        {
            View3D winView3D = new View3D(selectedVehicles);

            winView3D.Show();
        }

        private void about_programm_Click(object sender, RoutedEventArgs e)
        {
            aboutWindow winAbout = new aboutWindow();

            winAbout.Show();
        }
        private void Button_view2d_Click(object sender, RoutedEventArgs e)
        {
            View2D winView2d = new View2D(selectedVehicles);

            winView2d.Show();
        }
        private void Button_ContainerList_Click(object sender, RoutedEventArgs e)
        {
            ContainerList winContainerList = new ContainerList();
            winContainerList.vehicles=selectedVehicles.ToList();
            winContainerList.containers = wasteContainers.ToList();
            winContainerList.ShowContainers();
            winContainerList.Show();
        }
        private void Button_Calculate_Click(object sender, RoutedEventArgs e)
        {
            int MaxTonnage = 0;
            dataGrid1.CommitEdit();
            if (containers.Count == 0)
            {
                MessageBox.Show("Нет груза для расчета.");
            }
            else if (selectedVehicles.Count == 0) MessageBox.Show("Не выбран автомобиль.");
            else
            {
              List<Container> wasteContainersList = Calculation.CalculateLoadScheme(containers.Where(s => s.IsChecked).ToList(), selectedVehicles, textBox, MaxTonnage);
              UpdateCheckProperty(wasteContainersList);
            }
            dataGrid1.ItemsSource = null;
            dataGrid1.ItemsSource = containers;
            WriteLog("Выполнен расчет схемы загрузки контейнеров");

            SaveProtocolToFile();
        }
        public  void SaveProtocolToFile()
        {
            if (Directory.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData") == false)
            { Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData"); }

            string destination = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData\\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + " " +
                DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + " " + Environment.UserName + " protocol";

            StreamWriter file = new StreamWriter(new FileStream(destination +".txt",
            FileMode.Append));

            file.WriteLine(this.textBox.Text);
            file.Flush();
            file.Close();


        }

        private void selectAllContainers_Click(object sender, RoutedEventArgs e)
        {
            foreach (Container c in containers)
            {
                c.IsChecked = true;
            }
           dataGrid1.ItemsSource = null;
            dataGrid1.ItemsSource = containers;
        }
        private void deselectAllContainers_Click(object sender, RoutedEventArgs e)
        {
            foreach (Container c in containers)
            {
                c.IsChecked = false;
            }
            dataGrid1.ItemsSource = null;
            dataGrid1.ItemsSource = containers;
        }
        private void UpdateCheckProperty(List<Container> wasteContainers)
    {
        foreach (Container c in wasteContainers)
        {
            List<Container> list =containers.Where(s => s.Name == c.Name).ToList();
            if (list.Any())
            {
                list[0].IsChecked = false;
            }
            else { MessageBox.Show("Ошибка при изменении поля IsCheked"); }
        }
    }

        private void MenuDeleteContainer_Click(object sender, RoutedEventArgs e)
        { 
            containers.Remove((Container)dataGrid1.SelectedItem); 
        }
        private void MenuMoveContainers_Click(object sender, RoutedEventArgs e)
        {
            dataGrid1.CommitEdit();
            List<string> list = new List<string>();
            foreach (Container c in containers)
            {
                if (c.IsChecked) list.Add(c.Name);
            }
            textBox.Text = "Протокол переноса контейнеров...";
            SendRequestToService(list,"Новый");
            WriteLog("Перенос контейнеров в новое ГО");
        }
        private void MenuMoveContainersToExistingLoad_Click(object sender, RoutedEventArgs e)
            {
                dataGrid1.CommitEdit();
            List<string> list = new List<string>();
            foreach (Container c in containers)
            {   
                if (c.IsChecked) list.Add(c.Name);
            }
            string loadId="";
            InputDialog inputDialog = new InputDialog();
            if (inputDialog.ShowDialog() == true)
            {
                loadId = inputDialog.Answer;
                textBox.Text = "Протокол переноса контейнеров...";
                SendRequestToService(list, loadId);
            }
            WriteLog("Перенос контейнеров в существующее ГО");
        } 
        private void  SendRequestToService(List<string> list,string loadId)
        {
            string reqString = "http://localhost/ILSIntegrationServices/ShipmentContainerTransferResource/MoveContainers?containerId=" + string.Join(",", list.ToArray()) + "&load=" + loadId;
            HttpWebRequest r = WebRequest.Create(reqString) as HttpWebRequest;
            r.Method = "GET";
            r.Accept = "application/json";
            r.ContentType = "application/json";
            r.Headers.Add("UserName:user199");
            try
            {
                HttpWebResponse response = r.GetResponse() as HttpWebResponse;
                string responseBody = "";
                using (Stream rspStm = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(rspStm))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
                textBox.Text = textBox.Text + "\r\n";
                textBox.Text = textBox.Text +"Статус выполнения запроса: " + response.StatusCode;
                textBox.Text = textBox.Text + "\r\n";
                textBox.Text = textBox.Text + responseBody;
            }
            catch (WebException ex)
            {
                textBox.Text = textBox.Text + "Ошибка переноса контейнеров: " + ex.Message;
                textBox.Text = textBox.Text + "\n";
                textBox.Text = textBox.Text + "Текст запроса: " + reqString;
                textBox.Text = textBox.Text + "\r\n";
                StreamReader reader = new StreamReader(ex.Response.GetResponseStream());
                textBox.Text = textBox.Text + reader.ReadToEnd();

            }


        }
        private void MenuVehicle_Click(object sender, RoutedEventArgs e)
        {
            selectedVehicles.Remove((Vehicle)dataGrid2.SelectedItem);
           
        }
        private void CheckContainerOnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Container cont = (Container)dataGrid1.SelectedItem;
            foreach(Container c in containers)
            {
                if (c.Name==cont.Name) CheckContainer(c);
            }
        }
        private void CheckContainer(Container cont)
        {
            //if (cont.IsChecked == true) cont.IsChecked = false;
           // else cont.IsChecked = true;
            //MessageBox.Show("Выбран элемент");
            //int index = containers.IndexOf(cont);
            //containers.SetItem(index, cont);
            //dataGrid1.ItemsSource = containers; :TODO Не дописанная функция
            
        }

        private void vehicleTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Vehicle clonableVehicle = (Vehicle)vehicleTree.SelectedItem;
            selectedVehicles.Add(clonableVehicle.Clone() as Vehicle);
        }
    }
    
}
