using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2.Data;

namespace Server.Data.homebrew
{
    public class GameContentSet
    {
        public ICollection<Asset> Assets { get; set; }
    }
}
