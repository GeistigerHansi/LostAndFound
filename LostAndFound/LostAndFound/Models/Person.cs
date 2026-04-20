using System.Collections.Generic;

namespace LostAndFound.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Kontakt { get; set; }

        public List<Item> Items { get; set; }
    }
}
