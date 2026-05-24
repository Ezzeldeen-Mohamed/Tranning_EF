using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterManagement.Application.Resources
{
    public interface ILocalizationService
    {
        string GetString(string key);
    }
}
