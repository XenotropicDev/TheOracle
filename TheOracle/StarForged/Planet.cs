using System.Collections.Generic;

namespace TheOracle.StarForged
{
    public class Planet
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Atmosphere> Atmospheres { get; set; }
        public List<Settlement> Settlements { get; set; }
        public int MaxSpaceObservations { get; set; }
        public List<Spaceobservation> SpaceObservations { get; set; }
        public List<Numberofbiomes> NumberOfBiomes { get; set; }
        public List<Biome> PossibleBiomes { get; set; }
        public List<Life> PossibleLife { get; set; }
        public List<Closerlook> CloserLooks { get; set; }
    }

    public class Atmosphere
    {
        public int Chance { get; set; }
        public string Description { get; set; }
    }

    public class Settlement
    {
        public Terminus Terminus { get; set; }
        public Outlands Outlands { get; set; }
        public Expanse Expanse { get; set; }
    }

    public class Terminus
    {
        public string _30 { get; set; }
        public string _40 { get; set; }
        public string _75 { get; set; }
        public string _90 { get; set; }
        public string _100 { get; set; }
    }

    public class Outlands
    {
        public string _60 { get; set; }
        public string _65 { get; set; }
        public string _90 { get; set; }
        public string _97 { get; set; }
        public string _100 { get; set; }
    }

    public class Expanse
    {
        public string _80 { get; set; }
        public string _83 { get; set; }
        public string _93 { get; set; }
        public string _98 { get; set; }
        public string _100 { get; set; }
    }

    public class Spaceobservation
    {
        public string _11 { get; set; }
        public string _22 { get; set; }
        public string _33 { get; set; }
        public string _44 { get; set; }
        public string _55 { get; set; }
        public string _66 { get; set; }
        public string _77 { get; set; }
        public string _88 { get; set; }
        public string _92 { get; set; }
        public string _100 { get; set; }
    }

    public class Numberofbiomes
    {
        public int _20 { get; set; }
        public int _70 { get; set; }
        public int _90 { get; set; }
        public int _100 { get; set; }
    }

    public class Biome
    {
        public string _5 { get; set; }
        public string _10 { get; set; }
        public string _15 { get; set; }
        public string _20 { get; set; }
        public string _25 { get; set; }
        public string _30 { get; set; }
        public string _35 { get; set; }
        public string _40 { get; set; }
        public string _45 { get; set; }
        public string _50 { get; set; }
        public string _55 { get; set; }
        public string _60 { get; set; }
        public string _65 { get; set; }
        public string _70 { get; set; }
        public string _75 { get; set; }
        public string _80 { get; set; }
        public string _85 { get; set; }
        public string _90 { get; set; }
        public string _95 { get; set; }
        public string _100 { get; set; }
    }

    public class Life
    {
        public string _20 { get; set; }
        public string _50 { get; set; }
        public string _85 { get; set; }
        public string _100 { get; set; }
    }

    public class Closerlook
    {
        public string _6 { get; set; }
        public string _13 { get; set; }
        public string _20 { get; set; }
        public string _26 { get; set; }
        public string _32 { get; set; }
        public string _38 { get; set; }
        public string _44 { get; set; }
        public string _51 { get; set; }
        public string _57 { get; set; }
        public string _64 { get; set; }
        public string _71 { get; set; }
        public string _78 { get; set; }
        public string _88 { get; set; }
        public string _100 { get; set; }
    }
}