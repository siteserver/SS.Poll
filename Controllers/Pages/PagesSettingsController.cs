using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Poll.Core;
using SS.Poll.Core.Models;
using SS.Poll.Core.Utils;

namespace SS.Poll.Controllers.Pages
{
    [RoutePrefix("pages/settings")]
    public class PagesSettingsController : ApiController
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

                var fieldInfoList = FieldManager.GetFieldInfoList(pollInfo.Id);

                var attributeNames = PollManager.GetAllAttributeNames(fieldInfoList);
                var administratorSmsNotifyKeys =
                    PollUtils.StringCollectionToStringList(pollInfo.AdministratorSmsNotifyKeys);
                var userSmsNotifyKeys =
                    PollUtils.StringCollectionToStringList(pollInfo.UserSmsNotifyKeys);

                return Ok(new
                {
                    Value = pollInfo,
                    FieldInfoList = fieldInfoList,
                    AttributeNames = attributeNames,
                    AdministratorSmsNotifyKeys = administratorSmsNotifyKeys,
                    UserSmsNotifyKeys = userSmsNotifyKeys
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

                var type = request.GetPostString("type");
                if (PollUtils.EqualsIgnoreCase(type, nameof(PollInfo.IsClosed)))
                {
                    pollInfo.IsClosed = request.GetPostBool(nameof(PollInfo.IsClosed));
                    PollManager.Repository.Update(pollInfo);
                }
                else if (PollUtils.EqualsIgnoreCase(type, nameof(PollInfo.Title)))
                {
                    pollInfo.Title = request.GetPostString(nameof(PollInfo.Title));
                    PollManager.Repository.Update(pollInfo);
                }
                else if (PollUtils.EqualsIgnoreCase(type, nameof(PollInfo.Description)))
                {
                    pollInfo.Description = request.GetPostString(nameof(PollInfo.Description));
                    PollManager.Repository.Update(pollInfo);
                }
                else if (PollUtils.EqualsIgnoreCase(type, nameof(PollInfo.IsImage)))
                {
                    pollInfo.IsImage = request.GetPostBool(nameof(PollInfo.IsImage));
                    pollInfo.IsUrl = request.GetPostBool(nameof(PollInfo.IsUrl));
                    PollManager.Repository.Update(pollInfo);
                }
                else if (PollUtils.EqualsIgnoreCase(type, nameof(PollInfo.IsCheckbox)))
                {
                    pollInfo.IsCheckbox = request.GetPostBool(nameof(PollInfo.IsCheckbox));
                    pollInfo.CheckboxMin = PollUtils.ToInt(request.GetPostString(nameof(PollInfo.CheckboxMin)));
                    pollInfo.CheckboxMax = PollUtils.ToInt(request.GetPostString(nameof(PollInfo.CheckboxMax)));
                    PollManager.Repository.Update(pollInfo);
                }
                else if (PollUtils.EqualsIgnoreCase(type, nameof(PollInfo.IsTimeout)))
                {
                    pollInfo.IsTimeout = request.GetPostBool(nameof(PollInfo.IsTimeout));
                    pollInfo.TimeToStart = PollUtils.ToDateTime(request.GetPostString(nameof(PollInfo.TimeToStart)));
                    pollInfo.TimeToEnd = PollUtils.ToDateTime(request.GetPostString(nameof(PollInfo.TimeToEnd)));
                    PollManager.Repository.Update(pollInfo);
                }
                else if (PollUtils.EqualsIgnoreCase(type, nameof(PollInfo.IsCaptcha)))
                {
                    pollInfo.IsCaptcha = request.GetPostBool(nameof(PollInfo.IsCaptcha));
                    PollManager.Repository.Update(pollInfo);
                }
                else if (PollUtils.EqualsIgnoreCase(type, nameof(PollInfo.IsAdministratorSmsNotify)))
                {
                    pollInfo.IsAdministratorSmsNotify = request.GetPostBool(nameof(PollInfo.IsAdministratorSmsNotify));
                    pollInfo.AdministratorSmsNotifyTplId = request.GetPostString(nameof(PollInfo.AdministratorSmsNotifyTplId));
                    pollInfo.AdministratorSmsNotifyKeys = request.GetPostString(nameof(PollInfo.AdministratorSmsNotifyKeys));
                    pollInfo.AdministratorSmsNotifyMobile = request.GetPostString(nameof(PollInfo.AdministratorSmsNotifyMobile));

                    PollManager.Repository.Update(pollInfo);
                }
                else if (PollUtils.EqualsIgnoreCase(type, nameof(PollInfo.IsAdministratorMailNotify)))
                {
                    pollInfo.IsAdministratorMailNotify = request.GetPostBool(nameof(PollInfo.IsAdministratorMailNotify));
                    pollInfo.AdministratorMailNotifyAddress = request.GetPostString(nameof(PollInfo.AdministratorMailNotifyAddress));

                    PollManager.Repository.Update(pollInfo);
                }
                else if (PollUtils.EqualsIgnoreCase(type, nameof(PollInfo.IsUserSmsNotify)))
                {
                    pollInfo.IsUserSmsNotify = request.GetPostBool(nameof(PollInfo.IsUserSmsNotify));
                    pollInfo.UserSmsNotifyTplId = request.GetPostString(nameof(PollInfo.UserSmsNotifyTplId));
                    pollInfo.UserSmsNotifyKeys = request.GetPostString(nameof(PollInfo.UserSmsNotifyKeys));
                    pollInfo.UserSmsNotifyMobileName = request.GetPostString(nameof(PollInfo.UserSmsNotifyMobileName));

                    PollManager.Repository.Update(pollInfo);
                }

                return Ok(new { });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
