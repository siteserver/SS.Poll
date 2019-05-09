using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Poll.Core;
using SS.Poll.Core.Models;
using SS.Poll.Core.Utils;

namespace SS.Poll.Controllers
{
    public class PollController : ApiController
    {
        [HttpGet, Route("{siteId:int}/{channelId:int}/{contentId:int}")]
        public IHttpActionResult Get(int siteId, int channelId, int contentId)
        {
            try
            {
                var pollInfo = PollManager.GetPollInfo(siteId, channelId, contentId);
                var itemInfoList = ItemManager.GetItemInfoList(pollInfo.Id);

                var totalCount = itemInfoList.Sum(x => x.Count);

                return Ok(new
                {
                    Poll = pollInfo,
                    Items = itemInfoList,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("code/{pollId:int}")]
        public HttpResponseMessage GetCode(int pollId)
        {
            var request = Context.AuthenticatedRequest;
            var response = new HttpResponseMessage();

            var random = new Random();
            var validateCode = "";

            char[] s = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            for (var i = 0; i < 4; i++)
            {
                validateCode += s[random.Next(0, s.Length)].ToString();
            }

            var validateimage = new Bitmap(75, 25, PixelFormat.Format32bppRgb);

            var colors = PollUtils.Colors[random.Next(0, 5)];

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

            HttpContext.Current.Response.SetCookie(new HttpCookie("ss-poll:" + pollId)
            {
                Value = validateCode,
                Expires = DateTime.Now.AddDays(1)
            });

            response.Content = new ByteArrayContent(ms.ToArray());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        [HttpPost, Route("{pollId:int}")]
        public IHttpActionResult Submit(int pollId)
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var pollInfo = PollManager.Repository.GetPollInfo(pollId);
                if (pollInfo == null) return null;

                if (pollInfo.IsCaptcha)
                {
                    var code = request.GetPostString("code");
                    var cookie = HttpContext.Current.Request.Cookies.Get("ss-poll:" + pollId);
                    var cookieValue = cookie?.Value;
                    if (string.IsNullOrEmpty(cookieValue) || !PollUtils.EqualsIgnoreCase(cookieValue, code))
                    {
                        throw new Exception("提交失败，验证码不正确！");
                    }
                }

                var itemIds = request.GetPostString("itemIds").Split(',').Select(idStr => Convert.ToInt32(idStr)).ToList();
                if (pollInfo.IsCheckbox)
                {
                    if (pollInfo.CheckboxMin > 0 && itemIds.Count < pollInfo.CheckboxMin)
                    {
                        throw new Exception($"提交失败，最少需要选择{pollInfo.CheckboxMin}项！");
                    }
                    if (pollInfo.CheckboxMax > 0 && itemIds.Count > pollInfo.CheckboxMax)
                    {
                        throw new Exception($"提交失败，最多只能选择{pollInfo.CheckboxMax}项！");
                    }
                }

                var logInfo = new LogInfo
                {
                    PollId = pollInfo.Id,
                    ItemIds = string.Join(",", itemIds),
                    AddDate = DateTime.Now
                };

                var attributes = request.GetPostObject<Dictionary<string, string>>("attributes");

                var fieldInfoList = FieldManager.GetFieldInfoList(pollInfo.Id);
                foreach (var fieldInfo in fieldInfoList)
                {
                    string value;
                    attributes.TryGetValue(fieldInfo.Title, out value);
                    logInfo.Set(fieldInfo.Title, value);
                }

                LogManager.Repository.Insert(pollInfo, logInfo);

                ItemManager.Repository.AddCount(pollInfo.Id, itemIds);

                var itemInfoList = ItemManager.GetItemInfoList(pollInfo.Id);
                var totalCount = itemInfoList.Sum(x => x.Count);

                var items = new List<object>();
                foreach (var itemInfo in itemInfoList)
                {
                    var percentage = "0%";
                    if (totalCount > 0)
                    {
                        percentage = Convert.ToDouble(itemInfo.Count / (double)totalCount).ToString("0.0%");
                    }
                    items.Add(new
                    {
                        itemInfo.Id,
                        itemInfo.PollId,
                        itemInfo.Title,
                        itemInfo.SubTitle,
                        itemInfo.ImageUrl,
                        itemInfo.Count,
                        Percentage = percentage
                    });
                }

                return Ok(new
                {
                    TotalCount = totalCount,
                    Items = items
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("item")]
        public IHttpActionResult InsertItem()
        {
            try
            {
                var request = Context.AuthenticatedRequest;

                var siteId = request.GetQueryInt("siteId");
                var pollId = request.GetQueryInt("pollId");

                var itemInfo = new ItemInfo
                {
                    PollId = pollId,
                    Title = request.GetPostString("title"),
                    SubTitle = request.GetPostString("subTitle"),
                    ImageUrl = request.GetPostString("imageUrl"),
                    LinkUrl = request.GetPostString("linkUrl"),
                    Count = request.GetPostInt("count")
                };

                itemInfo.Id = ItemManager.Repository.Insert(itemInfo);

                return Ok(new
                {
                    Value = itemInfo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
