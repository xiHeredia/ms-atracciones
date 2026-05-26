using Atracciones.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using MsAtracciones.Api.Data;
using MsAtracciones.Api.Data.Entities;
using MsAtracciones.Api.Dtos;

namespace MsAtracciones.Api.Services;

public class AtraccionesService
{
    private static readonly HashSet<string> TiposParticipante = new(StringComparer.OrdinalIgnoreCase)
    {
        "Adulto",
        "Niño",
        "Nino",
        "Grupo",
        "Estudiante",
        "Senior"
    };

    private readonly AtraccionesDbContext _context;

    public AtraccionesService(AtraccionesDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AtraccionResponse>> ListarAtraccionesAsync(
        string? nombre,
        Guid? destinoGuid,
        Guid? categoriaGuid,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.Atracciones
            .AsNoTracking()
            .Where(x => x.AtEstado == "A" && x.AtDisponible);

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(x => EF.Functions.ILike(x.AtNombre, $"%{nombre.Trim()}%"));

        if (destinoGuid is not null)
            query = query.Where(x => x.DesGuid == destinoGuid.Value);

        if (categoriaGuid is not null)
        {
            var ids = _context.CategoriaAtracciones
                .Where(x => x.CatGuid == categoriaGuid.Value && x.CaEstado == "A")
                .Select(x => x.AtId);

            query = query.Where(x => ids.Contains(x.AtId));
        }

        IQueryable<AtraccionEntity> ordered = query.OrderBy(x => x.AtNombre);

        if (page is not null || pageSize is not null)
        {
            var currentPage = Math.Max(page ?? 1, 1);
            var currentPageSize = Math.Clamp(pageSize ?? 10, 1, 50);

            ordered = ordered
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize);
        }

        var atracciones = await ordered.ToListAsync(cancellationToken);

        return await MapAtraccionesAsync(atracciones, cancellationToken);
    }

    public async Task<AtraccionDetalleResponse> ObtenerDetalleAsync(Guid guid, CancellationToken cancellationToken)
    {
        var atraccion = await _context.Atracciones
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AtGuid == guid && x.AtEstado == "A" && x.AtDisponible, cancellationToken);

        if (atraccion is null)
            throw new NotFoundException("No se encontro la atraccion.");

        var response = (await MapAtraccionesAsync(new[] { atraccion }, cancellationToken))
            .Select(x => new AtraccionDetalleResponse
            {
                Guid = x.Guid,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                PrecioReferencia = x.PrecioReferencia,
                DestinoGuid = x.DestinoGuid,
                DestinoNombre = x.DestinoNombre,
                DestinoPais = x.DestinoPais,
                ImagenUrl = x.ImagenUrl,
                DuracionMinutos = x.DuracionMinutos,
                Direccion = x.Direccion,
                PuntoEncuentro = x.PuntoEncuentro,
                IncluyeTransporte = x.IncluyeTransporte,
                IncluyeAcompaniante = x.IncluyeAcompaniante,
                Disponible = x.Disponible,
                TotalResenias = x.TotalResenias
            })
            .Single();

        response.Categorias = await ObtenerCategoriasAsync(atraccion.AtId, cancellationToken);
        response.Idiomas = await ObtenerIdiomasAsync(atraccion.AtId, cancellationToken);
        response.Incluye = await ObtenerIncluyeAsync(atraccion.AtId, cancellationToken);
        response.Imagenes = await ObtenerImagenesAsync(atraccion.AtId, cancellationToken);
        response.Tickets = await ListarTicketsPorAtraccionAsync(guid, cancellationToken);

