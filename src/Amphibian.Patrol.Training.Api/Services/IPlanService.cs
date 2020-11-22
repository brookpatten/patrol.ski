using Amphibian.Patrol.Training.Api.Dtos;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Services
{
    public interface IPlanService
    {
        Task<PlanDto> GetPlan(int id, int currentUserId);
    }
}