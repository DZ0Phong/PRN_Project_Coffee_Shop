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
 Status��6)   $ * 6T    69   �� *Status+6T   5&      �� 5(      $��2��6   $ Vertical=�6&   $��Stretch=�6=   $��5,10,5,0q�6[   .+�5)      �� $��Update Status��6   $��14(�61   $
��Bold"�6?   �� 5*      
�������$��1q�61    �� Padding$ 10q�6E   $��0,5,0,0q�6R   ._�5+      �  5,      �� 5-      $
��Auto �6,   �� 5.      $
��Auto �6,   .+�50      �� $��0��6%   $ 
Horizontal=�62   $��0,0,0,10q�6K   .+�51      �� $��Current Status: ��6(   $��SemiBold"�6@   $��Center=�6V   �� 52      -   5         ��Name$ CurrentStatusTextBlock��52   (   $	��N/A��6H   $��Center=�6S   53      �� 55      $��1��6%   .+�56      �� $��Set new status:��6(   $��SemiBold"�6?   	� 57      -   5         	�Name$ PreparingRadioButton��57   *   $��	Preparing��6H    	� 	GroupName$ 	NewStatus��6\   	� 58      -   5        $ CompletedRadioButton��58   *   $��	Completed��6H   $ 	NewStatus��6\   	� 59      -   5        $ CancelledRadioButton��59   *   $��	Cancelled��6H   $ 	NewStatus��6\   �� 5:      -   5         ��Name$ UpdateStatusButton��5:   %   $��Update Status��6A   $��Right=�6Y   $��0,10,0,0q�6u   6�   5;      5<      5=      5>      5?      �� 5B   
   $��1��6   
�������$��1,0,0,0q�69   $��10,0,0,0q�6S   $ 10q�6e   ._�5C      �� .��5D      �� .+�5E      �� $��Order Details��6    $��16(�65   $
��Bold"�6C   �� 5F      -   5        $ OrderDetailsNote��5F       $$��Select an order to see details��6:   $��0,5,0,10q�6`   $��Italic$�6r   �� 5H      $��Customer Note:��6    $
��Bold"�66   $��0,10,0,0q�6H   �� 5I      -	   5        $ CustomerNoteTextBlock��5I       $
��Wrap=�6?   $��40��6S   $	��N/A��6b   �� 5K      $��Items:��6    $
��Bold"�6.   $��0,10,0,5q�6@     5L      -
   5        $ OrderDetailsDataGrid��5L      $ False��6=   $ False��6Y   $
 True��6p   	 5M        5N      $

 Item��61   $ 2* 6g    6?   �� *Product.ProductName+6g     5O      $	
 Qty��61   $
 Auto 6[    6>   �� *
Quantity+6[     5P      $
 Options��61   $ 3* 6^    6B   �� *	Options+6^   5R      5S      5T      5U      5V      5W      