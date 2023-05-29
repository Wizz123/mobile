using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Forum
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPage : ContentPage
    {
        Collection SelectedCollection;
        DBConnection conn = new DBConnection();
        public int UseId = (int)App.Current.Properties["UserId"];
        public bool IsAuthorized { get; set; }
        public DataTable User = new DataTable();
        public DataTable CollectionTAble = new DataTable();
        public IList<Collection> Collections { get; set; }
        public UserPage()
        {
            InitializeComponent();

            IsAuthorized = UseId > 0;

            // Основна логіка відображення елементів
            if (IsAuthorized)
            {
                // В разі наявності авторизації показати всі елементи сторінки
                ButtonOfReg.IsVisible = false;
                MainLabel.IsVisible = true;
                UsersCollectionsLabel.IsVisible = true;
                emailEntry.IsVisible = true;
                phoneEntry.IsVisible = true;
                nameEntry.IsVisible = true;
                loginEntry.IsVisible = true;
                registrationDateLabel.IsVisible = true;
                AddCollectionButton.IsVisible = true;
                Man.IsVisible = true;
                Logo.IsVisible = false;
                // Додаткова логіка відображення елементів, пов'язаних з авторизацією
                GetDataFromDB();
            }
            else
            {
                // В разі відсутності авторизації приховати деякі елементи
                ButtonOfReg.IsVisible = true;
                MainLabel.IsVisible = false;
                UsersCollectionsLabel.IsVisible = false;
                emailEntry.IsVisible = false;
                phoneEntry.IsVisible = false;
                nameEntry.IsVisible = false;
                loginEntry.IsVisible = false;
                registrationDateLabel.IsVisible = false;
                AddCollectionButton.IsVisible = false;
                // Додаткова логіка відображення елементів, пов'язаних з відсутністю авторизації
            }
        }

        async void GetDataFromDB()
        {
            User.Clear();
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM \"UserBasic\" WHERE user_id = @user_id", conn.GetConnection());
            command.Parameters.Add("@user_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = UseId;
            NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
            dataAdapter.Fill(User);

            CollectionTAble.Clear();
            NpgsqlCommand com = new NpgsqlCommand("SELECT * FROM \"Collection\" WHERE fk_user_id = @fk_user_id", conn.GetConnection());
            com.Parameters.Add("@fk_user_id", NpgsqlTypes.NpgsqlDbType.Integer).NpgsqlValue = UseId;
            NpgsqlDataAdapter comDataAdapter = new NpgsqlDataAdapter(com);
            comDataAdapter.Fill(CollectionTAble);
        }

        protected override void OnAppearing()
        {
            if (User.Rows.Count > 0)
            {
                emailEntry.Text = "Ел.пошта: " + User.Rows[0][2].ToString();
                phoneEntry.Text = "Номер телефону: " + User.Rows[0][1].ToString();
                nameEntry.Text = "Ім'я: " + User.Rows[0][0].ToString();
                loginEntry.Text = "Логін: " + User.Rows[0][4].ToString();
                registrationDateLabel.Text = "Дата реєстрації: " + User.Rows[0][3].ToString();

                Collections = new List<Collection>();
                for (int i = 0; i < CollectionTAble.Rows.Count; i++)
                {
                    Collections.Add(new Collection()
                    {
                        Name = CollectionTAble.Rows[i][1].ToString(),
                        Id = Convert.ToInt32(CollectionTAble.Rows[i][0]),
                        IsCoin = Convert.ToBoolean(CollectionTAble.Rows[i][2])
                    });
                }
                BindingContext = this;
                base.OnAppearing();
            }
        }

        private void OnLoginButtonClicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new Autorization());
        }

        private void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new Registration());
        }

        private void OnAddCollectionButtonClicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new AddCollectionPage());
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {
                SelectedCollection = e.CurrentSelection[0] as Collection;
                GetCollection.IsVisible = true;
                // Збереження айді вибраної колекції
                int selectedCollectionId = SelectedCollection.Id;
                // Передача айді в інші файли або методи
                // ...
                
            }
        }

        private void GetCollectionButtonClicked(object sender, EventArgs e)
        {
            App.Current.Properties["CollectionId"] = SelectedCollection.Id;
            Navigation.PushAsync(new CollectionPage());
        }
    }
}
