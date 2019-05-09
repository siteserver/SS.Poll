using System;
using System.Linq;
using SiteServer.Plugin;
using SS.Poll.Core;
using SS.Poll.Core.Parse;

namespace SS.Poll
{
    public class Main : PluginBase
    {
        public override void Startup(IService service)
        {
            service
                .AddContentMenu(contentInfo => new Menu
                {
                    Text = "投票管理",
                    Href = "pages/items.html"
                })
                .AddSiteMenu(siteId =>
                {
                    var pollInfoList = PollManager.GetPollInfoList(siteId, 0);
                    var menus = pollInfoList.Where(pollInfo => !string.IsNullOrEmpty(pollInfo.Title)).Select(pollInfo => new Menu
                    {
                        Text = PollManager.GetPollTitle(pollInfo),
                        Href = $"pages/items.html?pollId={pollInfo.Id}"
                    }).ToList();

                    menus.Add(new Menu
                    {
                        Text = "投票管理",
                        Href = "pages/polls.html"
                    });
                    menus.Add(new Menu
                    {
                        Text = "投票模板",
                        Href = "pages/templates.html"
                    });

                    return new Menu
                    {
                        Text = "投票",
                        IconClass = "fa fa-check-circle-o",
                        Menus = menus
                    };
                })
                .AddDatabaseTable(PollManager.Repository.TableName, PollManager.Repository.TableColumns)
                .AddDatabaseTable(ItemManager.Repository.TableName, ItemManager.Repository.TableColumns)
                .AddDatabaseTable(LogManager.Repository.TableName, LogManager.Repository.TableColumns)
                .AddDatabaseTable(FieldManager.Repository.TableName, FieldManager.Repository.TableColumns)
                .AddDatabaseTable(FieldManager.ItemRepository.TableName, FieldManager.ItemRepository.TableColumns)
                .AddStlElementParser(StlPoll.ElementName, StlPoll.Parse)
                ;

            service.ContentTranslateCompleted += (sender, args) =>
            {
                var pollInfo = PollManager.Repository.GetPollInfo(args.SiteId, args.ChannelId, args.ContentId);
                if (pollInfo == null) return;

                pollInfo.SiteId = args.TargetSiteId;
                pollInfo.ChannelId = args.TargetChannelId;
                pollInfo.ContentId = args.TargetContentId;
                pollInfo.TimeToStart = DateTime.Now;
                pollInfo.TimeToEnd = DateTime.Now.AddYears(1);
                PollManager.Repository.Insert(pollInfo);
            };

            service.ContentDeleteCompleted += (sender, args) =>
            {
                var pollId = PollManager.Repository.GetPollId(args.SiteId, args.ChannelId, args.ContentId);
                PollManager.Repository.Delete(args.SiteId, pollId);
            };
        }
        
    }
}