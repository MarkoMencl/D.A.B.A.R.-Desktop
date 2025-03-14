using EntitiesLayer.Entities;
using EntitiesLayer.HelperEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcccessLayer.Repositories
{
    public class ChatRepository : Repository<Chat>
    {
        public ChatRepository() : base(new DabarModel())
        {
        }

        public IQueryable<ChatWithUsernames> GetChatMessages(int buyerId, int sellerId)
        {
            var query = from chat in Entities
                        join sender in Context.Users on chat.user_id_sender equals sender.id
                        join receiver in Context.Users on chat.user_id_receiver equals receiver.id
                        where buyerId == chat.user_id_sender && sellerId == chat.user_id_receiver ||
                        buyerId == chat.user_id_receiver && sellerId == chat.user_id_sender
                        select new ChatWithUsernames
                        {
                            Chat = chat,
                            SenderUsername = sender.username,
                            ReceiverUsername = receiver.username
                        };

            return query;
        }

        public IQueryable<ChatWithUsernames> GetChats(int userId)
        {
            var query = from chat in Entities
                        join sender in Context.Users on chat.user_id_sender equals sender.id
                        join receiver in Context.Users on chat.user_id_receiver equals receiver.id
                        where chat.user_id_sender == userId || chat.user_id_receiver == userId
                        group new { chat, sender, receiver } by new
                        {
                            Pair = chat.user_id_sender == userId ? chat.user_id_receiver : chat.user_id_sender
                        } into groupedChats
                        let latestChat = groupedChats.OrderByDescending(c => c.chat.date).FirstOrDefault()
                        select new ChatWithUsernames
                        {
                            Chat = latestChat.chat,
                            SenderUsername = latestChat.sender.username,
                            ReceiverUsername = latestChat.receiver.username
                        };

            return query;
        }

        public override int Update(Chat entity, bool saveChanges = true)
        {
            throw new NotImplementedException();
        }
    }
}
