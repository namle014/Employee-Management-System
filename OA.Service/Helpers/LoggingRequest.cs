using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace OA.Service.Helpers
{
    public class LoggingRequest
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public LoggingRequest(RequestDelegate next, ILogger<LoggingRequest> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            //First, get the incoming request
            var body = await GetBodyString(context);

            var request = GenerateLogString(context, body);

            //============== Log request =============//
            _logger.LogInformation(request);

            //Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            //Create a new memory stream...
            using (var responseBody = new MemoryStream())
            {
                //...and use that for the temporary response body
                context.Response.Body = responseBody;

                //Continue down the Middleware pipeline, eventually returning to this class
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(new EventId(0), ex, $"Unhandled exception for request {context.TraceIdentifier}");
                    throw;
                }

                //Format the response from the server
                var response = await FormatResponse(context);

                //============== Log response =============//
                _logger.LogInformation(response);

                //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatResponse(HttpContext context)
        {
            //We need to read the response stream from the beginning...
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string body = await new StreamReader(context.Response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)

            var str = new StringBuilder();
            str.AppendLine($"Response log:");
            str.AppendLine($"Response Id: {context.TraceIdentifier}");
            if (body.Contains("href") || body.Contains("swagger - ui") || body.Contains("openapi"))
            {
                //str.AppendLine($"{body}");
            }
            else
            {
                str.AppendLine(body);
                //str.AppendLine($"{JsonConvert.DeserializeObject(body)}");
            }

            return str.ToString();
        }

        private async Task<string> GetBodyString(HttpContext context)
        {
            string body = string.Empty;
            if (context.Request.Headers.ContentLength == 0)
            {
                return body;
            }

            try
            {
                context.Request.EnableBuffering();

                if (context.Request.HasFormContentType)
                {
                    var form = await context.Request.ReadFormAsync();

                    var formData = new List<KeyValuePair<string, string>>();
                    foreach (var data in form)
                    {
                        var value = data.Value.ToString() ?? string.Empty;
                        formData.Add(new KeyValuePair<string, string>(data.Key, value));
                    }


                    body = $"Form data:\r\n{JsonConvert.SerializeObject(formData, Formatting.Indented)}\r\nNumber of upload files: {form.Files.Count}";
                }
                else
                {
                    using (var _bodyStream = new MemoryStream())
                    {
                        var bodyContext = string.Empty;
                        if (context.Request.Method.ToLower() == "get")
                        {
                            bodyContext = context.Request.QueryString.ToString();
                            body = $"QueryString: {bodyContext}";
                        }
                        else
                        {
                            await context.Request.Body.CopyToAsync(_bodyStream);
                            _bodyStream.Seek(0, SeekOrigin.Begin);
                            bodyContext = new StreamReader(_bodyStream).ReadToEnd();
                            body = $"Body: {bodyContext}";
                        }
                    }
                }

                //set position after read
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(0), ex, $"[{DateTime.Now}] Unhandled exception while logging input of request");
            }

            return body;
        }

        private string GenerateLogString(HttpContext context, string body, bool forSendEmail = false)
        {
            if (body.Contains("Password"))
            {
                var index = body.IndexOf("Password");
                char[] ch = body.ToCharArray();
                for (int i = 0; i < ch.Length; i++)
                {
                    if (i > index + 11)
                    {
                        if (ch[i].ToString() == "\"") break;
                        ch[i] = '*';
                    }
                }

                body = new string(ch);
            }
            var str = new StringBuilder();
            if (forSendEmail)
            {
                str.AppendLine($"<div><b>Request log:</b></div>");
                str.AppendLine($"<div><b>Request Id:</b> {context.TraceIdentifier}</div>");
                str.AppendLine($"<div><b>User name:</b> {context.User.FindFirstValue(ClaimTypes.NameIdentifier)}</div>");
                str.AppendLine($"<div><b>User token:</b> {context.Request.Headers["Authorization"]}</div>");
                str.AppendLine($"<div><b>HTTP Method:</b> {context.Request.Method}</div>");
                str.AppendLine($"<div><b>URL:</b> {context.Request.Host}{context.Request.Path}</div>");
                str.AppendLine($"<div>{body}</div>");
            }
            else
            {
                //If you do not take any row? then please comment on that row.
                str.AppendLine($"Request log:");
                str.AppendLine($"Request Id: {context.TraceIdentifier}");
                str.AppendLine($"Request IP: {context.Connection.RemoteIpAddress}");
                str.AppendLine($"User name: {context.User.FindFirstValue(ClaimTypes.NameIdentifier)}");
                str.AppendLine($"User token: {context.Request.Headers["Authorization"]}");
                str.AppendLine($"HTTP Method: {context.Request.Method}");
                str.AppendLine($"URL: {context.Request.Path}");
                str.AppendLine($"{body}");
            }

            return str.ToString();
        }
    }
}
