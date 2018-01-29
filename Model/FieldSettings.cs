using SiteServer.Plugin;
using SS.Poll.Core;

namespace SS.Poll.Model
{
    public class FieldSettings : ExtendedAttributes
    {
        public FieldSettings(string extendValues): base(extendValues) { }

        public bool IsValidate
        {
            get { return GetBool("IsValidate"); }
            set { Set("IsValidate", value.ToString()); }
        }

        public bool IsRequired
        {
            get { return GetBool("IsRequired"); }
            set { Set("IsRequired", value.ToString()); }
        }

        public int MinNum
        {
            get { return GetInt("MinNum"); }
            set { Set("MinNum", value.ToString()); }
        }

        public int MaxNum
        {
            get { return GetInt("MaxNum"); }
            set { Set("MaxNum", value.ToString()); }
        }

        public ValidateType ValidateType
        {
            get { return ValidateTypeUtils.GetEnumType(GetString("ValidateType")); }
            set { Set("ValidateType", value.Value); }
        }

        public string ErrorMessage
        {
            get { return GetString("ErrorMessage"); }
            set { Set("ErrorMessage", value); }
        }
    }
}
