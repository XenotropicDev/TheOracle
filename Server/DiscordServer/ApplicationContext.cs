using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Server.Data.homebrew;
using Server.GameInterfaces;
using TheOracle2;
using TheOracle2.Data;
using TheOracle2.GameObjects;

namespace Server.DiscordServer;

public class ApplicationContext : DbContext
{
    public DbSet<PlayerCharacter> PlayerCharacters { get; set; }
    public DbSet<TrackData> ProgressTrackers { get; set; }
    public DbSet<AssetData> CharacterAssets { get; set; }
    public DbSet<GameContentSet> GameContentSets { get; set; }
    public DbSet<Oracle> Oracles { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Move> Moves { get; set; }
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
        modelBuilder.Entity<PlayerCharacter>().Property(pc => pc.Impacts).UseCsvValueConverter();
        modelBuilder.Entity<AssetData>().Property(a => a.SelectedAbilities).UseCsvValueConverter();
        modelBuilder.Entity<AssetData>().Property(a => a.Inputs).UseCsvValueConverter();

        modelBuilder.Entity<Party>().Navigation(p => p.Characters).AutoInclude();

        modelBuilder.Entity<Asset>().Property(a => a.Aliases).UseCsvValueConverter();
        modelBuilder.Entity<AlterMove>().Property(a => a.Moves).UseCsvValueConverter();
        modelBuilder.Entity<AlterMove>().Property(a => a.Alters).UseCsvValueConverter();
        modelBuilder.Entity<Attachments>().Property(a => a.AssetTypes).UseCsvValueConverter();
        modelBuilder.Entity<Burn>().Property(a => a.Outcomes).UseCsvValueConverter();
        modelBuilder.Entity<ConditionMeter>().Property(a => a.Conditions).UseCsvValueConverter();
        modelBuilder.Entity<ConditionMeter>().Property(a => a.Aliases).UseCsvValueConverter();
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

            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseNpgsql(dbConnBuilder.ConnectionString);
        }
    }
}

public static class ApplicationContextExtenstions
{
    public static ValueConverter<IList<string>, string> stringArrayToCSVConverter = new ValueConverter<IList<string>, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<IList<string>>(v) ?? new List<string>()
            );

    public static ValueComparer<IList<string>> iListValueComparer = new ValueComparer<IList<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
            );

    public static ValueComparer<List<string>> listValueComparer = new ValueComparer<List<string>>(
        (c1, c2) => c1.SequenceEqual(c2),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c.ToList()
        );

    public static ValueConverter<List<string>, string> stringListToCSVConverter = new ValueConverter<List<string>, string>(
        v => JsonConvert.SerializeObject(v),
        v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>()
        );

    public static void UseCsvValueConverter<T>(this PropertyBuilder<T> builder) where T : ICollection<string>
    {
        if (typeof(T) == typeof(List<string>))
        {
            builder.HasConversion(stringListToCSVConverter);
            builder.Metadata.SetValueComparer(listValueComparer);
        }
        else
        {
            builder.HasConversion(stringArrayToCSVConverter);
            builder.Metadata.SetValueComparer(iListValueComparer);
        }
    }
}
