using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TicTacToe
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            MainPage mainPage = (MainPage)this.MainPage;
            mainPage.game.Save(mainPage.dataFolderName, mainPage.dataFileName);
        }

        protected override void OnResume()
        {
        }
    }
}
