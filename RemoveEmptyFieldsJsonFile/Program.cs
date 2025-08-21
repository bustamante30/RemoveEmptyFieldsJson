using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

class MainClass
{
    static async Task Main()
    {
        HttpClient client = new HttpClient();
        string s = await client.GetStringAsync("https://coderbyte.com/api/challenges/json/json-cleaning");
        ExpandoObject expandoObject = JsonConvert.DeserializeObject<ExpandoObject>(
            s, 
            new ExpandoObjectConverter()
        );
        var expandoDict = (IDictionary<string, object>)expandoObject;
        RemoveEmptyFields(expandoDict);
        var result = JsonConvert.SerializeObject(expandoObject);

        Console.WriteLine(result);
    }

    private static void RemoveEmptyFields(IDictionary<string, object>? expandoDict)
    {
        var innerDict = (IDictionary<string, object>)expandoDict;
        var keysToRemove = new List<string>();
        foreach (var innerKvp in innerDict)
        {
            if (innerKvp.Value is ExpandoObject innerEo)
            {
                RemoveEmptyFields(innerEo);
            }
            if (innerKvp.Value == null || (innerKvp.Value is string xx && (string.IsNullOrEmpty(xx)|| xx == "N/A")))
            {
                keysToRemove.Add(innerKvp.Key);
            }
            if (innerKvp.Value is List<object> list)
            {
                list.RemoveAll(item => item == null || (item is string str && (string.IsNullOrEmpty(str)|| str == "N/A")));
            }
        }
        foreach (var key in keysToRemove)
        {
            innerDict.Remove(key);
        }
    }
}