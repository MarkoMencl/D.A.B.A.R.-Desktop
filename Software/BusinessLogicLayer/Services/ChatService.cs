using BusinessLogicLayer.Managers;
using DataAcccessLayer.Repositories;
using EntitiesLayer.Entities;
using EntitiesLayer.HelperEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ChatService
    {
        public List<ChatWithUsernames> GetChats()
        {
            using(var repo = new ChatRepository())
            {
                var user = SessionManager.GetCurrentUser();

                List<ChatWithUsernames> chats = repo.GetChats(user.id).ToList();

                return chats;
            }
        }

        public List<ChatWithUsernames> GetMessages(int otherUserId)
        {
            using(var repo = new ChatRepository())
            {
                var user = SessionManager.GetCurrentUser();

                List<ChatWithUsernames> messages = repo.GetChatMessages(user.id, otherUserId).ToList();

                return messages;
            }
        }

        public void AddMessage(int currChatUser, string content)
        {
            using(var repo = new ChatRepository())
            {
                Chat chat = new Chat();
                var user = SessionManager.GetCurrentUser();

                chat.user_id_sender = user.id;
                chat.user_id_receiver = currChatUser;
                chat.chat_content = content;
                chat.date = DateTime.Now;

                repo.Add(chat);
            }
        }
    }
}
