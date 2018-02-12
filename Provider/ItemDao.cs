using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Poll.Models;

namespace SS.Poll.Provider
{
    public class ItemDao
    {
        public const string TableName = "ss_poll_item";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(ItemInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ItemInfo.PollId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ItemInfo.Title),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(ItemInfo.SubTitle),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(ItemInfo.ImageUrl),
                DataType = DataType.VarChar,
                DataLength = 500
            },
            new TableColumn
            {
                AttributeName = nameof(ItemInfo.LinkUrl),
                DataType = DataType.VarChar,
                DataLength = 500
            },
            new TableColumn
            {
                AttributeName = nameof(ItemInfo.Count),
                DataType = DataType.Integer
            }
        };

        private readonly string _connectionString;
        private readonly IDataApi _helper;

        public ItemDao(string connectionString, IDataApi dataApi)
        {
            _connectionString = connectionString;
            _helper = dataApi;
        }

        public int Insert(ItemInfo itemInfo)
        {
            string sqlString = $@"INSERT INTO {TableName}
(
    {nameof(ItemInfo.PollId)}, 
    {nameof(ItemInfo.Title)}, 
    {nameof(ItemInfo.SubTitle)}, 
    {nameof(ItemInfo.ImageUrl)}, 
    {nameof(ItemInfo.LinkUrl)}, 
    {nameof(ItemInfo.Count)}
) VALUES (
    @{nameof(ItemInfo.PollId)}, 
    @{nameof(ItemInfo.Title)}, 
    @{nameof(ItemInfo.SubTitle)}, 
    @{nameof(ItemInfo.ImageUrl)}, 
    @{nameof(ItemInfo.LinkUrl)}, 
    @{nameof(ItemInfo.Count)}
)";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter(nameof(itemInfo.PollId), itemInfo.PollId),
                _helper.GetParameter(nameof(itemInfo.Title), itemInfo.Title),
                _helper.GetParameter(nameof(itemInfo.SubTitle), itemInfo.SubTitle),
                _helper.GetParameter(nameof(itemInfo.ImageUrl), itemInfo.ImageUrl),
                _helper.GetParameter(nameof(itemInfo.LinkUrl), itemInfo.LinkUrl),
                _helper.GetParameter(nameof(itemInfo.Count), itemInfo.Count)
            };

            return _helper.ExecuteNonQueryAndReturnId(TableName, nameof(ItemInfo.Id), _connectionString, sqlString, parameters.ToArray());
        }

        public void Update(ItemInfo itemInfo)
        {
            string sqlString = $@"UPDATE {TableName} SET
                {nameof(ItemInfo.PollId)} = @{nameof(ItemInfo.PollId)}, 
                {nameof(ItemInfo.Title)} = @{nameof(ItemInfo.Title)}, 
                {nameof(ItemInfo.SubTitle)} = @{nameof(ItemInfo.SubTitle)}, 
                {nameof(ItemInfo.ImageUrl)} = @{nameof(ItemInfo.ImageUrl)}, 
                {nameof(ItemInfo.LinkUrl)} = @{nameof(ItemInfo.LinkUrl)}, 
                {nameof(ItemInfo.Count)} = @{nameof(ItemInfo.Count)}
            WHERE {nameof(ItemInfo.Id)} = @{nameof(ItemInfo.Id)}";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter(nameof(itemInfo.PollId), itemInfo.PollId),
                _helper.GetParameter(nameof(itemInfo.Title), itemInfo.Title),
                _helper.GetParameter(nameof(itemInfo.SubTitle), itemInfo.SubTitle),
                _helper.GetParameter(nameof(itemInfo.ImageUrl), itemInfo.ImageUrl),
                _helper.GetParameter(nameof(itemInfo.LinkUrl), itemInfo.LinkUrl),
                _helper.GetParameter(nameof(itemInfo.Count), itemInfo.Count),
                _helper.GetParameter(nameof(itemInfo.Id), itemInfo.Id)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parameters.ToArray());
        }

        public void DeleteAll(int pollId)
        {
            if (pollId <= 0) return;

            string sqlString =
                $"DELETE FROM {TableName} WHERE PollId = {pollId}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public void Delete(int itemId)
        {
            if (itemId <= 0) return;

            string sqlString =
                $"DELETE FROM {TableName} WHERE Id = {itemId}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public ItemInfo GetItemInfo(int itemId)
        {
            ItemInfo pollItemInfo = null;

            string sqlString = $@"SELECT {nameof(ItemInfo.Id)}, 
            {nameof(ItemInfo.PollId)}, 
            {nameof(ItemInfo.Title)}, 
            {nameof(ItemInfo.SubTitle)}, 
            {nameof(ItemInfo.ImageUrl)}, 
            {nameof(ItemInfo.LinkUrl)}, 
            {nameof(ItemInfo.Count)}
            FROM {TableName} WHERE {nameof(ItemInfo.Id)} = {itemId}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    pollItemInfo = GetPollItemInfo(rdr);
                }
                rdr.Close();
            }

            return pollItemInfo;
        }

        public List<ItemInfo> GetItemInfoList(int pollId)
        {
            int totalCount;
            return GetItemInfoList(pollId, out totalCount);
        }

        public List<ItemInfo> GetItemInfoList(int pollId, out int totalCount)
        {
            totalCount = 0;
            var list = new List<ItemInfo>();

            string sqlString = $@"SELECT {nameof(ItemInfo.Id)}, 
            {nameof(ItemInfo.PollId)}, 
            {nameof(ItemInfo.Title)},
            {nameof(ItemInfo.SubTitle)},
            {nameof(ItemInfo.ImageUrl)}, 
            {nameof(ItemInfo.LinkUrl)}, 
            {nameof(ItemInfo.Count)}
            FROM {TableName} WHERE {nameof(ItemInfo.PollId)} = {pollId}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var itemInfo = GetPollItemInfo(rdr);
                    totalCount += itemInfo.Count;
                    list.Add(itemInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public void AddCount(int pollId, List<int> itemIdList)
        {
            if (pollId <= 0 || itemIdList == null || itemIdList.Count <= 0) return;

            string sqlString =
                $"UPDATE {TableName} SET Count = Count + 1 WHERE {nameof(ItemInfo.Id)} IN ({string.Join(",", itemIdList)}) AND {nameof(ItemInfo.PollId)} = {pollId}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        private static ItemInfo GetPollItemInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var itemInfo = new ItemInfo();

            var i = 0;
            itemInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            itemInfo.PollId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            itemInfo.Title = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            itemInfo.SubTitle = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            itemInfo.ImageUrl = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            itemInfo.LinkUrl = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            itemInfo.Count = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);

            return itemInfo;
        }
    }
}
