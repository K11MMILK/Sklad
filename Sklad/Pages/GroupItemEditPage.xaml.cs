using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Sklad
{
    /// <summary>
    /// Логика взаимодействия для GroupEditPage.xaml
    /// </summary>
    public partial class GroupItemEditPage : Page
    {
        List<Field> fields = new List<Field>();
        string groupName;
        int itemId;

        public GroupItemEditPage(int item_id, string group_name)
        {
            InitializeComponent();

            groupName = group_name;
            itemId = item_id;
            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("", connection);
                command.CommandText = $"SELECT * FROM {group_name} WHERE item_id = {item_id}";


                var reader = command.ExecuteReader();
                var columns = new Dictionary<string, string>();
                if (reader.Read())
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.GetValue(i) != null)
                            columns.Add(reader.GetName(i), reader.GetValue(i).ToString());
                        else columns.Add(reader.GetName(i), null);
                    }
                else
                {
                    
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columns.Add(reader.GetName(i), null);
                    }
                    reader.Close();
                    SqlCommand cmd = new SqlCommand("", connection);
                    cmd.CommandText = $"INSERT INTO {group_name} (item_id) VALUES ({item_id})";
                    cmd.ExecuteNonQuery();
                }

                columns.Remove("group_id");
                columns.Remove("item_id");
                
                foreach(var val in columns)
                {
                    fields.Add(new Field(val.Key, val.Value));
                }
                

                DataContext = fields;

                DGridGroupEd.ItemsSource = fields;

                

                connection.Close();
            }


        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (stringBuilder.Length > 0)
            {
                MessageBox.Show(stringBuilder.ToString());
                return;
            }
            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("", connection);
                foreach (var item in fields)
                {
                    try
                    {
                        SqlCommand cmd1 = new SqlCommand($"SET FMTONLY ON; select {item.Key} from {groupName}; SET FMTONLY OFF", connection);
                        SqlDataReader reader = cmd1.ExecuteReader();
                        SqlDbType type = (SqlDbType)(int)reader.GetSchemaTable().Rows[0]["ProviderType"];
                        reader.Close();
                        if (type == SqlDbType.Int)
                        {
                            cmd.CommandText = $"UPDATE {groupName} SET {item.Key} = {item.Value} WHERE item_id = {itemId}";
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd.CommandText = $"UPDATE {groupName} SET {item.Key} = '{item.Value}' WHERE item_id = {itemId}";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch { MessageBox.Show("Ошибка ввода данных"); return; }
                }
                connection.Close();
            }
            this.NavigationService.Navigate(new MainPage());
        }

        private void DGridGroupEd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }



        private void DGridGroupEd_Selected_1(object sender, RoutedEventArgs e)
        {
        }
        private class Field
        {
            
            public string Key { get; set; }
            public string Value { get; set; }
            public Field (string name, string value)
            {
                Key = name;
                Value = value;
            }
        }
       private class tblDetails
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
    }
}
