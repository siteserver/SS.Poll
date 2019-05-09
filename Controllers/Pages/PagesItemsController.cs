using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Poll.Core;
using SS.Poll.Core.Models;
using SS.Poll.Core.Utils;

namespace SS.Poll.Controllers.Pages
{
    [RoutePrefix("pages/items")]
    public class PagesItemsController : ApiController
    {
        private const string Route = "";
        private const string RouteItemId = "{itemId:int}";
        private const string RouteActionsUp = "actions/up";
        private const string RouteActionsDown = "actions/down";
        private const string RouteActionsUpload = "actions/upload";
        private const string RouteActionsExport = "actions/export";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetItems()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var items = ItemManager.GetItemInfoList(pollInfo.Id);
                var totalCount = items.Sum(x => x.Count);
                var adminToken = Context.AdminApi.GetAccessToken(request.AdminId, request.AdminName, TimeSpan.FromDays(1));

                return Ok(new
                {
                    Value = items,
                    TotalCount = totalCount,
                    PollInfo = pollInfo,
                    AdminToken = adminToken
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Insert()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var title = request.GetPostString("title");
                var subTitle = request.GetPostString("subTitle");
                var imageUrl = request.GetPostString("imageUrl");
                var linkUrl = request.GetPostString("linkUrl");
                var count = request.GetPostInt("count");

                var itemInfo = new ItemInfo
                {
                    Id = 0,
                    PollId = pollInfo.Id,
                    Title = title,
                    SubTitle = subTitle,
                    ImageUrl = imageUrl,
                    LinkUrl = linkUrl,
                    Count = count
                };

                ItemManager.Repository.Insert(itemInfo);

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteItemId)]
        public IHttpActionResult Update(int itemId)
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var title = request.GetPostString("title");
                var subTitle = request.GetPostString("subTitle");
                var imageUrl = request.GetPostString("imageUrl");
                var linkUrl = request.GetPostString("linkUrl");
                var count = request.GetPostInt("count");

                var itemInfo = ItemManager.Repository.GetItemInfo(itemId);

                itemInfo.Title = title;
                itemInfo.SubTitle = subTitle;
                itemInfo.ImageUrl = imageUrl;
                itemInfo.LinkUrl = linkUrl;
                itemInfo.Count = count;

                ItemManager.Repository.Update(itemInfo);

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(RouteItemId)]
        public IHttpActionResult Delete(int itemId)
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                ItemManager.Repository.Delete(pollInfo.Id, itemId);

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsUp)]
        public IHttpActionResult Up()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var itemId = request.GetPostInt("itemId");

                ItemManager.Repository.TaxisUp(pollInfo.Id, itemId);

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsDown)]
        public IHttpActionResult Down()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var itemId = request.GetPostInt("itemId");

                ItemManager.Repository.TaxisDown(pollInfo.Id, itemId);

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsUpload)]
        public IHttpActionResult Upload()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var imageUrl = string.Empty;

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var filePath = Context.SiteApi.GetUploadFilePath(pollInfo.SiteId, postFile.FileName);

                    if (!PollUtils.IsImage(Path.GetExtension(filePath)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    imageUrl = Context.SiteApi.GetSiteUrlByFilePath(filePath);
                }

                return Ok(new
                {
                    Value = imageUrl
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsExport)]
        public IHttpActionResult Export()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var pollInfo = PollManager.GetPollInfo(request);
                if (pollInfo == null) return NotFound();
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(pollInfo.SiteId, PollUtils.PluginId)) return Unauthorized();

                var itemInfoList = ItemManager.GetItemInfoList(pollInfo.Id);
                var totalCount = itemInfoList.Sum(x => x.Count);

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

                var fileName = "投票统计.csv";
                var filePath = Context.UtilsApi.GetTemporaryFilesPath(fileName);

                CsvUtils.Export(filePath, head, rows);

                var url = Context.UtilsApi.GetRootUrl($"SiteFiles/TemporaryFiles/{fileName}");

                return Ok(new
                {
                    Value = url
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
