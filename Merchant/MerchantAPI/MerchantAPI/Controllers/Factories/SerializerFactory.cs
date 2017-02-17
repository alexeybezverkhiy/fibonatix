using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Web;
using Newtonsoft.Json;

namespace MerchantAPI.Controllers.Factories
{
    public class SerializerFactory
    {
        public static JsonSerializerSettings CreateJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
            };
        }
    }
}