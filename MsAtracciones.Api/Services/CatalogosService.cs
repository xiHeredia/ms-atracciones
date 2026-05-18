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

    public async Task<int> CrearIdiomaAsync(CrearIdiomaRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ValidationException("El nombre del idioma es obligatorio.");

        var idioma = new IdiomaEntity
        {
            IdiGuid = Guid.NewGuid(),
            IdiDescripcion = request.Nombre.Trim(),
            IdiFechaIngreso = DateTimeOffset.UtcNow,
            IdUsuarioIngreso = "api",
            IdiIpIngreso = "127.0.0.1",
            IdiEstado = "A"
        };

        await _context.Idiomas.AddAsync(idioma, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return idioma.IdiId;
    }

    public async Task<int> CrearIncluyeAsync(CrearIncluyeRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Descripcion))
            throw new ValidationException("La descripcion es obligatoria.");

        var incluye = new IncluyeEntity
        {
            IncGuid = Guid.NewGuid(),
            IncDescripcion = request.Descripcion.Trim(),
            IncEstado = "A"
        };

        await _context.Incluyes.AddAsync(incluye, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return incluye.IncId;
    }

    public async Task<int> CrearImagenAsync(CrearImagenRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            throw new ValidationException("La URL de la imagen es obligatoria.");

        var imagen = new ImagenEntity
        {
            ImgGuid = Guid.NewGuid(),
            ImgUrl = request.Url.Trim(),
            ImgDescripcion = request.Descripcion,
            ImgFechaIngreso = DateTimeOffset.UtcNow,
            ImgUsuarioIngreso = "api",
            ImgIpIngreso = "127.0.0.1",
            ImgEstado = "A"
        };

        await _context.Imagenes.AddAsync(imagen, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return imagen.ImgId;
    }

    public async Task<bool> ActualizarDestinoAsync(Guid guid, ActualizarDestinoRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Pais))
            throw new ValidationException("Nombre y pais son obligatorios.");

        var destino = await _context.Destinos.FirstOrDefaultAsync(x => x.DesGuid == guid && x.DesEstado == "A", cancellationToken);
        if (destino is null)
            throw new NotFoundException("No se encontro el destino.");

        destino.DesNombre = request.Nombre.Trim();
        destino.DesPais = request.Pais.Trim();
        destino.DesImagenUrl = request.ImagenUrl;
        destino.DesFechaMod = DateTimeOffset.UtcNow;
        destino.DesUsuarioMod = "api";
        destino.DesIpMod = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EliminarDestinoAsync(Guid guid, CancellationToken cancellationToken)
    {
        var destino = await _context.Destinos.FirstOrDefaultAsync(x => x.DesGuid == guid && x.DesEstado == "A", cancellationToken);
        if (destino is null)
            throw new NotFoundException("No se encontro el destino.");

        destino.DesEstado = "I";
        destino.DesFechaEliminacion = DateTimeOffset.UtcNow;
        destino.DesUsuarioEliminacion = "api";
        destino.DesIpEliminacion = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ActualizarCategoriaAsync(Guid guid, ActualizarCategoriaRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ValidationException("El nombre de la categoria es obligatorio.");

        var categoria = await _context.Categorias.FirstOrDefaultAsync(x => x.CatGuid == guid && x.CatEstado == "A", cancellationToken);
        if (categoria is null)
            throw new NotFoundException("No se encontro la categoria.");

        categoria.CatNombre = request.Nombre.Trim();
        categoria.CatParentId = request.ParentId;
        categoria.CatFechaMod = DateTimeOffset.UtcNow;
        categoria.CatUsuarioMod = "api";
        categoria.CatIpMod = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EliminarCategoriaAsync(Guid guid, CancellationToken cancellationToken)
    {
        var categoria = await _context.Categorias.FirstOrDefaultAsync(x => x.CatGuid == guid && x.CatEstado == "A", cancellationToken);
        if (categoria is null)
            throw new NotFoundException("No se encontro la categoria.");

        categoria.CatEstado = "I";
        categoria.CatFechaEliminacion = DateTimeOffset.UtcNow;
        categoria.CatUsuarioEliminacion = "api";
        categoria.CatIpEliminacion = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ActualizarIdiomaAsync(Guid guid, ActualizarIdiomaRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ValidationException("El nombre del idioma es obligatorio.");

        var idioma = await _context.Idiomas.FirstOrDefaultAsync(x => x.IdiGuid == guid && x.IdiEstado == "A", cancellationToken);
        if (idioma is null)
            throw new NotFoundException("No se encontro el idioma.");

        idioma.IdiDescripcion = request.Nombre.Trim();
        idioma.IdiFechaMod = DateTimeOffset.UtcNow;
        idioma.IdUsuarioMod = "api";
        idioma.IdiIpMod = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EliminarIdiomaAsync(Guid guid, CancellationToken cancellationToken)
    {
        var idioma = await _context.Idiomas.FirstOrDefaultAsync(x => x.IdiGuid == guid && x.IdiEstado == "A", cancellationToken);
        if (idioma is null)
            throw new NotFoundException("No se encontro el idioma.");

        idioma.IdiEstado = "I";
        idioma.IdiFechaEliminacion = DateTimeOffset.UtcNow;
        idioma.IdUsuarioEliminacion = "api";
        idioma.IdiIpEliminacion = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ActualizarIncluyeAsync(Guid guid, ActualizarIncluyeRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Descripcion))
            throw new ValidationException("La descripcion es obligatoria.");

        var incluye = await _context.Incluyes.FirstOrDefaultAsync(x => x.IncGuid == guid && x.IncEstado == "A", cancellationToken);
        if (incluye is null)
            throw new NotFoundException("No se encontro el elemento incluido.");

        incluye.IncDescripcion = request.Descripcion.Trim();

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EliminarIncluyeAsync(Guid guid, CancellationToken cancellationToken)
    {
        var incluye = await _context.Incluyes.FirstOrDefaultAsync(x => x.IncGuid == guid && x.IncEstado == "A", cancellationToken);
        if (incluye is null)
            throw new NotFoundException("No se encontro el elemento incluido.");

        incluye.IncEstado = "I";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ActualizarImagenAsync(Guid guid, ActualizarImagenRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            throw new ValidationException("La URL de la imagen es obligatoria.");

        var imagen = await _context.Imagenes.FirstOrDefaultAsync(x => x.ImgGuid == guid && x.ImgEstado == "A", cancellationToken);
        if (imagen is null)
            throw new NotFoundException("No se encontro la imagen.");

        imagen.ImgUrl = request.Url.Trim();
        imagen.ImgDescripcion = request.Descripcion;
        imagen.ImgFechaMod = DateTimeOffset.UtcNow;
        imagen.ImgUsuarioMod = "api";
        imagen.ImgIpMod = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EliminarImagenAsync(Guid guid, CancellationToken cancellationToken)
    {
        var imagen = await _context.Imagenes.FirstOrDefaultAsync(x => x.ImgGuid == guid && x.ImgEstado == "A", cancellationToken);
        if (imagen is null)
            throw new NotFoundException("No se encontro la imagen.");

        imagen.ImgEstado = "I";
        imagen.ImgFechaEliminacion = DateTimeOffset.UtcNow;
        imagen.ImgUsuarioEliminacion = "api";
        imagen.ImgIpEliminacion = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
