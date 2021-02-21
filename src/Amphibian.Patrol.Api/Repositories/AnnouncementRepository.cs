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
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly DbConnection _connection;

        public AnnouncementRepository(DbConnection connection)
        {
            _connection = connection;
        }
        public Task<IEnumerable<Announcement>> GetAnnouncements(int patrolId, DateTime? now, bool isInternal, bool isPublic)
        {
            return _connection.QueryAsync<Announcement>(@"
            select
            id,
            patrolid,
            createdbyuserid, 
            subject,
            announcementmarkdown,
            announcementhtml,
            createdat,
            postat,
            expireat
            from announcements
            where patrolid=@patrolId
            and (@now is null or @now > postat)
            and (@now is null or expireat is null or @now < expireat)
            and (
                (@isInternal = 1 and isInternal = 1)
                or
                (@isPublic = 1 and isPublic=1)
            )
            order by createdat desc
            ", new { patrolId, now, isInternal, isPublic });
        }

        public async Task InsertAnnouncement(Announcement announcement)
        {
            var id = (int)await _connection.InsertAsync(announcement).ConfigureAwait(false);
            announcement.Id = id;
        }

        public Task UpdateAnnouncement(Announcement announcement)
        {
            return _connection.UpdateAsync(announcement);
        }

        public Task<Announcement> GetById(int id)
        {
            return _connection.GetAsync<Announcement>(id);
        }
    }
}
