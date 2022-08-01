// See https://aka.ms/new-console-template for more information
using System.Text;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

var files = new DirectoryInfo("Schemas").GetFiles("*.json");
if (!Directory.Exists("Classes")) Directory.CreateDirectory("Classes");

var settings = new CSharpGeneratorSettings() 
{ 
    Namespace = "TheOracle",
    HandleReferences = true,
};
var errors = new StringBuilder();

foreach(var file in files)
{
    try
    {
        var schema = await JsonSchema.FromJsonAsync(file.OpenText().ReadToEnd());
        var gen = new CSharpGenerator(schema, settings);
        var classText = gen.GenerateFile();

        await File.WriteAllTextAsync(Path.Combine("Classes", file.Name.Replace(file.Extension, "") + ".cs"), classText);
    }
    catch (Exception ex)
    {
        Console.WriteLine(file.FullName + "\n" + ex.ToString());
        errors.AppendLine(file.FullName).AppendLine(ex.ToString());
    }
}

if (errors.Length > 0) await File.WriteAllTextAsync(Path.Combine("Classes", "errors.txt"), errors.ToString());
