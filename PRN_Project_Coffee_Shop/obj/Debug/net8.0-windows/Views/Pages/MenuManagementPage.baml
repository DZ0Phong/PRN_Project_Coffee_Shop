   M S B A M L   `   `   `  ����R  NPRN_Project_Coffee_Shop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullY1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages#PRN_Project_Coffee_Shop.Views.Pages  5      <    6PRN_Project_Coffee_Shop.Views.Pages.MenuManagementPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Clocal1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages     5        N� Title$  Menu Management��5	      .��5      � $��10q�6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $	��250 �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ ProductsDataGrid��5      $��0��6-      AutoGenerateColumns$ False��6=      CanUserAddRows$ False��6Y      
IsReadOnly$
 True��6p      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6O   1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 61   �� *	ProductId+6O     5      $
 Name��6%   $ * 6S   	 63   �� *ProductName+6S     5      $ Category��6%   $ * 6a   	 67   �� *Category.CategoryName+6a     5      $ Price��6%   $
 Auto 6e   	 64   �� *Price+
 �� StringFormat$
 {0:N0}��6e   4  .System.Windows.Controls.DataGridCheckBoxColumn  5      $ Out of Stock��6)   $
 Auto 6`   	 6?   �� *IsOutOfStock+6`   5      �� 5   
   $��1��6   $��5��6'   $��Center=�61   $��Stretch=�6N   �� 5    
   $��2��6   $
��Auto=�6'   .��5!      �� -   5         ��Name$ ProductForm��5!      $��10,0,0,0q�6.   .+�5"      �� $��Product Details��6   $��16(�63   $
��Bold"�6A   $��0,0,0,10q�6S   �� 5$      $��Product Name:��6   �� 5%      -   5         ��Name$ ProductNameTextBox��5%      $��0,5,0,10q�66   �� 5'      $��	Category:��6   �� 5(      -   5         ��Name$ CategoryComboBox��5(      $��0,5,0,10q�65    �� DisplayMemberPath$ CategoryName��6G    �� SelectedValuePath$ 
CategoryId��6h   �� 5*      $��Price:��6   �� 5+      -   5        $ PriceTextBox��5+      $��0,5,0,10q�60   �� 5-      -   5         ��Name$ IsOutOfStockCheckBox��5-      $��Is Out of Stock��69   $��0,5,0,10q�6S   �� 5/       �� Orientation$ 
Horizontal=�6   $��0,10,0,0q�66   .+�50      �� -   5         ��Name$ 	NewButton��50      $	��New��60   $��70��6>   6I   �� 51      -   5        $ 
SaveButton��51      $
��Save��61   $��70��6@   $	��5,0q�6K   6X   �� 52      -	   5        $ DeleteButton��52      $��Delete��63   $��70��6D   6O   53      �� 55      $��Ingredients��6   $��14(�6/   $
��Bold"�6=   $��	0,20,0,10q�6O   �� 56      -
   5         ��Name$ IngredientsListView��56      $	��150��68   $��0,0,0,10q�6E    �� View 57      �� 58      .P�59      �� $��
Ingredient��6-    �� Width$	 120��6|    �� DisplayMemberBinding 6A   �� *Ingredient.IngredientName+6|   �� 5:      $��Quantity��6-   $ 80��6?    �� CellTemplate 5;   "   �� 5<   &   .R�5=   *   �� $��70��6y   ��62   �� *QuantityRequired+ �� UpdateSourceTrigger$ PropertyChanged=�6y   5>   '   5@      5A      5C      �� 5E      $ 
Horizontal=�6   .+�5F      �� -   5        $ AddIngredientButton��5F      $��Add Ingredient��6:   $	��100��6S   6_   �� 5G      -   5        $ RemoveIngredientButton��5G      $��Remove��6=   $��70��6N   $	��5,0q�6Y   6f   5H      5J      5K      5L      5M      