using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Forum
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavoritesPage : ContentPage
    {
        int UseId;
        private Coin SelectedItem;
        public bool IsCoinSelected = true;
        private DataTable tableC = new DataTable();
        private DataTable tableB = new DataTable();
        DBConnection conn = new DBConnection();
        public ObservableCollection<Coin> Coins { get; set; }
        public FavoritesPage()
        {
            
            InitializeComponent();
            Coins = new ObservableCollection<Coin>();
            GetDataFromDB();
            
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            IsCoinSelected = false;
            Coins.Clear();
            GetDataFromDB();
            OnAppearing();
            BindingContext = null;
            BindingContext = this;
        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {
            IsCoinSelected = true;
            Coins.Clear();
            GetDataFromDB();
            OnAppearing();
            BindingContext = null;
            BindingContext = this;
        }

        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {
                SelectedItem = e.CurrentSelection[0] as Coin;
                ButtonStack.IsVisible = true;

            }
            else
            {
                SelectedItem = null;
                ButtonStack.IsVisible = false;
            }

        }

        async void GetDataFromDB()
        {
            UseId = (int)App.Current.Properties["UserId"];

            if (IsCoinSelected)
            {
                tableC.Clear();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"Coin\" INNER JOIN \"Favorite_coin\" ON \"Coin\".\"coin_id\"=\"Favorite_coin\".\"fk_coin_id\" WHERE fk_user_id = @fk_user_id", conn.GetConnection());
                cmd.Parameters.Add("@fk_user_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = UseId;
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(tableC);
            }
            else
            {
                tableB.Clear();
                NpgsqlCommand cmd1 = new NpgsqlCommand("SELECT * FROM \"Banknote\" INNER JOIN \"Favorite_banknote\" ON \"Banknote\".\"banknote_id\"=\"Favorite_banknote\".\"fk_banknote_id\" WHERE fk_user_id = @fk_user_id", conn.GetConnection());
                cmd1.Parameters.Add("@fk_user_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = UseId;
                NpgsqlDataAdapter adapter1 = new NpgsqlDataAdapter();
                adapter1.SelectCommand = cmd1;
                adapter1.Fill(tableB);


            }
        }
        protected override async void OnAppearing()
        {
            GetDataFromDB();
            UseId = (int)App.Current.Properties["UserId"];
            if (IsCoinSelected)
            {
               
                Coins.Clear();
                for (int i = 0; i < tableC.Rows.Count; i++)
                {
                    Coins.Add(new Coin()
                    {
                        Name = tableC.Rows[i][1].ToString(),
                        Country = tableC.Rows[i][3].ToString(),
                        Image = tableC.Rows[i][4].ToString(),
                        Weight = tableC.Rows[i][2].ToString(),
                        Size = tableC.Rows[i][7].ToString(),
                        Id = Convert.ToInt32(tableC.Rows[i][5]),
                        Year = tableC.Rows[i][6].ToString(),
                        Rare = tableC.Rows[i][8].ToString(),
                        Description = "Назва: " + tableC.Rows[i][1].ToString() + "; " + "Країна: " + tableC.Rows[i][3].ToString() + "; " + "Рік: " + tableC.Rows[i][6].ToString() + "; " + "Рідкісність: " + tableC.Rows[i][8] + "; " + "Вага: " + tableC.Rows[i][2].ToString() + "; " + "Розміри: " + tableC.Rows[i][7].ToString() + "."
                    });
                }
                BindingContext = this;
                base.OnAppearing();
            }
            else
            {
                
                Coins.Clear();
                for (int j = 0; j < tableB.Rows.Count; j++)
                {
                    Coins.Add(new Coin()
                    {
                        Name = tableB.Rows[j][0].ToString(),
                        Country = tableB.Rows[j][1].ToString(),
                        Image = tableB.Rows[j][4].ToString(),
                        Weight = tableB.Rows[j][2].ToString(),
                        Size = tableB.Rows[j][3].ToString(),
                        Id = Convert.ToInt32(tableB.Rows[j][5].ToString()),
                        Year = tableB.Rows[j][6].ToString(),
                        Rare = tableB.Rows[j][7].ToString(),
                        Description = "Назва: " + tableB.Rows[j][0].ToString() + "; " + "Країна: " + tableB.Rows[j][1].ToString() + "; " + "Рік: " + tableB.Rows[j][6].ToString() + "; " + "Рідкісність: " + tableB.Rows[j][7] + "; " + "Вага: " + tableB.Rows[j][2].ToString() + "; " + "Розміри: " + tableB.Rows[j][3].ToString() + "."
                    });
                }
                BindingContext = this;
                base.OnAppearing();
            }

        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            UseId = (int)App.Current.Properties["UserId"];
            if (IsCoinSelected)
            {
                NpgsqlCommand delcommand1 = new NpgsqlCommand("Delete From \"Favorite_coin\" Where fk_user_id = @fk_user_id AND fk_coin_id = @fk_coin_id", conn.GetConnection());
                delcommand1.Parameters.Add("@fk_user_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = UseId;
                delcommand1.Parameters.Add("@fk_coin_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = SelectedItem.Id;
                conn.OpenConnection();
                try
                {
                    int a = delcommand1.ExecuteNonQuery();
                    if(a == 1)
                    {
                        await DisplayAlert("Повідомлення", "Видалено", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Повідомлення", "Помилка видалення", "OK");
                    }
                }
                catch
                {
                    await DisplayAlert("Повідомлення", "Помилка видалення", "OK");
                }
                conn.CloseConnection();
            }
            else
            {
                NpgsqlCommand delcommand2 = new NpgsqlCommand("Delete From \"Favorite_banknote\" Where fk_user_id = @fk_user_id AND fk_banknote_id = @fk_banknote_id", conn.GetConnection());
                delcommand2.Parameters.Add("@fk_user_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = UseId;
                delcommand2.Parameters.Add("@fk_banknote_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = SelectedItem.Id;
                conn.OpenConnection();
                try
                {
                    int a1 = delcommand2.ExecuteNonQuery();
                    if (a1 == 1)
                    {
                        await DisplayAlert("Повідомлення", "Видалено", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Повідомлення", "Помилка видалення", "OK");
                    }
                }
                catch
                {
                    await DisplayAlert("Повідомлення", "Помилка видалення", "OK");
                }
                conn.CloseConnection();
            }
        }
    }
}