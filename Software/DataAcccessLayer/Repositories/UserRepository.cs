using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcccessLayer.Repositories
{
    public class UserRepository : Repository<User>
    {
        public UserRepository() : base(new DabarModel())
        {
        }

        public User CheckIfUserExists(string username, string password)
        {
            var query = from user in Entities
                        where user.username == username && user.password == password
                        select user;

            return query.FirstOrDefault();
        }

        public bool CheckIfUserExists(string username)
        {
            var query = from user in Entities
                        where user.username == username
                        select user;

            return query.Any();
        }

        public override int Update(User entity, bool saveChanges = true)
        {
            var userToUpdate = Entities.FirstOrDefault(u => u.id == entity.id);

            if (userToUpdate == null)
                throw new ArgumentException("User not found.");

            userToUpdate.id = entity.id;
            userToUpdate.username = entity.username;
            userToUpdate.email = entity.email;
            userToUpdate.contact = entity.contact;
            userToUpdate.location_id = entity.location_id;
            userToUpdate.language = entity.language;
            userToUpdate.image_id = entity.image_id;

            if (saveChanges)
                return Context.SaveChanges();

            return 0;
        }

    }
}
