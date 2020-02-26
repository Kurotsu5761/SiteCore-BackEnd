using Library.Core.Helper;
using Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        public List<User> GetUsers()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select * from [User]";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable userTable = dbSet.Tables["Table"];
                if (userTable != null && userTable.Rows.Count != 0)
                {
                    var users = userTable.AsEnumerable().Select(_ => new User
                    {
                        UserId = (int)_["UserId"],
                        EmailAddress = _["EmailAddress"].ToString(),
                        IsAdmin = (bool)_["IsAdmin"]
                    }).ToList();
                    return users;
                }
                else
                {
                    return null;
                }
            }
        }

        public User GetUserByUsername(string username)
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select * from [User] where EmailAddress = @Username";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "Username", Value = username, SqlDbType = SqlDbType.NVarChar });

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable userTable = dbSet.Tables["Table"];
                if(userTable != null && userTable.Rows.Count != 0)
                {
                    var user = userTable.AsEnumerable().Select(_ => new User
                    {
                        UserId = (int)_["UserId"],
                        EmailAddress = _["EmailAddress"].ToString(),
                        IsAdmin = (bool)_["IsAdmin"]
                    }).FirstOrDefault();
                    return user;
                }else
                {
                    return null;
                }
            }
        }
        public void Register(string username)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string getUser = @"Select * from [User] where EmailAddress =  @Username";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = getUser;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "Username", Value = username, SqlDbType = SqlDbType.NVarChar });
                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable userTable = dbSet.Tables["Table"];
                if (userTable != null && userTable.Rows.Count != 0)
                {
                    throw new Exception("User already Exits");
                }
                

                string command = @"Insert into [User] (EmailAddress, IsAdmin) values (@Username, @IsAdmin)";
               
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "IsAdmin", Value = false, SqlDbType = SqlDbType.Bit });

                sqlCommand.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void AddAdmin(int userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Update [User] set IsAdmin = 1 where UserId = @UserId";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "UserId", Value = userId, SqlDbType = SqlDbType.NVarChar });

                sqlCommand.ExecuteNonQuery();
                conn.Close();
            }
        }


        public List<Books> GetUserHistory(int userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select b.*, c.Name as CategoryName, c.Id as CategoryId, t.UserId as UserId, t.TimeOccured as DateRented from 
                                        Books b inner join Category c on b.CategoryId = c.Id
                                        full outer join LibraryTransaction t on t.BookId = b.Id
                                        where b.Id is not null 
                                        and t.status = 0
                                        and t.UserId = @UserId";

                command += " order by t.TimeOccured Desc";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.AddWithValue("UserId", userId);

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable booksTable = dbSet.Tables["Table"];
                int total = booksTable.Rows.Count;


                if (booksTable != null && booksTable.Rows.Count > 0)
                {

                    var books = booksTable.AsEnumerable().Select(_ => new Books
                    {
                        Id = (int)_["Id"],
                        Title = _["Title"].ToString(),
                        Subtitle = _["Subtitle"].ToString(),
                        BookStatus = getBookStatus((int)_["BookStatus"]),
                        ImageUrl = _["ImageUrl"].ToString(),
                        CurrentUser = new User { UserId = _["UserId"] == DBNull.Value ? 0 : (int)_["UserId"] },
                        DateRented = checkDate(_.IsNull("DateRented"), _["DateRented"]),
                        Category = new Category { Id = (int)_["CategoryId"], Name = _["CategoryName"].ToString() }
                    }).ToList();


                    //Get the Authors
                    List<int> Ids = booksTable.AsEnumerable().Select(_ => (int)_["Id"]).ToList();
                    string nextCommand = @"Select a.*, p.BookId from Author a inner join Publishes p on a.id = p.AuthorId where p.BookId in ({BookIds})";
                    sqlCommand.CommandText = nextCommand;
                    sqlCommand.AddArrayParameters("BookIds", Ids);


                    SqlDataAdapter nexAdapter = new SqlDataAdapter(sqlCommand);
                    dbSet = new DataSet();
                    nexAdapter.Fill(dbSet);
                    DataTable authorTable = dbSet.Tables["Table"];

                    if (authorTable != null && authorTable.Rows.Count > 0)
                    {
                        var authors = authorTable.AsEnumerable().Select(_ =>
                        {
                            return ((int)_["BookId"], new Author
                            {
                                Name = _["Name"].ToString(),
                                Id = (int)_["Id"],
                                ImageUrl = _["ImageUrl"].ToString()
                            });
                        }).ToList();
                        foreach (Books book in books)
                        {
                            book.Authors = authors.Where(_ => _.Item1 == book.Id)
                                .Select(_ => _.Item2).ToList();
                        }
                    }

                    return books;
                }
                else
                {
                    return new List<Books>();
                }
            }
        }

        #region HelperFunction
        private string getBookStatus(int bookStatus)
        {
            switch (bookStatus)
            {
                case 1:
                    return "Rented";
                case 0:
                default:
                    return "Available";
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
        #endregion
    }

}
