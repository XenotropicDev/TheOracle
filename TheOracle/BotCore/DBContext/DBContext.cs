using Microsoft.EntityFrameworkCore;
using System.IO;
using TheOracle.BotCore;

public class DiscordChannelContext : DbContext
{
    public DiscordChannelContext()
    {

    }
    public DiscordChannelContext(DbContextOptions<DiscordChannelContext> dbContextOptions) : base(dbContextOptions)
    {
        DbContextOptions = dbContextOptions;
        Database.EnsureCreated();
    }

    public DbSet<ChannelSettings> ChannelSettings { get; set; }
    public DbContextOptions<DiscordChannelContext> DbContextOptions { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(@"Data Source=ChannelSettings.sqlite;");
    }
}