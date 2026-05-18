using Microsoft.EntityFrameworkCore;
using MsAtracciones.Api.Data.Entities;

namespace MsAtracciones.Api.Data;

public class AtraccionesDbContext : DbContext
{
    public AtraccionesDbContext(DbContextOptions<AtraccionesDbContext> options)
        : base(options)
    {
    }

    public DbSet<DestinoEntity> Destinos => Set<DestinoEntity>();
    public DbSet<CategoriaEntity> Categorias => Set<CategoriaEntity>();
    public DbSet<IdiomaEntity> Idiomas => Set<IdiomaEntity>();
    public DbSet<IncluyeEntity> Incluyes => Set<IncluyeEntity>();
    public DbSet<ImagenEntity> Imagenes => Set<ImagenEntity>();
    public DbSet<AtraccionEntity> Atracciones => Set<AtraccionEntity>();
    public DbSet<TicketEntity> Tickets => Set<TicketEntity>();
    public DbSet<HorarioEntity> Horarios => Set<HorarioEntity>();
    public DbSet<CategoriaAtraccionEntity> CategoriaAtracciones => Set<CategoriaAtraccionEntity>();
    public DbSet<IdiomaAtraccionEntity> IdiomasAtraccion => Set<IdiomaAtraccionEntity>();
    public DbSet<ImagenAtraccionEntity> ImagenesAtraccion => Set<ImagenAtraccionEntity>();
    public DbSet<AtraccionIncluyeEntity> AtraccionesIncluye => Set<AtraccionIncluyeEntity>();
    public DbSet<ReseniaEntity> Resenias => Set<ReseniaEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureCatalogos(modelBuilder);
        ConfigureAtracciones(modelBuilder);
    }

    private static void ConfigureCatalogos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DestinoEntity>(builder =>
        {
            builder.ToTable("destino");
            builder.HasKey(x => x.DesId);
            builder.Property(x => x.DesId).HasColumnName("des_id").ValueGeneratedOnAdd();
            builder.Property(x => x.DesGuid).HasColumnName("des_guid").IsRequired();
            builder.Property(x => x.DesNombre).HasColumnName("des_nombre").HasMaxLength(150).IsRequired();
            builder.Property(x => x.DesPais).HasColumnName("des_pais").HasMaxLength(100).IsRequired();
            builder.Property(x => x.DesImagenUrl).HasColumnName("des_imagen_url").HasMaxLength(500);
            builder.Property(x => x.DesFechaIngreso).HasColumnName("des_fecha_ingreso").IsRequired();
            builder.Property(x => x.DesUsuarioIngreso).HasColumnName("des_usuario_ingreso").HasMaxLength(100).IsRequired();
            builder.Property(x => x.DesIpIngreso).HasColumnName("des_ip_ingreso").HasMaxLength(45).IsRequired();
            builder.Property(x => x.DesFechaMod).HasColumnName("des_fecha_mod");
            builder.Property(x => x.DesUsuarioMod).HasColumnName("des_usuario_mod").HasMaxLength(100);
            builder.Property(x => x.DesIpMod).HasColumnName("des_ip_mod").HasMaxLength(45);
            builder.Property(x => x.DesFechaEliminacion).HasColumnName("des_fecha_eliminacion");
            builder.Property(x => x.DesUsuarioEliminacion).HasColumnName("des_usuario_eliminacion").HasMaxLength(100);
            builder.Property(x => x.DesIpEliminacion).HasColumnName("des_ip_eliminacion").HasMaxLength(45);
            builder.Property(x => x.DesEstado).HasColumnName("des_estado").HasMaxLength(1).IsRequired();
            builder.HasIndex(x => x.DesGuid).IsUnique();
        });

        modelBuilder.Entity<CategoriaEntity>(builder =>
        {
            builder.ToTable("categoria");
            builder.HasKey(x => x.CatId);
            builder.Property(x => x.CatId).HasColumnName("cat_id").ValueGeneratedOnAdd();
            builder.Property(x => x.CatGuid).HasColumnName("cat_guid").IsRequired();
            builder.Property(x => x.CatParentId).HasColumnName("cat_parent_id");
            builder.Property(x => x.CatNombre).HasColumnName("cat_nombre").HasMaxLength(100).IsRequired();
            builder.Property(x => x.CatFechaIngreso).HasColumnName("cat_fecha_ingreso").IsRequired();
            builder.Property(x => x.CatUsuarioIngreso).HasColumnName("cat_usuario_ingreso").HasMaxLength(100).IsRequired();
            builder.Property(x => x.CatIpIngreso).HasColumnName("cat_ip_ingreso").HasMaxLength(45).IsRequired();
            builder.Property(x => x.CatFechaMod).HasColumnName("cat_fecha_mod");
            builder.Property(x => x.CatUsuarioMod).HasColumnName("cat_usuario_mod").HasMaxLength(100);
            builder.Property(x => x.CatIpMod).HasColumnName("cat_ip_mod").HasMaxLength(45);
            builder.Property(x => x.CatFechaEliminacion).HasColumnName("cat_fecha_eliminacion");
            builder.Property(x => x.CatUsuarioEliminacion).HasColumnName("cat_usuario_eliminacion").HasMaxLength(100);
            builder.Property(x => x.CatIpEliminacion).HasColumnName("cat_ip_eliminacion").HasMaxLength(45);
            builder.Property(x => x.CatEstado).HasColumnName("cat_estado").HasMaxLength(1).IsRequired();
            builder.HasIndex(x => x.CatGuid).IsUnique();
        });

        modelBuilder.Entity<IdiomaEntity>(builder =>
        {
            builder.ToTable("idioma");
            builder.HasKey(x => x.IdiId);
            builder.Property(x => x.IdiId).HasColumnName("idi_id").ValueGeneratedOnAdd();
            builder.Property(x => x.IdiGuid).HasColumnName("idi_guid").IsRequired();
            builder.Property(x => x.IdiDescripcion).HasColumnName("idi_descripcion").HasMaxLength(80).IsRequired();
            builder.Property(x => x.IdiFechaIngreso).HasColumnName("idi_fecha_ingreso").IsRequired();
            builder.Property(x => x.IdUsuarioIngreso).HasColumnName("id_usuario_ingreso").HasMaxLength(100).IsRequired();
            builder.Property(x => x.IdiIpIngreso).HasColumnName("idi_ip_ingreso").HasMaxLength(45).IsRequired();
            builder.Property(x => x.IdiFechaMod).HasColumnName("idi_fecha_mod");
            builder.Property(x => x.IdUsuarioMod).HasColumnName("id_usuario_mod").HasMaxLength(100);
            builder.Property(x => x.IdiIpMod).HasColumnName("idi_ip_mod").HasMaxLength(45);
            builder.Property(x => x.IdiFechaEliminacion).HasColumnName("idi_fecha_eliminacion");
            builder.Property(x => x.IdUsuarioEliminacion).HasColumnName("id_usuario_eliminacion").HasMaxLength(100);
            builder.Property(x => x.IdiIpEliminacion).HasColumnName("idi_ip_eliminacion").HasMaxLength(45);
            builder.Property(x => x.IdiEstado).HasColumnName("idi_estado").HasMaxLength(1).IsRequired();
            builder.HasIndex(x => x.IdiGuid).IsUnique();
        });

        modelBuilder.Entity<IncluyeEntity>(builder =>
        {
            builder.ToTable("incluye");
            builder.HasKey(x => x.IncId);
            builder.Property(x => x.IncId).HasColumnName("inc_id").ValueGeneratedOnAdd();
            builder.Property(x => x.IncGuid).HasColumnName("inc_guid").IsRequired();
            builder.Property(x => x.IncDescripcion).HasColumnName("inc_descripcion").HasMaxLength(200).IsRequired();
            builder.Property(x => x.IncEstado).HasColumnName("inc_estado").HasMaxLength(1).IsRequired();
            builder.HasIndex(x => x.IncGuid).IsUnique();
        });

        modelBuilder.Entity<ImagenEntity>(builder =>
        {
            builder.ToTable("imagen");
            builder.HasKey(x => x.ImgId);
            builder.Property(x => x.ImgId).HasColumnName("img_id").ValueGeneratedOnAdd();
            builder.Property(x => x.ImgGuid).HasColumnName("img_guid").IsRequired();
            builder.Property(x => x.ImgUrl).HasColumnName("img_url").HasMaxLength(500).IsRequired();
            builder.Property(x => x.ImgDescripcion).HasColumnName("img_descripcion").HasMaxLength(200);
            builder.Property(x => x.ImgFechaIngreso).HasColumnName("img_fecha_ingreso").IsRequired();
            builder.Property(x => x.ImgUsuarioIngreso).HasColumnName("img_usuario_ingreso").HasMaxLength(100).IsRequired();
            builder.Property(x => x.ImgIpIngreso).HasColumnName("img_ip_ingreso").HasMaxLength(45).IsRequired();
            builder.Property(x => x.ImgFechaMod).HasColumnName("img_fecha_mod");
            builder.Property(x => x.ImgUsuarioMod).HasColumnName("img_usuario_mod").HasMaxLength(100);
            builder.Property(x => x.ImgIpMod).HasColumnName("img_ip_mod").HasMaxLength(45);
            builder.Property(x => x.ImgFechaEliminacion).HasColumnName("img_fecha_eliminacion");
            builder.Property(x => x.ImgUsuarioEliminacion).HasColumnName("img_usuario_eliminacion").HasMaxLength(100);
            builder.Property(x => x.ImgIpEliminacion).HasColumnName("img_ip_eliminacion").HasMaxLength(45);
            builder.Property(x => x.ImgEstado).HasColumnName("img_estado").HasMaxLength(1).IsRequired();
            builder.HasIndex(x => x.ImgGuid).IsUnique();
        });
    }

    private static void ConfigureAtracciones(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AtraccionEntity>(builder =>
        {
            builder.ToTable("atraccion");
            builder.HasKey(x => x.AtId);
            builder.Property(x => x.AtId).HasColumnName("at_id").ValueGeneratedOnAdd();
            builder.Property(x => x.AtGuid).HasColumnName("at_guid").IsRequired();
            builder.Property(x => x.DesGuid).HasColumnName("des_guid").IsRequired();
            builder.Property(x => x.AtNumEstablecimiento).HasColumnName("at_num_establecimiento").HasMaxLength(30);
            builder.Property(x => x.AtNombre).HasColumnName("at_nombre").HasMaxLength(200).IsRequired();
            builder.Property(x => x.AtDescripcion).HasColumnName("at_descripcion").HasMaxLength(2000);
            builder.Property(x => x.AtTotalResenias).HasColumnName("at_total_resenias").IsRequired();
            builder.Property(x => x.AtDireccion).HasColumnName("at_direccion").HasMaxLength(300);
            builder.Property(x => x.AtDuracionMinutos).HasColumnName("at_duracion_minutos");
            builder.Property(x => x.AtPuntoEncuentro).HasColumnName("at_punto_encuentro").HasMaxLength(300);
            builder.Property(x => x.AtPrecioReferencia).HasColumnName("at_precio_referencia").HasPrecision(10, 2);
            builder.Property(x => x.AtIncluyeAcompaniante).HasColumnName("at_incluye_acompaniante").IsRequired();
            builder.Property(x => x.AtIncluyeTransporte).HasColumnName("at_incluye_transporte").IsRequired();
            builder.Property(x => x.AtDisponible).HasColumnName("at_disponible").IsRequired();
            builder.Property(x => x.AtFechaIngreso).HasColumnName("at_fecha_ingreso").IsRequired();
            builder.Property(x => x.AtUsuarioIngreso).HasColumnName("at_usuario_ingreso").HasMaxLength(100).IsRequired();
            builder.Property(x => x.AtIpIngreso).HasColumnName("at_ip_ingreso").HasMaxLength(45).IsRequired();
            builder.Property(x => x.AtFechaMod).HasColumnName("at_fecha_mod");
            builder.Property(x => x.AtUsuarioMod).HasColumnName("at_usuario_mod").HasMaxLength(100);
            builder.Property(x => x.AtIpMod).HasColumnName("at_ip_mod").HasMaxLength(45);
            builder.Property(x => x.AtFechaEliminacion).HasColumnName("at_fecha_eliminacion");
            builder.Property(x => x.AtUsuarioEliminacion).HasColumnName("at_usuario_eliminacion").HasMaxLength(100);
            builder.Property(x => x.AtIpEliminacion).HasColumnName("at_ip_eliminacion").HasMaxLength(45);
            builder.Property(x => x.AtEstado).HasColumnName("at_estado").HasMaxLength(1).IsRequired();
            builder.HasIndex(x => x.AtGuid).IsUnique();
            builder.HasIndex(x => x.DesGuid);
        });

        modelBuilder.Entity<TicketEntity>(builder =>
        {
            builder.ToTable("ticket");
            builder.HasKey(x => x.TckId);
            builder.Property(x => x.TckId).HasColumnName("tck_id").ValueGeneratedOnAdd();
            builder.Property(x => x.TckGuid).HasColumnName("tck_guid").IsRequired();
            builder.Property(x => x.AtId).HasColumnName("at_id").IsRequired();
            builder.Property(x => x.TckTitulo).HasColumnName("tck_titulo").HasMaxLength(150).IsRequired();
            builder.Property(x => x.TckPrecio).HasColumnName("tck_precio").HasPrecision(10, 2).IsRequired();
            builder.Property(x => x.TckTipoParticipante).HasColumnName("tck_tipo_participante").HasMaxLength(30).IsRequired();
            builder.Property(x => x.TckCapacidadMaxima).HasColumnName("tck_capacidad_maxima").IsRequired();
            builder.Property(x => x.TckCuposDisponibles).HasColumnName("tck_cupos_disponibles").IsRequired();
            builder.Property(x => x.TckFechaIngreso).HasColumnName("tck_fecha_ingreso").IsRequired();
            builder.Property(x => x.TckUsuarioIngreso).HasColumnName("tck_usuario_ingreso").HasMaxLength(100).IsRequired();
            builder.Property(x => x.TckIpIngreso).HasColumnName("tck_ip_ingreso").HasMaxLength(45).IsRequired();
            builder.Property(x => x.TckFechaMod).HasColumnName("tck_fecha_mod");
            builder.Property(x => x.TckUsuarioMod).HasColumnName("tck_usuario_mod").HasMaxLength(100);
            builder.Property(x => x.TckIpMod).HasColumnName("tck_ip_mod").HasMaxLength(45);
            builder.Property(x => x.TckFechaEliminacion).HasColumnName("tck_fecha_eliminacion");
            builder.Property(x => x.TckUsuarioEliminacion).HasColumnName("tck_usuario_eliminacion").HasMaxLength(100);
            builder.Property(x => x.TckIpEliminacion).HasColumnName("tck_ip_eliminacion").HasMaxLength(45);
            builder.Property(x => x.TckEstado).HasColumnName("tck_estado").HasMaxLength(1).IsRequired();
            builder.HasIndex(x => x.TckGuid).IsUnique();
            builder.HasOne(x => x.Atraccion).WithMany(x => x.Tickets).HasForeignKey(x => x.AtId);
        });

        modelBuilder.Entity<HorarioEntity>(builder =>
        {
            builder.ToTable("horario");
            builder.HasKey(x => x.HorId);
            builder.Property(x => x.HorId).HasColumnName("hor_id").ValueGeneratedOnAdd();
            builder.Property(x => x.HorGuid).HasColumnName("hor_guid").IsRequired();
            builder.Property(x => x.TckId).HasColumnName("tck_id").IsRequired();
            builder.Property(x => x.HorFecha).HasColumnName("hor_fecha").IsRequired();
            builder.Property(x => x.HorHoraInicio).HasColumnName("hor_hora_inicio").IsRequired();
            builder.Property(x => x.HorHoraFin).HasColumnName("hor_hora_fin");
            builder.Property(x => x.HorCuposDisponibles).HasColumnName("hor_cupos_disponibles").IsRequired();
            builder.Property(x => x.HorFechaIngreso).HasColumnName("hor_fecha_ingreso").IsRequired();
            builder.Property(x => x.HorUsuarioIngreso).HasColumnName("hor_usuario_ingreso").HasMaxLength(100).IsRequired();
            builder.Property(x => x.HorIpIngreso).HasColumnName("hor_ip_ingreso").HasMaxLength(45).IsRequired();
            builder.Property(x => x.HorFechaMod).HasColumnName("hor_fecha_mod");
            builder.Property(x => x.HorUsuarioMod).HasColumnName("hor_usuario_mod").HasMaxLength(100);
            builder.Property(x => x.HorIpMod).HasColumnName("hor_ip_mod").HasMaxLength(45);
            builder.Property(x => x.HorFechaEliminacion).HasColumnName("hor_fecha_eliminacion");
            builder.Property(x => x.HorUsuarioEliminacion).HasColumnName("hor_usuario_eliminacion").HasMaxLength(100);
            builder.Property(x => x.HorIpEliminacion).HasColumnName("hor_ip_eliminacion").HasMaxLength(45);
            builder.Property(x => x.HorEstado).HasColumnName("hor_estado").HasMaxLength(1).IsRequired();
            builder.HasIndex(x => x.HorGuid).IsUnique();
            builder.HasIndex(x => new { x.TckId, x.HorFecha, x.HorHoraInicio }).IsUnique();
            builder.HasOne(x => x.Ticket).WithMany(x => x.Horarios).HasForeignKey(x => x.TckId);
        });

        ConfigurePivots(modelBuilder);
        ConfigureResenias(modelBuilder);
    }

    private static void ConfigurePivots(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoriaAtraccionEntity>(builder =>
        {
            builder.ToTable("categoria_atraccion");
            builder.HasKey(x => new { x.CatGuid, x.AtId });
            builder.Property(x => x.CatGuid).HasColumnName("cat_guid").IsRequired();
            builder.Property(x => x.AtId).HasColumnName("at_id").IsRequired();
            builder.Property(x => x.CaFechaIngreso).HasColumnName("ca_fecha_ingreso").IsRequired();
            builder.Property(x => x.CaUsuarioIngreso).HasColumnName("ca_usuario_ingreso").HasMaxLength(100).IsRequired();
            builder.Property(x => x.CaFechaEliminacion).HasColumnName("ca_fecha_eliminacion");
            builder.Property(x => x.CaUsuarioEliminacion).HasColumnName("ca_usuario_eliminacion").HasMaxLength(100);
            builder.Property(x => x.CaEstado).HasColumnName("ca_estado").HasMaxLength(1).IsRequired();
        });

        modelBuilder.Entity<IdiomaAtraccionEntity>(builder =>
        {
            builder.ToTable("idioma_atraccion");
            builder.HasKey(x => new { x.IdiGuid, x.AtId });
            builder.Property(x => x.IdiGuid).HasColumnName("idi_guid").IsRequired();
            builder.Property(x => x.AtId).HasColumnName("at_id").IsRequired();
            builder.Property(x => x.IaFechaIngreso).HasColumnName("ia_fecha_ingreso").IsRequired();
            builder.Property(x => x.IaUsuarioIngreso).HasColumnName("ia_usuario_ingreso").HasMaxLength(100).IsRequired();
            builder.Property(x => x.IaFechaEliminacion).HasColumnName("ia_fecha_eliminacion");
            builder.Property(x => x.IaUsuarioEliminacion).HasColumnName("ia_usuario_eliminacion").HasMaxLength(100);
            builder.Property(x => x.IaEstado).HasColumnName("ia_estado").HasMaxLength(1).IsRequired();
        });

        modelBuilder.Entity<ImagenAtraccionEntity>(builder =>
        {
            builder.ToTable("imagen_atraccion");
            builder.HasKey(x => new { x.ImgGuid, x.AtId });
            builder.Property(x => x.ImgGuid).HasColumnName("img_guid").IsRequired();
            builder.Property(x => x.AtId).HasColumnName("at_id").IsRequired();
            builder.Property(x => x.ImaFechaIngreso).HasColumnName("ima_fecha_ingreso").IsRequired();
            builder.Property(x => x.ImaUsuarioIngreso).HasColumnName("ima_usuario_ingreso").HasMaxLength(100).IsRequired();
            builder.Property(x => x.ImaFechaEliminacion).HasColumnName("ima_fecha_eliminacion");
            builder.Property(x => x.ImaUsuarioEliminacion).HasColumnName("ima_usuario_eliminacion").HasMaxLength(100);
            builder.Property(x => x.ImaEstado).HasColumnName("ima_estado").HasMaxLength(1).IsRequired();
        });

        modelBuilder.Entity<AtraccionIncluyeEntity>(builder =>
        {
            builder.ToTable("atraccion_incluye");
            builder.HasKey(x => new { x.IncGuid, x.AtId });
            builder.Property(x => x.IncGuid).HasColumnName("inc_guid").IsRequired();
            builder.Property(x => x.AtId).HasColumnName("at_id").IsRequired();
            builder.Property(x => x.AiFechaIngreso).HasColumnName("ai_fecha_ingreso").IsRequired();
            builder.Property(x => x.AiUsuarioIngreso).HasColumnName("ai_usuario_ingreso").HasMaxLength(100).IsRequired();
            builder.Property(x => x.AiFechaEliminacion).HasColumnName("ai_fecha_eliminacion");
            builder.Property(x => x.AiUsuarioEliminacion).HasColumnName("ai_usuario_eliminacion").HasMaxLength(100);
            builder.Property(x => x.AiEstado).HasColumnName("ai_estado").HasMaxLength(1).IsRequired();
        });
    }

    private static void ConfigureResenias(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReseniaEntity>(builder =>
        {
            builder.ToTable("resenia");
            builder.HasKey(x => x.RsnId);
            builder.Property(x => x.RsnId).HasColumnName("rsn_id").ValueGeneratedOnAdd();
            builder.Property(x => x.RsnGuid).HasColumnName("rsn_guid").IsRequired();
            builder.Property(x => x.AtId).HasColumnName("at_id").IsRequired();
            builder.Property(x => x.RevGuid).HasColumnName("rev_guid").IsRequired();
            builder.Property(x => x.RsnComentario).HasColumnName("rsn_comentario").HasMaxLength(1000);
            builder.Property(x => x.RsnRating).HasColumnName("rsn_rating").IsRequired();
            builder.Property(x => x.RsnFechaCreacion).HasColumnName("rsn_fecha_creacion").IsRequired();
            builder.Property(x => x.RsnUsuarioCreacion).HasColumnName("rsn_usuario_creacion").HasMaxLength(100).IsRequired();
            builder.Property(x => x.RsnIpCreacion).HasColumnName("rsn_ip_creacion").HasMaxLength(45).IsRequired();
            builder.Property(x => x.RsnFechaMod).HasColumnName("rsn_fecha_mod");
            builder.Property(x => x.RsnUsuarioMod).HasColumnName("rsn_usuario_mod").HasMaxLength(100);
            builder.Property(x => x.RsnIpMod).HasColumnName("rsn_ip_mod").HasMaxLength(45);
            builder.Property(x => x.RsnFechaEliminacion).HasColumnName("rsn_fecha_eliminacion");
            builder.Property(x => x.RsnUsuarioEliminacion).HasColumnName("rsn_usuario_eliminacion").HasMaxLength(100);
            builder.Property(x => x.RsnIpEliminacion).HasColumnName("rsn_ip_eliminacion").HasMaxLength(45);
            builder.Property(x => x.RsnEstado).HasColumnName("rsn_estado").HasMaxLength(1).IsRequired();
        builder.HasIndex(x => x.RsnGuid).IsUnique();
        builder.HasIndex(x => x.RevGuid).IsUnique();
        });
    }
}
