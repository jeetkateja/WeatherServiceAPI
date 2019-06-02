using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using prud_WeatherService_BL;
using prud_WeatherService_BL.Entities;

namespace prud_WeatherServiceAPI.Controllers
{
    public class FileController : ApiController
    {
        private readonly IProcessFile _processfile;

        
        public FileController(IProcessFile processfile)
        {
            _processfile = processfile;
            //_processfile = Process;
        }

        public async Task<HttpResponseMessage> GenerateWeatherReportAsync()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            bool reportGen = false;
            if (httpRequest.Files.Count > 0)
            {
                try
                {
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var filePath = HttpContext.Current.Server.MapPath("~/input/" + postedFile.FileName);
                        var outputPath = HttpContext.Current.Server.MapPath("~/output/");
                        postedFile.SaveAs(filePath);


                        reportGen = await _processfile.ProcessFileAsync(filePath, outputPath);
                        
                    }
                }
                catch(Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
                if(reportGen)
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);

            }
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}
