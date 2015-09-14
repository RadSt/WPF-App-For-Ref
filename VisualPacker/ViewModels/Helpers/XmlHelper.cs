using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Xml.Linq;
using VisualPacker.Models;

namespace VisualPacker.ViewModels.Helpers
{
    static class XmlHelper
    {
        public static ObservableCollection<Container> LoadItemFromXML(String fileName)
        {
            NumberFormatInfo nfi = NumberFormatInfo.CurrentInfo;
            string CurrentDecimalSeparator = nfi.CurrencyDecimalSeparator;
            string downloadReport = "";
            string downloadReportPrice = "";
            ObservableCollection<Container> containers = new ObservableCollection<Container>();
            XDocument xDoc = XDocument.Load(fileName);
            XElement dapl = xDoc.Element("dapl");
            IEnumerable<XElement> elements = dapl.Element("items").Elements("item");
            foreach (XElement element in elements)
            {
                string contName = "";
                try
                {
                    IEnumerable<XElement> details = element.Elements();
                    Container container = new Container();
                    container.Kind = element.Attribute("kind").Value;
                    foreach (XElement d in details)
                    {
                        switch (d.Name.ToString())
                        {
                            case "name":
                                container.Name = element.Element("name").Value;
                                contName = element.Element("name").Value;
                                break;
                            case "short_name":
                                container.ShortName = element.Element("short_name").Value;
                                break;
                            case "container_type":
                                container.ContainerType = element.Element("container_type").Value;
                                break;
                            case "ship_to_name":
                                container.ShipToName = element.Element("ship_to_name").Value;
                                break;
                            case "internal_load_num":
                                container.ShipmentId = element.Element("internal_load_num").Value;
                                break;
                            case "length":
                                container.Length = int.Parse(d.Value.Replace(".00000", ""));
                                break;
                            case "height":
                                container.Height = int.Parse(d.Value.Replace(".00000", ""));
                                break;
                            case "width":
                                container.Width = int.Parse(d.Value.Replace(".00000", ""));
                                break;
                            case "price":
                                if (d.Value == "") { container.Price = 0; }
                                else
                                {
                                    container.Price = double.Parse(d.Value.Replace(".", CurrentDecimalSeparator));
                                }
                                break;
                            case "mass":
                                container.Mass = double.Parse(d.Value.Replace(".", CurrentDecimalSeparator));
                                break;
                            case "level":
                                container.Level = int.Parse(d.Value.Replace(".00000", ""));
                                break;
                            case "press":
                                IEnumerable<XElement> pr = d.Elements();
                                foreach (XElement p in pr)
                                {
                                    if (p.Name.ToString() == "length") container.PressLength = double.Parse(p.Value.Replace(".00000", CurrentDecimalSeparator));

                                    if (p.Name.ToString() == "width") container.PressWidth = double.Parse(p.Value.Replace(".00000", CurrentDecimalSeparator));
                                    if (p.Name.ToString() == "height")
                                    {
                                        container.PressHeight = double.Parse(p.Value.Replace(".00000", CurrentDecimalSeparator));
                                        if (container.PressHeight == 0) container.PressHeight = 10000;
                                    }
                                }
                                break;
                            case "order":
                                if (element.Element("order").Value == "")
                                {
                                    container.Order = 0;
                                }
                                else
                                {
                                    container.Order = int.Parse(element.Element("order").Value);
                                }
                                break;
                            case "order_type":
                                if (element.Element("order").Value == "СРОЧНЫЙ")
                                { container.Priority = 0; }
                                else { container.Priority = 1; }
                                break;
                            case "dir":
                                IEnumerable<XElement> dir = d.Elements();
                                foreach (XElement dr in dir)
                                {
                                    if (dr.Name.ToString() == "length")
                                    {
                                        dr.Value = dr.Value.ToLower(); //преобразуем в прописные буквы
                                        if (dr.Value == "") { container.DirLength = "a"; }
                                        else if (dr.Value == "у") { container.DirLength = "y"; } //заменяем русские буквы на английские
                                        else if (dr.Value == "x") { container.DirLength = "x"; } //заменяем русские буквы на английские
                                        else if (dr.Value == "а") { container.DirLength = "a"; } //заменяем русские буквы на английские
                                        else { container.DirLength = dr.Value; }
                                    }
                                }
                                break;
                            case "fragility":
                                IEnumerable<XElement> fragility = d.Elements();
                                foreach (XElement f in fragility)
                                {
                                    if (f.Name.ToString() == "Heigth") container.DirLength = f.Value;
                                }
                                break;
                            case "color":
                                //MessageBox.Show(element.Element("color").Value);
                                container.Color = d.Value;

                                //container.color = System.Drawing.ColorTranslator.FromHtml(element.Element("color").Value) ;
                                break;
                            case "Group":
                                IEnumerable<XElement> group = d.Elements();
                                foreach (XElement g in group)
                                {
                                    if (g.Name.ToString() == "name") container.GroupName = g.Value;
                                    if (g.Name.ToString() == "id") container.GroupId = int.Parse(g.Value);
                                    if (g.Name.ToString() == "quantity") container.GroupQuantity = int.Parse(g.Value);
                                }
                                break;
                            case "pal_quantity":
                                container.PalQuantity = int.Parse(d.Value);
                                break;
                            case "quantity":
                                container.Quantity = int.Parse(d.Value);
                                break;
                            case "only4bottom":
                                if (d.Value == "")
                                {
                                    container.Only4Bottom = 0;
                                }
                                else
                                {
                                    container.Only4Bottom = int.Parse(d.Value);
                                }

                                break;
                        }
                    }
                    //Проверяем на нулевые габариты и вес
                    if (container.Length == 0 || container.Width == 0 || container.Height == 0 || container.Mass == 0)
                    {
                        downloadReport = downloadReport + container.Name + ";";
                    }
                    if (container.Price == 0)
                    {
                        downloadReportPrice = downloadReportPrice + container.Name + ";";
                    }
                    container.IsChecked = true;
                    containers.Add(container);
                }
                catch { MessageBox.Show("Ошибка обработки контейнера  " + contName); }

            }

           
            if (downloadReport != "")
            {
                MessageBox.Show("Ошибка при загрузке контейнеров. Один или несколько контейнеров имеют нулевые ВГХ: " + downloadReport);
                //return new List<Container>();
            } 
            
            if (downloadReportPrice != "")
            {
                //MessageBox.Show("Ошибка при загрузке контейнеров. Один или несколько контейнеров имеют нулевую стоимость: " + downloadReportPrice);
                //return new List<Container>();
            }

            return containers;
        }
        public static ObservableCollection<Vehicle> LoadVehiclesFromXML(String fileName)
        {
            ObservableCollection<Vehicle> vehicles = new ObservableCollection<Vehicle>();
            XDocument xDoc = XDocument.Load(fileName);
            XElement dapl = xDoc.Element("dapl");
            IEnumerable<XElement> elements = dapl.Element("vehicles").Elements("vehicle");
            foreach (XElement element in elements)
            {
                IEnumerable<XElement> details = element.Elements();
                Vehicle vehicle = new Vehicle();
                vehicle.Kind = element.Attribute("kind").Value;
                foreach (XElement d in details)
                {
                    switch (d.Name.ToString())
                    {
                        case "name":
                            vehicle.Name = element.Element("name").Value;
                            break;
                        
                        case "length":
                            vehicle.Length = int.Parse(d.Value.Replace(".00000", ""));
                            break;
                        case "height":
                            vehicle.Height = int.Parse(d.Value.Replace(".00000", ""));
                            break;
                        case "width":
                            vehicle.Width = int.Parse(d.Value.Replace(".00000", ""));
                            break;
                        case "tonnage":
                            vehicle.Tonnage = int.Parse(d.Value.Replace(".00000", ""));
                            break;
                        case "fullTonnage":
                            vehicle.FullTonnage = int.Parse(d.Value.Replace(".00000", ""));
                            break;
                        case "door":
                            IEnumerable<XElement> door = d.Elements();
                            foreach (XElement p in door)
                            {
                                if (p.Name.ToString() == "width") vehicle.DoorWidth = int.Parse(p.Value.Replace(".00000", ","));
                                if (p.Name.ToString() == "height") vehicle.DoorHeight = int.Parse(p.Value.Replace(".00000", ","));
                                if (p.Name.ToString() == "center_distance") vehicle.DoorCenterDistance = int.Parse(p.Value.Replace(".00000", ","));
                            }
                            break;
                        case "roof":
                            vehicle.Roof = int.Parse(d.Value.Replace(".00000", ""));
                            break;
                        case "EmptyAvtoTonnage":
                            vehicle.EmptyAvtoTonnage = int.Parse(d.Value.Replace(".00000", ""));
                            break;
                        case "EmptyTrailerTonnage":
                            vehicle.EmptyTrailerTonnage = int.Parse(d.Value.Replace(".00000", ""));
                            break;
                    }
                }
                vehicles.Add(vehicle);
            }
            return vehicles;
        } 

        public static int ContainersCount(Object Data)
        {
            int ch = 0;
            if (Data is List<VerticalBlock>)
            {
                foreach (VerticalBlock c in (List<VerticalBlock>)Data) { ch = ch + c.Count; }
                return ch;
            }
            else if (Data is List<RowBlock>)
            {
                foreach (RowBlock c in (List<RowBlock>)Data) { ch = ch + c.Count; }
                return ch;
            }

            else if (Data is List<Container>)
            {
                foreach (Container c in (List<Container>)Data) { ch = ch + c.Count; }
                return ch;
            }
            else
            {
                MessageBox.Show("В форму расчета количества контейнеров передан неверный тип данных:" + Data.GetType());
                return 0;
            }
        }
    }
    }

