using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LostAndFoundApi.Models
{
    public class Person
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Kontakt { get; set; }

        public ICollection<Item> Items { get; set; }
    }
}
