using Amphibian.Patrol.Training.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public interface IPlanRepository
    {
        public Task<IEnumerable<Plan>> GetPlansForPatrol(int patrolId);
        Task InsertPlan(Plan plan);
        Task UpdatePlan(Plan plan);
        Task<Plan> GetPlan(int id);
    }
}