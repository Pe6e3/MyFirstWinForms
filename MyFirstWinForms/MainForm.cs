using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WinForms.DAL.Entities;

namespace MyFirstWinForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            LoadUserData(); // Вызов метода для загрузки данных и создания кнопок
            ClearUserInfoLabels(); // Очищаем информацию на метках
        }

        private void ClearUserInfoLabels()
        {
            ageLabel.Text = "";
            userInfoLabel.Text = "";
            userInfo.Text = "";
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void CloseButton_MouseEnter(object sender, EventArgs e)
        {
            CloseButton.ForeColor = Color.Red;
        }

        private void CloseButton_MouseLeave(object sender, EventArgs e)
        {
            CloseButton.ForeColor = Color.White;

        }

        private void LoadUserData()
        {
            Db db = new Db();

            using (MySqlConnection connection = db.GetConnection())
            {
                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter();

                MySqlCommand command = new MySqlCommand("SELECT id, login FROM `users`", connection);

                adapter.SelectCommand = command;
                adapter.Fill(table);

                ClearUserInfoLabels(); // Очищаем информацию на метках

                List<User> users = new List<User>();

                foreach (DataRow row in table.Rows)
                {
                    int id = Convert.ToInt32(row["id"]);
                    string login = row["login"].ToString();
                    User user = new User { Id = id, Login = login };
                    users.Add(user);
                }

                foreach (User user in users)
                {
                    string login = user.Login;
                    Button userButton = new Button();
                    userButton.Text = login;
                    userButton.Click += (sender, e) => HandleUserButtonClick(sender, e, user, connection, db);
                    flowLayoutPanel.Controls.Add(userButton);
                }
            }
        }


        private void HandleUserButtonClick(object sender, EventArgs e, User user, MySqlConnection connection, Db db)
        {
            using (MySqlConnection ageConnection = db.GetConnection())
            {
                MySqlCommand ageInfoCommand = new MySqlCommand("SELECT age, info FROM `userprofiles` WHERE `userid` = @uId", ageConnection);
                ageInfoCommand.Parameters.Add("@uId", MySqlDbType.Int32).Value = user.Id;

                using (MySqlDataReader ageInfoReader = ageInfoCommand.ExecuteReader())
                {
                    if (ageInfoReader.Read())
                    {
                        int age = ageInfoReader.IsDBNull(ageInfoReader.GetOrdinal("age")) ? 0 : ageInfoReader.GetInt32("age");
                        string info = ageInfoReader.IsDBNull(ageInfoReader.GetOrdinal("info")) ? "" : ageInfoReader.GetString("info");

                        ageLabel.Text = $"Возраст: {age}";
                        userInfo.Text = $"Инфо: {info}";
                    }
                    else
                    {
                        MessageBox.Show("Возраст и инфо не указаны");
                        ageLabel.Text = $"Возраст не указан";
                        userInfo.Text = $"Инфо не указано";
                    }

                    userInfoLabel.Text = $"Имя: {user.Login}";
                }
            }
        }






    }
}
