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
        Task<IEnumerable<Section>> GetSectionsForPlan(int planId);
        Task<IEnumerable<SectionSkill>> GetSectionSkills(int sectionId);
        Task<IEnumerable<SectionSkill>> GetSectionSkillsForPlan(int planId);
        Task<IEnumerable<Skill>> GetSkills(int patrolId);
        Task<IEnumerable<SectionLevel>> GetSectionLevels(int sectionId);
        Task<IEnumerable<SectionLevel>> GetSectionLevelsForPlan(int planId);
        Task<IEnumerable<Level>> GetLevels(int patrolId);
    }
}