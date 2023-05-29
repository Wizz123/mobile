using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Forum
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CollectionPage : ContentPage
    {
        private Coin SelectedItem;
        public bool IsCoinSelected = true;
        private DataTable tableC = new DataTable();
        private DataTable tableB = new DataTable();
        DBConnection conn = new DBConnection();
        int CollectionId = (int)App.Current.Properties["CollectionId"];
        public IList<Coin> Coins { get; set; }
        public CollectionPage()
        {
            InitializeComponent();
            GetDataFromDB();
        }
        async void GetDataFromDB()
        {
            NpgsqlCommand com = new NpgsqlCommand("Select is_coin_collection From \"Collection\" Where collection_id = @collection_id", conn.GetConnection());
            com.Parameters.Add("@collection_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = CollectionId;
            NpgsqlDataAdapter npgsql = new NpgsqlDataAdapter(com);
            DataTable table = new DataTable();
            npgsql.Fill(table);
            
            if (Convert.ToBoolean(table.Rows[0][0]) == true)
            {
                IsCoinSelected = true;
            }
            else
            {
                IsCoinSelected = false;
            }

            if (IsCoinSelected)
            {
                tableC.Clear();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"Coin\" INNER JOIN \"coin_collection\" ON \"Coin\".\"coin_id\"=\"coin_collection\".\"fk_coin_id\" WHERE fk_collection_id = @fk_collection_id", conn.GetConnection());
                cmd.Parameters.Add("@fk_collection_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = CollectionId;
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(tableC);
            }
            else
            {
                tableB.Clear();
                NpgsqlCommand cmd1 = new NpgsqlCommand("SELECT * FROM \"Banknote\" INNER JOIN \"banknote_collection\" ON \"Banknote\".\"banknote_id\"=\"banknote_collection\".\"fk_banknot_id\" WHERE fk_collection_id = @fk_collection_id", conn.GetConnection());
                cmd1.Parameters.Add("@fk_collection_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = CollectionId;
                NpgsqlDataAdapter adapter1 = new NpgsqlDataAdapter();
                adapter1.SelectCommand = cmd1;
                adapter1.Fill(tableB);
                

            }
        }
        protected override async void OnAppearing()
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
                try
                {
                    Coins = new List<Coin>();
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
                catch (Exception ex)
                {
                    await DisplayAlert(Title, ex.Message, "OK");
                }
            }

        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            if (IsCoinSelected)
            {
                NpgsqlCommand delItemCom = new NpgsqlCommand("Delete From \"coin_collection\" Where fk_collection_id = @fk_collection_id AND fk_coin_id = @fk_coin_id", conn.GetConnection());
                delItemCom.Parameters.Add("@fk_collection_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = CollectionId;
                delItemCom.Parameters.Add("@fk_coin_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = SelectedItem.Id;
                conn.OpenConnection();
                try
                {
                    int a = delItemCom.ExecuteNonQuery();
                    if(a == 1)
                    {
                        await DisplayAlert("Повідомлення", "Видалено вдало", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Повідомлення", "Помилка видалення", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Повідомлення", ex.Message, "OK");
                }
                finally
                {
                    conn.CloseConnection();
                }
            }
            else
            {
                NpgsqlCommand delItemCom1 = new NpgsqlCommand("Delete From \"banknote_collection\" Where fk_collection_id = @fk_collection_id AND fk_banknot_id = @fk_coin_id", conn.GetConnection());
                delItemCom1.Parameters.Add("@fk_collection_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = CollectionId;
                delItemCom1.Parameters.Add("@fk_coin_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = SelectedItem.Id;
                conn.OpenConnection();
                try
                {
                    int a1 = delItemCom1.ExecuteNonQuery();
                    if(a1 == 1)
                    {
                        await DisplayAlert("Повідомлення", "Видалено вдало", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Повідомлення", "Помилка видалення", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Повідомлення", ex.Message, "OK");
                }
                finally
                {
                    conn.CloseConnection();
                }
            }
            
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            NpgsqlCommand command = new NpgsqlCommand("Delete From \"Collection\" Where collection_id = @collection_id", conn.GetConnection());
            command.Parameters.Add("@collection_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = CollectionId;
            conn.OpenConnection();
            try
            {
                int a = command.ExecuteNonQuery();
                if(a == 1)
                {
                    await DisplayAlert("Повідомлення", "Видалено вдало", "OK");
                }
                else
                {
                    await DisplayAlert("Повідомлення", "Помилка видалення", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Повідомлення", ex.Message, "OK");
            }
            finally
            {
                conn.CloseConnection();
            }
        }
    }
}