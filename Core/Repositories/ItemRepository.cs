using System.Collections.Generic;
using System.Linq;
using Datory;
using SiteServer.Plugin;
using SS.Poll.Core.Models;

namespace SS.Poll.Core.Repositories
{
    public class ItemRepository
    {
        private readonly Repository<ItemInfo> _repository;

        public ItemRepository()
        {
            _repository = new Repository<ItemInfo>(Context.Environment.DatabaseType, Context.Environment.ConnectionString);
        }

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(ItemInfo.Id);
            public const string PollId = nameof(ItemInfo.PollId);
            public const string Taxis = nameof(ItemInfo.Taxis);
            public const string Count = nameof(ItemInfo.Count);
        }
        public int Insert(ItemInfo itemInfo)
        {
            itemInfo.Taxis = GetMaxTaxis(itemInfo.PollId) + 1;

            itemInfo.Id = _repository.Insert(itemInfo);

            ItemManager.ClearCache(itemInfo.PollId);

            return itemInfo.Id;
        }

        public void Update(ItemInfo itemInfo)
        {
            _repository.Update(itemInfo);
            ItemManager.ClearCache(itemInfo.PollId);
        }

        public void DeleteByPollId(int pollId)
        {
            if (pollId <= 0) return;

            _repository.Delete(Q
                .Where(Attr.PollId, pollId)
            );

            ItemManager.ClearCache(pollId);
        }

        public void Delete(int pollId, int itemId)
        {
            if (itemId <= 0) return;

            _repository.Delete(itemId);

            ItemManager.ClearCache(pollId);
        }

        public ItemInfo GetItemInfo(int itemId)
        {
            return _repository.Get(itemId);
        }

        public void AddCount(int pollId, List<int> itemIdList)
        {
            if (pollId <= 0 || itemIdList == null || itemIdList.Count <= 0) return;

            _repository.Increment(Attr.Count, Q
                .WhereIn(Attr.Id, itemIdList)
                .Where(Attr.PollId, pollId)
            );

            ItemManager.ClearCache(pollId);
        }

        private int GetMaxTaxis(int pollId)
        {
            return _repository.Max(Attr.Taxis, Q
                       .Where(Attr.PollId, pollId)
                   ) ?? 0;
        }

        public void TaxisDown(int pollId, int id)
        {
            var itemInfo = GetItemInfo(id);
            if (itemInfo == null) return;

            var higherFieldInfo = _repository.Get(Q
                .Where(Attr.PollId, itemInfo.PollId)
                .Where(Attr.Taxis, ">", itemInfo.Taxis)
                .OrderBy(Attr.Taxis)
            );

            if (higherFieldInfo != null)
            {
                var higherId = higherFieldInfo.Id;
                var higherTaxis = higherFieldInfo.Taxis;

                SetTaxis(pollId, id, higherTaxis);
                SetTaxis(pollId, higherId, itemInfo.Taxis);
            }
        }

        public void TaxisUp(int pollId, int id)
        {
            var itemInfo = GetItemInfo(id);
            if (itemInfo == null) return;

            var lowerInfo = _repository.Get(Q
                .Where(Attr.PollId, itemInfo.PollId)
                .Where(Attr.Taxis, "<", itemInfo.Taxis)
                .OrderByDesc(Attr.Taxis)
            );

            if (lowerInfo != null)
            {
                var lowerId = lowerInfo.Id;
                var lowerTaxis = lowerInfo.Taxis;

                SetTaxis(pollId, id, lowerTaxis);
                SetTaxis(pollId, lowerId, itemInfo.Taxis);
            }
        }

        private void SetTaxis(int pollId, int itemId, int taxis)
        {
            _repository.Update(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.Id, itemId)
            );

            ItemManager.ClearCache(pollId);
        }

        public List<KeyValuePair<string, ItemInfo>> GetAllItemInfoList(int pollId)
        {
            var pairs = new List<KeyValuePair<string, ItemInfo>>();

            var itemInfoList = _repository.GetAll(Q
                .Where(Attr.PollId, pollId)
                .OrderByDesc(Attr.Taxis, Attr.Id)
            );

            foreach (var itemInfo in itemInfoList)
            {
                var key = ItemManager.GetKey(itemInfo.PollId, itemInfo.Title);

                if (pairs.All(pair => pair.Key != key))
                {
                    var pair = new KeyValuePair<string, ItemInfo>(key, itemInfo);
                    pairs.Add(pair);
                }
            }

            return pairs;
        }
    }
}
