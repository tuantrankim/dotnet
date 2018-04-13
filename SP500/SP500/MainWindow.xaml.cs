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
        double tradePrice;
        double up;
        double down;
        double balance;
        Index[] values;
        int currentIdx = 0;
        public MainWindow()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tradePrice = Convert.ToDouble(Trade.Text);
            up = Convert.ToDouble(Up.Text);
            down = Convert.ToDouble(Down.Text);
            balance = Convert.ToDouble(Balance.Text);

            values = File.ReadAllLines("HistoricalQuotes.csv")
                                           .Skip(2)
                                           .Select(v => Index.FromCsv(v))
                                           .OrderBy(i => i.Date)
                                           .ToArray();
            dataGrid.ItemsSource = values;
        }
        
        

        private void Calculate()
        {
            if (currentIdx >= values.Length) return;

            Index idx = values[currentIdx];
            double val = idx.Close;

            if (val > tradePrice + up)//sell
            {
                //lbTrade.Text = "Sell (+)";
                tradePrice = val;
                balance = balance + tradePrice;
            }
            else if (val < tradePrice - down)//buy
            {
                //lbTrade.Text = "Buy (-)";
                tradePrice = val;
                balance = balance - tradePrice;
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
                Balance.Text = balance.ToString();
                dataGrid.SelectedIndex = currentIdx;
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }), currentIdx);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            currentIdx = ++dataGrid.SelectedIndex;
            if (currentIdx >= values.Length) return;
            Calculate();
            UpdateUI();
        }
        private async void btnAutoRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnAutoRun.IsEnabled = false;
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
            
            await Task.Run(() =>
            {
                try
                {
                    for (; currentIdx < values.Length; currentIdx++)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        Calculate();
                        UpdateUI();
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
            return currentIdx;

        }
    }
}
