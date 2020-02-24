using Library.Core.Helper;
using Library.Core.Models;
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
        public (IEnumerable<Books> books, int total) GetBooks(int filter = 0 , int pageNumber = 1, int pageSize = 0, string sortBy = "Title")
        {

            int startIndex = (pageNumber - 1) * pageSize;
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select b.*, c.Name as CategoryName, c.Id as CategoryId, t.UserId as UserId from 
                                        Books b, Category c, Transaction t
                                        where b.categoryId = c.Id and t.BookId = b.Id
                                        order by " + sortBy;
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;

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
                        CurrentUser = new User { UserId = (int)_["UserId"] }
                    }).ToList();


                    //Get the Authors
                    List<int> Ids = booksTable.AsEnumerable().Select(_ => (int)_["Id"]).ToList();
                    string nextCommand = @"Select a.*, p.BookId from Authors a inner join Publishes p on a.id = p.AuthorId where p.BooksId in ({BookIds})";
                    sqlCommand.AddArrayParameters("BookIds", Ids);

                    sqlCommand.CommandText = nextCommand;
                    SqlDataAdapter nexAdapter = new SqlDataAdapter(sqlCommand);
                    dbSet = new DataSet();
                    adapter.Fill(dbSet);
                    DataTable authorTable = dbSet.Tables["Table"];

                    var authors = authorTable.AsEnumerable().Select(_ =>
                    {
                        return ((int)_["BookId"], new Author
                        {
                            Name = _["Name"].ToString(),
                            Id = (int)_["Id"],
                            ImageUrl = _["ImageUrl"].ToString()
                        });
                    }).ToList();

                    if(authorTable != null && authorTable.Rows.Count > 0)
                    {
                        foreach( Books book in books)
                        {
                            book.Authors = authors.Select(_ =>
                            {
                                if (_.Item1 == book.Id)
                                    return _.Item2;
                                return null;
                            }).ToList();
                        }
                    }

                    return (books, total);
                } else {
                    return (null, 0);
                }
            }
        }
        
        public Books GetBookById(int id)
        {
            return null;
        }

        public void AddBook(Books book)
        {

        }

        public void EditBook(Books book)
        {
           
        }

        public void AddCategory(Category category)
        {

        }

        #endregion

        #region author
        public IEnumerable<Author> GetAuthors()
        {
            return null;
        }

        public Author GetAuthorById(int id)
        {
            return null;
        }

        public void AddAuthor(Author author)
        {

        }

        #endregion

        #region process

        public void Rent(string username, int bookId)
        {

        }


        public void Return(string username, int bookId)
        {

        }

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
        #endregion
    }
}
