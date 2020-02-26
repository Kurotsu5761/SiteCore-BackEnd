using Library.Core.Helper;
using Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Library.Core.Repository
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly string _connectionString;

        public LibraryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        #region books
        public (List<Books> books, int total) GetBooks(int userId = 0, int filter = 0 , int pageNumber = 1, int pageSize = 0, string sortBy = "Title")
        {

            int startIndex = (pageNumber - 1) * pageSize;
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select b.*, c.Name as CategoryName, c.Id as CategoryId, t.UserId as UserId, t.TimeOccured as DateRented from 
                                        Books b inner join Category c on b.CategoryId = c.Id
                                        full outer join LibraryTransaction t on t.BookId = b.Id and t.Status = 1
                                        where b.Id is not null";
                if (filter == 1)
                    command += " and b.BookStatus = 0";
                if (filter == 2 && userId != 0)
                    command += " and t.UserId = @UserId";
                                        

                command += " order by " + sortBy;
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.AddWithValue("UserId", userId);

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable booksTable = dbSet.Tables["Table"];
                int total = booksTable.Rows.Count;
                

                if(booksTable != null && booksTable.Rows.Count > 0) {

                    var books = booksTable.AsEnumerable().Select(_ => new Books
                    {
                        Id = (int)_["Id"],
                        Title = _["Title"].ToString(),
                        Subtitle = _["Subtitle"].ToString(),
                        BookStatus = getBookStatus((int)_["BookStatus"]),
                        ImageUrl = _["ImageUrl"].ToString(),
                        CurrentUser = new User { UserId = _["UserId"] == DBNull.Value ? 0 : (int)_["UserId"] },
                        DateRented = checkDate(_.IsNull("DateRented"), _["DateRented"]),
                        Category = new Category { Id = (int)_["CategoryId"], Name = _["CategoryName"].ToString()}
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

                    if(authorTable != null && authorTable.Rows.Count > 0)
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
                        foreach ( Books book in books)
                        {
                            book.Authors = authors.Where(_ =>_.Item1 == book.Id)
                                .Select(_ => _.Item2).ToList();
                        }
                    }

                    return (books, total);
                } else {
                    return (new List<Books>(), 0);
                }
            }
        }
        
        public Books GetBookById(int id)
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                string command = @"Select b.*, c.Name as CategoryName, c.Id as CategoryId, t.UserId as UserId, t.TimeOccured as DateRented from 
                                        Books b inner join Category c on b.CategoryId = c.Id
                                        full outer join LibraryTransaction t on t.BookId = b.Id and t.status = 1
                                        where b.Id = @id";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.AddWithValue("id", id);

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable booksTable = dbSet.Tables["Table"];
                int total = booksTable.Rows.Count;

                if (booksTable != null && booksTable.Rows.Count > 0)
                {

                    var book = booksTable.AsEnumerable().Select(_ => new Books
                    {
                        Id = (int)_["Id"],
                        Title = _["Title"].ToString(),
                        Subtitle = _["Subtitle"].ToString(),
                        BookStatus = getBookStatus((int)_["BookStatus"]),
                        ImageUrl = _["ImageUrl"].ToString(),
                        CurrentUser = new User { UserId = _["UserId"] == DBNull.Value ? 0 : (int)_["UserId"] },
                        DateRented = checkDate(_.IsNull("DateRented"), _["DateRented"]),
                        Category = new Category { Id = (int)_["CategoryId"], Name = _["CategoryName"].ToString() }
                    }).FirstOrDefault();
                   
                    string nextCommand = @"Select a.*, p.BookId from Author a inner join Publishes p on a.id = p.AuthorId where p.BookId = @BookId";
                    sqlCommand.CommandText = nextCommand;
                    sqlCommand.Parameters.AddWithValue("BookId", book.Id);


                    SqlDataAdapter nexAdapter = new SqlDataAdapter(sqlCommand);
                    dbSet = new DataSet();
                    nexAdapter.Fill(dbSet);
                    DataTable authorTable = dbSet.Tables["Table"];       

                    if (authorTable != null && authorTable.Rows.Count > 0)
                    {
                        book.Authors = authorTable.AsEnumerable().Select(_ =>
                        {
                            return new Author
                            {
                                Name = _["Name"].ToString(),
                                Id = (int)_["Id"],
                                ImageUrl = _["ImageUrl"].ToString()
                            };
                        }).ToList();
                    }

                    return book;
                }
                return null;
            }
        }

        public void AddBook(Books book)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string command = @"Insert into Books (Title, Subtitle, ImageUrl, BookStatus, CategoryId) 
                                    values (@Title, @Subtitle, @ImageUrl, @BookStatus, @CategoryId);
                                    Select cast(scope_identity() as int)";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "Title", Value = book.Title, SqlDbType = SqlDbType.NVarChar });
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "Subtitle", Value = book.Subtitle, SqlDbType = SqlDbType.NVarChar });
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "ImageUrl", Value = book.ImageUrl, SqlDbType = SqlDbType.NVarChar });
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "BookStatus", Value = 0, SqlDbType = SqlDbType.Int });
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "CategoryId", Value = book.Category.Id, SqlDbType = SqlDbType.Int });

                var bookId = sqlCommand.ExecuteScalar();

                command = @"Insert into Publishes (BookId, AuthorId) values (@BookId, @AuthorId)";
                sqlCommand.CommandText = command;
                
                foreach (var author in book.Authors)
                {
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "BookId", Value = bookId, SqlDbType = SqlDbType.Int });
                    sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "AuthorId", Value = author.Id, SqlDbType = SqlDbType.Int });
                    sqlCommand.ExecuteNonQuery();
                }
               
                conn.Close();
            }
        }

        //TODO: Implement EditBook
        public void EditBook(Books book)
        {
            throw new NotImplementedException();
        }

        public void AddCategory(Category category)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string command = @"Insert into Category (Name) values (@Name) ";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "Name", Value = category.Name, SqlDbType = SqlDbType.NVarChar });

                sqlCommand.ExecuteNonQuery();
                conn.Close();

            }
        }

        public List<Category> GetCategories()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select * from Category";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable categoryTable = dbSet.Tables["Table"];
                if (categoryTable != null && categoryTable.Rows.Count != 0)
                {
                    var categories = categoryTable.AsEnumerable().Select(_ => new Category
                    {
                        Id = (int)_["Id"],
                        Name = _["Name"].ToString()
                    }).ToList();
                    return categories;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region author
        public List<Author> GetAuthors()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select * from Author";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable authorTable = dbSet.Tables["Table"];
                if (authorTable != null && authorTable.Rows.Count != 0)
                {
                    var authors = authorTable.AsEnumerable().Select(_ => new Author
                    {
                        Id = (int)_["Id"],
                        Name = _["Name"].ToString(),
                        ImageUrl = _["ImageUrl"].ToString(),
                    }).ToList();
                    return authors;
                }
                else
                {
                    return null;
                }
            }
        }

        //TODO: Implement GetAuthorById
        public Author GetAuthorById(int id)
        {
            throw new NotImplementedException();
        }

        public void AddAuthor(Author author)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Insert into Author (Name, ImageUrl) values (@Name, @ImageUrl)";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "Name", Value = author.Name, SqlDbType = SqlDbType.NVarChar });
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "ImageUrl", Value = author.ImageUrl, SqlDbType = SqlDbType.NVarChar });

                sqlCommand.ExecuteNonQuery();
                conn.Close();

            }
        }

        #endregion

        #region process

        public void UpdateBookStatus(int bookId, int status)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Update Books set BookStatus = @Status where Id = @BookId";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "BookId", Value = bookId, SqlDbType = SqlDbType.Int });
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "Status", Value = status, SqlDbType = SqlDbType.Int });

                sqlCommand.ExecuteNonQuery();
                conn.Close();
            }
        }


        public void CreateTransaction(int userId, int bookId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string command = @"Insert into LibraryTransaction (UserId, BookId, TimeOccured, Status) values (@UserId, @BookId, @TimeOccured, @Status) ";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "BookId", Value = bookId, SqlDbType = SqlDbType.Int });
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "UserId", Value = userId, SqlDbType = SqlDbType.Int });
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "TimeOccured", Value = DateTime.Now, SqlDbType = SqlDbType.DateTime });
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "Status", Value = 1, SqlDbType = SqlDbType.Int });


                sqlCommand.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void UpdateTransaction(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Update LibraryTransaction set Status = @Status where Id = @Id";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "Id", Value = id, SqlDbType = SqlDbType.Int });
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "Status", Value = 0, SqlDbType = SqlDbType.Int });

                sqlCommand.ExecuteNonQuery();
                conn.Close();
            }
        }

        public int GetRentedByBookId(int userId, int bookId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select * from LibraryTransaction where UserId = @UserId and BookId = @BookId and status = 1";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "UserId", Value = userId, SqlDbType = SqlDbType.Int });
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "BookId", Value = bookId, SqlDbType = SqlDbType.Int });

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbSet = new DataSet();
                adapter.Fill(dbSet);

                DataTable transactions = dbSet.Tables["Table"];
                if (transactions != null && transactions.Rows.Count != 0)
                {
                    var transaction = transactions.AsEnumerable().Select(_ => (int)_["Id"]).FirstOrDefault();
                    return transaction;
                }
                else
                {
                    return 0;
                }
            }
        }


        #endregion

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
