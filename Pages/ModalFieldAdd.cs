using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SS.Poll.Core;
using SS.Poll.Models;

namespace SS.Poll.Pages
{
    public class ModalFieldAdd : PageBase
    {
        public Literal LtlMessage;

        public TextBox TbAttributeName;
        public TextBox TbDisplayName;
        public TextBox TbPlaceHolder;
        public DropDownList DdlIsDisabled;
        public DropDownList DdlFieldType;
        public DropDownList DdlItemType;
        public PlaceHolder PhItemCount;
        public TextBox TbItemCount;
        public TextBox TbItemValues;
        public Repeater RptItems;
        public PlaceHolder PhItemsType;
        public PlaceHolder PhItemsRapid;
        public PlaceHolder PhItems;

        private int _fieldId;

        public static string GetOpenWindowString(int siteId, int channelId, int contentId, int fieldId)
        {
            return LayerUtils.GetOpenScript(fieldId > 0 ? "编辑字段" : "新增字段", $"{nameof(ModalFieldAdd)}.aspx?siteId={siteId}&channelId={channelId}&contentId={contentId}&fieldId={fieldId}", 650, 600);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);

            if (IsPostBack) return;

            var fieldInfo = _fieldId > 0 ? Main.FieldDao.GetFieldInfo(_fieldId, true) : new FieldInfo();

            FieldTypeUtils.AddListItems(DdlFieldType);

            DdlItemType.SelectedValue = fieldInfo.Id != 0 ? false.ToString() : true.ToString();

            TbAttributeName.Text = fieldInfo.AttributeName;
            TbDisplayName.Text = fieldInfo.DisplayName;
            TbPlaceHolder.Text = fieldInfo.PlaceHolder;
            Utils.SelectListItems(DdlFieldType, fieldInfo.FieldType);

            Utils.SelectListItems(DdlIsDisabled, fieldInfo.IsDisabled.ToString());

            TbItemCount.Text = fieldInfo.Items.Count.ToString();
            RptItems.DataSource = FieldManager.GetFieldItemDataSet(fieldInfo.Items.Count, fieldInfo.Items);
            RptItems.ItemDataBound += RptItems_ItemDataBound;
            RptItems.DataBind();

            var isSelected = false;
            var list = new List<string>();
            foreach (var item in fieldInfo.Items)
            {
                list.Add(item.Value);
                if (item.IsSelected)
                {
                    isSelected = true;
                }
            }

            DdlItemType.SelectedValue = (!isSelected).ToString();
            TbItemValues.Text = string.Join(",", list);

            ReFresh(null, EventArgs.Empty);
        }

