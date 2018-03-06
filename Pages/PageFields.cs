using System;
using System.Web;
using System.Web.UI.WebControls;
using SS.Poll.Core;
using SS.Poll.Models;

namespace SS.Poll.Pages
{
    public class PageFields : PageBase
    {
        public Literal LtlMessage;
        public DataGrid DgContents;

        public Button BtnAddField;
        public Button BtnImport;
        public Button BtnExport;

        public static string GetRedirectUrl(int siteId, int channelId, int contentId, string returnUrl)
        {
            return $"{nameof(PageFields)}.aspx?siteId={siteId}&channelId={channelId}&contentId={contentId}&returnUrl={HttpUtility.UrlEncode(returnUrl)}";
        }

        private string PageUrl => GetRedirectUrl(SiteId, ChannelId, ContentId, ReturnUrl);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            //删除样式
            if (!string.IsNullOrEmpty(Request.QueryString["delete"]))
            {
                var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
                Main.FieldDao.Delete(fieldId);
                LtlMessage.Text = Utils.GetMessageHtml("字段删除成功！", true);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["taxis"]))
            {
                var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
                var direction = Request.QueryString["direction"];

                switch (direction.ToUpper())
                {
                    case "UP":
                        Main.FieldDao.TaxisUp(fieldId);
                        break;
                    case "DOWN":
                        Main.FieldDao.TaxisDown(fieldId);
                        break;
                }
                LtlMessage.Text = Utils.GetMessageHtml("排序成功！", true);
            }

            var fieldList = Main.FieldDao.GetFieldInfoList(SiteId, ChannelId, ContentId, false);

            DgContents.DataSource = fieldList;
            DgContents.ItemDataBound += DgContents_ItemDataBound;
            DgContents.DataBind();

            BtnAddField.Attributes.Add("onclick", ModalFieldAdd.GetOpenWindowString(SiteId, ChannelId, ContentId, 0));
        }

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var fieldInfo = (FieldInfo) e.Item.DataItem;
            var settings = new FieldSettings(fieldInfo.FieldSettings);

            var ltlAttributeName = (Literal) e.Item.FindControl("ltlAttributeName");
            var ltlDisplayName = (Literal) e.Item.FindControl("ltlDisplayName");
            var ltlFieldType = (Literal) e.Item.FindControl("ltlFieldType");
            var ltlIsDisabled = (Literal) e.Item.FindControl("ltlIsDisabled");
            var ltlValidate = (Literal) e.Item.FindControl("ltlValidate");
            var ltlActions = (Literal) e.Item.FindControl("ltlActions");

            ltlAttributeName.Text = fieldInfo.AttributeName;

            ltlDisplayName.Text = fieldInfo.DisplayName;
            ltlFieldType.Text = FieldTypeUtils.GetText(FieldTypeUtils.GetEnumType(fieldInfo.FieldType));

            ltlIsDisabled.Text = fieldInfo.IsDisabled
                ? @"<span class=""label label-danger"">已禁用</span>"
                : @"<span class=""label label-primary"">已启用</span>";
            ltlValidate.Text = ValidateTypeUtils.GetValidateInfo(settings.IsRequired,
                settings.MinNum, settings.MaxNum, settings.ValidateType);

            ltlActions.Text = $@"
<a class=""m-r-10"" href=""{PageUrl}&taxis={true}&direction=Up&fieldId={fieldInfo
                .Id}"">上升</a>
<a class=""m-r-10"" href=""{PageUrl}&taxis={true}&direction=Down&fieldId={fieldInfo
                .Id}"">下降</a>
<a class=""m-r-10"" href=""javascript"" onclick=""{ModalFieldAdd.GetOpenWindowString(SiteId, ChannelId, ContentId, fieldInfo.Id)}"">编辑</a>
<a class=""m-r-10"" href=""javascript"" onclick=""{ModalFieldValidate.GetOpenWindowString(SiteId, ChannelId, ContentId, fieldInfo.Id)}"">验证规则</a>
<a class=""m-r-10"" href=""{PageUrl}&delete={true}&fieldId={fieldInfo.Id}"">删除</a>";
        }
    }
}
