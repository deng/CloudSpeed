using Autofac;
using CloudSpeed.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CloudSpeed.Repositories
{
    public interface ICloudSpeedRepository : IUnitOfWork
    {
        Task CreateUploadLog(UploadLog entity);

        Task<UploadLog> GetUploadLog(string id);

        Task CreateFileName(FileName entity);

        Task<string> GetFileName(string id);

        Task CreateFileCid(FileCid entity);

        Task<bool> HasFileCid(string id);
        
        Task<FileCid> GetFileCid(string id);

        Task<IList<FileCid>> GetFileCids(FileCidStatus status, int skip, int limit);

        Task UpdateFileCid(string Id, string cid, FileCidStatus status);

        Task CreateFileMd5(FileMd5 entity);
        
        Task<FileMd5> GetFileMd5(string id);
    }
}
