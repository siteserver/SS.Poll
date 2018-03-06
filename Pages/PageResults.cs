using System;
using System.Web;
using System.Web.UI.WebControls;
using SS.Poll.Core;
using System.Collections.Generic;
using SS.Poll.Models;

namespace SS.Poll.Pages
{
    public class PageResults : PageBase
    {
        public Literal LtlMessage;
        public Repeater RptItems;
        public Button BtnExport;

        public Literal LtlScript;

        private int _totalCount;

        public static string GetRedirectUrl(int siteId, int channelId, int contentId, string returnUrl)
        {
            return $"{nameof(PageResults)}.aspx?siteId={siteId}&channelId={channelId}&contentId={contentId}&returnUrl={HttpUtility.UrlEncode(returnUrl)}";
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            var items = Main.ItemDao.GetItemInfoList(PollInfo.SiteId, PollInfo.ChannelId, PollInfo.ContentId, out _totalCount);

            RptItems.DataSource = items;
            RptItems.ItemDataBound += RptItems_ItemDataBound;
            RptItems.DataBind();
        }

        public void BtnExport_Click(object sender, EventArgs e)
        {
            int totalCount;
            var itemInfoList = Main.ItemDao.GetItemInfoList(PollInfo.SiteId, PollInfo.ChannelId, PollInfo.ContentId, out totalCount);

            var head = new List<string> { "序号", "标题", "票数", "占比" };

            var rows = new List<List<string>>();

            var index = 1;
            foreach (var itemInfo in itemInfoList)
            {
                double percent;
                if (totalCount == 0)
                {
                    percent = 0;
                }
                else
                {
                    var d = Convert.ToDouble(itemInfo.Count) / Convert.ToDouble(totalCount) * 100;
                    percent = Math.Round(d, 2);
                }
                var row = new List<string>
                        {
                            index++.ToString(),
                            itemInfo.Title,
                            itemInfo.Count.ToString(),
                            percent + "%"
                        };

                rows.Add(row);
            }

            var relatedPath = "投票统计.csv";

            CsvUtils.Export(Main.Instance.PluginApi.GetPluginPath(relatedPath), head, rows);

            HttpContext.Current.Response.Redirect(Main.Instance.PluginApi.GetPluginUrl(relatedPath));
        }

        private void RptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var itemInfo = (ItemInfo)e.Item.DataItem;

            var ltlImage = (Literal)e.Item.FindControl("ltlImage");
            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlSubTitle = (Literal)e.Item.FindControl("ltlSubTitle");
            var ltlSummary = (Literal)e.Item.FindControl("ltlSummary");
            var ltlProgress = (Literal)e.Item.FindControl("ltlProgress");

            double percent;
            if (_totalCount == 0)
            {
                percent = 0;
            }
            else
            {
                var d = Convert.ToDouble(itemInfo.Count) / Convert.ToDouble(_totalCount) * 100;
                percent = Math.Round(d, 2);
            }

            if (PollInfo.IsImage)
            {
                ltlImage.Text = $@"<img src=""{itemInfo.ImageUrl}"" class=""img-responsive img-circle"" style=""height: 72px;width: 72px;float: left;"">";
            }
            ltlTitle.Text = itemInfo.Title;
            ltlSubTitle.Text = itemInfo.SubTitle;
            ltlSummary.Text = $"票数：{itemInfo.Count}， 占比：{percent}%";
            ltlProgress.Text = $@"
<div class=""progress-bar progress-bar-primary"" role=""progressbar"" aria-valuenow=""60"" aria-valuemin=""0"" aria-valuemax=""100"" style=""width: {percent}%;"">
    <span class=""sr-only"">{percent}% Complete</span>
</div>";
        }
    }
}
