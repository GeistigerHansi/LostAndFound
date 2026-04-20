using System;
using System.ComponentModel.DataAnnotations;

namespace LostAndFoundApi.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Beschreibung { get; set; }
        public string Fundort { get; set; }
        public DateTime Datum { get; set; }
        public string Status { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
