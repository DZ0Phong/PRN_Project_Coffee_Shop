   M S B A M L   `   `   `  ����R  NPRN_Project_Coffee_Shop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:PRN_Project_Coffee_ShopPRN_Project_Coffee_Shop  5   	   (    "PRN_Project_Coffee_Shop.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:PRN_Project_Coffee_Shop     5   	     � Title$  Coffee Shop Management��5   	   $	��600��6(   $
��1000��65    � WindowStartupLocation$ CenterScreen=�5	   	   .��5
      �  � ColumnDefinitions 5   
   �� 5      $	��200 �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��0��6%   ���#f0f0f0.+�5      �� $��Coffee Shop��6   $��20(�6+   $
��Bold"�69   $��10q�6K   $��Center=�6W   �� 5      -   5         ��Name$ OrderManagementButton��5      $��Create Order��64   $
��10,5q�6K   6Y   �� 5      -   5        $ OrderStatusButton��5      $��Order Status��60   $
��10,5q�6G   6U   �� 5      -   5        $ CustomerManagementButton��5      $��Customer Management��67   $
��10,5q�6U   6c   �� 5      -   5        $ InventoryManagementButton��5      $��Inventory Management��68   $
��10,5q�6W   6e   �� 5      -   5        $ MenuManagementButton��5      $��Menu Management��63   $
��10,5q�6M   $y�	Collapsed=�6~   �� 5      -   5        $ FinancialsButton��5      $��
Financials��6/   $
��10,5q�6D   $y�	Collapsed=�6q   �� 5      -   5        $ EmployeeManagementButton��5      $��Employee Management��67   $
��10,5q�6U   $y�	Collapsed=�6�   �� 5!      -   5        $ LogoutButton��5!      $��Logout��6+   $��
10,20,10,5q�6<   $��Bottom=�6P   6k   5"       � 5%   
   -	   5          �Name$ 	MainFrame��5%      $��1��6#   $��0��63     � NavigationUIVisibility$ Hidden=�6@   �� 5(   
   $��1��6   $��2��6!   .@�5)      �� .��5*      �� -
   5         ��Name$ UserInfoTextBlock��5*      $	��5,0q�67   5+      5,      5-      5.      