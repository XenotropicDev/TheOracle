using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Dataforged;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Server.Data.homebrew;
using Server.GameInterfaces;
using TheOracle2;
using TheOracle2.GameObjects;

namespace Server.DiscordServer;

public class ApplicationContext : DbContext
{
    public DbSet<PlayerCharacter> PlayerCharacters { get; set; }
    public DbSet<TrackData> ProgressTrackers { get; set; }
    public DbSet<AssetData> CharacterAssets { get; set; }
    public DbSet<GameContentSet> GameContentSets { get; set; }
    public DbSet<OracleTable> Oracles { get; set; }
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
        modelBuilder.Entity<GameContentSet>().Property(e => e.Id).ValueGeneratedOnAdd();

        //modelBuilder.Entity<PlayerCharacter>().Property(pc => pc.Impacts).UseCsvValueConverter();
        //modelBuilder.Entity<AssetData>().Property(a => a.SelectedAbilities).UseCsvValueConverter();
        //modelBuilder.Entity<AssetData>().Property(a => a.Inputs).UseCsvValueConverter();

        //modelBuilder.Entity<Party>().Navigation(p => p.Characters).AutoInclude();

        //modelBuilder.Entity<Asset>().Property(a => a.Aliases).UseCsvValueConverter();
        //modelBuilder.Entity<AlterMove>().Property(a => a.Moves).UseCsvValueConverter();
        //modelBuilder.Entity<AlterMove>().Property(a => a.Alters).UseCsvValueConverter();
        //modelBuilder.Entity<Attachments>().Property(a => a.AssetTypes).UseCsvValueConverter();
        //modelBuilder.Entity<Burn>().Property(a => a.Outcomes).UseCsvValueConverter();
        //modelBuilder.Entity<ConditionMeter>().Property(a => a.Conditions).UseCsvValueConverter();
        //modelBuilder.Entity<ConditionMeter>().Property(a => a.Aliases).UseCsvValueConverter();
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

            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<ObservableCollection<string>>().HaveConversion<ObservableCollectionStringConverter, ObsCollComparer>();
        configurationBuilder.Properties<IList<string>>().HaveConversion<ListStringConverter, ListComparer>();
    }
}

//public static class ApplicationContextExtenstions
//{
//    public static ValueConverter<IList<string>, string> stringArrayToCSVConverter = new ValueConverter<IList<string>, string>(
//            v => JsonConvert.SerializeObject(v),
//            v => JsonConvert.DeserializeObject<ObservableCollection<string>>(v) ?? new ObservableCollection<string>()
//            );

//    public static ValueComparer<IList<string>> iListValueComparer = new ValueComparer<IList<string>>(
//            (c1, c2) => c1.SequenceEqual(c2),
//            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
//            c => c.ToList()
//            );

//    public static void UseCsvValueConverter<T>(this PropertyBuilder<T> builder) where T : IList<string>
//    {
//        builder.HasConversion(stringArrayToCSVConverter);
//        builder.Metadata.SetValueComparer(iListValueComparer);
//    }
//}

public class ObservableCollectionStringConverter : ValueConverter<ObservableCollection<string>, string>
{
    public ObservableCollectionStringConverter() : base(Serialize, Deserialize, null)
    {
    }

    static Expression<Func<string, ObservableCollection<string>>> Deserialize = x => JsonConvert.DeserializeObject<ObservableCollection<string>>(x);
    static Expression<Func<ObservableCollection<string>, string>> Serialize = x => JsonConvert.SerializeObject(x);
}

public class ObsCollComparer : ValueComparer<ObservableCollection<string>>
{
    public ObsCollComparer() : base((c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())))
    {
        
    }
}

public class ListStringConverter : ValueConverter<IList<string>, string>
{
    public ListStringConverter() : base(Serialize, Deserialize, null)
    {
    }

    static Expression<Func<string, IList<string>>> Deserialize = x => JsonConvert.DeserializeObject<List<string>>(x);
    static Expression<Func<IList<string>, string>> Serialize = x => JsonConvert.SerializeObject(x);
}

public class ListComparer : ValueComparer<IList<string>>
{
    public ListComparer() : base((c1, c2) => c1.SequenceEqual(c2), c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())))
    {    }
}
