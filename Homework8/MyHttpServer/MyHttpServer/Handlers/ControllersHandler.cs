﻿using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using HttpServer.Attributes;
using Newtonsoft.Json;

namespace HttpServer.Handlers;

public class ControllersHandler : Handler
{
    public override void HandleRequest(HttpListenerContext context)
    {
        try
        {
            var request = context.Request;
            using var response = context.Response;

            var strParams = request.Url!
                .Segments
                .Skip(1)
                .Select(s => s.Replace("/", ""))
                .ToArray();
        
            var controllerName = strParams[0];
            var methodName = strParams[1];
            var id = strParams.Length >= 3 ? strParams[2] : null;
            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(ControllerAttribute)))
                .FirstOrDefault(c =>
                    ((ControllerAttribute)Attribute.GetCustomAttribute(c, typeof(ControllerAttribute))!)
                    .ControllerName.Equals(controllerName, StringComparison.OrdinalIgnoreCase));

            var method = controller?.GetMethods()
                .FirstOrDefault(x => x.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().Name.Equals($"{request.HttpMethod}Attribute",
                                     StringComparison.OrdinalIgnoreCase) &&
                                 ((HttpMethodAttribute)attr).ActionName.Equals(methodName,
                                     StringComparison.OrdinalIgnoreCase)));

            var queryParams = method?.GetParameters()
                .Select((p, i) =>
                {
                    if (p.ParameterType == typeof(uint) && i == 0)
                    {
                        return Convert.ChangeType(id, p.ParameterType);
                    }
                    return Convert.ChangeType(strParams[i], p.ParameterType);
                })
                .ToArray();

            
            var query = request.QueryString;
            var idFromQuery = query["id"];
            
            if (request is { HttpMethod: "POST", HasEntityBody: true } && 
                methodName.Equals("SendToEmail", StringComparison.OrdinalIgnoreCase))
            {
                var encoding = request.ContentEncoding;
                var reader = new StreamReader(request.InputStream, encoding);

                var parsedData = HttpUtility.ParseQueryString(reader.ReadToEnd());
                var email = parsedData["email"];
                var password = parsedData["password"];
                
                var resultFromMethod = method?.Invoke(Activator.CreateInstance(controller!), new object[] {email!, password!});
                ProcessResult(resultFromMethod, response,context);   
            }
            else if (methodName.Equals("add", StringComparison.OrdinalIgnoreCase))
            {
                var login = query["login"];
                var password = query["password"];
                var resultFromMethod = method?.Invoke(Activator.CreateInstance(controller!), new object[] { login!, password! });
                ProcessResult(resultFromMethod, response,context);   
            }
            else if (methodName.Equals("getbyid", StringComparison.OrdinalIgnoreCase))
            {
                var resultFromMethod = method?.Invoke(Activator.CreateInstance(controller!), new object[] { idFromQuery! });
                ProcessResult(resultFromMethod, response,context);   
            }
            else if (methodName.Equals("delete", StringComparison.OrdinalIgnoreCase))
            {
                var resultFromMethod = method?.Invoke(Activator.CreateInstance(controller!), new object[] { idFromQuery! });
                ProcessResult(resultFromMethod, response,context);   
            }
            else if (methodName.Equals("update", StringComparison.OrdinalIgnoreCase))
            {
                var login = query["login"];
                var password = query["password"];
                var resultFromMethod = method?.Invoke(Activator.CreateInstance(controller!), new object[] { login!, password!, idFromQuery! });
                ProcessResult(resultFromMethod, response,context);   
            }
            else
            { 
                var resultFromMethod = method?.Invoke(Activator.CreateInstance(controller!), queryParams);
                ProcessResult(resultFromMethod, response,context);    
            }
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void ProcessResult<T>(T result, HttpListenerResponse response, HttpListenerContext context)
    {
        switch (result)
        {
            case string resultOfString:
            {
                response.ContentType = StaticFilesHandler.GetContentType(context.Request.Url!.LocalPath);
                var buffer = Encoding.UTF8.GetBytes(resultOfString);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                break;
            }
            case not null:
            {
                response.ContentType = StaticFilesHandler.GetContentType(context.Request.Url!.LocalPath);
                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
                var buffer = Encoding.UTF8.GetBytes(json);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                break;
            }
        }
    }
}