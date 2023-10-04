using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

public class HolyShiftDbContext : DbContext
{
    public HolyShiftDbContext(DbContextOptions<HolyShiftDbContext> options)
        : base(options) { }
}
