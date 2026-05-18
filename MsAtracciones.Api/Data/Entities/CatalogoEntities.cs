namespace MsAtracciones.Api.Data.Entities;

public class DestinoEntity
{
    public int DesId { get; set; }
    public Guid DesGuid { get; set; }
    public string DesNombre { get; set; } = null!;
    public string DesPais { get; set; } = null!;
    public string? DesImagenUrl { get; set; }
    public DateTimeOffset DesFechaIngreso { get; set; }
    public string DesUsuarioIngreso { get; set; } = null!;
    public string DesIpIngreso { get; set; } = null!;
    public DateTimeOffset? DesFechaMod { get; set; }
    public string? DesUsuarioMod { get; set; }
    public string? DesIpMod { get; set; }
    public DateTimeOffset? DesFechaEliminacion { get; set; }
    public string? DesUsuarioEliminacion { get; set; }
    public string? DesIpEliminacion { get; set; }
    public string DesEstado { get; set; } = "A";
}

public class CategoriaEntity
{
    public int CatId { get; set; }
    public Guid CatGuid { get; set; }
    public int? CatParentId { get; set; }
    public string CatNombre { get; set; } = null!;
    public DateTimeOffset CatFechaIngreso { get; set; }
    public string CatUsuarioIngreso { get; set; } = null!;
    public string CatIpIngreso { get; set; } = null!;
    public DateTimeOffset? CatFechaMod { get; set; }
    public string? CatUsuarioMod { get; set; }
    public string? CatIpMod { get; set; }
    public DateTimeOffset? CatFechaEliminacion { get; set; }
    public string? CatUsuarioEliminacion { get; set; }
    public string? CatIpEliminacion { get; set; }
    public string CatEstado { get; set; } = "A";
}

public class IdiomaEntity
{
    public int IdiId { get; set; }
    public Guid IdiGuid { get; set; }
    public string IdiDescripcion { get; set; } = null!;
    public DateTimeOffset IdiFechaIngreso { get; set; }
    public string IdUsuarioIngreso { get; set; } = null!;
    public string IdiIpIngreso { get; set; } = null!;
    public DateTimeOffset? IdiFechaMod { get; set; }
    public string? IdUsuarioMod { get; set; }
    public string? IdiIpMod { get; set; }
    public DateTimeOffset? IdiFechaEliminacion { get; set; }
    public string? IdUsuarioEliminacion { get; set; }
    public string? IdiIpEliminacion { get; set; }
    public string IdiEstado { get; set; } = "A";
}

public class IncluyeEntity
{
    public int IncId { get; set; }
    public Guid IncGuid { get; set; }
    public string IncDescripcion { get; set; } = null!;
    public string IncEstado { get; set; } = "A";
}

public class ImagenEntity
{
    public int ImgId { get; set; }
    public Guid ImgGuid { get; set; }
    public string ImgUrl { get; set; } = null!;
    public string? ImgDescripcion { get; set; }
    public DateTimeOffset ImgFechaIngreso { get; set; }
    public string ImgUsuarioIngreso { get; set; } = null!;
    public string ImgIpIngreso { get; set; } = null!;
    public DateTimeOffset? ImgFechaMod { get; set; }
    public string? ImgUsuarioMod { get; set; }
    public string? ImgIpMod { get; set; }
    public DateTimeOffset? ImgFechaEliminacion { get; set; }
    public string? ImgUsuarioEliminacion { get; set; }
    public string? ImgIpEliminacion { get; set; }
    public string ImgEstado { get; set; } = "A";
}
