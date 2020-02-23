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
        public IEnumerable<Books> GetBooks(int filter = 0 , int pageNumber = 1, int pageSize = 0, string sortBy = "Title")
        {

            int startIndex = (pageNumber - 1) * pageSize;
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string command = @"Select * from Books order by " + sortBy;
                command += " Offset " + startIndex + " rows";
                command += " fetch next " + pageSize + " rows only";
                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = command;
                sqlCommand.CommandType = CommandType.Text;

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataSet dbtable = new DataSet();
                adapter.Fill(dbtable);

                DataTable dt = dbtable.Tables["Table"];

                if(dt != null && dt.Rows.Count > 0) {
                    List<int> Ids = dt.AsEnumerable().Select(_ => (int)_["Id"]).ToList();
                    string nextCommand = @"Select a.* from Authors a inner join Publishes p on a.id = p.AuthorId where p.BooksId in ({BookIds})";
                    sqlCommand.AddArrayParameters("BookIds", Ids);

                    sqlCommand.CommandText = nextCommand;
                    SqlDataAdapter nexAdapter = new SqlDataAdapter(sqlCommand);
                    dbtable = new DataSet();
                    adapter.Fill(dbtable);
                } else {
                    return null;
                }
                return null;
            }
        }

        public void AddBook(Books book)
        {

        }

        public void EditBook(Books book)
        {
           
        }

        #endregion

        #region author
        public IEnumerable<Author> GetAuthors()
        {
            return null;
        }

        public void AddAuthor(Author author)
        {

        }

        #endregion

        #region process

        public void Rent(string username, List<int> bookIds)
        {

        }


        public void Return(string username, List<int> booksIds)
        {

        }
        #endregion
    }
}
