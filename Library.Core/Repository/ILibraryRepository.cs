using Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Repository
{
    public interface ILibraryRepository
    {
        (List<Books> books,int total) GetBooks(int userId = 0, int filter = 0, int pageNumber = 1, int pageSize = 0, string sortBy = "Title");
        Books GetBookById(int id);
        void AddBook(Books book);
        void EditBook(Books book);
        void AddCategory(Category category);
        List<Category> GetCategories();
        List<Author> GetAuthors();
        Author GetAuthorById(int id);
        void AddAuthor(Author author);
        void UpdateBookStatus(int bookId, int status);
        void CreateTransaction(int userId, int booksId);
        void UpdateTransaction(int id);
        int GetRentedByBookId(int userId, int bookId);
    }
}
