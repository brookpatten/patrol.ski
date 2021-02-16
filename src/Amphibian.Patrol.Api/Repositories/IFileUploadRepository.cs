using Amphibian.Patrol.Api.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Repositories
{
    public interface IFileUploadRepository
    {
        Task InsertImageUpload(FileUpload upload);
        Task<FileUpload> PersistUpload(IFormFile upload, int userId, int? patrolId);
    }
}