using ProjectX.API.Data.DTOs;

namespace ProjectX.API.Service;

public interface IPresetService
{
    Task<IEnumerable<PresetDto>> GetAllPresetsAsync(int? categoryId = null);
    Task<PresetDto?> GetPresetByIdAsync(int id);
    Task<PresetDto> CreatePresetAsync(CreatePresetDto dto);
    Task<PresetDto?> UpdatePresetAsync(int id, UpdatePresetDto dto);
    Task<bool> DeletePresetAsync(int id);
}
