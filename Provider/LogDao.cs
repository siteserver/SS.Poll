using System;
using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Poll.Models;

namespace SS.Poll.Provider
{
    public static class LogDao
    {
        public const string TableName = "ss_poll_log";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(LogInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.ChannelId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.ContentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.ItemIds),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.UniqueId),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.AddDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.AttributeValues),
                DataType = DataType.Text
            }
        };

        public static void Insert(LogInfo logInfo)
        {
            string sqlString = $@"INSERT INTO {TableName}
(
    {nameof(LogInfo.SiteId)}, 
    {nameof(LogInfo.ChannelId)}, 
    {nameof(LogInfo.ContentId)}, 
    {nameof(LogInfo.ItemIds)},
    {nameof(LogInfo.UniqueId)},
    {nameof(LogInfo.AddDate)},
    {nameof(LogInfo.AttributeValues)}
) VALUES (
    @{nameof(LogInfo.SiteId)}, 
    @{nameof(LogInfo.ChannelId)}, 
    @{nameof(LogInfo.ContentId)}, 
    @{nameof(LogInfo.ItemIds)}, 
    @{nameof(LogInfo.UniqueId)},
    @{nameof(LogInfo.AddDate)},
    @{nameof(LogInfo.AttributeValues)}
)";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(logInfo.SiteId), logInfo.SiteId),
                Context.DatabaseApi.GetParameter(nameof(logInfo.ChannelId), logInfo.ChannelId),
                Context.DatabaseApi.GetParameter(nameof(logInfo.ContentId), logInfo.ContentId),
                Context.DatabaseApi.GetParameter(nameof(logInfo.ItemIds), logInfo.ItemIds),
                Context.DatabaseApi.GetParameter(nameof(logInfo.UniqueId), logInfo.UniqueId),
                Context.DatabaseApi.GetParameter(nameof(logInfo.AddDate), logInfo.AddDate),
                Context.DatabaseApi.GetParameter(nameof(logInfo.AttributeValues), logInfo.ToString())
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters.ToArray());
        }

        public static void DeleteAll(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return;

            string sqlString = $"DELETE FROM {TableName} WHERE {nameof(LogInfo.SiteId)} = {siteId} AND {nameof(LogInfo.ChannelId)} = {channelId} AND {nameof(LogInfo.ContentId)} = {contentId}";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        }

        public static void Delete(List<int> logIdList)
        {
            if (logIdList == null || logIdList.Count <= 0) return;
            string sqlString =
                $"DELETE FROM {TableName} WHERE {nameof(LogInfo.Id)} IN ({string.Join(",", logIdList)})";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        }

        public static int GetCount(int siteId, int channelId, int contentId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(LogInfo.SiteId)} = {siteId} AND {nameof(LogInfo.ChannelId)} = {channelId} AND {nameof(LogInfo.ContentId)} = {contentId}";

            var count = 0;

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    count = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return count;
        }

        public static bool IsExists(int siteId, int channelId, int contentId, string uniqueId)
        {
            var sqlString =
                $"SELECT Id FROM {TableName} WHERE {nameof(LogInfo.SiteId)} = {siteId} AND {nameof(LogInfo.ChannelId)} = {channelId} AND {nameof(LogInfo.ContentId)} = {contentId} AND {nameof(LogInfo.UniqueId)} = @{nameof(LogInfo.UniqueId)}";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(LogInfo.UniqueId), uniqueId)
            };

            var exists = false;

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parameters.ToArray()))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public static List<LogInfo> GetPollLogInfoList(int siteId, int channelId, int contentId, int totalCount, int limit, int offset)
        {
            var pollLogInfoList = new List<LogInfo>();

            string sqlString =
                $@"SELECT {nameof(LogInfo.Id)},
    {nameof(LogInfo.SiteId)}, 
    {nameof(LogInfo.ChannelId)}, 
    {nameof(LogInfo.ContentId)}, 
    {nameof(LogInfo.ItemIds)},
    {nameof(LogInfo.UniqueId)},
    {nameof(LogInfo.AddDate)},
    {nameof(LogInfo.AttributeValues)}
            FROM {TableName} WHERE {nameof(LogInfo.SiteId)} = {siteId} AND {nameof(LogInfo.ChannelId)} = {channelId} AND {nameof(LogInfo.ContentId)} = {contentId}";

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var pollLogInfo = GetPollItemInfo(rdr);
                    if (!string.IsNullOrEmpty(pollLogInfo.AttributeValues))
                    {
                        pollLogInfoList.Add(pollLogInfo);
                    }
                }
                rdr.Close();
            }

            return pollLogInfoList;
        }

        public static List<LogInfo> GetAllPollLogInfoList(int siteId, int channelId, int contentId)
        {
            var pollLogInfoList = new List<LogInfo>();

            string sqlString =
                $@"SELECT {nameof(LogInfo.Id)},
    {nameof(LogInfo.SiteId)}, 
    {nameof(LogInfo.ChannelId)}, 
    {nameof(LogInfo.ContentId)}, 
    {nameof(LogInfo.ItemIds)},
    {nameof(LogInfo.UniqueId)},
    {nameof(LogInfo.AddDate)},
    {nameof(LogInfo.AttributeValues)}
            FROM {TableName} WHERE {nameof(LogInfo.SiteId)} = {siteId} AND {nameof(LogInfo.ChannelId)} = {channelId} AND {nameof(LogInfo.ContentId)} = {contentId}";

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var pollLogInfo = GetPollItemInfo(rdr);
                    if (!string.IsNullOrEmpty(pollLogInfo.AttributeValues))
                    {
                        pollLogInfoList.Add(pollLogInfo);
                    }
                }
                rdr.Close();
            }

            return pollLogInfoList;
        }

        private static LogInfo GetPollItemInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var logInfo = new LogInfo();

            var i = 0;
            logInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            logInfo.SiteId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            logInfo.ChannelId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            logInfo.ContentId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            logInfo.ItemIds = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            logInfo.UniqueId = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            logInfo.AddDate = rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);
            i++;
            logInfo.AttributeValues = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);

            logInfo.Load(logInfo.AttributeValues);

            return logInfo;
        }
    }
}
