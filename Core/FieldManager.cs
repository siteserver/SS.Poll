using System.Collections.Generic;
using System.Data;
using SS.Poll.Models;
using SS.Poll.Provider;

namespace SS.Poll.Core
{
    public sealed class FieldManager
    {
        public static List<FieldInfo> GetFieldInfoList(int siteId, int channelId, int contentId)
        {
            var cacheKey = CacheUtils.GetCacheKey(nameof(FieldManager), nameof(GetFieldInfoList), siteId.ToString(), channelId.ToString(), contentId.ToString());
            var list = CacheUtils.Get<List<FieldInfo>>(cacheKey);
            if (list != null) return list;

            list = FieldDao.GetFieldInfoList(siteId, channelId, contentId, true);
            CacheUtils.Insert(cacheKey, list);
            return list;
        }

        public static DataSet GetFieldItemDataSet(int styleItemCount, List<FieldItemInfo> itemInfoList)
        {
            var dataset = new DataSet();

            var dataTable = new DataTable("Items");

            dataTable.Columns.Add(new DataColumn
            {
                DataType = System.Type.GetType("System.Int32"),
                ColumnName = nameof(FieldItemInfo.Id)
            });

            dataTable.Columns.Add(new DataColumn
            {
                DataType = System.Type.GetType("System.Int32"),
                ColumnName = nameof(FieldItemInfo.FieldId)
            });

            dataTable.Columns.Add(new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = nameof(FieldItemInfo.Value)
            });

            dataTable.Columns.Add(new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = nameof(FieldItemInfo.IsSelected)
            });

            for (var i = 0; i < styleItemCount; i++)
            {
                var dataRow = dataTable.NewRow();

                var itemInfo = itemInfoList != null && itemInfoList.Count > i ? itemInfoList[i] : new FieldItemInfo();

                dataRow[nameof(FieldItemInfo.Id)] = itemInfo.Id;
                dataRow[nameof(FieldItemInfo.FieldId)] = itemInfo.FieldId;
                dataRow[nameof(FieldItemInfo.Value)] = itemInfo.Value;
                dataRow[nameof(FieldItemInfo.IsSelected)] = itemInfo.IsSelected.ToString();

                dataTable.Rows.Add(dataRow);
            }

            dataset.Tables.Add(dataTable);
            return dataset;
        }

    }

}
