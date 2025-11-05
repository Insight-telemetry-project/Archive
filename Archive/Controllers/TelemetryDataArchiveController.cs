using Archive.Services.Mongo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Archive.Models.Schema;

namespace Archive.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelemetryDataArchiveController : ControllerBase
    {
        private readonly FlightTelemetryMongoProxy _telemetryMongoProxy;

        public TelemetryDataArchiveController(FlightTelemetryMongoProxy telemetryMongoProxy)
        {
            _telemetryMongoProxy = telemetryMongoProxy;
        }

        [HttpGet("fields/{masterIndex}")]
        public async Task<IActionResult> GetFieldsByMasterIndex(int masterIndex)
        {
            List<TelemetrySensorFields>? result = await _telemetryMongoProxy.GetFromFieldsAsync(masterIndex);

            if (result == null)
                return NotFound($"No TelemetryFields found for Master Index {masterIndex}");

            return Ok(result);
        }

        [HttpGet("flight/{masterIndex}")]
        public async Task<IActionResult> GetFlightByMasterIndex(int masterIndex)
        {
            List<TelemetryFlightData>? result = await _telemetryMongoProxy.GetFromFlightDataAsync(masterIndex);

            if (result == null)
                return NotFound($"No TelemetryFlightData found for Master Index {masterIndex}");

            return Ok(result);

        }
    }
}
