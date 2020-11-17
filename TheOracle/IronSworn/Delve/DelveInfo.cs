using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheOracle.IronSworn.Delve
{
    public class DelveInfo
    {
        public List<Domain> Domains { get; set; }
        public List<Theme> Themes { get; set; }

        public IEmbed GetEmbed()
        {
            var builder = new EmbedBuilder();

            return builder.Build();
        }

        public IEmbed GetHelper()
        {
            var builder = new EmbedBuilder();

            return builder.Build();
        }

        public static DelveInfo BuildFromEmbed(IEmbed embed)
        {
            return new DelveInfo();
        }
    }
}
