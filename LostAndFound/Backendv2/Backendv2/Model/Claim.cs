namespace LostAndFound.WPF.Model
{
    public class Claim
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ClaimantName { get; set; } = string.Empty;
        public string ClaimantContact { get; set; } = string.Empty;
        public DateTime ClaimDate { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;
    }
}
