   M S B A M L   `   `   `  ���� A  =xstrat, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null3clr-namespace:xstrat.MVVM.Viewxstrat.MVVM.View  %clr-namespace:xstrat.Ui	xstrat.Ui  #    xstrat.MVVM.View.SettingsView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     0localclr-namespace:xstrat.MVVM.View     *local1clr-namespace:xstrat.Ui     .����   �� Orientation$  
Horizontal=�$	��775��$
��1150��.+��� $  Vertical=�.+��� $��0q����#202020 �� CornerRadius$ 10��$	��300��$	��200��$��10,10,10,10q�._��� $  Vertical=�$��5,5,5,5q�.+��� 
�������$��18(�$��General:��$��Center=�$��SemiBold"��� $  Vertical=�.+��� $  
Horizontal=�$
��Left=�.+��� 
�������$��14(�$��Right=�.��Stay logged in for 30 days   xstrat.Ui.ToggleSwitch  -     Name$ RememberMeSettings��$��20���� $  
Horizontal=�.+��� 
�������$��Bottom=�.��
Api-Url:�� -   $��APIText�����#336cb5$��Center=�
������ $��25��$��0q�.��https://api.xstrat.app�� $  
Horizontal=�.+��� 
�������$��Bottom=�.��Account Name:�� -   $��	UbiIDText�����#336cb5$��Center=�
������ $��25��$��0q�.��	Your_Name�� $��0q����#202020$ 10��$	��300��$	��230��$��10,10,10,10q�._��� $  Vertical=�$��5,5,5,5q�.+��� 
�������$��18(�$��Discord:��$��Center=�$��SemiBold"��� $  Vertical=�.+��� $  
Horizontal=�.+��� 
�������$��Bottom=�.��Discord-ID:�� -   $
��DCId�����#336cb5$��Center=�
������ $��25��$��0q�.��
1234567890�� -   $��DcAdminView��$  Vertical=�.+��� -   $��WebhookView��$  
Horizontal=�.+��� 
�������$��Bottom=�.��Discord-Webhook:�� -   $��	DCWebhook�����#336cb5$��Center=�
������ $��25��$��0q�.��&$https://discord.com/api/webhooks/...�� -   $��DiscordNotify1��$  
Horizontal=�.+��� 
�������$��Bottom=�.��New scrim notification:  -	   $ SwNew��$��20���� -
   $��DiscordNotify2��$  
Horizontal=�.+��� 
�������$��Bottom=�.��" Scrim time changed notification:  -   $ SwTimeChanged��$��20���� -   $��DiscordNotify3��$  
Horizontal=�.+��� 
�������$��Bottom=�.��#!Scrim starting soon notification:  -   $ SwStartingSoon��$��20���� -   $��DiscordNotify4��$  
Horizontal=�.+��� 
�������$��Bottom=�.��Weekly scrim summary:  -   $ SwWeeklySummary��$��20���� -   $��DiscordNotify5��$  
Horizontal=�.+��� 
�������$��Bottom=�.��" Starting soon time (in minutes):(� $��20��$��30��.��   xstrat.Ui.NumberBox  -     Name$ ScrimTimeDelay��   allowNegative .     Value$ 60��   limit$	 300���� $��0q����#202020$ 10��$	��300��$	��200��$��10,10,10,10q�._��� $  Vertical=�$��5,5,5,5q�.+��� 
�������$��18(�$��Skin Switcher:��$��SemiBold"�$��Center=��� 
�������$��14(�$��Savegames folder:��$��Center=�$��0,0,0,0q��� -   $��SkinSwitcherPathDisplay�����#252525$
��Wrap=�
�������$��10(�$��40��$�� ���� -   $��SkinSwitcherPickPathBtn��$��
Pick here:��
����������#252525$��0q�$��0,10,0,0q�$��30��$��90�� � 	Resources  �� 
TargetType	 �� Property
 �� Value��%:   '��       �� " ��.��� 	 �@��CornerRadius$
 10���� $��0,20,0,0q����#606060
������ $��WrapWithOverflow=�hfSelect this folder: "C:\Program Files (x86)\Ubisoft\Ubisoft Game Launcher\savegames\YourAccountID\635"�� -   $��SaveBtn��$
��Save��
����������#336cb5$��0q�$��10,10,10,10q�$��40��$	��300��$��22(� ��%:   '��       �� " ��.��� 	 �@��CornerRadius$
 10��