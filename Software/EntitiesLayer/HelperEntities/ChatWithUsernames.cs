using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesLayer.HelperEntities
{
    public class ChatWithUsernames
    {
        public Chat Chat { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }
    }
}
