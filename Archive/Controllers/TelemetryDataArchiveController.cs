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
        private readonly FlightTelemetryMongoProxy _telemetryService;

        public TelemetryDataArchiveController(FlightTelemetryMongoProxy telemetryService)
        {
            _telemetryService = telemetryService;
        }

        [HttpGet("fields/{masterIndex}")]
        public async Task<IActionResult> GetFieldsByMasterIndex(int masterIndex)
        {
            try
            {
                List<TelemetrySensorFields> result = await _telemetryService.GetFromFieldsAsync(masterIndex);
                return Ok(result);
            }
            catch (KeyNotFoundException exceptions)
            {
                return NotFound(exceptions.Message);
            }
        }

        [HttpGet("flight/{masterIndex}")]
        public async Task<IActionResult> GetFlightByMasterIndex(int masterIndex)
        {
            try
            {
                List<TelemetryFlightData> result = await _telemetryService.GetFromFlightDataAsync(masterIndex);
                return Ok(result);
            }
            catch (KeyNotFoundException exceptions)
            {
                return NotFound(exceptions.Message);
            }
        }
    }
}
