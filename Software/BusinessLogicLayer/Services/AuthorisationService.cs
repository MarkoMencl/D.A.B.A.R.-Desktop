using BusinessLogicLayer.Managers;
using DataAcccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class AuthorisationService
    {
        public bool GetUserCredentials(string username, string password)
        {
            using(var repo = new UserRepository())
            {
                string hashedPassword = HashPassword(password);
                User user = repo.CheckIfUserExists(username, hashedPassword) as User;

                if (user != null)
                {
                    SessionManager.SetCurrentUser(user);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool GetUserCredentials(string username, string password, string email, string phoneNumber)
        {
            using(var repo = new UserRepository())
            {
                bool doesUserExistAlready = repo.CheckIfUserExists(username);
                bool areCredentialsValid = CheckRegistrationCredentials(email, phoneNumber);

                if (doesUserExistAlready == false && areCredentialsValid == true)
                {
                    User newUser = new User();
                    string hashedPassword = HashPassword(password);

                    newUser.username = username;
                    newUser.password = hashedPassword;
                    newUser.email = email;
                    newUser.contact = phoneNumber;

                    repo.Add(newUser);
                    repo.SaveChanges();

                    SessionManager.SetCurrentUser(newUser);

                    return true;
                }
                else
                {
                    return false;
                }
            }  
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool CheckRegistrationCredentials(string email, string contactNumber)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            string contactNumberPattern = @"^\+([0-9]{1,3})([0-9]{4,14})$";

            if (Regex.IsMatch(email, emailPattern) && Regex.IsMatch(contactNumber, contactNumberPattern))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
