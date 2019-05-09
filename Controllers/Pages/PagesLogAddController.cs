using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Poll.Core;
using SS.Poll.Core.Models;
using SS.Poll.Core.Utils;

namespace SS.Poll.Controllers.Pages
{
    [RoutePrefix("pages/logAdd")]
    public class PagesLogAddController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var logId = request.GetQueryInt("logId");
                var fieldInfoList = FieldManager.GetFieldInfoList(pollInfo.Id);

                if (logId > 0)
                {
                    var logInfo = LogManager.Repository.GetLogInfo(logId);
                    foreach (var fieldInfo in fieldInfoList)
                    {
                        if (fieldInfo.FieldType == InputType.CheckBox.Value || fieldInfo.FieldType == InputType.SelectMultiple.Value)
                        {
                            fieldInfo.Value = PollUtils.JsonDeserialize<List<string>>(logInfo.Get<string>(fieldInfo.Title));
                        }
                        else if (fieldInfo.FieldType == InputType.Date.Value || fieldInfo.FieldType == InputType.DateTime.Value)
                        {
                            fieldInfo.Value = logInfo.Get<DateTime>(fieldInfo.Title);
                        }
                        else
                        {
                            fieldInfo.Value = logInfo.Get<string>(fieldInfo.Title);
                        }
                    }
                }

                return Ok(new
                {
                    Value = fieldInfoList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var logId = request.GetPostInt("logId");

                var logInfo = logId > 0
                    ? LogManager.Repository.GetLogInfo(logId)
                    : new LogInfo
                    {
                        PollId = pollInfo.Id,
                        AddDate = DateTime.Now
                    };
                var fieldInfoList = FieldManager.GetFieldInfoList(pollInfo.Id);
                foreach (var fieldInfo in fieldInfoList)
                {
                    if (request.IsPostExists(fieldInfo.Title))
                    {
                        var value = request.GetPostString(fieldInfo.Title);
                        if (fieldInfo.FieldType == InputType.Date.Value || fieldInfo.FieldType == InputType.DateTime.Value)
                        {
                            var dt = PollUtils.ToDateTime(request.GetPostString(fieldInfo.Title));
                            logInfo.Set(fieldInfo.Title, dt.ToLocalTime());
                        }

                        else
                        {
                            logInfo.Set(fieldInfo.Title, value);
                        }
                    }
                    
                }

                if (logId == 0)
                {
                    logInfo.Id = LogManager.Repository.Insert(pollInfo, logInfo);
                    NotifyManager.SendNotify(pollInfo, fieldInfoList, logInfo);
                }
                else
                {
                    LogManager.Repository.Update(logInfo);
                }

                return Ok(new{});
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
