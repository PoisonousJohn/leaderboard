using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace leaderboard.Controllers
{
    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {
        // GET api/values
        [HttpGet("{leaderboardName}")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

    }
}
