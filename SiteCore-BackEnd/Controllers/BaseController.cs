using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Library.Core.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SiteCore_BackEnd.Common;

namespace SiteCore_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected ILibraryRepository _libraryRepository { get; set; }
        protected IUserRepository _userRepository { get; set; }
        protected IAnalyticRepository _analyticRepository { get; set; }
        protected IAuthService _authService { get; set; }

        public BaseController(IConfiguration config)
        {
            _libraryRepository = new LibraryRepository(config.GetConnectionString("DefaultConnection"));
            _userRepository = new UserRepository(config.GetConnectionString("DefaultConnection"));
            _analyticRepository = new AnalyticRepository(config.GetConnectionString("DefaultConnection"));
            _authService = new AuthService(_userRepository, config);
        }

        protected User authorize(string[] token)
        {
            if (!token[0].Equals("Bearer"))
                throw new HttpException(403, "Not Authorized");

            var user = _authService.Authorize(token[1]);
            if (user == null)
                throw new HttpException(403, "Not Authorized");


            return _authService.Authorize(token[1]);
        }
    }
}