using System;
using System.Web;
using System.Web.UI.WebControls;
using SS.Poll.Core;
using SS.Poll.Models;
using SS.Poll.Provider;

namespace SS.Poll.Pages
{
    public class PageItems : PageBase
    {
        public Literal LtlMessage;
        public DataGrid DgContents;

        public Button BtnAddItem;
        public Button BtnImport;
        public Button BtnExport;

        public static string GetRedirectUrl(int siteId, int channelId, int contentId, string returnUrl)
        {
            return $"{nameof(PageItems)}.aspx?siteId={siteId}&channelId={channelId}&contentId={contentId}&returnUrl={HttpUtility.UrlEncode(returnUrl)}";
        }

        private string PageUrl => GetRedirectUrl(SiteId, ChannelId, ContentId, ReturnUrl);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            //删除样式
            if (!string.IsNullOrEmpty(Request.QueryString["delete"]))
            {
                var itemId = Convert.ToInt32(Request.QueryString["itemId"]);
                ItemDao.Delete(itemId);
                LtlMessage.Text = Utils.GetMessageHtml("投票项删除成功！", true);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["taxis"]))
            {
                var itemId = Convert.ToInt32(Request.QueryString["itemId"]);
                var direction = Request.QueryString["direction"];

                switch (direction.ToUpper())
                {
                    case "UP":
                        ItemDao.TaxisUp(itemId);
                        break;
                    case "DOWN":
                        ItemDao.TaxisDown(itemId);
                        break;
                }
                LtlMessage.Text = Utils.GetMessageHtml("排序成功！", true);
            }

            var itemList = ItemDao.GetItemInfoList(SiteId, ChannelId, ContentId);

            DgContents.DataSource = itemList;
            DgContents.ItemDataBound += DgContents_ItemDataBound;
            DgContents.DataBind();

            BtnAddItem.Attributes.Add("onclick", ModalItemAdd.GetOpenWindowString(SiteId, ChannelId, ContentId, 0));
        }

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var itemInfo = (ItemInfo) e.Item.DataItem;

            var ltlTitle = (Literal) e.Item.FindControl("ltlTitle");
            var ltlSubTitle = (Literal) e.Item.FindControl("ltlSubTitle");
            var ltlActions = (Literal) e.Item.FindControl("ltlActions");

            ltlTitle.Text = itemInfo.Title;
            ltlSubTitle.Text = itemInfo.SubTitle;

            ltlActions.Text = $@"
<a class=""m-r-10"" href=""{PageUrl}&taxis={true}&direction=Up&itemId={itemInfo.Id}"">上升</a>
<a class=""m-r-10"" href=""{PageUrl}&taxis={true}&direction=Down&itemId={itemInfo.Id}"">下降</a>
<a class=""m-r-10"" href=""javascript"" onclick=""{ModalItemAdd.GetOpenWindowString(SiteId, ChannelId, ContentId, itemInfo.Id)}"">编辑</a>
<a class=""m-r-10"" href=""{PageUrl}&delete={true}&itemId={itemInfo.Id}"">删除</a>";
        }
    }
}
