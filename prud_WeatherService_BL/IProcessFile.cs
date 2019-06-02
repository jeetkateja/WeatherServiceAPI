using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prud_WeatherService_BL
{
    public interface IProcessFile
    {
        Task<bool> ProcessFileAsync(string InputPath,string OutputPath);
    }
}
