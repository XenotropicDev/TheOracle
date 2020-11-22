using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheOracle.IronSworn.Delve
{
    public class DelveService
    {
        public List<Theme> Themes { get; set; } = new List<Theme>();
        public List<Domain> Domains { get; set; } = new List<Domain>();

        public static DelveService Load(string[] themeJsonFiles, string[] domainJsonFiles)
        {
            var delveService = new DelveService();
            foreach(var file in themeJsonFiles)
            {
                string json = File.ReadAllText(file);
                delveService.Themes.AddRange(JsonConvert.DeserializeObject<List<Theme>>(json));
            }
            foreach (var file in domainJsonFiles)
            {
                string json = File.ReadAllText(file);
                delveService.Domains.AddRange(JsonConvert.DeserializeObject<List<Domain>>(json));
            }

            return delveService;
        }
    }
}
