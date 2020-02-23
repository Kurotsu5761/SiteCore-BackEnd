using Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Repository
{
    public interface ILibraryRepository
    {
        IEnumerable<Books> GetBooks(int filter = 0, int pageNumber = 1, int pageSize = 0, string sortBy = "Title");
        void AddBook(Books book);
        void EditBook(Books book);
        IEnumerable<Author> GetAuthors();
        void AddAuthor(Author author);
        void Rent(string username, List<int> bookIds);
        void Return(string username, List<int> booksIds);
    }
}
