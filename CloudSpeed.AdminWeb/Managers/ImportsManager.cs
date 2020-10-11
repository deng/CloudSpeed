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
    public class ImportsManager
    {
        public ImportsManager()
        {

        }

        public Task GetImports(ImportsGetListRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
