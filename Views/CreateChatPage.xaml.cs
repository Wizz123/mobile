using Npgsql;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Forum
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateChatPage : ContentPage
    {
        DBConnection dbConnection;
        int userId;

        public CreateChatPage()
        {
            InitializeComponent();

            dbConnection = new DBConnection();
            

            chatList.ItemTapped += OnChatTapped;

            createButton.Clicked += OnCreateChatButtonClicked;

            chatList.SeparatorVisibility = SeparatorVisibility.Default;
            chatList.SeparatorColor = Color.Gray;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadChats();
        }

        private void LoadChats()
        {
            NpgsqlConnection connection = dbConnection.GetConnection();
            dbConnection.OpenConnection();

            try
            {
                string query = "SELECT * FROM \"Chat\"";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        List<Chat> chats = new List<Chat>();
                        while (reader.Read())
                        {
                            Chat chat = new Chat
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("chat_id")),
                                ChatName = reader.GetString(reader.GetOrdinal("chat_name")),
                                ChatTopic = reader.GetString(reader.GetOrdinal("chat_topic")),
                                UserId = reader.GetInt32(reader.GetOrdinal("fk_user_id"))
                            };
                            chats.Add(chat);
                        }
                        chatList.ItemsSource = chats;
                    }
                }
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

        private async void OnCreateChatButtonClicked(object sender, EventArgs e)
        {

            userId = (int)App.Current.Properties["UserId"];
            if (userId == 0)
            {
                await DisplayAlert("Помилка", "Авторизуйтесь", "OK");
            }
            else
            {
                string chatName = await DisplayPromptAsync("Створити чат", "Введіть назву чату");
                if (!string.IsNullOrWhiteSpace(chatName))
                {
                    string chatTopic = await DisplayPromptAsync("Створити чат", "Введіть тему чату");
                    if (!string.IsNullOrWhiteSpace(chatTopic))
                    {
                        using (NpgsqlConnection connection = dbConnection.GetConnection())
                        {
                            connection.Open();
                            string query = $"INSERT INTO \"Chat\" (chat_name, chat_topic, fk_user_id) VALUES ('{chatName}', '{chatTopic}', {userId})";
                            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                            {
                                int rowsAffected = command.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    await DisplayAlert("Створити чат", "Чат успішно створено", "OK");
                                    LoadChats();
                                }
                                else
                                {
                                    await DisplayAlert("Помилка", "Не вдалося створити чат", "OK");
                                }
                            }
                        }
                    }
                }
            }
        }

        private async void OnChatTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Chat selectedChat)
            {
                await Navigation.PushAsync(new ChatPage(selectedChat.Id));
            }
        }
    }

    public class Chat
    {
        public int Id { get; set; }
        public string ChatName { get; set; }
        public string ChatTopic { get; set; }
        public int UserId { get; set; }
    }
}