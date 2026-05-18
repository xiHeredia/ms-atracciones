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
}
