using Analyzer_Service.Models.Schema;
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
        private readonly ICacheMongo _cacheMongo;


        public TelemetryDataArchiveController(FlightTelemetryMongoProxy telemetryMongoProxy, IContrillerUtilscs contrillerUtilscs, ICacheMongo cacheMongo)
        {
            _telemetryMongoProxy = telemetryMongoProxy;
            _contrillerUtilscs = contrillerUtilscs;
            _cacheMongo = cacheMongo;
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
            TelemetryFlightData? result = await _telemetryMongoProxy.GetFromFlightDataAsync(masterIndex);

            if (result == null)
                return NotFound($"No TelemetryFlightData found for Master Index {masterIndex}");

            return Ok(result);

        }
        [HttpGet("all-flight")]
        public async Task<ActionResult<List<existingFlight>>> GetAllFlights()
        {
            List<existingFlight> result = await _contrillerUtilscs.GetExistingFlights();
            return Ok(result);
        }

        [HttpDelete("delete-flight/{masterIndex}")]
        public async Task<IActionResult> DeleteFlightByMasterIndex(int masterIndex)
        {
            await _telemetryMongoProxy.DeleteAllDataByMasterIndexAsync(masterIndex);
            return Ok(new { message = "Flight data has been deleted." });
        }

        [HttpGet("get-flight-points/{masterIndex}/{parameter}")]
        public async Task<IActionResult> GetFlightPointsByMasterIndex(int masterIndex,string parameter)
        {
            List<long> result = await _cacheMongo.GetParamterFlightDataAsync(masterIndex,parameter);
            if (result == null)
                return NotFound($"No Flight Points found for Master Index {masterIndex}");
            return Ok(result);
        }

        [HttpGet("get-flight-connections/{masterIndex}/{parameter}")]
        public async Task<IActionResult> GetFlightConnectionsByMasterIndex(int masterIndex, string parameter)
        {
            List<string> result = await _cacheMongo.GetConnectionsFlightDataAsync(masterIndex, parameter);
            if (result == null)
                return NotFound($"No Flight Connections found for Master Index {masterIndex}");
            return Ok(result);
        }

        [HttpGet("get-flight-historical-similarity/{masterIndex}/{parameter}")]
        public async Task<IActionResult> GetFlightHistoricalSimilarityByMasterIndex(int masterIndex, string parameter)
        {
            List<HistoricalSimilarityPoint> result = await _cacheMongo.GetHistoricalSimilarityFlightDataAsync(masterIndex, parameter);
            if (result == null)
                return NotFound($"No Flight Historical Similarity found for Master Index {masterIndex}");
            return Ok(result);
        }
    }
}
