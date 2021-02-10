using Amphibian.Patrol.Api.Models;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Repositories
{
    public interface IImageUploadRepository
    {
        Task InsertImageUpload(ImageUpload upload);
    }
}