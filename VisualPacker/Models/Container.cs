using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace VisualPacker.Models
{
    public class Container : ICloneable
    {
        private int priority; //приоритет загрузки

        private String color; //Цвет предмета при отображении на схеме укладки (в HTML формате) 

        //параметры контейнера
        public int Length { get; set; }  //длина
        public int Width { get; set; }  //ширина
        public virtual int Height { get; set; }  //высота

        public int Count { get; set; } 
        //количество контейнеров в данном объекте(Если kind=box то 1, для комбинированных объектов может быть больше одного)

        public string Kind { get; set; } 
        //box - обыкновенный прямоугольный ящик с тремя линейными размерами,cyl - цилинлр (бочка или кега) определяемая диамтром и высотой (пример ниже) 

        public string ShipToName { get; set; }  //Название ящика - не более 1000 симовлов 
        public string ShipmentId { get; set; }  //Название ящика - не более 1000 симовлов 
        public string Name { get; set; }  //Название ящика - не более 1000 симовлов 
        public string ShortName { get; set; }  //Название ящика - не более 1000 симовлов 
        public string ContainerType { get; set; }
        public double Mass { get; set; }  //массa
        public double Price { get; set; }  //Стоимость
        public bool IsChecked { get; set; }  //Стоимость

        public string DirLength { get; set; }
        public string DirWidth { get; set; }
        public string DirHeight { get; set; } 
        /* Ограничение на ориентацию ящика в транспортном средстве 
        Данный раздел определяет вдоль каких направлений могут быть ориентированы линейные размеры ящика 
        - a - вдоль любого направления   -->
        - x - ТОЛЬКО вдоль направления X (вдоль длины ТС)  -->
        - у - ТОЛЬКО вдоль направления Y (вдоль ширины ТС)  -->
        - z - ТОЛЬКО вдоль направления Z (вдоль высоты ТС)  -->
        В частности, для того чтобы ящик не кантовался при укладке надо указать значение z для параметра height.*/

        public int Order { get; set; } 
        /* Порядок загрузки. Если в списке ящиков есть ящики с разным порядком, то  сначала будут загужены ящики с порядком 0, затем с порядком 1, и т.п.  
        Данная функциональность используется в случае, когда ТС загружается или разгруажается в нескольких точках.  
        В этом случае алгоритм предложит такую схему укладки, при которой сначала можно загрузить ящики порядка 0,  
        затем, в следующей точке, дозагрузить к ним ящики порядка 1 и так далее */

        public string GroupName { get; set; }  //Название группы 
        public int GroupId { get; set; }  //Иденттификатор группы, должен быть одним и тем же для всех ящиков комплекта 
        /* Группы предмета.  -->
        В некоторых случаях, определенные наборы ящиков определяют некий комплект. Например, когда перевозится мебель в разобранном состоянии, 
        все составные части одного предмета мебели составлют комплект (или группу). При это в каждой ТС должно быть обязатльно загружено целое число комплектов. 
        То есть недопустимо, чтобы часть составных частей одного предмета медели вошли в одно ТС, а часть в другое. 
        Для того чтобы алгоритм вычислят схему укладки, в кторой все ящики одного коплекта обязательно входят в одно ТС, для ящиков такого типа надо определить параметры группы 
       <group>*/

        public int GroupQuantity { get; set; } 
        /* Количество ящиков такого типа в комлекте Например, в комплект шкафа могут входить 3 одинкаовых ящика "полки", 
        при этом количество самих комплектов может быть больше 1, например, 4. Тогда данное значение должно быть равно 3, 
        а параметр quantity (полное количество ящиков) 3*4=12  */

        public double PressLength { get; set; }  //Допустимое давление на яшик, если его длина ориентирована вертикально. 
        public double PressWidth { get; set; }  //Допустимое давление на яшик, если его ширина ориентирована вертикально. 
        public double PressHeight { get; set; }  // Допустимое давление на яшик, если его высота ориентирована вертикально. 
        /*Допустимое давление на яшик сверху, если он ориентирован указанным линейным размером вертикально. 
        Когда ящик хрупкий, и нельзя чтобы на него было поставлено много тяжелых ящиков, надо этим параметром ограничивать максимально допустимое давление. 
        Однако, подбирая наиболее удачное и оптимальное положение ящика, алгоритм может переворачивать его.  И для разных ориентаций ящика, может быть указано разное ограничение. 
        <!--  Если параметр не указан - ограничение отсутсвует. */

        public int FragilityLength { get; set; }  //хрупкость ящика, если его длина ориентирована вертикально.
        public int FragilityWidth { get; set; }  //хрупкость ящика, если его ширина ориентирована вертикально. 
        public int FragilityHeight { get; set; }  //хрупкость ящика, если его высота ориентирована вертикально. 
        /* Хрупкость ящика:  -->
        - 0 - означает, что ящик не имеет ограничений по хрупкости (установки на него сверху других предметов), 
        - 1 - означает, что никакие ящики не могут быть установлены сверху данного ящика,
        - N>1 -означает, что не более чем (N-1) ящиков такого же ти могут быть установлены сверху данного ящика.*/

        public int Freezable { get; set; }
        /* Замерзаемость ящика (морозостоякость)   -->
        В некоторых случаях, когда речь идет о перевозки продуктов, в зимнее время года, определенные ящики надо ставить подальше от двери, чтобы они не замерзли. 
        Этот параметр определяет требования к установке ящика подальше от двери, 0 - нет ограничение на установку, 1 - ящик чуствителен к морозу. */

        public int PalQuantity { get; set; }
        /* Количество ящиков на паллете.  -->
        Парамерт определяет количество ящков такого типа на паллетах, хранящихся на складе. 
        Данный параметр используется в методе рядной загрузки, когда мы должны укалдывать ящики последовательно, 
        причем каждый раз использовать такое кочлиество, которое кратное количеству ящиков на паллете. 
        Это надо для того, чтобы гузчики последовательног гурзили паллеты поступающие со склада, а не откладывали 
        с них ящики "на потом". Замечание: этот параметр никак не связан с укладкой ящиков на паллетах. */

        public int Level { get; set; }
        /*  Уровень укладки.  
        Это свойство является альтернативой всяким максимальным давлениеям на грань и 
        определяет какие предметы могут быть сверху каких.  
        Предметы 0-вого уровня могут быть в любом месте и допускают установку сверху любых предметов. 
        На предметы уровня N можно ставить только предметы уровня N и выше. */

        public int Quantity { get; set; }// Количество предметов такого типа в заказе на расчет 
        public int Only4Bottom { get; set; }
        public Point3D FirstPoint { get; set; } //положение левого нижнего угла

        public Container()
        {
            Count = 1;
            DirLength = "a";
            DirWidth = "a";
            DirHeight = "a";
            priority = 1;
            Order = 0;
            PressLength = 10000;
            PressWidth = 10000;
            PressHeight = 10000;
        }

        public virtual int Priority { get { return priority; }  set { priority = value; } } 

        public virtual string PriorityString
        {
            get
            {
                if (Priority == 0)
                {
                    return "Срочный";
                }
                return "Обычный";
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                PriorityString = value;
            }
        }


        public String Color
        {
            get
            {
                if (color == null)
                {
                    return "Blue";
                }
                return color.Replace("#", "").Replace("%", "");
            }
            set { color = value; }
        }


        public virtual int Area
        {
            get { return Length*Width; }
        }

        public double Volume
        {
            get { return Length*Width*Height; }
        }


        public String Vgh
        {
            get { return Length + "x" + Width + "x" + Height; }
        }

        public bool AreSame(Container c)
        {
            if ((Length <= c.Length & Length >= 0.7 * c.Length) & (Width <= c.Width & Width >= 0.7 * c.Width)) return true;
            return false;
        }

        public bool AreSame01(Container c)
        {
            if ((Length <= c.Length & Length >= 0.8 * c.Length) & (Width <= c.Width & Width >= 0.8 * c.Width)) return true;
            return false;
        }

        public bool IsSutableLength(double maxLength)
        {
            if (Length <= maxLength | (Width <= maxLength & DirLength != "y")) return true;
            return false;
        }

        public bool IsSutableWidth(double maxWidth)
        {
            if (Length >= Width)
            {
                if ((Length <= maxWidth & Length > maxWidth - 100 & DirLength != "y") |
                    (Width <= maxWidth & Width > maxWidth - 100 & DirLength != "x")) return true;
                return false;
            }
            if ((Length <= maxWidth & Length > maxWidth - 100 & DirLength != "x") |
                (Width <= maxWidth & Width > maxWidth - 100 & DirLength != "y")) return true;
            return false;
        }

        public virtual void ToContainerList(List<Container> tempList)
        {
            tempList.Add(this);
        }

        public bool AreEqual(Container c)
        {
            if (Length == c.Length & Width == c.Width & Height == c.Height) return true;
            return false;
        }

        public void RotateH()
        {
            int temp = Length;
            Length = Width;
            Width = temp;
        }

        public virtual object Clone()
        {
            Container container = new Container
            {
                Count = Count,
                Length = Length,
                Width = Width,
                Height = Height,
                Priority = Priority,
                Kind = Kind,
                Name = Name,
                ShipToName = ShipToName,
                ShipmentId = ShipmentId,
                ContainerType = ContainerType,
                Mass = Mass,
                Price = Price,
                DirLength = DirLength,
                DirWidth = DirWidth,
                DirHeight = DirHeight,
                Order = Order,
                GroupName = GroupName,
                GroupId = GroupId,
                GroupQuantity = GroupQuantity,
                PressLength = PressLength,
                PressWidth = PressWidth,
                PressHeight = PressHeight,
                FragilityLength = FragilityLength,
                FragilityWidth = FragilityWidth,
                FragilityHeight = FragilityHeight,
                Freezable = Freezable,
                Level = Level,
                Color = Color,
                Quantity = Quantity,
                Only4Bottom = Only4Bottom,
                FirstPoint = new Point3D(FirstPoint.X, FirstPoint.Y, FirstPoint.Z),
                IsChecked = IsChecked
            };
            return container;
        }
    }
}