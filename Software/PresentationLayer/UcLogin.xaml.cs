using BusinessLogicLayer.Managers;
using BusinessLogicLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for UcLogin.xaml
    /// </summary>
    public partial class UcLogin : UserControl
    {
        public UcLogin()
        {
            InitializeComponent();
        }

        AuthorisationService service = new AuthorisationService();

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            bool isSuccessfull = service.GetUserCredentials(txtLoginUsername.Text, txtLoginPassword.Text);

            if (isSuccessfull)
            {
                var user = SessionManager.GetCurrentUser();
                if (user != null && !string.IsNullOrEmpty(user.language))
                {
                    LocaleManager.SetLanguage(user.language);
                }

                var mainWindow = Application.Current.MainWindow as MainWindow;

                mainWindow.MainContentControl.Content = new UcMainPage();
                mainWindow.UpdateNavbar();
            }
        }
    }
}
