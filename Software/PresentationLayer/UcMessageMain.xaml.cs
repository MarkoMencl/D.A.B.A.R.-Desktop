using BusinessLogicLayer.Managers;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using EntitiesLayer.HelperEntities;
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
    /// Interaction logic for UcMessageMain.xaml
    /// </summary>
    public partial class UcMessageMain : UserControl
    {
        private User otherUser;

        public UcMessageMain()
        {
            InitializeComponent();
            GetChats();
            otherUser = new User();
            scrlvCurrentChat.ScrollToBottom();
        }

        public UcMessageMain(int otherUserId)
        {
            InitializeComponent();
            otherUser = new User();
            otherUser.id = otherUserId;
            GetChats();
            GetMessages();
            scrlvCurrentChat.ScrollToBottom();
        }

        ChatService service = new ChatService();

        private void GetChats()
        {
            List<ChatWithUsernames> chats = service.GetChats();

            foreach (ChatWithUsernames chat in chats)
            {
                UcMessage newestMessage = new UcMessage(chat, true);

                newestMessage.MouseLeftButtonUp += (sender, e) =>
                {
                    var user = SessionManager.GetCurrentUser();

                    if (user.id == chat.Chat.user_id_sender)
                    {
                        otherUser.id = chat.Chat.user_id_receiver;
                    }
                    else
                    {
                        otherUser.id = chat.Chat.user_id_sender;
                    }

                    stpCurrentChat.Children.Clear();
                    GetMessages();
                };

                stpLatestChats.Children.Add(newestMessage);
            }
        }

        private void GetMessages()
        {
            List<ChatWithUsernames> messages = service.GetMessages(otherUser.id).ToList();

            foreach (ChatWithUsernames chat in messages)
            {
                UcMessage message = new UcMessage(chat, false);

                stpCurrentChat.Children.Add(message);
            }
        }

        private void btnSendNewMessage_Click(object sender, RoutedEventArgs e)
        {
            if (txtWriteNewMessage.Text == "")
            {
                return;
            }

            service.AddMessage(otherUser.id, txtWriteNewMessage.Text);
            stpLatestChats.Children.Clear();
            GetChats();
            GetMessages();
            txtWriteNewMessage.Text = "";
            scrlvCurrentChat.ScrollToBottom();
        }
    }
}
