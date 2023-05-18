using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System.IO;
using SQLite;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using WDWPennyFinder.Data;
using System.Threading.Tasks;

namespace WDWPennyFinder
{
    public partial class App : Application
    {
        private static ItemDatabase database;
        public static ItemDatabase Database
        {
            get
            {
                // Insert initial data into the database
                Assembly assembly = typeof(App).Assembly;
                Stream dbStream = assembly.GetManifestResourceStream("WDWPennyFinder.item.db3");

                if (dbStream == null)
                {
                    throw new ArgumentException($"Resource not found.");
                }

                if (!File.Exists(DatabasePath))
                {
                    FileStream fileStream = File.Create(DatabasePath);
                    dbStream.Seek(0, SeekOrigin.Begin);
                    dbStream.CopyTo(fileStream);
                    dbStream.Close();
                }

                database = new ItemDatabase(DatabasePath);
                return database;
            }
        }

        public static string DatabasePath
        {
            get
            {
                string databasePath = Path.Combine(FileSystem.AppDataDirectory, "item.db3");
                return databasePath;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Get the current Page from the navigation stack
            var currentPage = MainPage.Navigation.NavigationStack.LastOrDefault();

            // Check if the current Page is the ItemsViewPage
            if (currentPage is ItemsPage itemsPage)
            {
                // Call the OnAppearing method of the ItemsViewPage
                Task.Run(() => itemsPage.CallAppearing());
            }
        }
    }
}
