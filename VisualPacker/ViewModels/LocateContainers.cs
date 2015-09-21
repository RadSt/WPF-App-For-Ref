using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VisualPacker.Models;
using VisualPacker.ViewModels.Helpers;

namespace VisualPacker.ViewModels
{
    public class LocateContainers
    {
       Vehicle vehicle;
       private List<Container> wasteList = new List<Container>();

        public LocateContainers(Vehicle vehicle)
        {
            this.vehicle = vehicle;
        }
        public List<Container> DownloadContainers(List<Container> containers, int maxTonnage)
        {
            vehicle.MaxHeight = vehicle.Height - 350;
            List<Container> tempList;
            while (CannotDownloadAll(containers, maxTonnage) == false & vehicle.MaxHeight > Vehicle.MinHeighCont)
            {
                vehicle.MaxHeight = vehicle.MaxHeight - 100;
            }
            vehicle.MaxHeight = Math.Min(vehicle.Height - 350, vehicle.MaxHeight + 100);
            tempList = DownloadContainersToVehicle(containers, maxTonnage);
            return tempList;
        }

        private bool CannotDownloadAll(List<Container> containers, int maxTonnage)
        {
            List<Container> tempList = DownloadContainersToVehicle(containers, maxTonnage);
            if (NotEmpty(tempList) & vehicle.MaxLength < 500)
            {
                return true;
            }
            return false;
        }

        private List<Container> DownloadContainersToVehicle(List<Container> containers, int maxTonnage)
        {
            ClearVehicle();
            vehicle.MaxLength = vehicle.Length;
            //делим тары на вертикальные блоки 
            List<VerticalBlock> vBlocks = new List<VerticalBlock>();
            FromTempListToContList fromTempListToContList = new FromTempListToContList();
            //разные заказы обрабатываем отдельно т.к. их нужно будет выгружать из машины в разное время

            //Негабаритный товар отсекаем 
            List<Container> containerList = containers.Where(s => s.IsSuitableLength(vehicle.Width) == true).ToList();
            wasteList = containers.Where(s => s.IsSuitableLength(vehicle.Width) == false).ToList();
            List<Container> wasteList2 = containerList.Where(s => s.Height >= vehicle.MaxHeight).ToList();
            containerList = containerList.Where(s => s.Height <= vehicle.MaxHeight).ToList();
            foreach (Container c in wasteList2)
            {
                wasteList.Add(c);
            }

            //маленькие тары будем загружать после основных тар 
            List<Container> tempSmall =
                containerList.Where(c => c.Length <= 400 & c.Width <= 400 & c.Height <= 400)
                    .OrderBy(s => s.Priority)
                    .ThenByDescending(s => s.Price)
                    .ToList();
            containerList = containerList.Where(c => c.Length > 400 | c.Width > 400 | c.Height > 400).ToList();

            //получаем список заказов
            //List<Container> tempSmall2 = new List<Container>();
            List<int> orderList = DistinctOrders(containerList);
            foreach (int order in orderList.OrderBy(o => o))
            {
                List<Container> tempList = containerList.Where(c => c.Order == order).ToList();

                tempList = ProcessingCabin(tempList, maxTonnage).ToList();
                tempList = ProcessingChassis(tempList, maxTonnage).ToList();

                //Если есть кабины то строим лесенку для предотвращения повреждения кабин
                //формируем первый ряд в один ярус
                int minHeight = 100;
                if (NotEmpty())
                {
                    int ch = 0;
                    for (ch = 1; ch < 7; ch++)
                    {
                        if (ch > 1)
                        {
                            minHeight = vehicle.Blocks[vehicle.Blocks.Count() - 1].MinHeight;
                        }
                        tempList = CreateRow3(tempList, minHeight, vehicle.MaxHeight, vehicle.Width, 0, maxTonnage);
                    }
                }

                //контейнеры которые штабелируются в два яруса и ставятся только на пол
                tempList = CreateVerticalPalletsLevel2(tempList, vehicle.MaxHeight);
                //отбираем контейнеры которые штабелируются в 3 яруса и ставятся только на пол
                tempList = CreateVerticalPalletsLevel3(tempList, 800, 400, 390, vehicle.MaxHeight, "1-");
                tempList = CreateVerticalPalletsLevel3(tempList, 800, 400, 500, vehicle.MaxHeight, "2-");
                tempList = CreateVerticalPalletsLevel3(tempList, 900, 360, 420, vehicle.MaxHeight, "3-");
                tempList = CreateVerticalPalletsLevel3(tempList, 890, 570, 540, vehicle.MaxHeight, "4-");

                //Обрабатываем остальные контейнеры
                if (NotEmpty())
                {
                    minHeight = vehicle.Blocks[vehicle.Blocks.Count() - 1].MinHeight;
                } //если есть ряды, то смотрим высоту последнего
                else
                {
                    minHeight = vehicle.Height - 500;
                } //если нет, то берем максимальную высоту кузова
                while (tempList.Any() & minHeight > 0 & vehicle.MaxLength > 0)
                {
                    int ch2 = vehicle.Blocks.Count();
                    tempList = CreateRow3(tempList, minHeight, vehicle.MaxHeight, vehicle.Width, 0, maxTonnage);
                    if (ch2 == vehicle.Blocks.Count())
                    {
                        minHeight = minHeight - 100;
                    }
                }
                ////////////////////////////////////
                //пытаемся доложить контейнеры сверху на ряды
                /////////////////////////////////////
                wasteList.AddRange(tempList);
            }
            wasteList = fromTempListToContList.ToContainerList(wasteList);
            wasteList = AddOnTopRow(wasteList);

            LoadSmallContainersBySquare(tempSmall, orderList);
            return wasteList;
        }

