using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Amphibian.Patrol.Api.Models;
using Dapper;

namespace Amphibian.Patrol.Api.Extensions
{
    /// <summary>
    /// sets the "kind" in date time objects from the database to be UTC
    /// </summary>
    public class DapperShiftStatusHandler : SqlMapper.TypeHandler<ShiftStatus>
    {
        public override void SetValue(IDbDataParameter parameter, ShiftStatus value)
        {
            parameter.Value = value.ToString();
        }

        public override ShiftStatus Parse(object value)
        {
            return (ShiftStatus)Enum.Parse(typeof(ShiftStatus), value.ToString());
        }
    }
}
