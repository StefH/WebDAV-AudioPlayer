using System;

namespace BlazorWebDavFunctionsApp.Utils
{
    public static class AzureFunctionUtils
    {
        public static bool IsAzureEnvironment()
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
        }

        // https://github.com/Azure/azure-webjobs-sdk/issues/1817
        public static string GetAzureWebJobsScriptRoot()
        {
            return Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot");
        }
    }
}