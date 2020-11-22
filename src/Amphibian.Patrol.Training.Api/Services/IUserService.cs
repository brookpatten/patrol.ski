﻿using Amphibian.Patrol.Training.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Services
{
    public interface IUserService
    {
        Task<User> AddUserToPatrol(int patrolId, Role? role, string firstName, string lastName, string email);
        Task AddUserToGroup(int userId, int groupId);
        Task RemoveUserFromGroup(int userId, int groupId);
    }
}