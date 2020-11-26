using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Infrastructure
{
    public interface IUnitOfWork
    {
        Task Begin();
        Task Commit();
        Task Rollback();
    }
}
