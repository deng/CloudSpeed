using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudSpeed.Sdk;
using CloudSpeed.Web.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using CloudSpeed.Services;
using CloudSpeed.Managers;

namespace filshareapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ApiControllerBase
    {
        private readonly ILogger<UploadController> _logger;
        private readonly LotusClient _lotusClient;
        private readonly LotusClientSetting _lotusClientSetting;
        private readonly CloudSpeedManager _CloudSpeedManager;

        public UploadController(ILogger<UploadController> logger, LotusClient lotusClient, LotusClientSetting lotusClientSetting, CloudSpeedManager CloudSpeedManager)
        {
            _logger = logger;
            _lotusClient = lotusClient;
            _lotusClientSetting = lotusClientSetting;
            _CloudSpeedManager = CloudSpeedManager;
        }

        [HttpPost]
        [Route("SmallFileUpload")]
        [RequestSizeLimit(2097152)]
        public async Task<ActionResult> UploadFile()
        {
            var item = await CreateLocalFileFromUploadStream(GetRewardPath);
            if (item == null)
                return Result(ApiResponse.BadRequestResult("please select file to upload"));

            return Ok(item.Item1);
        }

        [HttpPost]
        [Route("BigFileUpload")]
        [RequestSizeLimit(5368709120)]
        public async Task<ActionResult> BigFileUpload()
        {
            var item = await CreateLocalFileFromUploadStream(GetBigStoragePath);
            if (item == null)
                return Result(ApiResponse.BadRequestResult("please select file to upload"));
            var path = GetBigStoragePath(item.Item1);
            var size = new FileInfo(path).Length;
            var hasMiner = _lotusClientSetting.GetMinerByFileSize(size);
            if (string.IsNullOrEmpty(hasMiner))
            {
                System.IO.File.Delete(path);
                return Result(ApiResponse.BadRequestResult("file too large"));
            }
            else
            {
                var hashFileMd5 = await _CloudSpeedManager.GetFileMd5(item.Item1);
                if (hashFileMd5.Success)
                {
                    System.IO.File.Delete(path);
                    return Ok(hashFileMd5.Data.DataKey);
                }
                else
                {
                    await _CloudSpeedManager.CreateFileMd5(item.Item1);
                    await _CloudSpeedManager.CreateFileName(item.Item1, item.Item2);
                    return Ok(item.Item1);
                }
            }
        }

        private async Task<Tuple<string, string>> CreateLocalFileFromUploadStream(Func<string, string> storagePath)
        {
            if (string.IsNullOrEmpty(Request.ContentType))
                return null;
            var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(Request.ContentType).Boundary).Value;
            if (string.IsNullOrEmpty(boundary))
                return null;
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            while (true)
            {
                var section = await reader.ReadNextSectionAsync();
                if (section == null)
                    break;
                if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                {
                    if (contentDisposition.IsFileDisposition())
                    {
                        var key = CreateKey(contentDisposition.FileName.Value);
                        var path = storagePath(key);
                        await WriteFileAsync(section.Body, path);
                        return Tuple.Create<string, string>(key, contentDisposition.FileName.Value);
                    }
                }
            }
            return null;
        }

        private static async Task<int> WriteFileAsync(System.IO.Stream stream, string path)
        {
            int writeCount = 0;
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write, UploadConstants.BigFileWriteSize, true))
            {
                byte[] byteArr = new byte[UploadConstants.BigFileWriteSize];
                int readCount = 0;
                while ((readCount = await stream.ReadAsync(byteArr, 0, byteArr.Length)) > 0)
                {
                    await fileStream.WriteAsync(byteArr, 0, readCount);
                    writeCount += readCount;
                }
            }
            return writeCount;
        }

        private string CreateKey(string fileName)
        {
            return SequentialGuid.NewGuidString();
        }

        private string GetBigStoragePath(string key)
        {
            return _CloudSpeedManager.GetStoragePath(key);
        }

        private string GetRewardPath(string key)
        {
            return _CloudSpeedManager.GetRewardPath(key);
        }
    }
}
