using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using TimeZoneConverter;
using SendGrid.Helpers.Mail;
using Amphibian.Patrol.Api.Models;

namespace Amphibian.Patrol.Api.Extensions
{
    public static class HelperExtensionMethods
    {
        public static long ToUnixTime(this DateTime dt)
        {
            return (long)(dt - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        }

        public static DateTime FromUnixTime(this long dt)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0) + new TimeSpan(dt * 10000000);
        }

        public static TimeZoneInfo LocalTimeZone(this Models.Patrol patrol)
        {
            TimeZoneInfo timeZone;
            if (!string.IsNullOrEmpty(patrol.TimeZone))
            {
                timeZone = TZConvert.GetTimeZoneInfo(patrol.TimeZone);
            }
            else
            {
                timeZone = TZConvert.GetTimeZoneInfo("Eastern Standard Time");
            }
            return timeZone;
        }

        public static DateTime UtcToPatrolLocal(this DateTime utc,Models.Patrol patrol)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utc, patrol.LocalTimeZone());
        }

        public static DateTime? UtcToPatrolLocal(this DateTime? utc, Models.Patrol patrol)
        {
            if (utc.HasValue)
            {
                return utc.Value.UtcToPatrolLocal(patrol);
            }
            else
            {
                return null;
            }
        }

        public static DateTime UtcFromPatrolLocal(this DateTime local, Models.Patrol patrol)
        {
            return TimeZoneInfo.ConvertTimeToUtc(local, patrol.LocalTimeZone());
        }

        public static DateTime? UtcFromPatrolLocal(this DateTime? local, Models.Patrol patrol)
        {
            if (local.HasValue)
            {
                return local.Value.UtcFromPatrolLocal(patrol);
            }
            else
            {
                return null;
            }
        }

        public static void SendNewToken(this HttpResponse response,string jwt)
        {
            string key = "Authorization";
            string value = "Token " + jwt;

            if (response.Headers.ContainsKey(key))
            {
                response.Headers[key] = value;
            }
            else
            {
                response.Headers.Add(key, value);
            }
        }

        public static EmailAddress ToEmailAddress(this User user)
        {
            return new EmailAddress(user.Email, user.GetFullName());
        }

        public static async Task<EnumberableDiff<T,K>> DifferenceWith<T,K>(
            this IEnumerable<T> existing,
            IEnumerable<K> changed,
            Func<T,K,bool> compare=null,
            Func<K,Task> add=null,
            Func<T,Task> remove=null, 
            Func<T,K,Task> update=null)
        {
            var diff = new EnumberableDiff<T,K>()
            {
                Existing = existing,
                Changed = changed,
                Add = add,
                Compare = compare,
                Remove = remove,
                Update = update
            };
            await diff.Execute();
            return diff;
        }
    }

    public class EnumberableDiff<T,K>
    {
        public IEnumerable<T> Existing { get; set; }
        public IEnumerable<K> Changed { get; set; }
        public Func<K,Task> Add { get; set; }
        public Func<T,Task> Remove { get; set; }
        public Func<T,K,Task> Update { get; set; }
        public Func<T,K,bool> Compare { get; set; }
        public async Task Execute()
        {
            //process adds
            if (Add != null)
            {
                foreach (var e in Changed)
                {
                    if (!Existing.Any(y => Compare(y, e)))
                    {
                        await Add(e);
                    }
                }
            }
            foreach (var e in Existing)
            {
                if (Changed.Any(x => Compare(e, x)))
                {
                    if (Update != null)
                    {
                        await Update(e, Changed.Single(x => Compare(e, x)));
                    }
                }
                else
                {
                    if (Remove != null)
                    {
                        await Remove(e);
                    }
                }
            }
        }
    }
}
