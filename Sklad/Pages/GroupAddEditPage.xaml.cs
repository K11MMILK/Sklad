using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.Remoting.Messaging;
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
    /// Логика взаимодействия для GroupPage.xaml
    /// </summary>
    public partial class GroupAddEditPage : Page
    {
        bool fl = false;
        string groupName;
        List<Field> fields = new List<Field>();
        GroupListPage.Group group;
        public GroupAddEditPage(GroupListPage.Group group)
        {
            InitializeComponent();
            if (group != null)
            {
                fl = true;
                this.group = group;
                nameBox.Text = group.Name;
                nameBox.IsReadOnly = true;

                //заполнение dataGrid существующими полями


                    string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("", connection);
                        foreach (var field in group.Fields)
                        {
                        if (field == "item_id" || field == "group_id") continue;
                            SqlCommand cmd1 = new SqlCommand($"SET FMTONLY ON; select {field} from {group.Name}; SET FMTONLY OFF", connection);
                            SqlDataReader reader = cmd1.ExecuteReader();
                            SqlDbType type = (SqlDbType)(int)reader.GetSchemaTable().Rows[0]["ProviderType"];
                            reader.Close();
                            if (type == SqlDbType.Int)
                                {
                                    fields.Add(new Field(field, "Число"));
                                }
                            else fields.Add(new Field(field, "Текст"));
                        }
                        connection.Close();
                    }


                 //
                DataContext = fields;
                DGridGroups.ItemsSource = fields;

            }
            fieldTypeBox.SelectedIndex = 0;
            


        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GroupListPage());
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            groupName=nameBox.Text;
            groupName = groupName.Replace(" ", "_");
            string connectionString = $"data source={SkladEntities.ConnectionString()};initial catalog=Sklad;integrated security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                

                //создание таблицы если ее нет
                SqlCommand command = new SqlCommand();
                command.CommandText = $"CREATE TABLE {groupName} (group_id INT PRIMARY KEY IDENTITY, item_id INT, FOREIGN KEY (item_id) references items(id));";
                command.Connection = connection;
                if (!fl)
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        MessageBox.Show("Недопустимое название группы");
                        return;
                    }
                    }

                //добавление отсутствующих полей
                StringBuilder nCor = new StringBuilder();
                foreach (var field in fields)
                {
                    try
                    {
                        string fieldName = field.name.Replace(' ', '_');
                        string fieldType;
                        if (field.type == "Число") fieldType = "INT"; else fieldType = "VARCHAR(50)";
                        command.CommandText = $"ALTER TABLE {groupName} ADD {fieldName} {fieldType};";
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }
                    catch {
                        try { if (!group.Fields.Contains(field.name)) nCor.AppendLine(field.name); continue; }
                        catch
                        {
                            MessageBox.Show("Недопустимые символы в полях");
                            continue;
                        }
                        }
                }


                if (nCor.Length > 0) 
                {
                    MessageBox.Show("Данные поля не включены в таблицу из-за содержания недопустимых символов:\n" + nCor.ToString()); return;
                }


                //Удаление ненужных полей из бд
                
                    SqlCommand cmd = new SqlCommand("", connection);
                    cmd.CommandText = $"select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{nameBox.Text}'";
                    var reader = cmd.ExecuteReader();
                    List<string> fieldsForDel = new List<string>();
                    while (reader.Read())
                    {
                    bool a = false;
                    foreach (var field in fields)
                    {
                        if (field.name == reader.GetString(3) || reader.GetString(3) == "item_id"|| reader.GetString(3)=="group_id") { a = true; break; }
                    }
                        if( !a )
                    {
                        fieldsForDel.Add(reader.GetString(3));
                        
                    }
                    }
                    reader.Close();
                if (fieldsForDel.Count > 0)
                {
                    foreach (var v in fieldsForDel) {
                        SqlCommand cmd1 = new SqlCommand("", connection);
                        cmd1.CommandText = $"alter table {nameBox.Text} drop column {v}";
                        cmd1.ExecuteNonQuery(); 
                    }
                }




                MessageBox.Show("Группа сохранена");
                connection.Close();
                NavigationService.Navigate(new GroupListPage());
            }
        }
        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BtnAddField_Click(object sender, RoutedEventArgs e)
        {
            bool fl = false;
            if (fields.Count > 0)
            {
                foreach (Field field in fields)
                {
                    if (fieldNameBox.Text == field.name)
                    {
                        MessageBox.Show("Поле с таким названием уже существует");
                        fl = true;
                        break;
                    }
                }
                if (fl == false)
                {
                    fields.Add(new Field(fieldNameBox.Text.Replace(' ', '_'), fieldTypeBox.Text));
                    DGridGroups.ItemsSource = fields;
                    DGridGroups.Items.Refresh();
                    fieldNameBox.Clear();
                }
            }

            else
            {               
                    fields.Add(new Field(fieldNameBox.Text.Replace(' ', '_'), fieldTypeBox.Text));
                    DGridGroups.ItemsSource = fields;
                    DGridGroups.Items.Refresh();
                    fieldNameBox.Clear();
                

            }
        }

        private void BtnDelField_Click(object sender, RoutedEventArgs e)
        {
            var fieldsToRemoving = DGridGroups.SelectedItems.Cast<Field>().ToList();
            foreach (Field field in fieldsToRemoving)
            fields.Remove(field);
            DGridGroups.ItemsSource = fields;
            DGridGroups.Items.Refresh();
        }

        private void DGridGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
    public class Field
    {
        public string name { get; set; }
        public string type { get; set; }
        
        public Field (string name, string type)
        {
            this.name = name;
            this.type = type;
            name =name.Replace(' ', '_');
        }
    }
}
