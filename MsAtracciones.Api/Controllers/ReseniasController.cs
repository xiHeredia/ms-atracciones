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

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Crear([FromBody] CrearReseniaRequest request, CancellationToken cancellationToken)
    {
        var id = await _atraccionesService.CrearReseniaAsync(request, cancellationToken);
        return Ok(ApiResponse<int>.Ok(id, "Resenia creada correctamente."));
    }
}
