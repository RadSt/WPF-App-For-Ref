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
   static class Calculation
   {
       private static List<int> DistinctOrders(List<Container> containers)
       {   List<int> orderList =new List<int>();
           foreach (Container c in containers)
           {
               int order=c.Order;
                orderList.Contains(order);
                if (orderList.Contains(order))
                {
               //ничего не делаем
                }
                else
                {
                    orderList.Add(order);
                }
           }

           return orderList;
       }
       public static Point3D CalculateMassCenterRow(RowBlock rBlock)
       {
           double nLength = 0;
           double nWidth = 0;
           double nHeight = 0;

           Point3D massCenterPoint = new Point3D(0, 0, 0);
           if (rBlock.Mass == 0) { return massCenterPoint; }
           foreach (VerticalBlock v in rBlock.Blocks)
           {
               foreach (Container c in v.blocks)
               {
                   nLength = nLength + c.Mass * (c.FirstPoint.Y + c.Length / 2);
                   nWidth = nWidth + c.Mass * (c.FirstPoint.X + c.Width / 2);
                   nHeight = nHeight + c.Mass * (c.FirstPoint.Z + c.Height / 2);
               }
           }
           massCenterPoint.Y = nLength / rBlock.Mass;
           massCenterPoint.X = nWidth / rBlock.Mass;
           massCenterPoint.Z = nHeight / rBlock.Mass;
           return massCenterPoint;
       }
       public static List<Container> ListToContainerList(List<Container> blocks)
        {
            List<Container> tempList = new List<Container>();
            foreach (Object Data in blocks)
            {
                if (Data is VerticalBlock)
                {
                    VerticalBlock v = (VerticalBlock)Data;
                    v.ToContainerList(tempList);
                }
                else if (Data is RowBlock)
                {
                    RowBlock r = (RowBlock)Data;
                    r.ToContainerList(tempList);
                }
                else if (Data is HorizontalBlock)
                {
                    HorizontalBlock c = (HorizontalBlock)Data;
                    c.ToContainerList(tempList);
                }
                else if (Data is Container)
                {
                    Container c = (Container)Data;
                    c.ToContainerList(tempList);
                }
                else
                {
                    MessageBox.Show("В процедуру выгрузки контейнеров класса VerticalBlock передан неверный тип данных:" + Data.GetType());
                }
            }
            return tempList;
    }
       public static List<Container> ListToContainerListIncludeVerticalPallet(List<Container> blocks)
       {
           List<Container> tempList = new List<Container>();
           foreach (Object Data in blocks)
           {
               if (Data is VerticalBlock)
               {
                   VerticalBlock v = (VerticalBlock)Data;
                   v.ToContainerListIncludeVerticalPallet(tempList);
               }
               else if (Data is RowBlock)
               {
                   RowBlock r = (RowBlock)Data;
                   r.ToContainerList(tempList);
               }
               else if (Data is HorizontalBlock)
               {
                   HorizontalBlock c = (HorizontalBlock)Data;
                   c.ToContainerList(tempList);
               }
               else if (Data is Container)
               {
                   Container c = (Container)Data;
                   c.ToContainerList(tempList);
               }
               else
               {
                   MessageBox.Show("В процедуру выгрузки контейнеров класса VerticalBlock передан неверный тип данных:" + Data.GetType());
               }
           }
           return tempList;
       }
       public static List<Container> CalculateLoadScheme(List<Container> containers, ObservableCollection<Vehicle> selectedVehicles,TextBox textBox,int MaxTonnage)
       {    int tempMaxTonnage;
           int widthBetweenVehicles = 1000;
             Point3D tempPoint = new Point3D(0, 0, 0);
            textBox.Clear();
            textBox.AppendText("Протокол расчета схемы загрузки:\n");

            List<Container> tempList = prepareContainers(containers);

            foreach (Vehicle v in selectedVehicles) {
                if (MaxTonnage == 0) { tempMaxTonnage = v.Tonnage; }
                else { tempMaxTonnage = MaxTonnage; }
                tempList = v.DownloadContainers(tempList, tempMaxTonnage);
               v.SetFirstPoint(tempPoint);
               tempPoint.Y = tempPoint.Y + widthBetweenVehicles + v.Width;
               PutCargoInfoInTextBox(v,textBox);
               CheckOverweight(v, textBox, tempMaxTonnage);
            }
            PutWasteContainersInfoInTextBox(tempList, textBox);
            CheckErrors(tempList, textBox, selectedVehicles,containers);
             
           return tempList;
       }
       private static List<Container> prepareContainers(List<Container> tempList)
       {
           //поворачиваем контейнеры у которых длинна меньше ширины
           foreach (Container c in tempList){
               if (c.Length < c.Width & c.DirLength != "x") {
                   c.RotateH(); }
           }
           return tempList;
       }
       private static void PutCargoInfoInTextBox(Vehicle v,TextBox textBox)
       {
           textBox.AppendText("Загрузка автомобиля " + v.Name + " :\n");
           textBox.AppendText("  количество контейнеров - " + v.Count + " :\n");
           textBox.AppendText("  вес груза - " + v.Mass + " :\n");
       }
       private static void CheckOverweight(Vehicle v, TextBox textBox, int MaxTonnage)
       {
           Point3D p = v.GetMassCenter();
           double maxTonnage = MaxTonnage * (v.EmptyTonnage * v.Length + 2 * p.X * v.Mass) / (v.Length * (v.EmptyTonnage + v.Mass));
           if (v.Mass>maxTonnage & p.X<v.Length/2) {
               textBox.AppendText("Превышение нагрузки на переднюю ось \n");}
           if (v.Mass>maxTonnage & p.X>v.Length/2) {
               textBox.AppendText("Превышение нагрузки на заднюю ось \n");}
           if (v.Mass > maxTonnage & p.X== v.Length / 2) {
               textBox.AppendText("Превышение нагрузки на все оси \n");}
       }

       
       private static void PutWasteContainersInfoInTextBox(List<Container> tempList, TextBox textBox)
       {
           if (tempList.Count > 0)  {
               textBox.AppendText(tempList.Count + " контейнеров не вместилось в выбранные автомобили:\n");
               foreach (Container t in tempList) {
                   textBox.AppendText("  " + t.Name + " " + t.Vgh + " " + t.PriorityString + " приоритет " + "\n"); }
           }
           else  {
               textBox.AppendText("Все контейнеры успешно загружены.\n\n"); }
       }
       private static void CheckErrors(List<Container> tempList, TextBox textBox, ObservableCollection<Vehicle> selectedVehicles, List<Container> containers)
       {
           CheckIfPriorityContainerInWasteList(tempList,textBox);
           CheckContainerCount(containers, tempList, textBox,selectedVehicles);
       }
       private static void CheckIfPriorityContainerInWasteList(List<Container> tempList,TextBox textBox)
       {
           int priorityError = 0;
           foreach (Container t in tempList)
           {
               if (t.Priority == 0)
               {
                   priorityError++;
               }
           }
           if (priorityError > 0) { textBox.AppendText("Ошибка.Один или несколько приоритетных контейнеров не загрузились.\n"); }
       }
       private static void CheckContainerCount(List<Container> containers, List<Container> tempList, TextBox textBox, ObservableCollection<Vehicle> selectedVehicles)
       {
           int contCount = XmlHelper.ContainersCount(tempList);

           List<Container> newList = ListToContainerListIncludeVerticalPallet(tempList);
           foreach (Vehicle v in selectedVehicles) {
               contCount = contCount + v.Count;
               newList.AddRange(v.VehicleToContainerList());
           }
           if (containers.Count() != contCount)  {
               textBox.AppendText("Ошибка. Расхождение количества контейнеров: было " + containers.Count() + ", стало" + contCount + ".\n");
               PutContainerVarianceInTextBox(newList,containers,textBox); 
           }
       }
       private static void PutContainerVarianceInTextBox(List<Container> newList, List<Container> containers, TextBox textBox)
       {
                foreach (Container c in containers) {
                   List<Container> newList2 = newList.Where(t => t.Name == c.Name).ToList();
                   if (newList2.Count() == 0) { textBox.AppendText("Пропавший контейнер: " + c.Name + ".\n"); }
                   if (newList2.Count() > 1) { textBox.AppendText("Задублированный контейнер: " + c.Name + ".\n"); }
               }
       }
    }
}
