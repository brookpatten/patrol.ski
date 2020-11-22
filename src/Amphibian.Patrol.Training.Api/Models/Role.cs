using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Models
{
    public enum Role
    {
        Administrator,
        Coordinator
    }

    public static class RoleExtensions
    {
        public static bool FalseIfNullOrTrueIf(this Role? role,Func<Role,bool> trueIf)
        {
            if(!role.HasValue)
            {
                return false;
            }
            else
            {
                return trueIf(role.Value);
            }
        }
        public static bool FalseIfNullOrTrueIfIn(this Role? role, params Role[] roles)
        {
            return role.FalseIfNullOrTrueIf(x => roles.Contains(x));
        }
        public static bool CanMaintainPlans(this Role? role)
        {
            return role.FalseIfNullOrTrueIfIn(Role.Administrator);
        }
        public static bool CanMaintainUsers(this Role? role)
        {
            return role.FalseIfNullOrTrueIfIn(Role.Administrator);
        }
        public static bool CanMaintainGroups(this Role? role)
        {
            return role.FalseIfNullOrTrueIfIn(Role.Administrator);
        }
        public static bool CanMaintainAssignments(this Role? role)
        {
            return role.FalseIfNullOrTrueIfIn(Role.Administrator,Role.Coordinator);
        }
        public static bool CanRevokeSignatures(this Role? role)
        {
            return role.FalseIfNullOrTrueIfIn(Role.Administrator, Role.Coordinator);
        }
        public static bool CanMaintainShifts(this Role? role)
        {
            return role.FalseIfNullOrTrueIfIn(Role.Administrator, Role.Coordinator);
        }
        public static bool CanmaintainPatrol(this Role? role)
        {
            return role.FalseIfNullOrTrueIfIn(Role.Administrator);
        }
    }
}
