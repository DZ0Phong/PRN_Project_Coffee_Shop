   M S B A M L   `   `   `  ����R  NPRN_Project_Coffee_Shop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullY1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages#PRN_Project_Coffee_Shop.Views.Pages  5      =    7PRN_Project_Coffee_Shop.Views.Pages.OrderManagementPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Clocal1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages     5        N� Title$  Order Management��5	      .��5
      �  � ColumnDefinitions 5   
   �� 5      $��3* �6   �� 5      $��5* �6   .+�5   
   �� $��0��6   
�������$��0,0,1,0q�69   ._�5      �� .��5      �� -   5         ��Name$  MenuCategoriesItemsControl��5      ��5      �� 5      .R�5      �� $��5q�6)   .+�5   "   �� $��18(�6J   $
��Bold"�6X   $��5q�6j   ��6,   �� *CategoryName+6j   �� 5   "   ��6/   �� *
Products+��5   &   �� 5   *   .R�5   .   �� -   5        $	��2,1q�5   N   $	��5,3q�6[   $��Stretch=�6i   .��5   2   �  5   6   �� 5   :   $��* �6K   �� 5   :   $
��Auto �6K   .+�5!   6   �� $��0��6@   ��6P   �� *ProductName+�� 5"   6   $��1��6@   $��Right=�6x   ��6P   �� *Price+ �� StringFormat$ N0��6x   5#   3   5$   /   5%   +   5'   #   5(      5)      5+      5,      5-      � 50   
   $��1��6    � RowDefinitions 51      �� 52      $��* �6    �� 53      $
��Auto �6    .+�56      � $��0��6    57      �� 58      $��3* �6'   �� 59      $��2* �6'   .+�5=      &   System.Windows.Controls.DataGrid  -   5          Name$ OrderDataGrid��5=   +   $��0��6      AutoGenerateColumns$ False��6B      CanUserAddRows$ False��6^   ��5>      �� *CurrentOrderItems+   Columns 5?      0  *System.Windows.Controls.DataGridTextColumn  5@      ,  &System.Windows.Controls.DataGridColumn	   Header$
	 Item��6-   
   Width5  /System.Windows.Controls.DataGridLengthConverter$
 2* 6c      
IsReadOnly$
 True��6n   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6;   �� *Product.ProductName+6n     5A      $		 Qty��6-   $

 Auto 6W   $
 True��6d    6:   �� *
Quantity+6d     5B      $	 Price��6-   $
 * 6o   $
 True��6y    6<   �� *Product.Price+$ N0��6y     5C      $	 Total��6-   $
 * 6l   $
 True��6v    6<   �� *
TotalPrice+$ N0��6v   5E      �� 5H      $��1��6   
�������$��1,0,0,0q�6A    �� Padding$ 10q�6[   ._�5I      �� .��5J      �� -   5         ��Name$ OptionsPanel��5J   %   $y�	Collapsed=�6;   .+�5K      �� $��Customize Item��6(   $��16(�6>   $
��Bold"�6L   $��0,0,0,10q�6^   �� 5N      -   5        $ DrinkOptionsPanel��5N   )   .+�5O   "   �� $��Sugar:��6,   �� 5P   "   -   5         ��Name$ SugarSlider��5P   )    � Minimum$ 0Q�6>    � Maximum$	 100Q�6J    � Value$	 100Q�6X    �� TickFrequency$ 10Q�6d    �� IsSnapToTickEnabled$
 True��6w   6�   �� 5Q   "   $��Center=�6v   ��6,   ��  �� ElementName$ SugarSlider�� �� Path$ Value�$
 {0}%��6v   �� 5S   "   $
��Ice:��6,   $��0,10,0,0q�68   �� 5T   "   -   5        $ 	IceSlider��5T   )   $ 0Q�6<   $	 100Q�6H   $	 100Q�6V   $ 10Q�6b   $
 True��6u   6�   �� 5U   "   $��Center=�6t   ��6,   �� $ 	IceSlider��$ Value�$
 {0}%��6t   5V      �� 5Y      -   5        $ ToppingsPanel��5Y   )   $��0,15,0,0q�6@   .+�5Z   "   �� $��Toppings (Optional):��6,   �� 5[   "   -	   5        $ ToppingsItemsControl��5[   /   ��5\   &   �� 5]   *   .R�5^   .   �� -
   5        ��5^   7   �� *ProductName+
 � Tag 6W   �� *	ProductId+6�   5_   +   5a   #   5b      5d      5e      5f      5g      �� 5j      $��1��6    �� Orientation$ Vertical=�6&   $��10q�6=   .+�5k      �  5l      �� 5m      $��* �6+   �� 5n      $��* �6+   .+�5p      �� $��0��6!   .+�5q      �� $��Table/Delivery:��6$   �� 5r      -   5         ��Name$ TableComboBox��5r   #   �� 5s      $ ��Customer Email (Optional):��6$   �� 5t      -   5         ��Name$ CustomerEmailTextBox��5t   "   5u      �� 5v      $��1��6!   $��10,0,0,0q�61   .+�5w      �� $ ��Promotion Code (Optional):��6$   ]� 5x      .+�5y      �� -   5         ��Name$ ApplyPromotionButton��5y   %   $��Apply��6C   $��Right=�6S   6j   �� 5z      -   5        $ PromotionCodeTextBox��5z   &   5{      �� 5|      $ ��Delivery Notes (Optional):��6$   �� 5}      -   5        $ DeliveryNotesTextBox��5}   "   �� 5~      -   5         ��Name$ TotalAmountTextBlock��5~   $   $��16(�6B   $
��Bold"�6P   $��0,10,0,0q�6b   $��Total: 0 VND��6t   5      5�      �� 5�      $ 
Horizontal=�6   $��Right=�66   $��0,15,0,0q�6R   .+�5�      �� -   5        $ RemoveItemButton��5�      $��Remove Item��67   $	��120��6M   
��� E��6x   �� 5�      -   5        $��Confirm Order��5�      $	��120��65   $��10,0,0,0q�6A   6S   �� 5�      -   5        $��Cancel Order��5�      $	��120��64   $��10,0,0,0q�6@   6R   5�      5�      5�      5�      5�      