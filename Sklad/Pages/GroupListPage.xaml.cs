using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using Sklad.Pages;

namespace Sklad
{
    /// <summary>
    /// Логика взаимодействия для GroupListPage.xaml
    /// </summary>
    public partial class GroupListPage : Page
    {
        List<Group> groups = new List<Group>();
        public GroupListPage()
        {

            InitializeComponent();
            
            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                DataTable t = connection.GetSchema("Tables");
                foreach (DataRow row in t.Rows)
                {
                    List<string> fields = new List<string>();
                    var tableName = (string)row[2];
                    if (tableName=="items" || tableName=="workers"||tableName=="expense" || tableName == "Adding") continue;
                    SqlCommand cmd = new SqlCommand("", connection);
                    
                    cmd.CommandText = $"select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{tableName}'";
                    var reader = cmd.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        fields.Add(reader.GetString(3));
                    }

                    

                    groups.Add(new Group(tableName, fields));
                    reader.Close();
                }
                
                connection.Close();
            }

            DGridGroups.ItemsSource = groups;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GroupAddEditPage(null));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            
            var groupForDel = DGridGroups.SelectedItems.Cast<Group>().ToList();
            
            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                    SqlCommand cmd = new SqlCommand("", connection);
                foreach (var group in groupForDel)
                {
                    cmd.CommandText = $"drop table {group.Name}";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"update items set group_name=null where group_name='{group.Name}'";
                    cmd.ExecuteNonQuery();
                }
                    

                connection.Close();
            }

            this.NavigationService.Navigate(new GroupListPage());
        }

        private void Btnback_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MainPage());
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var foundedGroups = new List<Group>();
            string str = searchBox.Text;

            foreach (var group in groups)
            {
                if (group.Name.IndexOf(str, StringComparison.OrdinalIgnoreCase)>=0)
                {
                    foundedGroups.Add(group);
                }
            }

            DGridGroups.ItemsSource = foundedGroups;
        }

        public class Group
        {
            public string Name { get; set; }
            public List<String> Fields;
            public string filds { get; set;}
            public Group (string name, List<string> fields)
            {
                Name = name;
                Fields = fields;
                filds = "";
                foreach(var val in Fields)
                {
                    if(val!="item_id"&&val!="group_id")
                    filds = filds + val + '\n';
                }
            }
            
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GroupAddEditPage((sender as Button).DataContext as Group));
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Sklad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DGridGroups_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var gr = DGridGroups.SelectedItem as Group;
            if (gr == null) return;
            this.NavigationService.Navigate(new ItemGroupSearchListPage(gr));
        }
    }

    


}
