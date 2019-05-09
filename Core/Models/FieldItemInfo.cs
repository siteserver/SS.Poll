using Datory;

namespace SS.Poll.Core.Models
{
    [Table("ss_poll_field_item")]
    public class FieldItemInfo : Entity
    {
        [TableColumn]
        public int PollId { get; set; }

        [TableColumn]
        public int FieldId { get; set; }

        [TableColumn]
        public string Value { get; set; }

        [TableColumn]
        public bool IsSelected { get; set; }

        [TableColumn]
        public bool IsExtras { get; set; }
    }
}

