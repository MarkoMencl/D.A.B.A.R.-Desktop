using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesLayer.HelperEntities
{
    public class ReviewWithUsernames
    {
        public Rating Review { get; set; }
        public User User { get; set; }
    }
}
