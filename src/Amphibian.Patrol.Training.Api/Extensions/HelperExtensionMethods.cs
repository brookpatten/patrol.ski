using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace Amphibian.Patrol.Training.Api.Extensions
{
    public static class HelperExtensionMethods
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            var id = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            return int.Parse(id);
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
            foreach(var e in Existing)
            {
                var changed = Changed.SingleOrDefault(x => Compare(e, x));
                if(changed!=null && Update!=null)
                {
                    await Update(e, changed);
                }

                if(changed==null && Remove!=null)
                {
                    await Remove(e);
                }
            }
        }
    }
}
