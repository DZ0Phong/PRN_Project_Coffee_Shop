   M S B A M L   `   `   `  ����R  NPRN_Project_Coffee_Shop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullY1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages#PRN_Project_Coffee_Shop.Views.Pages  5      @    :PRN_Project_Coffee_Shop.Views.Pages.CustomerManagementPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Clocal1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages     5        N� Title$  Customer Management��5	      .��5      � $��10q�6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   � $��0��6   $��0,0,10,0q�6    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $��* �6    .+�5      �� $��	Customers��6   $��18(�6)   $
��Bold"�67   $��5q�6I   �� 5      $��1��6    �� Orientation$ 
Horizontal=�6&   $��5q�6?   .+�5      �� $��Search by Email/Name:��6   $��Center=�69   �� 5      -   5         ��Name$ SearchTextBox��5      $	��150��61   $	��5,0q�6=   �� 5      -   5         ��Name$ SearchButton��5      $��Search��6/   6@   �� 5      -   5        $ ResetButton��5      $��Reset��6.   $��5,0,0,0q�6X   5       &   System.Windows.Controls.DataGrid  5"      -   5          Name$ CustomersDataGrid��5"      $��2��62      AutoGenerateColumns$ False��6?      CanUserAddRows$ False��6[   	   Columns	 5#      0  *System.Windows.Controls.DataGridTextColumn  5$      ,  &System.Windows.Controls.DataGridColumn
   Header$
 Email��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6R      
IsReadOnly$
 True��6\   1  +System.Windows.Controls.DataGridBoundColumn   Binding 68   �� *Email+6\     5%      $

 Name��6)   $ * 6X    67   �� *CustomerName+6X     5&      $
 Points��6)   $
 Auto 6T   $
 True��6a    69   �� *Points+6a   5(      5)      � 5,   
   $��1��6    5-      �� 5.      $
��Auto �6    �� 5/      $��* �6    .+�52      �� $��Purchase History��6   $��18(�60   $
��Bold"�6>   $��5q�6P     54      -   5        $ PurchaseHistoryDataGrid��54      $��1��68   $ False��6E   $ False��6a      
IsReadOnly$
 True��6x   	 55        56      $
 Order ID��6)   $
 Auto 6W    6;   �� *	OrderId+6W     57      $

 Date��6)   $ * 6g    67   �� *	OrderDate+ �� StringFormat$ d��6g     58      $
 Total��6)   $ * 6s    68   �� *TotalAmount+$ 
{0:N0} VND��6s   5:      5;      5<      5=      