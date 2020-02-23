using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Core.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace SiteCore_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected ILibraryRepository _libraryRepository { get; set; }
        protected IUserRepository _userRepository { get; set; }
        public BaseController(IConfiguration config)
        {
            _libraryRepository = new LibraryRepository(config.GetConnectionString("DefaultConnection"));
            _userRepository = new UserRepository(config.GetConnectionString("DefaultConnection"));
        }
    }
}