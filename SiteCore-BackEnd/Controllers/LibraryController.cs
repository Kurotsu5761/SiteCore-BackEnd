using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Library.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SiteCore_BackEnd.Models;

namespace SiteCore_BackEnd.Controllers
{
    [Route("api/library")]
    [ApiController]
    public class LibraryController : BaseController
    {
        public LibraryController(IConfiguration config) : base(config) { }

        [HttpGet("books")]
        public PaginatedBook GetBooks(BookFilter filter = BookFilter.All, int page = 1, int pageSize = 10, string sortBy = "Title")
        {       
            try
            {
                int userId = 0;
                if(filter == BookFilter.Own)
                {
                    var token = this.Request.Headers["Authorization"].ToString().Split(" ");

                    var user = authorize(token);
                    userId = user.UserId;
                }
                var paginatedBooks = _libraryRepository.GetBooks(userId, (int)filter, page, pageSize, sortBy);
                var books = paginatedBooks.books;
                return new PaginatedBook( books, paginatedBooks.total);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("book/{id}")]
        public Books GetBookById(int id)
        {
            try
            {
                return _libraryRepository.GetBookById(id);   
            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("book")]
        public void AddBook(Books book)
        {
            var token = this.Request.Headers["Authorization"].ToString().Split(" ");

            try
            {
                var user = authorize(token);
                if (!user.IsAdmin)
                    throw new HttpException(403, "Not Authorized");

                _libraryRepository.AddBook(book);

            }
            catch
            {
                throw;
            }
        }

        [HttpPut("book/{id}")]
        public void EditBook(Books book)
        {
            var token = this.Request.Headers["Authorization"].ToString().Split(" ");

            try
            {
                var user = authorize(token);
                if (!user.IsAdmin)
                    throw new HttpException(403, "Not Authorized");

                _libraryRepository.EditBook(book);

            }
            catch
            {
                throw;
            }
        }
        
        [HttpGet("authors")]
        public List<Author> GetAuthors()
        {
            try
            {
                return _libraryRepository.GetAuthors();
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("authors/{id}")]
        public Author GetAuthorById(int id)
        {
            try{
                var author = _libraryRepository.GetAuthorById(id);
                return author;
            }
            catch
            {
                throw;
            }
            
        }

        [HttpPost("author")]
        public void AddAuthor(Author author)
        {
            if (string.IsNullOrEmpty(author.Name))
                throw new HttpException(400, "Bad Request");

            try
            {
                _libraryRepository.AddAuthor(author);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("category")]
        public void AddCategory(Category category)
        {
            if (string.IsNullOrEmpty(category.Name))
                throw new HttpException(400, "Bad Request");

            try
            {
                _libraryRepository.AddCategory(category);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("categories")]
        public List<Category> GetCategories()
        {
            try
            {
                return _libraryRepository.GetCategories();
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("rent")]
        public void Rent(int bookId)
        {
            var token = this.Request.Headers["Authorization"].ToString().Split(" ");

            try
            {
                var user = authorize(token);
                _libraryRepository.UpdateBookStatus(bookId, 1);
                _libraryRepository.CreateTransaction(user.UserId, bookId);

                var books = _libraryRepository.GetBooks(user.UserId, 2, 1, int.MaxValue).books;
                List<string> bookTitle = books.Select(_ => _.Title).ToList();
                _mailService.Send(user.EmailAddress, "Rent A Book", 
                    string.Format("You currently hold the following books from our library: {0}", string.Join(",", bookTitle))
                );
            }
            catch
            {
                throw;
            }


        }

        [HttpPost("return")]
        public void Return(int bookId)
        {
            var token = this.Request.Headers["Authorization"].ToString().Split(" ");
            
           
            try
            {
                var user = authorize(token);
                //TODO: Check if current user is the one holding the book
                var transactionId = _libraryRepository.GetRentedByBookId(user.UserId, bookId);
                if(transactionId != 0)
                {
                    _libraryRepository.UpdateTransaction(transactionId);
                    _libraryRepository.UpdateBookStatus(bookId, 0);
                }

                var books = _libraryRepository.GetBooks(user.UserId, 2, 1, int.MaxValue).books;
                string message;
                if (books != null || books.Count != 0)
                {
                    List<string> bookTitle = books.Select(_ => _.Title).ToList();
                    message = string.Format("You still hold the following books from our library: {0}", string.Join(",", bookTitle));
                    
                }
                else
                {
                    message = "Thank you, you had return all the books.";
                }
                _mailService.Send(user.EmailAddress, "Return A Book",message
                    );

            }
            catch
            {
                throw;
            }
            
        }
    }
}