using SiteServer.Plugin;
using SS.Poll.Core.Utils;

namespace SS.Poll.Core
{
    public static class MailTemplateManager
    {
        private static string GetTemplatesDirectoryPath()
        {
            return Context.PluginApi.GetPluginPath(PollUtils.PluginId, "assets/mail");
        }

        public static string GetTemplateHtml()
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var htmlPath = PollUtils.PathCombine(directoryPath, "template.html");
            var html = CacheUtils.Get<string>(htmlPath);
            if (html != null) return html;

            html = PollUtils.ReadText(htmlPath);

            CacheUtils.Insert(htmlPath, html, 24);
            return html;
        }

        public static string GetListHtml()
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var htmlPath = PollUtils.PathCombine(directoryPath, "list.html");
            var html = CacheUtils.Get<string>(htmlPath);
            if (html != null) return html;

            html = PollUtils.ReadText(htmlPath);

            CacheUtils.Insert(htmlPath, html, 24);
            return html;
        }
    }
}
