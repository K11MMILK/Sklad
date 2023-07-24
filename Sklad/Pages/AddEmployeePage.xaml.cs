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
    /// Логика взаимодействия для AddEmployeePage.xaml
    /// </summary>
    public partial class AddEmployeePage : Page
    {
        public AddEmployeePage()
        {
            InitializeComponent();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder s = new StringBuilder();

            if (nameBox.Text == "")
            {
                s.AppendLine( "Строка с именем пуста");
            }
            if(surnameBox.Text == "")
            {
                s.AppendLine("Строка с фамилией пуста");
            }
            if (idBox.Text == "")
            {
                s.AppendLine("Строка с табельным номером пуста");
            }
            if (s.Length > 0)
            {
                MessageBox.Show(s.ToString());
                return;
            }
            else
            {
                string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("", connection);
                    cmd.CommandText = $"select * from workers where worker_id = {idBox.Text}";
                    var reader  = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        cmd.CommandText = $"insert into workers (name_, surname_, thirdname_, worker_id) values ( '{nameBox.Text}', '{surnameBox.Text}', '{thirdnameBox.Text}', '{idBox.Text}')";
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Сотрудник добавлен");
                        NavigationService.GoBack();
                    }
                    else
                    {
                        reader.Close();
                        MessageBox.Show("Сотрудник с таким табельным номером уже существует");
                        return;
                    }

                    connection.Close();
                }
            }
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder s = new StringBuilder();

          
            if (idBox.Text == "")
            {
                s.AppendLine("Строка с табельным номером пуста");
            }
            if (s.Length > 0)
            {
                MessageBox.Show(s.ToString());
                return;
            }
            else
            {
                string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("", connection);
                    cmd.CommandText = $"select * from workers where worker_id = {idBox.Text}";
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Close();
                        cmd.CommandText = $"delete from expense where worker_id = {idBox.Text}";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = $"delete from Adding where worker_id = {idBox.Text}";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = $"delete from workers where worker_id={idBox.Text}";
                        cmd.ExecuteNonQuery();
                        
                        MessageBox.Show("Сотрудник удален");
                        NavigationService.GoBack();
                    }
                    else
                    {
                        reader.Close();
                        MessageBox.Show("Сотрудник с таким табельным номером не найден");
                        return;
                    }

                    connection.Close();
                }
            }
        }
    }
}
