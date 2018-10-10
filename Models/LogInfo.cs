using System;

namespace SS.Poll.Models
{
    public class LogInfo : AttributesImpl
    {
        public int Id { get; set; }

        public int SiteId { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public string ItemIds { get; set; }

        public string UniqueId { get; set; }

        public DateTime AddDate { get; set; }

        public string AttributeValues { get; set; }
    }
}
