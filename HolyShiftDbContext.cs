using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

public class HolyShiftDbContext : DbContext
{
    public HolyShiftDbContext(DbContextOptions<HolyShiftDbContext> options)
        : base(options) { }

    public DbSet<HolyShiftUser> Users { get; set; }
    public DbSet<ItemDbModel> Items { get; set; }
}
