   M S B A M L   `   `   `  ����R  NPRN_Project_Coffee_Shop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullY1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages#PRN_Project_Coffee_Shop.Views.Pages  5      A    ;PRN_Project_Coffee_Shop.Views.Pages.InventoryManagementPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Clocal1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages     5        N� Title$  Inventory Management��5	      .��5
      � $��10q�6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $	��250 �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ IngredientsDataGrid��5      $��0��60      AutoGenerateColumns$ False��6@      CanUserAddRows$ False��6\      
IsReadOnly$
 True��6�      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6R   1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 61   �� *IngredientId+6R     5      $
 Name��6%   $ * 6V   	 63   �� *IngredientName+6V     5      $ Stock��6%   $
 Auto 6X   	 64   �� *QuantityInStock+6X     5      $
 Unit��6%   $
 Auto 6L   	 63   �� *Unit+6L     5      $ Expiry��6%   $ * 6f   	 65   �� *
ExpiryDate+
 �� StringFormat$
 d��6f      RowStyle 5      �� 5       �� 
TargetType)  #System.Windows.Controls.DataGridRow"  6    �� Triggers 5      �� 5       �� Binding 6&   �� *IsLowOnStock+ �� Value True6G   .a�5      ��  �� Property � �� �� Value
 �����6;   5      5!      5#      �� 5%   
   $��1��6   $��5��6'   $��Center=�61   $��Stretch=�6N   �� 5(   
   $��2��6   $
��Auto=�6'   .��5)      �� -   5         ��Name$ IngredientForm��5)      $��10,0,0,0q�61   .+�5*      �� $��Ingredient Details��6   $��16(�66   $
��Bold"�6D   $��0,0,0,10q�6V   �� 5,      $��Ingredient Name:��6   �� 5-      -   5         ��Name$ IngredientNameTextBox��5-      $��0,5,0,10q�69   �� 5/      $��Quantity in Stock:��6   �� 50      -   5        $ QuantityTextBox��50      $��0,5,0,10q�63   �� 52      $��Unit:��6   �� 53      -   5        $ UnitTextBox��53      $��0,5,0,10q�6/   �� 55      $��Warning Threshold:��6   �� 56      -   5        $ ThresholdTextBox��56      $��0,5,0,10q�64   �� 58      $��Expiry Date:��6   (  "System.Windows.Controls.DatePicker  59      -   5          Name$ ExpiryDatePicker��59      $��0,5,0,10q�67   �� 5;       �� Orientation$ 
Horizontal=�6   $��0,10,0,0q�66   .+�5<      �� -   5         ��Name$ 	NewButton��5<      $	��New��60   $��70��6>   6I   �� 5=      -	   5        $ 
SaveButton��5=      $
��Save��61   $��70��6@   $	��5,0q�6K   6X   5>      5?      5@      5A      5B      