using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Poll.Controls;
using SS.Poll.Core;

namespace SS.Poll.Pages
{
    public class PageSettings : Page
    {
        public Literal LtlMessage;

        public CheckBox CbIsImage;
        public CheckBox CbIsUrl;
        public CheckBox CbIsTimeout;
        public CheckBox CbIsCheckbox;
        public PlaceHolder PhTimeout;
        public DateTimeTextBox TbTimeToStart;
        public DateTimeTextBox TbTimeToEnd;
        public PlaceHolder PhCheckbox;
        public TextBox TbCheckboxMin;
        public TextBox TbCheckboxMax;
        public CheckBox CbIsProfile;
        public CheckBox CbIsResult;

        public Literal LtlScript;

        private string _apiUrl;
        private int _siteId;
        private int _channelId;
        private int _contentId;
        private string _returnUrl;

        public static string GetRedirectUrl(string apiUrl, int siteId, int channelId, int contentId, string returnUrl)
        {
            return
                Main.FilesApi.GetPluginUrl(
                    $"{nameof(PageSettings)}.aspx?apiUrl={HttpUtility.UrlEncode(apiUrl)}&siteId={siteId}&channelId={channelId}&contentId={contentId}&returnUrl={HttpUtility.UrlEncode(returnUrl)}");
        }

        public string UrlEditItems => Main.FilesApi.GetPluginUrl($"build/index.html?pageType=edit_items&apiUrl={HttpUtility.UrlEncode(_apiUrl)}&siteId={_siteId}&channelId={_channelId}&contentId={_contentId}&returnUrl={HttpUtility.UrlEncode(_returnUrl)}");

        public string UrlEditFields
            => PageFields.GetRedirectUrl(_apiUrl, _siteId, _channelId, _contentId, _returnUrl);

        public string UrlReturn => _returnUrl;

        public void Page_Load(object sender, EventArgs e)
        {
            _apiUrl = HttpUtility.UrlDecode(Request.QueryString["apiUrl"]);
            _siteId = Convert.ToInt32(Request.QueryString["siteId"]);
            _channelId = Convert.ToInt32(Request.QueryString["channelId"]);
            _contentId = Convert.ToInt32(Request.QueryString["contentId"]);
            _returnUrl = HttpUtility.UrlDecode(Request.QueryString["returnUrl"]);

            if (!Main.AdminApi.IsSiteAuthorized(_siteId))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
                return;
            }

            if (IsPostBack) return;

            var pollInfo = Main.PollDao.GetPollInfo(_siteId, _channelId, _contentId);

            CbIsImage.Checked = pollInfo.IsImage;
            CbIsUrl.Checked = pollInfo.IsUrl;
            CbIsTimeout.Checked = pollInfo.IsTimeout;
            CbIsTimeout.AutoPostBack = true;
            PhCheckbox.Visible = CbIsCheckbox.Checked = pollInfo.IsCheckbox;
            CbIsCheckbox.AutoPostBack = true;
            PhTimeout.Visible = pollInfo.IsTimeout;
            TbTimeToStart.DateTime = pollInfo.TimeToStart;
            TbTimeToEnd.DateTime = pollInfo.TimeToEnd;
            TbCheckboxMin.Text = pollInfo.CheckboxMin.ToString();
            TbCheckboxMax.Text = pollInfo.CheckboxMax.ToString();
            CbIsProfile.Checked = pollInfo.IsProfile;
            CbIsResult.Checked = pollInfo.IsResult;
        }

        public void CbIsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            PhCheckbox.Visible = CbIsCheckbox.Checked;
        }

        public void CbIsTimeout_CheckedChanged(object sender, EventArgs e)
        {
            PhTimeout.Visible = CbIsTimeout.Checked;
        }

        public void BtnSubmit_Click(object sender, EventArgs e)
        {
            var pollInfo = Main.PollDao.GetPollInfo(_siteId, _channelId, _contentId);

            pollInfo.IsImage = CbIsImage.Checked;
            pollInfo.IsUrl = CbIsUrl.Checked;
            pollInfo.IsTimeout = CbIsTimeout.Checked;
            pollInfo.IsCheckbox = CbIsCheckbox.Checked;
            pollInfo.TimeToStart = TbTimeToStart.DateTime;
            pollInfo.TimeToEnd = TbTimeToEnd.DateTime;
            pollInfo.CheckboxMin = Convert.ToInt32(TbCheckboxMin.Text);
            pollInfo.CheckboxMax = Convert.ToInt32(TbCheckboxMax.Text);
            pollInfo.IsProfile = CbIsProfile.Checked;
            pollInfo.IsResult = CbIsResult.Checked;

            Main.PollDao.Update(pollInfo);

            LtlMessage.Text = Utils.GetMessageHtml("设置保存成功！", true);
        }
    }
}
