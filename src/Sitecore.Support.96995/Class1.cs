using Sitecore.Diagnostics;
using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Extensions;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.IO;
using System.Linq;


namespace Sitecore.Support.Mvc.Pipelines.HttpRequest
{
    [UsedImplicitly]
    public class TransferMvcLayout : HttpRequestProcessor
    {
        private static readonly string[] externalPathSchemes = new string[]
        {
            "http://",
            "https://"
        };

        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            string filePath = Context.Page.FilePath;
            if (!Sitecore.Mvc.Extensions.StringExtensions.IsWhiteSpaceOrNull(filePath) && !TransferMvcLayout.externalPathSchemes.Any((string p) => filePath.StartsWith(p)))
            {
                string text;
                try
                {
                    text = Path.GetExtension(filePath);
                }
                catch (ArgumentException)
                {
                    text = string.Empty;
                }
                if (MvcSettings.IsViewExtension(text) || !Sitecore.Mvc.Extensions.StringExtensions.IsAbsoluteViewPath(filePath))
                {
                    Tracer.Info("MVC Layout detected - transfering to ASP.NET MVC");
                    args.Context.Items["sc::IsContentUrl"] = "true";
                    args.AbortPipeline();
                }
            }
        }
    }
}