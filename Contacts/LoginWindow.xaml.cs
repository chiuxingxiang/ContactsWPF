using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;

namespace Contacts
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        private SQLiteHelper ContactsDB;
        public LoginWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            CreateUserTable();
        }

        private void CreateUserTable()
        {
            try
            {
                ContactsDB = new SQLiteHelper("Contacts.db");
                ContactsDB.CreateTable("Users", new string[] { "UserName", "Password" }, new string[] { "TEXT", "TEXT" });
                ContactsDB.CreateTable("Contacts", new string[] { "Name", "Phone", "Gender", "Department", "Birthday", "UserName" }, new string[] { "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT" });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ContactsDB.CloseConnection();
            }
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContactsDB = new SQLiteHelper("Contacts.db");

                SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT * FROM Users WHERE UserName='" + UserNameBox.Text.ToString() +"'");
                if (reader.Read())
                {
                    string UserName = reader[0].ToString();
                    string PassWord = reader[1].ToString();
                   
                    if (UserName == UserNameBox.Text && PassWord == PasswordBox.Password)
                    {
                        ContactsDB.CloseConnection();
                        ContactsWindow main = new ContactsWindow(UserNameBox.Text);
                        Application.Current.MainWindow = main;
                        Close();
                        main.Show();
                    }
                    else
                    {
                        MessageBox.Show("密码错误");
                    }
                }
                else
                {
                    MessageBox.Show("用户名不存在");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                ContactsDB.CloseConnection();
            }
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            ContactsDB = new SQLiteHelper("Contacts.db");
            SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT * FROM Users WHERE UserName='" + UserNameBox.Text.ToString() + "'");
            if (UserNameBox.Text == "")
            {
                MessageBox.Show("用户名不能为空");
            }
            else if (reader.Read())
            {
                MessageBox.Show("用户名已存在");
            }
            else
            {
                ContactsDB.InsertValues("Users", new string[] { UserNameBox.Text, PasswordBox.Password });
                ContactsDB.CloseConnection();
                MessageBox.Show("注册成功！请登录");
            }
            
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
