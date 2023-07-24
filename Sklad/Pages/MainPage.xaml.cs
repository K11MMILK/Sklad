using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
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
using System.Xml.Linq;
using Sklad.Pages;

namespace Sklad
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private static SkladEntities _context = SkladEntities.GetContext();
        public MainPage()
        {
            InitializeComponent();

            
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
           this.NavigationService.Navigate(new AddEditPage(null));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var itemsForRemoving = DGridItems.SelectedItems.Cast<items>().ToList();

            if(MessageBox.Show($"Вы хотите удалить следущие {itemsForRemoving.Count()} элементов?", "Внимание", MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)
            {
                try
                {
                    IList rows = DGridItems.SelectedItems;

                    string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        SqlCommand cmd = new SqlCommand("", connection);
                        foreach (items row in rows)
                        {
                            if (row.group_name != "")
                            {
                                cmd.CommandText = $"delete from {row.group_name} where item_id={row.id}";
                                cmd.ExecuteNonQuery();
                            }
                            cmd.CommandText = $"delete from expense where item_id={row.id}";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = $"delete from Adding where item_id={row.id}";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = $"delete from items where id={row.id}";
                            cmd.ExecuteNonQuery();
                            
                        }
                        MessageBox.Show("Данные удалены");



                        connection.Close();
                        this.NavigationService.Navigate(new MainPage());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void Sklad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddEditPage((sender as Button).DataContext as items));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                
                SkladEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridItems.ItemsSource = SkladEntities.GetContext().items.ToList();

                
            }
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //var foundedItems = new List<items>();
            //string str = searchBox.Text;
            //foreach (var item in SkladEntities.GetContext().items.ToList())
            //{
            //    if (item.cell_ != null && item.cell_.Contains(str) || item.desc_ != null && item.desc_.Contains(str) || item.name_ != null && item.name_.Contains(str))
            //    {
            //        foundedItems.Add(item);
            //    }
            //}
            //DGridItems.ItemsSource = foundedItems;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var foundedItems = new List<items>();
            string str = searchBox.Text;
            foreach (var item in SkladEntities.GetContext().items.ToList())
            {
                if (item.group_name!=null && item.group_name.IndexOf(str, StringComparison.OrdinalIgnoreCase)>=0 || item.cell_!=null && item.cell_.IndexOf(str, StringComparison.OrdinalIgnoreCase)>=0 || item.desc_ != null && item.desc_.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0 || item.name_ != null && item.name_.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    foundedItems.Add(item);
                }
            }
            DGridItems.ItemsSource = foundedItems;
        }

        private void BtnGroup_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GroupListPage());
        }

        private void DGridItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var itemForDesc =(items) DGridItems.SelectedItem;
            if (itemForDesc != null)
            this.NavigationService.Navigate(new DescriptionPage(itemForDesc));    
        }

        private void addWorkerBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddEmployeePage());
        }

        private void BtnExpense_Click(object sender, RoutedEventArgs e)
        {
            items selectedItem = (sender as Button).DataContext as items;
            this.NavigationService.Navigate(new ExpensePage(selectedItem));
        }

        private void BtnAdding_Click(object sender, RoutedEventArgs e)
        {
            items selectedItem = (sender as Button).DataContext as items;
            this.NavigationService.Navigate(new AddingPage(selectedItem));
        }
    }
}
