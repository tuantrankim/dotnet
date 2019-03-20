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

namespace HelloClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = "HELLO";//Factory<IFDMTSystem>.Proxy.PopulateFDMTSecurityPreviledges(ref currentUser);
                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                //Factory<IFDMTSystem>.DisposeProxy();
             
            }
        }
    }
}
