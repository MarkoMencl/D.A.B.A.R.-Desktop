using DataAcccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Managers
{
    public static class SessionManager
    {
        private static User user;

        public static User GetCurrentUser()
        {
            return user;
        }

        public static void SetCurrentUser(User newUser)
        {
            user = newUser;
        }

        public static void UpdateCurrentUser(User updatedUser)
        {
            SetCurrentUser(updatedUser);
            var userRepository = new UserRepository();
            userRepository.Update(updatedUser);
        }

        public static void RemoveCurrentUser()
        {
            user = null;
        }
    }
}
