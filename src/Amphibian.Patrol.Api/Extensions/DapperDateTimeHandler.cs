using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

namespace Amphibian.Patrol.Api.Extensions
{
    /// <summary>
    /// sets the "kind" in date time objects from the database to be UTC
    /// </summary>
    public class DapperDateTimeHandler : SqlMapper.TypeHandler<DateTime>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = value;
        }

        public override DateTime Parse(object value)
        {
            return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
        }
    }

    /// <summary>
    /// sets the "kind" in date time objects from the database to be UTC
    /// </summary>
    public class DapperDateNullableTimeHandler : SqlMapper.TypeHandler<DateTime?>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime? value)
        {
            parameter.Value = value;
        }

        public override DateTime? Parse(object value)
        {
            if (value != null)
            {
                return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
            }
            else
            {
                return null;
            }
        }
    }
}
