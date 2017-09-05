using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leaderboard.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ServiceStack.Redis;

namespace leaderboard.Controllers
{

    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {

        public LeaderboardController(ScoreRepository score)
        {
            _score = score;
        }

        ~LeaderboardController()
        {
        }

        [HttpGet("{leaderboardName}/{page}")]
        public IEnumerable<Score> Get([FromRoute]string leaderboardName, [FromRoute]int page)
        {
            if (page < 0)
            {
                throw new ArgumentException($"{nameof(page)} can't be negative");
            }
            return _score.GetLeaderboard(leaderboardName, page);
        }

        [HttpPost("{leaderboardName}/score")]
        public IActionResult PostScore( [FromRoute]string leaderboardName,
                                        [FromBody]Repositories.Score score)
        {
            _score.SetScore(leaderboardName, score);
            return Ok();
        }

        private readonly ScoreRepository _score;
    }
}
