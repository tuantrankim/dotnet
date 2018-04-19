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
        bool isCalibrating = false;
        private DateTime previousTime = DateTime.Now;
        DateTime processingDate, fromDate, toDate;

        bool isBuy = false;

        double tradePrice;
        double up, cUp;
        double down, cDown;
        double buyQty, sellQty, cBuyQty, cSellQty;
        double cashAmountPreset, cashAmount, cashAmountMin, cashAmountMax;
        double sharesAmountPreset, sharesAmount, sharesAmountMin, sharesAmountMax;
        double top = 0;
        double bottom = double.PositiveInfinity;
        double investedCash = 0;
        double cashFromSelling = 0;
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
        List<Trade> tradeHistory = new List<Trade>();
        int currentBuyLevel = 0;
        int currentSellLevel = 0;

        double[] levelSettings = { 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597,  };
        double GetLevelValue(int level)
        {
            double result = Math.Round(((Math.Pow(1.6180339, level) - Math.Pow(-0.6180339, level)) / 2.236067977));
            return result;
        }
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

            if (tradePrice == 0) tradePrice = currentValue;//initiate tradePrice

            if (currentValue > top)
            {
                top = currentValue;
                currentBuyLevel = 0;
            }

            if (currentValue < bottom)
            {
                bottom = currentValue;
                currentSellLevel = 0;
            }

            if (currentValue > tradePrice + up)//sell
            {
                
                //lbTrade.Text = "Sell (+)";
                isBuy = false;

                int sellLevel = (int)((currentValue - bottom) / up);

                if (sellLevel > currentSellLevel)
                {
                    tradePrice = currentValue;
                    currentSellLevel = sellLevel;
                    //sellShares = levelSettings[sellLevel];
                    double sellShares = GetLevelValue(sellLevel) * sellQty;

                    if (sharesAmount < sellShares) sellShares = sharesAmount;//Don't allow negative amount
                    sharesAmount -= sellShares;

                    tradeHistory.Add(
                        new Trade()
                        {
                            Date = idx.Date,
                            Id = sellLevel,
                            Level = sellLevel,
                            Qty = sellShares,
                            IsBuy = false,
                            Price = tradePrice,
                            Total = tradePrice * sellShares
                        });
                    cashFromSelling += tradePrice * sellShares;

                    cashAmount = cashAmount + tradePrice * sellShares;
                    
                    if (sharesAmount < sharesAmountMin) sharesAmountMin = sharesAmount;
                    if (cashAmount > cashAmountMax) cashAmountMax = cashAmount;
                }
            }
            else if (currentValue < tradePrice - down)//buy
            {
                //lbTrade.Text = "Buy (-)";
                isBuy = true;
                int buyLevel = (int)((top - currentValue) / down);

                if (buyLevel > currentBuyLevel)
                {
                    tradePrice = currentValue;
                    currentBuyLevel = buyLevel;
                    //buyShares = levelSettings[buyLevel];
                    double buyShares = GetLevelValue(buyLevel) * buyQty;

                    if (cashAmount < tradePrice * buyShares) buyShares = cashAmount/tradePrice;//Don't allow negative amount
                    cashAmount = cashAmount - tradePrice * buyShares;

                    tradeHistory.Add (
                        new Trade() 
                        { 
                            Date = idx.Date
                            , Id = buyLevel
                            , Level = buyLevel
                            , Qty = buyShares 
                            , IsBuy = true
                            , Price = tradePrice
                            , Total = tradePrice * buyShares
                        });
                    investedCash += tradePrice * buyShares;
                    
                    sharesAmount += buyShares;
                    if (sharesAmount > sharesAmountMax) sharesAmountMax = sharesAmount;
                    if (cashAmount < cashAmountMin) cashAmountMin = cashAmount;
                }
            }

        }

        private void UpdateGrid()
        {
            tradeHistoryGrid.ItemsSource = tradeHistory.Select(x => x).ToList();

            dataGrid.SelectedIndex = currentIdx;
            dataGrid.UpdateLayout();
            dataGrid.ScrollIntoView(dataGrid.SelectedItem);
        }
        private void UpdateUI()
        {
            if (currentIdx >= values.Length) return;

            var timeNow = DateTime.Now;

            if ((DateTime.Now - previousTime).Milliseconds <= 50) return;
            previousTime = timeNow;

            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                int cIdx = (int) o;
                if (cIdx >= values.Length) return;
                Index idx = values[cIdx];
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

                Top.Text = top.ToString();
                Bottom.Text = bottom.ToString();
                InvestedCash.Text = investedCash.ToString();
                CashFromSelling.Text = cashFromSelling.ToString();

                //if (!isCalibrating)
                //{
                //    //List<Trade> itemsSource = tradeHistoryGrid.ItemsSource as List<Trade>;
                //    //if ( itemsSource.Count() < tradeHistory.Count())
                //        tradeHistoryGrid.ItemsSource = tradeHistory.Select(x=>x).ToList();

                //    dataGrid.SelectedIndex = cIdx;
                //    dataGrid.UpdateLayout();
                //    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                //}
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
            if (dataGrid.SelectedIndex >= values.Length - 1) return;
            currentIdx = ++dataGrid.SelectedIndex;
            //for(; currentIdx < values.Length; currentIdx++)
            //{
            //    if (values[currentIdx].Date < fromDate) continue;
            //    if (values[currentIdx].Date >= fromDate) break;
            //}
            
            Calculate();
            UpdateUI();
            UpdateGrid();
        }
        private async void btnAutoRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex >= values.Length) return;
                if (dataGrid.SelectedIndex < 0) dataGrid.SelectedIndex = 0;
                currentIdx = dataGrid.SelectedIndex;
                btnAutoRun.IsEnabled = false;
                
                
                //if (dataGrid.SelectedIndex < 0) dataGrid.SelectedIndex = 0;
                //currentIdx = dataGrid.SelectedIndex;

                cts = new CancellationTokenSource();
                int count = await RunningTask(cts);
                if (currentIdx >= values.Length) currentIdx = values.Length-1;
                previousTime = DateTime.MinValue;
                UpdateUI();
                UpdateGrid();
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
                        //if (values[currentIdx].Date >= fromDate && values[currentIdx].Date <= toDate)
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
            tradePrice = 0;
            up = Convert.ToDouble(Up.Text);
            down = Convert.ToDouble(Down.Text);
            buyQty = Convert.ToDouble(BuyQty.Text);
            sellQty = Convert.ToDouble(SellQty.Text);

            cashAmountPreset = cashAmountMin = cashAmountMax = cashAmount = Convert.ToDouble(CashAmountPreset.Text);
            sharesAmountPreset = sharesAmountMin = sharesAmountMax = sharesAmount = Convert.ToDouble(SharesAmountPreset.Text);
            currentValue = Convert.ToDouble(Value.Text);

            fromDate = Convert.ToDateTime(FromDate.Text);
            toDate = Convert.ToDateTime(ToDate.Text);

            Trade.Text = tradePrice.ToString();
            CashAmount.Text = cashAmount.ToString();
            CashAmountMin.Text = cashAmountMin.ToString();
            CashAmountMax.Text = cashAmountMax.ToString();

            SharesAmount.Text = sharesAmount.ToString();
            SharesAmountMin.Text = sharesAmountMin.ToString();
            SharesAmountMax.Text = sharesAmountMax.ToString();

            Balance.Text = balance.ToString();
            SharesBalance.Text = sharesBalance.ToString();

            top = 0;
            bottom = double.PositiveInfinity;

            Top.Text = top.ToString();
            Bottom.Text = bottom.ToString();

            currentBuyLevel = 0;
            currentSellLevel = 0;
            investedCash = 0;
            cashFromSelling = 0;

            InvestedCash.Text = investedCash.ToString();
            CashFromSelling.Text = cashFromSelling.ToString();

            //values = File.ReadAllLines("HistoricalQuotes.csv")
            //                               .Skip(2)
            //                               .Select(v => Index.FromHistoricalQuotesCsv(v))
            //                               .Where(f => f.Date >= fromDate && f.Date <= toDate)
            //                               .OrderBy(i => i.Date)
            //                               .ToArray();

            values = File.ReadAllLines("SPY.csv")
                                           .Skip(1)
                                           .Select(v => Index.FromSPYCsv(v))
                                           .Where(f => f.Date >= fromDate && f.Date <= toDate)
                                           .OrderBy(i => i.Date)
                                           .ToArray();
            ProcessingDate.Text = DateTime.MinValue.ToShortDateString();
            dataGrid.ItemsSource = values;

            tradeHistory = new List<Trade>();
            tradeHistoryGrid.ItemsSource = tradeHistory;
        }
        int[] runningSteps = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10000 };
        private async void btnCalibrate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnAutoRun.IsEnabled = false;
                btnCalibrate.IsEnabled = false;
                isCalibrating = true;

                cSharesBalance = 0;
                cBalance = 0;
                foreach (var i in runningSteps)
                {
                    sellQty = i;
                    foreach (var j in runningSteps)
                    {
                        buyQty = j;
                        foreach (var k in runningSteps)
                        {
                            up = k;
                            foreach (var l in runningSteps)
                            {
                                down = l;
                                await Calibration();
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
                isCalibrating = false;
            }
        }

        private async Task Calibration()
        {
            currentIdx = 0;
            currentValue = 0;
            cashAmountMin = cashAmountMax = cashAmount = cashAmountPreset;
            sharesAmountMin = sharesAmountMax = sharesAmount = sharesAmountPreset;

            tradePrice = 0;
            top = 0;
            bottom = double.PositiveInfinity;

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
                cBalance = balance;
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
