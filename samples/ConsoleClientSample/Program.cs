using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edsoft.Hypermedia.Client;
using Edsoft.Hypermedia.Serializers;
using Newtonsoft.Json;

namespace ConsoleClientSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HypermediaClient(new Uri(args[0]), new HalSerializer());

            var query = client.CreateQuery().WithUrl(args[1]);

            var result = client.ExecuteQueryAsync(query).Result;

            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

            Console.WriteLine("");
            Console.WriteLine("Self Link: " + result.SelfLink);

            Console.ReadKey();
        }
    }
}
