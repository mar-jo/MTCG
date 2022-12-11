using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Server;
public class Response
{
    private Byte[] data = null;
    private String _status;
    private String _mime;

    private Response(String status, String mime, byte[] data)
    {
        this._status = status;
        this._mime = mime;
        this.data = data;
    }

    public static Response From(Request request)
    {
        if (request == null)
        {
            // TODO: Implement seperate function + .html
            Debug.WriteLine("[!] Error : 400 Bad Request!");
        }
        
        if (request.Type == "GET")
        {
            String file = HTTPServer.WEB_DIR + request.URL;
            FileInfo info = new FileInfo(file);

            if (info.Exists && info.Extension.Contains("."))
            {
                return MakeFromFile(info);
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(file + "/");

                if (!dir.Exists)
                {
                    Debug.WriteLine("[!] Error : 404 Page Not Found!");
                }

                FileInfo[] files = dir.GetFiles();

                foreach (FileInfo ff in files)
                {
                    if (ff.Name.Contains("default.html") || ff.Name.Contains("index.html"))
                    {
                        return MakeFromFile(ff);
                    }
                }
            }
        }
        else
        {
            // TODO: Implement seperate function + .html
            Debug.WriteLine("[!] Error : 405 Method Not Allowed!");
        }

        // TODO: Implement seperate function + .html
        Debug.WriteLine("[!] Error : 400 Bad Request!");
        return null;
    }

    private static Response MakeFromFile(FileInfo f)
    {
        FileStream fs = f.OpenRead();
        BinaryReader reader = new BinaryReader(fs);
        Byte[] d = new Byte[fs.Length];
        reader.Read(d, 0, d.Length);
        fs.Close();

        return new Response("200 OK", "html/text", d);
    }

    public void Post(NetworkStream stream)
    {
        StreamWriter writer = new StreamWriter(stream);
        writer.WriteLine(String.Format("{0} {1}\r\nServer: {2}\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContent-Length: {4}\r\n", 
            HTTPServer.VERSION, _status, HTTPServer.NAME, _mime, data.Length));

        stream.Write(data, 0, data.Length);
    }
}
