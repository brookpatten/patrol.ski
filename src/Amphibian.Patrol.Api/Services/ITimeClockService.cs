using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface ITimeClockService
    {
        Task<TimeEntry> ClockIn(int patrolId, int userId, DateTime? now = null);
        Task<TimeEntry> ClockOut(int timeEntryId, DateTime? now = null);
    }
}
