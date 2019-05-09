using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Poll.Core;
using SS.Poll.Core.Box;
using SS.Poll.Core.Utils;

namespace SS.Poll.Controllers.Pages
{
    [RoutePrefix("pages/fields")]
    public class PagesFieldsController : ApiController
    {
        private const string Route = "";
        private const string RouteExport = "actions/export";
        private const string RouteImport = "actions/import";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var adminToken = Context.AdminApi.GetAccessToken(request.AdminId, request.AdminName, TimeSpan.FromDays(1));

                var list = new List<object>();
                foreach (var fieldInfo in FieldManager.GetFieldInfoList(pollInfo.Id))
                {
                    list.Add(new
                    {
                        fieldInfo.Id,
                        fieldInfo.Title,
                        InputType = FieldManager.GetFieldTypeText(fieldInfo.FieldType),
                        fieldInfo.Validate,
                        fieldInfo.Taxis
                    });
                }

                return Ok(new
                {
                    Value = list,
                    AdminToken = adminToken
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public IHttpActionResult Delete()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var fieldId = request.GetQueryInt("fieldId");
                FieldManager.Repository.Delete(pollInfo.Id, fieldId);
                FieldManager.ClearCache(pollInfo.Id);

                var list = new List<object>();
                foreach (var fieldInfo in FieldManager.GetFieldInfoList(pollInfo.Id))
                {
                    list.Add(new
                    {
                        fieldInfo.Id,
                        fieldInfo.Title,
                        InputType = FieldManager.GetFieldTypeText(fieldInfo.FieldType),
                        fieldInfo.Validate,
                        fieldInfo.Taxis
                    });
                }

                return Ok(new
                {
                    Value = list
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteExport)]
        public IHttpActionResult Export()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                //var fileName = FieldManager.Export(pollInfo.Id);\

                var fileName = "表单字段.zip";
                var filePath = Context.UtilsApi.GetTemporaryFilesPath(fileName);
                var directoryPath = Context.UtilsApi.GetTemporaryFilesPath("PollFields");

                PollUtils.DeleteDirectoryIfExists(directoryPath);
                PollUtils.CreateDirectoryIfNotExists(directoryPath);

                PollBox.ExportFields(pollInfo.Id, directoryPath);

                Context.UtilsApi.CreateZip(filePath, directoryPath);

                var url = Context.UtilsApi.GetRootUrl($"SiteFiles/TemporaryFiles/{fileName}");

                return Ok(new
                {
                    Value = url
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteImport)]
        public IHttpActionResult Import()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read zip from body");
                    }
                    
                    var filePath = Context.UtilsApi.GetTemporaryFilesPath("poll.zip");
                    PollUtils.DeleteFileIfExists(filePath);

                    if (!PollUtils.EqualsIgnoreCase(Path.GetExtension(postFile.FileName), ".zip"))
                    {
                        return BadRequest("zip file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    var directoryPath = Context.UtilsApi.GetTemporaryFilesPath("poll");
                    PollUtils.DeleteDirectoryIfExists(directoryPath);
                    Context.UtilsApi.ExtractZip(filePath, directoryPath);

                    var isHistoric = PollBox.IsHistoric(directoryPath);
                    PollBox.ImportFields(pollInfo.SiteId, pollInfo.Id, directoryPath, isHistoric);

                    //FieldManager.Import(pollInfo.SiteId, pollInfo.Id, filePath);
                }

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
