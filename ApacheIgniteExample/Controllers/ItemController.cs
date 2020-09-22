using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApacheIgniteExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ICache<int, string> _cache;

        private readonly ILogger<ItemController> _logger;

        public ItemController(ILogger<ItemController> logger, ICache<int, string> cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpPost]
        public IActionResult Post()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (_cache.TryGet(id, out string item))
                return Ok(item);

            return Ok("Não encontrei miseravi");
        }

        [HttpGet("Add/{id}/{name}")]
        public IActionResult Add(int id, string name)
        {
            _cache.Put(id, name);
            return Ok($"Id: {id} \nName: {name}");
        }

    }
}
