// See https://aka.ms/new-console-template for more information

using Fluid;
using Fluid.ViewEngine;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

var serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
{
    // Intentionally taking this out.. leaving it commented temporarily so that issues related to this can be found/resolved quickly
    //ContractResolver = new IncludeIgnoredAttributesContractResolver(),
    ContractResolver = new CamelCasePropertyNamesContractResolver(),
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    NullValueHandling = NullValueHandling.Include,
    TypeNameHandling = TypeNameHandling.None
});

var options = GetTemplateOptions();

var parser = new FluidViewParser();

var raw = await File.ReadAllTextAsync("raw.liquid");
var template = parser.Parse(raw);

var model = new MyModel(55, "Howdy");

var ctx = new TemplateContext(JObject.FromObject(model, serializer), options);

Console.WriteLine(await template.RenderAsync(ctx));

Console.ReadLine();

static TemplateOptions GetTemplateOptions()
{
    var options = TemplateOptions.Default;
    options.ValueConverters.Add(value => value is JValue { Type: JTokenType.Boolean } v ? (bool?)v.Value : null);
    options.MemberAccessStrategy.IgnoreCasing = true;
    options.FileProvider = new FileProviderMapper(new PhysicalFileProvider(AppContext.BaseDirectory), "");
    return options;
}

record MyModel(int PropertyA, string PropertyB);