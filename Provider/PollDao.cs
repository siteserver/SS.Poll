using System;
using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Poll.Models;

namespace SS.Poll.Provider
{
    public class PollDao
    {
        public const string TableName = "ss_poll";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(PollInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.ChannelId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.ContentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.IsImage),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.IsUrl),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.IsResult),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.IsTimeout),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.TimeToStart),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.TimeToEnd),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.IsCheckbox),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.CheckboxMin),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.CheckboxMax),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(PollInfo.IsProfile),
                DataType = DataType.Boolean
            },
        };

        private readonly string _connectionString;
        private readonly IDataApi _helper;

        public PollDao(string connectionString, IDataApi dataApi)
        {
            _connectionString = connectionString;
            _helper = dataApi;
        }

        public int Insert(PollInfo pollInfo)
        {
            int pollId;

            string sqlString = $@"INSERT INTO {TableName}
           ({nameof(PollInfo.SiteId)}, 
            {nameof(PollInfo.ChannelId)}, 
            {nameof(PollInfo.ContentId)}, 
            {nameof(PollInfo.IsImage)}, 
            {nameof(PollInfo.IsUrl)}, 
            {nameof(PollInfo.IsResult)}, 
            {nameof(PollInfo.IsTimeout)}, 
            {nameof(PollInfo.TimeToStart)}, 
            {nameof(PollInfo.TimeToEnd)}, 
            {nameof(PollInfo.IsCheckbox)}, 
            {nameof(PollInfo.CheckboxMin)},
            {nameof(PollInfo.CheckboxMax)}, 
            {nameof(PollInfo.IsProfile)})
     VALUES
           (@{nameof(PollInfo.SiteId)}, 
            @{nameof(PollInfo.ChannelId)}, 
            @{nameof(PollInfo.ContentId)}, 
            @{nameof(PollInfo.IsImage)}, 
            @{nameof(PollInfo.IsUrl)}, 
            @{nameof(PollInfo.IsResult)}, 
            @{nameof(PollInfo.IsTimeout)}, 
            @{nameof(PollInfo.TimeToStart)}, 
            @{nameof(PollInfo.TimeToEnd)}, 
            @{nameof(PollInfo.IsCheckbox)}, 
            @{nameof(PollInfo.CheckboxMin)},
            @{nameof(PollInfo.CheckboxMax)}, 
            @{nameof(PollInfo.IsProfile)})";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter(nameof(pollInfo.SiteId), pollInfo.SiteId),
                _helper.GetParameter(nameof(pollInfo.ChannelId), pollInfo.ChannelId),
                _helper.GetParameter(nameof(pollInfo.ContentId), pollInfo.ContentId),
                _helper.GetParameter(nameof(pollInfo.IsImage), pollInfo.IsImage),
                _helper.GetParameter(nameof(pollInfo.IsUrl), pollInfo.IsUrl),
                _helper.GetParameter(nameof(pollInfo.IsResult), pollInfo.IsResult),
                _helper.GetParameter(nameof(pollInfo.IsTimeout), pollInfo.IsTimeout),
                _helper.GetParameter(nameof(pollInfo.TimeToStart), pollInfo.TimeToStart),
                _helper.GetParameter(nameof(pollInfo.TimeToEnd), pollInfo.TimeToEnd),
                _helper.GetParameter(nameof(pollInfo.IsCheckbox), pollInfo.IsCheckbox),
                _helper.GetParameter(nameof(pollInfo.CheckboxMin), pollInfo.CheckboxMin),
                _helper.GetParameter(nameof(pollInfo.CheckboxMax), pollInfo.CheckboxMax),
                _helper.GetParameter(nameof(pollInfo.IsProfile), pollInfo.IsProfile)
            };

            using (var conn = _helper.GetConnection(_connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        pollId = _helper.ExecuteNonQueryAndReturnId(TableName, nameof(PollInfo.Id), trans, sqlString, parameters.ToArray());

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return pollId;
        }

        public void Update(PollInfo pollInfo)
        {
            string sqlString = $@"UPDATE {TableName} SET
                {nameof(PollInfo.SiteId)} = @{nameof(PollInfo.SiteId)}, 
                {nameof(PollInfo.ChannelId)} = @{nameof(PollInfo.ChannelId)}, 
                {nameof(PollInfo.ContentId)} = @{nameof(PollInfo.ContentId)}, 
                {nameof(PollInfo.IsImage)} = @{nameof(PollInfo.IsImage)}, 
                {nameof(PollInfo.IsUrl)} = @{nameof(PollInfo.IsUrl)}, 
                {nameof(PollInfo.IsResult)} = @{nameof(PollInfo.IsResult)}, 
                {nameof(PollInfo.IsTimeout)} = @{nameof(PollInfo.IsTimeout)}, 
                {nameof(PollInfo.TimeToStart)} = @{nameof(PollInfo.TimeToStart)}, 
                {nameof(PollInfo.TimeToEnd)} = @{nameof(PollInfo.TimeToEnd)}, 
                {nameof(PollInfo.IsCheckbox)} = @{nameof(PollInfo.IsCheckbox)},
                {nameof(PollInfo.CheckboxMin)} = @{nameof(PollInfo.CheckboxMin)},
                {nameof(PollInfo.CheckboxMax)} = @{nameof(PollInfo.CheckboxMax)}, 
                {nameof(PollInfo.IsProfile)} = @{nameof(PollInfo.IsProfile)}
            WHERE {nameof(PollInfo.Id)} = @{nameof(PollInfo.Id)}";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter(nameof(pollInfo.SiteId), pollInfo.SiteId),
                _helper.GetParameter(nameof(pollInfo.ChannelId), pollInfo.ChannelId),
                _helper.GetParameter(nameof(pollInfo.ContentId), pollInfo.ContentId),
                _helper.GetParameter(nameof(pollInfo.IsImage), pollInfo.IsImage),
                _helper.GetParameter(nameof(pollInfo.IsUrl), pollInfo.IsUrl),
                _helper.GetParameter(nameof(pollInfo.IsResult), pollInfo.IsResult),
                _helper.GetParameter(nameof(pollInfo.IsTimeout), pollInfo.IsTimeout),
                _helper.GetParameter(nameof(pollInfo.TimeToStart), pollInfo.TimeToStart),
                _helper.GetParameter(nameof(pollInfo.TimeToEnd), pollInfo.TimeToEnd),
                _helper.GetParameter(nameof(pollInfo.IsCheckbox), pollInfo.IsCheckbox),
                _helper.GetParameter(nameof(pollInfo.CheckboxMin), pollInfo.CheckboxMin),
                _helper.GetParameter(nameof(pollInfo.CheckboxMax), pollInfo.CheckboxMax),
                _helper.GetParameter(nameof(pollInfo.IsProfile), pollInfo.IsProfile),
                _helper.GetParameter(nameof(pollInfo.Id), pollInfo.Id)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parameters.ToArray());
        }

        public void Delete(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return;

            //DataProviderWx.PollLogDao.DeleteAll(pollId);
            //DataProviderWx.PollItemDao.DeleteAll(siteId, pollId);

            string sqlString = $"DELETE FROM {TableName} WHERE {nameof(PollInfo.SiteId)} = {siteId} AND {nameof(PollInfo.ChannelId)} = {channelId} AND {nameof(PollInfo.ContentId)} = {contentId}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        //public void Delete(int siteId, List<int> pollIdList)
        //{
        //    if (pollIdList == null || pollIdList.Count <= 0) return;

        //    //foreach (var pollId in pollIdList)
        //    //{
        //    //    //DataProviderWx.PollLogDao.DeleteAll(pollId);
        //    //    //DataProviderWx.PollItemDao.DeleteAll(siteId, pollId);
        //    //}

        //    string sqlString =
        //        $"DELETE FROM {TableName} WHERE {nameof(PollInfo.Id)} IN ({string.Join(",", pollIdList)})";
        //    _helper.ExecuteNonQuery(_connectionString, sqlString);
        //}

        public PollInfo GetPollInfo(int siteId, int channelId, int contentId)
        {
            PollInfo pollInfo = null;

            string sqlString = $@"SELECT {nameof(PollInfo.Id)}, 
            {nameof(PollInfo.SiteId)}, 
            {nameof(PollInfo.ChannelId)}, 
            {nameof(PollInfo.ContentId)}, 
            {nameof(PollInfo.IsImage)}, 
            {nameof(PollInfo.IsUrl)}, 
            {nameof(PollInfo.IsResult)}, 
            {nameof(PollInfo.IsTimeout)}, 
            {nameof(PollInfo.TimeToStart)}, 
            {nameof(PollInfo.TimeToEnd)}, 
            {nameof(PollInfo.IsCheckbox)},
            {nameof(PollInfo.CheckboxMin)},
            {nameof(PollInfo.CheckboxMax)}, 
            {nameof(PollInfo.IsProfile)}
            FROM {TableName} WHERE {nameof(PollInfo.SiteId)} = {siteId} AND {nameof(PollInfo.ChannelId)} = {channelId} AND {nameof(PollInfo.ContentId)} = {contentId}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    pollInfo = GetPollInfo(rdr);
                }
                rdr.Close();
            }

            return pollInfo;
        }

        public PollInfo GetPollInfo(int id)
        {
            PollInfo pollInfo = null;

            string sqlString = $@"SELECT {nameof(PollInfo.Id)}, 
            {nameof(PollInfo.SiteId)}, 
            {nameof(PollInfo.ChannelId)}, 
            {nameof(PollInfo.ContentId)}, 
            {nameof(PollInfo.IsImage)}, 
            {nameof(PollInfo.IsUrl)}, 
            {nameof(PollInfo.IsResult)}, 
            {nameof(PollInfo.IsTimeout)}, 
            {nameof(PollInfo.TimeToStart)}, 
            {nameof(PollInfo.TimeToEnd)}, 
            {nameof(PollInfo.IsCheckbox)},
            {nameof(PollInfo.CheckboxMin)},
            {nameof(PollInfo.CheckboxMax)}, 
            {nameof(PollInfo.IsProfile)}
            FROM {TableName} WHERE {nameof(PollInfo.Id)} = {id}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    pollInfo = GetPollInfo(rdr);
                }
                rdr.Close();
            }

            return pollInfo;
        }

        private static PollInfo GetPollInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;
            
            var pollInfo = new PollInfo();

            var i = 0;
            pollInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            pollInfo.SiteId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            pollInfo.ChannelId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            pollInfo.ContentId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            pollInfo.IsImage = !rdr.IsDBNull(i) && rdr.GetBoolean(i);
            i++;
            pollInfo.IsUrl = !rdr.IsDBNull(i) && rdr.GetBoolean(i);
            i++;
            pollInfo.IsResult = !rdr.IsDBNull(i) && rdr.GetBoolean(i);
            i++;
            pollInfo.IsTimeout = !rdr.IsDBNull(i) && rdr.GetBoolean(i);
            i++;
            pollInfo.TimeToStart = rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);
            i++;
            pollInfo.TimeToEnd = rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);
            i++;
            pollInfo.IsCheckbox = !rdr.IsDBNull(i) && rdr.GetBoolean(i);
            i++;
            pollInfo.CheckboxMin = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            pollInfo.CheckboxMax = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            pollInfo.IsProfile = !rdr.IsDBNull(i) && rdr.GetBoolean(i);

            return pollInfo;
        }

    }
}
