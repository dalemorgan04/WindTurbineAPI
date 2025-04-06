using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WindTurbine.Domain.Entities;
using WindTurbineApi.Application.DTOs;
using WindTurbineApi.Application.Interfaces;

namespace WindTurbineApi.API.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class SensorRecordController : ControllerBase
    {
        private readonly ISensorRecordService _dataRecordService;
        private readonly ISensorService _sensorService;
        private readonly IMapper _mapper;
        private readonly ILogger<SensorRecordController> _logger;

        public SensorRecordController(ISensorRecordService dataRecordService, ISensorService sensorService, IMapper mapper, ILogger<SensorRecordController> logger)
        {
            _dataRecordService = dataRecordService;
            _sensorService = sensorService;
            _mapper = mapper;
            _logger = logger;
        }


        #region /api/data

        /// <summary>
        /// Retrieves a filtered list of all sensor records matching query parameters.
        /// </summary>
        /// <returns>A list of sensor details</returns>
        /// <response code="200">Returns the list of matching sensor records</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SensorRecordDto))]
        public async Task<ActionResult<IEnumerable<SensorRecordDto>>> GetAllSensorRecords(
            [FromQuery] string? sensorName,
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromQuery] double? aboveValue,
            [FromQuery] double? belowValue)
        {
            _logger.LogInformation("Retrieving data records with optional filters.");

            DateTime? parsedStartDate = null;
            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var dtStart))
            {
                parsedStartDate = dtStart;
            }

            DateTime? parsedEndDate = null;
            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var dtEnd))
            {
                parsedEndDate = dtEnd;
            }

            var records = await _dataRecordService.GetFilteredAsync(sensorName, parsedStartDate, parsedEndDate, aboveValue, belowValue);
            var dtos = _mapper.Map<IEnumerable<SensorRecordDto>>(records);
            return Ok(dtos);
        }

        /// <summary>
        /// Creates a new sensor data record
        /// </summary>
        /// <param name="recordDto">The data for the new sensor record</param>
        /// <returns>The newly created sensor record</returns>
        /// <response code="201">Returns the newly created sensor record</response>
        /// <response code="400">If the request is invalid (e.g., sensor not found, invalid timestamp format).</response>
        [HttpPost]
        [ProducesResponseType(typeof(SensorRecordDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SensorRecordDto>> CreateSensorRecord([FromBody] CreateSensorRecordDto recordDto)
        {
            _logger.LogInformation("Creating a new data record.");

            var sensor = await _sensorService.GetSensorByNameAsync(recordDto.SensorName); // Assuming sensor names are globally unique for this demo

            if (sensor == null)
            {
                _logger.LogError($"Sensor with name '{recordDto.SensorName}' not found.");
                return BadRequest($"Sensor with name '{recordDto.SensorName}' not found.");
            }

            if (!DateTime.TryParseExact(recordDto.Timestamp, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                _logger.LogError($"Invalid date format: '{recordDto.Timestamp}'. Expected 'yyyy-MM-dd HH:mm'.");
                return BadRequest($"Invalid date format: '{recordDto.Timestamp}'. Expected 'yyyy-MM-dd HH:mm'.");
            }

            var record = _mapper.Map<SensorRecord>(recordDto);
            record.Timestamp = parsedDate.ToUniversalTime(); // Store as UTC
            record.SensorId = sensor.Id;
            record.Sensor = sensor; // Ensure the navigation property is set

            var createdRecord = await _dataRecordService.CreateSensorRecordAsync(record);

            if (createdRecord == null)
            {
                _logger.LogError("Data record creation failed.");
                return BadRequest("Failed to create data record.");
            }

            var createdRecordDto = _mapper.Map<SensorRecordDto>(createdRecord);
            return CreatedAtAction(nameof(GetSensorRecordById), new { id = createdRecordDto.Id }, createdRecordDto);
        }

        /// <summary>
        /// Retrieves metadata for the filtered list of all sensor records
        /// </summary>
        /// <response code="200">Indicates the resource exists and provides the total count</response>
        [HttpHead("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> HeadAllSensorRecords(
            [FromQuery] string? sensorName,
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromQuery] double? aboveValue,
            [FromQuery] double? belowValue)
        {
            _logger.LogInformation("HEAD request received for data records with optional filters.");

            DateTime? parsedStartDate = null;
            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var dtStart))
            {
                parsedStartDate = dtStart;
            }

            DateTime? parsedEndDate = null;
            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var dtEnd))
            {
                parsedEndDate = dtEnd;
            }

            var records = await _dataRecordService.GetFilteredAsync(sensorName, parsedStartDate, parsedEndDate, aboveValue, belowValue);
            Response?.Headers?.Append("X-Total-Count", records?.Count().ToString() ?? "0");
            return Ok();
        }

        /// <summary>
        /// Gets the allowed HTTP methods for the sensor records resource
        /// </summary>
        /// <response code="200">Returns the allowed HTTP methods in the 'Allow' header.</response>
        [HttpOptions("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult OptionsAllSensorRecords()
        {
            _logger.LogInformation("OPTIONS request received for the sensor records resource");
            HttpContext.Response.Headers.Append("Allow", "GET, POST, HEAD, OPTIONS");
            return Ok();
        }

        #endregion /api/data


        #region /api/data/id

        /// <summary>
        /// Retrieves a specific sensor record by Id
        /// </summary>
        /// <returns>A specific sensor record</returns>
        /// <response code="200">Returns a specific sensor record by Id</response>
        /// <response code="404">Sensor record with specific id not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SensorRecordDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SensorRecordDto>> GetSensorRecordById(Guid id)
        {
            _logger.LogInformation($"Retrieving data record with id: {id}");
            var record = await _dataRecordService.GetSensorRecordByIdAsync(id);
            if (record == null)
            {
                _logger.LogWarning($"Data record with id: {id} not found.");
                return NotFound();
            }
            return Ok(_mapper.Map<SensorRecordDto>(record));
        }

        /// <summary>
        /// Deletes a specific sensor data record by Id
        /// </summary>
        /// <param name="id">The ID of the sensor data record to delete</param>
        /// <response code="204">The sensor data record was successfully deleted</response>
        /// <response code="404">No sensor data record found with this id</response>
        /// <response code="500">An error occurred while deleting the sensor data record</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSensorRecord(Guid id)
        {
            _logger.LogInformation($"Attempting to delete data record with id: {id}");

            var deletionResult = await _dataRecordService.DeleteSensorRecordAsync(id);

            if (deletionResult.IsSuccess)
            {
                _logger.LogInformation($"Data record with id: {id} successfully deleted.");
                return NoContent();
            }
            else if (deletionResult.IsNotFound)
            {
                _logger.LogWarning($"Data record with id: {id} not found for deletion.");
                return NotFound();
            }
            else
            {
                _logger.LogError($"Error deleting data record with id: {id}. Error: {deletionResult.Error}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the sensor data record.");
            }
        }

        /// <summary>
        /// Retrieves metadata for a specific sensor record by Id
        /// </summary>
        /// <param name="id">The ID of the sensor record</param>
        /// <response code="200">Indicates the resource exists</response>
        /// <response code="404">If the sensor record with the given ID is not found</response>
        [HttpHead("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HeadSensorRecordById(Guid id)
        {
            _logger.LogInformation("HEAD request received for data record with ID: {Id}", id);
            var record = await _dataRecordService.GetSensorRecordByIdAsync(id);
            if (record == null)
            {
                return NotFound();
            }
            return Ok();
        }

        /// <summary>
        /// Gets the allowed HTTP methods for a specific sensor record resource.
        /// </summary>
        /// <param name="id">The ID of the sensor record.</param>
        /// <response code="200">Returns the allowed HTTP methods in the 'Allow' header.</response>
        [HttpOptions("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult OptionsSensorRecordById(Guid id)
        {
            _logger.LogInformation("OPTIONS request received for data record with ID: {Id}", id);
            HttpContext.Response.Headers.Append("Allow", "GET, DELETE, HEAD, OPTIONS");
            return Ok();
        }

        #endregion /api/data/id
    }
}