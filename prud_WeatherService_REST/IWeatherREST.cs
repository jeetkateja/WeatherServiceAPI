using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;

namespace prud_WeatherService_REST
{
    interface IWeatherREST
    {
        Json GetWeatherReport();
    }
}
