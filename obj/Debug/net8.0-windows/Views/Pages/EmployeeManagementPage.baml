   M S B A M L   `   `   `  ����R  NPRN_Project_Coffee_Shop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullY1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages#PRN_Project_Coffee_Shop.Views.Pages  5      @    :PRN_Project_Coffee_Shop.Views.Pages.EmployeeManagementPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Clocal1clr-namespace:PRN_Project_Coffee_Shop.Views.Pages     5        N� Title$  Employee Management��5	      .��5      � $��10q�6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $	��250 �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ EmployeesDataGrid��5      $��0��6.      AutoGenerateColumns$ False��6>      CanUserAddRows$ False��6Z      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6P   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *
EmployeeId+6P     5      $ 	Full Name��6%   $ * 6U    68   �� *
FullName+6U     5      $ Position��6%   $ * 6T    67   �� *
Position+6T     5      $ Salary��6%   $
 Auto 6g    65   �� *Salary+	 �� StringFormat$	 {0:N0}��6g     5      $ Username��6%   $ * 6Y    67   �� *User.Username+6Y   5      �� 5   
   $��1��6   $��5��6'   $��Center=�61   $��Stretch=�6N   �� 5    
   $��2��6   $
��Auto=�6'   .��5!      �� -   5        
 ��Name$
 EmployeeForm��5!      $��10,0,0,0q�6/   .+�5"      �� $��Employee Details��6   $��16(�64   $
��Bold"�6B   $��0,0,0,10q�6T   �� 5$      $��
Full Name:��6   �� 5%      -   5         ��Name$ FullNameTextBox��5%      $��0,5,0,10q�63   �� 5'      $��	Position:��6   �� 5(      -   5        $ PositionTextBox��5(      $��0,5,0,10q�63   �� 5*      $��Salary:��6   �� 5+      -   5        $ SalaryTextBox��5+      $��0,5,0,10q�61   �� 5-      $��User Account:��6   �� 5.      -   5         ��Name$ UserComboBox��5.      $��0,5,0,10q�61    �� DisplayMemberPath$ Username��6C    �� SelectedValuePath$ UserId��6`   �� 50       �� Orientation$ 
Horizontal=�6   $��0,10,0,0q�66   .+�51      �� -   5         ��Name$ 	NewButton��51      $	��New��60   $��70��6>   6I   �� 52      -   5        $ 
SaveButton��52      $
��Save��61   $��70��6@   $	��5,0q�6K   6X   53      54      55      56      57      