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
using Amphibian.Patrol.Api.Extensions;

namespace Amphibian.Patrol.Api.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly DbConnection _connection;
        private readonly IMapper _mapper;

        public AssignmentRepository(DbConnection connection, IMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AssignmentHeaderDto>> GetAssignmentsForUser(int userId)
        {
            return await _connection.QueryAsync<AssignmentHeaderDto>(
                @"select 
                    a.Id
                    ,a.planid
                    ,a.userid
                    ,u.firstname userfirstname
                    ,u.lastname userlastname
                    ,a.assignedat
                    ,a.dueat
                    ,a.completedat
                    ,p2.name planname
                    ,(
                        select count(p.id)
	                    from plans p 
	                    inner join plansections ps on ps.planid=p.id
	                    inner join sectionskills ss on ss.sectionid=ps.sectionid
	                    inner join sectionlevels sl on sl.sectionid=ps.sectionid
	                    where p.id=a.planid
                    ) signaturesrequired
                    ,(
                        select count(id) from signatures s where s.assignmentid=a.id
                    ) signatures
                    from assignments a
                    inner join plans p2 on p2.id=a.planid
                    inner join users u on u.id=a.userid
                    where a.userid=@userId", new { userId }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AssignmentHeaderDto>> GetAssignmentsForPlan(int planId)
        {
            return await _connection.QueryAsync<AssignmentHeaderDto>(
                @"select 
                    a.Id
                    ,a.planid
                    ,a.userid
                    ,u.firstname userfirstname
                    ,u.lastname userlastname
                    ,a.assignedat
                    ,p.name planname
                    ,a.dueat
                    ,a.completedat
                    ,(
                        select count(p.id)
	                    from plans p 
	                    inner join plansections ps on ps.planid=p.id
	                    inner join sectionskills ss on ss.sectionid=ps.sectionid
	                    inner join sectionlevels sl on sl.sectionid=ps.sectionid
	                    where p.id=a.planid
                    ) signaturesrequired
                    ,(
                        select count(id) from signatures s where s.assignmentid=a.id
                    ) signatures
                    from assignments a
                    inner join users u on u.id=a.userid
                    inner join plans p on p.id=a.planid
                    where a.planid=@planId", new { planId }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AssignmentHeaderDto>> GetAssignments(int patrolId, int? planId, int? assignedToUserId, bool? complete)
        {
            return await _connection.QueryAsync<AssignmentHeaderDto>(
                @"select 
                    a.Id
                    ,a.planid
                    ,a.userid
                    ,u.firstname userfirstname
                    ,u.lastname userlastname
                    ,a.assignedat
                    ,p.name planname
                    ,a.dueat
                    ,a.completedat
                    ,(
                        select count(p.id)
	                    from plans p 
	                    inner join plansections ps on ps.planid=p.id
	                    inner join sectionskills ss on ss.sectionid=ps.sectionid
	                    inner join sectionlevels sl on sl.sectionid=ps.sectionid
	                    where p.id=a.planid
                    ) signaturesrequired
                    ,(
                        select count(id) from signatures s where s.assignmentid=a.id
                    ) signatures
                    from assignments a
                    inner join users u on u.id=a.userid
                    inner join plans p on p.id=a.planid
                    where (@planId is null or a.planid=@planId)
                    and p.patrolid=@patrolId
                    and (@assignedToUserId is null or a.userid=@assignedToUserId)
                    and (@complete is null or (a.completedat is not null and @complete=1) or (a.completedat is null and @complete=0))
                 order by u.lastname,p.name
                ", new { patrolId,planId, assignedToUserId, complete }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AssignmentHeaderDto>> GetIncompleteAssignments(int patrolId,int userId)
        {
            return await _connection.QueryAsync<AssignmentHeaderDto>(
                @"select 
                    a.Id
                    ,a.planid
                    ,a.userid
                    ,u.firstname userfirstname
                    ,u.lastname userlastname
                    ,a.assignedat
                    ,p.name planname
                    ,a.dueat
                    ,a.completedat
                    ,(
                        select count(p.id)
	                    from plans p 
	                    inner join plansections ps on ps.planid=p.id
	                    inner join sectionskills ss on ss.sectionid=ps.sectionid
	                    inner join sectionlevels sl on sl.sectionid=ps.sectionid
	                    where p.id=a.planid
                    ) signaturesrequired
                    ,(
                        select count(id) from signatures s where s.assignmentid=a.id
                    ) signatures
                    from assignments a
                    inner join users u on u.id=a.userid
                    inner join plans p on p.patrolid=@patrolId and p.id=a.planid
                    where a.completedat is null
                    and (
                        case when exists(
                        select * from plans p
                        inner join plansections ps on ps.planid=p.id
                        inner join sections s on s.id=ps.SectionId
                        inner join sectiongroups sg on sg.SectionId=s.id
                        inner join groups g on g.id=sg.GroupId
                        inner join groupusers gu on gu.GroupId=g.id
                        inner join users u on u.id=gu.UserId
                        where p.PatrolId=@patrolId and u.id=@userId
                        ) then 1 else 0 end
                    ) = 1
                    order by u.lastname,p.name
                    ", new { patrolId,userId }).ConfigureAwait(false);
        }

        public Task<IEnumerable<AssignmentHeaderDto>> GetIncompleteAssignments(int patrolId)
        {
            return _connection.QueryAsync<AssignmentHeaderDto>(
                @"select 
                    a.Id
                    ,a.planid
                    ,a.userid
                    ,u.firstname userfirstname
                    ,u.lastname userlastname
                    ,a.assignedat
                    ,p.name planname
                    ,a.dueat
                    ,a.completedat
                    ,(
                        select count(p.id)
	                    from plans p 
	                    inner join plansections ps on ps.planid=p.id
	                    inner join sectionskills ss on ss.sectionid=ps.sectionid
	                    inner join sectionlevels sl on sl.sectionid=ps.sectionid
	                    where p.id=a.planid
                    ) signaturesrequired
                    ,(
                        select count(id) from signatures s where s.assignmentid=a.id
                    ) signatures
                    from assignments a
                    inner join users u on u.id=a.userid
                    inner join plans p on p.patrolid=@patrolId
                    where a.completedat is null
                    order by u.lastname,p.name
                    ", new { patrolId });
        }

        public Task<Assignment> GetAssignment(int assignmentId)
        {
            return _connection.GetAsync<Assignment>(assignmentId);
        }

        public Task<IEnumerable<Signature>> GetSignaturesForAssignment(int assignmentId)
        {
            return _connection.SelectAsync<Signature>(x => x.AssignmentId == assignmentId);
        }

        public Task<IEnumerable<SignatureDto>> GetSignaturesWithUsersForAssignment(int assignmentId)
        {
            return _connection.QueryAsync<Signature,UserIdentifier,SignatureDto>(
                @"select 
                    s.id
                    ,s.assignmentid
                    ,s.sectionskillid
                    ,s.sectionlevelid
                    ,s.signedat
                    ,s.signedbyuserid
                    ,u.id
                    ,u.email
                    ,u.firstname
                    ,u.lastname
                from 
                    signatures s
                inner join users u on u.id=s.signedbyuserid
                where 
                    assignmentid=@assignmentId",(s,u)=>
                {
                    var dto = _mapper.Map<Signature, SignatureDto>(s);
                    dto.SignedBy = u;
                    return dto;
                },splitOn: "id",param:new { assignmentId });
        }

        public async Task InsertSignature(Signature signature)
        {
            signature.Id=(int)await _connection.InsertAsync(signature).ConfigureAwait(false).ToInt32();
        }

        public async Task UpdateAssignment(Assignment assignment)
        {
            await _connection.UpdateAsync(assignment);
        }

        public async Task InsertAssignment(Assignment assignment)
        {
            assignment.Id = (int)await _connection.InsertAsync(assignment).ConfigureAwait(false).ToInt32();
        }

        public async Task<IEnumerable<AssignmentCountByPlanByDayDto>> GetAssignmentCountsByDay(int patrolId, DateTime start, DateTime end)
        {
            var rows = await _connection.QueryAsync<AssignmentCountByDayDto>(@"
            CREATE TABLE #days (
                startsAt datetime,
                endsAt datetime
            );

            declare @DatePeriod datetime = @start
            while (@DatePeriod <= @end)
            begin
                insert into #days (startsAt,endsAt) values (@DatePeriod,dateadd(millisecond,999,dateadd(second,59,dateadd(minute,59,dateadd(hour,23,@DatePeriod)))));
                set @DatePeriod = DATEADD(day, 1,@DatePeriod);
            end

            select 
            d.startsAt day,
            p.id planid,
            p.name planname,
            (select top 1 color from sections s inner join plansections ps on ps.planid=p.id and ps.sectionid=s.id order by s.id asc) plancolor,
            count(a.id) as OpenAssignmentCount
            from #days d
            left join plans p on p.patrolid=@patrolId
            left join assignments a on 
            a.planid=p.id
            and a.AssignedAt <= d.startsAt
            and (a.CompletedAt is null or a.CompletedAt >= d.endsAt)
            group by d.startsAt,p.id,p.name
            order by d.startsAt,p.name

            drop table #days;", new { patrolId, start, end });

            var resultGroups = rows.GroupBy(x => x.PlanId).Select(x => new AssignmentCountByPlanByDayDto() { PlanId = x.First().PlanId, PlanName = x.First().PlanName, PlanColor = x.First().PlanColor, CountsByDay = x.ToList() });
            return resultGroups;
        }

        public async Task<IEnumerable<AssignmentProgressByDayDto>> GetAssignmentProgressByDay(int patrolId, DateTime start, DateTime end, int? planId, int? userId)
        {
            var rows = await _connection.QueryAsync<AssignmentProgressByDayDto>(@"
            CREATE TABLE #days (
                startsAt datetime,
                endsAt datetime
            );

            declare @DatePeriod datetime = @start
            while (@DatePeriod <= @end)
            begin
                insert into #days (startsAt,endsAt) values (@DatePeriod,dateadd(hour,24,@DatePeriod));
                set @DatePeriod = DATEADD(day, 1,@DatePeriod)
            end
            ;
            with _plan_signature_counts (id,signatures) as
            (
	            select 
	            p.id,
	            count(p.id)
	            from plans p 
	            inner join plansections ps on ps.planid=p.id
	            inner join sectionskills ss on ss.sectionid=ps.sectionid
	            inner join sectionlevels sl on sl.sectionid=ps.sectionid
	            where p.patrolid=@patrolid
	            group by p.id
            )
            select 
            d.startsAt day,
            p.id planId,
            p.name planName,
            psc.signatures requiredsignatures,
            a.id assignmentid,
            u.lastname userlastname,
            u.firstname userfirstname,
            u.email useremail,
            a.completedat,
            (select count(s.id) from signatures s where s.assignmentid=a.id and s.SignedAt<=d.endsAt) completedsignatures
            from #days d
            left join plans p on 
	            p.patrolid=@patrolId
	            and (@planId is null or @planId=p.id)
            left join assignments a on 
	            a.planid=p.id
	            and a.assignedat < @end
	            and (a.CompletedAt is null or a.CompletedAt > @start)
	            and (@userId is null or @userId=a.userid)
            left join users u on u.id=a.userid
            left join _plan_signature_counts psc on psc.id=p.id
            order by d.startsAt,p.name,u.lastname

            drop table #days;

            ", new { patrolId, start,end, planId, userId});
            return rows;
        }
    }
}
