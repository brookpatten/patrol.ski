using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Amphibian.Patrol.Api.Repositories;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Amphibian.Patrol.Configuration;

namespace Amphibian.Patrol.Api.Controllers
{
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly IFileUploadRepository _imageUploadRepository;
        private readonly string _imageRelativeUrl;

        public FileUploadController(ILogger<FileUploadController> logger, IFileUploadRepository imageUploadRepository, AppConfiguration appConfiguration)
        {
            _logger = logger;
            _imageUploadRepository = imageUploadRepository;
            _imageRelativeUrl = appConfiguration.UserFileRelativeUrl;
        }

        public class FileUpload
        {
            public int? PatrolId { get; set; }
            public IFormFile FormFile { get; set; }
        }
        [HttpPost]
        [Route("file/upload")]
        [Authorize]
        public async Task<IActionResult> UploadFile([FromForm]FileUpload file)
        {
            if (!file.PatrolId.HasValue ||  User.PatrolIds().Any(x => x == file.PatrolId.Value))
            {
                var record = await _imageUploadRepository.PersistUpload(file.FormFile, User.UserId(), file.PatrolId);

                var relativeUrl = _imageRelativeUrl + "/" + record.GetUniqueName();

                return Ok(new
                {
                    Size = record.FileSize,
                    RelativeUrl = relativeUrl,
                    Id = record.Id,
                    Name = record.Name
                });
            }
            else
            {
                return Forbid();
            }
        }
    }
}
