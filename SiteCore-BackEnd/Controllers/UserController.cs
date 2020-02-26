using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Library.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SiteCore_BackEnd.Common;
using SiteCore_BackEnd.Models;

namespace SiteCore_BackEnd.Controllers
{
    [Route("api/me")]
    [ApiController]
    public class UserController : BaseController
    {
        public UserController(IConfiguration config) : base(config) {
            
        }

        [HttpGet, Route("/api/login")]
        public User Login(string username)
        {
            var user = _authService.Authenticate(username); 
            if(user == null)
            {
                throw new HttpException(404, "User not found");
            }
            return user;
        }

        [HttpPost, Route("/api/register")]
        public User Register(string username)
        {
            _userRepository.Register(username);
            return _authService.Authenticate(username);
        }

        [HttpPut, Route("/users/make-admin")]
        public void MakeAdmin(int userId)
        {
            _userRepository.AddAdmin(userId);
        }
        
        [HttpGet("books")]
        public List<Books> GetUserBooks()
        {
            var token = this.Request.Headers["Authorization"].ToString().Split(" ");

            try
            {
                var user = authorize(token);
                var books = _libraryRepository.GetBooks(user.UserId, 2, 1, int.MaxValue).books;
                return books;
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("history")]
        public List<Books> GetHitory()
        {
            var token = this.Request.Headers["Authorization"].ToString().Split(" ");

            try
            {
                var user = authorize(token);
                return _userRepository.GetUserHistory(user.UserId);
            }
            catch
            {
                throw;
            }
        }
    }
}