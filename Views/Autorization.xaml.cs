using Forum;
using Forum.Views;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Forum
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Autorization : ContentPage
    {
        Entry usernameEntry;
        Entry passwordEntry;
        Button loginButton;

        DBConnection dbConnection;

        public Autorization()
        {
            dbConnection = new DBConnection();

            // Створення елементів відображення
            usernameEntry = new Entry
            {
                Placeholder = "Логін",
                Keyboard = Keyboard.Text
            };

            passwordEntry = new Entry
            {
                Placeholder = "Пароль",
                Keyboard = Keyboard.Text,
                IsPassword = true
            };

            loginButton = new Button
            {
                Text = "Увійти",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BorderWidth = 1.5, 
                CornerRadius = 50 ,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            loginButton.Clicked += OnLoginButtonClicked;


            // Розташування елементів на сторінці
            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children = {
                    new Label { Text = "Авторизація", FontSize = 24, HorizontalOptions = LayoutOptions.CenterAndExpand },
                    usernameEntry,
                    passwordEntry,
                    loginButton,
                }
            };
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string username = usernameEntry.Text;
            string password = passwordEntry.Text;

            // Відкриття з'єднання з базою даних
            dbConnection.OpenConnection();

            // Отримання користувача з бази даних
            User user = GetUserFromDatabase(username, password);

            // Закриття з'єднання з базою даних
            dbConnection.CloseConnection();

            if (user != null)
            {
                // Успішна авторизація
                // Збереження ідентифікатора користувача
                App.Current.Properties["IsUser"] = user.admin;
                App.Current.Properties["UserId"] = user.Id;
                await App.Current.SavePropertiesAsync();

                // Відкриття сторінки NotesPage
                //await Navigation.PushModalAsync(new AppShell());
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                // Помилка авторизації
                await DisplayAlert("Помилка", "Неправильний логін або пароль", "OK");
            }
        }

        private User GetUserFromDatabase(string username, string password)
        {
            NpgsqlConnection connection = dbConnection.GetConnection();

            // Виконання запиту до бази даних
            string query = $"SELECT * FROM \"UserBasic\" WHERE user_login = '{username}' AND user_password = '{password}'";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User user = new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("user_id")),
                            Username = reader.GetString(reader.GetOrdinal("user_login")),
                            Password = reader.GetString(reader.GetOrdinal("user_password")),
                            admin = reader.GetBoolean(reader.GetOrdinal("is_admin"))
                        };
                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }

    public class User
    {
        public bool admin { get; set; }
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}


