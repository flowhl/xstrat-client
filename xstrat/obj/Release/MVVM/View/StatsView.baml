   M S B A M L   `   `   `  ���� A  =xstrat, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null3clr-namespace:xstrat.MVVM.Viewxstrat.MVVM.View  %clr-namespace:xstrat.Ui	xstrat.Ui  [ WLiveChartsCore.SkiaSharpView.WPF, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null}Xclr-namespace:LiveChartsCore.SkiaSharpView.WPF;assembly=LiveChartsCore.SkiaSharpView.WPF LiveChartsCore.SkiaSharpView.WPF 3clr-namespace:xstrat.MVVM.Viewxstrat.MVVM.View       xstrat.MVVM.View.StatsView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     0localclr-namespace:xstrat.MVVM.View     &uiclr-namespace:xstrat.Ui     _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  blvcXclr-namespace:LiveChartsCore.SkiaSharpView.WPF;assembly=LiveChartsCore.SkiaSharpView.WPF  (vmsclr-namespace:xstrat.MVVM.View    � DataContext  ��  �� RelativeSource �� *Self+.���  � RowDefinitions �� $
��Auto ��� $��* �.+��� ���#202020 �� CornerRadius$ 10��$��5q�._���  �� Orientation$ Vertical=�.+�� .+��� $ 
Horizontal=�.+��� $��5q�$��Center=�$
��Left=�
�������$��15(�.��	Show Team   xstrat.Ui.ToggleSwitch  -     Name$ 
TeamToggle��   xstrat.Ui.ToggleSwitch!   PreviewMouseLeftButtonDown$+ %TeamToggle_PreviewMouseLeftButtonDown��$��26��$
��Left=��� $ 
Horizontal=�$��Right=�.+��� -   $��	ReloadBtn��$��Right=�$��30��$��30��
�������$��17(����#336cb5$��0q�$��5q� � 	Resources  �� 
TargetType	 �� Property
 �� Value��%:   '��       �� " ��.��� 	 �@��CornerRadius$
 10����'  !MaterialDesignThemes.Wpf.PackIcon  $��30��$��30��   Kind$ Reload=��� -   $��AddBtn��$��Right=�$��30��$��30��
�������$��17(����#336cb5$��0q�$��5q� ��%:   '��       �� " ��.��� 	 �@��CornerRadius$
 10����  $��30��$��30��$	 Add=��� $	��120��$��10q�.���� -   $��
PlayerList��$ Vertical=�$��10,0,0,0q��� $��Disabled=�$
��Auto=�$��1��$��5q�.���� ���#202020$ 10��$��5q�._��� $ Vertical=�$
��Auto��$��	70,0,70,0q�.+��� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$	��400��$ 5���� ���#336cb5$��25(�$��GENERAL��$��	20,0,20,0q��� ���#336cb5$��8��$��0q�$	��400��$ 5���� $ 
Horizontal=�$
��1100��$	��400��.+��� $ Vertical=�$��10q�.+��� 
�������$��Center=�$��18(�.��Player Overview1  +LiveChartsCore.SkiaSharpView.WPF.PolarChart  -     Name$ RadialChart��,  &LiveChartsCore.SkiaSharpView.WPF.Chart   AutoUpdateEnabled . $��	0,-40,0,0q�   TooltipPosition$	 Top=�   InitialRotation$ 0Q�$	��350��$	��350��   Series �� *RadialSeries+   	AngleAxes �� *RadialAngleAxes+�� $��	0,-30,0,0q����#909090$��Center=�$��10(�$
��Wrap=�$	��350����The values shown in this graph show the relative relations between each player. The highest value in each category gets assigned the global max value and every other datapoint decreases in relative relation�� $ Vertical=�$��10q�.+��� 
�������$��Center=�$��18(�.��Current Season MMR�� $	��320��$	��700��$
��Auto=�$��Disabled=�.��5  /LiveChartsCore.SkiaSharpView.WPF.CartesianChart  -     Name$ GenaralBarChart�� . $	��300��   Series �� *GeneralBarSeries+   XAxes �� *GeneralBarXAxes+�� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$	��415��$ 5���� ���#336cb5$��25(�$��SCRIM��$��	20,0,20,0q��� ���#336cb5$��8��$��0q�$	��415��$ 5���� $ Vertical=�$
��1100��$
��Auto��.+��� $ Vertical=�$��10q�.+��� 
�������$��Center=�$��18(�.��Scrim Responses�� $	��320��$
��Auto=�$��Disabled=�.��  $	��300�� �� *ScrimBarSeries+ �� *GeneralBarXAxes+�� $ Vertical=�$��0,50,0,0q�.+��� 
�������$��Center=�$��18(�.��Scrim Participation�� $
��Auto=�$��Disabled=�.��  $	��300��   ZoomMode$ X=�$
��1200��$	 Top=� �� *ScrimHistorySeries+ �� *ScrimHistoryXAxes+�� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$	��405��$ 5���� ���#336cb5$��25(�$��RANKED��$��	20,0,20,0q��� ���#336cb5$��8��$��0q�$	��405��$ 5���� $ Vertical=�$
��1100��.+��� 
�������$��Center=�$��18(�.��Ranked KD by Seasons�� $
��Auto=�$��Disabled=�.��  $	��300��$
��1000��$	 Top=� �� *RankedKDSeries+ �� *RankedKDXAxes+�� 
�������$��Center=�$��18(�$��0,50,0,0q�.��Current season�� $
��Auto=�$��Disabled=�.��  $	��300��$
��1000��$	 Top=� �� *RankedBalkenSeries+ �� *RankedBalkenXAxes+