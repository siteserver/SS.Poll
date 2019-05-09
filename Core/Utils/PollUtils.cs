using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SS.Poll.Core.Utils
{
    public static class PollUtils
    {
        public const string PluginId = "SS.Poll";

        public const int PageSize = 30;

        private const char UrlSeparatorChar = '/';
        private const char PathSeparatorChar = '\\';

        public static bool IsFileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static bool DeleteFileIfExists(string filePath)
        {
            var retVal = true;
            try
            {
                if (IsFileExists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                //try
                //{
                //    Scripting.FileSystemObject fso = new Scripting.FileSystemObjectClass();
                //    fso.DeleteFile(filePath, true);
                //}
                //catch
                //{
                //    retval = false;
                //}
                retVal = false;
            }
            return retVal;
        }

        public static void CopyDirectory(string sourcePath, string targetPath, bool isOverride)
        {
            if (!Directory.Exists(sourcePath)) return;

            CreateDirectoryIfNotExists(targetPath);
            var directoryInfo = new DirectoryInfo(sourcePath);
            foreach (var fileSystemInfo in directoryInfo.GetFileSystemInfos())
            {
                var destPath = Path.Combine(targetPath, fileSystemInfo.Name);
                if (fileSystemInfo is FileInfo)
                {
                    CopyFile(fileSystemInfo.FullName, destPath, isOverride);
                }
                else if (fileSystemInfo is DirectoryInfo)
                {
                    CopyDirectory(fileSystemInfo.FullName, destPath, isOverride);
                }
            }
        }

        private static void CopyFile(string sourceFilePath, string destFilePath, bool isOverride)
        {
            try
            {
                CreateDirectoryIfNotExists(destFilePath);

                File.Copy(sourceFilePath, destFilePath, isOverride);
            }
            catch
            {
                // ignored
            }
        }

        public static IEnumerable<string> GetDirectoryNames(string directoryPath)
        {
            var directories = Directory.GetDirectories(directoryPath);
            var retVal = new string[directories.Length];
            var i = 0;
            foreach (var directory in directories)
            {
                var directoryInfo = new DirectoryInfo(directory);
                retVal[i++] = directoryInfo.Name;
            }
            return retVal;
        }

        public static string ReadText(string filePath)
        {
            return File.ReadAllText(filePath, Encoding.UTF8);
        }

        public static void WriteText(string filePath, string content)
        {
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        public static string GetShortGuid(bool isUppercase)
        {
            var i = Guid.NewGuid().ToByteArray().Aggregate<byte, long>(1, (current, b) => current * (b + 1));
            var retVal = $"{i - DateTime.Now.Ticks:x}";
            return isUppercase ? retVal.ToUpper() : retVal.ToLower();
        }

        public static decimal ToDecimal(string intStr, decimal defaultValue)
        {
            if (!decimal.TryParse(intStr?.Trim(), out var i))
            {
                i = defaultValue;
            }
            return i;
        }

        public static string PathCombine(params string[] paths)
        {
            var retVal = string.Empty;
            if (paths != null && paths.Length > 0)
            {
                retVal = paths[0]?.Replace(UrlSeparatorChar, PathSeparatorChar).TrimEnd(PathSeparatorChar) ?? string.Empty;
                for (var i = 1; i < paths.Length; i++)
                {
                    var path = paths[i] != null ? paths[i].Replace(UrlSeparatorChar, PathSeparatorChar).Trim(PathSeparatorChar) : string.Empty;
                    retVal = Path.Combine(retVal, path);
                }
            }
            return retVal;
        }

        public static readonly Color[] Colors = { Color.FromArgb(37, 72, 91), Color.FromArgb(68, 24, 25), Color.FromArgb(17, 46, 2), Color.FromArgb(70, 16, 100), Color.FromArgb(24, 88, 74) };

        public static bool EqualsIgnoreCase(string a, string b)
        {
            if (a == b) return true;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            return string.Equals(a.Trim().ToLower(), b.Trim().ToLower());
        }

        public static int ToInt(string intStr, int defaultValue = 0)
        {
            return int.TryParse(intStr, out var i) ? i : defaultValue;
        }

        private static bool IsDirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public static void DeleteDirectoryIfExists(string directoryPath)
        {
            try
            {
                if (IsDirectoryExists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                }
            }
            catch
            {
                // ignored
            }
        }

        private static string GetDirectoryPath(string path)
        {
            var ext = Path.GetExtension(path);
            var directoryPath = !string.IsNullOrEmpty(ext) ? Path.GetDirectoryName(path) : path;
            return directoryPath;
        }

        public static void CreateDirectoryIfNotExists(string path)
        {
            var directoryPath = GetDirectoryPath(path);

            if (IsDirectoryExists(directoryPath)) return;

            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch
            {
                //Scripting.FileSystemObject fso = new Scripting.FileSystemObjectClass();
                //string[] directoryNames = directoryPath.Split('\\');
                //string thePath = directoryNames[0];
                //for (int i = 1; i < directoryNames.Length; i++)
                //{
                //    thePath = thePath + "\\" + directoryNames[i];
                //    if (StringUtils.Contains(thePath.ToLower(), ConfigUtils.Instance.PhysicalApplicationPath.ToLower()) && !IsDirectoryExists(thePath))
                //    {
                //        fso.CreateFolder(thePath);
                //    }
                //}                    
            }
        }

        public static string ObjectCollectionToString(ICollection collection)
        {
            var builder = new StringBuilder();
            if (collection == null) return builder.ToString();

            foreach (var obj in collection)
            {
                builder.Append(obj.ToString().Trim()).Append(",");
            }
            if (builder.Length != 0) builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        public static bool IsImage(string fileExtName)
        {
            var retVal = false;
            if (string.IsNullOrEmpty(fileExtName)) return false;
            fileExtName = fileExtName.ToLower().Trim();
            if (!fileExtName.StartsWith("."))
            {
                fileExtName = "." + fileExtName;
            }
            if (fileExtName == ".bmp" || fileExtName == ".gif" || fileExtName == ".jpg" || fileExtName == ".jpeg" || fileExtName == ".png" || fileExtName == ".pneg" || fileExtName == ".webp")
            {
                retVal = true;
            }
            return retVal;
        }

        public static DateTime ToDateTime(string dateTimeStr)
        {
            return ToDateTime(dateTimeStr, DateTime.Now);
        }

        public static DateTime ToDateTime(string dateTimeStr, DateTime defaultValue)
        {
            var datetime = defaultValue;
            if (!string.IsNullOrEmpty(dateTimeStr))
            {
                if (!DateTime.TryParse(dateTimeStr.Trim(), out datetime))
                {
                    datetime = defaultValue;
                }
                return datetime;
            }
            if (datetime <= DateTime.MinValue)
            {
                datetime = DateTime.Now;
            }
            return datetime;
        }

        public static string GetMessageHtml(string message, bool isSuccess)
        {
            return isSuccess
                ? $@"<div class=""alert alert-success"" role=""alert"">{message}</div>"
                : $@"<div class=""alert alert-danger"" role=""alert"">{message}</div>";
        }

        public static void SelectListItems(ListControl listControl, params string[] values)
        {
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    item.Selected = false;
                }
                foreach (ListItem item in listControl.Items)
                {
                    foreach (var value in values)
                    {
                        if (string.Equals(item.Value, value))
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }
        }

        public static object Eval(object dataItem, string name)
        {
            object o = null;
            try
            {
                o = DataBinder.Eval(dataItem, name);
            }
            catch
            {
                // ignored
            }
            if (o == DBNull.Value)
            {
                o = null;
            }
            return o;
        }

        public static int EvalInt(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o == null ? 0 : Convert.ToInt32(o);
        }

        public static decimal EvalDecimal(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o == null ? 0 : Convert.ToDecimal(o);
        }

        public static string EvalString(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o?.ToString() ?? string.Empty;
        }

        public static DateTime EvalDateTime(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            if (o == null)
            {
                return DateTime.MinValue;
            }
            return (DateTime)o;
        }

        public static bool EvalBool(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o != null && Convert.ToBoolean(o.ToString());
        }

        public static string GetPostMessageScript(int siteId, int channelId, int contentId, bool isSuccess)
        {
            var containerId = $"stl_poll_{siteId}_{channelId}_{contentId}";
            return $"<script>window.parent.postMessage({{containerId: '{containerId}', isSuccess: {isSuccess.ToString().ToLower()}}}, '*');</script>";
        }

        public static string GetControlRenderHtml(Control control)
        {
            var builder = new StringBuilder();
            if (control != null)
            {
                var sw = new System.IO.StringWriter(builder);
                var htw = new HtmlTextWriter(sw);
                control.RenderControl(htw);
            }
            return builder.ToString();
        }

        public static List<string> StringCollectionToStringList(string collection)
        {
            return StringCollectionToStringList(collection, ',');
        }

        public static List<string> StringCollectionToStringList(string collection, char split)
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(collection))
            {
                var array = collection.Split(split);
                foreach (var s in array)
                {
                    list.Add(s);
                }
            }
            return list;
        }

        public static string JsonSerialize(object obj)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                var timeFormat = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                settings.Converters.Add(timeFormat);

                return JsonConvert.SerializeObject(obj, settings);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static T JsonDeserialize<T>(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                var timeFormat = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                settings.Converters.Add(timeFormat);

                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch
            {
                return default(T);
            }
        }

        public static bool ToBool(string boolStr, bool defaultValue)
        {
            bool boolean;
            if (!bool.TryParse(boolStr?.Trim(), out boolean))
            {
                boolean = defaultValue;
            }
            return boolean;
        }

        public static int ToIntWithNagetive(string intStr, int defaultValue)
        {
            int i;
            if (!int.TryParse(intStr?.Trim(), out i))
            {
                i = defaultValue;
            }
            return i;
        }

        public static decimal ToDecimalWithNagetive(string intStr, decimal defaultValue)
        {
            decimal i;
            if (!decimal.TryParse(intStr?.Trim(), out i))
            {
                i = defaultValue;
            }
            return i;
        }

        public static bool ContainsIgnoreCase(List<string> list, string str)
        {
            if (list == null || list.Count == 0) return false;
            foreach (var item in list)
            {
                if (EqualsIgnoreCase(item, str))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
