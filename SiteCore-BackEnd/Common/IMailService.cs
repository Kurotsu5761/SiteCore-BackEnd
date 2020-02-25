using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteCore_BackEnd.Common
{
    public interface IMailService
    {
        void Send (string userEmail,string title, string messages);
    }
}
