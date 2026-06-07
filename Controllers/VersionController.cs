using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Headlight.Controllers
{

    public class VersionFile
    {
        public string Current { get; set; } = "";
        [NotMapped]
        public string Local { get; set; } = "";
    }

    [Produces("application/json")]
    [Route("api/version")]
    public class VersionController(IConfiguration config) : Controller
    {
        private const string GithubPortal = "https://raw.githubusercontent.com/vladventura/headlight/refs/heads/master/";
        private const string FileName = "version.json";
        private readonly static HttpClient HttpClient = new()
        {
            BaseAddress = new Uri(GithubPortal)
        };

        [HttpGet]
        public async Task<VersionFile> Get()
        {
            VersionFile result = new();
            using HttpResponseMessage response = await HttpClient.GetAsync(FileName);
            if (response.IsSuccessStatusCode)
            {
                VersionFile? jsonResponse = await response.Content.ReadFromJsonAsync<VersionFile>();
                if (jsonResponse != null)
                {
                    float localVersion = float.Parse(config["AppVersion"]?.ToString() ?? "0.0");
                    if (float.TryParse(jsonResponse.Current, out float incomingVersion))
                    {
                        if (incomingVersion > localVersion)
                        {
                            result.Current = jsonResponse.Current;
                            result.Local = localVersion.ToString();
                        }
                    }
                }
            }
            Response.Headers.Append("Cache-Control", "max-age=3600");
            return result;
        }
    }
}
