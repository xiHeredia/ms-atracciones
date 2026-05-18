namespace MsAtracciones.Api.Data.Entities;

public class AtraccionEntity
{
    public int AtId { get; set; }
    public Guid AtGuid { get; set; }
    public Guid DesGuid { get; set; }
    public string? AtNumEstablecimiento { get; set; }
    public string AtNombre { get; set; } = null!;
    public string? AtDescripcion { get; set; }
    public int AtTotalResenias { get; set; }
    public string? AtDireccion { get; set; }
    public int? AtDuracionMinutos { get; set; }
    public string? AtPuntoEncuentro { get; set; }
    public decimal? AtPrecioReferencia { get; set; }
    public bool AtIncluyeAcompaniante { get; set; }
    public bool AtIncluyeTransporte { get; set; }
    public bool AtDisponible { get; set; }
    public DateTimeOffset AtFechaIngreso { get; set; }
    public string AtUsuarioIngreso { get; set; } = null!;
    public string AtIpIngreso { get; set; } = null!;
    public DateTimeOffset? AtFechaMod { get; set; }
    public string? AtUsuarioMod { get; set; }
    public string? AtIpMod { get; set; }
    public DateTimeOffset? AtFechaEliminacion { get; set; }
    public string? AtUsuarioEliminacion { get; set; }
    public string? AtIpEliminacion { get; set; }
    public string AtEstado { get; set; } = "A";
    public ICollection<TicketEntity> Tickets { get; set; } = new List<TicketEntity>();
}

public class TicketEntity
{
    public int TckId { get; set; }
    public Guid TckGuid { get; set; }
    public int AtId { get; set; }
    public string TckTitulo { get; set; } = null!;
    public decimal TckPrecio { get; set; }
    public string TckTipoParticipante { get; set; } = null!;
    public int TckCapacidadMaxima { get; set; }
    public int TckCuposDisponibles { get; set; }
    public DateTimeOffset TckFechaIngreso { get; set; }
    public string TckUsuarioIngreso { get; set; } = null!;
    public string TckIpIngreso { get; set; } = null!;
    public DateTimeOffset? TckFechaMod { get; set; }
    public string? TckUsuarioMod { get; set; }
    public string? TckIpMod { get; set; }
    public DateTimeOffset? TckFechaEliminacion { get; set; }
    public string? TckUsuarioEliminacion { get; set; }
    public string? TckIpEliminacion { get; set; }
    public string TckEstado { get; set; } = "A";
    public AtraccionEntity Atraccion { get; set; } = null!;
    public ICollection<HorarioEntity> Horarios { get; set; } = new List<HorarioEntity>();
}

public class HorarioEntity
{
    public int HorId { get; set; }
    public Guid HorGuid { get; set; }
    public int TckId { get; set; }
    public DateOnly HorFecha { get; set; }
    public TimeOnly HorHoraInicio { get; set; }
    public TimeOnly? HorHoraFin { get; set; }
    public int HorCuposDisponibles { get; set; }
    public DateTimeOffset HorFechaIngreso { get; set; }
    public string HorUsuarioIngreso { get; set; } = null!;
    public string HorIpIngreso { get; set; } = null!;
    public DateTimeOffset? HorFechaMod { get; set; }
    public string? HorUsuarioMod { get; set; }
    public string? HorIpMod { get; set; }
    public DateTimeOffset? HorFechaEliminacion { get; set; }
    public string? HorUsuarioEliminacion { get; set; }
    public string? HorIpEliminacion { get; set; }
    public string HorEstado { get; set; } = "A";
    public TicketEntity Ticket { get; set; } = null!;
}

public class CategoriaAtraccionEntity
{
    public Guid CatGuid { get; set; }
    public int AtId { get; set; }
    public DateTimeOffset CaFechaIngreso { get; set; }
    public string CaUsuarioIngreso { get; set; } = null!;
    public DateTimeOffset? CaFechaEliminacion { get; set; }
    public string? CaUsuarioEliminacion { get; set; }
    public string CaEstado { get; set; } = "A";
}

public class IdiomaAtraccionEntity
{
    public Guid IdiGuid { get; set; }
    public int AtId { get; set; }
    public DateTimeOffset IaFechaIngreso { get; set; }
    public string IaUsuarioIngreso { get; set; } = null!;
    public DateTimeOffset? IaFechaEliminacion { get; set; }
    public string? IaUsuarioEliminacion { get; set; }
    public string IaEstado { get; set; } = "A";
}

public class ImagenAtraccionEntity
{
    public Guid ImgGuid { get; set; }
    public int AtId { get; set; }
    public DateTimeOffset ImaFechaIngreso { get; set; }
    public string ImaUsuarioIngreso { get; set; } = null!;
    public DateTimeOffset? ImaFechaEliminacion { get; set; }
    public string? ImaUsuarioEliminacion { get; set; }
    public string ImaEstado { get; set; } = "A";
}

public class AtraccionIncluyeEntity
{
    public Guid IncGuid { get; set; }
    public int AtId { get; set; }
    public DateTimeOffset AiFechaIngreso { get; set; }
    public string AiUsuarioIngreso { get; set; } = null!;
    public DateTimeOffset? AiFechaEliminacion { get; set; }
    public string? AiUsuarioEliminacion { get; set; }
    public string AiEstado { get; set; } = "A";
}

public class ReseniaEntity
{
    public int RsnId { get; set; }
    public Guid RsnGuid { get; set; }
    public int AtId { get; set; }
    public Guid RevGuid { get; set; }
    public string? RsnComentario { get; set; }
    public short RsnRating { get; set; }
    public DateTimeOffset RsnFechaCreacion { get; set; }
    public string RsnUsuarioCreacion { get; set; } = null!;
    public string RsnIpCreacion { get; set; } = null!;
    public DateTimeOffset? RsnFechaMod { get; set; }
    public string? RsnUsuarioMod { get; set; }
    public string? RsnIpMod { get; set; }
    public DateTimeOffset? RsnFechaEliminacion { get; set; }
    public string? RsnUsuarioEliminacion { get; set; }
    public string? RsnIpEliminacion { get; set; }
    public string RsnEstado { get; set; } = "A";
}
