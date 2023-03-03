using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.GameInterfaces;
using TheOracle2.Data;

namespace Server.Data.homebrew
{
    public class GameContentSet
    {
        public ICollection<HomebrewAsset> Assets { get; set; }
        public ICollection<Oracle> Oracles { get; set; }
    }
}
