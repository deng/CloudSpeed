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

        Task<FileCid> GetFileCidsByCid(string cid);

        Task UpdateFileCid(string Id, string cid, FileCidStatus status);

        Task UpdateFileCid(string Id, FileCidStatus status, string error);

        Task CreateFileMd5(FileMd5 entity);

        Task<FileMd5> GetFileMd5(string id);

        Task<bool> HasFileMd5(string id);

        Task<IList<FileJob>> GetFileJobs(FileJobStatus status, int skip, int limit);

        Task CreateFileJob(FileJob entity);

        Task UpdateFileJob(string id, string jobId, FileJobStatus status);

        Task UpdateFileJob(string id, FileJobStatus status, string error);

        Task<IList<FileDeal>> GetFileDeals(FileDealStatus status, int skip, int limit);

        Task CreateFileDeal(FileDeal entity);

        Task UpdateFileDeal(string id, string miner, string dealId, FileDealStatus status);

        Task UpdateFileDeal(string id, FileDealStatus status, string error);
    }
}
