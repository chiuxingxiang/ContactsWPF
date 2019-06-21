using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System.Data;

namespace Contacts
{
    /// <summary>
    /// ContactsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ContactsWindow : Window
    {
        private SQLiteHelper ContactsDB;
        private int offset = 0;
       
        public ContactsWindow(string username)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            CurrentUserName.Content = username;
            SelectGenderComboBoxValue();
            SelectDepartmentComboBoxValue();
            ShowContacts();
        }
        

        private void SelectGenderComboBoxValue()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("ID", typeof(Int32)));
                dt.Columns.Add(new DataColumn("Gender", typeof(string)));

                DataRow dr1 = dt.NewRow();
                dr1["ID"] = 1;
                dr1["Gender"] = "";
                dt.Rows.Add(dr1);

                DataRow dr2 = dt.NewRow();
                dr2["ID"] = 2;
                dr2["Gender"] = "女";
                dt.Rows.Add(dr2);

                DataRow dr3 = dt.NewRow();
                dr3["ID"] = 3;
                dr3["Gender"] = "男";
                dt.Rows.Add(dr3);
                SelectGender.ItemsSource = dt.DefaultView;
                SelectGender.DisplayMemberPath = "Gender";
                SelectGender.SelectedValuePath = "Gender";
                SelectGender.SelectedIndex = 0;

                GenderBox.ItemsSource = dt.DefaultView;
                GenderBox.DisplayMemberPath = "Gender";
                GenderBox.SelectedValuePath = "Gender";
                GenderBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                ContactsDB.CloseConnection();
            }
        }

        private void SelectDepartmentComboBoxValue()
        {
            try
            {
                ContactsDB = new SQLiteHelper("Contacts.db");
                SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT DISTINCT Department FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' GROUP by Department");
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                DataRow dr1 = dataTable.NewRow();
                dr1["Department"] = "";
                dataTable.Rows.InsertAt(dr1, 0);
                SelectDepartment.ItemsSource = dataTable.DefaultView;
                SelectDepartment.DisplayMemberPath = "Department";
                SelectDepartment.SelectedValuePath = "Department";
                SelectDepartment.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                ContactsDB.CloseConnection();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContactsDB = new SQLiteHelper("Contacts.db");
                if(NameBox.Text != "" && PhoneBox.Text != "" && GenderBox.Text != "" && DepartmentBox.Text != "" && BirthdayPicker.Text != "")
                {
                    ContactsDB.InsertValues("Contacts", new string[] { NameBox.Text, PhoneBox.Text, GenderBox.Text, DepartmentBox.Text, BirthdayPicker.Text, CurrentUserName.Content.ToString() });
                    //ShowContacts();
                    NameBox.Text = "";
                    PhoneBox.Text = "";
                    GenderBox.Text = "";
                    DepartmentBox.Text = "";
                    BirthdayPicker.Text = "";
                    SelectGenderComboBoxValue();
                    SelectDepartmentComboBoxValue();
                }
                else
                {
                    //MessageBox.Show("参数不能为空");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                ContactsDB.CloseConnection();
            }
        }

        private void ShowContacts()
        {
            try
            {
                ContactsDB = new SQLiteHelper("Contacts.db");
                SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts  WHERE UserName='" + CurrentUserName.Content + "' limit 10 offset " + offset);
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                DisplayContacts.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                ContactsDB.CloseConnection();
            }
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            ContactsDB.CloseConnection();
            LoginWindow main = new LoginWindow();
            Application.Current.MainWindow = main;
            Close();
            main.Show();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DisplayContacts_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void SearchNameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContactsDB = new SQLiteHelper("Contacts.db");
                if(SearchNameBox.Text.ToString() == "")
                {
                    SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Gender='" + SelectGender.SelectedValue.ToString() + "' AND Department='" + SelectDepartment.SelectedValue.ToString() + "'");
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    DisplayContacts.ItemsSource = dataTable.DefaultView;
                }else if(SelectGender.SelectedIndex == 0 && SelectDepartment.SelectedIndex == 0)
                {
                    SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Name LIKE '%" + SearchNameBox.Text.ToString() + "%'");
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    DisplayContacts.ItemsSource = dataTable.DefaultView;
                }
                else if(SelectGender.SelectedIndex != 0 && SelectDepartment.SelectedIndex == 0)
                {
                    SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Name LIKE '%" + SearchNameBox.Text.ToString() + "%' AND Gender='" + SelectGender.SelectedValue.ToString() + "'");
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    DisplayContacts.ItemsSource = dataTable.DefaultView;
                }
                else if (SelectGender.SelectedIndex == 0 && SelectDepartment.SelectedIndex != 0)
                {
                    SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Name LIKE '%" + SearchNameBox.Text.ToString() + "%' AND Department='" + SelectDepartment.SelectedValue.ToString() + "'");
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    DisplayContacts.ItemsSource = dataTable.DefaultView;
                }
                else if (SelectGender.SelectedIndex != 0 && SelectDepartment.SelectedIndex != 0)
                {
                    SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Name LIKE '%" + SearchNameBox.Text.ToString() + "%' AND Gender='" + SelectGender.SelectedValue.ToString() + "' AND Department='" + SelectDepartment.SelectedValue.ToString() + "'");
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    DisplayContacts.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                ContactsDB.CloseConnection();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContactsDB = new SQLiteHelper("Contacts.db");
                offset += 10;
                SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' limit 10 offset " + offset);
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                DisplayContacts.ItemsSource = dataTable.DefaultView;
                if (offset != 0) PreviousPageButton.IsEnabled = true;
                if (dataTable.Rows.Count < 10) NextPageButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                ContactsDB.CloseConnection();
            }
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContactsDB = new SQLiteHelper("Contacts.db");
                offset -= 10;
                SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' limit 10 offset " + offset);
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                DisplayContacts.ItemsSource = dataTable.DefaultView;
                if (offset == 0) PreviousPageButton.IsEnabled = false;
                NextPageButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                ContactsDB.CloseConnection();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int rowNumber = DisplayContacts.SelectedIndex;
                string selectedName = (DisplayContacts.Columns[0].GetCellContent(DisplayContacts.Items[rowNumber]) as TextBlock).Text;

                ContactsDB = new SQLiteHelper("Contacts.db");
                ContactsDB.ExecuteQuery("DELETE FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Name='" + selectedName + "'");
                SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE Department LIKE '" + SelectDepartment.Text + "%' AND Gender LIKE '%" + SelectGender.Text + "%' AND UserName='" + CurrentUserName.Content + "' limit 10 offset " + offset);
                
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                
                DisplayContacts.ItemsSource = dataTable.DefaultView;
                PreviousPageButton.IsEnabled = false;
                NextPageButton.IsEnabled = true;
                offset = 0;
                //ContactsDB.CloseConnection();
                //SelectGenderComboBoxValue();
                //SelectDepartmentComboBoxValue();
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

        private void SelectGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ContactsDB = new SQLiteHelper("Contacts.db");
                if (SelectGender.SelectedIndex == 0 && SelectDepartment.SelectedIndex == 0)
                {
                    if (SearchNameBox.Text == "")
                    {
                        ContactsDB = new SQLiteHelper("Contacts.db");
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' limit 10");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    else
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Name LIKE '%" + SearchNameBox.Text.ToString() + "%'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    
                }
                else if(SelectGender.SelectedIndex == 0 && SelectDepartment.SelectedIndex != 0)
                {
                    if (SearchNameBox.Text == "")
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Department='" + SelectDepartment.SelectedValue.ToString() + "'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    else
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Department='" + SelectDepartment.SelectedValue.ToString() + "' AND Name LIKE '%" + SearchNameBox.Text.ToString() + "%'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    
                }
                else if (SelectGender.SelectedIndex != 0 && SelectDepartment.SelectedIndex == 0)
                {
                    if (SearchNameBox.Text == "")
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Gender='" + SelectGender.SelectedValue.ToString() + "'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    else
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Gender='" + SelectGender.SelectedValue.ToString() + "' AND Name LIKE '%" + SearchNameBox.Text.ToString() + "%'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                }
                else
                {
                    if (SearchNameBox.Text == "")
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Gender='" + SelectGender.SelectedValue.ToString() + "' AND Department='" + SelectDepartment.SelectedValue.ToString() + "'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    else
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Gender='" + SelectGender.SelectedValue.ToString() + "' AND Department='" + SelectDepartment.SelectedValue.ToString() + "' AND Name LIKE '%" + SearchNameBox.Text.ToString() + "%'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                ContactsDB.CloseConnection();
            }
        }

        private void SelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ContactsDB = new SQLiteHelper("Contacts.db");
                if (SelectGender.SelectedIndex == 0 && SelectDepartment.SelectedIndex == 0)
                {
                    if(SearchNameBox.Text == "")
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' limit 10");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    else
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Name LIKE '%" + SearchNameBox.Text.ToString() + "%'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    
                }
                else if(SelectGender.SelectedIndex != 0 && SelectDepartment.SelectedIndex == 0)
                {
                    if (SearchNameBox.Text == "")
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Gender='" + SelectGender.SelectedValue.ToString() + "'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    else
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Gender='" + SelectGender.SelectedValue.ToString() + "' AND Name LIKE '%" + SearchNameBox.Text.ToString() + "%'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    
                }else if (SelectGender.SelectedIndex == 0)
                {
                    if (SearchNameBox.Text == "")
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Department='" + SelectDepartment.SelectedValue.ToString() + "'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    else
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Department='" + SelectDepartment.SelectedValue.ToString() + "' AND Name LIKE '%" + SearchNameBox.Text.ToString() + "%'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    
                }
                else
                {
                    if (SearchNameBox.Text == "")
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Department='" + SelectDepartment.SelectedValue.ToString() + "' AND Gender='" + SelectGender.SelectedValue.ToString() + "'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    else
                    {
                        SQLiteDataReader reader = ContactsDB.ExecuteQuery("SELECT Name, Phone, Gender, Department, Birthday FROM Contacts WHERE UserName='" + CurrentUserName.Content + "' AND Department='" + SelectDepartment.SelectedValue.ToString() + "' AND Gender='" + SelectGender.SelectedValue.ToString() + "' AND Name LIKE '%" + SearchNameBox.Text.ToString() + "%'");
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayContacts.ItemsSource = dataTable.DefaultView;
                    }
                    
                }
                
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                ContactsDB.CloseConnection();
            }
        }
    }
}
