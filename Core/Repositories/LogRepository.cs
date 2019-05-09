using System;
using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;
using SS.Poll.Core.Models;
using SS.Poll.Core.Utils;

namespace SS.Poll.Core.Repositories
{
    public class LogRepository
    {
        private readonly Repository<LogInfo> _repository;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public LogRepository()
        {
            _repository = new Repository<LogInfo>(Context.Environment.DatabaseType, Context.Environment.ConnectionString);
        }

        private static class Attr
        {
            public const string Id = nameof(LogInfo.Id);

            public const string PollId = nameof(LogInfo.PollId);
        }

        public int Insert(PollInfo pollInfo, LogInfo logInfo)
        {
            logInfo.PollId = pollInfo.Id;
            logInfo.Id = _repository.Insert(logInfo);

            pollInfo.TotalCount += 1;
            PollManager.Repository.Update(pollInfo);

            return logInfo.Id;
        }

        public void Update(LogInfo logInfo)
        {
            _repository.Update(logInfo);
        }

        public LogInfo GetLogInfo(int logId)
        {
            return _repository.Get(logId);
        }

        public void DeleteByPollId(int pollId)
        {
            if (pollId <= 0) return;

            _repository.Delete(Q.Where(Attr.PollId, pollId));
        }

        public void Delete(PollInfo pollInfo, LogInfo logInfo)
        {
            _repository.Delete(logInfo.Id);

            pollInfo.TotalCount -= 1;
            PollManager.Repository.Update(pollInfo);
        }

        public int GetCount(int pollId)
        {
            return _repository.Count(Q.Where(Attr.PollId, pollId));
        }

        public IList<LogInfo> GetLogInfoList(PollInfo pollInfo, int page)
        {
            if (pollInfo.TotalCount == 0)
            {
                return new List<LogInfo>();
            }

            if (pollInfo.TotalCount <= PollUtils.PageSize)
            {
                return GetLogInfoList(pollInfo.Id, 0, pollInfo.TotalCount);
            }

            if (page == 0) page = 1;
            var offset = (page - 1) * PollUtils.PageSize;
            var limit = pollInfo.TotalCount - offset > PollUtils.PageSize ? PollUtils.PageSize : pollInfo.TotalCount - offset;
            return GetLogInfoList(pollInfo.Id, offset, limit);
        }

        public IList<LogInfo> GetLogInfoList(int pollId, int offset, int limit)
        {
            var q = Q
                .Where(Attr.PollId, pollId)
                .OrderByDesc(Attr.Id);

            if (offset > 0)
            {
                q.Offset(offset);
            }

            if (limit > 0)
            {
                q.Limit(limit);
            }

            return _repository.GetAll(q);
        }
    }
}
