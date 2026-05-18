using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        request.UsuarioCreacion = GetUsuarioActual();
        var id = await _atraccionesService.CrearReseniaAsync(request, cancellationToken);
        return Ok(ApiResponse<int>.Ok(id, "Resenia creada correctamente."));
    }

    [HttpPost("/api/v1/atracciones/{atraccionGuid:guid}/resenias")]
    [AllowAnonymous]
    public async Task<IActionResult> CrearPorAtraccion(Guid atraccionGuid, [FromBody] CrearReseniaRequest request, CancellationToken cancellationToken)
    {
        request.AtraccionGuid = atraccionGuid;
        request.UsuarioCreacion = GetUsuarioActual();
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

    private string GetUsuarioActual()
    {
        if (User.Identity?.IsAuthenticated != true)
            return "booking-public";

        return User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
            ?? User.Identity.Name
            ?? "booking-authenticated";
    }
}
