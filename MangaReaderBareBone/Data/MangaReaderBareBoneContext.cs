﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaReaderBareBone.Models;

namespace MangaReaderBareBone.Data
{
    public class MangaReaderBareBoneContext : DbContext
    {
        public MangaReaderBareBoneContext (DbContextOptions<MangaReaderBareBoneContext> options)
            : base(options)
        {
        }

        public DbSet<MangaReaderBareBone.Models.Manga> Mangas { get; set; } = default!;

        public DbSet<MangaReaderBareBone.Models.MangaChapters>? MangaChapters { get; set; }

        public DbSet<MangaReaderBareBone.Models.MangaLog>? MangaLogs { get; set; }
    }
}
