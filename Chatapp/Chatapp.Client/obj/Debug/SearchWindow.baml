   M S B A M L   `   `   `  ����I  EChatapp.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Chatapp.ClientChatapp.Client  5      9!clr-namespace:Chatapp.Client.DataChatapp.Client.Data  5      !    Chatapp.Client.SearchWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      9d+http://schemas.microsoft.com/xaml/behaviors     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      .localclr-namespace:Chatapp.Client     5      2data!clr-namespace:Chatapp.Client.Data     5      H DMahApps.Metro, Version=1.6.5.1, Culture=neutral, PublicKeyToken=null8metro,http://metro.mahapps.com/winfx/xaml/controls  5	      S OMaterialDesignThemes.Wpf, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullX TMaterialDesignThemes.MahApps, Version=0.0.11.0, Culture=neutral, PublicKeyToken=nullHmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes   5
      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5         MaterialDesignPaper#���   5        � Title$  Founded friends��5      $	��300��6,   $	��300��69   (  "MahApps.Metro.Controls.MetroWindow   	TitleCaps .     TitleCharacterCasing$ Normal=�6W      ShowTitleBar .   PrimaryHueDarkBrush#���  5   (    � DataContext 5      ��   	VMLocator �� Source# [  �� Path$ SearchVM� � 	Resources 5        AddButtonStyle �� 
TargetType	 �� Property
 �� Value �� 
TargetType'  !MaterialDesignThemes.Wpf.PackIcon   Kind J� SnapsToDevicePixels �� Triggers P� Property P� Value �� 
TargetName  PrimaryHueMidBrush  SearchButtonStyle �� BasedOn   ParticipantsListBoxItemStyle �� RelativeSource �� AncestorType ��Name �� Padding ]� 
Conditions �� Property �� Value  ParticipantsDataTemplate  MaterialDesignDivider � ColumnDefinitions ��Name J� OpacityMask !� Visual �� ElementName A�Name 	 ByteBmpSrcConverter  �� 	Converter 
 
BlankImage! �� TargetNullValue" �� Triggers# �� Value$ �� Binding��%  &	       &	 �    7  &	 {    &	 �
    7 	 7 
 �� 5      " ��6"   .�5      �� 	 � ��

 ���� 6"   �� 5      	 � ��

 ���� 6#   �� 5      	 � ��#
 �  6"   �� 5      	 �@�Cursor$

 Hand��6   �� 5      	 � ��$
 Medium"�6"   �� 5      	 � ��$
 14(�6    �� 5      	 � ��$
 1q�6'   �� 5      	 � ��$
 Stretch=�62   �� 5      	 � ��$
 Stretch=�60   �� 5      	 � ��$
 0q�6   �� 5      	 � ��
 5      �� 5       " ��6   .R�5!      � ���	#00000000.+�5"   	     $��SendIcon��6!   $	 Add=�61    . #��z��5#   6   #��z��5$   6   #��z��5%   6   #��z��5&   6   $
��Auto��6Y   $
��Auto��6f   5'   	    5(      P� 5)   	    �@J�IsMouseOver . . �5*   
   �� $ SendIcon��6'   #
 �  6=   	 � ��5+   
   P� 5,   	    �@J�IsKeyboardFocused . . �5-   
   �� $ SendIcon��6'   #
 �  6=   	 � ��5.   
   P� 5/   	    � {� .  . �50   
   �� 	 �@J�Opacity$

 0.23Q�6$   51   
   53      55      56      �� 58      " ��6$   8   .�59      �� 	 � ��
 5:      �� 5;      " ��6   .R�5<      � ���	#00000000.+�5=   	     $��
SearchIcon��6!   $ Search=�63    . #��z��5>   6   #��z��5?   6   #��z��5@   6   #��z��5A   6   $
��Auto��6Y   $
��Auto��6f   5B   	    5C      P� 5D   	    �@J�IsMouseOver . . �5E   
   �� $ 
SearchIcon��6'   #
 �  6?   	 � ��5F   
   P� 5G   	    �@J�IsKeyboardFocused . . �5H   
   �� $ 
SearchIcon��6'   #
 �  6?   	 � ��5I   
   P� 5J   	    � {� .  . �5K   
   �� 	 �@J�Opacity$

 0.23Q�6$   5L   
   5N      5P      5Q      �� 5S      " ��6/   .�5T      �� 	 � ��

 ���� 6"   �� 5U      	 � ��
 5V      �� *HorizontalContentAlignment+ �� " ���� 5W      	 � ��
 5X      �� *VerticalContentAlignment+ �� " ���� 5Y      	 � ��$
 2,0,0,0q�6   �� 5Z      	 � ��$
 0q�6   �� 5[      	 � ��
 5\      �� 5]      " ��6   .R�5^      �� $ Bd��6   #��z��5_   !   #��z��5`   !   #��z��5a   !   # z��5b   !    . ._�5c   	   �� $��1��6   #��z��5d   /   # z 5e   /   #��z��5f   /   5g   	    5h      P� 5i   	    �@��
