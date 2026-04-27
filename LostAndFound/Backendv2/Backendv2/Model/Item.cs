namespace LostAndFound.WPF.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime FoundDate { get; set; } = DateTime.Now;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = "Unclaimed";
        public List<Claim> Claims { get; set; } = new();
    }
}
