   M S B A M L   `   `   `  ���� A  =xstrat, Version=1.1.5.0, Culture=neutral, PublicKeyToken=nullclr-namespace:xstratxstrat  =#clr-namespace:xstrat.MVVM.ViewModelxstrat.MVVM.ViewModel  3clr-namespace:xstrat.MVVM.Viewxstrat.MVVM.View      
xstrat.App   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     &localclr-namespace:xstrat     9	viewModel#clr-namespace:xstrat.MVVM.ViewModel     /viewclr-namespace:xstrat.MVVM.View       �� 	Resources   �� MergedDictionaries �� Source �� DataType)  #xstrat.MVVM.ViewModel.HomeViewModel   xstrat.MVVM.View.HomeView-  'xstrat.MVVM.ViewModel.SettingsViewModel#   xstrat.MVVM.View.SettingsView*  $xstrat.MVVM.ViewModel.AboutViewModel    xstrat.MVVM.View.AboutView1  +xstrat.MVVM.ViewModel.SkinSwitcherViewModel'   !xstrat.MVVM.View.SkinSwitcherView-	  'xstrat.MVVM.ViewModel.RoutinesViewModel#
   xstrat.MVVM.View.RoutinesView/  )xstrat.MVVM.ViewModel.StratMakerViewModel%   xstrat.MVVM.View.StratMakerView)   #xstrat.MVVM.ViewModel.TeamViewModel   xstrat.MVVM.View.TeamView-  'xstrat.MVVM.ViewModel.CalendarViewModel#   xstrat.MVVM.View.CalendarView*  $xstrat.MVVM.ViewModel.ScrimViewModel    xstrat.MVVM.View.ScrimView*  $xstrat.MVVM.ViewModel.StatsViewModel    xstrat.MVVM.View.StatsView   HighlightedBackgroundBrush �� Color  MenuBackgroundBrush  NormalBorderBrush  SolidMenuFontBrush  HighlightedText �� 
TargetType �� Property �� Value �� 
TargetType	 �� Orientation
 K� IsItemsHost ��Name � ColumnDefinitions ��Name z� SharedSizeGroup ��Name �Name F�Name �Name �� Path �� RelativeSource �� Padding �� AncestorType � IsSharedSizeScope-  'System.Windows.Input.KeyboardNavigation   DirectionalNavigation �� Triggers P� Property P� Value �� 
TargetName J� SnapsToDevicePixels �� Stretch  ScrollThumbs �� CornerRadius  ScrollBarTrackThumb  � RowDefinitions! ]�Name" ]� Thumb# n�Name$ ]� IncreaseRepeatButton% ��Name& J� Opacity' ]� DecreaseRepeatButton( P� 
SourceName) �� Angle��  �� $ Ui/MenuButtonTheme.xaml?�%t  (��       *, +)(��      *, +)(�� $     *, +)(�� 6     *, +)(�� H     *,	 +)(�� Z     *, +)(�� l     *, +)(�� ~     *, +)(�� �     *, +)(�� �     *, +)&	  �     &	 �     &	 �     &	 �     &	     'o�     7  7  7  'm� �    7   7  7  7  7  7  '��     &	 H    &	 �    '��     �� "  .R�  �� "  .R�  �� "  .R�  �� "  .R�  �� " 	 .R�
  �� "  .R�  �� "  .R�  �� "  .R�  �� "  .R�  �� "  .R�  �� $ #336cb5���� $ #505050���� $ #202020���� $ 	#FFFFFFFF���� $ 	#FFFFFFFF���� " o�.���  �@�OverridesDefaultStyle . ��  �@J�SnapsToDevicePixels . ��  � ��8   ��  � �� �� " o�.R��� 8�� 8�� $��0q�._��� }�. $	 
Horizontal=�
 . �� " m�.���  � �� �� " m�.R��� $ Border��#��z��#��z��$��0q�._��  �� $
 Col0��$��17��$
��Auto �$ MenuItemIconColumnGroup���� $
��Auto �$ MenuTextColumnGroup���� $
��Auto �$ MenuItemIGTColumnGroup���� $
 Col3��$��14 �.+��� $
 Icon��$��0��$��4,0,6,0q�$��Center=�$
��Icon��� $ 
GlyphPanel��$��0��$y�Hidden=�$��4,0,6,0q�$��Center=�.+�F� $ GlyphPanelpath��$��Center=�#��z��=����    �� �     >I Ѡ%& @�p �`Vl ��* �`Vl     Ѡ%& �0F 	$��LeftToRight=��� $ 
HeaderHost��$��1��#��z����. $��Header���� $ IGTHost��$��2��$��8,1,8,1q�$��InputGestureText��$��Center=�� $ 
ArrowPanel��$��3��$��4,0,6,0q�$��Center=�.+�F� $ ArrowPanelPath��$��Right=�$��Center=�#��z��"����        �     z � 	=  	= 	� $ SubMenuPopup��$��Right=�|�.  #��� -����� $ IsSubmenuOpen� �� *TemplatedParent+.���� $ SubMenuBorder��$��0q�$ 2,2,2,2q����� $ 
Foreground� �� " o�._�� $ SubMenu�� . .+��� 
 . $ Cycle=� P�  �@m�Role$ TopLevelHeader=�. ���  � ��$ 6,1,6,1q��� $ SubMenuPopup�� � ��$ Bottom=��� $
 Col0�� � ��$ 0���� $
 Col3�� � ��$
 Auto ��� $
 Icon�� � y�$ 	Collapsed=��� $ 
GlyphPanel�� � y�$ 	Collapsed=��� $ IGTHost�� � y�$ 	Collapsed=��� $ 
ArrowPanel�� � y�$ 	Collapsed=�P�  �@m�Role$ TopLevelItem=�. ���  � ��$ 6,1,6,1q��� $ 
ArrowPanel�� � y�$ 	Collapsed=�P�  �@m�Role$ SubmenuHeader=�. ���  � ��$	 Top=���  � ��$ 0,2,0,2q�P�  �@m�Role$ SubmenuItem=�. ���  � ��$	 Top=���  � ��$ 0,2,0,2q��� $ 
ArrowPanel�� � y�$ 	Collapsed=�P� " �@m�IsSuspendingPopupAnimation . . ��� $ SubMenuPopup�� � ��$
 None=�P�  Z�  �@m�Icon. ��� $
 Icon�� � y�$ 	Collapsed=�P�  �@m�	IsChecked . . ��� $ 
GlyphPanel�� � y�$ Visible=��� $
 Icon�� � y�$ 	Collapsed=�P�  �@m�IsHighlighted . . ��� 8   $ Border�� � ���� 8   � ���� 8  $ Border�� � ��P�  �@m�IsHighlighted .  . ��� 8  $ Border�� � ���� 8   � ���� 8  $ Border�� � ��P�  � {� .  . ���  � ��
 ������� " ��.���  �@�OverridesDefaultStyle . ��  � �� �#303030��  � ��
 ���� ��  � �� �� " ��.R��� #��z�� . $��10��$��10��._�F� ����pH$u {f�@QȨ� {f�@QȨ� 6��/\#@QH$u 6��/\#@p�9 {f�@q��!�$@{f�@q��!�$@6��/\#@Q�9 6��/\#@p    {f�@Q�� {f�@Q�� 6��/\#@Q    6��/\#@p�9 !����@q��!�$@!����@q��!�$@P�O�O@Q�9 P�O�O@pH$u <���� @QȨ� <���� @QȨ� �Gb@QH$u �Gb@�H$u     �Ȩ�     �Ȩ� �� �H$u �� 	#��z��$
��9.53��$
 Fill=�$	��Top=�$��9.677���� " n�.���  � �� �� " n�.R�� $
 Grid��.+��� $��Stretch=�$��Stretch=�$
��Auto��$
��Auto��
������ �� $ 
Rectangle1��$ 10��$��Stretch=�$��Stretch=�$
��Auto��$
��Auto��#��z�� P�  �@�Tag$ 
Horizontal��. ��� $ 
Rectangle1�� � ��$
 Auto���� $ 
Rectangle1�� � ��$ 7���� " n�.���  � �� �� " n�.R�� $
 Grid��.+��� $��Stretch=�$��Stretch=�$
��Auto��$
��Auto��
������ �� $ CornerScrollBarRectangle��$ 3��$��Stretch=�$��Stretch=�$
��Auto��$
��Auto��$��0,1,0,1q�#��z�� P�  �@�Tag$ 
Horizontal��. ��� $ CornerScrollBarRectangle�� � ��$
 Auto���� $ CornerScrollBarRectangle�� � ��$ 6���� " ��.���  �@��IsFlicksEnabled .  ��  � �� �#656565��  � �� �#303030��  � ��$ 7����  � �� �� " ��.R�� $ GridRoot��$��7��#��z��  �� $��0.00001* �.+�]� $! 
PART_Track��$��0����. |�.  " n� $# Thumb��#��z��#���  $ �� $% PageUp��$��ScrollBar.PageDownCommand��$& 0Q�|�.  ' �� $% PageDown��$��ScrollBar.PageUpCommand��$& 0Q�|�.   P� $( Thumb�� �@J�IsMouseOver . . ��� $ Thumb�� � �� �#808080P� $( Thumb�� �@n�
IsDragging . . ��� $ Thumb�� � �� �#555555P�  � {� .  . ��� $ Thumb�� � y�$ 	Collapsed=�P�  �@��Orientation$ 
Horizontal=�. ��� $ GridRoot�� �@�LayoutTransform �� $	) -90Q��� $ 
PART_Track�� �@�LayoutTransform �� $	) -90Q���  � ��$
 Auto����  � ��$ 8���� $ Thumb�� �@�Tag$ 
Horizontal���� $ PageDown�� � ��$ ScrollBar.PageLeftCommand���� $ PageUp�� � ��$  ScrollBar.PageRightCommand��