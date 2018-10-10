using System;
using System.Web;
using System.Web.UI;
using SS.Poll.Models;

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

            SiteId = Convert.ToInt32(Request.QueryString["siteId"]);
            ChannelId = Convert.ToInt32(Request.QueryString["channelId"]);
            ContentId = Convert.ToInt32(Request.QueryString["contentId"]);
            ReturnUrl = HttpUtility.UrlDecode(Request.QueryString["returnUrl"]);

            if (!Main.Instance.Request.AdminPermissions.HasSitePermissions(SiteId, Main.Instance.Id))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
            }

            PollInfo = Main.PollDao.GetPollInfo(SiteId, ChannelId, ContentId);
            if (PollInfo != null) return;

            PollInfo = new PollInfo
            {
                SiteId = SiteId,
                ChannelId = ChannelId,
                ContentId = ContentId,
                IsImage = true,
                IsUrl = false,
                IsTimeout = false,
                IsCheckbox = true
            };
            PollInfo.Id = Main.PollDao.Insert(PollInfo);
        }
    }
}
