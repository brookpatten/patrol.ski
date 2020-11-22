using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Services
{
    public interface IInitialSetupService
    {
        void DefaultInitialSetup(int patrolId);
        void EmptyInitialSetup(int patrolId);
    }
}
