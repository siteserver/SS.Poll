using System.Collections.Generic;
using System.Linq;
using Datory;
using SiteServer.Plugin;
using SS.Poll.Core.Models;
using SS.Poll.Core.Utils;

namespace SS.Poll.Core.Repositories
{
    public class FieldRepository
    {
        private readonly Repository<FieldInfo> _repository;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public FieldRepository()
        {
            _repository = new Repository<FieldInfo>(Context.Environment.DatabaseType, Context.Environment.ConnectionString);
        }

        private static class Attr
        {
            public const string Id = nameof(FieldInfo.Id);

            public const string PollId = nameof(FieldInfo.PollId);

            public const string Title = nameof(FieldInfo.Title);

            public const string Taxis = nameof(FieldInfo.Taxis);
        }

        public void Insert(int siteId, FieldInfo fieldInfo)
        {
            fieldInfo.Taxis = GetMaxTaxis(fieldInfo.PollId) + 1;

            fieldInfo.Id = _repository.Insert(fieldInfo);

            FieldManager.ItemRepository.InsertItems(fieldInfo.PollId, fieldInfo.Id, fieldInfo.Items);

            var formInfo = PollManager.GetPollInfo(siteId, fieldInfo.PollId);
            var list = PollUtils.StringCollectionToStringList(formInfo.ListAttributeNames);
            list.Add(fieldInfo.Title);
            formInfo.ListAttributeNames = PollUtils.ObjectCollectionToString(list);
            PollManager.Repository.Update(formInfo);

            FieldManager.ClearCache(fieldInfo.PollId);
        }

        public void Update(FieldInfo info, bool updateItems)
        {
            _repository.Update(info);

            if (updateItems)
            {
                FieldManager.ItemRepository.DeleteByFieldId(info.Id);
                FieldManager.ItemRepository.InsertItems(info.PollId, info.Id, info.Items);
            }

            FieldManager.ClearCache(info.PollId);
        }

        public void Delete(int pollId, int fieldId)
        {
            if (fieldId == 0) return;

            _repository.Delete(fieldId);

            FieldManager.ItemRepository.DeleteByFieldId(fieldId);

            FieldManager.ClearCache(pollId);
        }

        public void Delete(int pollId, string title)
        {
            var fieldId = _repository.Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.PollId, pollId)
                .Where(Attr.Title, title)
            );
            Delete(pollId, fieldId);
        }

        public void DeleteByPollId(int pollId)
        {
            _repository.Delete(Q.Where(Attr.PollId, pollId));

            FieldManager.ItemRepository.DeleteByPollId(pollId);

            FieldManager.ClearCache(pollId);
        }

        public bool IsTitleExists(int pollId, string title)
        {
            return _repository.Exists(Q
                .Where(Attr.PollId, pollId)
                .Where(Attr.Title, title)
            );
        }

        private int GetMaxTaxis(int pollId)
        {
            return _repository.Max(Attr.Taxis, Q
                       .Where(Attr.PollId, pollId)
                   ) ?? 0;
        }

        public List<KeyValuePair<string, FieldInfo>> GetAllFieldInfoList(int pollId)
        {
            var pairs = new List<KeyValuePair<string, FieldInfo>>();

            var allItemsDict = FieldManager.ItemRepository.GetAllItems(pollId);

            var fieldInfoList = _repository.GetAll(Q
                .Where(Attr.PollId, pollId)
                .OrderByDesc(Attr.Taxis, Attr.Id)
            );

            foreach (var fieldInfo in fieldInfoList)
            {
                fieldInfo.Validate = string.IsNullOrEmpty(fieldInfo.Validate) ? string.Empty : fieldInfo.Validate;
                fieldInfo.Value = fieldInfo.Value ?? string.Empty;

                allItemsDict.TryGetValue(fieldInfo.Id, out var items);
                if (items == null)
                {
                    items = new List<FieldItemInfo>();
                }
                fieldInfo.Items = items;

                var key = FieldManager.GetKey(fieldInfo.PollId, fieldInfo.Title);

                if (pairs.All(pair => pair.Key != key))
                {
                    var pair = new KeyValuePair<string, FieldInfo>(key, fieldInfo);
                    pairs.Add(pair);
                }
            }

            return pairs;
        }
    }
}
