   M S B A M L   `   `   `  ���� A  =xstrat, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null3clr-namespace:xstrat.MVVM.Viewxstrat.MVVM.View  1clr-namespace:xstrat.Calendarxstrat.Calendar  #    xstrat.MVVM.View.CalendarView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     0localclr-namespace:xstrat.MVVM.View     0local1clr-namespace:xstrat.Calendar     -     >�Name$  
CalendarUI��.��� .+��� $
��Auto=�.���� $��0q����#202020 �� CornerRadius$ 10��$��10,10,10,10q�._���  �� Orientation$ Vertical=�.+��� 
�������$��30(�$��Center=�.��Team Calendar:#   xstrat.Calendar.CalendarMonth  -     Name$ CalendarMonthUI��$��
10,10,10,0q�   	GridBrush$ #336cb5��   GridBorderThickness$	 0.8Q�$	��700��   Events ��  �� ElementName$ 
CalendarUI�� �� Path$ Events��� ���#222222$��
0,-100,0,0q�$ 10��._��� $ 
Horizontal=�$��0,0,0,0q�.+��� -   $
��Row1��$ Vertical=�$	��100��$	��200��$��10q�.+��� -   $��OffDayChecks��
����������#b53333$��16(�$��Center=�	 d� 	IsChecked$
	 TrueY�.��
Off days�� -   $��ScrimChecks��
����������#336cb5$��16(�$��Center=�$
	 TrueY�.��Scrims�� -   $
��Row2��$ Vertical=�$	��100��$	��200��$��10q��� -   $
��Row3��$ Vertical=�$	��100��$	��200��$��10q��� -   $
��Row4��$ Vertical=�$	��100��$	��200��$��10q��� -	   $
��Row5��$ Vertical=�$	��100��$	��200��$��10q