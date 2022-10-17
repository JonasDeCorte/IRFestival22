using IRFestival.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using System.Net;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly AppSettingsOptions _options;
        private readonly IFeatureManagerSnapshot featureManager;

        public SettingsController(IOptions<AppSettingsOptions> options, IFeatureManagerSnapshot featureManager)
        {
            _options = options.Value;
            this.featureManager = featureManager;
        }

        [HttpGet("Features")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AppSettingsOptions))]
        public async Task<ActionResult> Get()
        {
            string message = await featureManager.IsEnabledAsync("BuyTickets")
                ? "The ticket sale has started, go go go."
                : "you cannot buy any tickets at the moment";
            return Ok(message);
        }
    }
}