        private bool NotEmpty(List<Container> tempList)
        {
            if (tempList.Any())
            {
                return true;
            }
            return false;
        }

        private List<Container> AddOnTopRow(List<Container> tempList)
        {
            List<Container> returnList = tempList.ToList();
            foreach (RowBlock r in vehicle.Blocks)
            {
                foreach (VerticalBlock v in r.Blocks)
                {
                    int oldCount = returnList.Count();
                    returnList = v.AddOneContainerFromList(returnList, vehicle.MaxHeight);
                    if (oldCount == returnList.Count() + 1)
                    {
                        vehicle.Count = vehicle.Count + 1;
                    }
                }
            }
            return returnList;
        }

        private void LoadSmallContainersBySquare(List<Container> tempList, List<int> orderList)
        {
            foreach (int order in orderList.OrderBy(o => o))
            {
                //////////////////////////////////////////////////////////
                int orderLength = 0;
                foreach (RowBlock r in vehicle.Blocks)
                {
                    if (r.Order == order)
                    {
                        orderLength = orderLength + r.Width;
                    }
                }

                List<Container> tempSmall =
                    tempList.Where(c => c.Order == order)
                        .OrderBy(s => s.Priority)
                        .ThenByDescending(s => s.Price)
                        .ToList();
                tempList = tempList.Where(c => c.Order != order).ToList();
                int tempS = vehicle.Width * orderLength;

                foreach (Container c in tempSmall)
                {
                    if (tempS > 0)
                    {
                        vehicle.SmallBlocks.Add(c);
                        tempS = tempS - c.Width * c.Length;
                    }
                    else
                    {
                        tempList.Add(c);
                    }
                }
            }
            foreach (Container s in vehicle.SmallBlocks)
            {
                vehicle.Mass = vehicle.Mass + s.Mass;
            }
            vehicle.Count = vehicle.Count + vehicle.SmallBlocks.Count();
            wasteList.AddRange(tempList);
        }

