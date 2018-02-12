using System;

namespace SS.Poll.Models
{
    public class LogInfo : ExtendedAttributes
    {
        public int Id { get; set; }

        public int PollId { get; set; }

        public string ItemIds { get; set; }

        public string UniqueId { get; set; }

        public DateTime AddDate { get; set; }

        public string AttributeValues { get; set; }
    }
}
