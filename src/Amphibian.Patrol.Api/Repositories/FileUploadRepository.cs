using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using Dommel;

using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Dtos;
using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Amphibian.Patrol.Configuration;
using System.IO;

namespace Amphibian.Patrol.Api.Repositories
{
    public class FileUploadRepository : IFileUploadRepository
    {
        private readonly DbConnection _connection;
        private readonly string _imageRoot;

        public FileUploadRepository(DbConnection connection, AppConfiguration configuration)
        {
            _connection = connection;
            _imageRoot = configuration.UserFilePath;
        }
        public async Task InsertImageUpload(FileUpload upload)
        {
            var id = (int)await _connection.InsertAsync(upload).ConfigureAwait(false);
            upload.Id = id;
        }
        public async Task<FileUpload> PersistUpload(IFormFile upload, int userId, int? patrolId)
        {
            var dbRecord = new FileUpload()
            {
                FileSize = upload.Length,
                Name = upload.FileName,
                PatrolId = patrolId,
                UserId = userId
            };
            await this.InsertImageUpload(dbRecord);
            var name = dbRecord.GetUniqueName();
            string path = Path.Combine(_imageRoot, name);
            using (var fs = new FileStream(path,FileMode.CreateNew))
            {
                using (var rs = upload.OpenReadStream())
                {
                    await rs.CopyToAsync(fs);
                }
            }
            return dbRecord;
        }
    }
}
