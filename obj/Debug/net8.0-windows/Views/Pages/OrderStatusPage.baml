   M S B A M L   `   `   `  ����R  NPRN_Project_Coffee_Shop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullY1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages#PRN_Project_Coffee_Shop.Views.Pages  5      9    3PRN_Project_Coffee_Shop.Views.Pages.OrderStatusPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Clocal1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages     5        N� Title$  Order Status��5	      .��5      � $��10q�6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   � $��0��6    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��* �6    �� 5      $
��Auto �6    .+�5      ��  �� Orientation$ 
Horizontal=�6   $��5q�62   .+�5      �� $��Orders for:��6   $��16(�6/   $
��Bold"�6=   $��Center=�6O   (  "System.Windows.Controls.DatePicker  5      -   5          Name$ OrderDatePicker��5      $
��10,0q�66   $��Center=�6D   6_   5      &   System.Windows.Controls.DataGrid  5      -   5          Name$ OrdersDataGrid��5      $��1��6/      AutoGenerateColumns$ False��6<      CanUserAddRows$ False��6X      
IsReadOnly$
 True��6o   	   Columns	 5      0  *System.Windows.Controls.DataGridTextColumn  5       ,  &System.Windows.Controls.DataGridColumn
   Header$
 ID��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6Q   1  +System.Windows.Controls.DataGridBoundColumn   Binding 65   �� *	OrderId+6Q     5!      $

 Date��6)   $ * 6g    67   �� *	OrderDate+ �� StringFormat$ g��6g     5"      $
 Table��6)   $
 Auto 6X    68   �� *TableNumber+6X     5#      $
 Total��6)   $ * 6s    68   �� *TotalAmount+$ 
{0:N0} VND��6s     5$      $
 Status��6)   $ * 6T    69   �� *Status+6T   5&      �� 5(      $��2��6   $ 
Horizontal=�6&   $��Right=�6?   $��0,10,0,0q�6[   .+�5)      �� $��Update Status:��6   $��Center=�62   $��0,0,10,0q�6M   �� 5*      -   5         ��Name$ StatusComboBox��5*      $	��120��63   .@�5+      �� $��Pending��6#   �� 5,      $��	Preparing��6#   �� 5-      $��	Completed��6#   �� 5.      $��	Cancelled��6#   5/      �� 50      -   5         ��Name$ UpdateStatusButton��50      $��Update��65   $��80��6F   $��10,0,0,0q�6Q   6c   51      52      �� 55   
   $��1��6   
�������$��1,0,0,0q�69   $��10,0,0,0q�6S    �� Padding$ 10q�6e   ._�56      �� .��57      �� .+�58      �� $��Order Details��6    $��16(�65   $
��Bold"�6C   �� 59      -   5         ��Name$ OrderDetailsNote��59       $$��Select an order to see details��6:   $��0,5,0,10q�6`   $��Italic$�6r   �� 5;      $��Customer Note:��6    $
��Bold"�66   $��0,10,0,0q�6H   �� 5<      -   5        $ CustomerNoteTextBlock��5<       $
��Wrap=�6?   $��40��6S   $	��N/A��6b   �� 5>      $��Items:��6    $
��Bold"�6.   $��0,10,0,5q�6@     5?      -   5        $ OrderDetailsDataGrid��5?      $ False��6=   $ False��6Y   $
 True��6p   	 5@        5A      $

 Item��61   $ 2* 6g    6?   �� *Product.ProductName+6g     5B      $	
 Qty��61   $
 Auto 6[    6>   �� *
Quantity+6[     5C      $
 Options��61   $ 3* 6^    6B   �� *	Options+6^   5E      5F      5G      5H      5I      5J      