using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX.API.Data.DTOs;
using ProjectX.API.Service;

namespace ProjectX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PresetsController : ControllerBase
{
    private readonly IPresetService _presetService;
    private readonly ILogger<PresetsController> _logger;

    public PresetsController(IPresetService presetService, ILogger<PresetsController> logger)
    {
        _presetService = presetService;
        _logger = logger;
    }

    /// <summary>
    /// Get all presets with optional category filter (public)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PresetDto>>> GetAllPresets([FromQuery] int? categoryId)
    {
        try
        {
            var presets = await _presetService.GetAllPresetsAsync(categoryId);
            return Ok(presets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching presets");
            return StatusCode(500, new { message = "An error occurred while fetching presets" });
        }
    }

    /// <summary>
    /// Get preset by ID (public)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PresetDto>> GetPreset(int id)
    {
        try
        {
            var preset = await _presetService.GetPresetByIdAsync(id);
            if (preset == null)
            {
                return NotFound(new { message = $"Preset with ID {id} not found" });
            }

            return Ok(preset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching preset {Id}", id);
            return StatusCode(500, new { message = "An error occurred while fetching the preset" });
        }
    }

    /// <summary>
    /// Create a new preset with images and file (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PresetDto>> CreatePreset([FromForm] CreatePresetDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var preset = await _presetService.CreatePresetAsync(dto);
            return CreatedAtAction(nameof(GetPreset), new { id = preset.Id }, preset);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating preset");
            return StatusCode(500, new { message = "An error occurred while creating the preset" });
        }
    }

    /// <summary>
    /// Update an existing preset (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PresetDto>> UpdatePreset(int id, [FromForm] UpdatePresetDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var preset = await _presetService.UpdatePresetAsync(id, dto);
            if (preset == null)
            {
                return NotFound(new { message = $"Preset with ID {id} not found" });
            }

            return Ok(preset);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating preset {Id}", id);
            return StatusCode(500, new { message = "An error occurred while updating the preset" });
        }
    }

    /// <summary>
    /// Delete a preset (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeletePreset(int id)
    {
        try
        {
            var result = await _presetService.DeletePresetAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Preset with ID {id} not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting preset {Id}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the preset" });
        }
    }
}
