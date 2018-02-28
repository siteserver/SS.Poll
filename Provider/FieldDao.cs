using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Poll.Core;
using SS.Poll.Models;

namespace SS.Poll.Provider
{
    public class FieldDao
    {
        public const string TableName = "ss_poll_field";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.ChannelId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.ContentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.AttributeName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.AttributeValue),
                DataType = DataType.Text
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.DisplayName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.PlaceHolder),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.IsDisabled),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.FieldType),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(FieldInfo.FieldSettings),
                DataType = DataType.Text
            }
        };

        private readonly string _connectionString;
        private readonly IDataApi _helper;

        public FieldDao(string connectionString, IDataApi dataApi)
        {
            _connectionString = connectionString;
            _helper = dataApi;
        }

        private const string ParmId = "@Id";
        private const string ParmSiteId = "@SiteId";
        private const string ParmChannelId = "@ChannelId";
        private const string ParmContentId = "@ContentId";
        private const string ParmTaxis = "@Taxis";
        private const string ParmAttributeName = "@AttributeName";
        private const string ParmAttributeValue = "@AttributeValue";
        private const string ParmDisplayName = "@DisplayName";
        private const string ParmPlaceHolder = "@PlaceHolder";
        private const string ParmIsDisabled = "@IsDisabled";
        private const string ParmFieldType = "@FieldType";
        private const string ParmFieldSettings = "@FieldSettings";

        public int Insert(FieldInfo fieldInfo)
        {
            fieldInfo.Taxis = GetMaxTaxis(fieldInfo.SiteId, fieldInfo.ChannelId, fieldInfo.ContentId) + 1;

            string sqlString = $@"INSERT INTO {TableName}
(
    {nameof(FieldInfo.SiteId)}, 
    {nameof(FieldInfo.ChannelId)}, 
    {nameof(FieldInfo.ContentId)}, 
    {nameof(FieldInfo.Taxis)},
    {nameof(FieldInfo.AttributeName)},
    {nameof(FieldInfo.AttributeValue)},
    {nameof(FieldInfo.DisplayName)},
    {nameof(FieldInfo.PlaceHolder)},
    {nameof(FieldInfo.IsDisabled)},
    {nameof(FieldInfo.FieldType)},
    {nameof(FieldInfo.FieldSettings)}
) VALUES (
    @{nameof(FieldInfo.SiteId)}, 
    @{nameof(FieldInfo.ChannelId)}, 
    @{nameof(FieldInfo.ContentId)}, 
    @{nameof(FieldInfo.Taxis)},
    @{nameof(FieldInfo.AttributeName)},
    @{nameof(FieldInfo.AttributeValue)},
    @{nameof(FieldInfo.DisplayName)},
    @{nameof(FieldInfo.PlaceHolder)},
    @{nameof(FieldInfo.IsDisabled)},
    @{nameof(FieldInfo.FieldType)},
    @{nameof(FieldInfo.FieldSettings)}
)";

            var parameters = new []
			{
				_helper.GetParameter(ParmSiteId, fieldInfo.SiteId),
                _helper.GetParameter(ParmChannelId, fieldInfo.ChannelId),
                _helper.GetParameter(ParmContentId, fieldInfo.ContentId),
                _helper.GetParameter(ParmTaxis, fieldInfo.Taxis),
                _helper.GetParameter(ParmAttributeName, fieldInfo.AttributeName),
                _helper.GetParameter(ParmAttributeValue, fieldInfo.AttributeValue),
                _helper.GetParameter(ParmDisplayName, fieldInfo.DisplayName),
                _helper.GetParameter(ParmPlaceHolder, fieldInfo.PlaceHolder),
                _helper.GetParameter(ParmIsDisabled, fieldInfo.IsDisabled),
                _helper.GetParameter(ParmFieldType, fieldInfo.FieldType),
                _helper.GetParameter(ParmFieldSettings, fieldInfo.FieldSettings)
			};

            return _helper.ExecuteNonQueryAndReturnId(TableName, nameof(FieldInfo.Id), _connectionString, sqlString, parameters);
        }

        public void Update(FieldInfo info)
        {
            string sqlString = $@"UPDATE {TableName} SET
                {nameof(FieldInfo.SiteId)} = @{nameof(FieldInfo.SiteId)}, 
                {nameof(FieldInfo.ChannelId)} = @{nameof(FieldInfo.ChannelId)}, 
                {nameof(FieldInfo.ContentId)} = @{nameof(FieldInfo.ContentId)}, 
                {nameof(FieldInfo.Taxis)} = @{nameof(FieldInfo.Taxis)},
                {nameof(FieldInfo.AttributeName)} = @{nameof(FieldInfo.AttributeName)},
                {nameof(FieldInfo.AttributeValue)} = @{nameof(FieldInfo.AttributeValue)},
                {nameof(FieldInfo.DisplayName)} = @{nameof(FieldInfo.DisplayName)},
                {nameof(FieldInfo.PlaceHolder)} = @{nameof(FieldInfo.PlaceHolder)},
                {nameof(FieldInfo.IsDisabled)} = @{nameof(FieldInfo.IsDisabled)},
                {nameof(FieldInfo.FieldType)} = @{nameof(FieldInfo.FieldType)},
                {nameof(FieldInfo.FieldSettings)} = @{nameof(FieldInfo.FieldSettings)}
            WHERE {nameof(FieldInfo.Id)} = @{nameof(FieldInfo.Id)}";

            var updateParms = new []
			{
				_helper.GetParameter(ParmSiteId, info.SiteId),
                _helper.GetParameter(ParmChannelId, info.ChannelId),
                _helper.GetParameter(ParmContentId, info.ContentId),
                _helper.GetParameter(ParmTaxis, info.Taxis),
                _helper.GetParameter(ParmAttributeName, info.AttributeName),
                _helper.GetParameter(ParmAttributeValue, info.AttributeValue),
                _helper.GetParameter(ParmDisplayName, info.DisplayName),
                _helper.GetParameter(ParmPlaceHolder, info.PlaceHolder),
                _helper.GetParameter(ParmIsDisabled, info.IsDisabled),
                _helper.GetParameter(ParmFieldType, info.FieldType),
                _helper.GetParameter(ParmFieldSettings, info.FieldSettings),
                _helper.GetParameter(ParmId, info.Id)
			};

            _helper.ExecuteNonQuery(_connectionString, sqlString, updateParms);
        }

        public void Delete(int fieldId)
        {
            string sqlString = $"DELETE FROM {TableName} WHERE {nameof(FieldInfo.Id)} = @{nameof(FieldInfo.Id)}";

            var parms = new []
            {
                _helper.GetParameter(ParmId, fieldId)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parms);

            Main.FieldItemDao.DeleteItems(fieldId);
        }

        public List<FieldInfo> GetFieldInfoList(int siteId, int channelId, int contentId, bool isItems)
        {
            var list = new List<FieldInfo>();

            string sqlString =
                $@"SELECT
    {nameof(FieldInfo.Id)}, 
    {nameof(FieldInfo.SiteId)}, 
    {nameof(FieldInfo.ChannelId)}, 
    {nameof(FieldInfo.ContentId)}, 
    {nameof(FieldInfo.Taxis)},
    {nameof(FieldInfo.AttributeName)},
    {nameof(FieldInfo.AttributeValue)},
    {nameof(FieldInfo.DisplayName)},
    {nameof(FieldInfo.PlaceHolder)},
    {nameof(FieldInfo.IsDisabled)},
    {nameof(FieldInfo.FieldType)},
    {nameof(FieldInfo.FieldSettings)}
FROM {TableName} 
WHERE
    {nameof(FieldInfo.SiteId)} = @{nameof(FieldInfo.SiteId)} AND 
    {nameof(FieldInfo.ChannelId)} = @{nameof(FieldInfo.ChannelId)} AND 
    {nameof(FieldInfo.ContentId)} = @{nameof(FieldInfo.ContentId)}
ORDER BY {nameof(FieldInfo.Taxis)}";

            var parameters = new[]
            {
                _helper.GetParameter(ParmSiteId, siteId),
                _helper.GetParameter(ParmChannelId, channelId),
                _helper.GetParameter(ParmContentId, contentId)
            };

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parameters))
            {
                while (rdr.Read())
                {
                    var fieldInfo = GetFieldInfo(rdr);
                    if (fieldInfo != null)
                    {
                        if (isItems)
                        {
                            if (Utils.EqualsIgnoreCase(fieldInfo.FieldType, nameof(FieldType.CheckBox)) || Utils.EqualsIgnoreCase(fieldInfo.FieldType, nameof(FieldType.Radio)) || Utils.EqualsIgnoreCase(fieldInfo.FieldType, nameof(FieldType.SelectMultiple)) || Utils.EqualsIgnoreCase(fieldInfo.FieldType, nameof(FieldType.SelectOne)))
                            {
                                var items = Main.FieldItemDao.GetItemInfoList(fieldInfo.Id);
                                if (items != null && items.Count > 0)
                                {
                                    fieldInfo.Items = items;
                                }
                            }
                        }
                        list.Add(fieldInfo);
                    }
                }
                rdr.Close();
            }

            return list;
        }

        public bool IsExists(int siteId, int channelId, int contentId, string attributeName)
        {
            var exists = false;

            string sqlString = $@"SELECT Id FROM {TableName} WHERE 
    {nameof(FieldInfo.SiteId)} = @{nameof(FieldInfo.SiteId)} AND 
    {nameof(FieldInfo.ChannelId)} = @{nameof(FieldInfo.ChannelId)} AND 
    {nameof(FieldInfo.ContentId)} = @{nameof(FieldInfo.ContentId)} AND 
    {nameof(FieldInfo.AttributeName)} = @{nameof(FieldInfo.AttributeName)}";

            var parms = new []
			{
                _helper.GetParameter(ParmSiteId, siteId),
                _helper.GetParameter(ParmChannelId, channelId),
                _helper.GetParameter(ParmContentId, contentId),
                _helper.GetParameter(ParmAttributeName, attributeName)
            };

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public int GetCount(int siteId, int channelId, int contentId)
        {
            string sqlString = $@"SELECT COUNT(*) FROM {TableName} WHERE 
    {nameof(FieldInfo.SiteId)} = @{nameof(FieldInfo.SiteId)} AND 
    {nameof(FieldInfo.ChannelId)} = @{nameof(FieldInfo.ChannelId)} AND 
    {nameof(FieldInfo.ContentId)} = @{nameof(FieldInfo.ContentId)}";

            var parms = new[]
            {
                _helper.GetParameter(ParmSiteId, siteId),
                _helper.GetParameter(ParmChannelId, channelId),
                _helper.GetParameter(ParmContentId, contentId)
            };

            return Main.Dao.GetIntResult(sqlString, parms);
        }

        public FieldInfo GetFieldInfo(int id, bool isItems)
        {
            FieldInfo fieldInfo = null;

            string sqlString =
                $@"SELECT
    {nameof(FieldInfo.Id)}, 
    {nameof(FieldInfo.SiteId)}, 
    {nameof(FieldInfo.ChannelId)}, 
    {nameof(FieldInfo.ContentId)}, 
    {nameof(FieldInfo.Taxis)},
    {nameof(FieldInfo.AttributeName)},
    {nameof(FieldInfo.AttributeValue)},
    {nameof(FieldInfo.DisplayName)},
    {nameof(FieldInfo.PlaceHolder)},
    {nameof(FieldInfo.IsDisabled)},
    {nameof(FieldInfo.FieldType)},
    {nameof(FieldInfo.FieldSettings)}
FROM {TableName} 
WHERE {nameof(FieldInfo.Id)} = @{nameof(FieldInfo.Id)}";

            var parms = new []
			{
                _helper.GetParameter(ParmId, id)
			};

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parms))
            {
                if (rdr.Read())
                {
                    fieldInfo = GetFieldInfo(rdr);
                }
                rdr.Close();
            }

            if (fieldInfo != null && isItems)
            {
                fieldInfo.Items = Main.FieldItemDao.GetItemInfoList(fieldInfo.Id);
            }

            return fieldInfo;
        }

        public FieldInfo GetFieldInfo(int siteId, int channelId, int contentId, string attributeName)
        {
            FieldInfo fieldInfo = null;

            string sqlString =
                $@"SELECT
    {nameof(FieldInfo.Id)}, 
    {nameof(FieldInfo.SiteId)}, 
    {nameof(FieldInfo.ChannelId)}, 
    {nameof(FieldInfo.ContentId)}, 
    {nameof(FieldInfo.Taxis)},
    {nameof(FieldInfo.AttributeName)},
    {nameof(FieldInfo.AttributeValue)},
    {nameof(FieldInfo.DisplayName)},
    {nameof(FieldInfo.PlaceHolder)},
    {nameof(FieldInfo.IsDisabled)},
    {nameof(FieldInfo.FieldType)},
    {nameof(FieldInfo.FieldSettings)}
FROM {TableName} 
WHERE 
    {nameof(FieldInfo.SiteId)} = @{nameof(FieldInfo.SiteId)} AND 
    {nameof(FieldInfo.ChannelId)} = @{nameof(FieldInfo.ChannelId)} AND 
    {nameof(FieldInfo.ContentId)} = @{nameof(FieldInfo.ContentId)} AND 
    {nameof(FieldInfo.AttributeName)} = @{nameof(FieldInfo.AttributeName)}";

            var parms = new []
			{
                _helper.GetParameter(ParmSiteId, siteId),
                _helper.GetParameter(ParmChannelId, channelId),
                _helper.GetParameter(ParmContentId, contentId),
                _helper.GetParameter(ParmAttributeName, attributeName)
            };

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parms))
            {
                if (rdr.Read())
                {
                    fieldInfo = GetFieldInfo(rdr);
                }
                rdr.Close();
            }

            return fieldInfo;
        }

        private int GetMaxTaxis(int siteId, int channelId, int contentId)
        {
            string sqlString =
                $"SELECT MAX(Taxis) AS MaxTaxis FROM {TableName} WHERE {nameof(FieldInfo.SiteId)} = {siteId} AND {nameof(FieldInfo.ChannelId)} = {channelId} AND {nameof(FieldInfo.ContentId)} = {contentId}";
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
            var fieldInfo = GetFieldInfo(id, false);
            if (fieldInfo == null) return;

            var sqlString = _helper.ToTopSqlString(TableName, "Id, Taxis", $"WHERE {nameof(FieldInfo.SiteId)} = @{nameof(FieldInfo.SiteId)} AND {nameof(FieldInfo.ChannelId)} = @{nameof(FieldInfo.ChannelId)} AND {nameof(FieldInfo.ContentId)} = @{nameof(FieldInfo.ContentId)} AND Taxis > (SELECT Taxis FROM {TableName} WHERE Id = @Id)", "ORDER BY Taxis", 1);

            var higherId = 0;
            var higherTaxis = 0;

            var parms = new []
            {
                _helper.GetParameter(ParmSiteId, fieldInfo.SiteId),
                _helper.GetParameter(ParmChannelId, fieldInfo.ChannelId),
                _helper.GetParameter(ParmContentId, fieldInfo.ContentId),
                _helper.GetParameter(ParmId, id)
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
                SetTaxis(higherId, fieldInfo.Taxis);
            }
        }

        public void TaxisUp(int id)
        {
            var fieldInfo = GetFieldInfo(id, false);
            if (fieldInfo == null) return;

            var sqlString = _helper.ToTopSqlString(TableName, "Id, Taxis", $"WHERE {nameof(FieldInfo.SiteId)} = @{nameof(FieldInfo.SiteId)} AND {nameof(FieldInfo.ChannelId)} = @{nameof(FieldInfo.ChannelId)} AND {nameof(FieldInfo.ContentId)} = @{nameof(FieldInfo.ContentId)} AND Taxis < (SELECT Taxis FROM {TableName} WHERE Id = @Id)", "ORDER BY Taxis DESC", 1);
            var lowerId = 0;
            var lowerTaxis = 0;

            var parms = new []
            {
                _helper.GetParameter(ParmSiteId, fieldInfo.SiteId),
                _helper.GetParameter(ParmChannelId, fieldInfo.ChannelId),
                _helper.GetParameter(ParmContentId, fieldInfo.ContentId),
                _helper.GetParameter(ParmId, id)
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
                SetTaxis(lowerId, fieldInfo.Taxis);
            }
        }

        private void SetTaxis(int id, int taxis)
        {
            var sqlString = $"UPDATE {TableName} SET Taxis = @Taxis WHERE Id = @Id";

            var parms = new []
			{
				_helper.GetParameter(ParmTaxis, taxis),
                _helper.GetParameter(ParmId, id)
			};

            _helper.ExecuteNonQuery(_connectionString, sqlString, parms);
        }

        private static FieldInfo GetFieldInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var fieldInfo = new FieldInfo();

            var i = 0;
            fieldInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            fieldInfo.SiteId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            fieldInfo.ChannelId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            fieldInfo.ContentId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            fieldInfo.Taxis = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            fieldInfo.AttributeName = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            fieldInfo.AttributeValue = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            fieldInfo.DisplayName = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            fieldInfo.PlaceHolder = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            fieldInfo.IsDisabled = !rdr.IsDBNull(i) && rdr.GetBoolean(i);
            i++;
            fieldInfo.FieldType = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            fieldInfo.FieldSettings = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);

            return fieldInfo;
        }
    }
}
