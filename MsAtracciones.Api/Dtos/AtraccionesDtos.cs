namespace MsAtracciones.Api.Dtos;

public class CatalogoItemResponse
{
    public Guid Guid { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Extra { get; set; }
    public int? ParentId { get; set; }
    public string? ImagenUrl { get; set; }
}

public class ImagenResponse
{
    public Guid Guid { get; set; }
    public string Url { get; set; } = null!;
    public string? Descripcion { get; set; }
}

public class AtraccionResponse
{
    public Guid Guid { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal? PrecioReferencia { get; set; }
    public Guid DestinoGuid { get; set; }
    public string? DestinoNombre { get; set; }
    public string? DestinoPais { get; set; }
    public string? ImagenUrl { get; set; }
    public int? DuracionMinutos { get; set; }
    public string? Direccion { get; set; }
    public bool Disponible { get; set; }
    public int TotalResenias { get; set; }
}

public class AtraccionDetalleResponse : AtraccionResponse
{
    public IReadOnlyCollection<CatalogoItemResponse> Categorias { get; set; } = Array.Empty<CatalogoItemResponse>();
    public IReadOnlyCollection<CatalogoItemResponse> Idiomas { get; set; } = Array.Empty<CatalogoItemResponse>();
    public IReadOnlyCollection<CatalogoItemResponse> Incluye { get; set; } = Array.Empty<CatalogoItemResponse>();
    public IReadOnlyCollection<string> Imagenes { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<TicketResponse> Tickets { get; set; } = Array.Empty<TicketResponse>();
}

public class TicketResponse
{
    public Guid Guid { get; set; }
    public Guid AtraccionGuid { get; set; }
    public string? AtraccionNombre { get; set; }
    public string Titulo { get; set; } = null!;
    public decimal Precio { get; set; }
    public string TipoParticipante { get; set; } = null!;
    public int CapacidadMaxima { get; set; }
    public int CuposDisponibles { get; set; }
}

public class HorarioResponse
{
    public Guid Guid { get; set; }
    public Guid TicketGuid { get; set; }
    public string TicketTitulo { get; set; } = null!;
    public DateOnly Fecha { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public int CuposDisponibles { get; set; }
}

public class FiltrosAtraccionesResponse
{
    public IReadOnlyCollection<CatalogoItemResponse> Destinos { get; set; } = Array.Empty<CatalogoItemResponse>();
    public IReadOnlyCollection<CatalogoItemResponse> Categorias { get; set; } = Array.Empty<CatalogoItemResponse>();
    public IReadOnlyCollection<CatalogoItemResponse> Idiomas { get; set; } = Array.Empty<CatalogoItemResponse>();
    public decimal? PrecioMinimo { get; set; }
    public decimal? PrecioMaximo { get; set; }
}

public class CrearAtraccionRequest
{
    public Guid DestinoGuid { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Direccion { get; set; }
    public int? DuracionMinutos { get; set; }
    public string? PuntoEncuentro { get; set; }
    public decimal? PrecioReferencia { get; set; }
    public bool IncluyeAcompaniante { get; set; }
    public bool IncluyeTransporte { get; set; }
}

public class ActualizarAtraccionRequest : CrearAtraccionRequest
{
    public bool Disponible { get; set; } = true;
}

public class CrearTicketRequest
{
    public Guid AtraccionGuid { get; set; }
    public string Titulo { get; set; } = null!;
    public decimal Precio { get; set; }
    public string TipoParticipante { get; set; } = "Adulto";
    public int CapacidadMaxima { get; set; }
    public int CuposDisponibles { get; set; }
}

public class ActualizarTicketRequest : CrearTicketRequest
{
}

public class CrearHorarioRequest
{
    public Guid TicketGuid { get; set; }
    public DateOnly Fecha { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public int CuposDisponibles { get; set; }
}

public class ActualizarHorarioRequest : CrearHorarioRequest
{
}

public class ReseniaResponse
{
    public Guid Guid { get; set; }
    public Guid AtraccionGuid { get; set; }
    public Guid ReservaGuid { get; set; }
    public string? Comentario { get; set; }
    public short Rating { get; set; }
    public DateTimeOffset FechaCreacion { get; set; }
}

public class CrearReseniaRequest
{
    public Guid AtraccionGuid { get; set; }
    public Guid ReservaGuid { get; set; }
    public string? Comentario { get; set; }
    public short Rating { get; set; }
}

public class ActualizarReseniaRequest : CrearReseniaRequest
{
}

public class CrearDestinoRequest
{
    public string Nombre { get; set; } = null!;
    public string Pais { get; set; } = null!;
    public string? ImagenUrl { get; set; }
}

public class ActualizarDestinoRequest : CrearDestinoRequest
{
}

public class CrearCategoriaRequest
{
    public string Nombre { get; set; } = null!;
    public int? ParentId { get; set; }
}

public class ActualizarCategoriaRequest : CrearCategoriaRequest
{
}

public class CrearIdiomaRequest
{
    public string Nombre { get; set; } = null!;
}

public class ActualizarIdiomaRequest : CrearIdiomaRequest
{
}

public class CrearIncluyeRequest
{
    public string Descripcion { get; set; } = null!;
}

public class ActualizarIncluyeRequest : CrearIncluyeRequest
{
}

public class CrearImagenRequest
{
    public string Url { get; set; } = null!;
    public string? Descripcion { get; set; }
}

public class ActualizarImagenRequest : CrearImagenRequest
{
}
