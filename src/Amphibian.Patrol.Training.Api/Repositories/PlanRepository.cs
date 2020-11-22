﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;

using Amphibian.Patrol.Training.Api.Models;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public class PlanRepository: IPlanRepository
    {
        private readonly IDbConnection _connection;

        public PlanRepository(IDbConnection connection)
        {
            _connection = connection;
        }
    }
}