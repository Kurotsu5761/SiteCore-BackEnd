using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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

        [HttpGet, Route("login")]
        public User Login(string username)
        {
            var user = _authService.Authenticate(username); 
            if(user == null)
            {
                throw new HttpException(404, "User not found");
            }
            return user;
        }

        [HttpPost, Route("register")]
        public User Register(string username)
        {
            _userRepository.Register(username);
            return _authService.Authenticate(username);
        }
        
        [HttpGet("books")]
        public List<BookModel> GetUserBooks()
        {
            var token = this.Request.Headers["Authorization"].ToString().Split(" ");
            if (token[0].Equals("Bearer"))
                throw new HttpException(403, "Not Authorized");

            var user = _authService.Authorize(token[1]);

            if (user == null)
                throw new HttpException(403, "Not Authorized");
            
            return null;
        }

        [HttpGet("history")]
        public List<BookModel> GetHitory()
        {
            var token = this.Request.Headers["Authorization"].ToString().Split(" ");
            if (token[0].Equals("Bearer"))
                throw new HttpException(403, "Not Authorized");

            var user = _authService.Authorize(token[1]);

            if (user == null)
                throw new HttpException(403, "Not Authorized");



            return null;
        }
    }
}