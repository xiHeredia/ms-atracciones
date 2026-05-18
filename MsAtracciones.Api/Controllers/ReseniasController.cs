using Atracciones.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsAtracciones.Api.Dtos;
using MsAtracciones.Api.Services;

namespace MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1/resenias")]
public class ReseniasController : ControllerBase
{
    private readonly AtraccionesService _atraccionesService;

    public ReseniasController(AtraccionesService atraccionesService)
    {
        _atraccionesService = atraccionesService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar([FromQuery] Guid? atraccionGuid, CancellationToken cancellationToken)
    {
        var result = await _atraccionesService.ListarReseniasAsync(atraccionGuid, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ReseniaResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("/api/v1/atracciones/{atraccionGuid:guid}/resenias")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarPorAtraccion(Guid atraccionGuid, CancellationToken cancellationToken)
    {
        var result = await _atraccionesService.ListarReseniasAsync(atraccionGuid, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ReseniaResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Crear([FromBody] CrearReseniaRequest request, CancellationToken cancellationToken)
    {
        var id = await _atraccionesService.CrearReseniaAsync(request, cancellationToken);
        return Ok(ApiResponse<int>.Ok(id, "Resenia creada correctamente."));
    }

    [HttpPost("/api/v1/atracciones/{atraccionGuid:guid}/resenias")]
    [AllowAnonymous]
    public async Task<IActionResult> CrearPorAtraccion(Guid atraccionGuid, [FromBody] CrearReseniaRequest request, CancellationToken cancellationToken)
    {
        request.AtraccionGuid = atraccionGuid;
        var id = await _atraccionesService.CrearReseniaAsync(request, cancellationToken);
        return Ok(ApiResponse<int>.Ok(id, "Resenia creada correctamente."));
    }

    [HttpPut("{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> Actualizar(Guid guid, [FromBody] ActualizarReseniaRequest request, CancellationToken cancellationToken)
    {
        var ok = await _atraccionesService.ActualizarReseniaAsync(guid, request, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Resenia actualizada correctamente."));
    }

    [HttpDelete("{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> Eliminar(Guid guid, CancellationToken cancellationToken)
    {
        var ok = await _atraccionesService.EliminarReseniaAsync(guid, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Resenia eliminada correctamente."));
    }
}
