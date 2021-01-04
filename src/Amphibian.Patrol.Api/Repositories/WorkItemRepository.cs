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
    public class WorkItemRepository : IWorkItemRepository
    {
        private readonly DbConnection _connection;

        public WorkItemRepository(DbConnection connection)
        {
            _connection = connection;
        }
        public Task<RecurringWorkItem> GetRecurringWorkItem(int id)
        {
            return _connection.GetAsync<RecurringWorkItem>(id);
        }

        public Task<ShiftRecurringWorkItem> GetShiftRecurringWorkItem(int id)
        {
            return _connection.GetAsync<ShiftRecurringWorkItem>(id);
        }

        public Task<WorkItem> GetWorkItem(int id)
        {
            return _connection.GetAsync<WorkItem>(id);
        }

        public Task<WorkItemAssignment> GetWorkItemAssignment(int id)
        {
            return _connection.GetAsync<WorkItemAssignment>(id);
        }

        public async Task InsertRecurringWorkItem(RecurringWorkItem item)
        {
            int id = (int)await _connection.InsertAsync(item);
            item.Id = id;
        }

        public async Task InsertShiftRecurringWorkItem(ShiftRecurringWorkItem item)
        {
            int id = (int)await _connection.InsertAsync(item);
            item.Id = id;
        }

        public async Task InsertWorkItem(WorkItem item)
        {
            int id = (int)await _connection.InsertAsync(item);
            item.Id = id;
        }

        public async Task InsertWorkItemAssignment(WorkItemAssignment item)
        {
            int id = (int)await _connection.InsertAsync(item);
            item.Id = id;
        }

        public Task UpdateRecurringWorkItem(RecurringWorkItem item)
        {
            return _connection.UpdateAsync(item);
        }

        public Task UpdateShiftRecurringWorkItem(ShiftRecurringWorkItem item)
        {
            return _connection.UpdateAsync(item);
        }

        public Task UpdateWorkItem(WorkItem item)
        {
            return _connection.UpdateAsync(item);
        }

        public Task UpdateWorkItemAssignment(WorkItemAssignment item)
        {
            return _connection.UpdateAsync(item);
        }

        public Task<IEnumerable<WorkItemAssignment>> GetWorkItemAssignments(int workItemId)
        {
            return _connection.SelectAsync<WorkItemAssignment>(x => x.WorkItemId == workItemId);
        }

        public Task<IEnumerable<ShiftRecurringWorkItem>> GetShiftRecurringWorkItems(int recurringWorkItemId)
        {
            return _connection.SelectAsync<ShiftRecurringWorkItem>(x => x.RecurringWorkItemId == recurringWorkItemId);
        }

        public Task DeleteShiftRecurringWorkItem(ShiftRecurringWorkItem shiftRecurringWorkItem)
        {
            return _connection.DeleteAsync(shiftRecurringWorkItem);
        }
        public Task<IEnumerable<WorkItem>> GetWorkItems(int recurringWorkItemId,DateTime after)
        {
            return _connection.QueryAsync<WorkItem>(@"select wi.*
                from workitems wi 
                where wi.recurringworkitemid = @recurringWorkItemId and wi.scheduledat > @after", new { recurringWorkItemId, after });
        }

        public Task DeleteWorkItem(WorkItem item)
        {
            return _connection.DeleteAsync(item);
        }

        public Task DeleteWorkItemAssignment(WorkItemAssignment item)
        {
            return _connection.DeleteAsync(item);
        }

        public Task<IEnumerable<WorkItemAssignment>> GetWorkItemAssignments(int recurringWorkItemId, DateTime after)
        {
            return _connection.QueryAsync<WorkItemAssignment>(@"select distinct wia.*
                from workitems wi 
                inner join workitemassignments wia on wia.workitemid=wi.id
                where wi.recurringworkitemid = @recurringWorkItemId and wi.scheduledat > @after", new { recurringWorkItemId, after });
        }

        public Task<IEnumerable<WorkItem>> GetWorkItemsForShifts(IList<int> scheduledShiftIds)
        {
            return _connection.QueryAsync<WorkItem>(@"select * from workitems where scheduledshiftid in @scheduledShiftIds", new { scheduledShiftIds });
        }

        public Task<IEnumerable<WorkItemAssignment>> GetWorkItemAssignmentsForShifts(IList<int> scheduledShiftIds)
        {
            return _connection.QueryAsync<WorkItemAssignment>(@"select wia.*
                from workitems wi 
                inner join workitemassignments wia on wia.workitemid=wi.id
                where wi.scheduledshiftid in @scheduledShiftIds", new { scheduledShiftIds });
        }

        public Task<IEnumerable<RecurringWorkItem>> GetRecurringWorkItemsForShifts(IList<int> scheduledShiftIds)
        {
            return _connection.QueryAsync<RecurringWorkItem>(@"select distinct rwi.*
                from workitems wi 
                inner join recurringworkitems rwi on rwi.id=wi.recurringworkitemid
                where wi.scheduledshiftid in @scheduledShiftIds", new { scheduledShiftIds });
        }

        public Task<IEnumerable<ShiftRecurringWorkItem>> GetShiftRecurringWorkItemsForShifts(IList<int> scheduledShiftIds)
        {
            return _connection.QueryAsync<ShiftRecurringWorkItem>(@"select distinct srwi.*
                from workitems wi 
                inner join recurringworkitems rwi on rwi.id=wi.recurringworkitemid
                inner join shiftrecurringworkitems srwi on srwi.recurringworkitemid=rwi.id
                where wi.scheduledshiftid in @scheduledShiftIds", new { scheduledShiftIds });
        }

        public Task<IEnumerable<WorkItem>> GetIncompleteWorkItemsForUser(DateTime after, int userId, int patrolId)
        {
            return _connection.QueryAsync<WorkItem>(@"select distinct wi.* --,rwi.*,g.*,u.* 
                from workitems wi
                left join workitemassignments wia on wia.workitemid=wi.id
                --left join recurringworkitems rwi on rwi.id=wi.RecurringWorkItemId
                --left join groups g on g.id=wi.AdminGroupId
                --left join users u on u.id=wi.CreatedByUserId
                where 
                (wia.userid=@userId
                or wi.CompletionMode=@anyCompletionMode)
                and wi.scheduledat>=@after
                and wi.completedAt is null
                and wi.canceledAt is null
                and wi.patrolId=@patrolId
                order by wi.scheduledat", new { after, userId, patrolId, anyCompletionMode = CompletionMode.Any });
        }

        //public Task<IEnumerable<RecurringWorkItem>> GetRecurringWorkItems(int patrolId)
        //{
        //    return _connection.SelectAsync<RecurringWorkItem>(x => x.PatrolId == patrolId);
        //}

        public Task<IEnumerable<RecurringWorkItemDto>> GetRecurringWorkItems(int patrolId)
        {
            var shiftRecurringWorkItems = new Dictionary<int, List<ShiftRecurringWorkItemDto>>();
            return _connection.QueryAsync<RecurringWorkItemDto, ShiftRecurringWorkItemDto, Shift, RecurringWorkItemDto>(@"
            select
                rwi.*,srwi.*,s.*
                from recurringworkitems rwi
                left join shiftrecurringworkitems srwi on srwi.recurringworkitemid=rwi.id
                left join shifts s on s.id=srwi.shiftid
            where rwi.patrolId=@patrolId
            order by rwi.name,rwi.location
            ",
                (rwi, srwi, s) =>
                {
                    if (!shiftRecurringWorkItems.ContainsKey(rwi.Id))
                    {
                        shiftRecurringWorkItems.Add(rwi.Id, new List<ShiftRecurringWorkItemDto>());
                    }

                    srwi.Shift = s;
                    shiftRecurringWorkItems[rwi.Id].Add(srwi);
                    rwi.Shifts = shiftRecurringWorkItems[rwi.Id];

                    return rwi;
                }
                , new { patrolId });
        }

        public async Task<IEnumerable<WorkItemDto>> GetWorkItems(int userId,int? patrolId=null,bool? complete=null, int? completedByUserId = null, 
            int? recurringWorkItemId = null, DateTime? scheduledBefore = null, DateTime? scheduledAfter = null, int? shiftId = null, 
            int? adminGroupId = null, string name = null, string location = null, int? completableByUserId=null,int? workItemId=null, 
            bool deDuplicateRecurring=false, bool excludeIfMoreRecentCompleteRecurring=false)
        {
            var assignments = new Dictionary<int, List<WorkItemAssignmentDto>>();

            var results = await _connection.QueryAsync<WorkItemDto>(@"select distinct 
                wi.*
                ,(
                    case --when wi.createdbyuserid=@userId then 1
                    when wi.completionmode=@anyCompletionMode then 1
                    --when (select top 1 gu2.id from groupusers gu2 where gu2.groupid=wi.admingroupid and gu2.userid=@userId) is not null then 1
                    when (select top 1 wia2.id from workitemassignments wia2 where wia2.workitemid=wi.id and wia2.userid=@userId) is not null then 1
                    else 0
                    end
                 ) CanComplete
                ,(
                    case when wi.createdbyuserid=@userId then 1
                    when (select top 1 gu2.id from groupusers gu2 where gu2.groupid=wi.admingroupid and gu2.userid=@userId) is not null then 1
                    else 0
                    end
                 ) CanAdmin
                ,rwi.*
                ,g.*
                ,u.*
                ,ss.*
                ,s.*
                ,sg.*
                ,wia2.*
                ,wiau.*
                ,cncuser.*
                ,cmpuser.*
                from workitems wi
                left join recurringworkitems rwi on rwi.id=wi.RecurringWorkItemId
                left join ShiftRecurringWorkItems srwi on srwi.RecurringWorkItemId=rwi.id
                left join groups g on g.id=wi.AdminGroupId
                left join users u on u.id=wi.CreatedByUserId
                left join scheduledshifts ss on ss.id=wi.scheduledshiftid
                left join shifts s on s.id=ss.shiftid
                left join groups sg on sg.id=ss.groupid
                left join workitemassignments wia2 on wia2.workitemid=wi.id
                left join users wiau on wiau.id=wia2.userid
                left join users cncuser on cncuser.id=wi.canceledbyuserid
                left join users cmpuser on cmpuser.id=wi.completedbyuserid
                where 
                (@patrolId is null or wi.patrolId=@patrolId)
                and (@completedByUserId is null 
		                or (select top 1 wia.id from workitemassignments wia where wia.workitemid=wi.id and wia.userid=@completedByUserId and wia.CompletedAt is not null) is not null
		                or wi.canceledbyuserid=@completedByUserId)
                and (@recurringWorkItemId is null or wi.RecurringWorkItemId=@recurringWorkItemId)
                and (@scheduledBefore is null or wi.scheduledat < @scheduledBefore)
                and (@scheduledAfter is null or wi.ScheduledAt > @scheduledAfter)
                and (@shiftId is null or @shiftId=srwi.shiftid)
                and (@adminGroupId is null or wi.AdminGroupId = @adminGroupId)
                and (@name is null or wi.name like '%'+@name+'%')
                and (@location is null or wi.location like '%'+@location+'%')
                and (@complete is null or (@complete=1 and (wi.CompletedAt is not null or wi.CanceledAt is not null)) or (@complete=0 and wi.CompletedAt is null and wi.CanceledAt is null))
                and (@completableByUserId is null 
                        or (select top 1 wia.id from workitemassignments wia where wia.workitemid=wi.id and wia.userid=@completableByUserId) is not null
                        or wi.CompletionMode=@anyCompletionMode
                        or (select top 1 gu.id from groupusers gu where gu.groupid=wi.admingroupid and gu.userid=@completableByUserId) is not null
                        or wi.createdbyuserid=@completableByUserId)
                and (@workItemId is null or wi.id=@workItemId)
                and (@excludeIfMoreRecentCompleteRecurring=0 
                        or wi.recurringworkitemid is null 
                        or (wi.recurringworkitemid is not null 
                            and (select id from workitems wi2 where wi2.recurringworkitemid=wi.recurringworkitemid 
                                and wi2.scheduledat>wi.scheduledat 
                                and (wi2.completedat is not null 
                                    or wi2.canceledat is not null)) is null))
                --order by wi.name, wi.location, wi.scheduledat", new Type[] { typeof(WorkItemDto),typeof(RecurringWorkItem), typeof(Group), typeof(UserIdentifier)
                , typeof(ScheduledShiftAssignmentDto), typeof(Shift), typeof(Group), typeof(WorkItemAssignmentDto), typeof(UserIdentifier), typeof(UserIdentifier), typeof(UserIdentifier) },
                (objects) =>
                {
                    var wi = (WorkItemDto)objects[0];
                    var rwi = (RecurringWorkItem)objects[1];
                    var g = (Group)objects[2];
                    var u = (UserIdentifier)objects[3];
                    var ss = (ScheduledShiftAssignmentDto)objects[4];
                    var s = (Shift)objects[5];
                    var sg = (Group)objects[6];
                    var wia = (WorkItemAssignmentDto)objects[7];
                    var wiau = (UserIdentifier)objects[8];
                    var cncu = (UserIdentifier)objects[9];
                    var cmpu = (UserIdentifier)objects[10];

                    wi.RecurringWorkItem = rwi;
                    wi.AdminGroup = g;
                    wi.CreatedBy = u;
                    wi.CanceledBy = cncu;
                    wi.CompletedBy = cmpu;
                    if (ss != null)
                    {
                        wi.ScheduledShift = ss;
                        wi.ScheduledShift.Shift = s;
                        wi.ScheduledShift.Group = sg;
                    }

                    if (!assignments.ContainsKey(wi.Id))
                    {
                        assignments.Add(wi.Id, new List<WorkItemAssignmentDto>());
                    }
                    wia.User = wiau;
                    assignments[wi.Id].Add(wia);
                    wi.Assignments = assignments[wi.Id];

                    return wi;
                } ,
                new { userId,patrolId, complete, completedByUserId, recurringWorkItemId, scheduledBefore, scheduledAfter, shiftId, adminGroupId, name, location, completableByUserId, anyCompletionMode = CompletionMode.Any,workItemId, excludeIfMoreRecentCompleteRecurring });

            results = results.GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.Name).ThenBy(x => x.Location);

            if(deDuplicateRecurring)
            {
                results = results.GroupBy(x => x.RecurringWorkItemId).SelectMany(x =>
                {
                    //if it's a recurring workitem take only the most recent ocurence
                    if(x.Key.HasValue)
                    {
                        return new List<WorkItemDto>() { x.OrderByDescending(y=>y.ScheduledAt).First() }.AsEnumerable();
                    }
                    else
                    {
                        return x.AsEnumerable();
                    }
                });
            }

            return results;
        }
    }
}
