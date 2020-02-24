using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Repository
{
    public class AnalyticRepository : IAnalyticRepository
    {
        private readonly string _connectionString;

        public AnalyticRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
