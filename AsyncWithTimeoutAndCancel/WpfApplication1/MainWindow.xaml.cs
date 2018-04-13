using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SynchronizationContext synchronizationContext;
        private DateTime previousTime = DateTime.Now;
        CancellationTokenSource cts;
        public MainWindow()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                btnRun.IsEnabled = false;
                cts = new CancellationTokenSource();
                int count = await RunningTask(cts);
                MessageBox.Show("Counter " + count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            { btnRun.IsEnabled = true; }
        }
        
        // Timeout from caller
        private async void btnTimeout_Click(object sender, EventArgs e)
        {
            try
            {
                int timeout = Convert.ToInt32(txtTimeout.Text);
                
                btnTimeout.IsEnabled = false;
                cts = new CancellationTokenSource();
                Task<int> task = RunningTask(cts);

                if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                {
                    // task completed within timeout
                    //label1.Text = @"Finished Counter " + task.Result;
                    MessageBox.Show("Done");
                }
                else
                {
                    // timeout logic
                    cts.Cancel();
                    MessageBox.Show("Timeout");
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            { btnTimeout.IsEnabled = true; }
        }

        // Timeout from child task
        private async void btnTimeoutCancel_Click(object sender, RoutedEventArgs e)
        {
            int timeout = Convert.ToInt32(txtTimeout.Text);
            btnTimeoutCancel.IsEnabled = false;
            cts = new CancellationTokenSource(timeout);

            int count = await RunningTask(cts);
            if (cts.IsCancellationRequested)
            {
                MessageBox.Show("Cancel when Timeout. Count " + count);
            }
            else
            {
                MessageBox.Show("Counter " + count);
            }
            btnTimeoutCancel.IsEnabled = true;
        }

        private async Task<int> RunningTask()
        {
            var count = 0;

            await Task.Run(() =>
            {
                for (var i = 0; i <= 5000000; i++)
                {
                    UpdateUI(i);
                    count = i;
                }
            });
            return count;

        }

        private async Task<int> RunningTask(CancellationTokenSource cts)
        {
            var count = 0;

            await Task.Run(() =>
            {
                try
                {
                    for (var i = 0; i <= 5000000; i++)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        //if (cts.IsCancellationRequested) break;
                        UpdateUI(i);
                        count = i;
                    }
                }
                catch (Exception ex)
                {
                    synchronizationContext.Post(new SendOrPostCallback(o =>
                    {
                        label1.Text = (string)o;
                    }), ex.Message);
                }
                finally
                {
                    //Don't return insid finally bc of IDisposable
                }

            });
            return count;
        }
            
        
        
        public void UpdateUI(int value)
        {
            var timeNow = DateTime.Now;

            if ((DateTime.Now - previousTime).Milliseconds <= 50) return;

            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                label1.Text = @"Counter " + (int)o;
            }), value);

            previousTime = timeNow;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }

        
    }
}
