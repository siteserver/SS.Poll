using System;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Poll.Core;
using SS.Poll.Core.Box;
using SS.Poll.Core.Models;
using SS.Poll.Core.Utils;

namespace SS.Poll.Controllers.Pages
{
    [RoutePrefix("pages/polls")]
    public class PagesPollsController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsUp = "actions/up";
        private const string RouteActionsDown = "actions/down";
        private const string RouteExport = "actions/export";
        private const string RouteImport = "actions/import";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, PollUtils.PluginId))
                {
                    return Unauthorized();
                }

                var pollInfoList = PollManager.GetPollInfoList(siteId, 0);
                var adminToken = Context.AdminApi.GetAccessToken(request.AdminId, request.AdminName, TimeSpan.FromDays(1));

                return Ok(new
                {
                    Value = pollInfoList,
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

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, PollUtils.PluginId))
                {
                    return Unauthorized();
                }

                var pollId = request.GetQueryInt("pollId");

                PollManager.Repository.Delete(siteId, pollId);

                return Ok(new
                {
                    Value = PollManager.GetPollInfoList(siteId, 0)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Add()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, PollUtils.PluginId))
                {
                    return Unauthorized();
                }

                var pollInfo = new PollInfo
                {
                    SiteId = siteId,
                    Title = request.GetPostString("title"),
                    Description = request.GetPostString("description")
                };

                PollManager.Repository.Insert(pollInfo);

                return Ok(new
                {
                    Value = PollManager.GetPollInfoList(siteId, 0)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(Route)]
        public IHttpActionResult Edit()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, PollUtils.PluginId))
                {
                    return Unauthorized();
                }

                var pollId = request.GetPostInt("pollId");
                var pollInfo = PollManager.GetPollInfo(siteId, pollId);
                pollInfo.Title = request.GetPostString("title");
                pollInfo.Description = request.GetPostString("description");

                PollManager.Repository.Update(pollInfo);

                return Ok(new
                {
                    Value = PollManager.GetPollInfoList(siteId, 0)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsUp)]
        public IHttpActionResult Up()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, PollUtils.PluginId))
                {
                    return Unauthorized();
                }

                var pollId = request.GetPostInt("pollId");

                PollManager.Repository.UpdateTaxisToUp(siteId, pollId);

                return Ok(new
                {
                    Value = PollManager.GetPollInfoList(siteId, 0)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsDown)]
        public IHttpActionResult Down()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, PollUtils.PluginId))
                {
                    return Unauthorized();
                }

                var pollId = request.GetPostInt("pollId");

                PollManager.Repository.UpdateTaxisToDown(siteId, pollId);

                return Ok(new
                {
                    Value = PollManager.GetPollInfoList(siteId, 0)
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

                var fileName = $"{pollInfo.Title}.zip";
                var directoryPath = Context.UtilsApi.GetTemporaryFilesPath("poll");
                PollUtils.DeleteDirectoryIfExists(directoryPath);

                PollBox.ExportPoll(pollInfo.SiteId, directoryPath, pollInfo.Id);

                Context.UtilsApi.CreateZip(Context.UtilsApi.GetTemporaryFilesPath(fileName), directoryPath);

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
                var siteId = request.GetQueryInt("siteId");

                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, PollUtils.PluginId)) return Unauthorized();

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

                    PollBox.ImportPoll(siteId, directoryPath, true);

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
