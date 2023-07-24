using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using static Sklad.Pages.ExpensePage;

namespace Sklad.Pages
{
    /// <summary>
    /// Логика взаимодействия для ExpenseHistorPage.xaml
    /// </summary>
    public partial class ExpenseHistorPage : Page
    {
        public ExpenseHistorPage()
        {
            InitializeComponent();
        }

        private void Btnback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            List<HistoryRow> rows = new List<HistoryRow>();

            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("", connection);
                cmd.CommandText = $"Select * from expense join workers on order by expense.date_ desc";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    rows.Add(new HistoryRow(
                        Convert.ToString(reader["name_"]),
                        Convert.ToString(reader["surname_"]),
                        Convert.ToString(reader["thirdname_"]),
                        Convert.ToInt32(reader["count_"]),
                        (DateTime)reader["date_"]
                        ));
                }
                DGridHistory.ItemsSource = rows;
                connection.Close();
            }
        }

        private void DGridHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
