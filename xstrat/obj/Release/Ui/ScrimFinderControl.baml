   M S B A M L   `   `   `  ���� A  =xstrat, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null%clr-namespace:xstrat.Ui	xstrat.Ui  "    xstrat.Ui.ScrimFinderControl   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     )localclr-namespace:xstrat.Ui     _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  .��� .+��� -   $��RoutinesOverview��$	��300��$
��Left=����#181818  �� CornerRadius$  20��$	��700��$��10,10,10,10q�$	��Top=�._���  �� Orientation$ Vertical=�$��Center=�.+��� $ 
Horizontal=�$��Center=�$��0,5,0,0q�.+�'  !MaterialDesignThemes.Wpf.PackIcon  $��30��$��30��
�������$��Center=�   Kind$ 	Timetable=��� 
�������$��30(�$��Center=�$��Center=�$��SemiBold"�.��Scrim time finder:�� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$��85��$  5���� ���#336cb5$��DURATION��$��75��$��Center=��� ���#336cb5$��8��$��0q�$��85��$  5���� $ 
Horizontal=�$��Center=�.+�  $ Clock=����#AAAAAA$��20��$��20��$��Center=�(� $��20��.��   xstrat.Ui.NumberBox  -     Name$ DurHour��   allowNegative .     limit$ 23���� ���#AAAAAA$��Center=�$��15(�$��-5,0,0,0q�.��h(� $��20��.��  -   $ 	DurMinute�� .  $ 59���� ���#AAAAAA$��Center=�$��15(�$��-5,0,0,0q�.��min�� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$	��100��$  5���� ���#336cb5$
��DAYS��$��45��$��Center=��� ���#336cb5$��8��$��0q�$	��100��$  5���� $ 
Horizontal=�$��Center=�.+��� -   $��MoBtn��$��30��$��30��$��Mo��$��0q����#202020
�������$��Center=�$��5q� � 	Resources  �� 
TargetType �� Property	 �� Value��%:   '��       �� " ��.���  �@��CornerRadius$	 10���� -   $��TuBtn��$��30��$��30��$��Tu��$��0q����#202020
�������$��Center=�$��5q� ��%:   '��       �� " ��.���  �@��CornerRadius$	 10���� -   $��WeBtn��$��30��$��30��$��We��$��0q����#202020
�������$��Center=�$��5q� ��%:   '��       �� " ��.���  �@��CornerRadius$	 10���� -   $��ThBtn��$��30��$��30��$��Th��$��0q����#202020
�������$��Center=�$��5q� ��%:   '��       �� " ��.���  �@��CornerRadius$	 10���� -   $��FrBtn��$��30��$��30��$��Fr��$��0q����#202020
�������$��Center=�$��5q� ��%:   '��       �� " ��.���  �@��CornerRadius$	 10���� -	   $��SaBtn��$��30��$��30��$��Sa��$��0q����#202020
�������$��Center=�$��5q� ��%:   '��       �� " ��.���  �@��CornerRadius$	 10���� -
   $��SuBtn��$��30��$��30��$��Su��$��0q����#202020
�������$��Center=�$��5q� ��%:   '��       �� " ��.���  �@��CornerRadius$	 10���� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$	��100��$  5���� ���#336cb5$
��FROM��$��45��$��Center=��� ���#336cb5$��8��$��0q�$	��100��$  5���� $ 
Horizontal=�$��Center=�.+�  $ Clock=����#AAAAAA$��20��$��20��$��Center=�(� $��20��.��  -   $ FromHour�� .  $ 23���� ���#AAAAAA$��Center=�$��15(�$��-5,0,0,0q�.��h(� $��20��.��  -   $ 
FromMinute�� .  $ 59���� ���#AAAAAA$��Center=�$��15(�$��-5,0,0,0q�.��min�� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$	��100��$  5���� ���#336cb5$��TO��$��45��$��Center=��� ���#336cb5$��8��$��0q�$	��100��$  5���� $ 
Horizontal=�$��Center=�.+�  $ Clock=����#AAAAAA$��20��$��20��$��Center=�(� $��20��.��  -   $ ToHour�� .  $ 23���� ���#AAAAAA$��Center=�$��15(�$��-5,0,0,0q�.��h(� $��20��.��  -   $ ToMinute�� .  $ 59���� ���#AAAAAA$��Center=�$��15(�$��-5,0,0,0q�.��min�� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$��85��$  5���� ���#336cb5$��PLAYERS��$��75��$��Center=��� ���#336cb5$��8��$��0q�$��85��$  5��   xstrat.Ui.DataSelector  -   
  Name$
 CalendarTypeSelector��$	��150��   type$ 4���� $	��100��$
��Auto=�.���� -   $��
Playerlist��$ Vertical=�(� $��30��.��  -   $ PlayerAmount�� .  $	 100���� $��5q�$ 
Horizontal=�$��Center=�.+��� -   $��	SearchBtn��$��30��$	��120��$��Search��$��0q�$��15(����#202020
�������$��Center=� ��%:   '��       �� " ��.���  �@��CornerRadius$	 10���� -   $��NewBtn��$��30��$	��120��$��	New Scrim��$��0q�$��15(����#202020
�������$��Center=�$��10q� ��%:   '��       �� " ��.���  �@��CornerRadius$	 10��   xstrat.Ui.ColorDisplay     Color �#D64251$��	10,0,10,0q�   	NameInput$ Offday��   �#2D9ED5$��	10,0,10,0q�$ Scrim��   �#4ab859$��	10,0,10,0q�$ Recommended Scrim���� $��Center=�$��
10,20,10,0q����#888888$
��Wrap=���Create a new scrim using the button or use the search to find the perfect scrim times for your team. You can doubleclick a recommended time to create a new scrim from it or click existing scrims to edit details