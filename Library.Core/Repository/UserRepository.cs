using Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User GetUserByUsername(string username)
        {
            return null;
        }
        public void Register(string username)
        {
            
        }

        public List<Books> GetUserBooks()
        {
            return null;
        }

        public List<Books> GetUserHistory()
        {
            return null;
        }

    }
}
