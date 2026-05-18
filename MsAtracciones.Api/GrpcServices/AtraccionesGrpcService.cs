using System.Text.Json;
using Atracciones.Grpc;
using Atracciones.Shared.Models;
using Grpc.Core;
using MsAtracciones.Api.Dtos;
using MsAtracciones.Api.Services;

namespace MsAtracciones.Api.GrpcServices;

public class AtraccionesGrpcService : AtraccionesGrpc.AtraccionesGrpcBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly AtraccionesService _atraccionesService;

    public AtraccionesGrpcService(AtraccionesService atraccionesService)
    {
        _atraccionesService = atraccionesService;
    }

    public override async Task<JsonReply> ListarAtracciones(AtraccionesListRequest request, ServerCallContext context)
    {
        var destinoGuid = Guid.TryParse(request.DestinoGuid, out var des) ? des : (Guid?)null;
        var categoriaGuid = Guid.TryParse(request.CategoriaGuid, out var cat) ? cat : (Guid?)null;
        var result = await _atraccionesService.ListarAtraccionesAsync(
            string.IsNullOrWhiteSpace(request.Nombre) ? null : request.Nombre,
            destinoGuid,
            categoriaGuid,
            request.Page > 0 ? request.Page : null,
            request.PageSize > 0 ? request.PageSize : null,
            context.CancellationToken);

        return Ok(ApiResponse<IReadOnlyList<AtraccionResponse>>.Ok(result, "Consulta exitosa."));
    }

    public override async Task<JsonReply> ObtenerDetalle(GuidRequest request, ServerCallContext context)
    {
        var result = await _atraccionesService.ObtenerDetalleAsync(Guid.Parse(request.Guid), context.CancellationToken);
        return Ok(ApiResponse<AtraccionDetalleResponse>.Ok(result, "Consulta exitosa."));
    }

    public override async Task<JsonReply> ObtenerFiltros(EmptyRequest request, ServerCallContext context)
    {
        var result = await _atraccionesService.ObtenerFiltrosAsync(context.CancellationToken);
        return Ok(ApiResponse<FiltrosAtraccionesResponse>.Ok(result, "Consulta exitosa."));
    }

    public override async Task<JsonReply> ObtenerTickets(GuidRequest request, ServerCallContext context)
    {
        var result = await _atraccionesService.ListarTicketsPorAtraccionAsync(Guid.Parse(request.Guid), context.CancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TicketResponse>>.Ok(result, "Consulta exitosa."));
    }

    public override async Task<JsonReply> ObtenerHorariosAtraccion(HorariosAtraccionRequest request, ServerCallContext context)
    {
        var result = await _atraccionesService.ListarHorariosPorAtraccionAsync(
            Guid.Parse(request.AtraccionGuid),
            request.Disponibles,
            context.CancellationToken);
        return Ok(ApiResponse<IReadOnlyList<HorarioResponse>>.Ok(result, "Consulta exitosa."));
    }

    public override async Task<JsonReply> ObtenerTicketsPorHorario(HorarioTicketsRequest request, ServerCallContext context)
    {
        var result = await _atraccionesService.ListarTicketsPorHorarioAsync(
            Guid.Parse(request.AtraccionGuid),
            Guid.Parse(request.HorarioGuid),
            context.CancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TicketResponse>>.Ok(result, "Consulta exitosa."));
    }

    public override async Task<JsonReply> ObtenerHorariosPorTicket(GuidRequest request, ServerCallContext context)
    {
        var result = await _atraccionesService.ListarHorariosPorTicketAsync(Guid.Parse(request.Guid), context.CancellationToken);
        return Ok(ApiResponse<IReadOnlyList<HorarioResponse>>.Ok(result, "Consulta exitosa."));
    }

    public override async Task<JsonReply> ListarResenias(GuidRequest request, ServerCallContext context)
    {
        var result = await _atraccionesService.ListarReseniasAsync(Guid.Parse(request.Guid), context.CancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ReseniaResponse>>.Ok(result, "Consulta exitosa."));
    }

    public override async Task<JsonReply> CrearResenia(JsonRequest request, ServerCallContext context)
    {
        var body = JsonSerializer.Deserialize<CrearReseniaRequest>(request.Json, JsonOptions) ?? new CrearReseniaRequest();
        var result = await _atraccionesService.CrearReseniaAsync(body, context.CancellationToken);
        return Ok(ApiResponse<int>.Ok(result, "Resenia creada correctamente."));
    }

    private static JsonReply Ok<T>(T value)
    {
        return new JsonReply
        {
            StatusCode = StatusCodes.Status200OK,
            Json = JsonSerializer.Serialize(value, JsonOptions)
        };
    }
}
