using System;
using Datory;

namespace SS.Poll.Core.Models
{
    [Table("ss_poll_log")]
    public class LogInfo : Entity
    {
        [TableColumn]
        public int PollId { get; set; }

        [TableColumn]
        public string ItemIds { get; set; }

        [TableColumn]
        public DateTime? AddDate { get; set; }

        [TableColumn(Text = true, Extend = true)]
        public string AttributeValues { get; set; }
    }
}
