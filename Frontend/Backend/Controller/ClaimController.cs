using LostAndFound.WPF.Model;
using Microsoft.AspNetCore.Mvc;
using Server.EF_Core;
using System.Collections.Generic;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimController : ControllerBase
    {
        private static Context context = new Context();

        [HttpGet]
        public IEnumerable<Claim> Get()
        {
            return context.Claims;
        }

        [HttpGet("{id}")]
        public Claim Get(int id)
        {
            return context.Claims.Find(id);
        }

        [HttpPost]
        public void Post([FromBody] Claim value)
        {
            context.Claims.Add(value);
            context.SaveChanges();
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Claim value)
        {
            var edited = context.Claims.Find(id);
            context.Entry(edited).CurrentValues.SetValues(value);
            context.SaveChanges();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            context.Claims.Remove(context.Claims.Find(id));
            context.SaveChanges();
        }
    }
}
