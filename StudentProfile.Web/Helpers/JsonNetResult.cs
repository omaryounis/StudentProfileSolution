using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Dashboard_StudentProfile.Helpers
{
    //لحل مشكلة التاريخ فى الجافاسكربت
    //https://www.developer.com/net/dealing-with-json-dates-in-asp.net-mvc.html
    public class JsonNetResult : JsonResult
    {
        public object Data { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            if (Data != null)
            {
                JsonTextWriter writer = new JsonTextWriter(response.Output) {Formatting = Formatting.Indented};
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
                serializer.Serialize(writer, Data);
                writer.Flush();
            }
        }
    }
}