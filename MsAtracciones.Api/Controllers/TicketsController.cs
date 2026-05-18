using Atracciones.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsAtracciones.Api.Dtos;
using MsAtracciones.Api.Services;

namespace MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1/tickets")]
public class TicketsController : ControllerBase
{
    private readonly AtraccionesService _atraccionesService;

    public TicketsController(AtraccionesService atraccionesService)
    {
        _atraccionesService = atraccionesService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var result = await _atraccionesService.ListarTicketsAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TicketResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("{guid:guid}/horarios")]
    [AllowAnonymous]
    public async Task<IActionResult> Horarios(Guid guid, CancellationToken cancellationToken)
    {
        var result = await _atraccionesService.ListarHorariosPorTicketAsync(guid, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<HorarioResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Crear([FromBody] CrearTicketRequest request, CancellationToken cancellationToken)
    {
        var id = await _atraccionesService.CrearTicketAsync(request, cancellationToken);
        return Ok(ApiResponse<int>.Ok(id, "Ticket creado correctamente."));
    }

    [HttpPut("{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> Actualizar(Guid guid, [FromBody] ActualizarTicketRequest request, CancellationToken cancellationToken)
    {
        var ok = await _atraccionesService.ActualizarTicketAsync(guid, request, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Ticket actualizado correctamente."));
    }

    [HttpDelete("{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> Eliminar(Guid guid, CancellationToken cancellationToken)
    {
        var ok = await _atraccionesService.EliminarTicketAsync(guid, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Ticket eliminado correctamente."));
    }
}
