using MangaReaderBareBone.Models;
using Microsoft.EntityFrameworkCore;

namespace MangaReaderBareBone.Data
{
    public class MangaReaderBareBoneContext : DbContext
    {
        public MangaReaderBareBoneContext(DbContextOptions<MangaReaderBareBoneContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<MangaLogsChapterView>(
                    eb =>
                    {
                        eb.HasNoKey();
                        eb.ToView("vw_Manga_Logs_Chapter");
                        eb.Property(v => v.MangaId);
                    });
        }

        public DbSet<MangaReaderBareBone.Models.Manga> Mangas { get; set; } = default!;
        public DbSet<MangaReaderBareBone.Models.MangaChapters>? MangaChapters { get; set; }
        public DbSet<MangaReaderBareBone.Models.MangaLog>? MangaLogs { get; set; }
        public DbSet<MangaReaderBareBone.Models.MangaLogsChapterView>? MangaLogsChapterView { get; set; }
    }
}
