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

    public enum Permission
    {
        MaintainPlans,
        MaintainUsers,
        MaintainGroups,
        MaintainAssignments,
        RevokeSignatures,
        MaintainShifts,
        MaintainPatrol
    }

    public static class RoleExtensions
    {
        public static Dictionary<Role, IList<Permission>> DefaultPermissions = new Dictionary<Role, IList<Permission>>()
        {
            {Role.Administrator,Enum.GetValues(typeof(Permission)).Cast<Permission>().ToList() },
            {Role.Coordinator, new List<Permission>(){Permission.MaintainAssignments} }
        };

        public static IList<Permission> Permissions(this Role? role)
        {
            if (role.HasValue)
            {
                return DefaultPermissions[role.Value];
            }
            else
            {
                return new List<Permission>();
            }
        }

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

        public static bool Can(this Role? role, Permission permission)
        {
            return role.FalseIfNullOrTrueIf(x => DefaultPermissions[x].Contains(permission));
        }

        public static bool CanMaintainPlans(this Role? role)
        {
            return role.Can(Permission.MaintainPlans);
        }
        public static bool CanMaintainUsers(this Role? role)
        {
            return role.Can(Permission.MaintainUsers);
        }
        public static bool CanMaintainGroups(this Role? role)
        {
            return role.Can(Permission.MaintainGroups);
        }
        public static bool CanMaintainAssignments(this Role? role)
        {
            return role.Can(Permission.MaintainAssignments);
        }
        public static bool CanRevokeSignatures(this Role? role)
        {
            return role.Can(Permission.RevokeSignatures);
        }
        public static bool CanMaintainShifts(this Role? role)
        {
            return role.Can(Permission.MaintainShifts);
        }
        public static bool CanmaintainPatrol(this Role? role)
        {
            return role.Can(Permission.MaintainPatrol);
        }
    }
}
