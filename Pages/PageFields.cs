using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Poll.Core;
using SS.Poll.Model;

namespace SS.Poll.Pages
{
    public class PageFields : Page
    {
        public Literal LtlMessage;
        public DataGrid DgContents;
        public Button BtnAddField;
        public Button BtnAddFields;
        public Button BtnImport;
        public Button BtnExport;
        public Button BtnReturn;

        public PlaceHolder PhModalAdd;
        public Literal LtlModalAddTitle;
        public Literal LtlModalAddMessage;
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

        public PlaceHolder PhModalValidate;
        public Literal LtlModalValidateMessage;
        public DropDownList DdlIsValidate;
        public PlaceHolder PhValidate;
        public DropDownList DdlIsRequired;
        public PlaceHolder PhNum;
        public TextBox TbMinNum;
        public TextBox TbMaxNum;
        public DropDownList DdlValidateType;
        public TextBox TbErrorMessage;

        public Literal LtlScript;

        private string _apiUrl;
        private int _siteId;
        private int _channelId;
        private int _contentId;
        private string _returnUrl;

        public static string GetRedirectUrl(string apiUrl, int siteId, int channelId, int contentId, string returnUrl)
        {
            return
                Main.Instance.PluginApi.GetPluginUrl(
                    $"{nameof(PageFields)}.aspx?apiUrl={HttpUtility.UrlEncode(apiUrl)}&siteId={siteId}&channelId={channelId}&contentId={contentId}&returnUrl={HttpUtility.UrlEncode(returnUrl)}");
        }

        private string PageUrl => GetRedirectUrl(_apiUrl, _siteId, _channelId, _contentId, _returnUrl);

        public string UrlEditItems => Main.Instance.PluginApi.GetPluginUrl($"build/index.html?pageType=edit_items&apiUrl={HttpUtility.UrlEncode(_apiUrl)}&siteId={_siteId}&channelId={_channelId}&contentId={_contentId}&returnUrl={HttpUtility.UrlEncode(_returnUrl)}");

        public string UrlEditSettings
            => PageSettings.GetRedirectUrl(_apiUrl, _siteId, _channelId, _contentId, _returnUrl);

        public string UrlReturn => _returnUrl;

        public void Page_Load(object sender, EventArgs e)
        {
            _apiUrl = HttpUtility.UrlDecode(Request.QueryString["apiUrl"]);
            _siteId = Convert.ToInt32(Request.QueryString["siteId"]);
            _channelId = Convert.ToInt32(Request.QueryString["channelId"]);
            _contentId = Convert.ToInt32(Request.QueryString["contentId"]);
            _returnUrl = HttpUtility.UrlDecode(Request.QueryString["returnUrl"]);

            if (!Main.Instance.AdminApi.IsSiteAuthorized(_siteId))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
                return;
            }

            if (IsPostBack) return;

            //删除样式
            if (!string.IsNullOrEmpty(Request.QueryString["delete"]))
            {
                var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
                Main.FieldDao.Delete(fieldId);
                LtlMessage.Text = Utils.GetMessageHtml("字段删除成功！", true);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["taxis"]))
            {
                var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
                var direction = Request.QueryString["direction"];

                switch (direction.ToUpper())
                {
                    case "UP":
                        Main.FieldDao.TaxisUp(fieldId);
                        break;
                    case "DOWN":
                        Main.FieldDao.TaxisDown(fieldId);
                        break;
                }
                LtlMessage.Text = Utils.GetMessageHtml("排序成功！", true);
            }

            var fieldList = Main.FieldDao.GetFieldInfoList(_siteId, _channelId, _contentId, false);

            DgContents.DataSource = fieldList;
            DgContents.ItemDataBound += DgContents_ItemDataBound;
            DgContents.DataBind();

            //BtnReturn.Attributes.Add("onclick",
            //    $"location.href='{PageManagement.GetRedirectUrl(_siteId)}';return false;");
            BtnAddField.Attributes.Add("onclick",
                $"location.href = '{PageUrl}&addField={true}';return false;");

