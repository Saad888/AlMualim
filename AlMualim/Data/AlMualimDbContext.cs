using Microsoft.EntityFrameworkCore;
using AlMualim.Models;

namespace AlMualim.Data
{
    public class AlMualimDbContext : DbContext
    {
        public AlMualimDbContext(DbContextOptions<AlMualimDbContext> options) : base(options) {}
        
    }
}