        private static void RptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var cbIsSelected = (CheckBox) e.Item.FindControl("CbIsSelected");
            cbIsSelected.Checked = Utils.EvalBool(e.Item.DataItem, "IsSelected");
        }

        public void ReFresh(object sender, EventArgs e)
        {
            PhItemsType.Visible = PhItemsRapid.Visible = PhItems.Visible = PhItemCount.Visible = false;

            var fieldType = DdlFieldType.SelectedValue;
            if (Utils.IsSelectFieldType(fieldType))
            {
                PhItemsType.Visible = true;
                var isRapid = Convert.ToBoolean(DdlItemType.SelectedValue);
                if (isRapid)
                {
                    PhItemsRapid.Visible = true;
                    PhItemCount.Visible = false;
                    PhItems.Visible = false;
                }
                else
                {
                    PhItemsRapid.Visible = false;
                    PhItemCount.Visible = true;
                    SetCount_OnClick(sender, e);
                }
            }
        }

        public void SetCount_OnClick(object sender, EventArgs e)
        {
            var count = Convert.ToInt32(TbItemCount.Text);
            if (count > 0)
            {
                PhItems.Visible = true;
                var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);

                List<FieldItemInfo> items = null;
                if (fieldId > 0)
                {
                    items = Main.FieldItemDao.GetItemInfoList(fieldId);
                }
                RptItems.DataSource = FieldManager.GetFieldItemDataSet(count, items);
                RptItems.DataBind();
            }
            else
            {
                PhItems.Visible = false;
            }
        }

        public void Add_OnClick(object sender, EventArgs e)
        {
            var fieldType = DdlFieldType.SelectedValue;

            if (Utils.IsSelectFieldType(fieldType))
            {
                var isRapid = Convert.ToBoolean(DdlItemType.SelectedValue);
                if (!isRapid)
                {
                    var itemCount = Convert.ToInt32(TbItemCount.Text);
                    if (itemCount == 0)
                    {
                        LtlMessage.Text = Utils.GetMessageHtml("操作失败，选项数目必须大于0！", false);
                        return;
                    }
                }
            }

            var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);

            var isChanged = fieldId > 0 ? UpdateFieldInfo(fieldId, fieldType) : InsertFieldInfo(fieldType);

            if (isChanged)
            {
                LayerUtils.Close(Page);
            }
        }

        private bool UpdateFieldInfo(int fieldId, string fieldType)
        {
            var isChanged = false;

            var fieldInfo = Main.FieldDao.GetFieldInfo(fieldId, true);

            if (fieldInfo.AttributeName != TbAttributeName.Text &&
                Main.FieldDao.IsExists(SiteId, ChannelId, ContentId, TbAttributeName.Text))
            {
                LtlMessage.Text = Utils.GetMessageHtml($@"字段修改失败：字段名""{TbAttributeName.Text}""已存在", false);
                return false;
            }

            fieldInfo.AttributeName = TbAttributeName.Text;
            fieldInfo.DisplayName = TbDisplayName.Text;
            fieldInfo.PlaceHolder = TbPlaceHolder.Text;
            fieldInfo.IsDisabled = Convert.ToBoolean(DdlIsDisabled.SelectedValue);
            fieldInfo.FieldType = fieldType;

            List<FieldItemInfo> fieldItems = null;

            if (Utils.IsSelectFieldType(fieldType))
            {
                fieldItems = new List<FieldItemInfo>();

                var isRapid = Convert.ToBoolean(DdlItemType.SelectedValue);
                if (isRapid)
                {
                    var itemArray = TbItemValues.Text.Split(',');
                    if (itemArray.Length > 0)
                    {
                        foreach (var itemValue in itemArray)
                        {
                            fieldItems.Add(new FieldItemInfo
                            {
                                Id = 0,
                                FieldId = fieldId,
                                IsSelected = false,
                                Value = itemValue
                            });
                        }
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (RepeaterItem item in RptItems.Items)
                    {
                        var tbValue = (TextBox) item.FindControl("TbValue");
                        var cbIsSelected = (CheckBox) item.FindControl("CbIsSelected");

                        if ((Utils.EqualsIgnoreCase(fieldType, nameof(FieldType.Radio)) ||
                             Utils.EqualsIgnoreCase(fieldType, nameof(FieldType.SelectOne))) && isHasSelected &&
                            cbIsSelected.Checked)
                        {
                            LtlMessage.Text = Utils.GetMessageHtml("操作失败，只能有一个初始化时选定项！", false);
                            return false;
                        }
                        if (cbIsSelected.Checked) isHasSelected = true;

                        fieldItems.Add(new FieldItemInfo
                        {
                            Id = 0,
                            FieldId = fieldId,
                            IsSelected = cbIsSelected.Checked,
                            Value = tbValue.Text
                        });
                    }
                }
            }

            try
            {
                Main.FieldDao.Update(fieldInfo);
                Main.FieldItemDao.DeleteItems(fieldId);
                Main.FieldItemDao.InsertItems(fieldItems);
                isChanged = true;
            }
            catch (Exception ex)
            {
                LtlMessage.Text = Utils.GetMessageHtml($"字段修改失败：{ex.Message}！", false);
            }
            return isChanged;
        }

        private bool InsertFieldInfo(string fieldType)
        {
            var isChanged = false;

            if (Main.FieldDao.IsExists(SiteId, ChannelId, ContentId, TbAttributeName.Text))
            {
                LtlMessage.Text = Utils.GetMessageHtml($@"字段添加失败：字段名""{TbAttributeName.Text}""已存在", false);
                return false;
            }

            var fieldInfo = new FieldInfo
            {
                Id = 0,
                SiteId = SiteId,
                ChannelId = ChannelId,
                ContentId = ContentId,
                Taxis = 0,
                AttributeName = TbAttributeName.Text,
                DisplayName = TbDisplayName.Text,
                PlaceHolder = TbPlaceHolder.Text,
                IsDisabled = Convert.ToBoolean(DdlIsDisabled.SelectedValue),
                FieldType = fieldType
            };

            if (Utils.IsSelectFieldType(fieldType))
            {
                fieldInfo.Items = new List<FieldItemInfo>();

                var isRapid = Convert.ToBoolean(DdlItemType.SelectedValue);
                if (isRapid)
                {
                    var itemArray = TbItemValues.Text.Split(',');
                    if (itemArray.Length > 0)
                    {
                        foreach (var itemValue in itemArray)
                        {
                            fieldInfo.Items.Add(new FieldItemInfo
                            {
                                Id = 0,
                                FieldId = 0,
                                IsSelected = false,
                                Value = itemValue
                            });
                        }
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (RepeaterItem item in RptItems.Items)
                    {
                        var tbValue = (TextBox) item.FindControl("TbValue");
                        var cbIsSelected = (CheckBox) item.FindControl("CbIsSelected");

                        if ((Utils.EqualsIgnoreCase(fieldType, nameof(FieldType.Radio)) ||
                             Utils.EqualsIgnoreCase(fieldType, nameof(FieldType.SelectOne))) && isHasSelected &&
                            cbIsSelected.Checked)
                        {
                            LtlMessage.Text = Utils.GetMessageHtml("操作失败，只能有一个初始化时选定项！", false);
                            return false;
                        }
                        if (cbIsSelected.Checked) isHasSelected = true;

                        fieldInfo.Items.Add(new FieldItemInfo
                        {
                            Id = 0,
                            FieldId = 0,
                            IsSelected = cbIsSelected.Checked,
                            Value = tbValue.Text
                        });
                    }
                }
            }

            try
            {
                Main.FieldDao.Insert(fieldInfo);
                isChanged = true;
            }
            catch (Exception ex)
            {
                LtlMessage.Text = Utils.GetMessageHtml($"字段添加失败：{ex.Message}！", false);
            }
            return isChanged;
        }
    }
}