        private List<Container> ProcessingChassis(List<Container> tempList, int maxTonnage)
        {
            List<Container> tempRama =
                tempList.Where(s => s.Length > 1500 & s.Width > 6000).OrderBy(s => s.Width).ToList();
            tempList = tempList.Where(s => s.Length <= 1500 | s.Width <= 6000).ToList();
            if (!tempRama.Any())
            {
                return tempList;
            }
            HorizontalBlock hBlock = new HorizontalBlock();
            hBlock.RowCount = 2;
            hBlock.Width = tempRama[0].Width + 800;
            hBlock.Length = 2400;
            //добавляем в подушку ящики размеров 1200х800х800
            List<Container> temp500 = tempList.Where(s => s.Length == 1200 & s.Width == 800 & s.Height == 800).ToList();
            if (temp500.Count() >= 6)
            {
                tempList = tempList.Where(s => s.Length != 1200 | s.Width != 800 | s.Height != 800).ToList();
                foreach (Container c in temp500)
                {
                    if (hBlock.Add(c))
                    {
                        /*продолжаем*/
                    }
                    else
                    {
                        tempList.Add(c);
                    }
                }
            }
            //добавляем в подушку ящики размеров 1100х800х720
            temp500 = tempList.Where(s => s.Length == 1100 & s.Width == 800 & s.Height == 720).ToList();

            if (NotEmpty() | temp500.Count() >= 6)
            {
                tempList = tempList.Where(s => s.Length != 1100 | s.Width != 800 | s.Height != 720).ToList();
                foreach (Container c in temp500)
                {
                    if (!hBlock.Add(c))
                    {
                        tempList.Add(c);
                    }
                }
            }

            VerticalBlock vBlock = new VerticalBlock();
            if (hBlock.NotEmpty() & (vehicle.Mass + hBlock.Mass) <= maxTonnage)
            {
                vBlock.Add(hBlock, vehicle.Height - 250);
            }
            else
            {
                hBlock.ToContainerList(tempList);
            }

            foreach (Container r in tempRama)
            {
                if ((vehicle.Mass + r.Mass) <= maxTonnage)
                {
                    if (vBlock.Add(r, vehicle.Height - 350) == false)
                    {
                        MessageBox.Show("не удалось добавить раму в вертикальный блок.Сообщите администратору");
                        tempList.Add(r);
                    }
                }
            }
            RowBlock rBlock = new RowBlock();
            rBlock.Add(vBlock, vehicle.Width);
            AddRowToVehicle(rBlock);
            return tempList;
        }


        private void ClearVehicle()
        {
            vehicle.Blocks.Clear();
            vehicle.SmallBlocks.Clear();
            wasteList.Clear();
            vehicle.Count = 0;
            vehicle.Mass = 0;
        }

        private List<Container> ProcessingCabin(List<Container> tempList, int maxTonnage)
        {
            List<Container> tempCabin = tempList.Where(s => s.Only4Bottom == 1).OrderByDescending(s => s.Mass).ToList();
            tempList = tempList.Where(s => s.Only4Bottom != 1).ToList();

            foreach (Container c in tempCabin)
            {
                if (c.Length > vehicle.Width)
                {
                    if (c.Width <= vehicle.Width & c.DirLength == "a")
                    {
                        c.RotateH();
                    }
                    else
                    {
                        MessageBox.Show("Кабина " + c.Name + " превышает габариты кузова.Сообщите администратору");
                        break;
                    }
                }
                if (vehicle.Mass + c.Mass <= maxTonnage)
                {
                    AddCabinToRow(c);
                }
                else
                {
                    tempList.Add(c);
                }
            }
            return tempList;
        }

        private List<Container> CreateVerticalPalletsLevel3(List<Container> containers, int length, int width, int height,
           int maxHeight, string prefix)
        {
            FromTempListToContList fromTempListToContList = new FromTempListToContList();
            List<Container> t1 = fromTempListToContList.ToContainerList(containers);
            //используется для контроля потери контейнеров

            List<Container> only4Bottom =
                containers.Where(c => c.Length == length & c.Width == width & c.Height == height)
                    .OrderBy(c => c.Color)
                    .ToList();
            List<Container> tempList =
                containers.Where(c => c.Length != length | c.Width != width | c.Height != height).ToList();
            List<VerticalBlock> vBlocks = new List<VerticalBlock>();
            List<Container> tempList2 = new List<Container>();
            int nameCh = 0;
            while (only4Bottom.Any())
            {
                VerticalBlock vBlock = new VerticalBlock { Kind = "VerticalPallet", PressHeight = 0 };
                List<Container> sameList = only4Bottom.ToList();
                only4Bottom = new List<Container>();
                foreach (Container s in sameList)
                {
                    if (vBlock.Count == 3)
                    {
                        only4Bottom.Add(s);
                    }
                    else if (vBlock.Add(s, maxHeight) == false)
                    {
                        //MessageBox.Show("Не удалось добавить контейнер в связку.Сообщите администратору");
                        tempList2.Add(s);
                    }
                }

                if (vBlock.Count == 1)
                {
                    tempList2.Add(vBlock.Blocks[0]);
                }
                else if (vBlock.Count == 2)
                {
                    nameCh++;
                    vBlock.Name = "Связка паллет в 2 яруса  №" + prefix + nameCh;

                    vBlocks.Add(vBlock);
                }
                else if (vBlock.Count == 3)
                {
                    nameCh++;
                    vBlock.Name = "Связка паллет в 3 яруса  №" + prefix + nameCh;
                    vBlocks.Add(vBlock);
                }
            }
            tempList.AddRange(vBlocks);
            tempList.AddRange(tempList2);

            List<Container> t2 = fromTempListToContList.ToContainerList(tempList);
            //используется для контроля потери контейнеров
            if (t1.Count() != t2.Count())
            {
                MessageBox.Show("Потеря контейнеров в процедуре CreateVerticalPalletsLevel3");
            }
            return tempList;
        }

