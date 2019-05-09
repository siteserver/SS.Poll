using System;
using Datory;

namespace SS.Poll.Core.Models
{
    [Table("ss_poll_item")]
    public class ItemInfo : Entity, ICloneable
    {
        [TableColumn]
        public int PollId { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn]
        public string Title { get; set; }

        [TableColumn]
        public string SubTitle { get; set; }

        [TableColumn]
        public string ImageUrl { get; set; }

        [TableColumn]
        public string LinkUrl { get; set; }

        [TableColumn]
        public int Count { get; set; }

        public object Clone()
        {
            return (ItemInfo)MemberwiseClone();
        }
    }
}
