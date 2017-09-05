using System;
using System.IO;
using leaderboard.Repositories;
using Microsoft.Extensions.Configuration;
using ServiceStack.Redis;
using Xunit;

namespace Leaderboard.Tests
{
    public class ScoreSetTest
    {
        public ScoreSetTest()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.Test.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            var conf = builder.Build();
            var redisManager = new BasicRedisClientManager();
            _client = redisManager.GetClient();
            _client.FlushDb();
            _repo = new ScoreRepository(redisManager);
        }

        [Fact]
        public void SortingIsByHighestScore()
        {
            _client.FlushDb();
            const string testUserId = "testUserId";
            const string testUserId2 = "testUserId2";
            const int testScore = 100;
            const int testScore2 = 200;
            var score = new Score { userId = testUserId, score = testScore };
            var score2 = new Score { userId = testUserId2, score = testScore2 };
            _repo.SetScore(leaderboard, score);
            _repo.SetScore(leaderboard, score2);
            var scores = _repo.GetLeaderboard(leaderboard, 0);
            Assert.Equal(2, scores.Count);
            Assert.Equal(testScore2, scores[0].score);
            Assert.Equal(testUserId2, scores[0].userId);
            Assert.Equal(testScore, scores[1].score);
            Assert.Equal(testUserId, scores[1].userId);
        }

        [Fact]
        public void SetValidScore()
        {
            _client.FlushDb();
            const string testUserId = "testUserId";
            const int testScore = 100;
            const int testScore2 = 200;
            var score = new Score { userId = testUserId, score = testScore };
            _repo.SetScore(leaderboard, score);
            var scores = _repo.GetLeaderboard(leaderboard, 0);
            Assert.Equal(1, scores.Count);
            Assert.Equal(testUserId, scores[0].userId);
            Assert.Equal(testScore, scores[0].score);

            score.score = testScore2;

            _repo.SetScore(leaderboard, score);
            scores = _repo.GetLeaderboard(leaderboard, 0);
            Assert.Equal(1, scores.Count);
            Assert.Equal(testUserId, scores[0].userId);
            Assert.Equal(testScore2, scores[0].score);

            score.score = testScore;
            _repo.SetScore(leaderboard, score);
            scores = _repo.GetLeaderboard(leaderboard, 0);
            Assert.Equal(1, scores.Count);
            Assert.Equal(testUserId, scores[0].userId);
            Assert.Equal(testScore2, scores[0].score);
        }

        private readonly ScoreRepository _repo;
        private const string leaderboard = "leaderboardName";
        private IRedisClient _client;
    }
}