        private List<Container> CreateVerticalPalletsLevel2(List<Container> containers, int maxHeight)
        {
            FromTempListToContList fromTempListToContList = new FromTempListToContList();
            List<Container> t1 = fromTempListToContList.ToContainerList(containers);
            //используется для контроля потери контейнеров
            //контейнеры которые штабелируются в два яруса и ставятся только на пол
            List<Container> tempOnly4Bottom =
                containers.Where(c => c.Only4Bottom == 2).OrderBy(c => c.ContainerType).ToList();
            List<Container> tempList = containers.Where(c => c.Only4Bottom != 2).ToList();

            int nameCh = 0;

            while (tempOnly4Bottom.Any())
            {
                VerticalBlock vBlock = new VerticalBlock { Kind = "VerticalPallet" };
                nameCh++;
                vBlock.Name = "Связка паллет в 2 яруса №" + nameCh;
                vBlock.Only4Bottom = 2;

                Container firstContainer = tempOnly4Bottom[0];
                List<Container> sameList =
                    tempOnly4Bottom.Where(c => c.ContainerType == firstContainer.ContainerType)
                        .OrderByDescending(c => c.Mass)
                        .ToList();
                tempOnly4Bottom = tempOnly4Bottom.Where(c => c.ContainerType != firstContainer.ContainerType).ToList();

                foreach (Container s in sameList)
                {
                    if (vBlock.Count < 2)
                    {
                        if (vBlock.Add(s, maxHeight) == false)
                        {
                            tempOnly4Bottom.Add(s);
                        }
                    }
                    else
                    {
                        tempOnly4Bottom.Add(s);
                    }
                }
                if (vBlock.Count == 2)
                {
                    vBlock.PressHeight = 0;
                    tempList.Add(vBlock);
                }
                else if (vBlock.Count == 1)
                {
                    tempList.Add(vBlock.Blocks[0]);
                }
            }
            List<Container> t2 = fromTempListToContList.ToContainerList(tempList);
            //используется для контроля потери контейнеров
            if (t1.Count() > t2.Count())
            {
                MessageBox.Show("Ошибка! Потеря контейнеров в процедуре CreateVerticalPalletsLevel2");
            }
            else if (t1.Count() < t2.Count())
            {
                MessageBox.Show("Ошибка! Дублирование контейнеров в процедуре CreateVerticalPalletsLevel2");
            }
            return tempList;
        }

