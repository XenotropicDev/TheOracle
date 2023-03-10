using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.Data.homebrew;
using TheOracle2.Data;

namespace TheOracle2;

public record Player
{
    [Key]
    public ulong DiscordId { get; set; }
    public IronGame Game { get; set; }
    public bool SubsPerServer { get; set; }
    public List<GameContentSet> GameDataSets { get; set; }

}
