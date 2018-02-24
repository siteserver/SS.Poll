using System.Collections.Generic;

namespace SS.Poll.Models
{
	public class FieldInfo
	{
		public int Id { get; set; }

	    public int SiteId { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public int Taxis { get; set; }

        public string AttributeName { get; set; }

        public string AttributeValue { get; set; }

        public string DisplayName { get; set; }

        public string PlaceHolder { get; set; }

        public bool IsDisabled { get; set; }

	    public string FieldType { get; set; }

	    public string FieldSettings { get; set; }

	    public List<FieldItemInfo> Items { get; set; } = new List<FieldItemInfo>();
	}
}
