using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace ParsedCvData.JsonProcessor
{
    public class JsonProcessor
    {
        // Convert JObject to ExpandoObject and return dynamic data
        public dynamic ConvertToDynamic(JObject jsonObject)
        {
            var jsonData = jsonObject.ToString();
            return JsonConvert.DeserializeObject<ExpandoObject>(jsonData, new ExpandoObjectConverter());
        }
        // Extract data from ExpandoObject and return as a list of strings
        public List<string> ExtractData(dynamic dynamicObject)
        {
            var dataList = new List<string>();
            ExtractDataRecursive(dynamicObject, dataList);
            return dataList;
        }
        private void ExtractDataRecursive(dynamic expandoObject, List<string> dataList)
        {
            if (expandoObject is IDictionary<string, object> dictionary)
            {
                foreach (var kvp in dictionary)
                {
                    if (kvp.Value is string strValue)
                    {
                        dataList.Add(strValue);
                    }
                    else
                    {
                        ExtractDataRecursive(kvp.Value, dataList);
                    }
                }
            }
            else if (expandoObject is IEnumerable<object> list)
            {
                foreach (var item in list)
                {
                    ExtractDataRecursive(item, dataList);
                }
            }
        }
    }
}
