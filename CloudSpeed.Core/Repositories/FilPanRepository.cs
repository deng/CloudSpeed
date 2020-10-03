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

        public async Task<IList<FileCid>> GetFileCids(FileCidStatus status, int skip, int limit)
        {
            return await DbContext.FileCids.AsNoTracking().Where(a => a.Status == status).Skip(skip).Take(limit).ToListAsync();
        }

        public async Task UpdateFileCid(string Id, string cid, FileCidStatus status)
        {
            var entity = await DbContext.FileCids.FirstOrDefaultAsync(a => a.Id == Id);
            if (entity != null)
            {
                entity.Cid = cid;
                entity.Status = status;
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

        public async Task CreateFileJob(FileJob entity)
        {
            await DbContext.FileJobs.AddAsync(entity);
        }

        public Task Commit()
        {
            return DbContext.SaveChangesAsync();
        }
    }
}
