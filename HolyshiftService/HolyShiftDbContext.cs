using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data.Common;

public class HolyShiftDbContext : DbContext
{
    private readonly HolyShiftDbConfig _config;

    public HolyShiftDbContext(DbContextOptions<HolyShiftDbContext> options, IOptions<HolyShiftDbConfig> config)
        : base(options)
    {
        _config = config.Value;
    }

    public DbSet<HolyShiftUser> Users { get; set; } = null!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connection = new DbConnectionStringBuilder
        {
            ["Server"] = _config.Server,
            ["Database"] = _config.Database,
            ["User"] = _config.User,
            ["Password"] = _config.Password
        };

        optionsBuilder.UseMySql(
            connection.ConnectionString,
            ServerVersion.AutoDetect(connection.ConnectionString));
    }
}
