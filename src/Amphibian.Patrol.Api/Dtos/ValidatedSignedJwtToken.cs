using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Dtos
{
    public class ValidatedSignedJwtToken
    {
        public UserIdentifier User { get; set; }
        public Token Token { get; set; }
        public List<CurrentUserPatrolDto> Patrols { get; set; }
    }

    public static class ValidatedSignedJwtTokenExtensions
    {
        public static ValidatedSignedJwtToken ParseAllClaims(this ClaimsPrincipal principal)
        {
            UserIdentifier user = new UserIdentifier();
            user.Id = int.Parse(principal.Claims.Single(x => x.Type == "uid").Value);

            Token token = new Token();
            token.TokenGuid = Guid.Parse(principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value);
            token.CreatedAt = long.Parse(principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Iat).Value).FromUnixTime();

            List<CurrentUserPatrolDto> patrols = JsonSerializer.Deserialize<List<CurrentUserPatrolDto>>(principal.Claims.Single(x => x.Type == "patrols").Value);

            return new ValidatedSignedJwtToken()
            {
                User = user,
                Token = token,
                Patrols = patrols
            };
        }

        public static List<int> PatrolIds(this ClaimsPrincipal principal)
        {
            return principal.ParseAllClaims().Patrols.Select(x => x.Id).ToList();
        }

        public static Role? RoleInPatrol(this ClaimsPrincipal principal,int patrolId)
        {
            return principal.ParseAllClaims().Patrols.SingleOrDefault(x => x.Id == patrolId)?.Role;
        }


        public static bool AllowInPatrol(this ClaimsPrincipal principal, int patrolId, Permission permission)
        {
            var parsed = principal.ParseAllClaims();
            if(parsed.Patrols.Any(x=>x.Id==patrolId))
            {
                return parsed.Patrols.Single(x => x.Id == patrolId).Role.Can(permission);
            }
            else
            {
                return false;
            }
        }

        public static int UserId(this ClaimsPrincipal principal)
        {
            var id = principal.ParseAllClaims().User.Id;
            return id;
        }
    }
}
