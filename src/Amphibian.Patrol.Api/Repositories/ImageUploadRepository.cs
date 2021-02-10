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

namespace Amphibian.Patrol.Api.Repositories
{
    public class ImageUploadRepository : IImageUploadRepository
    {
        private readonly DbConnection _connection;

        public ImageUploadRepository(DbConnection connection)
        {
            _connection = connection;
        }
        public async Task InsertImageUpload(ImageUpload upload)
        {
            var id = (int)await _connection.InsertAsync(upload).ConfigureAwait(false);
            upload.Id = id;
        }
    }
}
