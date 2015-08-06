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
    internal static class Calculation
    {
        public static Point3D CalculateMassCenterRow(RowBlock rBlock)
        {
            double nLength = 0;
            double nWidth = 0;
            double nHeight = 0;

            var massCenterPoint = new Point3D(0, 0, 0);
            if (rBlock.Mass == 0)
            {
                return massCenterPoint;
            }
            foreach (var v in rBlock.Blocks)
            {
                foreach (var c in v.Blocks)
                {
                    nLength = nLength + c.Mass*(c.FirstPoint.Y + c.Length/2);
                    nWidth = nWidth + c.Mass*(c.FirstPoint.X + c.Width/2);
                    nHeight = nHeight + c.Mass*(c.FirstPoint.Z + c.Height/2);
                }
            }
            massCenterPoint.Y = nLength/rBlock.Mass;
            massCenterPoint.X = nWidth/rBlock.Mass;
            massCenterPoint.Z = nHeight/rBlock.Mass;
            return massCenterPoint;
        }

        public static List<Container> ListToContainerList(List<Container> blocks)
        {
            var tempList = new List<Container>();
            var fromTempListToContList = new FromTempListToContList();
            foreach (Object Data in blocks)
            {
                if (Data is VerticalBlock)
                {
                    var v = (VerticalBlock) Data;
                    v.ToContainerList(tempList);
                }
                else if (Data is RowBlock)
                {
                    var r = (RowBlock) Data;
                    fromTempListToContList.ToContainerList(tempList, r.Blocks);
                }
                else if (Data is HorizontalBlock)
                {
                    var c = (HorizontalBlock) Data;
                    c.ToContainerList(tempList);
                }
                else if (Data is Container)
                {
                    var c = (Container) Data;
                    c.ToContainerList(tempList);
                }
                else
                {
                    MessageBox.Show(
                        "В процедуру выгрузки контейнеров класса VerticalBlock передан неверный тип данных:" +
                        Data.GetType());
                }
            }
            return tempList;
        }

        public static List<Container> ListToContainerListIncludeVerticalPallet(List<Container> blocks)
        {
            var fromTempListToContList = new FromTempListToContList();
            var tempList = new List<Container>();
            foreach (Object Data in blocks)
            {
                if (Data is VerticalBlock)
                {
                    var v = (VerticalBlock) Data;
                    v.ToContainerListIncludeVerticalPallet(tempList);
                }
                else if (Data is RowBlock)
                {
                    var r = (RowBlock) Data;
                    fromTempListToContList.ToContainerList(tempList, r.Blocks);
                }
                else if (Data is HorizontalBlock)
                {
                    var c = (HorizontalBlock) Data;
                    c.ToContainerList(tempList);
                }
                else if (Data is Container)
                {
                    var c = (Container) Data;
                    c.ToContainerList(tempList);
                }
                else
                {
                    MessageBox.Show(
                        "В процедуру выгрузки контейнеров класса VerticalBlock передан неверный тип данных:" +
                        Data.GetType());
                }
            }
            return tempList;
        }

        public static List<Container> CalculateLoadScheme(List<Container> containers,
            ObservableCollection<Vehicle> selectedVehicles, TextBox textBox, int maxTonnage)
        {
            int tempMaxTonnage;
            var widthBetweenVehicles = 1000;
            var tempPoint = new Point3D(0, 0, 0);
            textBox.Clear();
            textBox.AppendText("Протокол расчета схемы загрузки:\n");

            var tempList = RotateContainers(containers);

            foreach (var v in selectedVehicles)
            {
                tempMaxTonnage = maxTonnage == 0 ? v.Tonnage : maxTonnage;
                tempList = v.DownloadContainers(tempList, tempMaxTonnage);
                v.SetFirstPoint(tempPoint);
                //tempPoint.Y = tempPoint.Y + widthBetweenVehicles + v.Width;
                tempPoint.Z = tempPoint.Z - widthBetweenVehicles - v.Width;
                PutCargoInfoInTextBox(v, textBox);
                CheckOverweight(v, textBox, tempMaxTonnage);
            }
            PutWasteContainersInfoInTextBox(tempList, textBox);
            CheckErrors(tempList, textBox, selectedVehicles, containers);

            return tempList;
        }

        private static List<Container> RotateContainers(List<Container> tempList)
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

        private static void PutCargoInfoInTextBox(Vehicle v, TextBox textBox)
        {
            textBox.AppendText("Загрузка автомобиля " + v.Name + " :\n");
            textBox.AppendText("  количество контейнеров - " + v.Count + " :\n");
            textBox.AppendText("  вес груза - " + v.Mass + " :\n");
        }

        private static void CheckOverweight(Vehicle v, TextBox textBox, int MaxTonnage)
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

        private static void PutWasteContainersInfoInTextBox(List<Container> tempList, TextBox textBox)
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

        private static void CheckErrors(List<Container> tempList, TextBox textBox,
            ObservableCollection<Vehicle> selectedVehicles, List<Container> containers)
        {
            CheckIfPriorityContainerInWasteList(tempList, textBox);
            CheckContainerCount(containers, tempList, textBox, selectedVehicles);
        }

        private static void CheckIfPriorityContainerInWasteList(List<Container> tempList, TextBox textBox)
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

        private static void CheckContainerCount(List<Container> containers, List<Container> tempList, TextBox textBox,
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

        private static void PutContainerVarianceInTextBox(List<Container> newList, List<Container> containers,
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