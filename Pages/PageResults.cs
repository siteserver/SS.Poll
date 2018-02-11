using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Poll.Core;
using SS.Poll.Model;
using System.Collections.Generic;

namespace SS.Poll.Pages
{
    public class PageResults : Page
    {
        public Literal LtlMessage;
        public Repeater RptItems;
        public Button BtnExport;

        public Literal LtlScript;

        private string _apiUrl;
        private int _siteId;
        private int _channelId;
        private int _contentId;
        private string _returnUrl;
        private PollInfo _pollInfo;
        private int _totalCount;

        public static string GetRedirectUrl(string apiUrl, int siteId, int channelId, int contentId, string returnUrl)
        {
            return
                Main.Instance.PluginApi.GetPluginUrl(
                    $"{nameof(PageResults)}.aspx?apiUrl={HttpUtility.UrlEncode(apiUrl)}&siteId={siteId}&channelId={channelId}&contentId={contentId}&returnUrl={HttpUtility.UrlEncode(returnUrl)}");
        }

        public string UrlPageLogs => PageLogs.GetRedirectUrl(_apiUrl, _siteId, _channelId, _contentId, _returnUrl);

        public string UrlReturn => _returnUrl;

        public void Page_Load(object sender, EventArgs e)
        {
            _apiUrl = HttpUtility.UrlDecode(Request.QueryString["apiUrl"]);
            _siteId = Convert.ToInt32(Request.QueryString["siteId"]);
            _channelId = Convert.ToInt32(Request.QueryString["channelId"]);
            _contentId = Convert.ToInt32(Request.QueryString["contentId"]);
            _returnUrl = HttpUtility.UrlDecode(Request.QueryString["returnUrl"]);
            _pollInfo = Main.PollDao.GetPollInfo(_siteId, _channelId, _contentId);

            if (!Main.Instance.AdminApi.IsSiteAuthorized(_siteId))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
                return;
            }

            if (IsPostBack) return;

            var items = Main.ItemDao.GetItemInfoList(_pollInfo.Id, out _totalCount);

            RptItems.DataSource = items;
            RptItems.ItemDataBound += RptItems_ItemDataBound;
            RptItems.DataBind();
        }

        public void BtnExport_Click(object sender, EventArgs e)
        {
            int totalCount;
            var itemInfoList = Main.ItemDao.GetItemInfoList(_pollInfo.Id, out totalCount);

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

            if (_pollInfo.IsImage)
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
