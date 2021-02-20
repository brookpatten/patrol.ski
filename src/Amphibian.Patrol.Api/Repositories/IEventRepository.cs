using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetEvents(int patrolId, DateTime from,DateTime to,bool isInternal,bool isPublic);
        Task<IEnumerable<Event>> GetUpcomingEvents(int patrolId, DateTime now, bool isInternal, bool isPublic);
        Task InsertEvent(Event patrolEvent);
        Task UpdateEvent(Event patrolEvent);
        Task<Event> GetById(int id);
        Task Delete(Event patrolEvent);
    }
}
