using Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Repository
{
    public interface IUserRepository
    {
        User GetUserByUsername(string username);
        void Register(string username);
        List<Books> GetUserBooks();
        List<Books> GetUserHistory();
    }
}
