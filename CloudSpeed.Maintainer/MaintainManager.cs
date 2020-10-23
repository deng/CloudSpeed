using CloudSpeed.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CloudSpeed.Managers;
using CloudSpeed.Settings;
using System;
using System.IO;
using System.Linq;
using CloudSpeed.Web.Requests;
using System.Globalization;
using System.Threading;
using CloudSpeed.Entities;
using System.IO.Compression;
using CloudSpeed.Repositories;
using Autofac;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using CloudSpeed.Identity;

namespace CloudSpeed.Maintainer
{
    public class MaintainManager
    {
        private readonly ILogger _logger;
        private readonly CloudSpeedManager _cloudSpeedManager;
        private readonly UploadSetting _uploadSetting;
        private readonly IConfiguration _configuration;

        public MaintainManager(ILoggerFactory loggerFactory, CloudSpeedManager cloudSpeedManager, UploadSetting uploadSetting, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<MaintainManager>();
            _cloudSpeedManager = cloudSpeedManager;
            _uploadSetting = uploadSetting;
            _configuration = configuration;
        }

        public async Task Maintain1(CancellationToken stoppingToken)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var skip = 0;
                var limit = 10;
                while (true)
                {
                    var fileNames = await repository.GetFileNames(skip, limit);
                    if (!fileNames.Any())
                        break;
                    _logger.LogInformation(string.Format("{0} files will be maintain", fileNames.Count()));
                    foreach (var fn in fileNames)
                    {
                        if (fn.Size == 0)
                        {
                            var path = _uploadSetting.GetStoragePath(fn.Id);
                            var fileInfo = new FileInfo(path);
                            await repository.UpdateFileName(fn.Id, fileInfo.Exists ? fileInfo.Length : 0, fn.Name.GetMimeType());
                        }
                    }
                    await repository.Commit();
                    skip += limit;
                }
                _logger.LogInformation("Maintain1 Done");
            }
        }

        public async Task Maintain2(CancellationToken stoppingToken)
        {
            var editSetting = _configuration.GetSection("EditSetting").Get<IDictionary<string, string>>();
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var memberManager = scope.Resolve<UserManager<Member>>();
                var cloudSpeedRepository = scope.Resolve<ICloudSpeedRepository>();
                foreach (var es in editSetting)
                {
                    var format = es.Key;
                    var userName = es.Value.ToLower();
                    var member = await memberManager.FindByNameAsync(userName);
                    if (member == null)
                    {
                        member = new Member()
                        {
                            UserName = userName,
                        };
                        var createdResult = await memberManager.CreateAsync(member, System.Guid.NewGuid().ToString());
                        if (!createdResult.Succeeded)
                        {
                            _logger.LogInformation(string.Format("create member fail {0}", createdResult.Errors.FirstOrDefault().Code ?? "create failure"));
                        }
                        else
                        {
                            _logger.LogInformation(string.Format("create member success {0}", userName));
                        }
                    }
                }
                var skip = 0;
                var limit = 10;
                while (true)
                {
                    var fileNames = await cloudSpeedRepository.GetFileNames(skip, limit);
                    if (!fileNames.Any())
                    {
                        break;
                    }
                    _logger.LogInformation(string.Format("{0} files will be maintain", fileNames.Count()));
                    foreach (var fn in fileNames)
                    {
                        var userName = editSetting[fn.Format].ToLower();
                        var member = await memberManager.FindByNameAsync(userName);
                        await cloudSpeedRepository.UpdateUploadLogByDataKey(fn.Id, member.Id);
                    }
                    await cloudSpeedRepository.Commit();
                    skip += limit;
                }
            }
            _logger.LogInformation("Maintain2 Done");
        }
    }
}