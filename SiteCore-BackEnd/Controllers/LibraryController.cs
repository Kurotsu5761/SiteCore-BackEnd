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
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : BaseController
    {
        public LibraryController(IConfiguration config) : base(config) { }

        [HttpGet("books")]
        public PaginatedBook GetBooks(BookFilter filter, int page = 1, int pageSize = 10, string sortBy = "Title")
        {
            try
            {
                //calls to AzureFunctions
                var paginatedBooks = _libraryRepository.GetBooks((int)filter, page, pageSize, sortBy);
                var books = paginatedBooks.books.ToList();
                return new PaginatedBook( books, paginatedBooks.total);
            }
            catch (Exception ex)
            {
                //log exception
                throw new Exception(ex.Message);
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
        public void AddBook(BookModel book)
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
        public void EditBook(BookModel book)
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
            return null;
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
        public void AddAuthor(AuthorModel author)
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

        [HttpPost("rent")]
        public void Rent(int bookId)
        {
            var token = this.Request.Headers["Authorization"].ToString().Split(" ");

            var user = authorize(token);
            try
            {
                _libraryRepository.Rent(user.EmailAddress, bookId);

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
            
            var user = authorize(token);
            try
            {
                _libraryRepository.Return(user.EmailAddress, bookId);

            }catch
            {
                throw;
            }
            
        }


        private User authorize(string[] token)
        {
            if (token[0].Equals("Bearer"))
                throw new HttpException(403, "Not Authorized");

            var user = _authService.Authorize(token[1]);
            if( user == null)
                throw new HttpException(403, "Not Authorized");


            return _authService.Authorize(token[1]);
        }
    }
}