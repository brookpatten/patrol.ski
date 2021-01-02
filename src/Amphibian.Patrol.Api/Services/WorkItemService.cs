using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;
using AutoMapper;
using Ganss.XSS;
using Markdig;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public class WorkItemService:IWorkItemService
    {
        private ILogger<WorkItemService> _logger;
        private IPatrolRepository _patrolRepository;
        private IGroupRepository _groupRepository;
        private IShiftRepository _shiftRepository;
        private ISystemClock _clock;
        private IEmailService _emailService;
        private IUserRepository _userRepository;
        private IWorkItemRepository _workItemRepository;
        private IMapper _mapper;

        public WorkItemService(ILogger<WorkItemService> logger, IPatrolRepository patrolRepository,
            IGroupRepository groupRepository, IShiftRepository shiftRepository, ISystemClock clock, IEmailService emailService, 
            IUserRepository userRepository, IWorkItemRepository workItemRepository,IMapper mapper)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _groupRepository = groupRepository;
            _shiftRepository = shiftRepository;
            _clock = clock;
            _emailService = emailService;
            _userRepository = userRepository;
            _workItemRepository = workItemRepository;
            _mapper = mapper;
        }

        public async Task CompleteWorkItem(int workItemId, int userId, string workNotes)
        {
            var workItem = await _workItemRepository.GetWorkItem(workItemId);
            var assignments = await _workItemRepository.GetWorkItemAssignments(workItemId);

            if(workItem.CompletedAt.HasValue)
            {
                return;
            }

            if(workItem.CompletionMode == CompletionMode.AdminOnly)
            {
                await this.CompleteWorkItemAdminOnly(workItem, assignments,userId,workNotes);
            }
            else if(workItem.CompletionMode == CompletionMode.AllAssigned)
            {
                await this.CompleteWorkItemAllAssigned(workItem, assignments, userId, workNotes);
            }
            else if(workItem.CompletionMode == CompletionMode.Any)
            {
                await this.CompleteWorkItemAny(workItem, assignments, userId, workNotes);
            }
            else if(workItem.CompletionMode == CompletionMode.AnyAssigned)
            {
                await this.CompleteWorkItemAnyAssigned(workItem, assignments, userId, workNotes);
            }
        }

        private async Task CompleteWorkItemAdminOnly(WorkItem item,IEnumerable<WorkItemAssignment> assignments, int userId, string workNotes)
        {
            IEnumerable<GroupUser> groupUsers = null;
            if(item.AdminGroupId.HasValue)
            {
                groupUsers = await _groupRepository.GetGroupUsersForGroup(item.AdminGroupId.Value);
            }

            if(item.CreatedByUserId == userId || (groupUsers!=null && groupUsers.Any(x=>x.UserId==userId)))
            {
                var assignment = assignments.SingleOrDefault(x => x.UserId == userId);
                if(assignment==null)
                {
                    assignment = new WorkItemAssignment()
                    {
                        UserId = userId,
                        WorkItemId = item.Id
                    };
                }

                var now = _clock.UtcNow.UtcDateTime;
                assignment.CompletedAt = now;
                assignment.WorkNotes = workNotes;

                if(assignment.Id==default(int))
                {
                    await _workItemRepository.InsertWorkItemAssignment(assignment);
                }
                else
                {
                    await _workItemRepository.UpdateWorkItemAssignment(assignment);
                }

                item.CompletedAt = now;
                await _workItemRepository.UpdateWorkItem(item);
            }
            else
            {
                throw new ApplicationException("Specified user is not allowed to complete"); 
            }
        }

        private async Task CompleteWorkItemAllAssigned(WorkItem item, IEnumerable<WorkItemAssignment> assignments, int userId, string workNotes)
        {
            var assignment = assignments.SingleOrDefault(x => x.UserId == userId);
            if(assignment!=null && !assignment.CompletedAt.HasValue)
            {
                var now = _clock.UtcNow.UtcDateTime;
                assignment.CompletedAt = now;
                assignment.WorkNotes = workNotes;
                await _workItemRepository.UpdateWorkItemAssignment(assignment);

                if(assignments.All(x=>x.CompletedAt.HasValue))
                {
                    item.CompletedAt = now;
                    await _workItemRepository.UpdateWorkItem(item);
                }
            }
            else
            {
                throw new ApplicationException("Unable to complete");
            }
        }

        private async Task CompleteWorkItemAny(WorkItem item, IEnumerable<WorkItemAssignment> assignments, int userId, string workNotes)
        {
            var assignment = assignments.SingleOrDefault(x => x.UserId == userId);
            if(assignment==null)
            {
                assignment = new WorkItemAssignment()
                {
                    UserId = userId,
                    WorkItemId = item.Id
                };
            }

            var now = _clock.UtcNow.UtcDateTime;
            assignment.CompletedAt = now;
            assignment.WorkNotes = workNotes;

            if(assignment.Id==default(int))
            {
                await _workItemRepository.InsertWorkItemAssignment(assignment);
            }
            else
            {
                await _workItemRepository.UpdateWorkItemAssignment(assignment);
            }

            item.CompletedAt = now;
            await _workItemRepository.UpdateWorkItem(item);
        }
        private async Task CompleteWorkItemAnyAssigned(WorkItem item, IEnumerable<WorkItemAssignment> assignments, int userId, string workNotes)
        {
            var assignment = assignments.SingleOrDefault(x => x.UserId == userId);
            if (assignment != null && !assignment.CompletedAt.HasValue)
            {
                var now = _clock.UtcNow.UtcDateTime;
                assignment.CompletedAt = now;
                assignment.WorkNotes = workNotes;
                await _workItemRepository.UpdateWorkItemAssignment(assignment);

                item.CompletedAt = now;
                await _workItemRepository.UpdateWorkItem(item);
            }
            else
            {
                throw new ApplicationException("Unable to complete");
            }
        }

        public async Task SaveRecurringWorkItem(RecurringWorkItemDto recurringWorkItem, int userId, bool populateWorkItems = true, bool populateWorkItemAssignments=true,DateTime? overrideNow=null)
        {
            var now = overrideNow ?? _clock.UtcNow.UtcDateTime;
            var patrol = await _patrolRepository.GetPatrol(recurringWorkItem.PatrolId);

            //save the recurring work item
            RecurringWorkItem item = null;
            List<ShiftRecurringWorkItem> shifts = new List<ShiftRecurringWorkItem>();
            if(recurringWorkItem.Id!=default(int))
            {
                item = await _workItemRepository.GetRecurringWorkItem(recurringWorkItem.Id);
                shifts = (await _workItemRepository.GetShiftRecurringWorkItems(recurringWorkItem.Id)).ToList();
            }
            else
            {
                item = new RecurringWorkItem();
                item.PatrolId = recurringWorkItem.PatrolId;
                item.CreatedAt = now;
                item.CreatedByUserId = userId;
            }

            item.AdminGroupId = recurringWorkItem.AdminGroupId;
            item.CompletionMode = recurringWorkItem.CompletionMode;
            item.DescriptionMarkup = SanitizeHtml(recurringWorkItem.DescriptionMarkup);
            item.Location = recurringWorkItem.Location;
            item.Name = recurringWorkItem.Name;

            if (recurringWorkItem.Shifts != null && recurringWorkItem.Shifts.Any())
            {
                item.MaximumRandomCount = recurringWorkItem.MaximumRandomCount;
                item.RecurEnd = null;
                item.RecurIntervalSeconds = null;
                item.RecurStart = null;
            }
            else
            {
                item.MaximumRandomCount = null;
                item.RecurEnd = recurringWorkItem.RecurEnd;
                item.RecurIntervalSeconds = recurringWorkItem.RecurIntervalSeconds;
                item.RecurStart = recurringWorkItem.RecurStart;
            }

            if (item.Id==default(int))
            {
                await _workItemRepository.InsertRecurringWorkItem(item);
                recurringWorkItem.Id = item.Id;
            }
            else
            {
                await _workItemRepository.UpdateRecurringWorkItem(item);
            }

            if (recurringWorkItem.Shifts != null && recurringWorkItem.Shifts.Any())
            {
                await shifts.DifferenceWith(recurringWorkItem.Shifts,
                    (wi, dto) => wi.Id == dto.Id,
                    dto => _workItemRepository.InsertShiftRecurringWorkItem(new ShiftRecurringWorkItem() { RecurringWorkItemId = item.Id, ScheduledAtHour = dto.ScheduledAtHour, ScheduledAtMinute = dto.ScheduledAtMinute, ShiftId = dto.ShiftId, ShiftAssignmentMode = dto.ShiftAssignmentMode }),
                    wi => _workItemRepository.DeleteShiftRecurringWorkItem(wi),
                    (wi, dto) =>
                    {
                        wi.ScheduledAtHour = dto.ScheduledAtHour;
                        wi.ScheduledAtMinute = dto.ScheduledAtMinute;
                        wi.ShiftId = dto.ShiftId;
                        wi.ShiftAssignmentMode = dto.ShiftAssignmentMode;
                        return _workItemRepository.UpdateShiftRecurringWorkItem(wi);
                    }
                    );
            }
            else
            {
                foreach (var shift in shifts)
                {
                    await _workItemRepository.DeleteShiftRecurringWorkItem(shift);
                }
            }

            if (populateWorkItems)
            {
                //save shift recurring work items
                if (recurringWorkItem.Shifts != null && recurringWorkItem.Shifts.Any())
                {
                    await this.PopulateShiftWorkItemOccurences(recurringWorkItem, now, userId, patrol,populateWorkItemAssignments);
                }
                else if (recurringWorkItem.RecurStart.HasValue && recurringWorkItem.RecurEnd.HasValue && recurringWorkItem.RecurIntervalSeconds.HasValue)
                {
                    await this.PopulateTimedWorkItemOccurences(recurringWorkItem, userId, recurringWorkItem.NextOccurenceUsers != null ? recurringWorkItem.NextOccurenceUsers.Select(x => x.Id).ToList() : null,now,populateWorkItemAssignments);
                }
                else
                {
                    throw new InvalidOperationException("Invalid recurrence specification");
                }
            }
        }

        public async Task PopulateShiftWorkItemOccurences(RecurringWorkItemDto recurringWorkItem,DateTime now, int userId,Models.Patrol patrol, bool populateWorkitemAssignments=true)
        {
            var shifts = await _workItemRepository.GetShiftRecurringWorkItems(recurringWorkItem.Id);
            var workItems = await _workItemRepository.GetWorkItems(recurringWorkItem.Id, now);
            
            var allScheduledShiftAssignments = new List<ScheduledShiftAssignmentDto>();
            
            foreach(var shift in shifts)
            {
                var scheduledShiftAssignments = await _shiftRepository.GetScheduledShiftAssignments(recurringWorkItem.PatrolId, from: now, shiftId: shift.ShiftId);
                allScheduledShiftAssignments.AddRange(scheduledShiftAssignments);

                var scheduledShifts = scheduledShiftAssignments.GroupBy(x => x.ScheduledShiftId);

                foreach(var scheduledShift in scheduledShifts)
                {
                    var shiftWorkItems = workItems.Where(x => x.ScheduledShiftId == scheduledShift.Key);
                    var shiftStartLocal = scheduledShift.First().StartsAt.UtcToPatrolLocal(patrol);

                    if(!shiftWorkItems.Any())
                    {
                        //create the work item
                        var workItem = new WorkItem()
                        {
                            AdminGroupId = recurringWorkItem.AdminGroupId,
                            CreatedAt = now,
                            CreatedByUserId = userId,
                            CompletionMode = recurringWorkItem.CompletionMode,
                            DescriptionMarkup = recurringWorkItem.DescriptionMarkup,
                            Location = recurringWorkItem.Location,
                            Name = recurringWorkItem.Name,
                            PatrolId = recurringWorkItem.PatrolId,
                            RecurringWorkItemId = recurringWorkItem.Id,
                            ScheduledShiftId = scheduledShift.Key,
                            ScheduledAt = new DateTime(shiftStartLocal.Year, shiftStartLocal.Month, shiftStartLocal.Day, shift.ScheduledAtHour, shift.ScheduledAtMinute, 0).UtcFromPatrolLocal(patrol)
                        };
                        await _workItemRepository.InsertWorkItem(workItem);
                    }
                    else
                    {
                        //realistically should only ever be 1
                        foreach(var workItem in shiftWorkItems)
                        {
                            if (!workItem.CompletedAt.HasValue && !workItem.CanceledAt.HasValue)
                            {
                                workItem.CompletionMode = recurringWorkItem.CompletionMode;
                                workItem.DescriptionMarkup = recurringWorkItem.DescriptionMarkup;
                                workItem.Location = recurringWorkItem.Location;
                                workItem.Name = recurringWorkItem.Name;
                                workItem.ScheduledAt = new DateTime(shiftStartLocal.Year, shiftStartLocal.Month, shiftStartLocal.Day, shift.ScheduledAtHour, shift.ScheduledAtMinute, 0).UtcFromPatrolLocal(patrol);
                                workItem.AdminGroupId = recurringWorkItem.AdminGroupId;
                                await _workItemRepository.UpdateWorkItem(workItem);
                            }
                        }
                    }
                }
            }

            if (populateWorkitemAssignments)
            {
                await RecalculateShiftWorkItemAssignments(allScheduledShiftAssignments);
            }
        }

        public async Task RecalculateShiftWorkItemAssignments(List<ScheduledShiftAssignmentDto> scheduledShiftAssignments)
        {
            var scheduledShiftsForRecalc = scheduledShiftAssignments
                .GroupBy(x => x.Id)
                .Select(x => x.First())
                .GroupBy(x => new { x.ScheduledShiftId, x.StartsAt, x.EndsAt })
                .OrderBy(x => x.Key.StartsAt);

            var scheduledShiftIds = scheduledShiftsForRecalc.Select(x => x.Key.ScheduledShiftId).Distinct().ToList();

            var allWorkItems = (await _workItemRepository.GetWorkItemsForShifts(scheduledShiftIds)).ToList();
            var allWorkItemAssignments = (await _workItemRepository.GetWorkItemAssignmentsForShifts(scheduledShiftIds)).ToList();
            var allRecurringWorkItems = (await _workItemRepository.GetRecurringWorkItemsForShifts(scheduledShiftIds)).ToList();
            var allShiftRecurringWorkItems = (await _workItemRepository.GetShiftRecurringWorkItemsForShifts(scheduledShiftIds)).ToList();

            foreach (var scheduledShift in scheduledShiftsForRecalc)
            {
                await RecalculateSingleShiftWorkItemAssignments(scheduledShift.Key.ScheduledShiftId,scheduledShift.ToList(), allWorkItems, allWorkItemAssignments, allRecurringWorkItems, allShiftRecurringWorkItems);
            }
        }

        private async Task RecalculateSingleShiftWorkItemAssignments(int scheduledShiftId,List<ScheduledShiftAssignmentDto> scheduledShiftAssignments,List<WorkItem> workItems, List<WorkItemAssignment> assignments, List<RecurringWorkItem> recurringWorkItems, List<ShiftRecurringWorkItem> shiftRecurringWorkItems)
        {
            var random = new Random();
            //find work items for shift
            var scheduledShiftWorkItems = workItems.Where(x => x.ScheduledShiftId == scheduledShiftId).ToList();
            //find auto assigned work items
            var autoShiftWorkItems = scheduledShiftWorkItems//.Where(wi => shiftRecurringWorkItems.Single(r => r.RecurringWorkItemId == wi.RecurringWorkItemId.Value).ShiftAssignmentMode == ShiftAssignmentMode.Random)
                .Select(x=>new 
                { 
                    WorkItem=x,
                    RecurringWorkItem=recurringWorkItems.Single(y=>y.Id==x.RecurringWorkItemId.Value),
                    Assignments = assignments.Where(z=>z.WorkItemId==x.Id).ToList(),
                    ShiftRecurringWorkItem = shiftRecurringWorkItems.First(y=>y.RecurringWorkItemId == x.RecurringWorkItemId)
                })
                .ToList();

            //dictionary of user ids with assignments
            var shiftAssignments = scheduledShiftAssignments.Where(x => x.ScheduledShiftId == scheduledShiftId && x.AssignedUser!=null)
                .Select(x => x.AssignedUser.Id).Distinct().Select(x => new { Id = x, Assignments = assignments.Where(y => y.UserId == x).ToList() })
                .ToDictionary(x => x.Id, x => x.Assignments.ToList());
                
                //assignments.Where(x => scheduledShiftWorkItems.Any(y => y.Id == x.WorkItemId)).GroupBy(x => new { x.UserId })
                //.ToDictionary(x=>x.Key.UserId,x=>x.ToList());

            var desiredUsersPerWorkItem = 0;
            if(scheduledShiftWorkItems.Count() > scheduledShiftAssignments.Count() && scheduledShiftAssignments.Any())
            {
                desiredUsersPerWorkItem = scheduledShiftWorkItems.Count() / scheduledShiftAssignments.Count();
            }
            else if (scheduledShiftWorkItems.Any())
            {
                desiredUsersPerWorkItem = scheduledShiftAssignments.Count() / scheduledShiftWorkItems.Count();
            }
            //round up / ensure nonzero
            desiredUsersPerWorkItem++;

            //loop through work items for the shift and assign users
            foreach (var wi in autoShiftWorkItems)
            {
                if(wi.ShiftRecurringWorkItem.ShiftAssignmentMode == ShiftAssignmentMode.Auto)
                {
                    for(var i=wi.Assignments.Count;i<desiredUsersPerWorkItem && (!wi.RecurringWorkItem.MaximumRandomCount.HasValue || wi.Assignments.Count < wi.RecurringWorkItem.MaximumRandomCount.Value);i++)
                    {
                        //find users not already used on this work item
                        var unusedUsers = shiftAssignments.Where(x => !wi.Assignments.Any(y => y.UserId == x.Key && x.Value.Any(z => z.WorkItemId == wi.WorkItem.Id)));
                        //find users with the least assignments
                        if (unusedUsers.Any())
                        {
                            var minAssigned = unusedUsers.Min(x => x.Value.Count);
                            //make a pool of the users with equal, least assignments
                            var userIdPool = unusedUsers.Where(x => x.Value.Count == minAssigned)
                                .Select(x => x.Key).ToList();
                            if (userIdPool.Any())
                            {
                                var index = random.Next(userIdPool.Count());
                                var assignment = new WorkItemAssignment()
                                {
                                    UserId = userIdPool[index],
                                    WorkItemId = wi.WorkItem.Id
                                };
                                await _workItemRepository.InsertWorkItemAssignment(assignment);
                                shiftAssignments[userIdPool[index]].Add(assignment);
                                wi.Assignments.Add(assignment);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if(wi.ShiftRecurringWorkItem.ShiftAssignmentMode == ShiftAssignmentMode.CopyPrevious)
                {
                    var previous = workItems.Where(x => x.RecurringWorkItemId == wi.WorkItem.RecurringWorkItemId && x.ScheduledAt < wi.WorkItem.ScheduledAt)
                        .OrderByDescending(x => x.ScheduledAt).FirstOrDefault();

                    if (previous != null)
                    {
                        //find users assigned to most recent work item
                        var previousAssignments = assignments.Where(x => x.WorkItemId == previous.Id).ToList();
                        //find users in specified shift
                        var ssas = scheduledShiftAssignments.Where(x => x.ScheduledShiftId == scheduledShiftId).ToList();
                        //find users in previous work item assignment that exist in current shift
                        var assignUserIds = previousAssignments.Where(x => ssas.Any(y => y.AssignedUser != null && y.AssignedUser.Id == x.UserId)).Select(x => x.UserId).ToList();
                        //find users that already are assigned to the wi
                        var existingAssignments = assignments.Where(x => x.WorkItemId == wi.WorkItem.Id).Select(x=>x.UserId).ToList();
                        //find users that need to be assigned that aren't already assigned
                        var missingUserIds = assignUserIds.Where(x => !existingAssignments.Any(y => y == x)).ToList();

                        foreach(var id in missingUserIds)
                        {
                            var assignment = new WorkItemAssignment()
                            {
                                WorkItemId = wi.WorkItem.Id,
                                UserId = id
                            };
                            await _workItemRepository.InsertWorkItemAssignment(assignment);
                            wi.Assignments.Add(assignment);
                            shiftAssignments[id].Add(assignment);
                        }
                    }
                }
            }
        }

        public async Task PopulateTimedWorkItemOccurences(RecurringWorkItem workItem, int userId, List<int> assigneeUserIds,DateTime now, bool populateWorkItemAssignments=true)
        {
            var existingWorkItems = (await _workItemRepository.GetWorkItems(workItem.Id, now)).ToList();

            var calculatedWorkItems = new List<WorkItem>();

            for(var scheduledAt = workItem.RecurStart.Value; scheduledAt <= workItem.RecurEnd.Value; scheduledAt = scheduledAt + new TimeSpan(0,0,workItem.RecurIntervalSeconds.Value))
            {
                if (scheduledAt >= now)
                {
                    calculatedWorkItems.Add(new WorkItem()
                    {
                        AdminGroupId = workItem.AdminGroupId,
                        CompletionMode = workItem.CompletionMode,
                        CreatedAt = now,
                        CreatedByUserId = userId,
                        DescriptionMarkup = workItem.DescriptionMarkup,
                        Location = workItem.Location,
                        Name = workItem.Name,
                        PatrolId = workItem.PatrolId,
                        RecurringWorkItemId = workItem.Id,
                        ScheduledAt = scheduledAt,
                    });
                }
            }

            
            var allAssignments = await _workItemRepository.GetWorkItemAssignments(workItem.Id, now);

            var missing = calculatedWorkItems.Where(x => !existingWorkItems.Any(y => y.ScheduledAt == x.ScheduledAt)).ToList();
            var toRemove = existingWorkItems.Where(x => x.ScheduledAt > now && !calculatedWorkItems.Any(y => y.ScheduledAt == x.ScheduledAt));
            var toUpdate = existingWorkItems.Where(x => x.ScheduledAt > now && calculatedWorkItems.Any(y => y.ScheduledAt == x.ScheduledAt));

            foreach (var wi in missing)
            {
                await _workItemRepository.InsertWorkItem(wi);

                if (populateWorkItemAssignments && assigneeUserIds != null)
                {
                    foreach (var uid in assigneeUserIds)
                    {
                        var assignment = new WorkItemAssignment()
                        {
                            WorkItemId = wi.Id,
                            UserId = uid
                        };
                        await _workItemRepository.InsertWorkItemAssignment(assignment);
                    }
                }
            }

            foreach (var wi in toRemove)
            {
                var toRemoveAssignments = allAssignments.Where(x => x.WorkItemId == wi.Id).ToList();
                foreach (var wis in toRemoveAssignments)
                {
                    await _workItemRepository.DeleteWorkItemAssignment(wis);
                }
                await _workItemRepository.DeleteWorkItem(wi);
            }

            foreach (var wi in toUpdate)
            {
                wi.AdminGroupId = workItem.AdminGroupId;
                wi.CompletionMode = workItem.CompletionMode;
                wi.DescriptionMarkup = workItem.DescriptionMarkup;
                wi.Location = workItem.Location;
                wi.Name = workItem.Name;
                await _workItemRepository.UpdateWorkItem(wi);

                if (populateWorkItemAssignments)
                {
                    var assignees = allAssignments.Where(x => x.WorkItemId == wi.Id).ToList();

                    if (assigneeUserIds != null)
                    {
                        await assignees.DifferenceWith(assigneeUserIds,
                            (a, id) => a.UserId == id,
                            id => _workItemRepository.InsertWorkItemAssignment(new WorkItemAssignment()
                            {
                                WorkItemId = wi.Id,
                                UserId = id
                            }), wia => _workItemRepository.DeleteWorkItemAssignment(wia));
                    }
                }
            }
            
        }

        public async Task SaveWorkItem(WorkItemDto workItem, int userId)
        {
            WorkItem wi;
            List<WorkItemAssignment> assignments = new List<WorkItemAssignment>();

            if(workItem.Id!=default(int))
            {
                wi = await _workItemRepository.GetWorkItem(workItem.Id);

                if(wi.RecurringWorkItemId.HasValue || workItem.RecurringWorkItemId.HasValue)
                {
                    throw new InvalidOperationException("Cannot edit recurring work items individually");
                }


                wi.Location = workItem.Location;
                wi.Name = workItem.Name;
                wi.ScheduledAt = workItem.ScheduledAt;
                wi.ScheduledShiftId = workItem.ScheduledShiftId;
                wi.DescriptionMarkup = SanitizeHtml(workItem.DescriptionMarkup);
                await _workItemRepository.UpdateWorkItem(wi);
                assignments = (await _workItemRepository.GetWorkItemAssignments(wi.Id)).ToList();
            }
            else
            {
                if (workItem.RecurringWorkItemId.HasValue)
                {
                    throw new InvalidOperationException("Cannot edit recurring work items individually");
                }
                wi = new WorkItem()
                {
                    AdminGroupId = workItem.AdminGroupId,
                    CompletionMode = workItem.CompletionMode,
                    DescriptionMarkup = SanitizeHtml(workItem.DescriptionMarkup),
                    Location = workItem.Location,
                    Name = workItem.Name,
                    PatrolId = workItem.PatrolId,
                    ScheduledAt = workItem.ScheduledAt,
                    ScheduledShiftId = workItem.ScheduledShiftId,
                };
                await _workItemRepository.InsertWorkItem(wi);
            }

            

            await assignments.DifferenceWith(workItem.Assignments.Select(x=>x.UserId).ToList(),
                        (a, id) => a.UserId == id,
                        id => _workItemRepository.InsertWorkItemAssignment(new WorkItemAssignment()
                        {
                            WorkItemId = wi.Id,
                            UserId = id
                        }), wia => _workItemRepository.DeleteWorkItemAssignment(wia));

        }

        public async Task<RecurringWorkItemDto> GetRecurringWorkItem(int id)
        {
            var now = _clock.UtcNow.UtcDateTime;
            var dto = new RecurringWorkItemDto();
            _mapper.Map<RecurringWorkItem, RecurringWorkItemDto>(await _workItemRepository.GetRecurringWorkItem(id), dto);

            if(dto.AdminGroupId.HasValue)
            {
                dto.AdminGroup = await _groupRepository.GetGroup(dto.AdminGroupId.Value);
            }

            dto.CreatedBy = await _userRepository.GetUser(dto.CreatedByUserId);

            dto.Shifts = new List<ShiftRecurringWorkItemDto>();

            var shifts = await _workItemRepository.GetShiftRecurringWorkItems(id);
            foreach(var shift in shifts)
            {
                var shiftDto = _mapper.Map<ShiftRecurringWorkItem, ShiftRecurringWorkItemDto>(shift);
                shiftDto.Shift = await _shiftRepository.GetShift(shiftDto.ShiftId);
                dto.Shifts.Add(shiftDto);
            }

            var nextOccurences = await _workItemRepository.GetWorkItems(id, now);
            var next = nextOccurences.OrderBy(x => x.ScheduledAt).FirstOrDefault();
            dto.NextOccurenceUsers = new List<UserIdentifier>();
            if(next!=null)
            {
                var assignments = await _workItemRepository.GetWorkItemAssignments(next.Id);
                foreach(var assignment in assignments)
                {
                    var user = await _userRepository.GetUser(assignment.UserId);
                    dto.NextOccurenceUsers.Add((UserIdentifier)user);
                }
            }

            return dto;
        }

        public async Task<WorkItemDto> GetWorkItem(int id)
        {
            var now = _clock.UtcNow.UtcDateTime;
            var dto = new WorkItemDto();
            _mapper.Map<WorkItem, WorkItemDto>(await _workItemRepository.GetWorkItem(id), dto);

            if (dto.AdminGroupId.HasValue)
            {
                dto.AdminGroup = await _groupRepository.GetGroup(dto.AdminGroupId.Value);
            }

            dto.CreatedBy = await _userRepository.GetUser(dto.CreatedByUserId);

            if(dto.ScheduledShiftId.HasValue)
            {
                dto.ScheduledShift = (await _shiftRepository.GetScheduledShiftAssignments(dto.PatrolId, scheduledShiftId: dto.ScheduledShiftId.Value)).FirstOrDefault();
            }


            dto.Assignments = new List<WorkItemAssignmentDto>();
            var assignments = await _workItemRepository.GetWorkItemAssignments(id);
            foreach (var assignment in assignments)
            {
                var user = await _userRepository.GetUser(assignment.UserId);
                var assignmentDto = _mapper.Map<WorkItemAssignment, WorkItemAssignmentDto>(assignment);
                assignmentDto.User = user;

                dto.Assignments.Add(assignmentDto);
            }

            return dto;
        }

        public Task AddWorkItemsToNewShiftOccurence(ScheduledShift shift)
        {
            throw new NotImplementedException();
        }

        public Task RemoveWorkItemsFromShiftOccurence(ScheduledShift shift)
        {
            throw new NotImplementedException();
        }

        public Task SwapScheduledShiftWorkItems(int scheduledShiftId, int fromUserId, int toUserId)
        {
            throw new NotImplementedException();
        }

        public async Task CancelWorkItem(int workItemId, int userId)
        {
            var now = _clock.UtcNow.UtcDateTime;
            var workItem = await _workItemRepository.GetWorkItem(workItemId);
            if(!workItem.CompletedAt.HasValue && !workItem.CanceledAt.HasValue)
            {
                workItem.CanceledAt = now;
                workItem.CanceledByUserId = userId;
                await _workItemRepository.UpdateWorkItem(workItem);
            }
            else
            {
                throw new InvalidOperationException("Cannot Cancel Completed or canceled work items");
            }
        }

        public async Task<bool> CanCompleteWorkItem(int workItemId, int userId)
        {
            var workItem = await _workItemRepository.GetWorkItem(workItemId);

            if(workItem.CompletionMode== CompletionMode.Any)
            {
                return true;
            }
            
            if(workItem.CreatedByUserId == userId)
            {
                return true;
            }

            if (workItem.CompletionMode == CompletionMode.AnyAssigned || workItem.CompletionMode == CompletionMode.AllAssigned)
            {
                var assignees = await _workItemRepository.GetWorkItemAssignments(workItemId);
                if(assignees.Any(x=>x.UserId==userId))
                {
                    return true;
                }
            }

            if(workItem.AdminGroupId.HasValue)
            {
                var members = await _groupRepository.GetUsersInGroup(workItem.AdminGroupId.Value);
                if(members.Any(x=>x.Id==userId))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CanCancelWorkItem(int workItemId, int userId)
        {
            var workItem = await _workItemRepository.GetWorkItem(workItemId);

            if (workItem.CreatedByUserId == userId)
            {
                return true;
            }

            if (workItem.AdminGroupId.HasValue)
            {
                var members = await _groupRepository.GetUsersInGroup(workItem.AdminGroupId.Value);
                if (members.Any(x => x.Id == userId))
                {
                    return true;
                }
            }

            return false;
        }

        private string SanitizeHtml(string description)
        {
            var sanitizer = new HtmlSanitizer();

            var sanitized = sanitizer.Sanitize(description);

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var normalized = Markdown.Normalize(sanitized, pipeline: pipeline);

            var html = Markdown.ToHtml(normalized, pipeline: pipeline);

            return html;
        }
    }
}
