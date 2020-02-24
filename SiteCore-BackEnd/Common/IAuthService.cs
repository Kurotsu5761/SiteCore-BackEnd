using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteCore_BackEnd.Common
{
    public interface IAuthService
    {
        User Authenticate(string username);
        User Authorize(string token);
    }
}
