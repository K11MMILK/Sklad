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

namespace Sklad
{
    /// <summary>
    /// Логика взаимодействия для DescriptionPage.xaml
    /// </summary>
    public partial class DescriptionPage : Page
    {
        public DescriptionPage(items item)
        {
            InitializeComponent();
            if (item == null) this.NavigationService.GoBack();
            if (item.group_name != null)
            {

                string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("", connection);
                    command.CommandText = $"SELECT * FROM {item.group_name} WHERE item_id = {item.id}";


                    var reader = command.ExecuteReader();
                    var columns = new Dictionary<string, string>();
                    if (reader.Read())
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columns.Add(reader.GetName(i),reader.GetValue(i).ToString());
                        }
                    else
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columns.Add(reader.GetName(i), null);
                        }

                    columns.Remove("group_id");
                    columns.Remove("item_id");



                    DataContext = columns;

                    DGridGroup.ItemsSource = columns;


                    connection.Close();
                }
            }
            DataContext = item;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