        return response;
    }

    public async Task<IReadOnlyList<TicketResponse>> ListarTicketsPorAtraccionAsync(Guid atraccionGuid, CancellationToken cancellationToken)
    {
        var atraccion = await _context.Atracciones
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AtGuid == atraccionGuid && x.AtEstado == "A", cancellationToken);

        if (atraccion is null)
            throw new NotFoundException("No se encontro la atraccion.");

        var tickets = await _context.Tickets
            .AsNoTracking()
            .Where(x => x.AtId == atraccion.AtId && x.TckEstado == "A")
            .OrderBy(x => x.TckPrecio)
            .ToListAsync(cancellationToken);

        return tickets.Select(x => ToTicketResponse(x, atraccion.AtGuid, atraccion.AtNombre)).ToList();
    }

    public async Task<IReadOnlyList<TicketResponse>> ListarTicketsAsync(CancellationToken cancellationToken)
    {
        var tickets = await _context.Tickets
            .AsNoTracking()
            .Where(x => x.TckEstado == "A")
            .OrderBy(x => x.TckTitulo)
            .ToListAsync(cancellationToken);

        if (tickets.Count == 0)
            return Array.Empty<TicketResponse>();

        var atraccionIds = tickets.Select(x => x.AtId).Distinct().ToList();
        var atracciones = await _context.Atracciones
            .AsNoTracking()
            .Where(x => atraccionIds.Contains(x.AtId))
            .ToDictionaryAsync(x => x.AtId, cancellationToken);

        return tickets.Select(x =>
        {
            atracciones.TryGetValue(x.AtId, out var atraccion);
            return ToTicketResponse(x, atraccion?.AtGuid ?? Guid.Empty, atraccion?.AtNombre);
        }).ToList();
    }

    public async Task<IReadOnlyList<HorarioResponse>> ListarHorariosDisponiblesPorAtraccionAsync(Guid atraccionGuid, CancellationToken cancellationToken)
    {
        return await ListarHorariosPorAtraccionAsync(atraccionGuid, onlyAvailable: true, cancellationToken);
    }

    public async Task<IReadOnlyList<HorarioResponse>> ListarHorariosPorAtraccionAsync(Guid atraccionGuid, bool onlyAvailable, CancellationToken cancellationToken)
    {
        var atraccion = await _context.Atracciones
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AtGuid == atraccionGuid && x.AtEstado == "A", cancellationToken);

        if (atraccion is null)
            throw new NotFoundException("No se encontro la atraccion.");

        var ticketIds = await _context.Tickets
            .AsNoTracking()
            .Where(x => x.AtId == atraccion.AtId && x.TckEstado == "A")
            .Select(x => x.TckId)
            .ToListAsync(cancellationToken);

        return await ListarHorariosAsync(ticketIds, onlyAvailable, cancellationToken);
    }

    public async Task<IReadOnlyList<TicketResponse>> ListarTicketsPorHorarioAsync(Guid atraccionGuid, Guid horarioGuid, CancellationToken cancellationToken)
    {
        var atraccion = await _context.Atracciones
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AtGuid == atraccionGuid && x.AtEstado == "A", cancellationToken);

        if (atraccion is null)
            throw new NotFoundException("No se encontro la atraccion.");

        var row = await _context.Horarios
            .AsNoTracking()
            .Where(x => x.HorGuid == horarioGuid && x.HorEstado == "A")
            .Join(
                _context.Tickets.AsNoTracking().Where(x => x.TckEstado == "A"),
                horario => horario.TckId,
                ticket => ticket.TckId,
                (horario, ticket) => new { Horario = horario, Ticket = ticket })
            .FirstOrDefaultAsync(x => x.Ticket.AtId == atraccion.AtId, cancellationToken);

        if (row is null)
            throw new NotFoundException("No se encontro el horario para la atraccion indicada.");

        var horariosMismoBloque = await _context.Horarios
            .AsNoTracking()
            .Where(x => x.HorEstado == "A" &&
                        x.HorFecha == row.Horario.HorFecha &&
                        x.HorHoraInicio == row.Horario.HorHoraInicio &&
                        x.HorHoraFin == row.Horario.HorHoraFin)
            .Join(
                _context.Tickets.AsNoTracking().Where(x => x.TckEstado == "A" && x.AtId == atraccion.AtId),
                horario => horario.TckId,
                ticket => ticket.TckId,
                (horario, ticket) => new { Horario = horario, Ticket = ticket })
            .OrderBy(x => x.Ticket.TckPrecio)
            .ToListAsync(cancellationToken);

        return horariosMismoBloque.Select(x =>
            new TicketResponse
            {
                Guid = x.Ticket.TckGuid,
                AtraccionGuid = atraccion.AtGuid,
                AtraccionNombre = atraccion.AtNombre,
                Titulo = x.Ticket.TckTitulo,
                Precio = x.Ticket.TckPrecio,
                TipoParticipante = x.Ticket.TckTipoParticipante,
                CapacidadMaxima = x.Ticket.TckCapacidadMaxima,
                CuposDisponibles = x.Horario.HorCuposDisponibles
            }).ToList();
    }

    public async Task<bool> DescontarCuposAsync(
        Guid atraccionGuid,
        Guid horarioGuid,
        MovimientoCuposRequest request,
        CancellationToken cancellationToken)
    {
        return await AplicarMovimientoCuposAsync(atraccionGuid, horarioGuid, request, descontar: true, cancellationToken);
    }

    public async Task<bool> LiberarCuposAsync(
        Guid atraccionGuid,
        Guid horarioGuid,
        MovimientoCuposRequest request,
        CancellationToken cancellationToken)
    {
        return await AplicarMovimientoCuposAsync(atraccionGuid, horarioGuid, request, descontar: false, cancellationToken);
    }

    private async Task<bool> AplicarMovimientoCuposAsync(
        Guid atraccionGuid,
        Guid horarioGuid,
        MovimientoCuposRequest request,
        bool descontar,
        CancellationToken cancellationToken)
    {
        ValidateMovimientoCupos(request);

        var atraccion = await _context.Atracciones
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AtGuid == atraccionGuid && x.AtEstado == "A", cancellationToken);

        if (atraccion is null)
            throw new NotFoundException("No se encontro la atraccion.");

        var horarioBase = await _context.Horarios
            .AsNoTracking()
            .Where(x => x.HorGuid == horarioGuid && x.HorEstado == "A")
            .Join(
                _context.Tickets.AsNoTracking().Where(x => x.AtId == atraccion.AtId && x.TckEstado == "A"),
                horario => horario.TckId,
                ticket => ticket.TckId,
                (horario, ticket) => horario)
            .FirstOrDefaultAsync(cancellationToken);

        if (horarioBase is null)
            throw new NotFoundException("No se encontro el horario para la atraccion indicada.");

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var now = DateTimeOffset.UtcNow;

        var detalles = request.Detalles ?? Array.Empty<MovimientoCuposDetalleRequest>();
        foreach (var detalle in detalles
            .GroupBy(x => x.TicketGuid)
            .Select(x => new MovimientoCuposDetalleRequest
            {
                TicketGuid = x.Key,
                Cantidad = x.Sum(y => y.Cantidad)
            }))
        {
            var ticket = await _context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.TckGuid == detalle.TicketGuid &&
                         x.AtId == atraccion.AtId &&
                         x.TckEstado == "A",
                    cancellationToken);

            if (ticket is null)
                throw new ValidationException($"El ticket {detalle.TicketGuid} no pertenece a la atraccion indicada.");

            var horario = await _context.Horarios
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.TckId == ticket.TckId &&
                         x.HorEstado == "A" &&
                         x.HorFecha == horarioBase.HorFecha &&
                         x.HorHoraInicio == horarioBase.HorHoraInicio &&
                         x.HorHoraFin == horarioBase.HorHoraFin,
                    cancellationToken);

            if (horario is null)
                throw new ValidationException($"No existe horario activo para el ticket {detalle.TicketGuid} en el bloque seleccionado.");

            if (descontar)
            {
                var affected = await _context.Horarios
                    .Where(x => x.HorId == horario.HorId && x.HorCuposDisponibles >= detalle.Cantidad)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.HorCuposDisponibles, x => x.HorCuposDisponibles - detalle.Cantidad)
                        .SetProperty(x => x.HorFechaMod, now)
                        .SetProperty(x => x.HorUsuarioMod, "booking")
                        .SetProperty(x => x.HorIpMod, "127.0.0.1"), cancellationToken);

                if (affected == 0)
                    throw new ValidationException($"No hay cupos suficientes para el ticket {detalle.TicketGuid}.");
            }
            else
            {
                await _context.Horarios
                    .Where(x => x.HorId == horario.HorId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.HorCuposDisponibles, x => x.HorCuposDisponibles + detalle.Cantidad)
                        .SetProperty(x => x.HorFechaMod, now)
                        .SetProperty(x => x.HorUsuarioMod, "booking")
                        .SetProperty(x => x.HorIpMod, "127.0.0.1"), cancellationToken);
            }
        }

        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<HorarioResponse>> ListarHorariosPorTicketAsync(Guid ticketGuid, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TckGuid == ticketGuid && x.TckEstado == "A", cancellationToken);

        if (ticket is null)
            throw new NotFoundException("No se encontro el ticket.");

        return await ListarHorariosAsync(new[] { ticket.TckId }, onlyAvailable: false, cancellationToken);
    }

    public async Task<IReadOnlyList<HorarioResponse>> ListarHorariosAsync(CancellationToken cancellationToken)
    {
        var ticketIds = await _context.Tickets
            .AsNoTracking()
            .Where(x => x.TckEstado == "A")
            .Select(x => x.TckId)
            .ToListAsync(cancellationToken);

        return await ListarHorariosAsync(ticketIds, onlyAvailable: false, cancellationToken);
    }

    public async Task<FiltrosAtraccionesResponse> ObtenerFiltrosAsync(CancellationToken cancellationToken)
    {
        var destinos = await _context.Destinos
            .AsNoTracking()
            .Where(x => x.DesEstado == "A")
            .OrderBy(x => x.DesNombre)
            .Select(x => new CatalogoItemResponse { Guid = x.DesGuid, Nombre = x.DesNombre, Extra = x.DesPais })
            .ToListAsync(cancellationToken);

        var categorias = await _context.Categorias
            .AsNoTracking()
            .Where(x => x.CatEstado == "A")
            .OrderBy(x => x.CatNombre)
            .Select(x => new CatalogoItemResponse { Guid = x.CatGuid, Nombre = x.CatNombre })
            .ToListAsync(cancellationToken);

        var idiomas = await _context.Idiomas
            .AsNoTracking()
            .Where(x => x.IdiEstado == "A")
            .OrderBy(x => x.IdiDescripcion)
            .Select(x => new CatalogoItemResponse { Guid = x.IdiGuid, Nombre = x.IdiDescripcion })
            .ToListAsync(cancellationToken);

        var precios = await _context.Atracciones
            .AsNoTracking()
            .Where(x => x.AtEstado == "A" && x.AtDisponible && x.AtPrecioReferencia != null)
            .Select(x => x.AtPrecioReferencia!.Value)
            .ToListAsync(cancellationToken);

        return new FiltrosAtraccionesResponse
        {
            Destinos = destinos,
            Categorias = categorias,
            Idiomas = idiomas,
            PrecioMinimo = precios.Count == 0 ? null : precios.Min(),
            PrecioMaximo = precios.Count == 0 ? null : precios.Max()
        };
    }

    public async Task<int> CrearAtraccionAsync(CrearAtraccionRequest request, CancellationToken cancellationToken)
    {
        ValidateAtraccion(request);

        var destinoExists = await _context.Destinos.AnyAsync(
            x => x.DesGuid == request.DestinoGuid && x.DesEstado == "A",
            cancellationToken);

        if (!destinoExists)
            throw new ValidationException("El destino indicado no existe.");

        var entity = new AtraccionEntity
        {
            AtGuid = Guid.NewGuid(),
            DesGuid = request.DestinoGuid,
            AtNombre = request.Nombre.Trim(),
            AtDescripcion = request.Descripcion,
            AtTotalResenias = 0,
            AtDireccion = request.Direccion,
            AtDuracionMinutos = request.DuracionMinutos,
            AtPuntoEncuentro = request.PuntoEncuentro,
            AtPrecioReferencia = request.PrecioReferencia,
            AtIncluyeAcompaniante = request.IncluyeAcompaniante,
            AtIncluyeTransporte = request.IncluyeTransporte,
            AtDisponible = true,
            AtFechaIngreso = DateTimeOffset.UtcNow,
            AtUsuarioIngreso = "api",
            AtIpIngreso = "127.0.0.1",
            AtEstado = "A"
        };

        await _context.Atracciones.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.AtId;
    }

    public async Task<bool> ActualizarAtraccionAsync(Guid guid, ActualizarAtraccionRequest request, CancellationToken cancellationToken)
    {
        ValidateAtraccion(request);

        var entity = await _context.Atracciones.FirstOrDefaultAsync(
            x => x.AtGuid == guid && x.AtEstado == "A",
            cancellationToken);

        if (entity is null)
            throw new NotFoundException("No se encontro la atraccion.");

        entity.DesGuid = request.DestinoGuid;
        entity.AtNombre = request.Nombre.Trim();
        entity.AtDescripcion = request.Descripcion;
        entity.AtDireccion = request.Direccion;
        entity.AtDuracionMinutos = request.DuracionMinutos;
        entity.AtPuntoEncuentro = request.PuntoEncuentro;
        entity.AtPrecioReferencia = request.PrecioReferencia;
        entity.AtIncluyeAcompaniante = request.IncluyeAcompaniante;
        entity.AtIncluyeTransporte = request.IncluyeTransporte;
        entity.AtDisponible = request.Disponible;
        entity.AtFechaMod = DateTimeOffset.UtcNow;
        entity.AtUsuarioMod = "api";
        entity.AtIpMod = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EliminarAtraccionAsync(Guid guid, CancellationToken cancellationToken)
    {
        var entity = await _context.Atracciones.FirstOrDefaultAsync(
            x => x.AtGuid == guid && x.AtEstado == "A",
            cancellationToken);

        if (entity is null)
            throw new NotFoundException("No se encontro la atraccion.");

        entity.AtEstado = "I";
        entity.AtDisponible = false;
        entity.AtFechaEliminacion = DateTimeOffset.UtcNow;
        entity.AtUsuarioEliminacion = "api";
        entity.AtIpEliminacion = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> CrearTicketAsync(CrearTicketRequest request, CancellationToken cancellationToken)
    {
        ValidateTicket(request);

        var atraccion = await _context.Atracciones.FirstOrDefaultAsync(
            x => x.AtGuid == request.AtraccionGuid && x.AtEstado == "A",
            cancellationToken);

        if (atraccion is null)
            throw new ValidationException("La atraccion indicada no existe.");

        var ticket = new TicketEntity
        {
            TckGuid = Guid.NewGuid(),
            AtId = atraccion.AtId,
            TckTitulo = request.Titulo.Trim(),
            TckPrecio = request.Precio,
            TckTipoParticipante = NormalizeTipoParticipante(request.TipoParticipante),
            TckCapacidadMaxima = request.CapacidadMaxima,
            TckCuposDisponibles = request.CuposDisponibles,
            TckFechaIngreso = DateTimeOffset.UtcNow,
            TckUsuarioIngreso = "api",
            TckIpIngreso = "127.0.0.1",
            TckEstado = "A"
        };

        await _context.Tickets.AddAsync(ticket, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ticket.TckId;
    }

    public async Task<bool> ActualizarTicketAsync(Guid guid, ActualizarTicketRequest request, CancellationToken cancellationToken)
    {
        ValidateTicket(request);

        var ticket = await _context.Tickets.FirstOrDefaultAsync(
            x => x.TckGuid == guid && x.TckEstado == "A",
            cancellationToken);

        if (ticket is null)
            throw new NotFoundException("No se encontro el ticket.");

        var atraccion = await _context.Atracciones.FirstOrDefaultAsync(
            x => x.AtGuid == request.AtraccionGuid && x.AtEstado == "A",
            cancellationToken);

        if (atraccion is null)
            throw new ValidationException("La atraccion indicada no existe.");

        ticket.AtId = atraccion.AtId;
        ticket.TckTitulo = request.Titulo.Trim();
        ticket.TckPrecio = request.Precio;
        ticket.TckTipoParticipante = NormalizeTipoParticipante(request.TipoParticipante);
        ticket.TckCapacidadMaxima = request.CapacidadMaxima;
        ticket.TckCuposDisponibles = request.CuposDisponibles;
        ticket.TckFechaMod = DateTimeOffset.UtcNow;
        ticket.TckUsuarioMod = "api";
        ticket.TckIpMod = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EliminarTicketAsync(Guid guid, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets
            .Include(x => x.Horarios.Where(h => h.HorEstado == "A"))
            .FirstOrDefaultAsync(x => x.TckGuid == guid && x.TckEstado == "A", cancellationToken);

        if (ticket is null)
            throw new NotFoundException("No se encontro el ticket.");

        ticket.TckEstado = "I";
        ticket.TckFechaEliminacion = DateTimeOffset.UtcNow;
        ticket.TckUsuarioEliminacion = "api";
        ticket.TckIpEliminacion = "127.0.0.1";

        foreach (var horario in ticket.Horarios)
        {
            horario.HorEstado = "I";
            horario.HorFechaEliminacion = DateTimeOffset.UtcNow;
            horario.HorUsuarioEliminacion = "api";
            horario.HorIpEliminacion = "127.0.0.1";
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> CrearHorarioAsync(CrearHorarioRequest request, CancellationToken cancellationToken)
    {
        ValidateHorario(request);

        var ticket = await _context.Tickets.FirstOrDefaultAsync(
            x => x.TckGuid == request.TicketGuid && x.TckEstado == "A",
            cancellationToken);

        if (ticket is null)
            throw new ValidationException("El ticket indicado no existe.");

        var horario = new HorarioEntity
        {
            HorGuid = Guid.NewGuid(),
            TckId = ticket.TckId,
            HorFecha = request.Fecha,
            HorHoraInicio = request.HoraInicio,
            HorHoraFin = request.HoraFin,
            HorCuposDisponibles = request.CuposDisponibles,
            HorFechaIngreso = DateTimeOffset.UtcNow,
            HorUsuarioIngreso = "api",
            HorIpIngreso = "127.0.0.1",
            HorEstado = "A"
        };

        await _context.Horarios.AddAsync(horario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return horario.HorId;
    }

    public async Task<bool> ActualizarHorarioAsync(Guid guid, ActualizarHorarioRequest request, CancellationToken cancellationToken)
    {
        ValidateHorario(request);

        var horario = await _context.Horarios.FirstOrDefaultAsync(
            x => x.HorGuid == guid && x.HorEstado == "A",
            cancellationToken);

        if (horario is null)
            throw new NotFoundException("No se encontro el horario.");

        var ticket = await _context.Tickets.FirstOrDefaultAsync(
            x => x.TckGuid == request.TicketGuid && x.TckEstado == "A",
            cancellationToken);

        if (ticket is null)
            throw new ValidationException("El ticket indicado no existe.");

        horario.TckId = ticket.TckId;
        horario.HorFecha = request.Fecha;
        horario.HorHoraInicio = request.HoraInicio;
        horario.HorHoraFin = request.HoraFin;
        horario.HorCuposDisponibles = request.CuposDisponibles;
        horario.HorFechaMod = DateTimeOffset.UtcNow;
        horario.HorUsuarioMod = "api";
        horario.HorIpMod = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EliminarHorarioAsync(Guid guid, CancellationToken cancellationToken)
    {
        var horario = await _context.Horarios.FirstOrDefaultAsync(
            x => x.HorGuid == guid && x.HorEstado == "A",
            cancellationToken);

        if (horario is null)
            throw new NotFoundException("No se encontro el horario.");

        horario.HorEstado = "I";
        horario.HorFechaEliminacion = DateTimeOffset.UtcNow;
        horario.HorUsuarioEliminacion = "api";
        horario.HorIpEliminacion = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<ReseniaResponse>> ListarReseniasAsync(Guid? atraccionGuid, CancellationToken cancellationToken)
    {
        var query = _context.Resenias.AsNoTracking().Where(x => x.RsnEstado == "A");

        if (atraccionGuid is not null)
        {
            var atraccion = await _context.Atracciones
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AtGuid == atraccionGuid.Value && x.AtEstado == "A", cancellationToken);

            if (atraccion is null)
                throw new NotFoundException("No se encontro la atraccion.");

            query = query.Where(x => x.AtId == atraccion.AtId);
        }

        var resenias = await query
            .OrderByDescending(x => x.RsnFechaCreacion)
            .ToListAsync(cancellationToken);

        var atraccionIds = resenias.Select(x => x.AtId).Distinct().ToList();
        var atracciones = await _context.Atracciones
            .AsNoTracking()
            .Where(x => atraccionIds.Contains(x.AtId))
            .ToDictionaryAsync(x => x.AtId, x => x.AtGuid, cancellationToken);

        return resenias.Select(x => new ReseniaResponse
        {
            Guid = x.RsnGuid,
            AtraccionGuid = atracciones.GetValueOrDefault(x.AtId),
            ReservaGuid = x.RevGuid,
            Comentario = x.RsnComentario,
            Rating = x.RsnRating,
            FechaCreacion = x.RsnFechaCreacion
        }).ToList();
    }

    public async Task<int> CrearReseniaAsync(CrearReseniaRequest request, CancellationToken cancellationToken)
    {
        if (request.AtraccionGuid == Guid.Empty)
            throw new ValidationException("El atraccionGuid es obligatorio.");

        if (request.ReservaGuid == Guid.Empty)
            throw new ValidationException("El reservaGuid es obligatorio.");

        if (request.Rating is < 1 or > 5)
            throw new ValidationException("El rating debe estar entre 1 y 5.");

        var atraccion = await _context.Atracciones.FirstOrDefaultAsync(
            x => x.AtGuid == request.AtraccionGuid && x.AtEstado == "A",
            cancellationToken);

        if (atraccion is null)
            throw new NotFoundException("No se encontro la atraccion.");

        var exists = await _context.Resenias.AnyAsync(
            x => x.RevGuid == request.ReservaGuid && x.RsnEstado == "A",
            cancellationToken);

        if (exists)
            throw new ValidationException("Ya existe una resena para esa reserva.");

        var resenia = new ReseniaEntity
        {
            RsnGuid = Guid.NewGuid(),
            AtId = atraccion.AtId,
            RevGuid = request.ReservaGuid,
            RsnComentario = request.Comentario,
            RsnRating = request.Rating,
            RsnFechaCreacion = DateTimeOffset.UtcNow,
            RsnUsuarioCreacion = string.IsNullOrWhiteSpace(request.UsuarioCreacion)
                ? "booking-public"
                : request.UsuarioCreacion.Trim(),
            RsnIpCreacion = "127.0.0.1",
            RsnEstado = "A"
        };

        atraccion.AtTotalResenias++;

        await _context.Resenias.AddAsync(resenia, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return resenia.RsnId;
    }

    public async Task<bool> ActualizarReseniaAsync(Guid guid, ActualizarReseniaRequest request, CancellationToken cancellationToken)
    {
        if (request.AtraccionGuid == Guid.Empty)
            throw new ValidationException("El atraccionGuid es obligatorio.");

        if (request.ReservaGuid == Guid.Empty)
            throw new ValidationException("El reservaGuid es obligatorio.");

        if (request.Rating is < 1 or > 5)
            throw new ValidationException("El rating debe estar entre 1 y 5.");

        var resenia = await _context.Resenias.FirstOrDefaultAsync(
            x => x.RsnGuid == guid && x.RsnEstado == "A",
            cancellationToken);

        if (resenia is null)
            throw new NotFoundException("No se encontro la resena.");

        var atraccion = await _context.Atracciones.FirstOrDefaultAsync(
            x => x.AtGuid == request.AtraccionGuid && x.AtEstado == "A",
            cancellationToken);

        if (atraccion is null)
            throw new NotFoundException("No se encontro la atraccion.");

        var exists = await _context.Resenias.AnyAsync(
            x => x.RsnGuid != guid && x.RevGuid == request.ReservaGuid && x.RsnEstado == "A",
            cancellationToken);

        if (exists)
            throw new ValidationException("Ya existe una resena para esa reserva.");

        if (resenia.AtId != atraccion.AtId)
        {
            var previousAtraccion = await _context.Atracciones.FirstOrDefaultAsync(
                x => x.AtId == resenia.AtId,
                cancellationToken);

            if (previousAtraccion is not null && previousAtraccion.AtTotalResenias > 0)
                previousAtraccion.AtTotalResenias--;

            atraccion.AtTotalResenias++;
            resenia.AtId = atraccion.AtId;
        }

        resenia.RevGuid = request.ReservaGuid;
        resenia.RsnComentario = request.Comentario;
        resenia.RsnRating = request.Rating;
        resenia.RsnFechaMod = DateTimeOffset.UtcNow;
        resenia.RsnUsuarioMod = "api";
        resenia.RsnIpMod = "127.0.0.1";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EliminarReseniaAsync(Guid guid, CancellationToken cancellationToken)
    {
        var resenia = await _context.Resenias.FirstOrDefaultAsync(
            x => x.RsnGuid == guid && x.RsnEstado == "A",
            cancellationToken);

        if (resenia is null)
            throw new NotFoundException("No se encontro la resena.");

        resenia.RsnEstado = "I";
        resenia.RsnFechaEliminacion = DateTimeOffset.UtcNow;
        resenia.RsnUsuarioEliminacion = "api";
        resenia.RsnIpEliminacion = "127.0.0.1";

        var atraccion = await _context.Atracciones.FirstOrDefaultAsync(x => x.AtId == resenia.AtId, cancellationToken);
        if (atraccion is not null && atraccion.AtTotalResenias > 0)
            atraccion.AtTotalResenias--;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<IReadOnlyList<HorarioResponse>> ListarHorariosAsync(
        IReadOnlyCollection<int> ticketIds,
        bool onlyAvailable,
        CancellationToken cancellationToken)
    {
        if (ticketIds.Count == 0)
            return Array.Empty<HorarioResponse>();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var query = _context.Horarios
            .AsNoTracking()
            .Where(x => ticketIds.Contains(x.TckId) && x.HorEstado == "A");

        if (onlyAvailable)
            query = query.Where(x => x.HorCuposDisponibles > 0 && x.HorFecha >= today);

        var horarios = await query
            .OrderBy(x => x.HorFecha)
            .ThenBy(x => x.HorHoraInicio)
            .ToListAsync(cancellationToken);

        var tickets = await _context.Tickets
            .AsNoTracking()
            .Where(x => ticketIds.Contains(x.TckId))
            .ToDictionaryAsync(x => x.TckId, cancellationToken);

        return horarios.Select(x =>
        {
            var ticket = tickets[x.TckId];
            return new HorarioResponse
            {
                Guid = x.HorGuid,
                TicketGuid = ticket.TckGuid,
                TicketTitulo = ticket.TckTitulo,
                Fecha = x.HorFecha,
                HoraInicio = x.HorHoraInicio,
                HoraFin = x.HorHoraFin,
                CuposDisponibles = x.HorCuposDisponibles
            };
        }).ToList();
    }

    private async Task<IReadOnlyList<AtraccionResponse>> MapAtraccionesAsync(
        IReadOnlyCollection<AtraccionEntity> atracciones,
        CancellationToken cancellationToken)
    {
        var destinoGuids = atracciones.Select(x => x.DesGuid).Distinct().ToList();
        var destinos = await _context.Destinos
            .AsNoTracking()
            .Where(x => destinoGuids.Contains(x.DesGuid))
            .ToDictionaryAsync(x => x.DesGuid, cancellationToken);

        var atraccionIds = atracciones.Select(x => x.AtId).Distinct().ToList();
        var imagenesPorAtraccion = await _context.ImagenesAtraccion
            .AsNoTracking()
            .Where(x => atraccionIds.Contains(x.AtId) && x.ImaEstado == "A")
            .Join(
                _context.Imagenes.AsNoTracking().Where(x => x.ImgEstado == "A"),
                ia => ia.ImgGuid,
                img => img.ImgGuid,
                (ia, img) => new { ia.AtId, img.ImgUrl })
            .GroupBy(x => x.AtId)
            .Select(x => new { AtId = x.Key, ImagenUrl = x.Select(i => i.ImgUrl).FirstOrDefault() })
            .ToDictionaryAsync(x => x.AtId, x => x.ImagenUrl, cancellationToken);

        return atracciones.Select(x =>
        {
            destinos.TryGetValue(x.DesGuid, out var destino);
            imagenesPorAtraccion.TryGetValue(x.AtId, out var imagenUrl);
            return ToAtraccionResponse(x, destino, imagenUrl);
        }).ToList();
    }

    private async Task<IReadOnlyCollection<CatalogoItemResponse>> ObtenerCategoriasAsync(int atraccionId, CancellationToken cancellationToken)
    {
        var guids = await _context.CategoriaAtracciones
            .AsNoTracking()
            .Where(x => x.AtId == atraccionId && x.CaEstado == "A")
            .Select(x => x.CatGuid)
            .ToListAsync(cancellationToken);

        return await _context.Categorias
            .AsNoTracking()
            .Where(x => guids.Contains(x.CatGuid) && x.CatEstado == "A")
            .Select(x => new CatalogoItemResponse { Guid = x.CatGuid, Nombre = x.CatNombre })
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyCollection<CatalogoItemResponse>> ObtenerIdiomasAsync(int atraccionId, CancellationToken cancellationToken)
    {
        var guids = await _context.IdiomasAtraccion
            .AsNoTracking()
            .Where(x => x.AtId == atraccionId && x.IaEstado == "A")
            .Select(x => x.IdiGuid)
            .ToListAsync(cancellationToken);

        return await _context.Idiomas
            .AsNoTracking()
            .Where(x => guids.Contains(x.IdiGuid) && x.IdiEstado == "A")
            .Select(x => new CatalogoItemResponse { Guid = x.IdiGuid, Nombre = x.IdiDescripcion })
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyCollection<CatalogoItemResponse>> ObtenerIncluyeAsync(int atraccionId, CancellationToken cancellationToken)
    {
        var guids = await _context.AtraccionesIncluye
            .AsNoTracking()
            .Where(x => x.AtId == atraccionId && x.AiEstado == "A")
            .Select(x => x.IncGuid)
            .ToListAsync(cancellationToken);

        return await _context.Incluyes
            .AsNoTracking()
            .Where(x => guids.Contains(x.IncGuid) && x.IncEstado == "A")
            .Select(x => new CatalogoItemResponse { Guid = x.IncGuid, Nombre = x.IncDescripcion })
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyCollection<string>> ObtenerImagenesAsync(int atraccionId, CancellationToken cancellationToken)
    {
        var guids = await _context.ImagenesAtraccion
            .AsNoTracking()
            .Where(x => x.AtId == atraccionId && x.ImaEstado == "A")
            .Select(x => x.ImgGuid)
            .ToListAsync(cancellationToken);

        return await _context.Imagenes
            .AsNoTracking()
            .Where(x => guids.Contains(x.ImgGuid) && x.ImgEstado == "A")
            .Select(x => x.ImgUrl)
            .ToListAsync(cancellationToken);
    }

    private static AtraccionResponse ToAtraccionResponse(AtraccionEntity entity, DestinoEntity? destino, string? imagenUrl)
    {
        return new AtraccionResponse
        {
            Guid = entity.AtGuid,
            Nombre = entity.AtNombre,
            Descripcion = entity.AtDescripcion,
            PrecioReferencia = entity.AtPrecioReferencia,
            DestinoGuid = entity.DesGuid,
            DestinoNombre = destino?.DesNombre,
            DestinoPais = destino?.DesPais,
            ImagenUrl = imagenUrl,
            DuracionMinutos = entity.AtDuracionMinutos,
            Direccion = entity.AtDireccion,
            PuntoEncuentro = entity.AtPuntoEncuentro,
            IncluyeTransporte = entity.AtIncluyeTransporte,
            IncluyeAcompaniante = entity.AtIncluyeAcompaniante,
            Disponible = entity.AtDisponible,
            TotalResenias = entity.AtTotalResenias
        };
    }

    private static TicketResponse ToTicketResponse(TicketEntity entity, Guid atraccionGuid, string? atraccionNombre = null)
    {
        return new TicketResponse
        {
            Guid = entity.TckGuid,
            AtraccionGuid = atraccionGuid,
            AtraccionNombre = atraccionNombre,
            Titulo = entity.TckTitulo,
            Precio = entity.TckPrecio,
            TipoParticipante = entity.TckTipoParticipante,
            CapacidadMaxima = entity.TckCapacidadMaxima,
            CuposDisponibles = entity.TckCuposDisponibles
        };
    }

    private static void ValidateAtraccion(CrearAtraccionRequest request)
    {
        var errors = new List<string>();

        if (request.DestinoGuid == Guid.Empty)
            errors.Add("El destinoGuid es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Nombre))
            errors.Add("El nombre de la atraccion es obligatorio.");

        if (request.DuracionMinutos is <= 0)
            errors.Add("La duracion debe ser mayor a cero.");

        if (request.PrecioReferencia is < 0)
            errors.Add("El precio de referencia no puede ser negativo.");

        if (errors.Count > 0)
            throw new ValidationException("Error de validacion.", errors);
    }

    private static void ValidateTicket(CrearTicketRequest request)
    {
        var errors = new List<string>();

        if (request.AtraccionGuid == Guid.Empty)
            errors.Add("El atraccionGuid es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Titulo))
            errors.Add("El titulo del ticket es obligatorio.");

        if (request.Precio < 0)
            errors.Add("El precio no puede ser negativo.");

        if (request.CapacidadMaxima <= 0)
            errors.Add("La capacidad maxima debe ser mayor a cero.");

        if (request.CuposDisponibles < 0 || request.CuposDisponibles > request.CapacidadMaxima)
            errors.Add("Los cupos disponibles deben estar entre cero y la capacidad maxima.");

        if (!TiposParticipante.Contains(NormalizeTipoParticipante(request.TipoParticipante)))
            errors.Add("El tipo de participante no es valido.");

        if (errors.Count > 0)
            throw new ValidationException("Error de validacion.", errors);
    }

    private static void ValidateHorario(CrearHorarioRequest request)
    {
        if (request.TicketGuid == Guid.Empty)
            throw new ValidationException("El ticketGuid es obligatorio.");

        if (request.CuposDisponibles < 0)
            throw new ValidationException("Los cupos disponibles no pueden ser negativos.");

        if (request.HoraFin is not null && request.HoraFin <= request.HoraInicio)
            throw new ValidationException("La hora fin debe ser mayor a la hora inicio.");
    }

    private static void ValidateMovimientoCupos(MovimientoCuposRequest request)
    {
        var errors = new List<string>();
        var detalles = request.Detalles ?? Array.Empty<MovimientoCuposDetalleRequest>();

        if (detalles.Count == 0)
            errors.Add("Debe enviar al menos una linea para mover cupos.");

        foreach (var detalle in detalles)
        {
            if (detalle.TicketGuid == Guid.Empty)
                errors.Add("Cada linea debe tener ticketGuid o tck_guid.");

            if (detalle.Cantidad <= 0)
                errors.Add("La cantidad debe ser mayor a cero.");
        }

        if (errors.Count > 0)
            throw new ValidationException("Error de validacion.", errors);
    }

    private static string NormalizeTipoParticipante(string value)
    {
        return value.Trim().Equals("Nino", StringComparison.OrdinalIgnoreCase) ? "Niño" : value.Trim();
    }
}
