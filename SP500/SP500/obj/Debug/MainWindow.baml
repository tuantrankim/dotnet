   M S B A M L   `   `   `  ����@  <SP500, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SP500SP500  5   	       SP500.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:SP500     5   	   -   5          � Title$  
MainWindow��5   	   $	��601��6   $��1156.725��6)   .��5	      � .+�5
   
   &   System.Windows.Controls.DataGrid  -   5          Name$ dataGrid��5
      $��
232,93,5,5q�6%   �� 5   
   -   5         ��Name$
 lbUp��5      $
��Left=�6"   $��10,7,0,0q�6=   $
��Wrap=�6O   $	��Top=�6c   $��Up��6{   $��46��6�   �� 5   
   -   5        $ lbDown��5      $
��Left=�6$   $��	10,48,0,0q�6?   $
��Wrap=�6R   $	��Top=�6f   $
��Down��6~   $��46��6�   �� 5   
   -   5        $ lbProcessingDate��5      $
��Left=�6.   $��
14,132,0,0q�6I   $
��Wrap=�6]   $	��Top=�6q   $��ProcessingDate��6�   $��82��6�   �� 5   
   -   5        $ lbCashAmount��5      $
��Left=�6*   $��
12,350,0,0q�6E   $
��Wrap=�6Y   $	��Top=�6m   $��Cash Amount ($)��6�   $��94��6�   �� 5   
   -   5         ��Name$ Up��5      $
��Left=�6   $��23��69   $��	10,26,0,0q�6E    �� TextWrapping$
 Wrap=�6X   $��5��6l   $	��Top=�6u   $��39��6�   �� 5   
   -   5        $
 Down��5      $
��Left=�6    $��23��6;   $��	10,58,0,0q�6G   $
 Wrap=�6Z   $��5��6n   $	��Top=�6w   $��39��6�   �� 5   
   -	   5        $ ProcessingDate��5      $
��Left=�6*   $��23��6E   $��
14,153,0,0q�6Q   $
 Wrap=�6e   $��
01/01/2000��6y   $	��Top=�6�   $	��107��6�   �� 5   
   -
   5        $ 
CashAmount��5      $
��Left=�6&   $��23��6A   $��125,343,0,0q�6M   $
 Wrap=�6b   $��10000��6v   $	��Top=�6�   $	��107��6�   �� 5   
   -   5         ��Name$ 
btnAutoRun��5      $��AutoRun��6%   $
��Left=�67   $��
559,58,0,0q�6R   $	��Top=�6f   $��75��6~   6�   �� 5   
   -   5        $ btnNext��5      $
��Next��6"   $
��Left=�61   $��
559,15,0,0q�6L   $	��Top=�6`   $��75��6x   6�   �� 5   
   -   5        $ lbValue��5      $
��Left=�6%   $��
14,182,0,0q�6@   $
��Wrap=�6T   $	��Top=�6h   $��Value��6�   $��46��6�   �� 5   
   -   5        $ Value��5      $
��Left=�6!   $��23��6<   $��
65,181,0,0q�6H   $
 Wrap=�6\   $��0��6p   $	��Top=�6y   $��78��6�    J� RenderTransformOrigin$ 8.642,0.463&�6�   �� 5   
   -   5        $ lbTrade��5      $
��Left=�6%   $��
14,229,0,0q�6@   $
��Wrap=�6T   $	��Top=�6h   $��Buy (-) / Sell (+)��6�   $��46��6�   �� 5   
   -   5        $ Trade��5      $
��Left=�6!   $��23��6<   $��
65,228,0,0q�6H   $
 Wrap=�6\   $��0��6p   $	��Top=�6y   $��78��6�   $ 8.642,0.463&�6�   �� 5   
   -   5        $ lbTop��5      $
��Left=�6#   $��	180,9,0,0q�6>   $
��Wrap=�6Q   $	��Top=�6e   $	��Top��6}   $��46��6�   �� 5   
   -   5        $ lbBottom��5      $
��Left=�6&   $��
176,45,0,0q�6A   $
��Wrap=�6U   $	��Top=�6i   $��Bottom��6�   $��46��6�   �� 5   
   -   5        $	 Top��5      $
��Left=�6   $��23��6:   $��
176,26,0,0q�6F   $
 Wrap=�6Z   $��5��6n   $	��Top=�6w   $��78��6�    �� SelectionOpacity$ 0Q�6�   �� 5   
   -   5        $ Bottom��5      $
��Left=�6"   $��23��6=   $��
176,58,0,0q�6I   $
 Wrap=�6]   $��	999999999��6q   $	��Top=�6�   $��78��6�   �� 5   
   -   5        $ btnReset��5      $��Reset Input��6#   $
