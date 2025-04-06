using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WindTurbine.Domain.Entities;
using WindTurbineApi.Application.DTOs;
using WindTurbineApi.Application.Interfaces;

namespace WindTurbineApi.API.Controllers
{
    [ApiController]
    [Route("api/sensor")]
    public class SensorController : ControllerBase
    {
        private readonly ISensorService _sensorService;
        private readonly IMapper _mapper;
        private readonly ILogger<SensorController> _logger;

        public SensorController(ISensorService sensorService, IMapper mapper, ILogger<SensorController> logger)
        {
            _sensorService = sensorService;
            _mapper = mapper;
            _logger = logger;
        }

        #region /api/sensor

        /// <summary>
        /// Retrieves a list of all sensors
        /// </summary>
        /// <returns>A list of sensors</returns>
        /// <response code="200">Returns the list of sensors.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SensorDto>), 200)]
        public async Task<ActionResult<IEnumerable<SensorDto>>> GetAllSensors()
        {
            _logger.LogInformation("Retrieving all sensors.");
            var sensors = await _sensorService.GetAllSensorsAsync();
            return Ok(_mapper.Map<IEnumerable<SensorDto>>(sensors));
        }

        /// <summary>
        /// Creates a new sensor
        /// </summary>
        /// <returns>Created sensor.</returns>
        /// <response code="200">Returns the created sensor</response>
        /// <response code="400">Failure to create sensor</response>
        /// <response code="400">Failure to create sensor due to name already in use by another sensor</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SensorDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<SensorDto>> CreateSensor([FromBody] CreateSensorDto sensorDto)
        {
            _logger.LogInformation("Attempting to create a new sensor with name: {SensorName}", sensorDto.Name);

            // Map the DTO to the Domain entity *before* passing to the service
            var sensor = _mapper.Map<Sensor>(sensorDto);

            var creationResult = await _sensorService.CreateSensorAsync(sensor);

            if (creationResult.IsSuccess)
            {
                var createdSensorDto = _mapper.Map<SensorDto>(creationResult.Value);
                return CreatedAtAction(nameof(GetSensorById), new { id = createdSensorDto.Id }, createdSensorDto);
            }
            else
            {
                _logger.LogError("Sensor creation failed for name: {SensorName}. Error: {Error}", sensorDto.Name, creationResult.Error);

                if (creationResult.Error?.Contains("already exists") ?? false)
                {
                    return Conflict(creationResult.Error);
                }

                return BadRequest(creationResult.Error);
            }
        }

