using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using VisualPacker.Models;
using VisualPacker.ViewModels.Helpers;

namespace VisualPacker.ViewModels
{
    public class Calculation
    {
        public List<Container> ListToContainerListIncludeVerticalPallet(List<Container> blocks)
        {
            FromTempListToContList fromTempListToContList = new FromTempListToContList();
            List<Container> tempList = fromTempListToContList.ToContainerList(blocks);
            return tempList;
        }

        public List<Container> CalculateLoadScheme(List<Container> containers,
            ObservableCollection<Vehicle> selectedVehicles, TextBox textBox, int maxTonnage)
        {
            int tempMaxTonnage;
            textBox.Clear();
            textBox.AppendText("Протокол расчета схемы загрузки:\n");

            var tempList = RotateContainers(containers);

            foreach (Vehicle v in selectedVehicles)
            {
                tempMaxTonnage = maxTonnage == 0 ? v.Tonnage : maxTonnage;
                tempList = v.DownloadContainers(tempList, tempMaxTonnage);
                PutCargoInfoInTextBox(v, textBox);
                CheckOverweight(v, textBox, tempMaxTonnage);
            }
            PutWasteContainersInfoInTextBox(tempList, textBox);
            CheckErrors(tempList, textBox, selectedVehicles, containers);

            return tempList;
        }

        private List<Container> RotateContainers(List<Container> tempList)
        {
            //поворачиваем контейнеры у которых длинна меньше ширины
            foreach (var c in tempList)
            {
                if (c.Length < c.Width & c.DirLength != "x")
                {
                    c.RotateH();
                }
            }
            return tempList;
        }

        private void PutCargoInfoInTextBox(Vehicle v, TextBox textBox)
        {
            textBox.AppendText("Загрузка автомобиля " + v.Name + " :\n");
            textBox.AppendText("  количество контейнеров - " + v.Count + " :\n");
            textBox.AppendText("  вес груза - " + v.Mass + " :\n");
        }

        private void CheckOverweight(Vehicle v, TextBox textBox, int MaxTonnage)
        {
            var p = v.GetMassCenter();
            var maxTonnage = MaxTonnage*(v.EmptyTonnage*v.Length + 2*p.X*v.Mass)/(v.Length*(v.EmptyTonnage + v.Mass));
            if (v.Mass > maxTonnage & p.X < v.Length/2)
            {
                textBox.AppendText("Превышение нагрузки на переднюю ось \n");
            }
            if (v.Mass > maxTonnage & p.X > v.Length/2)
            {
                textBox.AppendText("Превышение нагрузки на заднюю ось \n");
            }
            if (v.Mass > maxTonnage & p.X == v.Length/2)
            {
                textBox.AppendText("Превышение нагрузки на все оси \n");
            }
        }

        private void PutWasteContainersInfoInTextBox(List<Container> tempList, TextBox textBox)
        {
            if (tempList.Count > 0)
            {
                textBox.AppendText(tempList.Count + " контейнеров не вместилось в выбранные автомобили:\n");
                foreach (var t in tempList)
                {
                    textBox.AppendText("  " + t.Name + " " + t.Vgh + " " + t.PriorityString + " приоритет " + "\n");
                }
            }
            else
            {
                textBox.AppendText("Все контейнеры успешно загружены.\n\n");
            }
        }

        private void CheckErrors(List<Container> tempList, TextBox textBox,
            ObservableCollection<Vehicle> selectedVehicles, List<Container> containers)
        {
            CheckIfPriorityContainerInWasteList(tempList, textBox);
            CheckContainerCount(containers, tempList, textBox, selectedVehicles);
        }

        private void CheckIfPriorityContainerInWasteList(List<Container> tempList, TextBox textBox)
        {
            var priorityError = 0;
            foreach (var t in tempList)
            {
                if (t.Priority == 0)
                {
                    priorityError++;
                }
            }
            if (priorityError > 0)
            {
                textBox.AppendText("Ошибка.Один или несколько приоритетных контейнеров не загрузились.\n");
            }
        }

        private void CheckContainerCount(List<Container> containers, List<Container> tempList, TextBox textBox,
            ObservableCollection<Vehicle> selectedVehicles)
        {
            var contCount = XmlHelper.ContainersCount(tempList);

            var newList = ListToContainerListIncludeVerticalPallet(tempList);
            foreach (var v in selectedVehicles)
            {
                contCount = contCount + v.Count;
                newList.AddRange(v.VehicleToContainerList());
            }
            if (containers.Count() != contCount)
            {
                textBox.AppendText("Ошибка. Расхождение количества контейнеров: было " + containers.Count() + ", стало" +
                                   contCount + ".\n");
                PutContainerVarianceInTextBox(newList, containers, textBox);
            }
        }

        private void PutContainerVarianceInTextBox(List<Container> newList, List<Container> containers,
            TextBox textBox)
        {
            foreach (var c in containers)
            {
                var newList2 = newList.Where(t => t.Name == c.Name).ToList();
                if (!newList2.Any())
                {
                    textBox.AppendText("Пропавший контейнер: " + c.Name + ".\n");
                }
                if (newList2.Count() > 1)
                {
                    textBox.AppendText("Задублированный контейнер: " + c.Name + ".\n");
                }
            }
        }
    }
}