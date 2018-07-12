using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Newtonsoft.Json;
using System;

namespace docshost.test
{
    public class Startup
    {
        static HttpClient httpclient = new HttpClient();

        private static readonly string fileFolder = "C:/Users/t-zhiliu/Documents/GitHub/azure-docs-pr/_site/articles";
        private static readonly string branch = "master";
        private static readonly string commit = "0";
        private static readonly string version = "1.0";

        private static readonly string url = "http://localhost:56310/";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
                //await HasBlob();
                //await FindDoc();
                await PutBlob();
                //await PutDoc();
            });
        }

        public static async Task PutDoc()
        {
            List<Document> documents = new List<Document>();
            var allfiles = Directory.GetFiles(fileFolder, "*.json", SearchOption.AllDirectories);

            foreach (var file in allfiles)
            {
                StreamReader sr = new StreamReader(file);
                string srContent = sr.ReadToEnd();
                sr.Close();

                dynamic ob = JsonConvert.DeserializeObject(srContent);
                Document d = new Document
                {
                    Locale = ob.locale ?? "en-us",
                    Url = ob.redirectionUrl ?? "/azure/fake",
                    Version = version,
                    Commit = commit,
                    Blob = "Empty",
                    Id = ob.id ?? null
                };

                documents.Add(d);
            }
            
            HttpResponseMessage responseMessage = await httpclient.PutAsJsonAsync(url + "doc", documents);
        }

        public static async Task FindDoc()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "branch", "master" },
                { "basename", "/azure" },
                { "url", "/azure/azure-resource-manager/resource-group-template-deploy" },
                { "locale", "en-us" },
                { "version", "1.0" }
            };
            
            HttpResponseMessage responseMessage = await httpclient.PostAsJsonAsync(url + "finddoc", dic);
            
        }

        public static async Task PutBlob()
        {
            List<string> contents = new List<string>();
            var allfiles = Directory.GetFiles(fileFolder, "*.json", SearchOption.AllDirectories);

            foreach (var file in allfiles)
            {
                StreamReader sr = new StreamReader(file);
                string srContent = sr.ReadToEnd();
                sr.Close();

                dynamic ob = JsonConvert.DeserializeObject(srContent);
                if (ob["content"] == null)
                {
                    continue;
                }
                string c = ob.content;

                contents.Add(c);
            }

            HttpResponseMessage responseMessage = await httpclient.PutAsJsonAsync(url + "blob", contents);
              
        }

        public static async Task HasBlob()
        {
            string[] hashes = new string[] { "--84MqDZvRtnEyllAQtL2plumzI", "-3LuApQ44GeMKIGRIjBn_-1JZrs" };

            HttpResponseMessage responseMessage = await httpclient.PostAsJsonAsync(url + "hasblob", hashes);
            
        }
    }
}
