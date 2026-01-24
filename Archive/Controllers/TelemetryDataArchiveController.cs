using Archive.Models.Dto;
using Archive.Models.Interface;
using Archive.Models.Schema;
using Archive.Services.Mongo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archive.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelemetryDataArchiveController : ControllerBase
    {
        private readonly FlightTelemetryMongoProxy _telemetryMongoProxy;
        private readonly IContrillerUtilscs _contrillerUtilscs;


        public TelemetryDataArchiveController(FlightTelemetryMongoProxy telemetryMongoProxy, IContrillerUtilscs contrillerUtilscs)
        {
            _telemetryMongoProxy = telemetryMongoProxy;
            _contrillerUtilscs = contrillerUtilscs;
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
        [HttpGet("all-flight")]
        public async Task<ActionResult<List<existingFlight>>> GetAllFlights()
        {
            List<existingFlight> result =await _contrillerUtilscs.GetExistingFlights();
            return Ok(result);
        }

        [HttpDelete("delete-flight/{masterIndex}")]
        public async Task<IActionResult> DeleteFlightByMasterIndex(int masterIndex)
        {
            await _telemetryMongoProxy.DeleteAllDataByMasterIndexAsync(masterIndex);
            return Ok(new { message = "Flight data has been deleted." });
        }
    }
}
