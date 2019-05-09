using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Poll.Core;
using SS.Poll.Core.Utils;

namespace SS.Poll.Controllers.Pages
{
    [RoutePrefix("pages/logs")]
    public class PagesLogsController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsExport = "actions/export";
        private const string RouteActionsVisible = "actions/visible";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var fieldInfoList = FieldManager.GetFieldInfoList(pollInfo.Id);
                var listAttributeNames = PollUtils.StringCollectionToStringList(pollInfo.ListAttributeNames);
                var allAttributeNames = PollManager.GetAllAttributeNames(fieldInfoList);

                var pages = Convert.ToInt32(Math.Ceiling((double)pollInfo.TotalCount / PollUtils.PageSize));
                if (pages == 0) pages = 1;
                var page = request.GetQueryInt("page", 1);
                if (page > pages) page = pages;
                var logInfoList = LogManager.Repository.GetLogInfoList(pollInfo, page);

                var logs = new List<Dictionary<string, object>>();
                foreach (var logInfo in logInfoList)
                {
                    logs.Add(LogManager.GetDict(fieldInfoList, logInfo));
                }

                return Ok(new
                {
                    Value = logs,
                    Count = pollInfo.TotalCount,
                    Pages = pages,
                    Page = page,
                    FieldInfoList = fieldInfoList,
                    AllAttributeNames = allAttributeNames,
                    ListAttributeNames = listAttributeNames
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

                var logId = request.GetQueryInt("logId");
                var logInfo = LogManager.Repository.GetLogInfo(logId);
                if (logInfo == null) return NotFound();

                LogManager.Repository.Delete(pollInfo, logInfo);

                var pages = Convert.ToInt32(Math.Ceiling((double)pollInfo.TotalCount / PollUtils.PageSize));
                if (pages == 0) pages = 1;
                var page = request.GetQueryInt("page", 1);
                if (page > pages) page = pages;
                var logInfoList = LogManager.Repository.GetLogInfoList(pollInfo, page);

                var logs = new List<IDictionary<string, object>>();
                foreach (var info in logInfoList)
                {
                    logs.Add(info.ToDictionary());
                }

                return Ok(new
                {
                    Value = logs,
                    Count = pollInfo.TotalCount,
                    Pages = pages,
                    Page = page
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsExport)]
        public IHttpActionResult Export()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var fieldInfoList = FieldManager.GetFieldInfoList(pollInfo.Id);
                var logs = LogManager.Repository.GetLogInfoList(pollInfo.Id, 0, pollInfo.TotalCount);

                var head = new List<string> { "编号" };
                foreach (var fieldInfo in fieldInfoList)
                {
                    head.Add(fieldInfo.Title);
                }
                head.Add("添加时间");

                var rows = new List<List<string>>();
                
                foreach (var log in logs)
                {
                    var row = new List<string>
                    {
                        log.Guid
                    };
                    foreach (var fieldInfo in fieldInfoList)
                    {
                        row.Add(LogManager.GetValue(fieldInfo, log));
                    }

                    if (log.AddDate.HasValue)
                    {
                        row.Add(log.AddDate.Value.ToString("yyyy-MM-dd HH:mm"));
                    }

                    rows.Add(row);
                }

                var fileName = $"{pollInfo.Title}.csv";
                CsvUtils.Export(Context.PluginApi.GetPluginPath(PollUtils.PluginId, fileName), head, rows);
                var downloadUrl = Context.PluginApi.GetPluginUrl(PollUtils.PluginId, fileName);

                return Ok(new
                {
                    Value = downloadUrl
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsVisible)]
        public IHttpActionResult Visible()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var attributeName = request.GetPostString("attributeName");

                var attributeNames = PollUtils.StringCollectionToStringList(pollInfo.ListAttributeNames);
                if (attributeNames.Contains(attributeName))
                {
                    attributeNames.Remove(attributeName);
                }
                else
                {
                    attributeNames.Add(attributeName);
                }

                pollInfo.ListAttributeNames = PollUtils.ObjectCollectionToString(attributeNames);
                PollManager.Repository.Update(pollInfo);

                return Ok(new
                {
                    Value = attributeNames
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