            if (!string.IsNullOrEmpty(Request.QueryString["addField"]))
            {
                PhModalAdd.Visible = true;
                var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
                var fieldInfo = fieldId > 0 ? Main.FieldDao.GetFieldInfo(fieldId, true) : new FieldInfo();

                LtlModalAddTitle.Text = fieldId > 0 ? "编辑字段" : "新增字段";
                LtlScript.Text = @"<script>
setTimeout(function() {
    $('#modalAdd').modal();
}, 100);
</script>";

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
            else if (!string.IsNullOrEmpty(Request.QueryString["validateField"]))
            {
                PhModalValidate.Visible = true;
                var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
                var fieldInfo = Main.FieldDao.GetFieldInfo(fieldId, false);

                LtlScript.Text = @"<script>
setTimeout(function() {
    $('#modalValidate').modal();
}, 100);
</script>";

                var settings = new FieldSettings(fieldInfo.FieldSettings);
                Utils.SelectListItems(DdlIsValidate, settings.IsValidate.ToString());
                Utils.SelectListItems(DdlIsRequired, settings.IsRequired.ToString());

                if (Utils.EqualsIgnoreCase(fieldInfo.FieldType, nameof(FieldType.Text)) || Utils.EqualsIgnoreCase(fieldInfo.FieldType, nameof(FieldType.TextArea)))
                {
                    PhNum.Visible = true;
                }
                else
                {
                    PhNum.Visible = false;
                }

                TbMinNum.Text = settings.MinNum.ToString();
                TbMaxNum.Text = settings.MaxNum.ToString();

                ValidateTypeUtils.AddListItems(DdlValidateType);
                Utils.SelectListItems(DdlValidateType, settings.ValidateType.Value);

                TbErrorMessage.Text = settings.ErrorMessage;

                Validate_SelectedIndexChanged(null, EventArgs.Empty);
            }

            //var redirectUrl = GetRedirectUrl(PublishmentSystemId, _tableStyle, _tableName, _relatedIdentity, _itemId);

            //btnAddStyle.Attributes.Add("onclick", ModalTableStyleAdd.GetOpenWindowString(PublishmentSystemId, 0, _relatedIdentities, _tableName, string.Empty, _tableStyle, redirectUrl));
            //btnAddStyles.Attributes.Add("onclick", ModalTableStylesAdd.GetOpenWindowString(PublishmentSystemId, _relatedIdentities, _tableName, _tableStyle, redirectUrl));

            //btnImport.Attributes.Add("onclick", ModalTableStyleImport.GetOpenWindowString(_tableName, _tableStyle, PublishmentSystemId, _relatedIdentity));
            //btnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToSingleTableStyle(_tableStyle, _tableName, PublishmentSystemId, _relatedIdentity));
        }

        private static void RptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var cbIsSelected = (CheckBox) e.Item.FindControl("CbIsSelected");
            cbIsSelected.Checked = Utils.EvalBool(e.Item.DataItem, "IsSelected");
        }

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var fieldInfo = (FieldInfo) e.Item.DataItem;
            var settings = new FieldSettings(fieldInfo.FieldSettings);

            var ltlAttributeName = (Literal) e.Item.FindControl("ltlAttributeName");
            var ltlDisplayName = (Literal) e.Item.FindControl("ltlDisplayName");
            var ltlFieldType = (Literal) e.Item.FindControl("ltlFieldType");
            var ltlIsDisabled = (Literal) e.Item.FindControl("ltlIsDisabled");
            var ltlValidate = (Literal) e.Item.FindControl("ltlValidate");
            var ltlActions = (Literal) e.Item.FindControl("ltlActions");

            ltlAttributeName.Text = fieldInfo.AttributeName;

            ltlDisplayName.Text = fieldInfo.DisplayName;
            ltlFieldType.Text = FieldTypeUtils.GetText(FieldTypeUtils.GetEnumType(fieldInfo.FieldType));

            ltlIsDisabled.Text = fieldInfo.IsDisabled
                ? @"<span class=""label label-danger"">已禁用</span>"
                : @"<span class=""label label-primary"">已启用</span>";
            ltlValidate.Text = ValidateTypeUtils.GetValidateInfo(settings.IsRequired,
                settings.MinNum, settings.MaxNum, settings.ValidateType);

            ltlActions.Text = $@"
