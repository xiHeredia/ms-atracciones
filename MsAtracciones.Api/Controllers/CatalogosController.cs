using Atracciones.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsAtracciones.Api.Dtos;
using MsAtracciones.Api.Services;

namespace MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class CatalogosController : ControllerBase
{
    private readonly CatalogosService _catalogosService;

    public CatalogosController(CatalogosService catalogosService)
    {
        _catalogosService = catalogosService;
    }

    [HttpGet("destinos")]
    [AllowAnonymous]
    public async Task<IActionResult> Destinos(CancellationToken cancellationToken)
    {
        var result = await _catalogosService.ListarDestinosAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CatalogoItemResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("categorias")]
    [AllowAnonymous]
    public async Task<IActionResult> Categorias(CancellationToken cancellationToken)
    {
        var result = await _catalogosService.ListarCategoriasAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CatalogoItemResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("idiomas")]
    [AllowAnonymous]
    public async Task<IActionResult> Idiomas(CancellationToken cancellationToken)
    {
        var result = await _catalogosService.ListarIdiomasAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CatalogoItemResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("incluye")]
    [AllowAnonymous]
    public async Task<IActionResult> Incluye(CancellationToken cancellationToken)
    {
        var result = await _catalogosService.ListarIncluyeAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CatalogoItemResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("incluyes")]
    [AllowAnonymous]
    public async Task<IActionResult> Incluyes(CancellationToken cancellationToken)
    {
        var result = await _catalogosService.ListarIncluyeAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CatalogoItemResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpGet("imagenes")]
    [AllowAnonymous]
    public async Task<IActionResult> Imagenes(CancellationToken cancellationToken)
    {
        var result = await _catalogosService.ListarImagenesAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ImagenResponse>>.Ok(result, "Consulta exitosa."));
    }

    [HttpPost("destinos")]
    [Authorize]
    public async Task<IActionResult> CrearDestino([FromBody] CrearDestinoRequest request, CancellationToken cancellationToken)
    {
        var id = await _catalogosService.CrearDestinoAsync(request, cancellationToken);
        return Ok(ApiResponse<int>.Ok(id, "Destino creado correctamente."));
    }

    [HttpPost("categorias")]
    [Authorize]
    public async Task<IActionResult> CrearCategoria([FromBody] CrearCategoriaRequest request, CancellationToken cancellationToken)
    {
        var id = await _catalogosService.CrearCategoriaAsync(request, cancellationToken);
        return Ok(ApiResponse<int>.Ok(id, "Categoria creada correctamente."));
    }
}
