using System.Diagnostics;
using SqlTableToClassGenerator;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Beginning to generate commands");

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.local.json", true)
    .AddCommandLine(args)
    .Build();

var connectionString = config.GetConnectionString("Main");
var targetPath = config["TargetPath"];
var desiredNamespace = config["Namespace"];
var enableForDotNetStandard2 = config["EnableForDotNetStandard2"].ToNullableBool();


var ignoredSchemas = config["IgnoredSchemas"]?.Split("|", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
var classPostfix = config["ClassPostfix"];
var classPrefix = config["ClassPrefix"];

if (!connectionString.HasContent())
{
    Console.Write(StaticStrings.ConnectionStringMessage);
    connectionString = Console.ReadLine();
}

if (!targetPath.HasContent())
{
    Console.Write(StaticStrings.TargetPathMessage);
    targetPath = Console.ReadLine();
}

if (!desiredNamespace.HasContent())
{
    Console.Write(StaticStrings.NamespaceMessage);
    desiredNamespace = Console.ReadLine();
}

while (!enableForDotNetStandard2.HasValue)
{
    Console.Write(StaticStrings.DotNetStandardMessage);
    enableForDotNetStandard2 = Console.ReadLine().ToBool();
}

Argue.HasContent(connectionString);
Argue.HasContent(targetPath);
Argue.HasContent(desiredNamespace);

Console.WriteLine("IgnoredSchemas: {0}", string.Join(", ", ignoredSchemas));
Console.WriteLine("Class Prefix: {0}", classPrefix);
Console.WriteLine("Class Postfix: {0}", classPostfix);

var sw = new Stopwatch();
sw.Start();
await SqlTableGenerator.GenerateTableClasses(connectionString!, targetPath!, 
    desiredNamespace!, enableForDotNetStandard2.Value, classPrefix,
    classPostfix, ignoredSchemas);
sw.Stop();

Console.WriteLine(StaticStrings.FinishMessage, sw.ElapsedMilliseconds);