        private List<Container> CreateRow3(List<Container> containers, int minHeight, int maxHeight, int maxWidth,
            int show, int maxTonnage)
        {
            //первым делом создаем вертикальные блоки нужной высоты (не меньше minHeight;  
            var vBlocks = new List<VerticalBlock>();
            var tempList = containers.OrderByDescending(c => c.Length).ToList();
            var smallContainers = new List<Container>();
            var lowBlocks = new List<VerticalBlock>();
            RowBlock result;
            //поворачиваем контейнеры которые должны стоять вдоль борта
            foreach (var c in tempList)
            {
                if ((c.Length > c.Width & c.DirLength == "x") | c.Length > maxWidth)
                {
                    c.RotateH();
                    c.DirLength = "x";
                }
            }
            if (minHeight == 0)
            {
                MessageBox.Show("Ошибка: в процедуру  CreateRow3 передан параметр minHeight с нулевым значением");
            }

            while (tempList.Any())
            {
                var firstCont = tempList[0];
                var check4 = tempList.Count();
                var sameCont1 =
                    tempList.Where(s => s.AreSame01(firstCont))
                        .OrderByDescending(s => s.Mass)
                        .ThenBy(s => s.Color)
                        .ToList();
                tempList = tempList.Where(s => (s.AreSame01(firstCont) == false)).OrderBy(s => s.Color).ToList();
                if (sameCont1.Count() + tempList.Count() != check4)
                {
                    MessageBox.Show("неправильная фильтрация контейнеров");
                }
                var vBlock = new VerticalBlock();
                foreach (var c in sameCont1)
                {
                    if (c.Height < vehicle.Height & c.Height >= maxHeight & vBlock.Count == 0)
                    {
                        //если контейнер выше чем нужно но меньше макс. высота, то добавляем его в отдельный вертикальный блок
                        if (vBlock.Add(c, vehicle.Height) == false)
                        {
                            tempList.Add(c);
                        }
                    }
                    else if (vBlock.Height <= minHeight)
                    {
                        if (vBlock.Add(c, maxHeight) == false)
                        {
                            tempList.Add(c);
                        }
                    }
                    else
                    {
                        tempList.Add(c);
                    }
                }

                if (vBlock.Height >= minHeight)
                {
                    vBlocks.Add(vBlock);
                }
                else
                {
                    lowBlocks.Add(vBlock);
                } //низкие блоки будем использовать при уплотнении самого плотного ряда
            }

            if (XmlHelper.ContainersCount(containers) - XmlHelper.ContainersCount(tempList) -
                XmlHelper.ContainersCount(vBlocks) - XmlHelper.ContainersCount(lowBlocks) != 0)
            {
                MessageBox.Show("Расхождение количества контейнеров 3");
            }
            //LoadSchemeCalculation win = new LoadSchemeCalculation(VBlocks);
            //win.ShowDialog();
            //формируем ряды
            var rBlocks = new List<RowBlock>();
            var sameWidth = 100;
            var sh = vBlocks.Count();
            while (sameWidth < vehicle.MaxLength)
            {
                CreateRows(vBlocks, sameWidth, rBlocks);
                sameWidth = sameWidth + 100;
            }
            var sh2 = VerticalBlocksDistinctCount(vBlocks);
            if (sh != sh2)
            {
                MessageBox.Show("Расхождение количества вертикальных блоков");
            }
            //выбираем самый плотный ряд 

            if (rBlocks.Any())
            {
                rBlocks = rBlocks.OrderByDescending(s => s.FullnessWidth).ThenByDescending(s => s.MinHeight).ToList();
                result = rBlocks[0];
                vBlocks = vBlocks.Where(s => result.ContainsVerticalBlock(s, vBlocks) == false).ToList();

                if ((result.FullnessWidth > 0.4 | containers.Count() == 1) & (vehicle.Mass + result.Mass) <= maxTonnage)
                // if (result.FullnessWidth > 0.4 | Containers.Count() <= 5)
                {
                    if (result.FullnessWidth < 0.9)
                    {
                        //сначала пытаемся добавить высокий блок 
                        vBlocks = AddExtraBlock(result, vBlocks);
                        //потом пытаемся добавить низкий блок
                        lowBlocks = AddExtraBlock(result, lowBlocks);
                    }
                    AddRowToVehicle(result);
                }
                else
                {
                    vBlocks.AddRange(result.Blocks);
                }
            }
            //оставшиеся низкие блоки выгружаем в общий список
            vBlocks.AddRange(lowBlocks);
            //неиспользованные вертикальные ряды выгружаем в список контейнеров
            foreach (var v in vBlocks)
            {
                v.ToContainerList(tempList);
            }
            //возвращаем маленькие контейнеры в общий список
            tempList.AddRange(smallContainers);
            //Поворачиваем контейнеры обратно;
            foreach (var c in tempList.Where(c => c.Length < c.Width))
            {
                c.RotateH();
            }
            return tempList;
        } //CreateRow3

        private int VerticalBlocksDistinctCount(List<VerticalBlock> data)
        {
            // нужна для проверки потери вертикальных контейнеров в процедуре CreateRows
            List<VerticalBlock> tempList = new List<VerticalBlock>();
            foreach (VerticalBlock c in data)
            {
                if (ListContainVerticalBlock(tempList, c) == false)
                {
                    tempList.Add(c);
                }
            }
            return tempList.Count();
        }

