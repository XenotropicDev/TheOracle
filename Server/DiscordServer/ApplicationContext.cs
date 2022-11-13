using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Server.GameInterfaces;
using TheOracle2;
using TheOracle2.GameObjects;

namespace Server.DiscordServer;

public class ApplicationContext : DbContext
{
    public DbSet<PlayerCharacter> PlayerCharacters { get; set; }
    public DbSet<TrackData> ProgressTrackers { get; set; }
    public DbSet<AssetData> CharacterAssets { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Party> Parties { get; set; }

    public ApplicationContext()
    {
    }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var stringArrayToCSVConverter = new ValueConverter<IList<string>, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<IList<string>>(v) ?? new List<string>()
            );

        var valueComparer = new ValueComparer<IList<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
            );

        modelBuilder.Entity<PlayerCharacter>().Property(pc => pc.Impacts).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<AssetData>().Property(a => a.SelectedAbilities).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<AssetData>().Property(a => a.Inputs).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile("dbSettings.json", optional: false, reloadOnChange: true)
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            .Build();

            var dbConn = config.GetSection("dbConnectionString").Value;
            var dbPass = config.GetSection("dbPassword").Value;
            var dbConnBuilder = new NpgsqlConnectionStringBuilder(dbConn) { Password = dbPass };

            optionsBuilder.UseNpgsql(dbConnBuilder.ConnectionString);
        }
    }
}
