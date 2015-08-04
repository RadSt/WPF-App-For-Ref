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
          foreach (Object o in Blocks)
          {
              if (o is VerticalBlock)
              {
                  VerticalBlock vBlock = (VerticalBlock)o;
                  verticalBlock.Blocks.Add(vBlock.Clone() as VerticalBlock);
              }
              else if (o is RowBlock)
              {
                  RowBlock rBlock = (RowBlock)o;
                  MessageBox.Show("Ошибка. Клонирование рядов не выполнено");
              }
              else if (o is Container)
              {
                  Container cont = (Container)o;
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
           List<Container> SameList = tempList.Where(c => c.Length <= maxLength & c.Width <= maxWidth & c.Order == Order).OrderBy(s => s.Priority).ThenByDescending(s => s.Length).ThenByDescending(s => s.Price).ToList();
           tempList=tempList.Where(c => c.Length>maxLength |c.Width>maxWidth |c.Order!=Order).ToList();
           foreach (Container c in SameList ){
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

       public void SetFirstPoint(Point3D point)
        {
            FirstPoint = point;
            Point3D tempPoint = point;
            foreach (Object Data in Blocks)
            {
                if (Data is VerticalBlock)
                {
                    VerticalBlock v = (VerticalBlock)Data;
                       v.SetFirstPoint(tempPoint);
                        tempPoint.Z = tempPoint.Z + v.Height;
                }
                else if (Data is RowBlock)
                {
                    RowBlock r = (RowBlock)Data;
                    r.SetFirstPoint(tempPoint);
                    tempPoint.Z = tempPoint.Z + r.Height;
                }
                else if (Data is HorizontalBlock)
                {
                    HorizontalBlock h = (HorizontalBlock)Data;
                    h.FirstPoint=tempPoint;
                    tempPoint.Z = tempPoint.Z + h.Height;
                }
                else if (Data is Container)
                {
                    Container c = (Container)Data;
                    c.FirstPoint = tempPoint;
                    tempPoint.Z = tempPoint.Z + c.Height;
                }
                else
                {
                    MessageBox.Show("В процедуру рисования SetFirstPoint класса VerticalBlock передан неверный тип данных:" + Data.GetType());
                }
            }
        }
        public new void RotateH()
        {
            int temp = Length;
            Length = Width;
            Width = temp;
            foreach (Object Data in Blocks)
            {
                if (Data is VerticalBlock)
                {
                    VerticalBlock c = (VerticalBlock)Data;
                    c.RotateH();
                }
                else if (Data is RowBlock)
                {
                    RowBlock c = (RowBlock)Data;
                    // ничего не делаем
                }
                else if (Data is HorizontalBlock)
                {
                    HorizontalBlock c = (HorizontalBlock)Data;
                    c.RotateH();
                }
                else if (Data is Container)
                {
                    Container c = (Container)Data;
                    c.RotateH();
                }
                else
                {
                    MessageBox.Show("В процедуру поворота вертикального блока передан неверный тип данных:" + Data.GetType());
                }
               
            
            }

        }

        public override void ToContainerList(List<Container> tempList)
        {
            FromTempListToContList fromTempListToContList = new FromTempListToContList();
            foreach (Object data in Blocks)
            {
                if (data is VerticalBlock)
                {
                    VerticalBlock verticalBlock = (VerticalBlock)data;
                    if (verticalBlock.Kind == "VerticalPallet")
                    {
                        tempList.Add(verticalBlock);
                    }
                    else
                    {
                        ToContainerList(tempList);
                    }
                }
                else if (data is RowBlock)
                {
                    RowBlock rowBlock = (RowBlock)data;
                    fromTempListToContList.ToContainerList(tempList, rowBlock.Blocks);
                }
                else if (data is HorizontalBlock)
                {
                    HorizontalBlock horizontalBlock = (HorizontalBlock)data;
                    horizontalBlock.ToContainerList(tempList);
                }
                else if (data is Container)
                {
                    Container container = (Container)data;
                    container.ToContainerList(tempList);
                }
                else
                {
                    MessageBox.Show("В процедуру выгрузки контейнеров класса VerticalBlock передан неверный тип данных:" + data.GetType());
                }

                //if (data is VerticalBlock)
                //{
                //    VerticalBlock verticalBlock = (VerticalBlock)data;
                //    if (verticalBlock.Kind == "VerticalPallet")
                //    {
                //        tempList.Add(verticalBlock);
                //    }
                //    else
                //    {
                //        ToContainerList(tempList);
                //    }
                //}
                //else
                //{
                //    fromTempListToContList.ToContainerList(tempList, Blocks);
                //}
                
            }
        }
           public void  ToContainerListIncludeVerticalPallet(List<Container> tempList)
        {
            FromTempListToContList fromTempListToContList = new FromTempListToContList();
           foreach (Object Data in Blocks)
           {   
                if (Data is VerticalBlock)
                {   
                   VerticalBlock v=(VerticalBlock)Data;
                       v.ToContainerList(tempList);
                }
                else if (Data is RowBlock)
                {
                   RowBlock r =(RowBlock)Data;
                   fromTempListToContList.ToContainerList(tempList, r.Blocks); 
                }
                else if (Data is HorizontalBlock)
                {
                    HorizontalBlock c = (HorizontalBlock)Data;
                    c.ToContainerList(tempList);
                }
                else if (Data is Container)
                {
                    Container c=(Container)Data;
                    c.ToContainerList(tempList);
                }
                else
                {
                    MessageBox.Show("В процедуру выгрузки контейнеров класса VerticalBlock передан неверный тип данных:" + Data.GetType());
                }
           }
        }
    }
}
