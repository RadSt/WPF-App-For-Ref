   M S B A M L   `   `   `  ���� I  E3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null4'clr-namespace:_3DTools;assembly=3DTools_3DTools   VisualPacker   VisualPacker.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6dg-http://schemas.microsoft.com/wpf/2008/toolkit  4tool3d'clr-namespace:_3DTools;assembly=3DTools    � Title$  Visual Packer��$	��850��$
��auto��$
��auto��$
��1000��.���� .+�o� .@�m� -   $
��file��$��Файл��.@�m� -   $��message��$��Message��m� -   $��check��$��Check�� m� IsCheckable .  m� 	IsChecked . m� -   $
��exit��$
��Exit��m� -   $��Reports��$��Отчеты��.@�m� -   $��report��$!��Схема загрузки��m� -   $��report2��$)��#Список контейнеров��m� -   $
��view��$��Схема 3D��m� -	   $��view2��$��Схема 2D��m� -
   $
��help��$��Справка��.@�m� -   $��about��$��About��c� -   $��toolbar1��@��� -   $��Рассчитать��� $
��auto��$	��600�� � ColumnDefinitions �� $	��150 ��� $
��auto ��� .+�� $��0��.+�R� -   $��vehicleTree��$��5q�$
��auto������ .R��� ����  �� Path$
 Name��� -   $��gridSplitter1��$��1��$��Center=�$��Stretch=�$��10��� $��2��$	��600�� � RowDefinitions �� $
��Auto ��� $
��Auto ��� .+�9� -   $��	expander1��$��0��$
��Left=� 9� ExpandDirection$
 Down=�$��Груз��$	��300��$
��auto��$	��Top=� 9� 
IsExpanded . $	��300��.��)  #Microsoft.Windows.Controls.DataGrid  -     Name$ 	dataGrid1��	   AutoGenerateColumns	 .  $
��auto��$
��Left=�$	��Top=�$
��auto��
   
IsReadOnly
 .    CanUserDeleteRows .    CanUserAddRows .    CanUserReorderColumns .     CanUserResizeColumns .     CanUserResizeRows .     CanUserSortColumns .  $   HorizontalScrollBarVisibility$
 Auto=� � ContextMenu �� .@�m� -   $��1��m� -   $��2��9� -   $��	expander2��$��1��$
��Left=�$
 Down=�$-��'Выбранные автомобили��$	��100��$	��100��$	��Top=� . .��  -   $��	dataGrid2��	 .  $
��auto��$
��Left=�$	��Top=�$
��auto�� J� 	AllowDrop .  .   .   .   .   .   .  
 .  �� .@�m� -   $��Удалить���� -   $��textBox��$��2��$
��Left=�$	��350��$	��350��$	��800��