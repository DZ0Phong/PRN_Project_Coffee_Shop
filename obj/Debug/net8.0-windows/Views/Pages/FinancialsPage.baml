   M S B A M L   `   `   `  ����R  NPRN_Project_Coffee_Shop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullY1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages#PRN_Project_Coffee_Shop.Views.Pages  5      8    2PRN_Project_Coffee_Shop.Views.Pages.FinancialsPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Clocal1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages     5        N� Title$  
Financials��5	      .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��Financial Report��6   $��18(�6,   $
��Bold"�6:   $��5q�6L   �� 5   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��5q�6;   .+�5      �� $��Select Date:��6   $��Center=�6,   (  "System.Windows.Controls.DatePicker  5      -   5          Name$ ReportDatePicker��5      $	��150��63   $	��5,0q�6?   6L   5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ RevenueDataGrid��5      $��2��6,      AutoGenerateColumns$ False��69      CanUserAddRows$ False��6U      
IsReadOnly$
 True��6l      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn	   Header$	 Order ID��6%   
   Width5  /System.Windows.Controls.DataGridLengthConverter$

 Auto 6S   1  +System.Windows.Controls.DataGridBoundColumn   Binding 67   �� *	OrderId+6S     5      $
	 Time��6%   $
 * 6c    63   �� *	OrderDate+ �� StringFormat$ T��6c     5      $	 Staff��6%   $
 * 6V    64   �� *User.Username+6V     5      $	 Total��6%   $
 * 6o    64   �� *TotalAmount+$ 
{0:N0} VND��6o   5!      �� 5#   
   -   5         ��Name$ TotalRevenueTextBlock��5#      $��3��63   $,��&Total Revenue for Selected Date: 0 VND��6@   $��16(�6n   $
��Bold"�6|   $��Right=�6�   $��0,10,5,0q�6�   5$      5%      