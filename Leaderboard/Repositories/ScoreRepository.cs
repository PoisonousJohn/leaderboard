using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;

namespace leaderboard.Repositories
{
    public class Score
    {
        public string userId;
        public int score;
    }

    public class ScoreRepository
    {

		public ScoreRepository(IRedisClientsManager redisManager)
        {
            _redis = redisManager.GetClient();
        }

        ~ScoreRepository()
        {
            _redis.Dispose();
        }

        public List<Score> GetLeaderboard(string leaderboardName, int page)
        {
            var start = page * ENTRIES_PER_PAGE;
            var offset = page * ENTRIES_PER_PAGE;
            var result = _redis.GetRangeWithScoresFromSortedSetDesc(leaderboardName, offset, offset + ENTRIES_PER_PAGE);
            return result.Select(i => new Score { userId = i.Key, score = (int)i.Value}).ToList();
        }

        public void SetScore(string leaderboardName, Score score)
        {
            if (score == null)
            {
                throw new ArgumentNullException();
            }

            if (string.IsNullOrEmpty(score.userId) || score.score < 0)
            {
                throw new ArgumentException($"{nameof(score)} is not valid");
            }

            var currentScore = _redis.GetItemScoreInSortedSet(leaderboardName, score.userId);
            if (currentScore >= score.score)
            {
                return;
            }
            _redis.RemoveItemFromSortedSet(leaderboardName, score.userId);
            _redis.AddItemToSortedSet(leaderboardName, score.userId, score.score);
        }

        private const int ENTRIES_PER_PAGE = 20;

        private readonly IRedisClient _redis;
    }
}