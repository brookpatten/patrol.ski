using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using Dommel;

using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Dtos;
using System.Data.Common;

namespace Amphibian.Patrol.Api.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly DbConnection _connection;

        public EventRepository(DbConnection connection)
        {
            _connection = connection;
        }
        public Task<IEnumerable<Event>> GetEvents(int patrolId, DateTime from, DateTime to)
        {
            return _connection.QueryAsync<Event>(@"
            select
            *
            from events
            where patrolid=@patrolId
            and endsat >= @from
            and startsat <= @to
            order by startsat asc
            ",new { patrolId, from,to });
        }

        /// <summary>
        /// returns the next 5 events + the next two weeks of events
        /// </summary>
        /// <param name="patrolId"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        public Task<IEnumerable<Event>> GetUpcomingEvents(int patrolId, DateTime now)
        {
            return _connection.QueryAsync<Event>(@"
            declare @twoweeks datetime;
            set @twoweeks = dateadd(day,14,@now);
            select * from
            (
	            select * 
	            from events e
	            where 
		            startsat > @now 
		            and startsat < @twoweeks 
		            and patrolid=@patrolId
            union
	            select top 5 * 
	            from events 
	            where 
		            startsat > @now 
		            and patrolid=@patrolId 
	            order by startsat asc
            ) results
            order by startsat asc
            ", new { patrolId, now});
        }

        public async Task InsertEvent(Event patrolEvent)
        {
            var id = (int)await _connection.InsertAsync(patrolEvent).ConfigureAwait(false);
            patrolEvent.Id = id;
        }

        public Task UpdateEvent(Event patrolEvent)
        {
            return _connection.UpdateAsync(patrolEvent);
        }

        public Task<Event> GetById(int id)
        {
            return _connection.GetAsync<Event>(id);
        }

        public Task Delete(Event patrolEvent)
        {
            return _connection.DeleteAsync<Event>(patrolEvent);
        }
    }
}
