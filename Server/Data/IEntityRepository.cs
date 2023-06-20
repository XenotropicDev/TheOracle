using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.GameInterfaces;
using Dataforged;

namespace Server.Data;

public interface IEntityRepository
{
    IEnumerable<OracleGameEntity> GetEntities();
    OracleGameEntity? GetEntity(string id);
}

public class JsonEntityRepository : IEntityRepository
{
    private List<OracleGameEntity>? Entities;

    public OracleGameEntity? GetEntity(string id)
    {
        return GetEntities().FirstOrDefault(o => o.Id == id);
    }

    public IEnumerable<OracleGameEntity> GetEntities()
    {
        if (Entities == null)
        {
            Entities = new List<OracleGameEntity>();
            var files = new DirectoryInfo(Path.Combine("Data", "game entities")).GetFiles("*.json").ToList();

            foreach (var file in files)
            {
                using var fileStream = file.OpenText();
                string text = fileStream.ReadToEnd();

                var root = JsonConvert.DeserializeObject<List<OracleGameEntity>>(text, new JsonSerializerSettings() { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });

                if (root != null) Entities.AddRange(root);
            }
        }

        return Entities;
    }
}
