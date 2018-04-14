using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;

namespace SP500
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SynchronizationContext synchronizationContext;
        CancellationTokenSource cts;
        private DateTime previousTime = DateTime.Now;
        DateTime processingDate, fromDate, toDate;
        bool isBuy = false;

        double tradePrice;
        double up, cUp;
        double down, cDown;
        double buyQty, sellQty, cBuyQty, cSellQty;
        double cashAmountPreset, cashAmount, cashAmountMin, cashAmountMax;
        double sharesAmountPreset, sharesAmount, sharesAmountMin, sharesAmountMax;
        double balance
        {
            get
            {
                return cashAmount + sharesAmount * currentValue;
            }
        }
        double sharesBalance
        {
            get
            {
                return sharesAmount + cashAmount/currentValue;
            }
        }
        double cBalance;
        double cSharesBalance;
        double currentValue;

        Index[] values = {};
        int currentIdx = 0;
        public MainWindow()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnReset_Click(this, null);
        }
        
        

        private void Calculate()
        {
            if (currentIdx >= values.Length) return;

            Index idx = values[currentIdx];
            currentValue = idx.Close;

            if (currentValue > tradePrice + up)//sell
            {
                //lbTrade.Text = "Sell (+)";
                isBuy = false;
                tradePrice = currentValue;
                cashAmount = cashAmount + tradePrice * sellQty;
                sharesAmount-=sellQty;
                if (sharesAmount < sharesAmountMin) sharesAmountMin = sharesAmount;
                if (cashAmount > cashAmountMax) cashAmountMax = cashAmount;
            }
            else if (currentValue < tradePrice - down)//buy
            {
                //lbTrade.Text = "Buy (-)";
                isBuy = true;
                tradePrice = currentValue;
                cashAmount = cashAmount - tradePrice * buyQty;
                sharesAmount+=buyQty;
                if (sharesAmount > sharesAmountMax) sharesAmountMax = sharesAmount;
                if (cashAmount < cashAmountMin) cashAmountMin = cashAmount;
            }

        }

        private void UpdateUI()
        {
            if (currentIdx >= values.Length) return;

            var timeNow = DateTime.Now;

            if ((DateTime.Now - previousTime).Milliseconds <= 5) return;
            previousTime = timeNow;

            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                if (currentIdx >= values.Length) return;
                Index idx = values[currentIdx];
                ProcessingDate.Text = idx.Date.ToShortDateString();
                Value.Text = idx.Close.ToString();
                Trade.Text = tradePrice.ToString();
                CashAmount.Text = cashAmount.ToString();
                CashAmountMin.Text = cashAmountMin.ToString();
                CashAmountMax.Text = cashAmountMax.ToString();
                
                SharesAmount.Text = sharesAmount.ToString();
                SharesAmountMin.Text = sharesAmountMin.ToString();
                SharesAmountMax.Text = sharesAmountMax.ToString();

                Balance.Text = balance.ToString();
                SharesBalance.Text = sharesBalance.ToString();

                if(isBuy) lbTrade.Text = "Buy (-)";
                else lbTrade.Text = "Sell (+)";

                dataGrid.SelectedIndex = currentIdx;
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);

                //Update Calibration
                //calUp.Text = cUp.ToString();
                //calDown.Text = cDown.ToString();
                //calBuyQty.Text = cBuyQty.ToString();
                //calSellQty.Text = cSellQty.ToString();
                //calBalance.Text = cBalance.ToString();
                //calSharesBalance.Text = cSharesBalance.ToString();
                
            }), currentIdx);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            currentIdx = ++dataGrid.SelectedIndex;
            for(; currentIdx < values.Length; currentIdx++)
            {
                if (values[currentIdx].Date < fromDate) continue;
                if (values[currentIdx].Date >= fromDate) break;
            }
            
            if (currentIdx >= values.Length) return;
            Calculate();
            UpdateUI();
        }
        private async void btnAutoRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnAutoRun.IsEnabled = false;
                if (dataGrid.SelectedIndex < 0) dataGrid.SelectedIndex = 0;
                currentIdx = dataGrid.SelectedIndex;
                
                cts = new CancellationTokenSource();
                int count = await RunningTask(cts);
                if (currentIdx >= values.Length) currentIdx = values.Length-1;
                previousTime = DateTime.MinValue;
                UpdateUI();
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                MessageBox.Show("Counter " + count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            { btnAutoRun.IsEnabled = true; }
        }

        private async Task<int> RunningTask(CancellationTokenSource cts)
        {
            int count = 0;
            await Task.Run(() =>
            {
                try
                {
                    for (; currentIdx < values.Length; currentIdx++)
                    {
                        if (values[currentIdx].Date >= fromDate && values[currentIdx].Date <= toDate)
                        {
                            count++;
                            cts.Token.ThrowIfCancellationRequested();
                            Calculate();
                            UpdateUI();
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    synchronizationContext.Post(new SendOrPostCallback(o =>
                    {
                        //error.Text = (string)o;
                    }), ex.Message);
                }
                finally
                {
                    //Don't return insid finally bc of IDisposable
                }
            });
            return count;

        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            tradePrice = Convert.ToDouble(Trade.Text);
            up = Convert.ToDouble(Up.Text);
            down = Convert.ToDouble(Down.Text);
            buyQty = Convert.ToDouble(BuyQty.Text);
            sellQty = Convert.ToDouble(SellQty.Text);

            cashAmountPreset = cashAmountMin = cashAmountMax = cashAmount = Convert.ToDouble(CashAmountPreset.Text);
            sharesAmountPreset = sharesAmountMin = sharesAmountMax = sharesAmount = Convert.ToDouble(SharesAmountPreset.Text);
            currentValue = Convert.ToDouble(Value.Text);

            fromDate = Convert.ToDateTime(FromDate.Text);
            toDate = Convert.ToDateTime(ToDate.Text);


            CashAmount.Text = cashAmount.ToString();
            CashAmountMin.Text = cashAmountMin.ToString();
            CashAmountMax.Text = cashAmountMax.ToString();

            SharesAmount.Text = sharesAmount.ToString();
            SharesAmountMin.Text = sharesAmountMin.ToString();
            SharesAmountMax.Text = sharesAmountMax.ToString();

            Balance.Text = balance.ToString();
            SharesBalance.Text = sharesBalance.ToString();

            values = File.ReadAllLines("HistoricalQuotes.csv")
                                           .Skip(2)
                                           .Select(v => Index.FromCsv(v))
                                           .OrderBy(i => i.Date)
                                           .ToArray();
            ProcessingDate.Text = DateTime.MinValue.ToShortDateString();
            dataGrid.ItemsSource = values;
        }

        private async void btnCalibrate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnAutoRun.IsEnabled = false;
                btnCalibrate.IsEnabled = false;
                cSharesBalance = 0;
                cBalance = 0;

                for (int up = 1; up < 10; up++)
                {
                    for (int down = 1; down < 10; down++)
                    {
                        for (int sellQty = 1; sellQty < 10; sellQty++)
                        {
                            for (int buyQty = 1; buyQty < 10; buyQty++)
                            {

                                currentIdx = 0;
                                currentValue = 0;
                                cashAmountPreset = cashAmountMin = cashAmountMax = cashAmount = Convert.ToDouble(CashAmountPreset.Text);
                                sharesAmountPreset = sharesAmountMin = sharesAmountMax = sharesAmount = Convert.ToDouble(SharesAmountPreset.Text);
                                

                                Up.Text = up.ToString();
                                Down.Text = down.ToString();
                                BuyQty.Text = buyQty.ToString();
                                SellQty.Text = sellQty.ToString();
                                Balance.Text = balance.ToString();
                                SharesBalance.Text = sharesBalance.ToString();

                                



                                cts = new CancellationTokenSource();
                                int count = await RunningTask(cts);

                                //Calibration update
                                if (sharesBalance > cSharesBalance)
                                {
                                    cSharesBalance = sharesBalance;
                                    cUp = up;
                                    cDown = down;
                                    cBuyQty = buyQty;
                                    cSellQty = sellQty;

                                    calUp.Text = cUp.ToString();
                                    calDown.Text = cDown.ToString();
                                    calBuyQty.Text = cBuyQty.ToString();
                                    calSellQty.Text = cSellQty.ToString();
                                    calBalance.Text = cBalance.ToString();
                                    calSharesBalance.Text = cSharesBalance.ToString();
                                }

                                

                                
                               

                            }
                        }
                    }
                }
                //if (currentIdx >= values.Length) currentIdx = values.Length - 1;
                //previousTime = DateTime.MinValue;
                //UpdateUI();
                //dataGrid.UpdateLayout();
                //dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                //MessageBox.Show("Counter " + count);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            { 
                btnAutoRun.IsEnabled = true;
                btnCalibrate.IsEnabled = true;
            }
        }
    }
}
