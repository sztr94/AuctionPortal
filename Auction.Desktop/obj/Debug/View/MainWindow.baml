   M S B A M L   `   `   `  ����J  FAuction.Desktop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:Auction.Desktop.ViewModelAuction.Desktop.ViewModel  5   	   1clr-namespace:Auction.DesktopAuction.Desktop  5   	        Auction.Desktop.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   =	viewModel'clr-namespace:Auction.Desktop.ViewModel     5   	   /localclr-namespace:Auction.Desktop     5   	     � Title$0  *Aukciós portál - Tárgyak karbantartása��5	   	   $	��400��68   $	��400��6E   $	��450��6Q   $	��400��6a    � 	Resources 5
      6   0Auction.Desktop.ViewModel.BiddingAmountConverter   biddingAmountConverter��%   &	          5   
   6+   5      .��5      � .+�5   
   �  � RowDefinitions 5   
   �� 5      $��25 �6   �� 5      $��* �6   �� 5      $��32 �6    � ColumnDefinitions 5   
   �� 5      $	��400 �6   �� 5      $��* �6   .+�5   
   o� $��0��6   $��2��6   .@�5      m� $��
Adatbázis��6   .@�5      m� $��Új tárgy felvétele��6    m� Command 6;   �� *CreateObjectCommand+{�6c   �� *
IsLoaded+m� 5      $��Tárgyak listázása��6    66   �� *LoadCommand+m� 5      $��Változtatások mentése��6    6:   �� *SaveCommand+{�6Z   �� *
IsLoaded+�� 5      m� 5      $��Kijelentkezés��6    62   �� *LogOutCommand+5       5!      &   System.Windows.Controls.DataGrid  5#      -   5        $��
objectGrid��5#      $��1��6)   $��2��66      AutoGenerateColumns .     
IsReadOnly . ��. $
��Auto=�6�   ��6x   �� *	Objects+ �� SelectedItem 6�   �� *SelectedObject+   Columns 5$      0  *System.Windows.Controls.DataGridTextColumn  5%      ,  &System.Windows.Controls.DataGridColumn	   Header$
	 Név��6)   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 66   �� *Name+  5&      $	 Ár (Ft)��6)   
 6:   �� *ActualPrice+5(      �� 5)      $��2��6    �� Orientation$ 
Horizontal=�6&   $
��Left=�6?   .+�5*      �� $��Tárgy megtekintése��6   $��6,5,0,5q�66   $	��100��6�   ��6G   �� *ViewObjectCommand+��6n   �� *SelectedObject+{�6�   �� *
IsLoaded+6�   5+      5,      5-      5.      