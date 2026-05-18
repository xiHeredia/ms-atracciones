using Atracciones.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsAtracciones.Api.Dtos;
using MsAtracciones.Api.Services;

namespace MsAtracciones.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/horarios")]
public class HorariosController : ControllerBase
{
    private readonly AtraccionesService _atraccionesService;

    public HorariosController(AtraccionesService atraccionesService)
    {
        _atraccionesService = atraccionesService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var result = await _atraccionesService.ListarHorariosAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<HorarioResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearHorarioRequest request, CancellationToken cancellationToken)
    {
        var id = await _atraccionesService.CrearHorarioAsync(request, cancellationToken);
        return Ok(ApiResponse<int>.Ok(id, "Horario creado correctamente."));
    }

    [HttpPut("{guid:guid}")]
    public async Task<IActionResult> Actualizar(Guid guid, [FromBody] ActualizarHorarioRequest request, CancellationToken cancellationToken)
    {
        var ok = await _atraccionesService.ActualizarHorarioAsync(guid, request, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Horario actualizado correctamente."));
    }

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> Eliminar(Guid guid, CancellationToken cancellationToken)
    {
        var ok = await _atraccionesService.EliminarHorarioAsync(guid, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Horario eliminado correctamente."));
    }
}
