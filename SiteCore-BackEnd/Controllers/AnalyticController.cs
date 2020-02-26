using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Library.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace SiteCore_BackEnd.Controllers
{
    [Route("api/analytics")]
    [ApiController]
    public class AnalyticController : BaseController
    {
        public AnalyticController(IConfiguration config) : base(config) { }

        [HttpGet]
        public List<Analytic> GetAnalytics(AnalyticFilter by, int id)
        {   
            var token = this.Request.Headers["Authorization"].ToString().Split(" ");

            try
            {
                var user = authorize(token);
                if (!user.IsAdmin)
                    throw new HttpException(403, "Not Authorized");

                switch (by)
                {
                    case AnalyticFilter.Category:
                        return _analyticRepository.GetAnalyticByCategory(id);
                    case AnalyticFilter.Author:
                        return _analyticRepository.GetAnalyticByAuthor(id);
                    case AnalyticFilter.User:
                        return _analyticRepository.GetAnalyticByUser(id);
                    default:
                        return _analyticRepository.GetAnalytics();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}