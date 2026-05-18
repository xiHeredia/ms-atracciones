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

    [HttpPost("idiomas")]
    [Authorize]
    public async Task<IActionResult> CrearIdioma([FromBody] CrearIdiomaRequest request, CancellationToken cancellationToken)
    {
        var id = await _catalogosService.CrearIdiomaAsync(request, cancellationToken);
        return Ok(ApiResponse<int>.Ok(id, "Idioma creado correctamente."));
    }

    [HttpPost("incluye")]
    [HttpPost("incluyes")]
    [Authorize]
    public async Task<IActionResult> CrearIncluye([FromBody] CrearIncluyeRequest request, CancellationToken cancellationToken)
    {
        var id = await _catalogosService.CrearIncluyeAsync(request, cancellationToken);
        return Ok(ApiResponse<int>.Ok(id, "Elemento incluido creado correctamente."));
    }

    [HttpPost("imagenes")]
    [Authorize]
    public async Task<IActionResult> CrearImagen([FromBody] CrearImagenRequest request, CancellationToken cancellationToken)
    {
        var id = await _catalogosService.CrearImagenAsync(request, cancellationToken);
        return Ok(ApiResponse<int>.Ok(id, "Imagen creada correctamente."));
    }

    [HttpPut("destinos/{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> ActualizarDestino(Guid guid, [FromBody] ActualizarDestinoRequest request, CancellationToken cancellationToken)
    {
        var ok = await _catalogosService.ActualizarDestinoAsync(guid, request, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Destino actualizado correctamente."));
    }

    [HttpDelete("destinos/{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> EliminarDestino(Guid guid, CancellationToken cancellationToken)
    {
        var ok = await _catalogosService.EliminarDestinoAsync(guid, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Destino eliminado correctamente."));
    }

    [HttpPut("categorias/{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> ActualizarCategoria(Guid guid, [FromBody] ActualizarCategoriaRequest request, CancellationToken cancellationToken)
    {
        var ok = await _catalogosService.ActualizarCategoriaAsync(guid, request, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Categoria actualizada correctamente."));
    }

    [HttpDelete("categorias/{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> EliminarCategoria(Guid guid, CancellationToken cancellationToken)
    {
        var ok = await _catalogosService.EliminarCategoriaAsync(guid, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Categoria eliminada correctamente."));
    }

    [HttpPut("idiomas/{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> ActualizarIdioma(Guid guid, [FromBody] ActualizarIdiomaRequest request, CancellationToken cancellationToken)
    {
        var ok = await _catalogosService.ActualizarIdiomaAsync(guid, request, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Idioma actualizado correctamente."));
    }

    [HttpDelete("idiomas/{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> EliminarIdioma(Guid guid, CancellationToken cancellationToken)
    {
        var ok = await _catalogosService.EliminarIdiomaAsync(guid, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Idioma eliminado correctamente."));
    }

    [HttpPut("incluye/{guid:guid}")]
    [HttpPut("incluyes/{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> ActualizarIncluye(Guid guid, [FromBody] ActualizarIncluyeRequest request, CancellationToken cancellationToken)
    {
        var ok = await _catalogosService.ActualizarIncluyeAsync(guid, request, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Elemento incluido actualizado correctamente."));
    }

    [HttpDelete("incluye/{guid:guid}")]
    [HttpDelete("incluyes/{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> EliminarIncluye(Guid guid, CancellationToken cancellationToken)
    {
        var ok = await _catalogosService.EliminarIncluyeAsync(guid, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Elemento incluido eliminado correctamente."));
    }

    [HttpPut("imagenes/{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> ActualizarImagen(Guid guid, [FromBody] ActualizarImagenRequest request, CancellationToken cancellationToken)
    {
        var ok = await _catalogosService.ActualizarImagenAsync(guid, request, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Imagen actualizada correctamente."));
    }

    [HttpDelete("imagenes/{guid:guid}")]
    [Authorize]
    public async Task<IActionResult> EliminarImagen(Guid guid, CancellationToken cancellationToken)
    {
        var ok = await _catalogosService.EliminarImagenAsync(guid, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok, "Imagen eliminada correctamente."));
    }
}
