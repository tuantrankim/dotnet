using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace UnhandledExceptionHandler
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        internal bool ThrowThreadException { get; set; }
        internal Thread CurrentThread { get; set; }
        public Window1()
        {
            InitializeComponent();
        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.chkIsThread.IsChecked.HasValue && this.chkIsThread.IsChecked.Value)
                {
                    this.ThrowThreadException = true;
                    return;
                }
                throw new ApplicationException("User Defined Exception thrown");
            }
            catch(Exception ex)
            {
                if (rbtnCatch.IsChecked.HasValue && rbtnCatch.IsChecked.Value)
                    MessageBox.Show(ex.Message, "Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    App app = App.Current as App;
                    app.DoHandle = rbtnApplication.IsChecked.HasValue && rbtnApplication.IsChecked.Value;
                    throw ex;
                }
            }
        }

        private void chkIsThread_Checked(object sender, RoutedEventArgs e)
        {
            this.CurrentThread = this.CurrentThread ?? new Thread(new ThreadStart(this.RunMe));
            if(this.CurrentThread.ThreadState == ThreadState.Unstarted)
                this.CurrentThread.Start();
        }

        private void RunMe()
        {
            do
            {
                lock (this)
                {
                    if (this.ThrowThreadException)
                    {
                        this.ThrowThreadException = false;
                        throw new ApplicationException("User Defined Exception thrown from Thread");
                    }
                }
            } while (true);
            
        }
    }
}
