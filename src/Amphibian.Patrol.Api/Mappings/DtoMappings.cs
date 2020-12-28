using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Mappings
{
    public static class DtoMappings
    {
        public static IServiceCollection AddMappings(this IServiceCollection services)
        {
            var configuration = GetMapperConfiguration();
            return services.AddTransient<IMapper>(p => configuration.CreateMapper());
        }

        public static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Plan, PlanDto>();
                cfg.CreateMap<Assignment, AssignmentDto>();
                cfg.CreateMap<Section, SectionDto>();
                cfg.CreateMap<SectionSkill, SectionSkillDto>();
                cfg.CreateMap<SectionLevel, SectionLevelDto>();
                cfg.CreateMap<Signature, SignatureDto>();
                cfg.CreateMap<User, PatrolUserDto>();
                cfg.CreateMap<SectionGroup, SectionGroupDto>();

                cfg.CreateMap<RecurringWorkItem, RecurringWorkItemDto>();
                cfg.CreateMap<RecurringWorkItemDto, RecurringWorkItem>();
                cfg.CreateMap<WorkItem, WorkItemDto>();
                cfg.CreateMap<WorkItemDto, WorkItem>();
                cfg.CreateMap<WorkItemAssignment, WorkItemAssignmentDto>();
                cfg.CreateMap<WorkItemAssignmentDto, WorkItemAssignment>();
                cfg.CreateMap<ShiftRecurringWorkItem, ShiftRecurringWorkItemDto>();
                cfg.CreateMap<ShiftRecurringWorkItemDto, ShiftRecurringWorkItem>();
            });
            return configuration;
        }
    }
}
