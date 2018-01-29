using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Poll.Core;
using SS.Poll.Model;
using System.Collections.Generic;

namespace SS.Poll.Pages
{
    public class PageLogs : Page
    {
        public Literal LtlMessage;
        public Literal LtlFieldNames;
        public Repeater RptLogs;
        public Button BtnExport;

        public Literal LtlScript;

        private string _apiUrl;
        private int _siteId;
        private int _channelId;
        private int _contentId;
        private string _returnUrl;
        private PollInfo _pollInfo;
        private List<FieldInfo> _fieldInfoList;

        public static string GetRedirectUrl(string apiUrl, int siteId, int channelId, int contentId, string returnUrl)
        {
            return
                Main.FilesApi.GetPluginUrl(
                    $"{nameof(PageLogs)}.aspx?apiUrl={HttpUtility.UrlEncode(apiUrl)}&siteId={siteId}&channelId={channelId}&contentId={contentId}&returnUrl={HttpUtility.UrlEncode(returnUrl)}");
        }

        public string UrlPageResults => PageResults.GetRedirectUrl(_apiUrl, _siteId, _channelId, _contentId, _returnUrl);

        public string UrlReturn => _returnUrl;

        public void Page_Load(object sender, EventArgs e)
        {
            _apiUrl = HttpUtility.UrlDecode(Request.QueryString["apiUrl"]);
            _siteId = Convert.ToInt32(Request.QueryString["siteId"]);
            _channelId = Convert.ToInt32(Request.QueryString["channelId"]);
            _contentId = Convert.ToInt32(Request.QueryString["contentId"]);
            _returnUrl = HttpUtility.UrlDecode(Request.QueryString["returnUrl"]);
            _pollInfo = Main.PollDao.GetPollInfo(_siteId, _channelId, _contentId);
            _fieldInfoList = Main.FieldDao.GetFieldInfoList(_siteId, _channelId, _contentId, false);

            if (!Main.AdminApi.IsSiteAuthorized(_siteId))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
                return;
            }

            if (IsPostBack) return;

            foreach (var fieldInfo in _fieldInfoList)
            {
                LtlFieldNames.Text += $@"<th scope=""col"">{fieldInfo.DisplayName}</th>";
            }

            var totalCount = Main.LogDao.GetCount(_pollInfo.Id);
            var logs = Main.LogDao.GetPollLogInfoList(_pollInfo.Id, totalCount, 30, 0);

            RptLogs.DataSource = logs;
            RptLogs.ItemDataBound += RptLogs_ItemDataBound;
            RptLogs.DataBind();
        }

        public void BtnExport_Click(object sender, EventArgs e)
        {
            var logs = Main.LogDao.GetAllPollLogInfoList(_pollInfo.Id);

            var head = new List<string> { "序号"};
            foreach (var fieldInfo in _fieldInfoList)
            {
                head.Add(fieldInfo.DisplayName);
            }
            head.Add("提交时间");

            var rows = new List<List<string>>();

            var index = 1;
            foreach (var log in logs)
            {
                var row = new List<string>
                {
                    index++.ToString()
                };
                foreach (var fieldInfo in _fieldInfoList)
                {
                    row.Add(log.GetString(fieldInfo.AttributeName));
                }
                row.Add(log.AddDate.ToString("yyyy-MM-dd HH:mm"));

                rows.Add(row);
            }

            var relatedPath = "投票清单.csv";

            CsvUtils.Export(Main.FilesApi.GetPluginPath(relatedPath), head, rows);

            HttpContext.Current.Response.Redirect(Main.FilesApi.GetPluginUrl(relatedPath));
        }

        private void RptLogs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var logInfo = (LogInfo)e.Item.DataItem;

            var ltlValues = (Literal)e.Item.FindControl("ltlValues");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");

            foreach (var fieldInfo in _fieldInfoList)
            {
                ltlValues.Text += $@"<td>{logInfo.GetString(fieldInfo.AttributeName)}</td>";
            }

            ltlAddDate.Text = logInfo.AddDate.ToString("yyyy-MM-dd HH:mm");
        }
    }
}
