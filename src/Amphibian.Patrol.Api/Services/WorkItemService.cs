using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public class WorkItemService
    {
        private ILogger<WorkItemService> _logger;
        private IPatrolRepository _patrolRepository;
        private IGroupRepository _groupRepository;
        private IShiftRepository _shiftRepository;
        private ISystemClock _clock;
        private IEmailService _emailService;
        private IUserRepository _userRepository;
        private IWorkItemRepository _workItemRepository;

        public WorkItemService(ILogger<WorkItemService> logger, IPatrolRepository patrolRepository,
            IGroupRepository groupRepository, IShiftRepository shiftRepository, ISystemClock clock, IEmailService emailService, 
            IUserRepository userRepository, IWorkItemRepository workItemRepository)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _groupRepository = groupRepository;
            _shiftRepository = shiftRepository;
            _clock = clock;
            _emailService = emailService;
            _userRepository = userRepository;
            _workItemRepository = workItemRepository;
        }
    }
}
