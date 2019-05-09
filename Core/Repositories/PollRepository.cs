using System;
using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;
using SS.Poll.Core.Models;

namespace SS.Poll.Core.Repositories
{
    public class PollRepository
    {
        private readonly Repository<PollInfo> _repository;

        public PollRepository()
        {
            _repository = new Repository<PollInfo>(Context.Environment.DatabaseType, Context.Environment.ConnectionString);
        }

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(PollInfo.Id);

            public const string SiteId = nameof(PollInfo.SiteId);

            public const string ChannelId = nameof(PollInfo.ChannelId);

            public const string ContentId = nameof(PollInfo.ContentId);

            public const string Taxis = nameof(PollInfo.Taxis);
        }

        public int Insert(PollInfo pollInfo)
        {
            if (pollInfo.SiteId == 0) return 0;
            if (pollInfo.ChannelId == 0 && pollInfo.ContentId == 0 && string.IsNullOrEmpty(pollInfo.Title)) return 0;

            if (pollInfo.ContentId == 0)
            {
                pollInfo.Taxis = GetMaxTaxis(pollInfo.SiteId) + 1;
            }

            pollInfo.Id = _repository.Insert(pollInfo);

            PollManager.ClearCache(pollInfo.SiteId);

            return pollInfo.Id;
        }

        public bool Update(PollInfo pollInfo)
        {
            var updated = _repository.Update(pollInfo);

            PollManager.UpdateCache(pollInfo);

            return updated;
        }

        public void Delete(int siteId, int pollId)
        {
            if (pollId <= 0) return;

            _repository.Delete(pollId);

            FieldManager.Repository.DeleteByPollId(pollId);
            ItemManager.Repository.DeleteByPollId(pollId);
            LogManager.Repository.DeleteByPollId(pollId);

            PollManager.ClearCache(siteId);
        }

        public PollInfo GetPollInfo(int siteId, int channelId, int contentId)
        {
            return _repository.Get(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.ContentId, contentId)
            );
        }

        public int GetPollId(int siteId, int channelId, int contentId)
        {
            return _repository.Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.ContentId, contentId)
            );
        }

        public PollInfo GetPollInfo(int id)
        {
            return _repository.Get(id);
        }

        public void UpdateTaxisToDown(int siteId, int pollId)
        {
            var taxis = _repository.Get<int>(Q.Where(Attr.Id, pollId));

            var dataInfo = _repository.Get(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.Taxis, ">", taxis)
                .OrderBy(Attr.Taxis)
            );

            if (dataInfo == null) return;

            var higherId = dataInfo.Id;
            var higherTaxis = dataInfo.Taxis;

            SetTaxis(siteId, pollId, higherTaxis);
            SetTaxis(siteId, higherId, taxis);
        }

        public void UpdateTaxisToUp(int siteId, int pollId)
        {
            var taxis = _repository.Get<int>(Q.Where(Attr.Id, pollId));

            var dataInfo = _repository.Get(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.Taxis, "<", taxis)
                .OrderByDesc(Attr.Taxis)
            );

            if (dataInfo == null) return;

            var lowerId = dataInfo.Id;
            var lowerTaxis = dataInfo.Taxis;

            SetTaxis(siteId, pollId, lowerTaxis);
            SetTaxis(siteId, lowerId, taxis);
        }

        private int GetMaxTaxis(int siteId)
        {
            return _repository.Max(Attr.Taxis, Q.Where(Attr.SiteId, siteId)) ?? 0;
        }

        private void SetTaxis(int siteId, int pollId, int taxis)
        {
            _repository.Update(Q.Set(Attr.Taxis, taxis).Where(Attr.Id, pollId));

            PollManager.ClearCache(siteId);
        }

        public IList<PollInfo> GetPollInfoList(int siteId)
        {
            return _repository.GetAll(Q.Where(Attr.SiteId, siteId).OrderByDesc(Attr.Taxis, Attr.Id));
        }

        public PollInfo CreateDefaultPoll(int siteId, int channelId, int contentId)
        {
            var pollInfo = new PollInfo
            {
                SiteId = siteId,
                ChannelId = channelId,
                ContentId = contentId,
                IsImage = true,
                IsUrl = false,
                IsTimeout = false,
                IsCheckbox = true,
                Title = "默认投票",
                Description = string.Empty,
                Settings = string.Empty
            };
            pollInfo.Id = Insert(pollInfo);

            FieldManager.Repository.Insert(siteId, new FieldInfo
            {
                PollId = pollInfo.Id,
                Title = "姓名",
                PlaceHolder = "请输入您的姓名",
                FieldType = InputType.Text.Value,
                Validate = "required"
            });

            FieldManager.Repository.Insert(siteId, new FieldInfo
            {
                PollId = pollInfo.Id,
                Title = "手机",
                PlaceHolder = "请输入您的手机号码",
                FieldType = InputType.Text.Value,
                Validate = "mobile"
            });

            FieldManager.Repository.Insert(siteId, new FieldInfo
            {
                PollId = pollInfo.Id,
                Title = "邮箱",
                PlaceHolder = "请输入您的电子邮箱",
                FieldType = InputType.Text.Value,
                Validate = "email"
            });

            FieldManager.Repository.Insert(siteId, new FieldInfo
            {
                PollId = pollInfo.Id,
                Title = "留言",
                PlaceHolder = "请输入您的留言",
                Validate = "required",
                FieldType = InputType.TextArea.Value
            });

            return pollInfo;
        }

        public string GetImportTitle(int siteId, string title)
        {
            string importTitle;
            if (title.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var inputNameCount = 0;
                var lastInputName = title.Substring(title.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstInputName = title.Substring(0, title.Length - lastInputName.Length);
                try
                {
                    inputNameCount = int.Parse(lastInputName);
                }
                catch
                {
                    // ignored
                }
                inputNameCount++;
                importTitle = firstInputName + inputNameCount;
            }
            else
            {
                importTitle = title + "_1";
            }

            var inputInfo = PollManager.GetPollInfo(siteId, title);
            if (inputInfo != null)
            {
                importTitle = GetImportTitle(siteId, importTitle);
            }

            return importTitle;
        }
    }
}
