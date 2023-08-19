using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

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
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `users`", db.GetConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);

            ClearUserInfoLabels(); // Очищаем информацию на метках

            foreach (DataRow row in table.Rows)
            {
                string login = row["login"].ToString();
                Button userButton = new Button();
                userButton.Text = login;
                userButton.Click += (sender, e) => HandleUserButtonClick(sender, e, login); // Обработчик для клика по кнопке
                flowLayoutPanel.Controls.Add(userButton); // Добавляем кнопку в FlowLayoutPanel
            }
        }


        private void HandleUserButtonClick(object sender, EventArgs e, string login)
        {
            Db db = new Db();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            using (MySqlConnection connection = db.GetConnection()) // Открываем подключение
            {
                MySqlCommand command = new MySqlCommand("SELECT id, name, surname FROM `users` WHERE `login` = @uL", connection);
                command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = login;

                adapter.SelectCommand = command;
                adapter.Fill(table);
                ClearUserInfoLabels();

                if (table.Rows.Count > 0)
                {
                    string name = table.Rows[0]["name"].ToString();
                    string surname = table.Rows[0]["surname"].ToString();
                    int id = Convert.ToInt32(table.Rows[0]["id"]);

                    MySqlCommand ageInfoCommand = new MySqlCommand("SELECT age, info FROM `userprofiles` WHERE `userid` = @uId", connection);
                    ageInfoCommand.Parameters.Add("@uId", MySqlDbType.Int32).Value = id;

                    using (MySqlDataReader reader = ageInfoCommand.ExecuteReader())
                    {
                        int age = 0;
                        string info = "";
                        if (reader.Read())
                        {
                            try
                            {
                                age = reader.GetInt32("age");
                            }
                            catch
                            {
                                MessageBox.Show("Не получилось получить возраст");
                            }

                            try
                            {
                                info = reader.GetString("info");
                            }
                            catch
                            {
                                MessageBox.Show("Не получилось получить инфо");
                            }

                            ageLabel.Text = $"Возраст: {age}";
                            userInfo.Text = $"Инфо: {info}";
                        }
                        userInfoLabel.Text = $"Имя: {name}\nФамилия: {surname}";
                    }
                }
            }

        }




    }
}
