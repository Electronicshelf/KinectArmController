   M S B A M L   `   `   `  ����  kinectEllipse    kinectEllipse.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  
MainWindow��5      $	��845��6   $
��1428��6(   .��5      � .+�5   
   �� -   5        $��image��5   K   $	��773��6   $
��Left=�6   $��	12,12,0,0q�68   $
��Fill=�6X   $	��Top=�6g   $
��1058��6   �� 5   
   -   5        $��CamUpButton��5   b   $��	Camera up��6   $	��113��6%   $
��Left=�62   $��1186,39,0,0q�6M   $	��Top=�6u   $	��174��6�   6�   �� 5   
   -   5        $��CamDownButton��5   e   $��Camera down��6   $	��124��6'   $
��Left=�64   $��1186,169,0,0q�6O   $	��Top=�6z   $	��174��6�   6�   �� 5	   
   -   5        $��stackPanel1��5	   S   $	��132��6   $
��Left=�6"   $��1186,389,0,0q�6=   $	��Top=�6f   $	��163��6~    � BindingGroup 5
      &   System.Windows.Data.BindingGroup  5         Name$ ImageToShowGroup��6      SharesProposedValues . 67   .+�5      	� -   5        $��ColorRadioButton��5   6   $��Color��6   $��20��6*   $��3q�6N   $��14(�6Y    d� 	IsChecked$
 TrueY�6g   $
��Left=�6x   6�   	� 5      -   5        $��DepthRadioButton��5   6   $��Depth��6   $��22��6*   $��3q�6N   $��14(�6Y   $��91��6g   $
��Left=�6r   6�   	� 5      -   5        $��NoneRadioButton��5   C   $
��None��6   $��14(�6)   $��24��67   $��91��6Z   $
��Left=�6e   6�   5      �� 5   
   -	   5        $��canvas��5   L   $	��773��6   $
��Left=�6   $��	12,12,0,0q�69   $	��Top=�6Z   $
��1058��6r   5      5      5      