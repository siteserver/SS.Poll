using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SS.Poll.Core;
using SS.Poll.Models;

namespace SS.Poll.Pages
{
    public class ModalItemAdd : PageBase
    {
        public Literal LtlMessage;

        public TextBox TbTitle;
        public TextBox TbSubTitle;
        public PlaceHolder PhImageUrl;
        public Literal LtlImageUrl;
        public HiddenField HfImageUrl;
        public PlaceHolder PhLinkUrl;
        public TextBox TbLinkUrl;
        public TextBox TbCount;

        private int _itemId;

        public string UploadUrl => $"{nameof(ModalItemAdd)}.aspx?siteId={SiteId}&channelId={ChannelId}&contentId={ContentId}&itemId={_itemId}&upload=true";

        public static string GetOpenWindowString(int siteId, int channelId, int contentId, int itemId)
        {
            return LayerUtils.GetOpenScript(itemId > 0 ? "编辑投票项" : "新增投票项", $"{nameof(ModalItemAdd)}.aspx?siteId={siteId}&channelId={channelId}&contentId={contentId}&itemId={itemId}", 650, 600);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _itemId = Convert.ToInt32(Request.QueryString["itemId"]);

            if (!string.IsNullOrEmpty(Request.QueryString["upload"]))
            {
                var attributes = Upload();
                var json = Utils.JsonSerialize(attributes);
                Response.Write(json);
                Response.End();
                return;
            }

            if (IsPostBack) return;

            var itemInfo = _itemId > 0 ? Main.ItemDao.GetItemInfo(_itemId) : new ItemInfo();

            TbTitle.Text = itemInfo.Title;
            TbSubTitle.Text = itemInfo.SubTitle;
            LtlImageUrl.Text = !string.IsNullOrEmpty(itemInfo.ImageUrl)
                ? $@"<img id=""imageUrl"" src=""{itemInfo.ImageUrl}"" class=""img-thumbnail"" />"
                : @"<img id=""imageUrl"" src="""" class=""img-thumbnail"" style=""display: none"" />";
            HfImageUrl.Value = itemInfo.ImageUrl;
            TbLinkUrl.Text = itemInfo.LinkUrl;
            TbCount.Text = itemInfo.Count.ToString();

            PhImageUrl.Visible = PollInfo.IsImage;
            PhLinkUrl.Visible = PollInfo.IsUrl;
        }

        private Dictionary<string, string> Upload()
        {
            var success = false;
            var message = string.Empty;
            var imageUrl = string.Empty;

            if (Request.Files["Filedata"] != null)
            {
                var postedFile = Request.Files["Filedata"];
                try
                {
                    if (!string.IsNullOrEmpty(postedFile?.FileName))
                    {
                        var filePath = postedFile.FileName;
                        var fileExtName = filePath.ToLower().Substring(filePath.LastIndexOf(".", StringComparison.Ordinal) + 1);
                        if (fileExtName == "jpg" || fileExtName == "jpeg" || fileExtName == "png" || fileExtName == "gif")
                        {
                            var localFilePath = SiteServer.Plugin.Context.UtilsApi.GetUploadFilePath(SiteId, filePath);
                            postedFile.SaveAs(localFilePath);
                            imageUrl = SiteServer.Plugin.Context.SiteApi.GetSiteUrlByFilePath(localFilePath);
                            success = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }

            var jsonAttributes = new Dictionary<string, string>();
            if (success)
            {
                jsonAttributes.Add("success", "true");
                jsonAttributes.Add("imageUrl", imageUrl);
            }
            else
            {
                jsonAttributes.Add("success", "false");
                if (string.IsNullOrEmpty(message))
                {
                    message = "图片上传失败";
                }
                jsonAttributes.Add("message", message);
            }

            return jsonAttributes;
        }

        public void Add_OnClick(object sender, EventArgs e)
        {
            var isChanged = _itemId > 0 ? UpdateItemInfo(_itemId) : InsertItemInfo();

            if (isChanged)
            {
                LayerUtils.Close(Page);
            }
        }

        private bool UpdateItemInfo(int itemId)
        {
            var isChanged = false;

            var itemInfo = Main.ItemDao.GetItemInfo(itemId);

            itemInfo.Title = TbTitle.Text;
            itemInfo.SubTitle = TbSubTitle.Text;
            itemInfo.ImageUrl = HfImageUrl.Value;
            itemInfo.LinkUrl = TbLinkUrl.Text;
            itemInfo.Count = Utils.ToInt(TbCount.Text);

            try
            {
                Main.ItemDao.Update(itemInfo);
                isChanged = true;
            }
            catch (Exception ex)
            {
                LtlMessage.Text = Utils.GetMessageHtml($"投票项修改失败：{ex.Message}！", false);
            }
            return isChanged;
        }

        private bool InsertItemInfo()
        {
            var isChanged = false;

            var itemInfo = new ItemInfo
            {
                Id = 0,
                SiteId = SiteId,
                ChannelId = ChannelId,
                ContentId = ContentId,
                Title = TbTitle.Text,
                SubTitle = TbSubTitle.Text,
                ImageUrl = HfImageUrl.Value,
                LinkUrl = TbLinkUrl.Text,
                Count = Utils.ToInt(TbCount.Text)
            };

            try
            {
                Main.ItemDao.Insert(itemInfo);
                isChanged = true;
            }
            catch (Exception ex)
            {
                LtlMessage.Text = Utils.GetMessageHtml($"投票项添加失败：{ex.Message}！", false);
            }
            return isChanged;
        }
    }
}
