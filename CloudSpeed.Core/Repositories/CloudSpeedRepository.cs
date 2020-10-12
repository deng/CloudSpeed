using CloudSpeed.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

namespace CloudSpeed.Repositories
{
    public class CloudSpeedRepository : RepositoryBase<CloudSpeedDbContext>, ICloudSpeedRepository
    {
        public CloudSpeedRepository(CloudSpeedDbContext dbContext) : base(dbContext)
        {

        }

        public async Task CreateUploadLog(UploadLog entity)
        {
            await DbContext.UploadLogs.AddAsync(entity);
        }

        public Task<UploadLog> GetUploadLog(string id)
        {
            return DbContext.UploadLogs.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task CreateFileName(FileName entity)
        {
            await DbContext.FileNames.AddAsync(entity);
        }

        public Task<string> GetFileName(string id)
        {
            return DbContext.FileNames.Where(a => a.Id == id).Select(a => a.Name).FirstOrDefaultAsync();
        }

        public async Task CreateFileCid(FileCid entity)
        {
            await DbContext.FileCids.AddAsync(entity);
        }

        public Task<bool> HasFileCid(string id)
        {
            return DbContext.FileCids.AnyAsync(a => a.Id == id);
        }

        public Task<FileCid> GetFileCid(string id)
        {
            return DbContext.FileCids.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }

        public Task<FileCid> GetFileCidsByCid(string cid)
        {
            return DbContext.FileCids.AsNoTracking().FirstOrDefaultAsync(a => a.Cid == cid);
        }

        public async Task<IList<FileCid>> GetFileCids(FileCidStatus status, int skip, int limit)
        {
            return await DbContext.FileCids.AsNoTracking().Where(a => a.Status == status).Skip(skip).Take(limit).ToListAsync();
        }

        public async Task UpdateFileCid(string id, string cid, FileCidStatus status)
        {
            var entity = await DbContext.FileCids.FirstOrDefaultAsync(a => a.Id == id);
            if (entity != null)
            {
                entity.Cid = cid;
                entity.Status = status;
                entity.Updated = DateTime.Now;
            }
        }

        public async Task UpdateFileCid(string id, FileCidStatus status, string error)
        {
            var entity = await DbContext.FileCids.FirstOrDefaultAsync(a => a.Id == id);
            if (entity != null)
            {
                entity.Status = status;
                entity.Error = error;
                entity.Updated = DateTime.Now;
            }
        }

        public async Task CreateFileMd5(FileMd5 entity)
        {
            await DbContext.FileMd5s.AddAsync(entity);
        }

        public Task<FileMd5> GetFileMd5(string id)
        {
            return DbContext.FileMd5s.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }

        public Task<bool> HasFileMd5(string id)
        {
            return DbContext.FileMd5s.AnyAsync(a => a.Id == id);
        }

        public async Task<IList<FileJob>> GetFileJobs(FileJobStatus status, int skip, int limit)
        {
            return await DbContext.FileJobs.AsNoTracking().Where(a => a.Status == status).Skip(skip).Take(limit).ToListAsync();
        }

        public async Task<IDictionary<FileJobStatus, int>> CountJobsGroupByStatus()
        {
            return await DbContext.FileJobs.AsNoTracking().GroupBy(a => a.Status).ToDictionaryAsync(a => a.Key, a => a.Count());
        }

        public async Task<IList<FileJob>> GetFileJobs(int skip, int limit)
        {
            return await DbContext.FileJobs.AsNoTracking().Skip(skip).Take(limit).ToListAsync();
        }

        public async Task<int> CountFileJobs()
        {
            return await DbContext.FileJobs.CountAsync();
        }

        public async Task CreateFileJob(FileJob entity)
        {
            await DbContext.FileJobs.AddAsync(entity);
        }

        public async Task UpdateFileJob(string id, string jobId, FileJobStatus status)
        {
            var entity = await DbContext.FileJobs.FirstOrDefaultAsync(a => a.Id == id);
            if (entity != null)
            {
                entity.JobId = jobId;
                entity.Status = status;
                entity.Updated = DateTime.Now;
            }
        }

        public async Task UpdateFileJob(string id, FileJobStatus status, string error)
        {
            var entity = await DbContext.FileJobs.FirstOrDefaultAsync(a => a.Id == id);
            if (entity != null)
            {
                entity.Status = status;
                entity.Error = error;
                entity.Updated = DateTime.Now;
            }
        }

        public async Task<IList<FileDeal>> GetFileDeals(FileDealStatus status, int skip, int limit)
        {
            return await DbContext.FileDeals.AsNoTracking().Where(a => a.Status == status).Skip(skip).Take(limit).ToListAsync();
        }

        public async Task<IDictionary<FileDealStatus, int>> CountDealsGroupByStatus()
        {
            return await DbContext.FileDeals.AsNoTracking().GroupBy(a => a.Status).ToDictionaryAsync(a => a.Key, a => a.Count());
        }

        public async Task<IList<FileDeal>> GetFileDeals(int skip, int limit)
        {
            return await DbContext.FileDeals.AsNoTracking().Skip(skip).Take(limit).ToListAsync();
        }

        public async Task<int> CountFileDeals()
        {
            return await DbContext.FileDeals.CountAsync();
        }

        public async Task CreateFileDeal(FileDeal entity)
        {
            await DbContext.FileDeals.AddAsync(entity);
        }

        public async Task UpdateFileDeal(string id, string miner, string dealId, FileDealStatus status)
        {
            var entity = await DbContext.FileDeals.FirstOrDefaultAsync(a => a.Id == id);
            if (entity != null)
            {
                entity.Status = status;
                entity.Miner = miner;
                entity.DealId = dealId;
                entity.Updated = DateTime.Now;
            }
        }

        public async Task UpdateFileDeal(string id, FileDealStatus status, string error)
        {
            var entity = await DbContext.FileDeals.FirstOrDefaultAsync(a => a.Id == id);
            if (entity != null)
            {
                entity.Status = status;
                entity.Error = error;
                entity.Updated = DateTime.Now;
            }
        }

        public async Task CreateFileImport(FileImport entity)
        {
            await DbContext.FileImports.AddAsync(entity);
        }

        public async Task UpdateFileImport(string id, FileImportStatus status, string error, int total, int success, int failed)
        {
            var entity = await DbContext.FileImports.FirstOrDefaultAsync(a => a.Id == id);
            if (entity != null)
            {
                entity.Status = status;
                entity.Error = error;
                entity.Total = total;
                entity.Success = success;
                entity.Failed = failed;
                entity.Updated = DateTime.Now;
            }
        }

        public async Task<IList<FileImport>> GetFileImports(int skip, int limit)
        {
            return await DbContext.FileImports.AsNoTracking().Skip(skip).Take(limit).ToListAsync();
        }

        public async Task<int> CountFileImports()
        {
            return await DbContext.FileImports.CountAsync();
        }

        public Task Commit()
        {
            return DbContext.SaveChangesAsync();
        }
    }
}
