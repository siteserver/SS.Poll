using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SS.Poll.Model;

namespace SS.Poll.Core
{
    public class FieldTypeParser
    {
        private FieldTypeParser()
        {
        }

        public static string Parse(FieldInfo fieldInfo, FieldSettings settings)
        {
            string retval;

            var fieldType = FieldTypeUtils.GetEnumType(fieldInfo.FieldType);

            switch (fieldType)
            {
                case FieldType.Text:
                    retval = ParseText(fieldInfo, settings);
                    break;
                case FieldType.TextArea:
                    retval = ParseTextArea(fieldInfo, settings);
                    break;
                case FieldType.CheckBox:
                    retval = ParseCheckBox(fieldInfo, settings);
                    break;
                case FieldType.Radio:
                    retval = ParseRadio(fieldInfo, settings);
                    break;
                case FieldType.SelectOne:
                    retval = ParseSelectOne(fieldInfo, settings);
                    break;
                case FieldType.SelectMultiple:
                    retval = ParseSelectMultiple(fieldInfo, settings);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string ParseText(FieldInfo fieldInfo, FieldSettings settings)
        {
            var vValidate = string.Empty;
            if (settings.IsValidate)
            {
                var validateList = new List<string>();
                if (settings.IsRequired)
                {
                    validateList.Add("required");
                }
                
                if (settings.ValidateType == ValidateType.Integer)
                {
                    validateList.Add("digits");
                }
                else if (settings.ValidateType == ValidateType.Email)
                {
                    validateList.Add("email");
                }

                if (validateList.Count > 0)
                {
                    vValidate = $@"v-validate=""'{string.Join("|", validateList)}'""";
                }
            }

            return
                $@"<input v-model=""attributes.{fieldInfo.AttributeName}"" name=""{fieldInfo.AttributeName}"" {vValidate} :class=""{{'error': errors.has('{fieldInfo.AttributeName}') }}"" type=""text"" />";

            //return $@"<input id=""{fieldInfo.AttributeName}"" name=""{fieldInfo.AttributeName}"" type=""text"" class=""form-control"" value=""{fieldInfo.AttributeValue}"" {validateAttributes} />";
        }

        public static string ParseTextArea(FieldInfo fieldInfo, FieldSettings settings)
        {
            var validateAttributes = ValidateTypeUtils.GetValidateAttributes(settings.IsValidate, fieldInfo.DisplayName, settings.IsRequired, settings.MinNum, settings.MaxNum, settings.ValidateType, settings.ErrorMessage);

            return $@"<textarea id=""{fieldInfo.AttributeName}"" name=""{fieldInfo.AttributeName}"" class=""form-control"" {validateAttributes}>{HttpUtility.HtmlDecode(fieldInfo.AttributeValue)}</textarea>";
        }

        private static string ParseCheckBox(FieldInfo fieldInfo, FieldSettings settings)
        {
            var builder = new StringBuilder();

            var items = fieldInfo.Items ?? new List<FieldItemInfo>();

            var checkBoxList = new CheckBoxList
            {
                RepeatLayout = RepeatLayout.UnorderedList,
                ID = fieldInfo.AttributeName
            };
            checkBoxList.Attributes.Add("style", "list-style: none;");
            var selectedValues = !string.IsNullOrEmpty(fieldInfo.AttributeValue) ? fieldInfo.AttributeValue : string.Empty;
            var selectedValueArray = selectedValues.Split(',');

            //验证属性
            ValidateTypeUtils.GetValidateAttributesForListItem(checkBoxList, settings.IsValidate, fieldInfo.DisplayName, settings.IsRequired, settings.MinNum, settings.MaxNum, settings.ValidateType, settings.ErrorMessage);

            foreach (var item in items)
            {
                var isSelected = selectedValueArray.Contains(item.Value);
                var listItem = new ListItem(item.Value, item.Value)
                {
                    Selected = isSelected
                };

                checkBoxList.Items.Add(listItem);
            }
            checkBoxList.Attributes.Add("isListItem", "true");
            builder.Append(Utils.GetControlRenderHtml(checkBoxList));

            var i = 0;
            foreach (var item in items)
            {
                builder.Replace($@"name=""{fieldInfo.AttributeName}${i}""",
                    $@"name=""{fieldInfo.AttributeName}"" value=""{item.Value}""");
                i++;
            }

            return builder.ToString();
        }

        private static string ParseRadio(FieldInfo fieldInfo, FieldSettings settings)
        {
            var builder = new StringBuilder();

            var items = fieldInfo.Items ?? new List<FieldItemInfo>();

            var radioButtonList = new RadioButtonList
            {
                RepeatLayout = RepeatLayout.UnorderedList,
                ID = fieldInfo.AttributeName,
            };
            radioButtonList.Attributes.Add("style", "list-style: none;");
            var selectedValue = !string.IsNullOrEmpty(fieldInfo.AttributeValue) ? fieldInfo.AttributeValue : string.Empty;

            //验证属性
            ValidateTypeUtils.GetValidateAttributesForListItem(radioButtonList, settings.IsValidate, fieldInfo.DisplayName, settings.IsRequired, settings.MinNum, settings.MaxNum, settings.ValidateType, settings.ErrorMessage);

            foreach (var item in items)
            {
                bool isSelected = item.Value == selectedValue;
                var listItem = new ListItem(item.Value, item.Value)
                {
                    Selected = isSelected
                };
                radioButtonList.Items.Add(listItem);
            }
            radioButtonList.Attributes.Add("isListItem", "true");
            builder.Append(Utils.GetControlRenderHtml(radioButtonList));

            return builder.ToString();
        }

        private static string ParseSelectOne(FieldInfo fieldInfo, FieldSettings settings)
        {
            var builder = new StringBuilder();

            var items = fieldInfo.Items ?? new List<FieldItemInfo>();

            var selectedValue = !string.IsNullOrEmpty(fieldInfo.AttributeValue) ? fieldInfo.AttributeValue : string.Empty;
            //验证属性
            var validateAttributes = ValidateTypeUtils.GetValidateAttributes(settings.IsValidate, fieldInfo.DisplayName, settings.IsRequired, settings.MinNum, settings.MaxNum, settings.ValidateType, settings.ErrorMessage);
            builder.Append(string.Format(@"<select id=""{0}"" name=""{0}"" class=""form-control""  isListItem=""true"" {1}>", fieldInfo.AttributeName, validateAttributes));
            foreach (var item in items)
            {
                var isSelected = item.Value == selectedValue ? "selected" : string.Empty;

                builder.Append($@"<option value=""{item.Value}"" {isSelected}>{item.Value}</option>");
            }
            builder.Append("</select>");

            return builder.ToString();
        }

        private static string ParseSelectMultiple(FieldInfo fieldInfo, FieldSettings settings)
        {
            var builder = new StringBuilder();

            var items = fieldInfo.Items ?? new List<FieldItemInfo>();

            var selectedValues = !string.IsNullOrEmpty(fieldInfo.AttributeValue) ? fieldInfo.AttributeValue : string.Empty;
            var selectedValueList = Utils.StringCollectionToStringList(selectedValues);
            //验证属性
            var validateAttributes = ValidateTypeUtils.GetValidateAttributes(settings.IsValidate, fieldInfo.DisplayName, settings.IsRequired, settings.MinNum, settings.MaxNum, settings.ValidateType, settings.ErrorMessage);
            builder.Append(string.Format(@"<select id=""{0}"" name=""{0}"" class=""form-control""  isListItem=""true"" multiple  {1}>", fieldInfo.AttributeName, validateAttributes));
            foreach (var item in items)
            {
                string isSelected = selectedValueList.Contains(item.Value) ? "selected" : string.Empty;

                builder.Append($@"<option value=""{item.Value}"" {isSelected}>{item.Value}</option>");
            }
            builder.Append("</select>");

            return builder.ToString();
        }
    }
}
