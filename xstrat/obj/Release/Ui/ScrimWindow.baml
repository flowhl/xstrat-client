   M S B A M L   `   `   `  ���� A  =xstrat, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null%clr-namespace:xstrat.Ui	xstrat.Ui      xstrat.Ui.ScrimWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:xstrat.Ui     _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  -     � Title$  Scrim��$
��Auto��$	��700�����#161616 � ShowInTaskbar .   � WindowStartupLocation$ CenterOwner=�.��� .+���  �� Orientation$ Vertical=�.+��� -   $��	TypeLabel�����#336cb5$��Center=�$��18(�$��SemiBold"�.��Create Scrim�� $ 
Horizontal=�$��Center=�.+��� $	��250��$��Right=�$��Center=�
�������$��28(�$
��Bold"�.��	Your Team�� $��Center=����#336cb5$��38(�$
��Bold"�.��VS�� -   $��OpponentNameBox��$	��250��$
��Left=�$��Center=�
�������$��28(�$��SemiBold"�
������ $��0q�.��Name�� -   $��TitleBox��
����������#202020$��0q�$��24(� �� AcceptsReturn .  $��Center=�$��0,5,0,0q�$	��500��.��Title�� -   $��
CommentBox��
����������#202020$��0q�$��15(� �� TextWrapping$
 Wrap=�$��Center=�$��0,5,0,0q�$	��500��.��Descr�� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$	��225�� �� CornerRadius$ 5���� ���#336cb5$
��DATE��$��45��$��Center=��� ���#336cb5$��8��$��0q�$	��225��$ 5��(  "System.Windows.Controls.DatePicker  -   $��CalendarDatePicker��
������ $��4q�$��Center=�$��0q����#336cb5$	��150��   CalendarStyle ��  �� 
TargetType&   System.Windows.Controls.Calendar"  .��� 	 �� Property	 � ��
 �� Value

 �   ��� 	 � ��$
 0q� � 	Resources :  4System.Windows.Controls.Primitives.DatePickerTextBox��%`   '        �� "  .��� 	 � ��

 ���� �� 	 � ��$
 0q��� 	 � ��
 �#336cb5�� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$	��225��$ 5���� ���#336cb5$
��FROM��$��45��$��Center=��� ���#336cb5$��8��$��0q�$	��225��$ 5���� $ 
Horizontal=�$��Center=�.+�'  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Clock=����#AAAAAA$��20��$��20��$��Center=�(� $��20��.��   xstrat.Ui.NumberBox  -     Name$ FromHour��   allowNegative .     limit$ 23���� ���#AAAAAA$��Center=�$��15(�$��-5,0,0,0q�.��h(� $��20��.��  -   $ 
FromMinute�� .  $ 59���� ���#AAAAAA$��Center=�$��15(�$��-5,0,0,0q�.��min�� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$	��225��$ 5���� ���#336cb5$��TO��$��45��$��Center=��� ���#336cb5$��8��$��0q�$	��225��$ 5���� $ 
Horizontal=�$��Center=�.+�  $ Clock=����#AAAAAA$��20��$��20��$��Center=�(� $��20��.��  -	   $ ToHour�� .  $ 23���� ���#AAAAAA$��Center=�$��15(�$��-5,0,0,0q�.��h(� $��20��.��  -
   $ ToMinute�� .  $ 59���� ���#AAAAAA$��Center=�$��15(�$��-5,0,0,0q�.��min�� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$	��225��$ 5���� ���#336cb5$
��Maps��$��45��$��Center=��� ���#336cb5$��8��$��0q�$	��225��$ 5���� $ Vertical=�.+�   xstrat.Ui.DataSelector  -     Name$ MapSelector1��   type$ 5��$	��200��$��0,5,0,0q�  -   $ MapSelector2��$ 5��$	��200��$��0,5,0,0q�  -   $ MapSelector3��$ 5��$	��200��$��0,5,0,0q��� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$	��225��$ 5���� ���#336cb5$
��Mode��$��45��$��Center=��� ���#336cb5$��8��$��0q�$	��225��$ 5��  -   $ ScrimModeSelector��$ 6��$	��200���� -   $��
UserViewer��$ Vertical=�.+��� $ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� ���#336cb5$��8��$��0q�$	��225��$ 5���� ���#336cb5$��Player��$��45��$��Center=��� ���#336cb5$��8��$��0q�$	��225��$ 5���� $��45��$��Disabled=�$
��Auto=�$��	60,0,60,0q�.���� -   $��PlayerStack��$ 
Horizontal=�$��30��$��Center=��� $��0,10,0,0q�$ Vertical=�.+��� -   $��CreatorLabel�����#808080$��Center=��� -   $��CreationDateLabel�����#808080$��Center=��� $ 
Horizontal=�$��	0,10,0,30q�$��Center=�.+��� -   $��SaveBtn��$��40��$	��250��
�������$
��Save��$��17(����#202020$��0q�$��10q� ��%:   '��       �� " ��.��� 	 �@��CornerRadius$
 10���� -   $��	DeleteBtn��$��40��$	��250��
�������$��Delete��$��17(����#202020$��0q�$��10q� ��%:   '��       �� " ��.��� 	 �@��CornerRadius$
 10��