<a class=""m-r-10"" href=""{PageUrl}&taxis={true}&direction=Up&fieldId={fieldInfo
                .Id}"">上升</a>
<a class=""m-r-10"" href=""{PageUrl}&taxis={true}&direction=Down&fieldId={fieldInfo
                .Id}"">下降</a>
<a class=""m-r-10"" href=""{PageUrl}&addField={true}&fieldId={fieldInfo.Id}"">编辑</a>
<a class=""m-r-10"" href=""{PageUrl}&validateField={true}&fieldId={fieldInfo.Id}"">验证规则</a>
<a class=""m-r-10"" href=""{PageUrl}&delete={true}&fieldId={fieldInfo.Id}"">删除</a>";
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
                        LtlModalAddMessage.Text = Utils.GetMessageHtml("操作失败，选项数目必须大于0！", false);
                        return;
                    }
                }
            }

            var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);

            var isChanged = fieldId > 0 ? UpdateFieldInfo(fieldId, fieldType) : InsertFieldInfo(fieldType);

            if (isChanged)
            {
                Response.Redirect(PageUrl);
            }
        }

        private bool UpdateFieldInfo(int fieldId, string fieldType)
        {
            var isChanged = false;

            var fieldInfo = Main.FieldDao.GetFieldInfo(fieldId, true);

            if (fieldInfo.AttributeName != TbAttributeName.Text &&
                Main.FieldDao.IsExists(_siteId, _channelId, _contentId, TbAttributeName.Text))
            {
                LtlModalAddMessage.Text = Utils.GetMessageHtml($@"字段修改失败：字段名""{TbAttributeName.Text}""已存在", false);
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
                            LtlModalAddMessage.Text = Utils.GetMessageHtml("操作失败，只能有一个初始化时选定项！", false);
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
                LtlModalAddMessage.Text = Utils.GetMessageHtml($"字段修改失败：{ex.Message}！", false);
            }
            return isChanged;
        }

        private bool InsertFieldInfo(string fieldType)
        {
            var isChanged = false;

            if (Main.FieldDao.IsExists(_siteId, _channelId, _contentId, TbAttributeName.Text))
            {
                LtlModalAddMessage.Text = Utils.GetMessageHtml($@"字段添加失败：字段名""{TbAttributeName.Text}""已存在", false);
                return false;
            }

            var fieldInfo = new FieldInfo
            {
                Id = 0,
                PublishmentSystemId = _siteId,
                ChannelId = _channelId,
                ContentId = _contentId,
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
                            LtlModalAddMessage.Text = Utils.GetMessageHtml("操作失败，只能有一个初始化时选定项！", false);
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
                LtlModalAddMessage.Text = Utils.GetMessageHtml($"字段添加失败：{ex.Message}！", false);
            }
            return isChanged;
        }

        public void Validate_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhValidate.Visible = Convert.ToBoolean(DdlIsValidate.SelectedValue);
        }

        public void BtnValidate_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            var fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);
            var fieldInfo = Main.FieldDao.GetFieldInfo(fieldId, false);
            var settings = new FieldSettings(fieldInfo.FieldSettings)
            {
                IsValidate = Convert.ToBoolean(DdlIsValidate.SelectedValue),
                IsRequired = Convert.ToBoolean(DdlIsRequired.SelectedValue),
                MinNum = Convert.ToInt32(TbMinNum.Text),
                MaxNum = Convert.ToInt32(TbMaxNum.Text),
                ValidateType = ValidateTypeUtils.GetEnumType(DdlValidateType.SelectedValue),
                ErrorMessage = TbErrorMessage.Text
            };

            fieldInfo.FieldSettings = settings.ToString();

            try
            {
                Main.FieldDao.Update(fieldInfo);
                isChanged = true;
            }
            catch (Exception ex)
            {
                LtlModalValidateMessage.Text = Utils.GetMessageHtml($"设置表单验证失败：{ex.Message}", false);
            }

            if (isChanged)
            {
                Response.Redirect(PageUrl);
            }
        }
    }
}
