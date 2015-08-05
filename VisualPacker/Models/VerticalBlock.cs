using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace VisualPacker.Models
{
   public class VerticalBlock: Container
    {
         public List<Container> Blocks=new List<Container>();
         public double RealVolume;
         public double MaxPress = 10000;

     public VerticalBlock()
     {
         Count = 0;
         DirLength = "a";
     }
      public override object Clone() 
      {
          VerticalBlock verticalBlock = new VerticalBlock
          {
              Count = Count,
              Length = Length,
              Width = Width,
              Height = Height,
              Priority = Priority,
              Kind = Kind,
              Name = Name,
              ContainerType = ContainerType,
              Mass = Mass,
              Price = Price,
              DirLength = DirLength,
              DirWidth = DirWidth,
              DirHeight = DirHeight,
              Order = Order,
              Color = Color,
              Only4Bottom = Only4Bottom,
              FirstPoint = new Point3D(FirstPoint.X, FirstPoint.Y, FirstPoint.Z)
          };
          foreach (Object obj in Blocks)
          {
              if (obj is VerticalBlock)
              {
                  VerticalBlock vBlock = (VerticalBlock)obj;
                  verticalBlock.Blocks.Add(vBlock.Clone() as VerticalBlock);
              }
              else if (obj is Container)
              {
                  Container cont = (Container)obj;
                  verticalBlock.Blocks.Add(cont.Clone() as Container);
              }
          }
          return verticalBlock;
      }
        public bool Add(Container c, int maxHeight)
        {
            if (
                (Height + c.Height) <= maxHeight &  (((Blocks.Count > 0) & (c.Only4Bottom == 1))) == false & (MaxPress>=c.Mass)) 
            {   if (c.DirLength != "a") //если направленность контейнера не подходит, то блок не добавляем
                 {
                     if (DirLength == "a") { DirLength = c.DirLength;}
                     else if (DirLength!=c.DirLength) {return false;} 
                 };
                if (Order != c.Order & Count != 0) { 
                    return false; 
                } //в одном блоке не может быть контейнеров из разных заказов
                Blocks.Add(c);
                Height = Height + c.Height;
                Length = Math.Max(Length, c.Length);
                Width = Math.Max(Width, c.Width);
                Mass = Mass + c.Mass;
                Price = Price + c.Price;
                MaxPress = Math.Min(MaxPress-c.Mass, c.PressHeight);
                Count=Count+c.Count;
                Order = c.Order;
                RealVolume = RealVolume + c.Volume;
                Priority = Math.Min(Priority, c.Priority);
                return true;
            }
            return false;
        }

       public List<Container> AddOneContainerFromList(List<Container> tempList,int maxHeight)
        {  
           int maxLength=Blocks[Blocks.Count()-1].Length;
           int maxWidth=Blocks[Blocks.Count()-1].Width;
           int oldCount=Blocks.Count();
           List<Container> sameList = tempList.Where(c => c.Length <= maxLength & c.Width <= maxWidth & c.Order == Order).OrderBy(s => s.Priority).ThenByDescending(s => s.Length).ThenByDescending(s => s.Price).ToList();
           tempList=tempList.Where(c => c.Length>maxLength |c.Width>maxWidth |c.Order!=Order).ToList();
           foreach (Container c in sameList ){
               if (oldCount==Blocks.Count())  {
                   if (Add(c, maxHeight) == false)
                   {
                       tempList.Add(c);}
       
               }
               else { tempList.Add(c); }
           }
           return tempList;
        }
        public bool AreSame(VerticalBlock v)
        {
            if (0.8*Width <= v.Width & 1.2*Width >= v.Width) return true;
            return false;
        }

       public bool AreSame01(VerticalBlock v)
       {
           if (0.9 * Width <= v.Width & 1.1 * Width >= v.Width) return true;
           return false;
       }

       public bool AreEqual(VerticalBlock v)
       {
           if (Width == v.Width & Length == v.Length) return true;
           return false;
       }

       public void SetFirstPointVerticalBlock(Point3D point)
        {
            FirstPoint = point;
            Point3D tempPoint = point;
            foreach (Object data in Blocks)
            {
                if (data is VerticalBlock)
                {
                    VerticalBlock v = (VerticalBlock)data;
                       v.SetFirstPointVerticalBlock(tempPoint);
                        tempPoint.Z = tempPoint.Z + v.Height;
                }
                else if (data is RowBlock)
                {
                    RowBlock r = (RowBlock)data;
                    r.SetFirstPointForVerticalBlock(tempPoint);
                    tempPoint.Z = tempPoint.Z + r.Height;
                }
                else if (data is HorizontalBlock)
                {
                    HorizontalBlock h = (HorizontalBlock)data;
                    h.FirstPoint=tempPoint;
                    tempPoint.Z = tempPoint.Z + h.Height;
                }
                else if (data is Container)
                {
                    Container c = (Container)data;
                    c.FirstPoint = tempPoint;
                    tempPoint.Z = tempPoint.Z + c.Height;
                }
                else
                {
                    MessageBox.Show("В процедуру рисования SetFirstPointForVerticalBlock класса VerticalBlock передан неверный тип данных:" + data.GetType());
                }
            }
        }
        public new void RotateH()
        {
            int temp = Length;
            Length = Width;
            Width = temp;
            foreach (Object data in Blocks)
            {
                if (data is VerticalBlock)
                {
                    VerticalBlock c = (VerticalBlock)data;
                    c.RotateH();
                }
                else if (data is RowBlock)
                {
                    RowBlock c = (RowBlock)data;
                    // ничего не делаем
                }
                else if (data is HorizontalBlock)
                {
                    HorizontalBlock c = (HorizontalBlock)data;
                    c.RotateH();
                }
                else if (data is Container)
                {
                    Container c = (Container)data;
                    c.RotateH();
                }
                else
                {
                    MessageBox.Show("В процедуру поворота вертикального блока передан неверный тип данных:" + data.GetType());
                }
               
            
            }

        }

        public override void ToContainerList(List<Container> tempList)
        {
            FromTempListToContList fromTempListToContList = new FromTempListToContList();
            fromTempListToContList.ToContainerList(tempList, Blocks);
        }

        public void  ToContainerListIncludeVerticalPallet(List<Container> tempList)
        {
            FromTempListToContList fromTempListToContList = new FromTempListToContList();
           foreach (Object data in Blocks)
           {   
                if (data is VerticalBlock)
                {   
                   VerticalBlock v=(VerticalBlock)data;
                       v.ToContainerList(tempList);
                }
                else if (data is RowBlock)
                {
                   RowBlock r =(RowBlock)data;
                   fromTempListToContList.ToContainerList(tempList, r.Blocks); 
                }
                else if (data is HorizontalBlock)
                {
                    HorizontalBlock c = (HorizontalBlock)data;
                    c.ToContainerList(tempList);
                }
                else if (data is Container)
                {
                    Container c=(Container)data;
                    c.ToContainerList(tempList);
                }
                else
                {
                    MessageBox.Show("В процедуру выгрузки контейнеров класса VerticalBlock передан неверный тип данных:" + data.GetType());
                }
           }
        }
    }
}
