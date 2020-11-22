using Amphibian.Patrol.Training.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphibian.Patrol.Training.Api.Dtos;

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
        Task<IEnumerable<SectionSkillDto>> GetSectionSkillsForPlan(int planId);
        Task<IEnumerable<Skill>> GetSkills(int patrolId);
        Task<IEnumerable<SectionLevel>> GetSectionLevels(int sectionId);
        Task<IEnumerable<SectionLevelDto>> GetSectionLevelsForPlan(int planId);
        Task<IEnumerable<Level>> GetLevels(int patrolId);
        Task InsertSection(Section section);
        Task InsertPlanSection(PlanSection planSection);
        Task InsertSectionLevel(SectionLevel sectionLevel);
        Task InsertSectionSkill(SectionSkill sectionSkill);
        Task InsertLevel(Level level);
        Task InsertSkill(Skill skill);
    }
}