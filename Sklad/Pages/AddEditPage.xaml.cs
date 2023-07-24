using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private items item=new items();
        private items itemStatic = new items();
        private List<string> groups=new List<string>();
        public AddEditPage(items selectedItem)
        {
            InitializeComponent();
            if(selectedItem!=null)
                item=selectedItem;
            DataContext = item;
            itemStatic.group_name = item.group_name;


            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                DataTable t = connection.GetSchema("Tables");
                foreach(DataRow row in t.Rows)
                {
                    var tableName = (string)row[2];
                    groups.Add(tableName);

                }
                groups.Remove("items");
                groups.Remove("workers");
                groups.Remove("expense");
                groups.Remove("Adding");
                connection.Close();
            }
            groupBox.ItemsSource=groups;

        }

        //private void BtnSave_Click(object sender, RoutedEventArgs e)
        //{
        //    StringBuilder stringBuilder = new StringBuilder();
        //    if (item.cell_ == null)
        //        stringBuilder.AppendLine("Введите ячейку хранения");
        //    if(item.name_ == null)
        //        stringBuilder.AppendLine("Введите название");
        //    else
        //    if (item.name_.Length<1)
        //        stringBuilder.AppendLine("Введите название");
        //    foreach(char c in item.count_.ToString())
        //    {
                
        //        if (!char.IsDigit(c))
        //        {
        //            stringBuilder.AppendLine("Неверно указано количество");
        //                break;
        //        }
        //    }
        //    if (stringBuilder.Length > 0)
        //    {
        //        MessageBox.Show(stringBuilder.ToString());
        //        return;
        //    }
        //    if (item.id == 0)

        //        SkladEntities.GetContext().items.Add(item);

        //    try
        //    {
        //        SkladEntities.GetContext().SaveChanges();
        //        MessageBox.Show("Информация сохранена");
        //        NavigationService.Navigate(new Uri("MainPage.xaml", UriKind.Relative));
        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.Message.ToString());
        //    }


            
            
        //}

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (item.cell_ == null)
                stringBuilder.AppendLine("Введите ячейку хранения");

            if (item.name_ == null)
                stringBuilder.AppendLine("Введите название");
            else
            if (item.name_.Length < 1)
                stringBuilder.AppendLine("Введите название");
            foreach (char c in item.count_.ToString())
            {

                if (!char.IsDigit(c))
                {
                    stringBuilder.AppendLine("Неверно указано количество");
                    break;
                }
            }
            if (stringBuilder.Length > 0)
            {
                MessageBox.Show(stringBuilder.ToString());
                return;
            }
            if (item.id == 0)
            {

                SkladEntities.GetContext().items.Add(item);
                SkladEntities.GetContext().SaveChanges();
            }

            try
            {

                string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("", connection);
                    cmd.CommandText = $"update items set name_='{item.name_}', count_=0, desc_='{item.desc_}', group_name='{item.group_name}', cell_='{item.cell_}' where id={item.id}";
                    cmd.ExecuteNonQuery();


                    if (itemStatic.group_name!=null && itemStatic.group_name != item.group_name)
                    {
                        cmd.CommandText = $"delete from {itemStatic.group_name} where item_id = {item.id}";
                        cmd.ExecuteNonQuery();
                    }
                }
                if (item.group_name != null)
                    this.NavigationService.Navigate(new GroupItemEditPage(item.id, item.group_name));
                else this.NavigationService.Navigate(new MainPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void countBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void cellBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void dexcBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void groupBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void groupBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            groupBox.IsDropDownOpen = true;
            var tb = (TextBox)e.OriginalSource;
            tb.Select(tb.SelectionStart + tb.SelectionLength, 0);
            CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(groupBox.ItemsSource);
            cv.Filter = s =>
                ((string)s).IndexOf(groupBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}
