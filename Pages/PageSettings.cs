using System;
using System.Web;
using System.Web.UI.WebControls;
using SS.Poll.Controls;
using SS.Poll.Core;

namespace SS.Poll.Pages
{
    public class PageSettings : PageBase
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

        public static string GetRedirectUrl(int siteId, int channelId, int contentId, string returnUrl)
        {
            return $"{nameof(PageSettings)}.aspx?siteId={siteId}&channelId={channelId}&contentId={contentId}&returnUrl={HttpUtility.UrlEncode(returnUrl)}";
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            CbIsImage.Checked = PollInfo.IsImage;
            CbIsUrl.Checked = PollInfo.IsUrl;
            CbIsTimeout.Checked = PollInfo.IsTimeout;
            CbIsTimeout.AutoPostBack = true;
            PhCheckbox.Visible = CbIsCheckbox.Checked = PollInfo.IsCheckbox;
            CbIsCheckbox.AutoPostBack = true;
            PhTimeout.Visible = PollInfo.IsTimeout;
            TbTimeToStart.DateTime = PollInfo.TimeToStart;
            TbTimeToEnd.DateTime = PollInfo.TimeToEnd;
            TbCheckboxMin.Text = PollInfo.CheckboxMin.ToString();
            TbCheckboxMax.Text = PollInfo.CheckboxMax.ToString();
            CbIsProfile.Checked = PollInfo.IsProfile;
            CbIsResult.Checked = PollInfo.IsResult;
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
            PollInfo.IsImage = CbIsImage.Checked;
            PollInfo.IsUrl = CbIsUrl.Checked;
            PollInfo.IsTimeout = CbIsTimeout.Checked;
            PollInfo.IsCheckbox = CbIsCheckbox.Checked;
            PollInfo.TimeToStart = TbTimeToStart.DateTime;
            PollInfo.TimeToEnd = TbTimeToEnd.DateTime;
            PollInfo.CheckboxMin = Convert.ToInt32(TbCheckboxMin.Text);
            PollInfo.CheckboxMax = Convert.ToInt32(TbCheckboxMax.Text);
            PollInfo.IsProfile = CbIsProfile.Checked;
            PollInfo.IsResult = CbIsResult.Checked;

            Main.PollDao.Update(PollInfo);

            LtlMessage.Text = Utils.GetMessageHtml("设置保存成功！", true);
        }
    }
}
