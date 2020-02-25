using Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Repository
{
    public interface IAnalyticRepository
    {
        List<Analytic> GetAnalyticByCategory(int categoryId);
        List<Analytic> GetAnalyticByAuthor(int authorId);
        List<Analytic> GetAnalyticByUser(int userId);
    }
}
