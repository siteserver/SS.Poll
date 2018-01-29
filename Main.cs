using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SS.Poll.Core;
using SS.Poll.Model;
using SS.Poll.Pages;
using SS.Poll.Parse;
using SS.Poll.Provider;

namespace SS.Poll
{
    public class Main : IPlugin
    {
        public static string ConnectionString { get; private set; }
        public static IDataApi DataApi { get; private set; }
        public static IAdminApi AdminApi { get; private set; }
        public static IFilesApi FilesApi { get; private set; }

        public static Dao Dao { get; private set; }
        public static PollDao PollDao { get; private set; }
        public static ItemDao ItemDao { get; private set; }
        public static LogDao LogDao { get; private set; }
        public static FieldDao FieldDao { get; private set; }
        public static FieldItemDao FieldItemDao { get; private set; }

        public void Startup(IContext context, IService service)
        {
            ConnectionString = context.Environment.ConnectionString;
            DataApi = context.DataApi;
            AdminApi = context.AdminApi;
            FilesApi = context.FilesApi;

            Dao = new Dao(ConnectionString, DataApi);
            PollDao = new PollDao(ConnectionString, DataApi);
            ItemDao = new ItemDao(ConnectionString, DataApi);
            LogDao = new LogDao(ConnectionString, DataApi);
            FieldDao = new FieldDao(ConnectionString, DataApi);
            FieldItemDao = new FieldItemDao(ConnectionString, DataApi);

            service
                .AddContentLinks(new List<HyperLink>
                {
                    //new PluginContentLink
                    //{
                    //    Text = "编辑投票",
                    //    Href = "http://localhost:3000?pageType=edit_items"
                    //},
                    //new PluginContentLink
                    //{
                    //    Text = "查看投票",
                    //    Href = "http://localhost:3000?pageType=view_items"
                    //}
                    new HyperLink
                    {
                        Text = "编辑投票",
                        NavigateUrl = "build/index.html"
                    },
                    new HyperLink
                    {
                        Text = "查看投票",
                        NavigateUrl = $"{nameof(PageResults)}.aspx"
                    }
                })
                .AddDatabaseTable(PollDao.TableName, PollDao.Columns)
                .AddDatabaseTable(ItemDao.TableName, ItemDao.Columns)
                .AddDatabaseTable(LogDao.TableName, LogDao.Columns)
                .AddDatabaseTable(FieldDao.TableName, FieldDao.Columns)
                .AddDatabaseTable(FieldItemDao.TableName, FieldItemDao.Columns)
                .AddStlElementParser(StlPoll.ElementName, StlPoll.Parse)
                ;

            service.ContentTranslateCompleted += (sender, args) =>
            {
                var pollInfo = PollDao.GetPollInfo(args.SiteId, args.ChannelId, args.ContentId);
                if (pollInfo == null) return;

                pollInfo.PublishmentSystemId = args.TargetSiteId;
                pollInfo.ChannelId = args.TargetChannelId;
                pollInfo.ContentId = args.TargetContentId;
                pollInfo.TimeToStart = DateTime.Now;
                pollInfo.TimeToEnd = DateTime.Now.AddYears(1);
                PollDao.Insert(pollInfo);
            };

            service.ContentDeleteCompleted += (sender, args) =>
            {
                PollDao.Delete(args.SiteId, args.ChannelId, args.ContentId);
            };

            service.ApiGet += (sender, args) =>
            {
                var request = args.Request;

                if (!string.IsNullOrEmpty(args.Name) && !string.IsNullOrEmpty(args.Id))
                {
                    if (args.Name == "code")
                    {
                        var response = new HttpResponseMessage();

                        var random = new Random();
                        var validateCode = "";

                        char[] s = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
                        for (var i = 0; i < 4; i++)
                        {
                            validateCode += s[random.Next(0, s.Length)].ToString();
                        }

                        var validateimage = new Bitmap(75, 25, PixelFormat.Format32bppRgb);

                        var colors = Utils.Colors[random.Next(0, 5)];

                        var g = Graphics.FromImage(validateimage);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(240, 243, 248)), 0, 0, 100, 100); //矩形框
                        g.DrawString(validateCode, new Font(FontFamily.GenericSerif, 18, FontStyle.Bold | FontStyle.Italic), new SolidBrush(colors), new PointF(10, 0));//字体/颜色

                        for (var i = 0; i < 100; i++)
                        {
                            var x = random.Next(validateimage.Width);
                            var y = random.Next(validateimage.Height);

                            validateimage.SetPixel(x, y, Color.FromArgb(random.Next()));
                        }

                        g.Save();
                        var ms = new MemoryStream();
                        validateimage.Save(ms, ImageFormat.Png);

                        request.SetCookie("ss-poll:" + args.Id, validateCode, DateTime.Now.AddDays(1));

                        response.Content = new ByteArrayContent(ms.ToArray());
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                        response.StatusCode = HttpStatusCode.OK;

                        return response;
                    }
                    if (args.Name == "logs")
                    {
                        var limit = request.GetQueryInt("limit");
                        var offset = request.GetQueryInt("offset");

                        var pollId = Convert.ToInt32(args.Id);

                        var totalCount = LogDao.GetCount(pollId);
                        var logs = LogDao.GetPollLogInfoList(pollId, totalCount, limit, offset);

                        return new
                        {
                            Logs = logs,
                            TotalCount = totalCount
                        };
                    }
                }
                if (string.IsNullOrEmpty(args.Name) && string.IsNullOrEmpty(args.Id))
                {
                    var siteId = request.GetQueryInt("siteId");
                    var channelId = request.GetQueryInt("channelId");
                    var contentId = request.GetQueryInt("contentId");

                    var pollInfo = PollDao.GetPollInfo(siteId, channelId, contentId);
                    if (pollInfo == null)
                    {
                        pollInfo = new PollInfo
                        {
                            PublishmentSystemId = siteId,
                            ChannelId = channelId,
                            ContentId = contentId,
                            IsImage = true,
                            IsUrl = false,
                            IsTimeout = false,
                            IsCheckbox = true
                        };
                        pollInfo.Id = PollDao.Insert(pollInfo);
                    }
                    int totalCount;
                    var itemInfoList = ItemDao.GetItemInfoList(pollInfo.Id, out totalCount);

                    return new
                    {
                        Poll = pollInfo,
                        Items = itemInfoList,
                        TotalCount = totalCount
                    };
                }

