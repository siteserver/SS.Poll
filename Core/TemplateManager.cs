using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using SiteServer.Plugin;
using SS.Poll.Core.Models;
using SS.Poll.Core.Utils;

namespace SS.Poll.Core
{
    public static class TemplateManager
    {
        private static string CacheGetFileContent(string filePath)
        {
            ObjectCache cache = MemoryCache.Default;

            if (cache[filePath] is string fileContents) return fileContents;

            var policy = new CacheItemPolicy
            {
                SlidingExpiration = new TimeSpan(0, 1, 0, 0)
            };
            policy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string> { filePath }));

            fileContents = PollUtils.ReadText(filePath);

            cache.Set(filePath, fileContents, policy);

            return fileContents;
        }

        private static string GetTemplatesDirectoryPath()
        {
            return Context.PluginApi.GetPluginPath(PollUtils.PluginId, "templates");
        }

        public static List<TemplateInfo> GetTemplateInfoList(string type)
        {
            var templateInfoList = new List<TemplateInfo>();

            var directoryPath = GetTemplatesDirectoryPath();
            var directoryNames = PollUtils.GetDirectoryNames(directoryPath);
            foreach (var directoryName in directoryNames)
            {
                var templateInfo = GetTemplateInfo(directoryPath, directoryName);
                if (templateInfo == null) continue;
                if (string.IsNullOrEmpty(type) && string.IsNullOrEmpty(templateInfo.Type) || PollUtils.EqualsIgnoreCase(type, templateInfo.Type))
                {
                    templateInfoList.Add(templateInfo);
                }
            }

            return templateInfoList;
        }

        public static TemplateInfo GetTemplateInfo(string name)
        {
            var directoryPath = GetTemplatesDirectoryPath();
            return GetTemplateInfo(directoryPath, name);
        }

        private static TemplateInfo GetTemplateInfo(string templatesDirectoryPath, string name)
        {
            TemplateInfo templateInfo = null;

            var configPath = PollUtils.PathCombine(templatesDirectoryPath, name, "config.json");
            if (PollUtils.IsFileExists(configPath))
            {
                templateInfo = Context.UtilsApi.JsonDeserialize<TemplateInfo>(PollUtils.ReadText(configPath));
                templateInfo.Name = name;
            }

            return templateInfo;
        }

        public static void Clone(string nameToClone, TemplateInfo templateInfo, string templateHtml = null)
        {
            var directoryPath = Context.PluginApi.GetPluginPath(PollUtils.PluginId, "templates");

            PollUtils.CopyDirectory(PollUtils.PathCombine(directoryPath, nameToClone), PollUtils.PathCombine(directoryPath, templateInfo.Name), true);

            var configJson = Context.UtilsApi.JsonSerialize(templateInfo);
            var configPath = PollUtils.PathCombine(directoryPath, templateInfo.Name, "config.json");
            PollUtils.WriteText(configPath, configJson);

            if (templateHtml != null)
            {
                SetTemplateHtml(templateInfo, templateHtml);
            }
        }

        public static void Edit(TemplateInfo templateInfo)
        {
            var directoryPath = Context.PluginApi.GetPluginPath(PollUtils.PluginId, "templates");

            var configJson = Context.UtilsApi.JsonSerialize(templateInfo);
            var configPath = PollUtils.PathCombine(directoryPath, templateInfo.Name, "config.json");
            PollUtils.WriteText(configPath, configJson);
        }

        public static string GetTemplateHtml(TemplateInfo templateInfo)
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var htmlPath = PollUtils.PathCombine(directoryPath, templateInfo.Name, templateInfo.Main);
            return CacheGetFileContent(htmlPath);
        }

        public static void SetTemplateHtml(TemplateInfo templateInfo, string html)
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var htmlPath = PollUtils.PathCombine(directoryPath, templateInfo.Name, templateInfo.Main);

            PollUtils.WriteText(htmlPath, html);
        }

        public static void DeleteTemplate(string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            var directoryPath = GetTemplatesDirectoryPath();
            var templatePath = PollUtils.PathCombine(directoryPath, name);
            PollUtils.DeleteDirectoryIfExists(templatePath);
        }
    }
}
