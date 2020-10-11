using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Autofac;
using CloudSpeed.Entities;
using CloudSpeed.Services;
using CloudSpeed.Settings;
using CloudSpeed.Repositories;
using System.Threading.Tasks;
using CloudSpeed.Web.Responses;
using CloudSpeed.Web.Models;
using CloudSpeed.Web.Requests;
using CloudSpeed.Sdk;
using CloudSpeed.AdminWeb.Requests;

namespace CloudSpeed.AdminWeb.Managers
{
    public class FilesManager
    {
        public FilesManager()
        {

        }

        public Task GetDashboardInfo(DashboardFilesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task GetFiles(FilesGetListRequest request)
        {
            throw new NotImplementedException();
        }

        public Task CreateImport(FilesImportRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
