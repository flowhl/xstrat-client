   M S B A M L   `   `   `  ����A  =xstrat, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:xstrat.MVVM.ViewModelxstrat.MVVM.ViewModel  5   	   3clr-namespace:xstrat.MVVM.Viewxstrat.MVVM.View  5   	   clr-namespace:xstratxstrat  5   	       xstrat.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9	viewModel#clr-namespace:xstrat.MVVM.ViewModel     5   	   /viewclr-namespace:xstrat.MVVM.View     5   	   &localclr-namespace:xstrat     5   	   _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFMaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  6,   $	��800��5
   	   $
��1350��6     � WindowStyle$
  None=�5   	    � 
ResizeMode$ CanResizeWithGrip=�5   	   
������  � AllowsTransparency .  � DataContext 5      )  #xstrat.MVVM.ViewModel.MainViewModel  5   
   .��5      �� ���#131313 �� CornerRadius$ 20��6"   ._�5      �  � ColumnDefinitions 5      �� 5      $	��200 �6'   �� 5       � RowDefinitions 5      �� 5      $��25 �6$   �� 5      .+�5#      �� -   5        $��2��5#      ���#303030._�5(      � $��Stretch=�6   .+�5*      �� $��./Images/topbar.png��6    $
��Left=�6=   $��Center=�6X   $��3,0,0,0q�6s   �� 5,      $��Right=�6%    �� Orientation$ 
Horizontal=�5-   !   .+�5/      �� -   5        $��ButtonMinimize��50   !   $��20��51   !   $��20��6,   
������ $��0q�53   !   
�������$
��Bold"�55   !   $��0,0,0,1q�56   !   $��Bottom=�58      ��59      '  !MaterialDesignThemes.Wpf.PackIcon  5:   "   $��Bottom=�6:   $��Bottom=�6U      Kind$ WindowMinimize=�6x   6�   5<      �� 5=      -   5        $��ButtonFullscreen��5>   !   $��20��5?   !   $��20��6,   
������ $��0q�5A   !   
�������$
��Bold"�5C   !   $��0,0,0,3q�5D   !   ��5F        5G   "   -   5        $��ButtonFullscreenIcon��5G   :   $��Bottom=�6V   $��Bottom=�6q   $ 
Fullscreen=�6�   6�   5I      �� 5K      -   5        $��ButtonClose��5L      $��20��5M   !   $��20��6,   
������ $��0q�5O   !   
�������$
��Bold"�5Q   !   $��0,0,0,3q�5R   !   ��5T        5U   "   $��Bottom=�6:   $��Bottom=�6U   $ WindowClose=�6x   6�   5W      5Z      5\      5]      �� 5_      ���#101010$��2��6.   $ 12��6;   ._�5`      �� .+�5a      	� $
��Home��6&   $��50��5b      
�������$��14(�5d         MenuButtonTheme#��[  5e      	 d� 	IsChecked$
	 TrueY�5f      ��5g      �� *HomeViewCommand+	� 5j      $��Strats��6&   $��50��5k      
�������$��14(�5m      #��[  5n      ��5o      �� *StratMakerViewCommand+	� 5q      $
��Team��6"   $��50��5r      
�������$��14(�5t      #��[  5u      ��5v      �� *TeamViewCommand+	� 5x      $��Calendar��6"   $��50��5y      
�������$��14(�5{      #��[  5|      ��5}      �� *CalendarViewCommand+	� 5      $��Scrims��6"   $��50��5�      
�������$��14(�5�      #��[  5�      ��5�      �� *ScrimViewCommand+	� 5�      $��Stats��6"   $��50��5�      
�������$��14(�5�      #��[  5�      ��5�      �� *StatsViewCommand+	� 5�      $��Routines��6"   $��50��5�      
�������$��14(�5�      #��[  5�      ��5�      �� *RoutinesViewCommand+	� 5�      $��Skin Switcher��6&   $��50��5�      
�������$��14(�5�      #��[  5�      ��5�      �� *SkinSwitcherViewCommand+	� 5�      $��Settings��6&   $��50��5�      
�������$��14(�5�      #��[  5�      ��5�      �� *SettingsViewCommand+	� 5�      $��About��6&   $��50��5�      
�������$��14(�5�      #��[  5�      ��5�      �� *AboutViewCommand+5�      5�      �� 5�      -   5        $��contentControl��5�      $��1��5�   !   $��1��5�      $��	0,0,10,10q�5�      ��5�      �� *CurrentView+5�      5�      5�      