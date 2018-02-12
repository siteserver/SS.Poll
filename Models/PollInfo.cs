using System;

namespace SS.Poll.Models
{
    public class PollInfo
    {
        public int Id { get; set; }
        public int PublishmentSystemId { get; set; }
        public int ChannelId { get; set; }
        public int ContentId { get; set; }
        public bool IsImage { get; set; }
        public bool IsUrl { get; set; }
        public bool IsResult { get; set; }
        public bool IsTimeout { get; set; }
        public DateTime TimeToStart { get; set; }
        public DateTime TimeToEnd { get; set; }
        public bool IsCheckbox { get; set; }
        public int CheckboxMin { get; set; }
        public int CheckboxMax { get; set; }
        public bool IsProfile { get; set; }
    }
}
