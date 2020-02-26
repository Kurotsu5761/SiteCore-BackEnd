using Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Repository
{
    public interface IUserRepository
    {
        List<User> GetUsers();
        User GetUserByUsername(string username);
        void Register(string username);
        void AddAdmin(int userId);
        List<Books> GetUserHistory(int userId);
    }
}
