using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}