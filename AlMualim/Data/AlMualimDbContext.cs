using Microsoft.EntityFrameworkCore;
using AlMualim.Models;

namespace AlMualim.Data
{
    public class AlMualimDbContext : DbContext
    {
        public AlMualimDbContext(DbContextOptions<AlMualimDbContext> options) : base(options) {}
        public DbSet<AlMualim.Models.Notes> Notes { get; set; }
        public DbSet<AlMualim.Models.Topics> Topics { get; set; }
        public DbSet<AlMualim.Models.Surah> Surah { get; set; }
        public DbSet<AlMualim.Models.Tags> Tags {get; set;}
    }
}