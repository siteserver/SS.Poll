using System.Collections.Generic;
using System.Linq;
using SiteServer.Plugin;
using SS.Poll.Core.Models;
using SS.Poll.Core.Repositories;
using SS.Poll.Core.Utils;

namespace SS.Poll.Core
{
    public static class PollManager
    {
        public static PollRepository Repository => new PollRepository();

        private static class PollManagerCache
        {
            private static readonly object LockObject = new object();

            private static string GetCacheKey(int siteId)
            {
                return $"SS.Poll.Core.PollManager.{siteId}";
            }

            public static IList<PollInfo> GetCachePollInfoList(int siteId)
            {
                var cacheKey = GetCacheKey(siteId);
                var retVal = CacheUtils.Get<IList<PollInfo>>(cacheKey);
                if (retVal != null) return retVal;

                lock (LockObject)
                {
                    retVal = CacheUtils.Get<IList<PollInfo>>(cacheKey);
                    if (retVal == null)
                    {
                        retVal = Repository.GetPollInfoList(siteId);

                        CacheUtils.Insert(cacheKey, retVal, 12);
                    }
                }

                return retVal;
            }

            public static void Update(PollInfo pollInfo)
            {
                lock (LockObject)
                {
                    var pollInfoList = GetCachePollInfoList(pollInfo.SiteId).ToList();
                    var index = pollInfoList.FindIndex(x => x.Id == pollInfo.Id);
                    if (index != -1)
                    {
                        pollInfoList[index] = pollInfo;
                    }
                    else
                    {
                        pollInfoList.Add(pollInfo);
                    }
                }
            }

            public static void Clear(int siteId)
            {
                var cacheKey = GetCacheKey(siteId);
                CacheUtils.Remove(cacheKey);
            }
        }

        public static List<PollInfo> GetPollInfoList(int siteId, int channelId)
        {
            var pollInfoList = PollManagerCache.GetCachePollInfoList(siteId);

            return pollInfoList.Where(pollInfo => pollInfo.ChannelId == channelId).OrderBy(pollInfo => pollInfo.Taxis == 0 ? int.MaxValue : pollInfo.Taxis).ToList();
        }

        public static PollInfo GetPollInfo(int siteId, int pollId)
        {
            var pollInfoList = PollManagerCache.GetCachePollInfoList(siteId);

            return pollInfoList.FirstOrDefault(x => x.Id == pollId);
        }

        public static PollInfo GetPollInfo(IAuthenticatedRequest request)
        {
            var siteId = request.GetQueryInt("siteId");
            var channelId = request.GetQueryInt("channelId");
            var contentId = request.GetQueryInt("contentId");
            var pollId = request.GetQueryInt("pollId");

            return pollId > 0 ? GetPollInfo(siteId, pollId) : GetPollInfo(siteId, channelId, contentId);
        }

        public static PollInfo GetPollInfo(int siteId, int channelId, int contentId)
        {
            var pollInfoList = PollManagerCache.GetCachePollInfoList(siteId);
            var pollInfo = pollInfoList.FirstOrDefault(x => x.ChannelId == channelId && x.ContentId == contentId) ??
                           Repository.CreateDefaultPoll(siteId, channelId, contentId);

            return pollInfo ?? Repository.CreateDefaultPoll(siteId, channelId, contentId);
        }

        public static PollInfo GetPollInfo(int siteId, string title)
        {
            var pollInfoList = PollManagerCache.GetCachePollInfoList(siteId);
            return pollInfoList.FirstOrDefault(x => x.Title == title);
        }

        public static List<string> GetAllAttributeNames(List<FieldInfo> fieldInfoList)
        {
            var allAttributeNames = new List<string>
            {
                nameof(LogInfo.Guid)
            };
            foreach (var fieldInfo in fieldInfoList)
            {
                allAttributeNames.Add(fieldInfo.Title);
            }
            allAttributeNames.Add(nameof(LogInfo.AddDate));

            return allAttributeNames;
        }

        public static string GetPollTitle(PollInfo pollInfo)
        {
            var text = "投票管理 (0)";
            if (pollInfo == null) return text;

            if (pollInfo.TotalCount == 0)
            {
                pollInfo.TotalCount = LogManager.Repository.GetCount(pollInfo.Id);
                if (pollInfo.TotalCount > 0)
                {
                    Repository.Update(pollInfo);
                }
            }

            return $"{(pollInfo.ContentId > 0 ? "投票管理" : pollInfo.Title)} ({pollInfo.TotalCount})";
        }

        //        public static string GetTemplateHtml(string templateType, string directoryName)
        //        {
        //            var htmlPath = Context.PluginApi.GetPluginPath(PollUtils.PluginId, $"templates/{directoryName}/index.html");

        //            var html = CacheUtils.Get<string>(htmlPath);
        //            if (html != null) return html;

        //            html = PollUtils.ReadText(htmlPath);
        //            var startIndex = html.IndexOf("<body", StringComparison.Ordinal) + 5;
        //            var length = html.IndexOf("</body>", StringComparison.Ordinal) - startIndex;
        //            html = html.Substring(startIndex, length);
        //            html = html.Substring(html.IndexOf('\n'));

        ////            var jsPath = Context.PluginApi.GetPluginPath(PollUtils.PluginId, $"assets/js/{templateType}.js");
        ////            var javascript = PollUtils.ReadText(jsPath);
        ////            html = html.Replace(
        ////                $@"<script src=""../../assets/js/{templateType}.js"" type=""text/javascript""></script>",
        ////                $@"<script type=""text/javascript"">
        ////{javascript}
        ////</script>");
        //            html = html.Replace("../../", "{stl.rootUrl}/SiteFiles/plugins/SS.Poll/");
        //            html = html.Replace("../", "{stl.rootUrl}/SiteFiles/plugins/SS.Poll/templates/");

        //            CacheUtils.InsertHours(htmlPath, html, 1);
        //            return html;
        //        }

        public static void UpdateCache(PollInfo pollInfo)
        {
            PollManagerCache.Update(pollInfo);
        }

        public static void ClearCache(int siteId)
        {
            PollManagerCache.Clear(siteId);
        }
    }
}
