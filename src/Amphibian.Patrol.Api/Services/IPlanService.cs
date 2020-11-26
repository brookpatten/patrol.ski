using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IPlanService
    {
        Task<PlanDto> GetPlan(int id, int? currentUserId=null);
        Task<bool> IsPlanFormatValid(PlanDto plan);
        Task<Plan> CreatePlan(string name, int patrolId, int? copyFromPlanId);
        Task UpdatePlan(PlanDto dto);
    }
}