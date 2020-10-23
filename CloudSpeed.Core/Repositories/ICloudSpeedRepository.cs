using Autofac;
using CloudSpeed.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CloudSpeed.Entities.DTO;

namespace CloudSpeed.Repositories
{
    public interface ICloudSpeedRepository : IUnitOfWork
    {
        Task CreateUploadLog(UploadLog entity);

        Task<UploadLog> GetUploadLog(string id);

        Task<UploadLog> GetUploadLogByDataKey(string dataKey);

        Task UpdateUploadLogByDataKey(string dataKey, string userId);

        Task<IEnumerable<UploadLog>> GetUploadLogs(int skip, int limit);

        Task<IEnumerable<UploadLog>> GetUploadLogs(UploadParamMap paramMap, int skip, int limit);

        Task<int> CountUploadLogs(UploadParamMap paramMap);

        Task CreateFileName(FileName entity);

        Task UpdateFileName(string id, long size, string format);

        Task<string> GetFileName(string id);

        Task<IEnumerable<FileName>> GetFileNames(int skip, int limit);

        Task CreateFileCid(FileCid entity);

        Task<bool> HasFileCid(string id);

        Task<FileCid> GetFileCid(string id);

        Task<IList<FileCid>> GetFileCids(FileCidStatus status, int skip, int limit);

        Task<FileCid> GetFileCidsByCid(string cid);

        Task<string> GetFileCidByDealId(string dealId);

        Task UpdateFileCid(string Id, string cid, FileCidStatus status);

        Task UpdateFileCid(string Id, FileCidStatus status, string error);

        Task CreateFileMd5(FileMd5 entity);

        Task<FileMd5> GetFileMd5(string id);

        Task<bool> HasFileMd5(string id);

        Task<IList<FileJob>> GetFileJobs(FileJobStatus status, int skip, int limit);

        IDictionary<string, int> CountJobsGroupByStatus();

        Task<IList<FileJob>> GetFileJobs(FileJobParamMap paramMap, int skip, int limit);

        Task<int> CountFileJobs(FileJobParamMap paramMap);

        Task CreateFileJob(FileJob entity);

        Task UpdateFileJob(string id, string jobId, FileJobStatus status);

        Task UpdateFileJob(string id, FileJobStatus status, string error);

        Task<IList<FileDeal>> GetFileDeals(FileDealStatus status, int skip, int limit);

        IDictionary<string, int> CountDealsGroupByStatus();

        Task<IList<FileDeal>> GetFileDeals(FileDealParamMap paramMap, int skip, int limit);

        Task<int> CountFileDeals(FileDealParamMap paramMap);

        Task CreateFileDeal(FileDeal entity);

        Task UpdateFileDeal(string id, string miner, string dealId, FileDealStatus status);

        Task UpdateFileDeal(string id, FileDealStatus status, string error);

        Task<FileDeal> GetFileDealByCid(string cid);

        Task CreateFileImport(FileImport entity);

        Task UpdateFileImport(string id, FileImportStatus status, string error, int total, int success, int failed);

        Task<IList<FileImport>> GetFileImports(int skip, int limit);

        Task<int> CountFileImports();
    }
}
