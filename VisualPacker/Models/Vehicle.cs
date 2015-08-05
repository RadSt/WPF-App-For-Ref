using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using VisualPacker.ViewModels;
using VisualPacker.ViewModels.Helpers;

namespace VisualPacker.Models
{
    public class Vehicle : ICloneable
    {
        private List<Container> wasteList = new List<Container>();
        const int minHeighCont=800;
        public Vehicle()
        {
            Length = 6000;
            Width = 2400;
            Height = 2400;
            MaxHeight = 0;
            Tonnage = 24000;
            FullTonnage = 24000;
            Mass = 0;
            Count = 0;
            DoorWidth = 2000;
            DoorHeight = 2000;
            DoorCenterDistance = 0;
            Roof = 200;
        }

        public int MaxLength { get; set; }
        public string Name { get; set; }
        public string Kind { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        private int MaxHeight { get; set; }
        public int Tonnage { get; set; }
        public int FullTonnage { get; set; }

        public int EmptyTonnage
        {
            get { return FullTonnage - Tonnage; }
        }

        public double Mass { get; set; }
        public List<RowBlock> Blocks = new List<RowBlock>();
        public List<Container> SmallBlocks = new List<Container>();
        public int Count { get; set; }
        /* //-- Параметры передней оси 
        private int front_axle_distance = 1600;
        public int Front_axle_distance { get { return front_axle_distance; } set { front_axle_distance = value; } } //Расстояние от оси, до начала грузового отсека
        private int front_axle_max_tonnage = 16000;
        public int Front_axle_max_tonnage { get { return front_axle_max_tonnage; } set { front_axle_max_tonnage = value; } }
        private int front_axle_empty_tonnage = 8900;
        public int Front_axle_empty_tonnage { get { return front_axle_empty_tonnage; } set { front_axle_empty_tonnage = value; } }
        private double front_axle_current_tonnage = 0;
        public double Front_axle_current_tonnage { get { return front_axle_current_tonnage; } set { front_axle_current_tonnage = value; } }
        //!-- Параметры задней оси -->
        private int back_axle_distance = 2000;
        public int Back_axle_distance { get { return back_axle_distance; } set { back_axle_distance = value; } } //Расстояние от оси, до конца грузового отсека
        private double back_axle_max_tonnage = 24000;
        public double Back_axle_max_tonnage { get { return back_axle_max_tonnage; } set { back_axle_max_tonnage = value; } }
        private double back_axle_empty_tonnage = 5700;
        public double Back_axle_empty_tonnage { get { return back_axle_empty_tonnage; } set { back_axle_empty_tonnage = value; } }
        private double back_axle_current_tonnage = 0;
        public double Back_axle_current_tonnage { get { return back_axle_current_tonnage; } set { back_axle_current_tonnage = value; } }
        private int back_axle_count = 1;
        public int Back_axle_count { get { return back_axle_count; } set { back_axle_count = value; } }*/
        //-- Положение двери на боковой стенке вагона (вагон грузится от двери к краям). -->
        public int DoorWidth { get; set; } // Ширина двери. -->
        public int DoorHeight { get; set; } //- Высота двери. -->
        public int DoorCenterDistance { get; set; } // Смещение двери относительно центра вагона. -->
        public int Roof { get; set; } // Высота скругленного свода (крыши) вагона в центре. -->
        public Point3D FirstPoint { get; set; } //положение левого нижнего угла

        public object Clone()
        {
            Vehicle vehicle = new Vehicle
            {
                Name = Name,
                Kind = Kind,
                Length = Length,
                Width = Width,
                Height = Height,
                Tonnage = Tonnage,
                Mass = Mass,
                DoorWidth = DoorWidth,
                DoorHeight = DoorHeight,
                DoorCenterDistance = DoorCenterDistance,
                Roof = Roof
            };
            return vehicle;
        }

        public bool NotEmpty()
        {
            return Blocks.Any();
        }

        public void SetFirstPoint(Point3D point)
        {
            //присваиваем начальные координаты для груза
            FirstPoint = point;
            Point3D tempPoint = point;
            foreach (RowBlock r in Blocks)
            {
                Point3D pointM = Calculation.CalculateMassCenterRow(r);
                //if () {r.Blocks.Reverse();}
                r.SetFirstPointForVerticalBlock(tempPoint);
                tempPoint.X = tempPoint.X + r.Width;
            }
        }

        public int GetArea()
        {
            int area = Length*Width;
            return area;
        }

        public double Volume()
        {
            double l = Length/1000;
            double w = Width/1000;
            double h = Height/1000;
            return (l*w*h);
        }

        public List<int> DistinctOrders(List<Container> containers)
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

        public List<int> DistinctOrders(List<RowBlock> rowBlocks)
        {
            List<int> orderList = new List<int>();
            foreach (RowBlock r in rowBlocks)
            {
                if (!orderList.Contains(r.Order))
                {
                    orderList.Add(r.Order);
                }
            }
            return orderList;
        }

        public List<int> DistinctOrdersInRow(List<RowBlock> rBlocks)
        {
            List<int> orderList = new List<int>();

            foreach (RowBlock r in rBlocks)
            {
                if (!orderList.Contains(r.Order))
                {
                    orderList.Add(r.Order);
                }
            }
            return orderList;
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

        private int VerticalBlocksDistinctCount(List<VerticalBlock> Data)
        {
            // нужна для проверки потери вертикальных контейнеров в процедуре CreateRows
            List<VerticalBlock> tempList = new List<VerticalBlock>();
            foreach (VerticalBlock c in Data)
            {
                if (ListContainVerticalBlock(tempList, c) == false)
                {
                    tempList.Add(c);
                }
            }
            return tempList.Count();
        }

        public void CreateRows(List<VerticalBlock> vBlocks, int sameWidth, List<RowBlock> rBlocks)
        {
            List<VerticalBlock> sameBlock =
                vBlocks.Where(s => s.IsSutableWidth(sameWidth)).OrderByDescending(s => s.Width).ToList();
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
                    if (tempS.Length > Width)
                    {
                        /*пропускаем блок т.к. он не подходит по ширине и длине*/
                    }
                    else if (rBlock.Add(tempS, Width) == false)
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

        public List<Container> CreateRow3(List<Container> containers, int minHeight, int maxHeight, int maxWidth,
            int show, int maxTonnage)
        {
            //первым делом создаем вертикальные блоки нужной высоты (не меньше minHeight;  
            List<VerticalBlock> vBlocks = new List<VerticalBlock>();
            List<Container> tempList = containers.OrderByDescending(c => c.Length).ToList();
            List<Container> smallContainers = new List<Container>();
            List<VerticalBlock> lowBlocks = new List<VerticalBlock>();
            RowBlock result;
            //поворачиваем контейнеры которые должны стоять вдоль борта
            foreach (Container c in tempList)
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
                Container firstCont = tempList[0];
                int check4 = tempList.Count();
                List<Container> sameCont1 =
                    tempList.Where(s => s.AreSame01(firstCont))
                        .OrderByDescending(s => s.Mass)
                        .ThenBy(s => s.Color)
                        .ToList();
                tempList = tempList.Where(s => (s.AreSame01(firstCont) == false)).OrderBy(s => s.Color).ToList();
                if (sameCont1.Count() + tempList.Count() != check4)
                {
                    MessageBox.Show("неправильная фильтрация контейнеров");
                }
                VerticalBlock vBlock = new VerticalBlock();
                foreach (Container c in sameCont1)
                {
                    if (c.Height < Height & c.Height >= maxHeight & vBlock.Count == 0)
                    {
                        //если контейнер выше чем нужно но меньше макс. высота, то добавляем его в отдельный вертикальный блок
                        if (vBlock.Add(c, Height) == false)
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
            //ReportWindow win = new ReportWindow(vBlocks);
            //win.ShowDialog();
            //формируем ряды
            List<RowBlock> rBlocks = new List<RowBlock>();
            int sameWidth = 100;
            int sh = vBlocks.Count();
            while (sameWidth < MaxLength)
            {
                CreateRows(vBlocks, sameWidth, rBlocks);
                sameWidth = sameWidth + 100;
            }
            int sh2 = VerticalBlocksDistinctCount(vBlocks);
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

                if ((result.FullnessWidth > 0.4 | containers.Count() == 1) & (Mass + result.Mass) <= maxTonnage)
                    // if (result.FullnessWidth > 0.4 | containers.Count() <= 5)
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
            foreach (VerticalBlock v in vBlocks)
            {
                v.ToContainerList(tempList);
            }
            //возвращаем маленькие контейнеры в общий список
            tempList.AddRange(smallContainers);
            //Поворачиваем контейнеры обратно;
            foreach (Container c in tempList.Where(c => c.Length < c.Width))
            {
                c.RotateH();
            }
            return tempList;
        } //CreateRow3


        public List<VerticalBlock> AddExtraBlock(RowBlock rBlock, List<VerticalBlock> vBlocks)
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

                rBlock.Add(extraBlock, Width);

                tempBlocks.AddRange(extraBlocks.Where(v => v != extraBlock));
            }
            return tempBlocks;
        }

        public List<Container> CreateVerticalPalletsLevel2(List<Container> containers, int maxHeight)
        {
            List<Container> t1 = Calculation.ListToContainerList(containers);
            //используется для контроля потери контейнеров
            //контейнеры которые штабелируются в два яруса и ставятся только на пол
            List<Container> tempOnly4Bottom =
                containers.Where(c => c.Only4Bottom == 2).OrderBy(c => c.ContainerType).ToList();
            List<Container> tempList = containers.Where(c => c.Only4Bottom != 2).ToList();

            int nameCh = 0;

            while (tempOnly4Bottom.Any())
            {
                VerticalBlock vBlock = new VerticalBlock();
                vBlock.Kind = "VerticalPallet";
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
            List<Container> t2 = Calculation.ListToContainerList(tempList);
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

        public List<Container> CreateVerticalPalletsLevel3(List<Container> containers, int length, int width, int height,
            int maxHeight, string prefix)
        {
            List<Container> t1 = Calculation.ListToContainerList(containers);
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
                VerticalBlock vBlock = new VerticalBlock {Kind = "VerticalPallet", PressHeight = 0};
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

            List<Container> t2 = Calculation.ListToContainerList(tempList);
            //используется для контроля потери контейнеров
            if (t1.Count() != t2.Count())
            {
                MessageBox.Show("Потеря контейнеров в процедуре CreateVerticalPalletsLevel3");
            }
            return tempList;
        }


        public List<Container> DownloadContainers(List<Container> containers, int maxTonnage)
        {
            MaxHeight = Height - 350;
            List<Container> tempList = new List<Container>();
            while (CannotDownloadAll(containers, maxTonnage) == false & MaxHeight > minHeighCont)
            {
                MaxHeight = MaxHeight - 100;
            }
            MaxHeight = Math.Min(Height - 350, MaxHeight + 100);
            tempList = DownloadContainersToVehicle(containers, maxTonnage);
            return tempList;
        }

        private bool CannotDownloadAll(List<Container> containers, int maxTonnage)
        {
            List<Container> tempList = DownloadContainersToVehicle(containers, maxTonnage);
            if (NotEmpty(tempList) & MaxLength < 500)
            {
                return true;
            }
            return false;
        }

        private bool NotEmpty(List<Container> tempList)
        {
            if (tempList.Any())
            {
                return true;
            }
            return false;
        }

        private List<Container> DownloadContainersToVehicle(List<Container> containers, int maxTonnage)
        {
            ClearVehicle();
            MaxLength = Length;
            //делим тары на вертикальные блоки 
            List<VerticalBlock> vBlocks = new List<VerticalBlock>();
            //разные заказы обрабатываем отдельно т.к. их нужно будет выгружать из машины в разное время

            //Негабаритный товар отсекаем 
            List<Container> containerList = containers.Where(s => s.IsSutableLength(Width) == true).ToList();
            wasteList = containers.Where(s => s.IsSutableLength(Width) == false).ToList();
            List<Container> wasteList2 = containerList.Where(s => s.Height >= MaxHeight).ToList();
            containerList = containerList.Where(s => s.Height <= MaxHeight).ToList();
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
                            minHeight = Blocks[Blocks.Count() - 1].MinHeight;
                        }
                        tempList = CreateRow3(tempList, minHeight, MaxHeight, Width, 0, maxTonnage);
                    }
                }

                //контейнеры которые штабелируются в два яруса и ставятся только на пол
                tempList = CreateVerticalPalletsLevel2(tempList, MaxHeight);
                //отбираем контейнеры которые штабелируются в 3 яруса и ставятся только на пол
                tempList = CreateVerticalPalletsLevel3(tempList, 800, 400, 390, MaxHeight, "1-");
                tempList = CreateVerticalPalletsLevel3(tempList, 800, 400, 500, MaxHeight, "2-");
                tempList = CreateVerticalPalletsLevel3(tempList, 900, 360, 420, MaxHeight, "3-");
                tempList = CreateVerticalPalletsLevel3(tempList, 890, 570, 540, MaxHeight, "4-");

                //Обрабатываем остальные контейнеры
                if (NotEmpty())
                {
                    minHeight = Blocks[Blocks.Count() - 1].MinHeight;
                } //если есть ряды, то смотрим высоту последнего
                else
                {
                    minHeight = Height - 500;
                } //если нет, то берем максимальную высоту кузова
                while (tempList.Any() & minHeight > 0 & MaxLength > 0)
                {
                    int ch2 = Blocks.Count();
                    tempList = CreateRow3(tempList, minHeight, MaxHeight, Width, 0, maxTonnage);
                    if (ch2 == Blocks.Count())
                    {
                        minHeight = minHeight - 100;
                    }
                }
                ////////////////////////////////////
                //пытаемся доложить контейнеры сверху на ряды


                /////////////////////////////////////
                wasteList.AddRange(tempList);
            }
            wasteList = Calculation.ListToContainerListIncludeVerticalPallet(wasteList);
            wasteList = AddOnTopRow(wasteList);

            LoadSmallContainersBySquare(tempSmall, orderList);
            return wasteList;
        }

        private void ClearVehicle()
        {
            Blocks.Clear();
            SmallBlocks.Clear();
            wasteList.Clear();
            Count = 0;
            Mass = 0;
        }

        private List<Container> ProcessingCabin(List<Container> tempList, int maxTonnage)
        {
            List<Container> tempCabin = tempList.Where(s => s.Only4Bottom == 1).OrderByDescending(s => s.Mass).ToList();
            tempList = tempList.Where(s => s.Only4Bottom != 1).ToList();

            foreach (Container c in tempCabin)
            {
                if (c.Length > Width)
                {
                    if (c.Width <= Width & c.DirLength == "a")
                    {
                        c.RotateH();
                    }
                    else
                    {
                        MessageBox.Show("Кабина " + c.Name + " превышает габариты кузова.Сообщите администратору");
                        break;
                    }
                }
                if (Mass + c.Mass <= maxTonnage)
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

        ////////////////////////////////////////////////////////////////////////////////////
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
            if (hBlock.NotEmpty() & (Mass + hBlock.Mass) <= maxTonnage)
            {
                vBlock.Add(hBlock, Height - 250);
            }
            else
            {
                hBlock.ToContainerList(tempList);
            }

            foreach (Container r in tempRama)
            {
                if ((Mass + r.Mass) <= maxTonnage)
                {
                    if (vBlock.Add(r, Height - 350) == false)
                    {
                        MessageBox.Show("не удалось добавить раму в вертикальный блок.Сообщите администратору");
                        tempList.Add(r);
                    }
                }
            }
            RowBlock rBlock = new RowBlock();
            rBlock.Add(vBlock, Width);
            AddRowToVehicle(rBlock);


            return tempList;
        }

        private void AddRowToVehicle(RowBlock rBlock)
        {
            Blocks.Add(rBlock);
            MaxLength = MaxLength - rBlock.Width;
            Mass = Mass + rBlock.Mass;
            Count = Count + rBlock.Count;
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
            rBlock.Add(vBlock, MaxLength);

            if (MaxLength >= rBlock.Width & Tonnage >= rBlock.Mass + Mass)
            {
                AddRowToVehicle(rBlock);
            }
            else
            {
                //помещаем кабину в список невместившихся контейнеров
                wasteList.Add(c);
            }
        }

        private List<Container> AddOnTopRow(List<Container> tempList)
        {
            List<Container> returnList = tempList.ToList();
            foreach (RowBlock r in Blocks)
            {
                foreach (VerticalBlock v in r.Blocks)
                {
                    int oldCount = returnList.Count();
                    returnList = v.AddOneContainerFromList(returnList, MaxHeight);
                    if (oldCount == returnList.Count() + 1)
                    {
                        Count = Count + 1;
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
                foreach (RowBlock r in Blocks)
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
                int tempS = Width*orderLength;

                foreach (Container c in tempSmall)
                {
                    if (tempS > 0)
                    {
                        SmallBlocks.Add(c);
                        tempS = tempS - c.Width*c.Length;
                    }
                    else
                    {
                        tempList.Add(c);
                    }
                }
            }
            foreach (Container s in SmallBlocks)
            {
                Mass = Mass + s.Mass;
            }
            Count = Count + SmallBlocks.Count();
            wasteList.AddRange(tempList);
        }

        public Point3D GetMassCenter()
        {
            Point3D point = new Point3D(0, 0, 0);
            double nLength = 0;
            double nWidth = 0;
            double nHeight = 0;
            foreach (RowBlock r in Blocks)
            {
                Point3D pointRow = Calculation.CalculateMassCenterRow(r);
                nLength = nLength + r.Mass*pointRow.Y;
                nWidth = nWidth + r.Mass*pointRow.X;
                nHeight = nHeight + r.Mass*pointRow.Z;
            }
            point.Y = nLength/Mass;
            point.X = nWidth/Mass;
            point.Z = nHeight/Mass;
            return point;
        }

        public List<Container> VehicleToContainerList()
        {
            FromTempListToContList fromTempListToContList = new FromTempListToContList();
            List<Container> tempList = new List<Container>();
            fromTempListToContList.ToContainerList(tempList, Blocks);
            tempList.AddRange(SmallBlocks);
            return tempList;
        }
    }
}