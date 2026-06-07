using Headlight.Data;
using Headlight.Models;
using Headlight.Models.API.Stats;
using Headlight.Models.JSON;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Controllers
{
    [Produces("application/json")]
    [Route("api/Stats")]
    public class StatsController(AppDbContext context) : Controller
    {
        [HttpGet("RandomGame")]
        public GameJSON GetRandomGame()
        {
            Game? pickedRandom = context.Games
                .Include(g => g.Platform)
                .Include(g => g.Status)
                .OrderBy(g => Guid.NewGuid())
                .FirstOrDefault();
            GameJSON? randomGame = null;
            if (pickedRandom != null)
            {
                randomGame = new()
                {
                    Id = pickedRandom.Id,
                    Name = pickedRandom.Name,
                    Platform = pickedRandom.Platform?.Name ?? "",
                    Status = pickedRandom.Status?.Name ?? ""
                };
            }
            Response.Headers.CacheControl = "max-age=3600";
            return randomGame ?? new();
        }

        [HttpGet]
        public Stats Get()
        {
            List<StatsTimeFrame> timeFrames = GetStatsTimeFrame(context.Statuses);
            
            return new()
            {
                StatsTimeFrames = timeFrames,
            };
        }

        private List<StatsTimeFrame> GetStatsTimeFrame(IQueryable<Status> statuses)
        {
            List<StatsTimeFrame> result = [];

            foreach(TimeFrameKeys key in Enum.GetValues(typeof(TimeFrameKeys)))
            {
                StatsTimeFrame timeFrame = new()
                {
                    TimeFrameKey = key,
                    TimeFrameName = key.ToString()
                };

                List<StatusAnalytic> statusAnalytics = [];
                
                foreach(Status status in statuses)
                {
                    statusAnalytics.Add(GetAnalyticForTimeFrame(status, key));
                }
                timeFrame.StatusAnalytics = statusAnalytics;

                result.Add(timeFrame);
            }

            return result;
        }

        private StatusAnalytic GetAnalyticForTimeFrame(Status status, TimeFrameKeys key)
        {
            StatusAnalytic analytic = new();

            IQueryable<Game> games = context.Games.Where(g => g.StatusId == status.Id);

            switch (key)
            {
                case TimeFrameKeys.Month:
                    games = StatsMonthCondition(games);
                    break;
                case TimeFrameKeys.Year:
                    games = StatsYearCondition(games);
                    break;
                default:
                    break;
            }

            analytic.Count = games.Count();
            analytic.Status = status.Name;
            analytic.StatusId = status.Id;

            TimeSpan? avgTTF = new TimeSpan();
            foreach (Game game in games)
            {
                if (game.AddedDateTime != null && game.FinishedDateTime != null)
                {
                    DateTime added = (DateTime)game.AddedDateTime;
                    avgTTF += game.FinishedDateTime?.Subtract(added);
                }
            }

            if (avgTTF > new TimeSpan())
            {
                analytic.AvgTimeToFinish = TimeSpan.FromSeconds((avgTTF.Value.TotalSeconds) / analytic.Count);
            }
            else
            {
                analytic.AvgTimeToFinish = avgTTF;
            }

            return analytic;
        }

        private static IQueryable<Game> StatsYearCondition(IQueryable<Game> query)
        {
            return query
                .Where(g => g.AddedDateTime != null)
                .Where(g => g.AddedDateTime >= DateTime.Now.AddYears(-1).ToUniversalTime());
        }

        private static IQueryable<Game> StatsMonthCondition(IQueryable<Game> query)
        {
            return query
                .Where(g => g.AddedDateTime != null)
                .Where(g => g.AddedDateTime >= DateTime.Now.AddMonths(-1).ToUniversalTime());
        }
    }
}
