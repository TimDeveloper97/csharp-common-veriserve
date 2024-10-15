using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var a = new A()
{
    A1 = new Dictionary<string, dynamic>()
};
a.A1.Add("Key1", new Dictionary<string, string>
{
    { "KKey1","KValue1" },
    { "KKey2","KValue2" },
});
var b = new Dictionary<string, string> { { "HValue1", "HValue2" } };
a.A1.Add("Key2", new Dictionary<string, Dictionary<string, string>>
{
    { "HKey1", b},
    { "HKey2", b},
});

var json = JsonConvert.SerializeObject(a);

var output = JsonConvert.DeserializeObject<A>(json);

var first = output.A1.FirstOrDefault();
var second = output.A1.Skip(1).FirstOrDefault();

Console.WriteLine("Hello, World!");

class A
{
    [JsonConverter(typeof(DynamicConverter))]
    public Dictionary<string, dynamic> A1 { get; set; }
}

public class DynamicConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
        => typeof(object) == objectType;

    public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        if (token is not null && token.Type == JTokenType.Object)
        {
            var result = new Dictionary<string, object>();

            foreach (var prop in token.Children<JProperty>())
            {
                result[prop.Name] = prop.Value.Type == JTokenType.Object
                    ? ProcessNestedObject(prop.Value) : prop.Value.ToString();
            }

            return result;
        }

        return token?.ToObject<object>();
    }

    private object ProcessNestedObject(JToken token)
    {
        var result = new Dictionary<string, object>();

        foreach (var prop in token.Children<JProperty>())
        {
            result[prop.Name] = prop.Value.Type == JTokenType.Object
                ? ProcessNestedObject(prop.Value) : prop.Value.ToString();
        }

        return result;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        => JToken.FromObject(value).WriteTo(writer);
}
