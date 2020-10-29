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
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using CloudSpeed.Identity;

namespace CloudSpeed.Uploader
{
    public class UploadManager
    {
        private readonly ILogger _logger;
        private readonly CloudSpeedManager _cloudSpeedManager;
        private readonly UploadSetting _uploadSetting;
        private readonly IConfiguration _configuration;
        private readonly UserManager<Member> _memberManager;

        public UploadManager(ILoggerFactory loggerFactory,
            CloudSpeedManager cloudSpeedManager, UploadSetting uploadSetting, IConfiguration configuration,
            UserManager<Member> memberManager)
        {
            _logger = loggerFactory.CreateLogger<UploadManager>();
            _cloudSpeedManager = cloudSpeedManager;
            _uploadSetting = uploadSetting;
            _configuration = configuration;
            _memberManager = memberManager;
        }

        public async Task UploadAll(string path, bool zip, CancellationToken stoppingToken)
        {
            if (Directory.Exists(path))
            {
                var di = new DirectoryInfo(path);
                if (zip)
                {
                    var tempZipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), di.Name + ".zip");
                    var tempZipInfo = new FileInfo(tempZipPath);
                    tempZipInfo.Directory.Create();
                    ZipFile.CreateFromDirectory(path, tempZipPath);
                    await Upload(tempZipPath, stoppingToken);
                    tempZipInfo.Delete();
                    tempZipInfo.Directory.Delete();
                    _logger.LogInformation(string.Format("1 zip file uploaded for directory  {0}", path));
                    return;
                }
                var files = di.GetFiles("*", SearchOption.AllDirectories);
                var i = 0;
                _logger.LogInformation(string.Format("{0} files found", files.Length));
                foreach (var file in files)
                {
                    if (stoppingToken.IsCancellationRequested) break;
                    i++;
                    await Upload(file.FullName, stoppingToken);
                    _logger.LogInformation(string.Format("{0} file uploaded, {1:P2}", i, (float)i / files.Length));
                }
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(string.Format("{0} file uploaded, {1:P2}, {2} file cancelled", i, (float)i / files.Length, files.Length - i));
                }
            }
            else if (File.Exists(path))
            {
                await Upload(path, stoppingToken);
                _logger.LogInformation(string.Format("1 file uploaded, {1}", path));
            }
            else
            {
                _logger.LogInformation(string.Format("{0} not found", path));
            }
        }

        public async Task Upload(string source, CancellationToken stoppingToken)
        {
            var sourceInfo = new FileInfo(source);
            if (!sourceInfo.Exists)
                return;
            var sourceSize = sourceInfo.Length;
            if (_uploadSetting.MaxFileSize > 0 && sourceSize > _uploadSetting.MaxFileSize)
            {
                _logger.LogInformation("file too large {source}", source);
            }
            else if (_uploadSetting.MinFileSize > 0 && sourceSize < _uploadSetting.MinFileSize)
            {
                _logger.LogInformation("file too small {source}", source);
            }
            else
            {
                var md5 = _cloudSpeedManager.GetMD5HashFromFile(source);
                if (string.IsNullOrEmpty(md5))
                {
                    _logger.LogInformation("file exists {source}", source);
                    return;
                }
                var hasFileMd5 = await _cloudSpeedManager.CheckFileMd5ById(md5);
                if (!hasFileMd5)
                {
                    var dataKey = SequentialGuid.NewGuidString();
                    try
                    {
                        var editSetting = _configuration.GetSection("EditSetting").Get<IDictionary<string, string>>();
                        var target = _uploadSetting.GetStoragePath(dataKey);
                        var format = sourceInfo.Name.GetMimeType();
                        if (!editSetting.ContainsKey(format))
                        {
                            _logger.LogError(0, "file format not found at edit setting {format}", format);
                            return;
                        }
                        var userName = editSetting[format].ToLower();
                        var member = await _memberManager.FindByNameAsync(userName);
                        if (member == null)
                        {
                            _logger.LogError(0, "member not found {userName}", userName);
                            return;
                        }
                        File.Copy(source, target);
                        await _cloudSpeedManager.CreateFileMd5(md5, dataKey);
                        await _cloudSpeedManager.CreateFileName(dataKey, sourceInfo.Name, sourceSize);
                        await _cloudSpeedManager.CreateUploadLog(member.Id, new PanePostRequest { DataKey = dataKey });
                        _logger.LogInformation("file upload successfully {dataKey} - {source}", dataKey, source);
                    }
                    catch (System.Exception ex)
                    {
                        _logger.LogError(0, ex, "file upload failed {dataKey} - {source}", dataKey, source);
                    }
                }
                else
                {
                    _logger.LogInformation("file exists {source}", source);
                }
            }
        }
    }
}