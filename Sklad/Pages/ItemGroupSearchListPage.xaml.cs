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
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Runtime.Remoting.Messaging;
using System.Collections;
using System.Windows.Media.Animation;

namespace Sklad.Pages
{
    /// <summary>
    /// Логика взаимодействия для ItemGroupSearchListPage.xaml
    /// </summary>
    public partial class ItemGroupSearchListPage : Page
    {
        GroupListPage.Group group;
        public ItemGroupSearchListPage(GroupListPage.Group group)
        {
            InitializeComponent();
            this.group = group;           

        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddEditPage(null));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            IList rows = DGridItems.SelectedItems;

            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("", connection);
                foreach (DataRowView row in rows)
                {
                    cmd.CommandText = $"delete from {group.Name} where item_id={row[0]}";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"delete from expense where item_id={row[0]}";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"delete from Adding where item_id={row[0]}";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"delete from items where id={row[0]}";
                    cmd.ExecuteNonQuery();
                    
                }
                MessageBox.Show("Данные удалены");
                


                connection.Close();
            }
            this.NavigationService.Navigate(new ItemGroupSearchListPage(group));

        }

        private void Btnback_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GroupListPage());
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            object obj = ((Button)sender).CommandParameter;
            items item = new items();
            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("", connection);
                cmd.CommandText = $"select * from items where id = {obj}";
                var reader = cmd.ExecuteReader();
               reader.Read();
                    item.id = (int)obj;
                    item.name_ = reader.GetValue(1).ToString();
                    item.cell_ = reader.GetValue(3).ToString();
                    item.desc_ = reader.GetValue(4).ToString();
                    item.group_name = reader.GetValue(5).ToString();
                    item.count_ = (int) reader.GetValue(2);
                reader.Close();
            }
            this.NavigationService.Navigate(new AddEditPage(item));
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string field;
                SqlCommand cmd = new SqlCommand("", connection);
                switch (searchComboBox.Text) {

                    case "Название товара":
                        field = "name_";
                        break;
                    case "Количество":
                        field = "count_";
                        break;
                    case "Ячейка":
                        field = "cell_";
                        break;
                    default:
                        field = searchComboBox.Text;
                        break;

                } 
                cmd.CommandText = $"select id, name_ as 'Название товара', count_ as 'Количество', cell_ as 'Ячейка', {group.Name}.* into #TempTable from items join {group.Name} on items.id = {group.Name}.item_id and {field}='{searchBox.Text}'; " +
                    $"alter table #TempTable drop column group_id, item_id;" +
                    $"select * from #TempTable;" +
                    $"Drop table #TempTable";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd.CommandText, connection);
                DataSet dt = new DataSet();
                try
                {
                    dataAdapter.Fill(dt);
                    DGridItems.DataContext = dt.Tables[0];
                    DGridItems.ItemsSource = dt.Tables[0].DefaultView;
                }
                catch
                {
                    MessageBox.Show("Неправильный тип данных");
                }
            }
        }

        private void DGridGroups_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            DataRowView obj = DGridItems.SelectedItem as DataRowView;
            if (obj == null) return;
            items item = new items();
            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                
                       
                SqlCommand cmd = new SqlCommand("", connection);
                cmd.CommandText = $"select * from items where id = {obj[0]}";
                var reader = cmd.ExecuteReader();
                reader.Read();
                item.id = (int)obj[0];
                item.name_ = reader.GetValue(1).ToString();
                item.cell_ = reader.GetValue(3).ToString();
                item.desc_ = reader.GetValue(4).ToString();
                item.group_name = reader.GetValue(5).ToString();
                item.count_ = (int)reader.GetValue(2);
                reader.Close();
                
                



                connection.Close();
            }
            this.NavigationService.Navigate(new DescriptionPage(item));

        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Sklad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnExpense_Click(object sender, RoutedEventArgs e)
        {
            object obj = ((Button)sender).CommandParameter;
            items item = new items();
            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("", connection);
                cmd.CommandText = $"select * from items where id = {obj}";
                var reader = cmd.ExecuteReader();
                reader.Read();
                item.id = (int)obj;
                item.name_ = reader.GetValue(1).ToString();
                item.cell_ = reader.GetValue(3).ToString();
                item.desc_ = reader.GetValue(4).ToString();
                item.group_name = reader.GetValue(5).ToString();
                item.count_ = (int)reader.GetValue(2);
                reader.Close();
            }
            this.NavigationService.Navigate(new ExpensePage(item));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("", connection);


                //cmd.CommandText = $"select * " +
                //    $"from items join {group.Name} " +
                //    $"on items.id = {group.Name}.item_id";


                cmd.CommandText = $"select id, name_ as 'Название товара', count_ as 'Количество', cell_ as 'Ячейка', {group.Name}.* into #TempTable from items join {group.Name} on items.id = {group.Name}.item_id; " +
                    $"alter table #TempTable drop column group_id, item_id;" +
                    $"select * from #TempTable;" +
                    $"Drop table #TempTable";


                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd.CommandText, connection);
                DataSet dt = new DataSet();
                dataAdapter.Fill(dt);
                DGridItems.DataContext = dt.Tables[0];
                DGridItems.ItemsSource = dt.Tables[0].DefaultView;



               

                object row = this.DGridItems.Items[0]; //Grab the first row
                DataGridColumn coll = this.DGridItems.Columns[this.DGridItems.Columns.Count - 1]; //Grab the last column
                this.DGridItems.ScrollIntoView(row, coll); //Set the view



                foreach (var col in dt.Tables[0].Columns)
                {
                    searchComboBox.Items.Add(col.ToString());

                }
                searchComboBox.Items.Remove(searchComboBox.Items[0]);
                searchComboBox.SelectedIndex = 0;
                connection.Close();
            }
        }

        private void BtnAdding_Click(object sender, RoutedEventArgs e)
        {
            object obj = ((Button)sender).CommandParameter;
            items item = new items();
            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("", connection);
                cmd.CommandText = $"select * from items where id = {obj}";
                var reader = cmd.ExecuteReader();
                reader.Read();
                item.id = (int)obj;
                item.name_ = reader.GetValue(1).ToString();
                item.cell_ = reader.GetValue(3).ToString();
                item.desc_ = reader.GetValue(4).ToString();
                item.group_name = reader.GetValue(5).ToString();
                item.count_ = (int)reader.GetValue(2);
                reader.Close();
            }
            this.NavigationService.Navigate(new AddingPage(item));
        }
    }
}
