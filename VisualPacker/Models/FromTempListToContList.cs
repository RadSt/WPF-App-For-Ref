using System;
using System.Collections.Generic;
using System.Windows;

namespace VisualPacker.Models
{
    public class FromTempListToContList
    {
        public void ToContainerList<T>(List<Container> tempList, List<T> blocks)
        {
            foreach (Object data in blocks)
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
                        verticalBlock.ToContainerList(tempList);
                    }
                }
                else if (data is RowBlock)
                {
                    RowBlock rowBlock = (RowBlock)data;
                    ToContainerList(tempList, rowBlock.Blocks);
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
                    MessageBox.Show("В процедуру выгрузки контейнеров класса RowBlock передан неверный тип данных:" + data.GetType());
                }
            }
        }

        public List<Container> ToContainerList<T>(List<T> blocks)
        {
            var tempList = new List<Container>();
            var fromTempListToContList = new FromTempListToContList();
            foreach (Object data in blocks)
            {
                if (data is VerticalBlock)
                {
                    var verticalBlock = (VerticalBlock)data;
                    verticalBlock.ToContainerList(tempList);
                }
                else if (data is RowBlock)
                {
                    var rowBlock = (RowBlock)data;
                    fromTempListToContList.ToContainerList(tempList, rowBlock.Blocks);
                }
                else if (data is HorizontalBlock)
                {
                    var horizontalBlock = (HorizontalBlock)data;
                    horizontalBlock.ToContainerList(tempList);
                }
                else if (data is Container)
                {
                    var container = (Container)data;
                    container.ToContainerList(tempList);
                }
                else
                {
                    MessageBox.Show(
                        "В процедуру выгрузки контейнеров класса VerticalBlock передан неверный тип данных:" +
                        data.GetType());
                }
            }
            return tempList;
        }
    }
}