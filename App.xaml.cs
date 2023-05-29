using Prism.Ioc;
using Xamarin.Forms;
using Forum;
using Forum.Views;

namespace Forum
{
    public partial class App
    {
        public static DBConnection DatabaseConnection { get; private set; }

        public App()
        {
            InitializeComponent();

            // Ініціалізація з'єднання з базою даних
            DatabaseConnection = new DBConnection();
            
        }

        protected override void OnStart()
        {
            App.Current.Properties["UserId"] = 0;
            App.Current.Properties["IsAdmin"] = false;
            // Відкриття з'єднання з базою даних
            DatabaseConnection.OpenConnection();

            // Перехід до головної сторінки
            MainPage = new AppShell();
            
        }

        protected override void OnSleep()
        {
            // Закриття з'єднання з базою даних
            DatabaseConnection.CloseConnection();
        }

        protected override void OnResume()
        {
            // Відкриття з'єднання з базою даних при відновленні додатка зі сплячого режиму
            DatabaseConnection.OpenConnection();
        }
    }
}

