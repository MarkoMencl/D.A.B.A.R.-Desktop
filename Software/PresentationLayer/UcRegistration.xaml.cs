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
    /// Interaction logic for UcRegistration.xaml
    /// </summary>
    public partial class UcRegistration : UserControl
    {
        public UcRegistration()
        {
            InitializeComponent();
        }

        AuthorisationService service = new AuthorisationService();

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            bool isSuccessfull = service.GetUserCredentials(txtRegistrationUsername.Text, txtRegistrationPassword.Text, txtRegistrationEmail.Text, txtRegistrationContact.Text);

            if (isSuccessfull)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;

                mainWindow.MainContentControl.Content = new UcMainPage();
                mainWindow.UpdateNavbar();
            }
        }
    }
}
