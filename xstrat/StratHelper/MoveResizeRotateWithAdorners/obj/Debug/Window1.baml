   M S B A M L   `   `   `  ����W  SMoveResizeRotateWithAdorners, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DiagramDesignerDiagramDesigner  5   @       DiagramDesigner.Window1   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   9diagramdesignerclr-namespace:DiagramDesigner     6@     � WindowStartupLocation$  CenterScreen=�5   	    � Title$ WPF Diagram Designer��5   	   ���#303030$	��600��5   	   $	��880��6    � 	Resources 5	       �� MergedDictionaries �� Source�� 5
       5      �� 5   
   $! Resources/DesignerItem.xaml?�6   %    5      .��5      � .+�5      �� -   5        $�� s e l e c t��5      $��	20,10,0,0q�5      
�������$��UISegoe)�5      $��15(�5      $
��Left=�5      $	��Top=�5      �� 5      -   5         ��Name$ DesignerCanvas��5      }�.  J� SnapsToDevicePixels . .+�5!      )   #DiagramDesigner.StratContentControl  $	��100��6,   $	��100��5"      $��1q�5#       �� Left$	 710��5$      
 �� Top$	 300��5%         DesignerItemStyle#��[  5&      .��5'   
   A� 	 J� IsHitTestVisible	 .  ��5(      
� 5)      
 
� Center$
 0.2, 0.2&�6"    
� GradientOrigin$ 0.2, 0.2&�64    
� RadiusX$	 0.8Q�6N    
� RadiusY$	 0.8Q�6\   .��5*      �  � Color$ 	LightBlue��6    � Offset$ 0Q�6/   � 5+      $
 Blue��6   $	 0.9Q�6*   5,      5.      5/   	     52      $	��100��6,   $	��100��53      $��1q�54      $	 640��55      $	 320��56      #��[  57      .��58   
   A� 	 .  ��59      
� 5:      $
 0.2,0.2&�6"   $ 0.2,0.2&�63   $	 0.8Q�6L   $	 0.8Q�6Z   .��5;      � $
 Lime��6   $ 0Q�6*   � 5<      $ 	#FF00AA00��6   $	 0.9Q�6/   5=      5?      5@   	     5C      $	��100��6,   $	��100��5D      $��1q�5E      $	 695��5F      $	 370��5G      #��[  5H      .��5I   
   A� 	 .  ��5J      
� 5K      $
 0.2,0.2&�6"   $ 0.2,0.2&�63   $	 0.8Q�6L   $	 0.8Q�6Z   .��5L      � $ White��6   $ 0Q�6+   � 5M      $ 	#FFFF8300��6   $	 0.9Q�6/   5N      5P      5Q   	     5T      $	��100��6,   $	��100��5U      $	 210��5V      $	 220��5W      #��[  5X      .��5Y   
   �� 	 .  $
��Fill=�6)   $��Media/cross.png��68   5Z   	     5]      $	��100��6,   $	��100��5^      $ 75��5_      $	 385��5`      #��[  5a      .��5b      �� 	 .  $
��Fill=�61   $��Media/paint.png��6@   5c      5d      5e      5f      