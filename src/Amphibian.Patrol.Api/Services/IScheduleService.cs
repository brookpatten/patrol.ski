using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IScheduleService
    {
        Task<ScheduledShift> ScheduleShift(ScheduledShiftUpdateDto dto);
        Task CancelShift(int scheduledShiftId);

        Task ReleaseShift(int scheduledShiftAssignmentId);
        Task ClaimShift(int scheduledShiftAssignmentId, int userId);
        Task ApproveShiftSwap(int scheduledShiftAssignmentId, int userId);
        Task DeclineShiftSwap(int scheduledShiftAssignmentId,int userId, string reason);
        Task CancelShiftRelease(int scheduledShiftAssignmentId);
        Task ReplicatePeriod(int patrolId,bool clearTargetPeriodFirst,DateTime replicatedPeriodStart, DateTime replicatedPeriodEnd, DateTime targetPeriodStart, DateTime targetPeriodEnd);
    }
}