IsSelected . . �5j   
   �� $ Bd��6'   	 � ��
 �#E3F2FD67   �� 5k   
   #
 � .�5l   )   	 � ��5m   
   ]� 5n   	    5o   
   �� 5p       �@��
IsSelected . 6+   �� 5q       �@��IsSelectionActive .  6;   ./�5s   
   �� $ Bd��6'   	 � ��
 �#E3F2FD67   �� 5t   
   #
 � .�5u   )   	 � ��5v   
   P� 5w   	    � {� .  . �5x   
   �� #
 � ��6'   	 � ��5y   
   5{      5}      5~      �� 5�      .R�5�      �� $��0,0,0,1q�6   #���  6&   $��50��5�      $��-2,0,0,0q�6!   ��5�      �� $ ActualWidth� �� *FindAncestor+$ ListBoxItemL�._�5�      �  5�      �� 5�      $��5 �6   �� 5�      $��50 �6   �� 5�      �� 5�      $��20 �6   �� 5�      $��20 �6   .+�5�      �� $ ParticipantRct��6   #���  6)   $y�Hidden=�5�       � 5�      $��1��6   $��6q�6    .  5�      !� 5�   	    6   �� $ ClipEllipse��.+�5�      A� $ ClipEllipse��6   
�������6%     5�      $ AccountCircle=�6     . $
��Auto��6P   $
��Auto��6]   $��-4q�5�   2   $��Stretch=�6>   $��Stretch=�6\   �� 5�      $��UniformToFill=�5�   K   ��5�      �� *Photo+8    8!  5�   K   5�      �� 5�      $��2��6   $��Center=�6!   $
��Left=�6<   $	��5,0q�5�       $��SemiBold"�6-   $��CharacterEllipsis=�6C    . ��5�       �� *Name+66   �� 5�      $��4��6   ���	#FF6E6E6E#���  5�   !   $��Right=�5�   !   ��5�      �� * DataContext.SendRequestCommand+$
 root��5�      5�      " 5�      �� 5�      $
# true��5�   ^   $ 5�      �� $ 
IsSelected� �� *FindAncestor+$ ListBoxItemL�.a�5�      �� $ ParticipantRct��6$   	 � y�$
 Visible=�6@   5�      5�      5�      .��5�      � % � RowDefinitions% 5�      �� 5�      $��50 �6   �� 5�      �� 5�      $
��Auto �6   .+�5�      �� -   5        & ��Name$& searchUsername��5�      $��1��6   $��20, 10, 50, 10q�64   )  #MaterialDesignThemes.Wpf.HintAssist'   Hint$' Search for a friend��5�   "   $��Stretch=�6W   ��5�   "   �� *SearchUsername+( �� UpdateSourceTrigger$( PropertyChanged=��� 5�      $��1��6   ���	#FF6E6E6E$��4q�62   #���  5�   !   $��Right=�5�   !   ��5�      �� *SearchCommand+�� 5�      $��1��6   $��1,1,1,1q�6    . #���  5�      ._�5�      �� -   5        ) ��Name$
) root��5�      #���  5�      #���  5�      $��Disabled=�5�      $
��Auto=�5�      ��5�      �� *Participants+* �� SelectedItem* 5�      �� *SelectedParticipant+5�      5�      5�      5�      