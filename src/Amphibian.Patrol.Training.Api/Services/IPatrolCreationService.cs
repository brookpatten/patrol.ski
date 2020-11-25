using Amphibian.Patrol.Training.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Amphibian.Patrol.Training.Api.Services.PatrolCreationService;

namespace Amphibian.Patrol.Training.Api.Services
{
    public interface IPatrolCreationService
    {
        Task<Models.Patrol> CreateNewPatrol(int userId, string name);
        Task CreateDefaultInitialSetup(int patrolId);
        Task CreateDemoInitialSetup(int patrolId);
        Task<Tuple<User,Models.Patrol>> CreateDemoUserAndPatrol();
        Task CreateBuiltInPlan(BuiltInPlan plan, int patrolId);
    }
}
