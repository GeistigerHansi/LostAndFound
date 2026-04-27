using LostAndFound.WPF.Model;
using Microsoft.AspNetCore.Mvc;
using Server.EF_Core;
using System.Collections.Generic;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private static Context context = new Context();

        [HttpGet]
        public IEnumerable<Item> Get()
        {
            return context.Items;
        }

        [HttpGet("{id}")]
        public Item Get(int id)
        {
            return context.Items.Find(id);
        }

        [HttpPost]
        public void Post([FromBody] Item value)
        {
            context.Items.Add(value);
            context.SaveChanges();
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Item value)
        {
            var edited = context.Items.Find(id);
            context.Entry(edited).CurrentValues.SetValues(value);
            context.SaveChanges();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            context.Items.Remove(context.Items.Find(id));
            context.SaveChanges();
        }
    }
}
