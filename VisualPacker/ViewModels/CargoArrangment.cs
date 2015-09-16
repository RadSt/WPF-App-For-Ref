using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VisualPacker.Models;
using DataGrid = Microsoft.Windows.Controls.DataGrid;

namespace VisualPacker.ViewModels
{
    public class CargoArrangment
    {
        ObservableCollection<Container> containers=new ObservableCollection<Container>();
        ObservableCollection<Vehicle> selectedVehicles=new ObservableCollection<Vehicle>();
        private readonly Calculation calculation = new Calculation();
        public CargoArrangment( ObservableCollection<Container> containers, ObservableCollection<Vehicle> selectedVehicles)
        {
            this.containers = containers;
            this.selectedVehicles = selectedVehicles;
        }
        public void ArrangeCargoToVehicle(DataGrid dataGrid1,TextBox textBox)
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
            //WriteLog("Выполнен расчет схемы загрузки контейнеров");

            //SaveProtocolToFile();
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
    }
}