using Amphibian.Patrol.Api.Models;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IEventService
    {
        Task PostEvent(Event theEvent,int userId);
    }
}