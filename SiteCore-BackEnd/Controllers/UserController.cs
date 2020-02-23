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
    public class UserController : BaseController
    {
        public UserController(IConfiguration config) : base(config) { }

        [HttpGet("login")]
        public string login(string username)
        {
            return null;
        }
        
        [HttpGet("books")]
        public List<BookModel> GetUserBooks(string username)
        {
            return null;
        }

        [HttpGet("{userId}/history")]
        public List<BookModel> GetHitory(string username)
        {
            return null;
        }
    }
}