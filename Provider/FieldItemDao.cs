using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Poll.Models;

namespace SS.Poll.Provider
{
    public class FieldItemDao
    {
        public const string TableName = "ss_poll_field_item";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(FieldItemInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldItemInfo.FieldId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(FieldItemInfo.Value),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(FieldItemInfo.IsSelected),
                DataType = DataType.Boolean
            }
        };

        private readonly string _connectionString;
        private readonly IDataApi _helper;

        public FieldItemDao(string connectionString, IDataApi dataApi)
        {
            _connectionString = connectionString;
            _helper = dataApi;
        }

        private const string ParmFieldId = "@FieldId";
        private const string ParmValue = "@Value";
        private const string ParmIsSelected = "@IsSelected";

        public void Insert(IDbTransaction trans, FieldItemInfo itemInfo)
        {
            var sqlString = $@"INSERT INTO {TableName} (
    {nameof(FieldItemInfo.FieldId)},
    {nameof(FieldItemInfo.Value)},
    {nameof(FieldItemInfo.IsSelected)}
) VALUES (
    @{nameof(FieldItemInfo.FieldId)}, 
    @{nameof(FieldItemInfo.Value)},
    @{nameof(FieldItemInfo.IsSelected)}
)";

            var insertItemParms = new[]
            {
                _helper.GetParameter(ParmFieldId, itemInfo.FieldId),
                _helper.GetParameter(ParmValue, itemInfo.Value),
                _helper.GetParameter(ParmIsSelected, itemInfo.IsSelected)
            };

            _helper.ExecuteNonQuery(trans, sqlString, insertItemParms);
        }

        public void InsertItems(List<FieldItemInfo> items)
        {
            if (items == null || items.Count == 0) return;
            
            using (var conn = _helper.GetConnection(_connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var itemInfo in items)
                        {
                            Insert(trans, itemInfo);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void DeleteItems(int fieldId)
        {
            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(FieldItemInfo.FieldId)} = @{nameof(FieldItemInfo.FieldId)}";

            var parms = new []
			{
				_helper.GetParameter(ParmFieldId, fieldId)
			};

            _helper.ExecuteNonQuery(_connectionString, sqlString, parms);
        }

        public List<FieldItemInfo> GetItemInfoList(int fieldId)
        {
            var items = new List<FieldItemInfo>();

            var sqlString =
                $@"SELECT {nameof(FieldItemInfo.Id)}, {nameof(FieldItemInfo.FieldId)}, {nameof(FieldItemInfo.Value)}, {nameof(FieldItemInfo.IsSelected)} FROM {TableName} WHERE ({nameof(FieldItemInfo.FieldId)} = @{nameof(FieldItemInfo.FieldId)})";

            var parms = new []
			{
                _helper.GetParameter(ParmFieldId, fieldId)
			};

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parms))
            {
                while (rdr.Read())
                {
                    items.Add(GetFieldItemInfo(rdr));
                }
                rdr.Close();
            }

            return items;
        }

        private static FieldItemInfo GetFieldItemInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var itemInfo = new FieldItemInfo();

            var i = 0;
            itemInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            itemInfo.FieldId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            itemInfo.Value = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            itemInfo.IsSelected = !rdr.IsDBNull(i) && rdr.GetBoolean(i);

            return itemInfo;
        }
    }
}
