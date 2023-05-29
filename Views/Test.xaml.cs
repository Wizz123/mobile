using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Forum
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Test : ContentPage
    {
        private Coin SelectedItem;
        public bool IsCoinSelected = true;
        private DataTable tableC = new DataTable();
        private DataTable tableB = new DataTable();
        DBConnection conn = new DBConnection();
        int UserId;
        public IList<Coin> Coins { get; set; }
        public Test()
        {
            InitializeComponent();
            GetDataFromDB();
        }

        async void GetDataFromDB()
        {
            
            if (IsCoinSelected)
            {
                tableC.Clear();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"Coin\"", conn.GetConnection());
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(tableC);
            }
            else
            {
                tableB.Clear();
                NpgsqlCommand cmd1 = new NpgsqlCommand("SELECT * FROM \"Banknote\"", conn.GetConnection());
                NpgsqlDataAdapter adapter1 = new NpgsqlDataAdapter();
                adapter1.SelectCommand = cmd1;
                adapter1.Fill(tableB);

                
            }
        }

        protected override void OnAppearing()
        {
            if (IsCoinSelected)
            {
                Coins = new List<Coin>();
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
                Coins = new List<Coin>();
                for(int j = 0; j < tableB.Rows.Count; j++)
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

            // Очистка текущего выделения
            //((CollectionView)sender).SelectedItem = null;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            string searchText = SearchEntry.Text;

            if (!string.IsNullOrEmpty(searchText))
            {
                var filteredCoins = Coins.Where(coin => coin.Name.ToLower().Contains(searchText.ToLower())).ToList();
                Coins = new List<Coin>(filteredCoins);
            }
            else
            {
                GetDataFromDB();
                OnAppearing();
            }

            // Обновление привязки данных
            BindingContext = null;
            BindingContext = this;
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            ButtonStack.IsVisible = false;
            IsCoinSelected = false; 
            GetDataFromDB();
            OnAppearing();
            BindingContext = null;
            BindingContext = this;
        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {
            ButtonStack.IsVisible = false;  
            IsCoinSelected = true;
            GetDataFromDB();
            OnAppearing();
            BindingContext = null;
            BindingContext = this;
        }

        private async void AddToFavorites_Clicked(object sender, EventArgs e)
        {

            int userId = (int)App.Current.Properties["UserId"];

            if (SelectedItem != null)
            {
                if (IsCoinSelected)
                {
                    NpgsqlCommand com1 = new NpgsqlCommand("SELECT * FROM \"Favorite_coin\" Where fk_user_id = @userid and fk_coin_id = @coinid", conn.GetConnection());
                    com1.Parameters.Add("@userid", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = userId;
                    com1.Parameters.Add("@coinid", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = SelectedItem.Id;
                    NpgsqlDataAdapter adapter1 = new NpgsqlDataAdapter(com1);
                    DataTable dt1 = new DataTable();
                    adapter1.Fill(dt1);
                    if(dt1.Rows.Count > 0)
                    {
                        await DisplayAlert("Повідомлення", "Цей елемент вже доданий до Обраних", "OK");
                    }
                    else
                    {
                        NpgsqlCommand addcommand1 = new NpgsqlCommand("Insert Into \"Favorite_coin\" (fk_user_id, fk_coin_id) Values (@fk_user_id, @fk_coin_id)", conn.GetConnection());
                        addcommand1.Parameters.Add("@fk_user_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = userId;
                        addcommand1.Parameters.Add("@fk_coin_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = SelectedItem.Id;
                        conn.OpenConnection();
                        try
                        {
                            int a1 = addcommand1.ExecuteNonQuery();
                            if(a1 == 1)
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
                            await DisplayAlert("Повідомлення", "Помилка додавання", "OK");
                        }
                        conn.CloseConnection();
                    }
                }
                else
                {
                    NpgsqlCommand com2 = new NpgsqlCommand("SELECT * FROM \"Favorite_banknote\" Where fk_user_id = @userid and fk_banknote_id = @banknoteid", conn.GetConnection());
                    com2.Parameters.Add("@userid", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = userId;
                    com2.Parameters.Add("@banknoteid", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = SelectedItem.Id;
                    NpgsqlDataAdapter adapter2 = new NpgsqlDataAdapter(com2);
                    DataTable dt2 = new DataTable();
                    adapter2.Fill(dt2);
                    if (dt2.Rows.Count > 0)
                    {
                        await DisplayAlert("Повідомлення", "Цей елемент вже доданий до Обраних", "OK");
                    }
                    else
                    {
                        NpgsqlCommand addcommand2 = new NpgsqlCommand("Insert Into \"Favorite_banknote\" (fk_user_id, fk_banknote_id) Values (@fk_user_id, @fk_banknote_id)", conn.GetConnection());
                        addcommand2.Parameters.Add("@fk_user_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = userId;
                        addcommand2.Parameters.Add("@fk_banknote_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = SelectedItem.Id;
                        conn.OpenConnection();
                        try
                        {
                            int a2 = addcommand2.ExecuteNonQuery();
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
                            await DisplayAlert("Повідомлення", "Помилка додавання", "OK");
                        }
                        conn.CloseConnection();
                    }
                }
            }
        }

        private async void AddToCollection_Clicked(object sender, EventArgs e)
        {
            
            
            UserId = (int)App.Current.Properties["UserId"];


            if (SelectedItem != null)
            {
                if (IsCoinSelected)
                {
                    NpgsqlCommand command = new NpgsqlCommand("SELECT collection_id, collection_name FROM \"Collection\" Where fk_user_id = @fr_user_id AND is_coin_collection = @iscoinselection", conn.GetConnection());
                    command.Parameters.Add("@fr_user_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = UserId;
                    command.Parameters.Add("@iscoinselection", NpgsqlTypes.NpgsqlDbType.Boolean).NpgsqlValue = true;
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        string collections = "";
                        for(int i = 0; i < dt.Rows.Count; i++)
                        {
                            collections+= dt.Rows[i][0].ToString() + " - " + dt.Rows[i][1] + "; ";
                        }
                        string InputString = await DisplayPromptAsync("Вибір колекції", collections, "OK", "Cancel", "Номер колеції", initialValue: "3", keyboard: Keyboard.Numeric);
                        int collectionId = Convert.ToInt32(InputString);
                        
                        NpgsqlCommand search = new NpgsqlCommand("Select * From \"coin_collection\" Where fk_coin_id = @fk_coin_id AND fk_collection_id = @fk_collection_id", conn.GetConnection());
                        search.Parameters.Add("@fk_coin_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = SelectedItem.Id;
                        search.Parameters.Add("@fk_collection_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = collectionId;
                        NpgsqlDataAdapter searchadapter = new NpgsqlDataAdapter(search);
                        DataTable td = new DataTable();
                        searchadapter.Fill(td);
                        if(td.Rows.Count > 0)
                        {
                            await DisplayAlert("Повідомлення", "Елемент вже доданий до цієї колекції", "OK");
                        }
                        else
                        {
                            NpgsqlCommand AddCoinCommand = new NpgsqlCommand("Insert Into \"coin_collection\" (fk_collection_id, fk_coin_id) Values (@fk_collection_id, @fk_coin_id)", conn.GetConnection());
                            AddCoinCommand.Parameters.Add("@fk_collection_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = collectionId;
                            AddCoinCommand.Parameters.Add("@fk_coin_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = SelectedItem.Id;
                            conn.OpenConnection();
                            try
                            {
                                int a3 = AddCoinCommand.ExecuteNonQuery();
                                if(a3 == 1)
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
                                await DisplayAlert("Повідомлення", "Помилка додавання", "OK");
                            }
                            conn.CloseConnection();
                        }
                    }
                    else
                    {
                        await DisplayAlert("Повідомлення", "Немає доступних колекцій", "OK");
                    }
                }
                else
                {
                    NpgsqlCommand command1 = new NpgsqlCommand("SELECT collection_id, collection_name FROM \"Collection\" Where fk_user_id = @fr_user_id AND is_coin_collection = @iscoinselection", conn.GetConnection());
                    command1.Parameters.Add("@fr_user_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = UserId;
                    command1.Parameters.Add("@iscoinselection", NpgsqlTypes.NpgsqlDbType.Boolean).NpgsqlValue = false;
                    NpgsqlDataAdapter adapter1 = new NpgsqlDataAdapter(command1);
                    DataTable dt1 = new DataTable();
                    adapter1.Fill(dt1);
                    if (dt1.Rows.Count > 0)
                    {
                        string collections1 = "";
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            collections1 += dt1.Rows[i][0].ToString() + " - " + dt1.Rows[i][1] + "; ";
                        }
                        string InputString1 = await DisplayPromptAsync("Вибір колекції", collections1, "OK", "Cancel", "Номер колеції", initialValue: "3", keyboard: Keyboard.Numeric);
                        int collectionId1 = Convert.ToInt32(InputString1);
                        NpgsqlCommand search1 = new NpgsqlCommand("Select * From \"banknote_collection\" Where fk_banknot_id = @fk_banknot_id AND fk_collection_id = @fk_collection_id", conn.GetConnection());
                        search1.Parameters.Add("@fk_banknot_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = SelectedItem.Id;
                        search1.Parameters.Add("@fk_collection_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = collectionId1;
                        NpgsqlDataAdapter searchadapter1 = new NpgsqlDataAdapter(search1);
                        DataTable td1 = new DataTable();
                        searchadapter1.Fill(td1);
                        if (td1.Rows.Count > 0)
                        {
                            await DisplayAlert("Повідомлення", "Елемент вже доданий до цієї колекції", "OK");
                        }
                        else
                        {
                            NpgsqlCommand AddBankCommand = new NpgsqlCommand("Insert Into \"banknote_collection\" (fk_collection_id, fk_banknot_id) Values (@fk_collection_id, @fk_banknot_id)", conn.GetConnection());
                            AddBankCommand.Parameters.Add("@fk_collection_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = collectionId1;
                            AddBankCommand.Parameters.Add("@fk_banknot_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = SelectedItem.Id;
                            conn.OpenConnection();
                            try
                            {
                                int a31 = AddBankCommand.ExecuteNonQuery();
                                if (a31 == 1)
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
                                await DisplayAlert("Повідомлення", "Помилка додавання", "OK");
                            }
                            conn.CloseConnection();
                        }
                    }
                    else
                    {
                        await DisplayAlert("Повідомлення", "Немає доступних колекцій", "OK");
                    }
                }
            }
        }

        private async void Info_Clicked(object sender, EventArgs e)
        {
            if (SelectedItem != null)
            {
                if (SelectedItem is Coin selectedCoin)
                {
                    await DisplayAlert(selectedCoin.Name?.ToString(), selectedCoin.Description?.ToString(), "OK");
                }
            }
            // Очистка текущего выделения
            var collectionView = sender as CollectionView;
            if (collectionView != null)
            {
                collectionView.SelectedItem = null;
            }
        }
    }
}