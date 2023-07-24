using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace Sklad
{
    
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
            InitializeComponent();

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");

            string s = connectionStringsSection.ConnectionStrings["SkladEntities"].ConnectionString;
            List<string> ss = new List<string>(s.Split(';','\"'));
            foreach (var val in ss)
            {
                if (val.Contains("data source"))
                {
                    string valv = val.Remove(0,12);
                    SkladEntities.CnS = valv;
                    break;
                }
            }

            frame.Navigate(new MainPage());
           //frame.Navigate(new CnStrPage());
        }

        private void Sklad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
           
            
        }

        private void frame_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}
