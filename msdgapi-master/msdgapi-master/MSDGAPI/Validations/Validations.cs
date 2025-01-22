using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Xml;
using IMI.Logger;
using Newtonsoft.Json;

namespace MSDGAPI.Validations
{
    public class Validations
    {
        #region Private Members

        private static CacheDependency cacheDepConfig;
        private static XmlDocument XmlDoc;

        #endregion Private Members

        #region Public Methods

        public static string RequestDataValidation(string service, string action, object data)
        {
            Dictionary<string, string> requestData = new Dictionary<string, string>();

            try
            {
                LogData.Write("MSDGAPI", "Validation", LogMode.Info, string.Format("Validations- Request Data: {0}", data));
                var dictionaryData = JsonConvert.DeserializeObject<Dictionary<string, object>>(data.ToString());
                foreach (var item in dictionaryData)
                {
                    if (item.Value.GetType().Name == "JObject")
                    {
                        Dictionary<string, string> childDictData = null;
                        try
                        {
                            childDictData = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.Value.ToString());
                        }
                        catch { }

                        if (childDictData == null)
                            return "Invalid request";

                        requestData = requestData.Concat(childDictData).ToDictionary(dk => dk.Key, dv => dv.Value);
                        continue;
                    }

                    requestData.Add(item.Key, item.Value.ToString());
                }
            }
            catch { }

            try
            {
                if (requestData == null || !requestData.Any())
                    return "Invalid request";

                if (string.IsNullOrEmpty(service) || string.IsNullOrEmpty(action))
                    return "Missing request Parameters";

                LoadValidationRules();

                requestData = new Dictionary<string, string>(requestData, StringComparer.OrdinalIgnoreCase);

                var rules = XmlDoc.DocumentElement.SelectNodes(service.ToLower() + "/" + action.ToLower() + "/item");
                foreach (XmlNode rule in rules)
                {
                    if (!requestData.ContainsKey(rule.Attributes["param"].Value))
                        return "Missing request Parameters";

                    if (rule.Attributes["dependon"] != null)
                    {
                        var parentvalue = requestData[rule.Attributes["dependon"].Value];
                        if (!string.IsNullOrEmpty(parentvalue) && rule.Attributes["dependonvalue"].Value.IndexOf("[" + parentvalue.ToLower() + "]") < 0)
                            continue;
                    }

                    var value = requestData[rule.Attributes["param"].Value];
                    if (rule.Attributes["required"] != null && rule.Attributes["required"].Value == "true" && string.IsNullOrEmpty(value))
                        return rule.Attributes["required-message"].Value;

                    if (rule.Attributes["pattern"] != null && !string.IsNullOrEmpty(value))
                    {
                        var result = Regex.Match(value, rule.Attributes["pattern"].Value);
                        if (!result.Success)
                            return rule.Attributes["invalid-message"].Value;
                    }

                    if (rule.Attributes["allowed"] != null && !string.IsNullOrEmpty(value))
                    {
                        if (!rule.Attributes["allowed"].Value.Contains("[" + value.ToLower() + "]"))
                            return rule.Attributes["invalid-message"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "Validation", LogMode.Excep, ex, string.Format("Validations- RequestDataValidation- Ex:{0}", ex.Message));
                return "INTERNAL_ERROR";
            }

            return string.Empty;
        }

        #endregion Public Methods

        #region Private Methods

        private static void LoadValidationRules()
        {
            var validationRulesFilePath = ConfigurationManager.AppSettings["VALIDATION_RULES_XML_PATH"] ?? "";
            try
            {
                if (!string.IsNullOrEmpty(validationRulesFilePath) && File.Exists(validationRulesFilePath))
                {
                    if (cacheDepConfig == null || cacheDepConfig.HasChanged || XmlDoc == null || XmlDoc.DocumentElement == null)
                    {
                        cacheDepConfig = new CacheDependency(validationRulesFilePath, DateTime.Now);
                        XmlDoc = new XmlDocument();
                        var reader = XmlReader.Create(validationRulesFilePath);
                        XmlDoc.Load(reader);
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "Validation", LogMode.Excep, ex, string.Format("Validations- LoadValidationRules- Ex:{0}", ex.Message));
            }
        }

        #endregion Private Methods
    }
}