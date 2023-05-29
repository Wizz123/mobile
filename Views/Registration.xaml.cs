using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Forum
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registration : ContentPage
    {
        DBConnection conn = new DBConnection();
        public Registration()
        {
            InitializeComponent();
        }

        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            try
            {
                string phoneNumber = phoneNumberEntry.Text;
                string email = emailEntry.Text;
                string name = nameEntry.Text;
                string username = usernameEntry.Text;
                string password = passwordEntry.Text;

                NpgsqlCommand npgsqlCommand = new NpgsqlCommand("Insert Into \"UserBasic\" (user_phone, user_mail, user_name, user_login, user_password, registration_date, user_image, last_login) Values (@user_phone, @user_mail, @user_name, @user_login, @user_password, @registration_date, @user_image, @last_login)", conn.GetConnection());
                npgsqlCommand.Parameters.Add("@user_phone", NpgsqlDbType.Varchar).NpgsqlValue = phoneNumber;
                npgsqlCommand.Parameters.Add("@user_mail", NpgsqlDbType.Varchar).NpgsqlValue = email;
                npgsqlCommand.Parameters.Add("@user_name", NpgsqlDbType.Varchar).NpgsqlValue = name;
                npgsqlCommand.Parameters.Add("@user_login", NpgsqlDbType.Varchar).NpgsqlValue = username;
                npgsqlCommand.Parameters.Add("@user_password", NpgsqlDbType.Varchar).NpgsqlValue = password;
                npgsqlCommand.Parameters.AddWithValue("@registration_date", DateTime.Now.Date);
                npgsqlCommand.Parameters.Add("@user_image", NpgsqlDbType.Varchar).NpgsqlValue = "user.png";
                
                npgsqlCommand.Parameters.Add("@last_login", NpgsqlDbType.Timestamp).NpgsqlValue = DateTime.Now;
                conn.OpenConnection();
                if (phoneNumber != null && email != null && name != null && username != null && password != null)
                {
                    try
                    {
                        int a = npgsqlCommand.ExecuteNonQuery();
                        if (a == 1)
                        {
                            await DisplayAlert("Повідомлення", "Реестрація вдала", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Повідомлення", "Помилка реєстрації", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("1Повідомлення", ex.Message, "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Повідомлення", "Заповніть усі поля", "OK");
                }
                conn.CloseConnection();
            }
            catch(Exception ex)
            {
                await DisplayAlert("2Повідомлення", ex.Message, "OK");
            }
        }
    }
}