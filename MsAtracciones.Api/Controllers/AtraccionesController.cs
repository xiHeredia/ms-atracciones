using Atracciones.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsAtracciones.Api.Dtos;
using MsAtracciones.Api.Services;

namespace MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1/atracciones")]
public class AtraccionesController : ControllerBase
{
    private readonly AtraccionesService _atraccionesService;

    public AtraccionesController(AtraccionesService atraccionesService)
    {
        _atraccionesService = atraccionesService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar(
        [FromQuery] string? nombre,
        [FromQuery] Guid? destinoGuid,
        [FromQuery] Guid? categoriaGuid,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] int? limit,
        CancellationToken cancellationToken)
    {
        var result = await _atraccionesService.ListarAtraccionesAsync(nombre, destinoGuid, categoriaGuid, page, pageSize ?? limit, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<AtraccionResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("filtros")]
    [AllowAnonymous]
    public async Task<IActionResult> Filtros(CancellationToken cancellationToken)
    {
        var result = await _atraccionesService.ObtenerFiltrosAsync(cancellationToken);
        return Ok(ApiResponse<FiltrosAtraccionesResponse>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("{guid:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObtenerPorGuid(Guid guid, CancellationToken cancellationToken)
    {
        var result = await _atraccionesService.ObtenerDetalleAsync(guid, cancellationToken);
        return Ok(ApiResponse<AtraccionDetalleResponse>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("{guid:guid}/tickets")]
    [AllowAnonymous]
    public async Task<IActionResult> Tickets(Guid guid, CancellationToken cancellationToken)
    {
        var result = await _atraccionesService.ListarTicketsPorAtraccionAsync(guid, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TicketResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("{guid:guid}/horarios-disponibles")]
    [AllowAnonymous]
    public async Task<IActionResult> HorariosDisponibles(Guid guid, CancellationToken cancellationToken)
    {
        var result = await _atraccionesService.ListarHorariosDisponiblesPorAtraccionAsync(guid, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<HorarioResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("{guid:guid}/horarios")]
    [AllowAnonymous]
    public async Task<IActionResult> Horarios(Guid guid, [FromQuery] bool disponibles, CancellationToken cancellationToken)
    {
        var result = await _atraccionesService.ListarHorariosPorAtraccionAsync(guid, disponibles, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<HorarioResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("{guid:guid}/horarios/{horarioId:guid}/tickets")]
    [AllowAnonymous]
    public async Task<IActionResult> TicketsPorHorario(Guid guid, Guid horarioId, CancellationToken cancellationToken)
    {
        var result = await _atraccionesService.ListarTicketsPorHorarioAsync(guid, horarioId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TicketResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Crear([FromBody] CrearAtraccionRequest request, CancellationToken cancellationToken)
    {
        var id = await _atraccionesService.CrearAtraccionAsync(request, cancellationToken);
        return Ok(ApiResponse<int>.Ok(id, "Atraccion creada correctamente."));
    }

    [HttpPut("{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> Actualizar(Guid guid, [FromBody] ActualizarAtraccionRequest request, CancellationToken cancellationToken)
    {
        var ok = await _atraccionesService.ActualizarAtraccionAsync(guid, request, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Atraccion actualizada correctamente."));
    }

    [HttpDelete("{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> Eliminar(Guid guid, CancellationToken cancellationToken)
    {
        var ok = await _atraccionesService.EliminarAtraccionAsync(guid, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Atraccion eliminada logicamente."));
    }
}
