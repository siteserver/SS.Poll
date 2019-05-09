using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;
using SS.Poll.Core.Models;

namespace SS.Poll.Core.Repositories
{
    public class FieldItemRepository
    {
        private readonly Repository<FieldItemInfo> _repository;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public FieldItemRepository()
        {
            _repository = new Repository<FieldItemInfo>(Context.Environment.DatabaseType, Context.Environment.ConnectionString);
        }

        private static class Attr
        {
            public const string PollId = nameof(FieldItemInfo.PollId);

            public const string FieldId = nameof(FieldItemInfo.FieldId);
        }

        public void InsertItems(int pollId, int fieldId, List<FieldItemInfo> items)
        {
            if (pollId <= 0 || fieldId <= 0 || items == null || items.Count == 0) return;

            foreach (var itemInfo in items)
            {
                itemInfo.PollId = pollId;
                itemInfo.FieldId = fieldId;
                _repository.Insert(itemInfo);
            }
        }

        public void DeleteByPollId(int pollId)
        {
            if (pollId == 0) return;

            _repository.Delete(Q.Where(Attr.PollId, pollId));
        }

        public void DeleteByFieldId(int fieldId)
        {
            if (fieldId == 0) return;

            _repository.Delete(Q.Where(Attr.FieldId, fieldId));
        }

        public Dictionary<int, List<FieldItemInfo>> GetAllItems(int pollId)
        {
            var allDict = new Dictionary<int, List<FieldItemInfo>>();

            var fieldItemInfoList = _repository.GetAll(Q.Where(Attr.PollId, pollId));

            foreach (var fieldItemInfo in fieldItemInfoList)
            {
                allDict.TryGetValue(fieldItemInfo.FieldId, out var list);

                if (list == null)
                {
                    list = new List<FieldItemInfo>();
                }

                list.Add(fieldItemInfo);

                allDict[fieldItemInfo.FieldId] = list;
            }

            return allDict;
        }
    }
}