                throw new Exception("请求的资源不在服务器上");
            };

            service.ApiDelete += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Name) && !string.IsNullOrEmpty(args.Id) && args.Name == "item")
                {
                    var itemId = Convert.ToInt32(args.Id);
                    ItemDao.Delete(itemId);
                    return new {};
                }

                throw new Exception("请求的资源不在服务器上");
            };

            service.ApiPost += (sender, args) =>
            {
                var request = args.Request;

                if (!string.IsNullOrEmpty(args.Name) && !string.IsNullOrEmpty(args.Id))
                {
                    if (Utils.EqualsIgnoreCase(args.Name, nameof(StlPoll.ApiSubmit)))
                    {
                        return StlPoll.ApiSubmit(request, args.Id);
                    }
                }
                if (!string.IsNullOrEmpty(args.Name))
                {
                    var siteId = request.GetQueryInt("siteId");

                    if (args.Name.ToLower() == "item")
                    {
                        var itemInfo = new ItemInfo
                        {
                            PollId = request.GetPostInt("pollId"),
                            Title = request.GetPostString("title"),
                            SubTitle = request.GetPostString("subTitle"),
                            ImageUrl = request.GetPostString("imageUrl"),
                            LinkUrl = request.GetPostString("linkUrl"),
                            Count = request.GetPostInt("count")
                        };

                        itemInfo.Id = ItemDao.Insert(itemInfo);

                        return itemInfo;
                    }
                    if (args.Name.ToLower() == "image")
                    {
                        var errorMessage = string.Empty;
                        var imageUrl = string.Empty;

                        try
                        {
                            if (request.HttpRequest.Files.Count > 0)
                            {
                                var postedFile = request.HttpRequest.Files[0];
                                var filePath = postedFile.FileName;
                                var fileExtName = Path.GetExtension(filePath).ToLower();
                                var localFilePath = FilesApi.GetUploadFilePath(siteId, filePath);

                                if (fileExtName != ".jpg" && fileExtName != ".png")
                                {
                                    errorMessage = "上传图片格式不正确！";
                                }
                                else
                                {
                                    postedFile.SaveAs(localFilePath);
                                    imageUrl = FilesApi.GetSiteUrlByFilePath(localFilePath);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMessage = ex.Message;
                        }

                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            throw new Exception(errorMessage);
                        }

                        return imageUrl;
                    }
                }

                throw new Exception("请求的资源不在服务器上");
            };

            service.ApiPut += (sender, args) =>
            {
                var request = args.Request;

                if (!string.IsNullOrEmpty(args.Name) && !string.IsNullOrEmpty(args.Id))
                {
                    var pollId = Convert.ToInt32(args.Id);

                    //if (name.ToLower() == "poll")
                    //{
                    //    var pollInfo = Main.PollDao.GetPollInfo(pollId);

                    //    pollInfo.IsImage = context.GetPostBool("isImage");
                    //    pollInfo.IsUrl = context.GetPostBool("isUrl");
                    //    pollInfo.IsTimeout = context.GetPostBool("isTimeout");
                    //    pollInfo.IsCheckbox = context.GetPostBool("isCheckbox");
                    //    pollInfo.TimeToStart = Convert.ToDateTime(context.GetPostString("timeToStart"));
                    //    pollInfo.TimeToEnd = Convert.ToDateTime(context.GetPostString("timeToEnd"));
                    //    pollInfo.CheckboxMin = context.GetPostInt("checkboxMin");
                    //    pollInfo.CheckboxMax = context.GetPostInt("checkboxMax");
                    //    pollInfo.IsProfile = context.GetPostBool("isProfile");
                    //    pollInfo.IsResult = context.GetPostBool("isResult");

                    //    Main.PollDao.Update(pollInfo);

                    //    return pollInfo;
                    //}
                    if (args.Name.ToLower() == "item")
                    {
                        var itemInfo = ItemDao.GetItemInfo(pollId);

                        itemInfo.Title = request.GetPostString("title");
                        itemInfo.SubTitle = request.GetPostString("subTitle");
                        itemInfo.ImageUrl = request.GetPostString("imageUrl");
                        itemInfo.LinkUrl = request.GetPostString("linkUrl");
                        itemInfo.Count = request.GetPostInt("count");

                        ItemDao.Update(itemInfo);

                        return itemInfo;
                    }
                }

                throw new Exception("请求的资源不在服务器上");
            };
        }
        
    }
}