   M S B A M L   `   `   `  ����A  =xstrat, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null3clr-namespace:xstrat.MVVM.Viewxstrat.MVVM.View  5      %clr-namespace:xstrat.Ui	xstrat.Ui  6;   [ WLiveChartsCore.SkiaSharpView.WPF, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null}Xclr-namespace:LiveChartsCore.SkiaSharpView.WPF;assembly=LiveChartsCore.SkiaSharpView.WPF LiveChartsCore.SkiaSharpView.WPF 5      3clr-namespace:xstrat.MVVM.Viewxstrat.MVVM.View  5           xstrat.MVVM.View.StatsView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0localclr-namespace:xstrat.MVVM.View     5      &uiclr-namespace:xstrat.Ui     6;   _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  6^   blvcXclr-namespace:LiveChartsCore.SkiaSharpView.WPF;assembly=LiveChartsCore.SkiaSharpView.WPF  5      (vmsclr-namespace:xstrat.MVVM.View  5        � DataContext  5      ��  �� RelativeSource �� *Self+.��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   6,   �� 5      $��* �6   6)   .+�5   
   �� ���#202020 �� CornerRadius$ 10��6&   $��5q�68   ._�5      ��  �� Orientation$ Vertical=�6   .+�5      � .+�5      �� $ 
Horizontal=�6!   .+�5      �� $��5q�6    $��Center=�6+   $
��Left=�6F   
�������$��15(�6t   .��6�   	Show Team6�      xstrat.Ui.ToggleSwitch  5      -   5          Name$ 
TeamToggle��5   �      xstrat.Ui.ToggleSwitch!   PreviewMouseLeftButtonDown$+ %TeamToggle_PreviewMouseLeftButtonDown��66   $��26��6*   $
��Left=�6y   6�   5      �� 5      $ 
Horizontal=�6!   $��Right=�6:   .+�5      �� -   5        $��	ReloadBtn��5   T   $��Right=�6!   $��30��6=   $��30��6I   
�������$��17(�6x   ���#336cb5$��0q�6�   $��5q�6�    � 	Resources 5       �� 
TargetType	 �� Property
 �� Value��%_   '��       �� 5   "   " ��6(   .�5   &   �� 	 �@��CornerRadius$
 10��6E   5   #   5       ��5!      '  !MaterialDesignThemes.Wpf.PackIcon  5"   "   $��30��6:   $��30��6F      Kind$ Reload=�6Q   5$      �� 5%      -   5        $��AddBtn��5%   T   $��Right=�6!   $��30��6=   $��30��6I   
�������$��17(�6u   ���#336cb5$��0q�6�   $��5q�6�    5&      ��%_   '��       �� 5'   "   " ��6(   .�5(   &   �� 	 �@��CornerRadius$
 10��6E   5)   #   5*      ��5+        5,   "   $��30��6:   $��30��6F   $	 Add=�6Q   5.      5/      51      �� 52      $	��120��6   $��10q�6/   .��53      �� -   5        $��
PlayerList��53   8   $ Vertical=�6!   $��10,0,0,0q�6J   55      56      57      58      �� 59   
   $��Disabled=�6   $
��Auto=�6@   $��1��6c   $��5q�6p   .��5:      �� ���#202020$ 10��6*   $��5q�6<   ._�5;      �� $ Vertical=�6   $
��Auto��64   $��	70,0,70,0q�6C   .+�5=      �� $ 
Horizontal=�6!   $��Center=�6:   $��0,10,0,0q�6W   .+�5>      �� ���#336cb5$��8��66   $��0q�6A   $	��400��6U   $ 5��6a   6t   �� 5?      ���#336cb5$��25(�65   $��GENERAL��6C   $��	20,0,20,0q�6U   6j   �� 5@      ���#336cb5$��8��66   $��0q�6A   $	��400��6U   $ 5��6a   6t   5A      �� 5C      $ 
Horizontal=�6!   $
��1100��6:   $	��400��6G   .+�5D      �� $ Vertical=�6%   $��10q�6<   .+�5E      �� 
�������$��Center=�67   $��18(�6T   .��6b   Player Overview6s   1  +LiveChartsCore.SkiaSharpView.WPF.PolarChart  5F      -   5          Name$ RadialChart��5F   -   ,  &LiveChartsCore.SkiaSharpView.WPF.Chart   AutoUpdateEnabled . $��	0,-40,0,0q�6{      TooltipPosition$	 Top=�6�      InitialRotation$ 0Q�6�   $	��350��6�   $	��350��6�      Series 6B   �� *RadialSeries+   	AngleAxes 6�   �� *RadialAngleAxes+6�   �� 5G      $��	0,-30,0,0q�6(   ���#909090$��Center=�6P   $��10(�6m   $
��Wrap=�6{   $	��350��6�   ��The values shown in this graph show the relative relations between each player. The highest value in each category gets assigned the global max value and every other datapoint decreases in relative relation6k  5H      �� 5I      $ Vertical=�6%   $��10q�6=   .+�5J      �� 
�������$��Center=�67   $��18(�6T   .��6b   Current Season MMR6v   �� 5K      $	��320��6+   $	��700��68   $
��Auto=�6D   $��Disabled=�6i   .��5L   "   5  /LiveChartsCore.SkiaSharpView.WPF.CartesianChart  -   5          Name$ GenaralBarChart��5L   5    . $	��300��6�      Series 6N   �� *GeneralBarSeries+   XAxes 6�   �� *GeneralBarXAxes+6�   5M      5N      5O      �� 5R      $ 
Horizontal=�6!   $��Center=�6:   $��0,10,0,0q�6W   .+�5S      �� ���#336cb5$��8��66   $��0q�6A   $	��415��6U   $ 5��6a   6t   �� 5T      ���#336cb5$��25(�65   $��SCRIM��6C   $��	20,0,20,0q�6S   6h   �� 5U      ���#336cb5$��8��66   $��0q�6A   $	��415��6U   $ 5��6a   6t   5V      �� 5X      $ Vertical=�6!   $
��1100��68   $
��Auto��6E   .+�5Y      �� $ Vertical=�6%   $��10q�6<   .+�5Z      �� 
�������$��Center=�67   $��18(�6T   .��6b   Scrim Responses6s   �� 5[      $	��320��6+   $
��Auto=�68   $��Disabled=�6]   .��5\   "     $	��300��6y    65   �� *ScrimBarSeries+ 6W   �� *GeneralBarXAxes+6�   5]      5^      �� 5_      $ Vertical=�6%   $��0,50,0,0q�6<   .+�5`      �� 
�������$��Center=�67   $��18(�6T   .��6b   Scrim Participation6w   �� 5a      $
��Auto=�6+   $��Disabled=�6P   .��5b   "     $	��300��6      ZoomMode$ X=�6�   $
��1200��6�   $	 Top=�6�    65   �� *ScrimHistorySeries+ 6[   �� *ScrimHistoryXAxes+6�   5c      5d      5e      �� 5h      $ 
Horizontal=�6!   $��Center=�6:   $��0,10,0,0q�6W   .+�5i      �� ���#336cb5$��8��66   $��0q�6A   $	��405��6U   $ 5��6a   6t   �� 5j      ���#336cb5$��25(�65   $��RANKED��6C   $��	20,0,20,0q�6T   6i   �� 5k      ���#336cb5$��8��66   $��0q�6A   $	��405��6U   $ 5��6a   6t   5l      �� 5n      $ Vertical=�6!   $
��1100��68   .+�5o      �� 
�������$��Center=�63   $��18(�6P   .��6^   Ranked KD by Seasons6t   �� 5p      $
��Auto=�6'   $��Disabled=�6L   .��5q        $	��300��6s   $
��1000��6�   $	 Top=�6�    61   �� *RankedKDSeries+ 6S   �� *RankedKDXAxes+6�   5r      �� 5s      
�������$��Center=�63   $��18(�6P   $��0,50,0,0q�6^   .��6p   Current season6�   �� 5t      $
��Auto=�6'   $��Disabled=�6L   .��5u        $	��300��6{   $
��1000��6�   $	 Top=�6�    61   �� *RankedBalkenSeries+ 6W   �� *RankedBalkenXAxes+6�   5v      5w      5x      5y      5z      5{      5|      