��Left=�69   $��
26,105,0,0q�6T   $	��Top=�6h   $��75��6�   6�   �� 5   
   -   5        $ lbShares��5      $
��Left=�6&   $��
14,422,0,0q�6A   $
��Wrap=�6U   $	��Top=�6i   $��Shares Amount��6�   $��82��6�   $��23��6�   �� 5   
   -   5        $ SharesAmount��5      $
��Left=�6(   $��23��6C   $��115,422,0,0q�6O   $
 Wrap=�6d   $	��100��6x   $	��Top=�6�   $	��107��6�   �� 5    
   -   5        $ 	lbBalance��5       $
��Left=�6'   $��
14,282,0,0q�6B   $
��Wrap=�6V   $	��Top=�6j   $��Balance ($)��6�   $��82��6�   $��23��6�   �� 5!   
   -   5        $ Balance��5!      $
��Left=�6#   $��23��6>   $��115,282,0,0q�6J   $
 Wrap=�6_   $��10000��6s   $	��Top=�6�   $	��107��6�   �� 5"   
   -   5        $ lbBalance_Copy��5"      $
��Left=�6,   $��
14,305,0,0q�6G   $
��Wrap=�6[   $	��Top=�6o   $��Shares Balance��6�   $��82��6�   $��33��6�   �� 5#   
   -   5        $ SharesBalance��5#      $
��Left=�6)   $��23��6D   $��115,305,0,0q�6P   $
 Wrap=�6e   $	��100��6y   $	��Top=�6�   $	��107��6�   �� 5$   
   -   5        $ CashAmountMin��5$      $
��Left=�6)   $��23��6D   $��
16,382,0,0q�6P   $
 Wrap=�6d   $��	999999999��6x   $	��Top=�6�   $��90��6�   �� 5%   
   -   5        $ CashAmountMax��5%      $
��Left=�6)   $��23��6D   $��145,382,0,0q�6P   $
 Wrap=�6e   $��0��6y   $	��Top=�6�   $��87��6�   �� 5&   
   -   5        $ lbCashAmountMin��5&      $
��Left=�6-   $��	5,366,0,0q�6H   $
��Wrap=�6[   $	��Top=�6o   $	��Min��6�   $��94��6�   �� 5'   
   -   5        $ lbCashAmountMax��5'      $
��Left=�6-   $��125,366,0,0q�6H   $
��Wrap=�6]   $	��Top=�6q   $	��Max��6�   $��94��6�   �� 5(   
   -    5        $ SharesAmountMin��5(      $
��Left=�6+   $��23��6F   $��
19,466,0,0q�6R   $
 Wrap=�6f   $��	999999999��6z   $	��Top=�6�   $��90��6�   �� 5)   
   -!   5        $ SharesAmountMax��5)      $
��Left=�6+   $��23��6F   $��148,466,0,0q�6R   $
 Wrap=�6g   $��0��6{   $	��Top=�6�   $��87��6�   �� 5*   
   -"   5        $ lbSharesAmountMin��5*      $
��Left=�6/   $��	8,450,0,0q�6J   $
��Wrap=�6]   $	��Top=�6q   $	��Min��6�   $��94��6�   �� 5+   
   -#   5        $ lbShareAmountMax��5+      $
��Left=�6.   $��128,450,0,0q�6I   $
��Wrap=�6^   $	��Top=�6r   $	��Max��6�   $��94��6�   �� 5,   
   -$   5        $ lbSellAmount��5,      $
��Left=�6*   $��65,7,0,0q�6E   $
��Wrap=�6W   $	��Top=�6k   $��Sell Qty��6�   $��46��6�   �� 5-   
   -%   5        $ lbBuyQty��5-      $
��Left=�6&   $��	65,48,0,0q�6A   $
��Wrap=�6T   $	��Top=�6h   $��Buy Qty��6�   $��46��6�   �� 5.   
   -&   5        $ SellQty��5.      $
��Left=�6#   $��23��6>   $��	65,26,0,0q�6J   $
 Wrap=�6]   $��5��6q   $	��Top=�6z   $��39��6�   �� 5/   
   -'   5        $ BuyQty��5/      $
��Left=�6"   $��23��6=   $��	65,58,0,0q�6I   $
 Wrap=�6\   $��5��6p   $	��Top=�6y   $��39��6�   �� 50   
   -(   5        $ lbCashAmountPreset��50      $
��Left=�60   $��	274,7,0,0q�6K   $
��Wrap=�6^   $	��Top=�6r   $��Preset Cash ($)��6�   $��94��6�   �� 51   
   -)   5        $ CashAmountPreset��51      $
