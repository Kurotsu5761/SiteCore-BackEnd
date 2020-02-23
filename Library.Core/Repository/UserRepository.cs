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
    }
}
