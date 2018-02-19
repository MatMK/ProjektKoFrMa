using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace KoFrMaDaemon.ConnectionToServer
{
    public class Connection
    {
        static HttpClient client = new HttpClient();
        static async Task<Uri> CreateProductAsync(Tasks task)
        {
            string taskInJSON = "";
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/products", taskInJSON);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
    }
}
