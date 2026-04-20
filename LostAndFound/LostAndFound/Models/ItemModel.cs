using System;

namespace LostAndFound.Models
{
    public class ItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Beschreibung { get; set; }
        public string Fundort { get; set; }
        public DateTime Datum { get; set; }
        public string Status { get; set; }
        public int PersonId { get; set; }
    }
}