        /// <summary>
        /// Retrieves metadata for the list of all sensors
        /// </summary>
        /// <response code="200">Indicates the resource exists.</response>
        [HttpHead("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> HeadAllSensors()
        {
            _logger.LogInformation("HEAD request received for all sensors.");
            var sensors = await _sensorService.GetAllSensorsAsync();
            Response.Headers.Append("X-Total-Count", sensors?.Count().ToString() ?? "0");
            return Ok();
        }

        /// <summary>
        /// Gets the allowed HTTP methods for the sensors
        /// </summary>
        /// <response code="200">Returns the allowed HTTP methods in the 'Allow' header.</response>
        [HttpOptions("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult OptionsSensors()
        {
            _logger.LogInformation("OPTIONS request received for the sensors resource (creation).");
            HttpContext.Response.Headers.Append("Allow", "GET, POST, HEAD, OPTIONS");
            return Ok();
        }

        #endregion /api/sensor


        #region /api/sensor/id

        /// <summary>
        /// Retrieves a sensor by Id
        /// </summary>
        /// <returns>A specific sensor.</returns>
        /// <response code="200">Returns the sensor</response>
        /// <response code="404">No sensor found with this id</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SensorDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SensorDto>> GetSensorById(Guid id)
        {
            _logger.LogInformation($"Retrieving sensor with id: {id}");
            var sensor = await _sensorService.GetSensorByIdAsync(id);
            if (sensor == null)
            {
                _logger.LogWarning($"Sensor with id: {id} not found.");
                return NotFound();
            }
            return Ok(_mapper.Map<SensorDto>(sensor));
        }

        /// <summary>
        /// Deletes a sensor by Id.
        /// </summary>
        /// <param name="id">The ID of the sensor to delete.</param>
        /// <response code="204">The sensor was successfully deleted.</response>
        /// <response code="404">No sensor found with this id.</response>
        /// <response code="500">An error occurred while deleting the sensor.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSensor(Guid id)
        {
            _logger.LogInformation($"Attempting to delete sensor with id: {id}");

            var deletionResult = await _sensorService.DeleteSensorAsync(id);

            if (deletionResult.IsSuccess)
            {
                _logger.LogInformation($"Sensor with id: {id} successfully deleted.");
                return NoContent(); // 204 No Content for successful deletion
            }
            else if (deletionResult.IsNotFound)
            {
                _logger.LogWarning($"Sensor with id: {id} not found for deletion.");
                return NotFound();
            }
            else
            {
                _logger.LogError($"Error deleting sensor with id: {id}. Error: {deletionResult.Error}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the sensor.");
            }
        }

        /// <summary>
        /// Retrieves metadata for a specific sensor
        /// </summary>
        /// <param name="id">The ID of the sensor.</param>
        /// <response code="200">Indicates the sesnor exists</response>
        /// <response code="404">If the sensor with the given ID is not found.</response>
        [HttpHead("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HeadSensorById(Guid id)
        {
            _logger.LogInformation("HEAD request received for sensor with ID: {Id}", id);
            var sensor = await _sensorService.GetSensorByIdAsync(id);
            if (sensor == null)
            {
                return NotFound();
            }
            return Ok();
        }

        /// <summary>
        /// Gets the allowed HTTP methods for a specific sensor
        /// </summary>
        /// <param name="id">The ID of the sensor</param>
        /// <response code="200">Returns the allowed HTTP methods in the 'Allow' header</response>
        [HttpOptions("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult OptionsSensorById(Guid id)
        {
            _logger.LogInformation("OPTIONS request received for sensor with ID: {Id}", id);

            HttpContext.Response.Headers.Append("Allow", "GET, DELETE, HEAD, OPTIONS");
            return Ok();
        }

        #endregion /api/sensor/id


        #region /api/sensor/name

        /// <summary>
        /// Retrieves a specific sensor by name
        /// </summary>
        /// <returns>Details of a specific sensor</returns>
        /// <response code="200">Returns the sensor</response>
        /// <response code="404">No sensor found with this name</response>
        [HttpGet("name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SensorDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SensorDto>> GetSensorByName(string name)
        {
            _logger.LogInformation($"Retrieving sensor with name: {name}");
            var sensor = await _sensorService.GetSensorByNameAsync(name);
            if (sensor == null)
            {
                _logger.LogWarning($"Sensor with name: {name} not found.");
                return NotFound();
            }
            return Ok(_mapper.Map<SensorDto>(sensor));
        }

        /// <summary>
        /// Retrieves metadata for a specific sensor by name.
        /// </summary>
        /// <param name="name">The name of the sensor.</param>
        /// <response code="200">Indicates the sensor exists</response>
        /// <response code="404">If the sensor with the given name is not found</response>
        [HttpHead("name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HeadSensorByName(string name)
        {
            _logger.LogInformation("HEAD request received for sensor with name: {Name}", name);
            var sensor = await _sensorService.GetSensorByNameAsync(name);
            if (sensor == null)
            {
                return NotFound();
            }
            return Ok();
        }

        /// <summary>
        /// Gets the allowed HTTP methods for a specific sensor by name.
        /// </summary>
        /// <param name="name">The name of the sensor</param>
        /// <response code="200">Returns the allowed HTTP methods in the 'Allow' header.</response>
        [HttpOptions("name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult OptionsSensorByName(string name)
        {
            _logger.LogInformation("OPTIONS request received for sensor with name: {Name}", name);

            HttpContext.Response.Headers.Append("Allow", "GET, HEAD, OPTIONS");
            return Ok();
        }

        #endregion /api/sensor/name
    }
}
