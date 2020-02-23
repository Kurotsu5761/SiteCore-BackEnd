using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public List<BookModel> GetBooks(BookFilter filter, int page = 1, int pageSize = 10, string sortBy = "Title")
        {
            try
            {
                //calls to AzureFunctions
                _libraryRepository.GetBooks((int)filter, page, pageSize, sortBy);
                return null;
            }
            catch (Exception ex)
            {
                //log exception
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("book/{id}")]
        public BookModel GetBookById(int id)
        {
            try
            {
                return null;

            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("book")]
        public void AddBook(BookModel book)
        {

        }

        [HttpPut("book/{id}")]
        public void EditBook(BookModel book, int id)
        {

        }
        
        [HttpGet("authors")]
        public List<AuthorModel> GetAuthors()
        {
            return null;
        }

        [HttpGet("authors/{id}")]
        public AuthorModel GetAuthorById(int id)
        {
            return null;
        }

        [HttpPost("author")]
        public void AddAuthor(AuthorModel author)
        {

        }

        [HttpPost("rent")]
        public void Rent(string username, List<int> bookIds)
        {

        }

        [HttpPost("return")]
        public void Return(string username, List<int> bookIds)
        {

        }
    }
}