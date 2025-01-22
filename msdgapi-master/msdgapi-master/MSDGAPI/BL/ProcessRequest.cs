using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using MSDGAPI.Eseva.BL;
using System.IO;
using IMI.Logger;

namespace MSDGAPI.BL
{
    public class ProcessRequest
    {
        public static object Process(HttpRequestMessage request, Object data)
        {
            object returnObject = new { ResCode = "500", ResDesc = "INTERNAL_ERROR" };
            IEnumerable<string> headerValues = null;

            request.Headers.TryGetValues("Service", out headerValues);
            var assemblyName = headerValues != null ? headerValues.FirstOrDefault().ToLower() : string.Empty;

            request.Headers.TryGetValues("Action", out headerValues);
            var methodName = headerValues != null ? headerValues.FirstOrDefault() : string.Empty;

            var errorMessage = Validations.Validations.RequestDataValidation(assemblyName, methodName, data);
            if(!string.IsNullOrEmpty(errorMessage))
                return new { ResCode = errorMessage == "INTERNAL_ERROR" ? "500" : "400", ResDesc = errorMessage };

            request.Headers.TryGetValues("UserId", out headerValues);
            var userId = headerValues != null ? headerValues.FirstOrDefault() : string.Empty;

            var assemblyInfo = CacheConfigXml.GetAssembDetails(assemblyName, methodName);
            if (assemblyInfo == null || string.IsNullOrEmpty(assemblyInfo.MethodName))
                return new { ResCode = "503", ResDesc = "Service Not Found" };

            var assemblyPath = assemblyInfo.AssemblyPath + assemblyName + ".dll";

            MethodInfo methodInfo = null;
            ConstructorInfo consInfo = null;
            object responder = null;

            try
            {
                //Load the assembly and get it's information
                var type = System.Reflection.Assembly.LoadFrom(assemblyPath).GetType(assemblyInfo.ClassName);
                methodInfo = type.GetMethod(assemblyInfo.MethodName); //Get the reference of the method

                try
                {
                    consInfo = type.GetConstructor(Type.EmptyTypes);
                    responder = consInfo.Invoke(null);

                    if (responder != null)
                        returnObject = methodInfo.Invoke(responder, new object[] { data }); //Invoke the method
                }
                catch { }                
            }
            catch (FileLoadException ex)
            {
                LogData.Write("MSDGAPI", "MSDGAPI", LogMode.Excep, ex, string.Format("ProcessRequest => Process- Ex:{0}", ex.Message));
                return new { ResCode = "503", ResDesc = "Service Not Found" };
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "MSDGAPI", LogMode.Excep, ex, string.Format("ProcessRequest => Process- Ex:{0}", ex.Message));
                return new { ResCode = "503", ResDesc = "Service Not Found" };
            }
            finally
            {
                methodInfo = null;
                consInfo = null;
                responder = null;
            }

            return returnObject;
        }
    }
}