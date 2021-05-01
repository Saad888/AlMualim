using Microsoft.EntityFrameworkCore;
using AlMualim.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AlMualim.Data
{
    public class AlMualimDbContext : IdentityDbContext
    {
        public AlMualimDbContext(DbContextOptions<AlMualimDbContext> options) : base(options) {}
        public DbSet<AlMualim.Models.Notes> Notes { get; set; }
        public DbSet<AlMualim.Models.Topics> Topics { get; set; }
        public DbSet<AlMualim.Models.Tags> Tags {get; set;}
        public DbSet<AlMualim.Models.Stories> Stories {get; set;}
        public DbSet<AlMualim.Models.Subscriptions> Subscriptions {get; set;}
    }
}