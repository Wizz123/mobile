using Npgsql;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Forum
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        DBConnection dbConnection;
        int chatId;

        public ChatPage(int chatId)
        {
            this.chatId = chatId;
            dbConnection = new DBConnection();
            InitializeComponent();

            messageList.ItemTemplate = new DataTemplate(() =>
            {
                var loginLabel = new Label
                {
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold
                };
                loginLabel.SetBinding(Label.TextProperty, "UserLogin");

                var messageLabel = new Label
                {
                    LineBreakMode = LineBreakMode.WordWrap
                };
                messageLabel.SetBinding(Label.TextProperty, "MessageBody");

                var sendDataLabel = new Label
                {
                    FontSize = 12,
                    TextColor = Color.Gray,
                    LineBreakMode = LineBreakMode.WordWrap
                };
                sendDataLabel.SetBinding(Label.TextProperty, new Binding("SendData", stringFormat: "{0:dd.MM.yyyy HH:mm}"));

                var messageLayout = new Grid();
                messageLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                messageLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                messageLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var flexLayout = new FlexLayout();
                flexLayout.Wrap = FlexWrap.Wrap;
                flexLayout.Children.Add(messageLabel);

                messageLayout.Children.Add(loginLabel, 0, 0);
                messageLayout.Children.Add(flexLayout, 1, 0);
                messageLayout.Children.Add(sendDataLabel, 2, 0);

                var messageFrame = new Frame
                {
                    Content = messageLayout,
                    Padding = new Thickness(10), // Відступи від рамки
                    BorderColor = Color.Gray, // Колір рамки
                    BackgroundColor = Color.White, // Колір фону рамки
                    VerticalOptions = LayoutOptions.FillAndExpand // Розтягнути messageLayout по вертикалі
                };

                return new ViewCell { View = messageFrame };
            });

            messageEntry = new Entry
            {
                Placeholder = "Напишіть повідомлення",
                Keyboard = Keyboard.Text
            };

            sendButton = new Button
            {
                Text = "Відправити",
                BackgroundColor = Color.FromHex("#4CAF50"),
                TextColor = Color.White
            };
            sendButton.Clicked += OnSendButtonClicked;

            Content = new Grid
            {
                Margin = new Thickness(20),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                Children =
                {
                    { new Label { Text = GetChatName(), FontSize = 24, HorizontalOptions = LayoutOptions.CenterAndExpand }, 0, 0 },
                    { messageList, 0, 1 },
                    { new StackLayout { BackgroundColor = Color.FromHex("#F2F2F2"), Children = { messageEntry } }, 0, 2 },
                    { sendButton, 0, 3 }
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadMessages();
        }

        private string GetChatName()
        {
            NpgsqlConnection connection = dbConnection.GetConnection();
            dbConnection.OpenConnection();

            try
            {
                string query = $"SELECT chat_name FROM \"Chat\" WHERE chat_id = {chatId}";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return result.ToString();
                    }
                }
            }
            finally
            {
                dbConnection.CloseConnection();
            }

            return "Невідомий чат";
        }

        private void LoadMessages()
        {
            NpgsqlConnection connection = dbConnection.GetConnection();
            dbConnection.OpenConnection();

            try
            {
                string query = $"SELECT m.massage_id, m.message_body, m.send_date, m.fk_user_id, m.fk_chat_id, u.user_login FROM \"Message\" m JOIN \"UserBasic\" u ON m.fk_user_id = u.user_id WHERE m.fk_chat_id = {chatId}";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        List<Message> messages = new List<Message>();
                        while (reader.Read())
                        {
                            Message message = new Message
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("massage_id")),
                                MessageBody = reader.GetString(reader.GetOrdinal("message_body")),
                                SendData = reader.GetDateTime(reader.GetOrdinal("send_date")),
                                UserId = reader.GetInt32(reader.GetOrdinal("fk_user_id")),
                                ChatId = chatId,
                                UserLogin = reader.GetString(reader.GetOrdinal("user_login"))
                            };
                            messages.Add(message);
                        }
                        messageList.ItemsSource = messages;
                    }
                }
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            int UserId = (int)App.Current.Properties["UserId"];
            if (UserId == 0)
            {
                await DisplayAlert("Помилка", "Авторизуйтеся", "OK");
            }
            else
            {
                string messageBody = messageEntry.Text;

                if (!string.IsNullOrWhiteSpace(messageBody))
                {
                    using (NpgsqlConnection connection = dbConnection.GetConnection())
                    {
                        connection.Open();

                        string query = $"INSERT INTO \"Message\" (message_body, send_date, fk_user_id, fk_chat_id) VALUES ('{messageBody}', '{DateTime.Now}', {App.Current.Properties["UserId"]}, {chatId})";
                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            try
                            {
                                int rowsAffected = command.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    messageEntry.Text = "";
                                    LoadMessages();
                                }
                                else
                                {
                                    await DisplayAlert("Помилка", "Не вдалося надіслати повідомлення", "OK");
                                }
                            }
                            catch (Exception ex)
                            {
                                await DisplayAlert("Помилка", $"Сталася помилка при відправці повідомлення: {ex.Message}", "OK");
                            }
                        }
                    }
                }

            }
        }
    }

    public class Message
    {
        public int Id { get; set; }
        public string MessageBody { get; set; }
        public DateTime SendData { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public string UserLogin { get; set; }
    }
}
