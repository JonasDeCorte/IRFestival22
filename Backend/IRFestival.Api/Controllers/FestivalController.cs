﻿using IRFestival.Api.Data;
using IRFestival.Api.Domain;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FestivalController : ControllerBase
    {
        private readonly FestivalDbContext _ctx;
        private readonly TelemetryClient telemetryClient;

        public FestivalController(FestivalDbContext ctx, TelemetryClient telemetryClient)
        {
            _ctx = ctx;
            this.telemetryClient = telemetryClient;
        }

        [HttpGet("LineUp")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Schedule))]
        public async Task<ActionResult> GetLineUp()
        {
            var lineUp = await _ctx.Schedules.Include(x => x.Festival)
                                                        .Include(x => x.Items)
                                                        .ThenInclude(x => x.Artist)
                                                        .Include(x => x.Items)
                                                        .ThenInclude(x => x.Stage)
                                                        .FirstOrDefaultAsync();

            return Ok(lineUp);
        }

        [HttpGet("Artists")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Artist>))]
        public async Task<ActionResult> GetArtists(bool? rating)
        {
            List<Artist> artists = await _ctx.Artists.ToListAsync();
            if (rating.HasValue && rating.Value)
            {
                telemetryClient.TrackEvent($"LIST OF ARTISTS WITH RATINGS");
            }
            else
            {
                telemetryClient.TrackEvent($"LIST OF ARTISTS without RATINGS");
            }
            return base.Ok(artists);
        }

        [HttpGet("Stages")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Stage>))]
        public async Task<ActionResult> GetStages()
        {
            List<Stage> stages = await _ctx.Stages.ToListAsync();
            return Ok(stages);
        }

        [HttpPost("Favorite")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ScheduleItem))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> SetAsFavorite(int id)
        {
            var schedule = await _ctx.ScheduleItems.FirstOrDefaultAsync(si => si.Id == id);
            if (schedule != null)
            {
                schedule.IsFavorite = !schedule.IsFavorite;
                return Ok(schedule);
            }
            return NotFound();
        }
    }
}