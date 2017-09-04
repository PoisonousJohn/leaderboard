using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ServiceStack.Redis;

namespace leaderboard.Controllers
{
    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {

        public LeaderboardController(IRedisClientsManager redis)
        {
            _redis = redis.GetClient();
        }

        // GET api/values
        [HttpGet("{leaderboardName}")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        private readonly IRedisClient _redis;
    }
}
