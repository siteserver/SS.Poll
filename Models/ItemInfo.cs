namespace SS.Poll.Models
{
    public class ItemInfo
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string ImageUrl { get; set; }
        public string LinkUrl { get; set; }
        public int Count { get; set; }
    }
}
