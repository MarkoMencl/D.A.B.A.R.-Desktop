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
    /// Interaction logic for UcMessage.xaml
    /// </summary>
    public partial class UcMessage : UserControl
    {
        public int SenderID { get; }
        public int ReceiverID { get; }

        public UcMessage(ChatWithUsernames chatWithUsernames, bool isPreviewMessage)
        {
            InitializeComponent();
            var user = SessionManager.GetCurrentUser();
            Chat chat = chatWithUsernames.Chat;

            SenderID = chat.user_id_sender;
            ReceiverID = chat.user_id_receiver;
            txtblMessageContent.Text = chat.chat_content;
            txtblDate.Text = chat.date.ToString("yyyy-MM-dd HH:mm:ss");

            if (isPreviewMessage)
            {
                txtblMessageContent.TextWrapping = TextWrapping.NoWrap;
                txtblMessageContent.TextTrimming = TextTrimming.CharacterEllipsis;

                txtblUsername.Text = SenderID == user.id
                    ? chatWithUsernames.ReceiverUsername
                    : chatWithUsernames.SenderUsername;
            }
            else
            {
                txtblMessageContent.TextWrapping = TextWrapping.Wrap;
                txtblMessageContent.TextTrimming = TextTrimming.None;

                if (SenderID == user.id)
                {
                    txtblUsername.Text = chatWithUsernames.SenderUsername;
                    HorizontalAlignment = HorizontalAlignment.Right;
                }
                else
                {
                    txtblUsername.Text = SenderID == chat.user_id_sender 
                        ? chatWithUsernames.SenderUsername 
                        : chatWithUsernames.ReceiverUsername;

                    HorizontalAlignment = HorizontalAlignment.Left;
                }
            }
        }
    }
}
