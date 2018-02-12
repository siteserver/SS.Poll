using System;
using System.Web.UI.WebControls;
using SS.Poll.Core;
using SS.Poll.Models;

namespace SS.Poll.Pages
{
    public class ModalFieldValidate : PageBase
    {
        public Literal LtlMessage;
        public DropDownList DdlIsValidate;
        public PlaceHolder PhValidate;
        public DropDownList DdlIsRequired;
        public PlaceHolder PhNum;
        public TextBox TbMinNum;
        public TextBox TbMaxNum;
        public DropDownList DdlValidateType;
        public TextBox TbErrorMessage;

        private int _fieldId;

        public static string GetOpenWindowString(int siteId, int channelId, int contentId, int fieldId)
        {
            return LayerUtils.GetOpenScript("设置验证规则", Main.Instance.PluginApi.GetPluginUrl($"{nameof(ModalFieldValidate)}.aspx?siteId={siteId}&channelId={channelId}&contentId={contentId}&fieldId={fieldId}"));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _fieldId = Convert.ToInt32(Request.QueryString["fieldId"]);

            if (IsPostBack) return;

            var fieldInfo = Main.FieldDao.GetFieldInfo(_fieldId, false);

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
                LtlMessage.Text = Utils.GetMessageHtml($"设置表单验证失败：{ex.Message}", false);
            }

            if (isChanged)
            {
                LayerUtils.Close(Page);
            }
        }
    }
}
