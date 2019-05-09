using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteServer.Plugin;
using SS.Poll.Core.Models;
using SS.Poll.Core.Utils;

namespace SS.Poll.Core
{
    public static class NotifyManager
    {
        public static void SendNotify(PollInfo pollInfo, List<FieldInfo> fieldInfoList, LogInfo logInfo)
        {
            if (pollInfo.IsAdministratorSmsNotify &&
                !string.IsNullOrEmpty(pollInfo.AdministratorSmsNotifyTplId) &&
                !string.IsNullOrEmpty(pollInfo.AdministratorSmsNotifyMobile))
            {
                var smsPlugin = Context.PluginApi.GetPlugin<SMS.Plugin>();
                if (smsPlugin != null && smsPlugin.IsReady)
                {
                    var parameters = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(pollInfo.AdministratorSmsNotifyKeys))
                    {
                        var keys = pollInfo.AdministratorSmsNotifyKeys.Split(',');
                        foreach (var key in keys)
                        {
                            if (PollUtils.EqualsIgnoreCase(key, nameof(LogInfo.Id)))
                            {
                                parameters.Add(key, logInfo.Id.ToString());
                            }
                            else if (PollUtils.EqualsIgnoreCase(key, nameof(LogInfo.AddDate)))
                            {
                                if (logInfo.AddDate.HasValue)
                                {
                                    parameters.Add(key, logInfo.AddDate.Value.ToString("yyyy-MM-dd HH:mm"));
                                }
                            }
                            else
                            {
                                var value = string.Empty;
                                var fieldInfo =
                                    fieldInfoList.FirstOrDefault(x => PollUtils.EqualsIgnoreCase(key, x.Title));
                                if (fieldInfo != null)
                                {
                                    value = LogManager.GetValue(fieldInfo, logInfo);
                                }

                                parameters.Add(key, value);
                            }
                        }
                    }

                    smsPlugin.Send(pollInfo.AdministratorSmsNotifyMobile,
                        pollInfo.AdministratorSmsNotifyTplId, parameters, out _);
                }
            }

            if (pollInfo.IsAdministratorMailNotify &&
                !string.IsNullOrEmpty(pollInfo.AdministratorMailNotifyAddress))
            {
                var mailPlugin = Context.PluginApi.GetPlugin<Mail.Plugin>();
                if (mailPlugin != null && mailPlugin.IsReady)
                {
                    var templateHtml = MailTemplateManager.GetTemplateHtml();
                    var listHtml = MailTemplateManager.GetListHtml();

                    var keyValueList = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("编号", logInfo.Guid)
                    };
                    if (logInfo.AddDate.HasValue)
                    {
                        keyValueList.Add(new KeyValuePair<string, string>("提交时间", logInfo.AddDate.Value.ToString("yyyy-MM-dd HH:mm")));
                    }
                    foreach (var fieldInfo in fieldInfoList)
                    {
                        keyValueList.Add(new KeyValuePair<string, string>(fieldInfo.Title,
                            LogManager.GetValue(fieldInfo, logInfo)));
                    }

                    var list = new StringBuilder();
                    foreach (var kv in keyValueList)
                    {
                        list.Append(listHtml.Replace("{{key}}", kv.Key).Replace("{{value}}", kv.Value));
                    }

                    var siteInfo = Context.SiteApi.GetSiteInfo(pollInfo.SiteId);

                    mailPlugin.Send(pollInfo.AdministratorMailNotifyAddress, string.Empty,
                        "[SiteServer CMS] 通知邮件",
                        templateHtml.Replace("{{title}}", $"{pollInfo.Title} - {siteInfo.SiteName}").Replace("{{list}}", list.ToString()), out _);
                }
            }

            if (pollInfo.IsUserSmsNotify &&
                !string.IsNullOrEmpty(pollInfo.UserSmsNotifyTplId) &&
                !string.IsNullOrEmpty(pollInfo.UserSmsNotifyMobileName))
            {
                var smsPlugin = Context.PluginApi.GetPlugin<SMS.Plugin>();
                if (smsPlugin != null && smsPlugin.IsReady)
                {
                    var parameters = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(pollInfo.UserSmsNotifyKeys))
                    {
                        var keys = pollInfo.UserSmsNotifyKeys.Split(',');
                        foreach (var key in keys)
                        {
                            if (PollUtils.EqualsIgnoreCase(key, nameof(LogInfo.Id)))
                            {
                                parameters.Add(key, logInfo.Id.ToString());
                            }
                            else if (PollUtils.EqualsIgnoreCase(key, nameof(LogInfo.AddDate)))
                            {
                                if (logInfo.AddDate.HasValue)
                                {
                                    parameters.Add(key, logInfo.AddDate.Value.ToString("yyyy-MM-dd HH:mm"));
                                }
                            }
                            else
                            {
                                var value = string.Empty;
                                var fieldInfo =
                                    fieldInfoList.FirstOrDefault(x => PollUtils.EqualsIgnoreCase(key, x.Title));
                                if (fieldInfo != null)
                                {
                                    value = LogManager.GetValue(fieldInfo, logInfo);
                                }

                                parameters.Add(key, value);
                            }
                        }
                    }

                    var mobileFieldInfo = fieldInfoList.FirstOrDefault(x => PollUtils.EqualsIgnoreCase(pollInfo.UserSmsNotifyMobileName, x.Title));
                    if (mobileFieldInfo != null)
                    {
                        var mobile = LogManager.GetValue(mobileFieldInfo, logInfo);
                        if (!string.IsNullOrEmpty(mobile))
                        {
                            smsPlugin.Send(mobile, pollInfo.UserSmsNotifyTplId, parameters, out _);
                        }
                    }
                }
            }
        }
    }
}
