   M S B A M L   `   `   `  ����  PRN_Project_Coffee_Shop=    7PRN_Project_Coffee_Shop.Views.IngredientSelectionWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Select Ingredient��5   	   $	��350��6#   $	��300��60    � WindowStartupLocation$ CenterScreen=�5   	   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5	      $
��Auto �6   .+�5   
   �� -   5         ��Name$ IngredientsListView��5      $��0��60    �� SelectionMode$ Single=�6=    �� View 5      �� 5      .P�5      �� $��
Ingredient��6%    �� Width$	 150��6i    �� DisplayMemberBinding 69   �� *IngredientName+6i   �� 5      $��In Stock��6%   $	 100��6h    67   �� *QuantityInStock+6h   5      5      �� 5   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   $��0,10,0,0q�6W   .+�5      �� -   5        	 ��Name$	 SelectButton��5      $��Select��6+   $��70��6<   
 �� 	IsDefault$

 True��6G   6X   �� 5      -   5        $	 CancelButton��5      $��Cancel��6+   $��70��6<   $	��5,0q�6G    �� IsCancel$
 True��6T   6d   5      5      5      