using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using Newtonsoft.Json;

namespace prud_WeatherService_BL
{
    public class ProcessTextFile : IProcessFile
    {
        private string rootdir = "";
        private string rootOutputDir = "";
        private bool success = false;

        public async Task<bool> ProcessFileAsync(string InputPath,string OutputPath)
        {
            rootdir = InputPath;
            rootOutputDir = OutputPath;

            try
            {
                success = await ProcessAndGenerateWeatherReportFromFileAsync(rootdir, rootOutputDir);
            }
            catch(Exception ex)
            {
                success = false;
            }

            return success;

        }
         
        private async Task<bool> ProcessAndGenerateWeatherReportFromFileAsync(string InputPath, string outputPath)
        {
            Dictionary<int, string> dictCityData = new Dictionary<int, string>();
            bool reportGenerated = false;
            
            //Process file and get id,name of city from file
            dictCityData = GetCityIdandNameFromFile(InputPath);
            if (dictCityData.Count > 0)
            {
                //generate report and save in output folder
                reportGenerated = await GenerateReportFromDataAsync(dictCityData, outputPath);
            }
            else
                reportGenerated =  false;
            return reportGenerated;
        }

        private async Task<bool> GenerateReportFromDataAsync(Dictionary<int, string> dictCityData, string outputPath)
        {
            bool success = false;
            var url = ConfigurationManager.AppSettings["weatherapiurl"];
            var appid = ConfigurationManager.AppSettings["appid"];
            int fileCount = 0;

            for (int i = 0; i < dictCityData.Count; i++)
            {
               //generate response for each line
                using (var client = new HttpClient())
                {
                    UriBuilder builder = new UriBuilder(url);
                    builder.Query = "id=" + dictCityData.Keys.ElementAt(i).ToString() + "&appid=" + appid;

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(builder.Uri);

                    if (response.IsSuccessStatusCode)
                    {
                        fileCount++;
                        using(HttpContent content = response.Content)
                        {
                            
                            if (!GenerateOutputFile(dictCityData.Values.ElementAt(i).ToString(), outputPath,response))
                                fileCount--;

                        }
                    }
                } 
            }

            if (fileCount == dictCityData.Count)
                success = true;
            else
                success = false;

            return success;
        }

        private bool GenerateOutputFile(string FileName, string outputPath,HttpResponseMessage response )
        {
            bool sucess = false;
            string responseBody = response.Content.ReadAsStringAsync().Result;

            string FolderName = Path.Combine(outputPath, DateTime.Now.ToShortDateString());
            if (!Directory.Exists(FolderName))
                Directory.CreateDirectory(FolderName);

            string outputfilepath = Path.Combine(FolderName, FileName+".txt");

            if (!File.Exists(outputfilepath))
            {
                File.WriteAllText(outputfilepath, responseBody);
                success = true;
            }
            else
                success = false;

           
            return sucess;
        }

        private static Dictionary<int,string> GetCityIdandNameFromFile(string InputPath)
        {
            Dictionary<int, string> dictCityData = new Dictionary<int, string>();
            if (File.Exists(InputPath))
            {
                using (StreamReader file = new StreamReader(InputPath))
                {
                    int counter = 0;
                    string ln;

                    while ((ln = file.ReadLine()) != null)
                    {
                        Int64 CityId = Convert.ToInt64(ln.Split('=')[0]);
                        string CityName = ln.Split('=')[1];
                        dictCityData.Add(Convert.ToInt32(CityId), CityName);
                        counter++;
                    }
                    file.Close();
                }
            }
            return dictCityData;
        }
    }
}
