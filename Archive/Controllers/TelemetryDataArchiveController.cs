using Analyzer_Service.Models.Schema;
using Archive.Models.Dto;
using Archive.Models.Enum;
using Archive.Models.Interface;
using Archive.Models.Interface.Export;
using Archive.Models.Mongo;
using Archive.Models.Schema;
using Archive.Services.Mongo;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archive.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelemetryDataArchiveController : ControllerBase
    {
        private readonly IFlightTelemetryMongoProxy _telemetryMongoProxy;
        private readonly IContrillerUtilscs _contrillerUtilscs;
        private readonly IExportService _exportService;
        public TelemetryDataArchiveController(
            IFlightTelemetryMongoProxy telemetryMongoProxy,
            IContrillerUtilscs contrillerUtilscs,
            IExportService exportService)
        {
            _telemetryMongoProxy = telemetryMongoProxy;
            _contrillerUtilscs = contrillerUtilscs;
            _exportService = exportService;
        }

        [HttpGet("fields/{masterIndex}")]
        public async Task<IActionResult> GetFieldsByMasterIndex(int masterIndex)
        {
            List<TelemetrySensorFields>? result =
                await _telemetryMongoProxy.GetFromFieldsAsync(masterIndex);

            if (result == null)
                return NotFound($"No TelemetryFields found for Master Index {masterIndex}");

            return Ok(result);
        }

        [HttpGet("flight/{masterIndex}")]
        public async Task<IActionResult> GetFlightByMasterIndex(int masterIndex)
        {
            TelemetryFlightData? result =await _telemetryMongoProxy.GetFromFlightDataAsync(masterIndex);

            if (result == null)
                return NotFound($"No TelemetryFlightData found for Master Index {masterIndex}");

            return Ok(result);
        }

        [HttpGet("all-flight")]
        public async Task<ActionResult<List<existingFlight>>> GetAllFlights()
        {
            List<existingFlight> result =
                await _contrillerUtilscs.GetExistingFlights();

            return Ok(result);
        }

        

        [HttpGet("get-flight-points/{masterIndex}/{parameter}")]
        public async Task<IActionResult> GetFlightPointsByMasterIndex(int masterIndex, string parameter)
        {
            List<AnomalyWindow> result =
                await _contrillerUtilscs.GetAnomaliesByParameter(masterIndex, parameter);

            if (result == null)
                return NotFound($"No Flight Points found for Master Index {masterIndex}");

            return Ok(result);
        }

        [HttpGet("get-flight-connections/{masterIndex}/{parameter}")]
        public async Task<IActionResult> GetFlightConnectionsByMasterIndex(int masterIndex, string parameter)
        {
            List<string> result =
                await _contrillerUtilscs.GetConnectionsByParameter(masterIndex, parameter);

            if (result == null || result.Count == 0)
                return Ok(new List<string>());

            return Ok(result);
        }

        [HttpGet("get-flight-historical-similarity/{masterIndex}/{parameter}")]
        public async Task<IActionResult> GetFlightHistoricalSimilarityByMasterIndex(int masterIndex, string parameter)
        {
            List<HistoricalSimilarityPoint> result =
                await _contrillerUtilscs.GetHistoricalSimilarityFlightDataAsync(masterIndex, parameter);

            if (result == null)
                return NotFound($"No Flight Historical Similarity found for Master Index {masterIndex}");

            return Ok(result);
        }

        [HttpGet("get-all-special-points-for-flight/{masterIndex}")]
        public async Task<IActionResult> GetAllSpecialPointsForFlight(int masterIndex)
        {
            FlightSuspiciousPointsDto result =
                await _contrillerUtilscs.GetAllSuspiciousPointsForFlightAsync(masterIndex);

            if (result == null)
                return NotFound($"No Special Points found for Master Index {masterIndex}");

            return Ok(result);
        }


        [HttpGet("export/{masterIndex}/{format}")]
        public async Task<IActionResult> ExportFlight(int masterIndex, ExportFormat format)
        {
            Stream fileStream = await _exportService.ExportFlightAsync(masterIndex, format);

            string fileName = $"flight_{masterIndex}.zip";

            return File(
                fileStream,
                "application/zip",
                fileName
            );
        }

        [HttpPost("investigations")]
        public async Task<IActionResult> CreateInvestigation([FromBody] CreateInvestigationDto dto)
        {
            Investigation investigation = new Investigation
            {
                MasterIndex = dto.MasterIndex,
                Param = dto.Param,
                Time = dto.Time,
                Value = dto.Value,
                Name = dto.Name,
                Description = dto.Description
            };

            Investigation created = await _telemetryMongoProxy.CreateInvestigationAsync(investigation);

            return Ok(created);
        }

        [HttpGet("investigations/{masterIndex:int}")]
        public async Task<IActionResult> GetInvestigationsForFlight(int masterIndex)
        {
            List<Investigation> result =
                await _telemetryMongoProxy.GetInvestigationsByFlightAsync(masterIndex);

            return Ok(result);
        }

        [HttpPost("update-investigations/{id}")]
        public async Task<IActionResult> UpdateInvestigation(string id, [FromBody] UpdateInvestigationDto dto)
        {
            Investigation? updated = await _telemetryMongoProxy.UpdateInvestigationAsync(id, dto.Name, dto.Description);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("investigations/{id}")]
        public async Task<IActionResult> DeleteInvestigation(string id)
        {
            await _telemetryMongoProxy.DeleteInvestigationAsync(id);
            return NoContent();
        }

        [HttpDelete("delete-flight/{masterIndex}")]
        public async Task<IActionResult> DeleteFlightByMasterIndex(int masterIndex)
        {
            await _telemetryMongoProxy.RemoveHistoricalReferencesToFlightAsync(masterIndex);

            await _telemetryMongoProxy.DeleteAllDataByMasterIndexAsync(masterIndex);

            return Ok(new { message = "Flight data has been deleted." });
        }
    }
}