using System;
using System.Collections.Generic;

namespace VisualPacker.Models
{
    internal class Pallet : Container
    {
        private int area;
        private int heightPallet; //высота паллеты
        private int heightContainers; //высота    

        public override int Height
        {
            get { return heightPallet + heightContainers; }
        }

        public List<Container> Blocks = new List<Container>();

        public Pallet()
        {
            Count = 0;
            Kind = "Pallet";
            Length = 1200;
            Color = "White";
            Only4Bottom = 0;
            Width = 800;
            Name = "комбинированная паллета";
            Mass = 0;
            Price = 0;
            PressLength = 0; //Допустимое давление на яшик, если его длина ориентирована вертикально. 
            PressWidth = 0;
            PressHeight = 100;
            FragilityLength = 1;
            FragilityWidth = 1;
            FragilityHeight = 1;
            Freezable = 0;
            Level = 0;
            heightPallet = 150;
            area = 0;
        }

        public bool Add(Container c)
        {
            if (0.9*Length*Width >= (area + c.Area) & (Order == c.Order | Blocks.Count == 0))
            {
                Blocks.Add(c);
                Count = Count + 1;
                area = area + c.Area;
                Mass = Mass + c.Mass;
                heightContainers = Math.Max(heightContainers, c.Height);
                Order = c.Order;
                Price = Price + c.Price;
                return true;
            }
            return false;
        }
    }
}