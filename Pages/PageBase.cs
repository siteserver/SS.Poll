using System;
using System.Web;
using System.Web.UI;
using SS.Poll.Models;
using SS.Poll.Provider;

namespace SS.Poll.Pages
{
    public class PageBase : Page
    {
        public int SiteId { get; private set; }
        public int ChannelId { get; private set; }
        public int ContentId { get; private set; }
        public string ReturnUrl { get; private set; }

        public PollInfo PollInfo { get; private set; }

        public string PageItemsUrl => PageItems.GetRedirectUrl(SiteId, ChannelId, ContentId, ReturnUrl);

        public string PageFieldsUrl => PageFields.GetRedirectUrl(SiteId, ChannelId, ContentId, ReturnUrl);

        public string PageSettingsUrl => PageSettings.GetRedirectUrl(SiteId, ChannelId, ContentId, ReturnUrl);

        public string PageResultsUrl => PageResults.GetRedirectUrl(SiteId, ChannelId, ContentId, ReturnUrl);

        public string PageLogsUrl => PageLogs.GetRedirectUrl(SiteId, ChannelId, ContentId, ReturnUrl);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var request = SiteServer.Plugin.Context.GetCurrentRequest();
            SiteId = request.GetQueryInt("siteId");
            ChannelId = request.GetQueryInt("channelId");
            ContentId = request.GetQueryInt("contentId");
            ReturnUrl = HttpUtility.UrlDecode(request.GetQueryString("returnUrl"));

            if (!request.AdminPermissions.HasSitePermissions(SiteId, Main.PluginId))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
            }

            PollInfo = PollDao.GetPollInfo(SiteId, ChannelId, ContentId);
            if (PollInfo != null) return;

            PollInfo = new PollInfo
            {
                SiteId = SiteId,
                ChannelId = ChannelId,
                ContentId = ContentId,
                IsImage = true,
                IsUrl = false,
                IsTimeout = false,
                IsCheckbox = true,
                TimeToStart=DateTime.Now,
                TimeToEnd=DateTime.Now
            };
            PollInfo.Id = PollDao.Insert(PollInfo);
        }
    }
}