        private void CreateRows(List<VerticalBlock> vBlocks, int sameWidth, List<RowBlock> rBlocks)
        {
            List<VerticalBlock> sameBlock =
                vBlocks.Where(s => s.IsSuitableWidth(sameWidth)).OrderByDescending(s => s.Width).ToList();
            while (sameBlock.Any())
            {
                List<VerticalBlock> tempBlocks = new List<VerticalBlock>();
                RowBlock rBlock = new RowBlock();
                foreach (VerticalBlock s in sameBlock)
                {
                    VerticalBlock tempS;
                    if (s.Length <= sameWidth & s.Length > sameWidth - 100) //если нужно, то поворачиваем блок
                    {
                        tempS = s.Clone() as VerticalBlock;
                        tempS.RotateH();
                    }
                    else
                    {
                        tempS = s;
                    }
                    if (tempS.Length > vehicle.Width)
                    {
                        /*пропускаем блок т.к. он не подходит по ширине и длине*/
                    }
                    else if (rBlock.Add(tempS, vehicle.Width) == false)
                    {
                        tempBlocks.Add(tempS);
                    }
                }
                if (rBlock.Count > 0)
                {
                    rBlocks.Add(rBlock);
                }
                sameBlock = tempBlocks.ToList();
            }
        }


        private bool NotEmpty()
        {
            return vehicle.Blocks.Any();
        }

        private List<int> DistinctOrders(List<Container> containers)
        {
            List<int> orderList = new List<int>();
            foreach (Container c in containers)
            {
                if (!orderList.Contains(c.Order))
                {
                    orderList.Add(c.Order);
                }
            }

            return orderList;
        }
        private void AddCabinToRow(Container c)
        {
            VerticalBlock vBlock = new VerticalBlock();
            if (vBlock.Add(c, 10000) == false)
            {
                MessageBox.Show("Ошибка добавления кабины в вертикальный блок.Сообщите администратору");
                return;
            }
            //добавляем вертикальный ряд в горизонтальный ряд
            RowBlock rBlock = new RowBlock();
            rBlock.Add(vBlock, vehicle.MaxLength);

            if (vehicle.MaxLength >= rBlock.Width & vehicle.Tonnage >= rBlock.Mass + vehicle.Mass)
            {
                AddRowToVehicle(rBlock);
            }
            else
            {
                //помещаем кабину в список невместившихся контейнеров
                wasteList.Add(c);
            }
        }

        private void AddRowToVehicle(RowBlock rBlock)
        {
            vehicle.Blocks.Add(rBlock);
            vehicle.MaxLength = vehicle.MaxLength - rBlock.Width;
            vehicle.Mass = vehicle.Mass + rBlock.Mass;
            vehicle.Count = vehicle.Count + rBlock.Count;
        }

        private bool ListContainVerticalBlock(List<VerticalBlock> tempList, VerticalBlock v)
        {
            bool result = false;
            foreach (VerticalBlock t in tempList)
            {
                if (t.Blocks[0].Name == v.Name)
                {
                    result = true;
                }
            }
            return result;
        }

        private List<VerticalBlock> AddExtraBlock(RowBlock rBlock, List<VerticalBlock> vBlocks)
        {
            int length = rBlock.MaxLength - rBlock.Length;

            List<VerticalBlock> extraBlocks =
                vBlocks.Where(
                    v => (v.Length <= length & v.Width <= rBlock.Width) | (v.Length <= rBlock.Width & v.Width <= length))
                    .OrderByDescending(v => v.RealVolume)
                    .ToList();
            List<VerticalBlock> tempBlocks =
                vBlocks.Where(
                    v =>
                        ((v.Length <= length & v.Width <= rBlock.Width) | (v.Length <= rBlock.Width & v.Width <= length)) ==
                        false).ToList();
            if (extraBlocks.Count() + tempBlocks.Count() != vBlocks.Count())
            {
                MessageBox.Show("Ошибка при фильтрации блоков. AddExtraBlock");
            }
            if (extraBlocks.Any())
            {
                VerticalBlock extraBlock = extraBlocks[0];
                //MessageBox.Show("Подобрано дополнительных блоков: " + extraBlocks.Count().ToString());
                if (extraBlock.Length > (rBlock.MaxLength - rBlock.Length) | extraBlock.Width > rBlock.Width)
                {
                    extraBlock.RotateH();
                }

                rBlock.Add(extraBlock, vehicle.Width);

                tempBlocks.AddRange(extraBlocks.Where(v => v != extraBlock));
            }
            return tempBlocks;
        }
    }
}