using System.Collections.Generic;
using System.Linq;
using SS.Poll.Core.Models;
using SS.Poll.Core.Repositories;
using SS.Poll.Core.Utils;

namespace SS.Poll.Core
{
    public static class ItemManager
    {
        public static ItemRepository Repository => new ItemRepository();

        private static class ItemManagerCache
        {
            private static readonly object LockObject = new object();

            private static string GetCacheKey(int pollId)
            {
                return $"SS.Poll.Core.ItemManager.{pollId}";
            }

            public static List<KeyValuePair<string, ItemInfo>> GetAllItemInfoList(int pollId)
            {
                var cacheKey = GetCacheKey(pollId);
                var retVal = CacheUtils.Get<List<KeyValuePair<string, ItemInfo>>>(cacheKey);
                if (retVal != null) return retVal;

                lock (LockObject)
                {
                    retVal = CacheUtils.Get<List<KeyValuePair<string, ItemInfo>>>(cacheKey);
                    if (retVal == null)
                    {
                        retVal = Repository.GetAllItemInfoList(pollId);

                        CacheUtils.Insert(cacheKey, retVal, 12);
                    }
                }

                return retVal;
            }

            public static void Clear(int pollId)
            {
                var cacheKey = GetCacheKey(pollId);
                CacheUtils.Remove(cacheKey);
            }
        }

        public static List<ItemInfo> GetItemInfoList(int pollId)
        {
            var fieldInfoList = new List<ItemInfo>();

            var entries = ItemManagerCache.GetAllItemInfoList(pollId);
            var startKey = GetKeyPrefix(pollId);
            var list = entries.Where(tuple => tuple.Key.StartsWith(startKey)).ToList();
            foreach (var pair in list)
            {
                if (pair.IsDefault()) continue;

                fieldInfoList.Add((ItemInfo)pair.Value.Clone());
            }

            return fieldInfoList.OrderBy(fieldInfo => fieldInfo.Taxis == 0 ? int.MaxValue : fieldInfo.Taxis).ToList();
        }

        public static ItemInfo GetItemInfo(int pollId, int id)
        {
            var entries = ItemManagerCache.GetAllItemInfoList(pollId);

            var entry = entries.FirstOrDefault(x => x.Value != null && x.Value.Id == id);
            return entry.IsDefault() ? null : (ItemInfo)entry.Value.Clone();
        }

        public static void ClearCache(int pollId)
        {
            ItemManagerCache.Clear(pollId);
        }

        private static string GetKeyPrefix(int pollId)
        {
            return $"{pollId}$";
        }

        public static string GetKey(int pollId, string title)
        {
            return $"{GetKeyPrefix(pollId)}{title}";
        }
    }
}
