using System.Web.Http;
using Takenet.PerformanceMonitor.WebApi;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(eseva.PeformanceMonitorConfig), "Start")]

namespace eseva 
{
    public static class PeformanceMonitorConfig
	{
		public static void RegisterPerformanceMonitor() 
		{    
            PerformanceCounter.Initialize();
		}

        public static void Start() 
		{
			//You must run Visual Studio as administrator!
            RegisterPerformanceMonitor();
        }
    }
}