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
        [HttpGet]
        public Stats Get()
        {
            List<Status> statuses = [.. context.Statuses];

            List<StatsTimeFrame> timeFrames = GetStatsTimeFrame(statuses);
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
            
            return new() {
                StatsTimeFrames = timeFrames,
                RandomGame = randomGame
            };
        }

        private List<StatsTimeFrame> GetStatsTimeFrame(List<Status> statuses)
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

            List<Game> games = [.. context.Games.Where(g => g.StatusId == status.Id)];

            switch (key)
            {
                case TimeFrameKeys.Month:
                    games = [.. games.Where(StatsMonthCondition)];
                    break;
                case TimeFrameKeys.Year:
                    games = [.. games.Where(StatsYearCondition)];
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

        private bool StatsYearCondition(Game game)
        {
            bool result = game.AddedDateTime != null;
            if (result)
            {
                result &= game.AddedDateTime!.Value >= DateTime.Now.AddYears(-1);
            }
            return result;
        }

        private bool StatsMonthCondition(Game game)
        {
            bool result = game.AddedDateTime != null;
            if (result)
            {
                result &= game.AddedDateTime!.Value >= DateTime.Now.AddMonths(-1);
            }
            return result;
        }
    }
}
