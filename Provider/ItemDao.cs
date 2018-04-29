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
                AttributeName = nameof(ItemInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ItemInfo.ChannelId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ItemInfo.ContentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ItemInfo.Taxis),
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
            itemInfo.Taxis = GetMaxTaxis(itemInfo.SiteId, itemInfo.ChannelId, itemInfo.ContentId) + 1;

            string sqlString = $@"INSERT INTO {TableName}
(
    {nameof(ItemInfo.SiteId)}, 
    {nameof(ItemInfo.ChannelId)}, 
    {nameof(ItemInfo.ContentId)}, 
    {nameof(ItemInfo.Taxis)},
    {nameof(ItemInfo.Title)}, 
    {nameof(ItemInfo.SubTitle)}, 
    {nameof(ItemInfo.ImageUrl)}, 
    {nameof(ItemInfo.LinkUrl)}, 
    {nameof(ItemInfo.Count)}
) VALUES (
    @{nameof(ItemInfo.SiteId)}, 
    @{nameof(ItemInfo.ChannelId)}, 
    @{nameof(ItemInfo.ContentId)},
    @{nameof(ItemInfo.Taxis)},
    @{nameof(ItemInfo.Title)}, 
    @{nameof(ItemInfo.SubTitle)}, 
    @{nameof(ItemInfo.ImageUrl)}, 
    @{nameof(ItemInfo.LinkUrl)}, 
    @{nameof(ItemInfo.Count)}
)";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter(nameof(itemInfo.SiteId), itemInfo.SiteId),
                _helper.GetParameter(nameof(itemInfo.ChannelId), itemInfo.ChannelId),
                _helper.GetParameter(nameof(itemInfo.ContentId), itemInfo.ContentId),
                _helper.GetParameter(nameof(itemInfo.Taxis), itemInfo.Taxis),
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
                {nameof(ItemInfo.SiteId)} = @{nameof(ItemInfo.SiteId)}, 
                {nameof(ItemInfo.ChannelId)} = @{nameof(ItemInfo.ChannelId)},
                {nameof(ItemInfo.ContentId)} = @{nameof(ItemInfo.ContentId)},
                {nameof(ItemInfo.Taxis)} = @{nameof(ItemInfo.Taxis)},
                {nameof(ItemInfo.Title)} = @{nameof(ItemInfo.Title)}, 
                {nameof(ItemInfo.SubTitle)} = @{nameof(ItemInfo.SubTitle)}, 
                {nameof(ItemInfo.ImageUrl)} = @{nameof(ItemInfo.ImageUrl)}, 
                {nameof(ItemInfo.LinkUrl)} = @{nameof(ItemInfo.LinkUrl)}, 
                {nameof(ItemInfo.Count)} = @{nameof(ItemInfo.Count)}
            WHERE {nameof(ItemInfo.Id)} = @{nameof(ItemInfo.Id)}";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter(nameof(itemInfo.SiteId), itemInfo.SiteId),
                _helper.GetParameter(nameof(itemInfo.ChannelId), itemInfo.ChannelId),
                _helper.GetParameter(nameof(itemInfo.ContentId), itemInfo.ContentId),
                _helper.GetParameter(nameof(itemInfo.Taxis), itemInfo.Taxis),
                _helper.GetParameter(nameof(itemInfo.Title), itemInfo.Title),
                _helper.GetParameter(nameof(itemInfo.SubTitle), itemInfo.SubTitle),
                _helper.GetParameter(nameof(itemInfo.ImageUrl), itemInfo.ImageUrl),
                _helper.GetParameter(nameof(itemInfo.LinkUrl), itemInfo.LinkUrl),
                _helper.GetParameter(nameof(itemInfo.Count), itemInfo.Count),
                _helper.GetParameter(nameof(itemInfo.Id), itemInfo.Id)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parameters.ToArray());
        }

        public void DeleteAll(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return;

            string sqlString =
                $"DELETE FROM {TableName} WHERE {nameof(ItemInfo.SiteId)} = {siteId} AND {nameof(ItemInfo.ChannelId)} = {channelId} AND {nameof(ItemInfo.ContentId)} = {contentId}";
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
            {nameof(ItemInfo.SiteId)}, 
            {nameof(ItemInfo.ChannelId)}, 
            {nameof(ItemInfo.ContentId)}, 
            {nameof(ItemInfo.Taxis)}, 
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

        public List<ItemInfo> GetItemInfoList(int siteId, int channelId, int contentId)
        {
            int totalCount;
            return GetItemInfoList(siteId, channelId, contentId, out totalCount);
        }

        public List<ItemInfo> GetItemInfoList(int siteId, int channelId, int contentId, out int totalCount)
        {
            totalCount = 0;
            var list = new List<ItemInfo>();

            string sqlString = $@"SELECT {nameof(ItemInfo.Id)}, 
            {nameof(ItemInfo.SiteId)}, 
            {nameof(ItemInfo.ChannelId)}, 
            {nameof(ItemInfo.ContentId)},
            {nameof(ItemInfo.Taxis)},
            {nameof(ItemInfo.Title)},
            {nameof(ItemInfo.SubTitle)},
            {nameof(ItemInfo.ImageUrl)}, 
            {nameof(ItemInfo.LinkUrl)}, 
            {nameof(ItemInfo.Count)}
            FROM {TableName} WHERE {nameof(ItemInfo.SiteId)} = {siteId} AND {nameof(ItemInfo.ChannelId)} = {channelId} AND {nameof(ItemInfo.ContentId)} = {contentId}";

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

        public void AddCount(int siteId, int channelId, int contentId, List<int> itemIdList)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0 || itemIdList == null || itemIdList.Count <= 0) return;

            string sqlString =
                $"UPDATE {TableName} SET Count = Count + 1 WHERE {nameof(ItemInfo.Id)} IN ({string.Join(",", itemIdList)}) AND {nameof(ItemInfo.SiteId)} = {siteId} AND {nameof(ItemInfo.ChannelId)} = {channelId} AND {nameof(ItemInfo.ContentId)} = {contentId}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        private int GetMaxTaxis(int siteId, int channelId, int contentId)
        {
            string sqlString =
                $"SELECT MAX(Taxis) AS MaxTaxis FROM {TableName} WHERE {nameof(ItemInfo.SiteId)} = {siteId} AND {nameof(ItemInfo.ChannelId)} = {channelId} AND {nameof(ItemInfo.ContentId)} = {contentId}";
            var maxTaxis = 0;

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    maxTaxis = rdr.GetInt32(0);
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        public void TaxisDown(int id)
        {
            var itemInfo = GetItemInfo(id);
            if (itemInfo == null) return;

            var sqlString = _helper.GetPageSqlString(TableName, "Id, Taxis", $"WHERE {nameof(ItemInfo.SiteId)} = @{nameof(ItemInfo.SiteId)} AND {nameof(ItemInfo.ChannelId)} = @{nameof(ItemInfo.ChannelId)} AND {nameof(ItemInfo.ContentId)} = @{nameof(ItemInfo.ContentId)} AND Taxis > (SELECT Taxis FROM {TableName} WHERE Id = @Id)", "ORDER BY Taxis", 0, 1);

            var higherId = 0;
            var higherTaxis = 0;

            var parms = new[]
            {
                _helper.GetParameter(nameof(itemInfo.SiteId), itemInfo.SiteId),
                _helper.GetParameter(nameof(itemInfo.ChannelId), itemInfo.ChannelId),
                _helper.GetParameter(nameof(itemInfo.ContentId), itemInfo.ContentId),
                _helper.GetParameter(nameof(itemInfo.Id), id)
            };

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    higherId = rdr.GetInt32(0);
                    higherTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            if (higherId != 0)
            {
                SetTaxis(id, higherTaxis);
                SetTaxis(higherId, itemInfo.Taxis);
            }
        }

        public void TaxisUp(int id)
        {
            var itemInfo = GetItemInfo(id);
            if (itemInfo == null) return;

            var sqlString = _helper.GetPageSqlString(TableName, "Id, Taxis", $"WHERE {nameof(ItemInfo.SiteId)} = @{nameof(ItemInfo.SiteId)} AND {nameof(ItemInfo.ChannelId)} = @{nameof(ItemInfo.ChannelId)} AND {nameof(ItemInfo.ContentId)} = @{nameof(ItemInfo.ContentId)} AND Taxis < (SELECT Taxis FROM {TableName} WHERE Id = @Id)", "ORDER BY Taxis DESC", 0, 1);
            var lowerId = 0;
            var lowerTaxis = 0;

            var parms = new[]
            {
                _helper.GetParameter(nameof(itemInfo.SiteId), itemInfo.SiteId),
                _helper.GetParameter(nameof(itemInfo.ChannelId), itemInfo.ChannelId),
                _helper.GetParameter(nameof(itemInfo.ContentId), itemInfo.ContentId),
                _helper.GetParameter(nameof(itemInfo.Id), id)
            };

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    lowerId = rdr.GetInt32(0);
                    lowerTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            if (lowerId != 0)
            {
                SetTaxis(id, lowerTaxis);
                SetTaxis(lowerId, itemInfo.Taxis);
            }
        }

        private void SetTaxis(int id, int taxis)
        {
            var sqlString = $"UPDATE {TableName} SET Taxis = @Taxis WHERE Id = @Id";

            var parms = new[]
            {
                _helper.GetParameter(nameof(ItemInfo.Taxis), taxis),
                _helper.GetParameter(nameof(ItemInfo.Id), id)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parms);
        }

        private static ItemInfo GetPollItemInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var itemInfo = new ItemInfo();

            var i = 0;
            itemInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            itemInfo.SiteId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            itemInfo.ChannelId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            itemInfo.ContentId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            itemInfo.Taxis = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
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
