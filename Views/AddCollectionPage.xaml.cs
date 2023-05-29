using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Forum
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCollectionPage : ContentPage
    {
        DBConnection conn = new DBConnection();
        public int UseId = (int)App.Current.Properties["UserId"]; /*Вадим, этот айди нужно вытащить из авторизации*/
        public bool IsCoinSelected = true;
        public AddCollectionPage()
        {
            InitializeComponent();
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            if (collectionTypeSwitch.IsToggled)
            {
                IsCoinSelected = false;
            }
            string CollectionName = collectionEntry.Text.ToString();
            if (IsCoinSelected)
            {
                NpgsqlCommand command = new NpgsqlCommand("Insert Into \"Collection\" (collection_name, is_coin_collection, is_bancnote_collection, fk_user_id) Values (@collection_name, @is_coin_collection, @is_banknote_collection, @fk_user_id)", conn.GetConnection());
                command.Parameters.Add("@collection_name", NpgsqlTypes.NpgsqlDbType.Varchar).NpgsqlValue = CollectionName;
                command.Parameters.Add("@is_coin_collection", NpgsqlTypes.NpgsqlDbType.Boolean).NpgsqlValue = true;
                command.Parameters.Add("@is_banknote_collection", NpgsqlTypes.NpgsqlDbType.Boolean).NpgsqlValue = false;
                command.Parameters.Add("@fk_user_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = UseId;
                conn.OpenConnection();
                try
                {
                    int a1 = command.ExecuteNonQuery();
                    if(a1 == 1)
                    {
                        await DisplayAlert("Повідомлення", "Додано вдало", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Повідомлення", "Помилка додавання", "OK");
                    }
                }
                catch(Exception ex)
                {
                    await DisplayAlert("Повідомлення", "1Помилка додавання: " + ex.Message, "OK");
                }
                conn.CloseConnection();
            }
            else
            {
                NpgsqlCommand command1 = new NpgsqlCommand("Insert Into \"Collection\" (collection_name, is_coin_collection, is_bancnote_collection, fk_user_id) Values (@collection_name, @is_coin_collection, @is_banknote_collection, @fk_user_id)", conn.GetConnection());
                command1.Parameters.Add("@collection_name", NpgsqlTypes.NpgsqlDbType.Varchar).NpgsqlValue = CollectionName;
                command1.Parameters.Add("@is_coin_collection", NpgsqlTypes.NpgsqlDbType.Boolean).NpgsqlValue = false;
                command1.Parameters.Add("@is_banknote_collection", NpgsqlTypes.NpgsqlDbType.Boolean).NpgsqlValue = true;
                command1.Parameters.Add("@fk_user_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = UseId;
                conn.OpenConnection();
                try
                {
                    int a2 = command1.ExecuteNonQuery();
                    if (a2 == 1)
                    {
                        await DisplayAlert("Повідомлення", "Додано вдало", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Повідомлення", "Помилка додавання", "OK");
                    }
                }
                catch
                {
                    await DisplayAlert("Повідомлення", "1Помилка додавання", "OK");
                }
                conn.CloseConnection();
            }

        }
    }
}