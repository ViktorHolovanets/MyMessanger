   M S B A M L   `   `   `  ����P  LMyMessangerExamServer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:MyMessangerExamServerMyMessangerExamServer  5   	   D @Emoji.Wpf, Version=0.3.3.0, Culture=neutral, PublicKeyToken=null8*clr-namespace:Emoji.Wpf;assembly=Emoji.Wpf	Emoji.Wpf 5   	   &     MyMessangerExamServer.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:MyMessangerExamServer     5   	   <emoji*clr-namespace:Emoji.Wpf;assembly=Emoji.Wpf     5   	   _ [Hardcodet.NotifyIcon.Wpf, Version=1.1.0.0, Culture=neutral, PublicKeyToken=682384a853a08aad)tb http://www.hardcodet.net/taskbar  5	   	   -   5          �Name$  WindowServer��5   O    � Title$ Server��6	   $	��450��6   $	��800��6%    � Icon$ /Resources/server.png��61   .��5      �  � ColumnDefinitions 5   
   �� 5      $
��0.5* �6   �� 5      $��* �6   .+�5   
   3  -Hardcodet.Wpf.TaskbarNotification.TaskbarIcon  -   5          Name$ TbIInfo��5         
IconSource$ /Resources/server.ico��5         ToolTipText$ Ofline��5         	TrayPopup 5      �� 5      -   5         ��Name$ brdrTaskbarIcon��5      
�������
��� ���$��2q�6Z   	 �� CornerRadius$	 4��6n   $y�Hidden=�6   ._�5      ��    FavsScrollViewer#��[  6#   .��5      �� -   5        
 ��Name$
 tasklbOnlain��5   "   ��5      �� 5   "   .R�5   &   � .+�5   *   �� ��64   �� *GetUser.Name+5   '   5   #   5      5       5!       � ContextMenu 5#      �� 5$      $��14(�6   $	��120��6,   ��5%      �� 5&       �� 
TargetType" ��6*   .R�5'      �� ���	#FF1F2C39$	 3��6<    �� Padding$ 3q�6M   $��Center=�6Y   ._�5(   "   �� 5)      5*      ��5,      �� 5-       �� 
TargetType" m�6    .�5.      ��  �� Property � �� �� Value 5/   "   �� 50   &   " m�66   .R�51   *   �� $ brdrMenuItem��61   $��2q�6G   ���	#FF1F2C39._�52   .   �� $��Header��6?   53   +    �� Triggers 54   *   P� 55   .    P� Property �@J�IsMouseOver P� Value . . �56   2   ��  �� 
TargetName$ brdrMenuItem��69    � �� �	#FF7C49EF6}   57   /   59   '   5;      5<      .@�5>      m� -   5         m�Name$ mItOpen��5>      $��Згорнути��60   
�������6U   m� 5?      -   5        $ mItClose��5?      $��❌ Закрити��61   
�������6W   5@      5B      �� 5C   
   .+�5G      �� $
��Port��6   .��5H      �� -   5         ��Name$ tbPort��5H      $��16(�6*   6:   5I      �� 5J      -   5         ��Name$ btnStart��5J      $��2��6>   $��0 2 0 0q�6R   .��6c   Start6j   �� 5K      -	   5        $ btnStop��5K      $
��Stop��6=   5L      �� 5M   
   $��1��6   $��18(�6%   .@�5N      �� $��Онлайн��6   .��5O      �� -
   5        $
 lbOnlineUser��5O        LBViewOnline#��[ 60   5P      �� 5Q      $#��Всі користувачі��6   .��5R      �� .+�5S      �� -   5        .��5S   2   Save Change6?   �� 5T      -   5        $
 	lbAllUser��5T        	LBViewAll#��[ 61   5U      5V      5W      5X      5Y      