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

namespace Sklad.Pages
{
    /// <summary>
    /// Логика взаимодействия для ExpensePage.xaml
    /// </summary>
    public partial class ExpensePage : Page
    {
        items selectedItem;
        public ExpensePage(items selectedItem)
        {
            InitializeComponent();
            itemNameBlock.Text = selectedItem.name_;
            itemCountBlock.Text = selectedItem.count_.ToString();
            this.selectedItem = selectedItem;

            List<HistoryRow> rows = new List<HistoryRow>();

            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("", connection);
                cmd.CommandText = $"Select * from expense join workers on expense.item_id = {selectedItem.id} and workers.worker_id=expense.worker_id order by expense.date_ desc";
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
                DGridHistory.ItemsSource=rows;
                connection.Close();
            }

        }

        private void DGridHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void Btnback_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                StringBuilder s = new StringBuilder();
                if (countBox.Text == "" || countBox.Text==null)
                {
                    s.AppendLine("Введите количество");
                }
                else
                {
                    bool fl = false;
                    foreach (char c in countBox.Text.ToString())
                    {
                        if (!char.IsDigit(c))
                        {
                            s.AppendLine("Неверное количество");
                            fl = true;
                            break;
                        }
                    }

                    if (!fl && Convert.ToInt32(countBox.Text) > selectedItem.count_)
                    {
                        s.AppendLine("Неверное количество");
                    }
                }
                if (idBox.Text == "")
                {
                    s.AppendLine("Введите табельный номер");
                }
                else
                {

                    SqlCommand cmd = new SqlCommand("", connection);
                    cmd.CommandText = $"select * from workers where worker_id='{idBox.Text}'";
                    var reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        s.AppendLine("Неверный табельный номер");
                    }
                    reader.Close();

                }

                

                if (s.Length > 0)
                {
                    MessageBox.Show(s.ToString());
                    return;
                }
                else
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("", connection);

                        cmd.CommandText = $"insert into expense (item_id, worker_id, count_, date_) values ({selectedItem.id}, {idBox.Text}, {Convert.ToInt32(countBox.Text)}, getdate())";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = $"update items set count_={selectedItem.count_ - Convert.ToInt32(countBox.Text)} where id={selectedItem.id}";
                        cmd.ExecuteNonQuery();


                        MessageBox.Show("Данные сохранены");
                        connection.Close();
                        this.NavigationService.GoBack();
                    }
                    catch
                    {
                        MessageBox.Show("Сохранение не удалось");
                        connection.Close();
                        return;
                    }
                }

            }

        }
        public class HistoryRow
        {
            public string name_ { get; set;}
            public string surname_ { get; set;}
            public string thirdname_ { get; set;}
            public int count_ { get; set;}
            public DateTime date_ { get; set;}
            public HistoryRow(string name_, string surname_, string thirdname_, int count_, DateTime date_)
            {
                this.name_ = name_;
                this.surname_ = surname_;
                this.thirdname_ = thirdname_;
                this.count_ = count_;
                this.date_ = date_.Date;
            }
        }
    }
}
