using Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Repository
{
    public interface ILibraryRepository
    {
        (IEnumerable<Books> books,int total) GetBooks(int filter = 0, int pageNumber = 1, int pageSize = 0, string sortBy = "Title");
        Books GetBookById(int id);
        void AddBook(Books book);
        void EditBook(Books book);
        void AddCategory(Category category);
        IEnumerable<Author> GetAuthors();
        Author GetAuthorById(int id);
        void AddAuthor(Author author);
        void Rent(string username, int bookId);
        void Return(string username, int booksId);
    }
}
