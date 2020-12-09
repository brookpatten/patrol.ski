using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface ITimeClockService
    {
        Task<CurrentTimeEntryDto> ClockIn(int patrolId, int userId, DateTime? now = null);
        Task<CurrentTimeEntryDto> ClockOut(int timeEntryId, DateTime? now = null);
        Task<CurrentTimeEntryDto> GetCurrent(int patrolId, int userId);
    }
}
