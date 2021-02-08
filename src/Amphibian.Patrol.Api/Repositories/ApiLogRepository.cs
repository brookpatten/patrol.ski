using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

using Dapper;
using Dommel;
using AutoMapper;

using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Dtos;
using System.Data.Common;

namespace Amphibian.Patrol.Api.Repositories
{
    public class ApiLogRepository : IApiLogRepository
    {
        private readonly DbConnection _connection;
        public ApiLogRepository(DbConnection connection)
        {
            _connection = connection;
        }

        public Task<IEnumerable<RouteMetrics>> GetRouteMetrics(DateTime? from, DateTime? to, int? userId, string route=null)
        {
            return _connection.QueryAsync<RouteMetrics>(@"
                select
                al.route
                ,min(al.durationms) minms
                ,avg(al.durationms) avgms
                ,max(al.durationms) maxms
                ,sum(al.durationms) summs
                from apilogs al
                where
                (@from is null or @from<=al.StartedAt)
                and (@to is null or al.StartedAt <= @to)
                and (@userId is null or al.userId=@userId)
                and (@route is null or al.route like @route+'%')
                group by route
                order by avg(al.durationms) desc"
                , new { from, to, userId, route });
        }

        public Task<IEnumerable<ApiLogDto>> SearchApiLogs(DateTime? from, DateTime? to, int? userId, string route)
        {
            return _connection.QueryAsync<ApiLogDto, UserIdentifier, ApiLogDto>(@"
                select 
                al.*
                ,u.id
                ,u.email
                ,u.firstname
                ,u.lastname
                ,u.nspnumber
                from apilogs al
                left join users u on u.id=al.userid
                where
                (@from is null or @from<=al.StartedAt)
                and (@to is null or al.StartedAt <= @to)
                and (@userId is null or al.userId=@userId)
                and (@route is null or al.route like @route + '%')
                order by al.StartedAt asc"
                , (a, u) =>
                {
                    a.User = u;
                    return a;
                }, new { from, to, userId, route });
        }

        public async Task InsertApiLog(ApiLog log)
        {
            log.Id = (int)await _connection.InsertAsync(log);
        }

        public async Task UpdateApiLog(ApiLog log)
        {
            await _connection.UpdateAsync(log);
        }
    }
}