��Left=�6,   $��23��6G   $��
274,23,0,0q�6S   $
 Wrap=�6g   $��10000��6{   $	��Top=�6�   $	��107��6�   �� 52   
   -*   5        $ lbSharesPreset��52      $
��Left=�6,   $��
274,46,0,0q�6G   $
��Wrap=�6[   $	��Top=�6o   $��Preset Shares Qty��6�   $��82��6�   $��23��6�   �� 53   
   -+   5        $ SharesAmountPreset��53      $
��Left=�6.   $��23��6I   $��
274,58,0,0q�6U   $
 Wrap=�6i   $	��100��6}   $	��Top=�6�   $	��107��6�   �� 54   
   -,   5        $ 
lbFromDate��54      $
��Left=�6(   $��	395,5,0,0q�6C   $
��Wrap=�6V   $	��Top=�6j   $��ProcessingDate��6�   $��82��6�   �� 55   
   --   5        $ FromDate��55      $
��Left=�6$   $��23��6?   $��
395,23,0,0q�6K   $
 Wrap=�6_   $��
01/01/2000��6s   $	��Top=�6�   $	��107��6�   �� 56   
   -.   5        $ lbTodate��56      $
��Left=�6&   $��
395,46,0,0q�6A   $
��Wrap=�6U   $	��Top=�6i   $��ProcessingDate��6�   $��82��6�   �� 57   
   -/   5        $ ToDate��57      $
��Left=�6"   $��23��6=   $��
395,64,0,0q�6I   $
 Wrap=�6]   $��
01/01/2030��6q   $	��Top=�6�   $	��107��6�   �� 58   
   -0   5        $ btnCalibrate��58      $��	Calibrate��6'   $
��Left=�6;   $��
657,59,0,0q�6V   $	��Top=�6j   $��75��6�   6�   �� 59   
   -1   5        $ 	lbUp_Copy��59      $
��Left=�6'   $��	751,2,0,0q�6B   $
��Wrap=�6U   $	��Top=�6i   $��Up��6�   $��46��6�   �� 5:   
   -2   5        $ lbDown_Copy��5:      $
��Left=�6)   $��
751,45,0,0q�6D   $
��Wrap=�6X   $	��Top=�6l   $
��Down��6�   $��46��6�   �� 5;   
   -3   5        $ calUp��5;      $
��Left=�6!   $��23��6<   $��
751,20,0,0q�6H   $
 Wrap=�6\   $��5��6p   $	��Top=�6y   $��39��6�   �� 5<   
   -4   5        $ calDown��5<      $
��Left=�6#   $��23��6>   $��
751,58,0,0q�6J   $
 Wrap=�6^   $��5��6r   $	��Top=�6{   $��39��6�   �� 5=   
   -5   5        $ lbSellAmount_Copy��5=      $
��Left=�6/   $��	806,2,0,0q�6J   $
��Wrap=�6]   $	��Top=�6q   $��Sell Qty��6�   $��46��6�   �� 5>   
   -6   5        $ lbBuyQty_Copy��5>      $
��Left=�6+   $��
806,43,0,0q�6F   $
��Wrap=�6Z   $	��Top=�6n   $��Buy Qty��6�   $��46��6�   �� 5?   
   -7   5        $ 
calSellQty��5?      $
��Left=�6&   $��23��6A   $��
806,20,0,0q�6M   $
 Wrap=�6a   $��5��6u   $	��Top=�6~   $��39��6�   �� 5@   
   -8   5        $ 	calBuyQty��5@      $
��Left=�6%   $��23��6@   $��
806,58,0,0q�6L   $
 Wrap=�6`   $��5��6t   $	��Top=�6}   $��39��6�   �� 5A   
   -9   5        $ lbBalance_Copy1��5A      $
��Left=�6-   $��	870,2,0,0q�6H   $
��Wrap=�6[   $	��Top=�6o   $��Balance ($)��6�   $��82��6�   $��23��6�   �� 5B   
   -:   5        $ 
calBalance��5B      $
��Left=�6&   $��23��6A   $��
870,20,0,0q�6M   $
 Wrap=�6a   $��10000��6u   $	��Top=�6�   $	��107��6�   �� 5C   
   -;   5        $ lbBalance_Copy2��5C      $
��Left=�6-   $��
870,43,0,0q�6H   $
��Wrap=�6\   $	��Top=�6p   $��Shares Balance��6�   $��82��6�   $��33��6�   �� 5D   
   -<   5        $ calSharesBalance��5D      $
��Left=�6,   $��23��6G   $��
870,59,0,0q�6S   $
 Wrap=�6g   $	��100��6{   $	��Top=�6�   $	��107��6�   5F      5G      