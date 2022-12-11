using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Server;
public class Request
{
    public String Type { get; set; }
    public String URL { get; set; }
    public String Host { get; set; }

    // TODO: Maybe Implement?
    //public String Refer { get; set; }

    private Request(string type, string URL, string host)
    {
        this.Type = type;
        this.URL = URL;
        this.Host = host;
    }

    public static Request GetRequest(String request)
    {
        if (String.IsNullOrEmpty(request))
        {
            return null;
        }

        String[] tokens = request.Split(' ');
        String type = tokens[0];
        String url = tokens[1];
        String host = tokens[4];

        return new Request(type, url, host);
    }
}
