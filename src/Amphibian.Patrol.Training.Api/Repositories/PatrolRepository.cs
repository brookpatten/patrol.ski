using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

using Dapper;
using Dommel;

using Amphibian.Patrol.Training.Api.Models;
using System.Data.Common;
using Amphibian.Patrol.Training.Api.Dtos;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public class PatrolRepository : IPatrolRepository
    {
        private readonly DbConnection _connection;

        public PatrolRepository(DbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Amphibian.Patrol.Training.Api.Models.Patrol> GetPatrol(int id)
        {
            return await _connection.GetAsync<Amphibian.Patrol.Training.Api.Models.Patrol>(id)
                .ConfigureAwait(false);
        }

        public async Task InsertPatrol(Amphibian.Patrol.Training.Api.Models.Patrol patrol)
        {
            patrol.Id = (int)await _connection.InsertAsync(patrol)
                .ConfigureAwait(false);
        }

        public async Task UpdatePatrol(Amphibian.Patrol.Training.Api.Models.Patrol patrol)
        {
            await _connection.UpdateAsync(patrol)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<PatrolUser>> GetPatrolUsersForPatrol(int patrolId)
        {
            return await _connection.SelectAsync<PatrolUser>(x => x.PatrolId == patrolId)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetUsersForPatrol(int patrolId)
        {
            return await _connection.QueryAsync<User>(
                @$"select 
                    u.id
                    ,u.email
                    ,u.firstname
                    ,u.lastname
                    --intentionally don't fill the password fields, doesn't make sense in this context
                    from patrolusers pu
                    inner join users u on u.id=pu.userid
                   where 
                    pu.patrolId=@patrolId", new { patrolId })
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<CurrentUserPatrolDto>> GetPatrolsForUser(int userId)
        {
            return await _connection.QueryAsync<CurrentUserPatrolDto>(
                @$"select 
                    p.id
                    ,p.name
                    ,pu.role
                    from patrolusers pu
                    inner join patrols p on p.id=pu.patrolid
                   where 
                    pu.userid=@userId", new { userId })
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<PatrolUser>> GetPatrolUsersForUser(int userId)
        {
            return await _connection.SelectAsync<PatrolUser>(x => x.UserId == userId)
                .ConfigureAwait(false);
        }

        public async Task<PatrolUser> GetPatrolUser(int userId, int patrolId)
        {
            var pus = await _connection.SelectAsync<PatrolUser>(x => x.UserId == userId && x.PatrolId == patrolId)
                .ConfigureAwait(false);
            if(pus.Any())
            {
                return pus.First();
            }
            else
            {
                return null;
            }
        }

        public async Task<PatrolUser> GetPatrolUser(int id)
        {
            return await _connection.GetAsync<PatrolUser>(id)
                .ConfigureAwait(false);
        }

        public async Task InsertPatrolUser(PatrolUser patrolUser)
        {
            patrolUser.Id = (int)await _connection.InsertAsync(patrolUser)
                .ConfigureAwait(false);
        }

        public async Task DeletePatrolUser(int patrolId, int userId)
        {
            await _connection.DeleteMultipleAsync<PatrolUser>(x => x.PatrolId == patrolId && x.UserId == userId)
                .ConfigureAwait(false);
        }

        public async Task UpdatePatrolUser(PatrolUser patrolUser)
        {
            await _connection.UpdateAsync(patrolUser)
                .ConfigureAwait(false);
        }
    }
}
