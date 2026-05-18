using Atracciones.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using MsAtracciones.Api.Data;
using MsAtracciones.Api.Data.Entities;
using MsAtracciones.Api.Dtos;

namespace MsAtracciones.Api.Services;

public class CatalogosService
{
    private readonly AtraccionesDbContext _context;

    public CatalogosService(AtraccionesDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CatalogoItemResponse>> ListarDestinosAsync(CancellationToken cancellationToken)
    {
        return await _context.Destinos
            .AsNoTracking()
            .Where(x => x.DesEstado == "A")
            .OrderBy(x => x.DesNombre)
            .Select(x => new CatalogoItemResponse
            {
                Guid = x.DesGuid,
                Nombre = x.DesNombre,
                Extra = x.DesPais,
                ImagenUrl = x.DesImagenUrl
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CatalogoItemResponse>> ListarCategoriasAsync(CancellationToken cancellationToken)
    {
        return await _context.Categorias
            .AsNoTracking()
            .Where(x => x.CatEstado == "A")
            .OrderBy(x => x.CatNombre)
            .Select(x => new CatalogoItemResponse
            {
                Guid = x.CatGuid,
                Nombre = x.CatNombre,
                ParentId = x.CatParentId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CatalogoItemResponse>> ListarIdiomasAsync(CancellationToken cancellationToken)
    {
        return await _context.Idiomas
            .AsNoTracking()
            .Where(x => x.IdiEstado == "A")
            .OrderBy(x => x.IdiDescripcion)
            .Select(x => new CatalogoItemResponse { Guid = x.IdiGuid, Nombre = x.IdiDescripcion })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CatalogoItemResponse>> ListarIncluyeAsync(CancellationToken cancellationToken)
    {
        return await _context.Incluyes
            .AsNoTracking()
            .Where(x => x.IncEstado == "A")
            .OrderBy(x => x.IncDescripcion)
            .Select(x => new CatalogoItemResponse { Guid = x.IncGuid, Nombre = x.IncDescripcion })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ImagenResponse>> ListarImagenesAsync(CancellationToken cancellationToken)
    {
        return await _context.Imagenes
            .AsNoTracking()
            .Where(x => x.ImgEstado == "A")
            .OrderBy(x => x.ImgDescripcion)
            .ThenBy(x => x.ImgUrl)
            .Select(x => new ImagenResponse
            {
                Guid = x.ImgGuid,
                Url = x.ImgUrl,
                Descripcion = x.ImgDescripcion
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CrearDestinoAsync(CrearDestinoRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Pais))
            throw new ValidationException("Nombre y pais son obligatorios.");

        var destino = new DestinoEntity
        {
            DesGuid = Guid.NewGuid(),
            DesNombre = request.Nombre.Trim(),
            DesPais = request.Pais.Trim(),
            DesImagenUrl = request.ImagenUrl,
            DesFechaIngreso = DateTimeOffset.UtcNow,
            DesUsuarioIngreso = "api",
            DesIpIngreso = "127.0.0.1",
            DesEstado = "A"
        };

        await _context.Destinos.AddAsync(destino, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return destino.DesId;
    }

    public async Task<int> CrearCategoriaAsync(CrearCategoriaRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ValidationException("El nombre de la categoria es obligatorio.");

        var categoria = new CategoriaEntity
        {
            CatGuid = Guid.NewGuid(),
            CatNombre = request.Nombre.Trim(),
            CatParentId = request.ParentId,
            CatFechaIngreso = DateTimeOffset.UtcNow,
            CatUsuarioIngreso = "api",
            CatIpIngreso = "127.0.0.1",
            CatEstado = "A"
        };

        await _context.Categorias.AddAsync(categoria, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return categoria.CatId;
    }
}
