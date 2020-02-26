using Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        public List<Analytic> GetAnalytics()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select u.EmailAddress, u.UserId, b.Title, t.TimeOccured as DateRented, c.Id as CategoryId
                    , c.Name as CategoryName from [User] u 
                inner join LibraryTransaction t on u.UserId =  t.UserId 
                inner join Books b on b.Id = t.BookId
                inner join Category c on c.Id = b.CategoryId";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable analyticTable = dbSet.Tables["Table"];
                return Transform(analyticTable);
            }
        }

        public List<Analytic> GetAnalyticByCategory(int categoryId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select u.EmailAddress, u.UserId, b.Title, t.TimeOccured as DateRented, c.Id as CategoryId
                    , c.Name as CategoryName from [User] u 
                inner join LibraryTransaction t on u.UserId =  t.UserId 
                inner join Books b on b.Id = t.BookId
                inner join Category c on c.Id = b.CategoryId
                where c.Id = @CategoryId";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "CategoryId", Value = categoryId, SqlDbType = SqlDbType.Int });

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable analyticTable = dbSet.Tables["Table"];
                return Transform(analyticTable);
            }
        }
        public List<Analytic> GetAnalyticByAuthor(int authorId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select u.EmailAddress, u.UserId, b.Title, t.TimeOccured as DateRented, 
                    c.Id as CategoryId, c.Name as CategoryName from [User] u 
                inner join LibraryTransaction t on u.UserId =  t.UserId 
                inner join Books b on b.Id = t.BookId
                inner join Category c on c.Id = b.CategoryId
                inner join Publishes p on p.BookId = b.Id
                where p.AuthorId = @AuthorId";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "AuthorId", Value = authorId, SqlDbType = SqlDbType.Int });

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable analyticTable = dbSet.Tables["Table"];
                return Transform(analyticTable);
            }
        }
        public List<Analytic> GetAnalyticByUser(int userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select u.EmailAddress, u.UserId, b.Title, t.TimeOccured as DateRented, c.Id as CategoryId, c.Name as CategoryName from [User] u 
                inner join LibraryTransaction t on u.UserId =  t.UserId 
                inner join Books b on b.Id = t.BookId
                inner join Category c on c.Id = b.CategoryId
                where t.UserId = @UserId";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "UserId", Value = userId , SqlDbType = SqlDbType.Int });

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable analyticTable = dbSet.Tables["Table"];
                return Transform(analyticTable);
            }
        }

        private DateTime? checkDate(bool isNull, object date)
        {
            if (isNull)
            {
                return null;
            }
            else
            {
                return (DateTime)date;
            }
        }

        private List<Analytic> Transform (DataTable dt)
        {
            if (dt != null && dt.Rows.Count != 0)
            {
                var analytics = dt.AsEnumerable().Select(_ => new Analytic
                {
                    BookTitle = _["Title"].ToString(),
                    CategoryId = (int)_["CategoryId"],
                    CategoryName = _["CategoryName"].ToString(),
                    DateRented = checkDate(_.IsNull("DateRented"), _["DateRented"]),
                    UserId = (int)_["UserId"],
                    UserEmail = _["EmailAddress"].ToString()
                }).ToList();
                return analytics;
            }
            else
            {
                return new List<Analytic>();
            }
        }
    }
}
