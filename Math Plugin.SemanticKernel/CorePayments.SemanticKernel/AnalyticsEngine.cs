using CorePayments.Infrastructure.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

using System.Text.Json;
using Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Microsoft.SemanticKernel.Connectors.OpenAI;
namespace CorePayments.SemanticKernel
{
    public class AnalyticsEngine : IAnalyticsEngine
    {
        readonly AnalyticsEngineSettings _settings;

        public AnalyticsEngine(
            IOptions<AnalyticsEngineSettings> settings)
        {
            _settings = settings.Value;
        }


        public async Task<string> ReviewTransactions(IEnumerable<Transaction> transactions, string query)
        {
            //var builder = Kernel.CreateBuilder();

            var folder = RepoFiles.SamplePluginsPath();

            

              var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion
                (deploymentName: _settings.OpenAICompletionsDeployment
                     ,   // The name of your deployment (e.g., "gpt-35-turbo")
               endpoint: _settings.OpenAIEndpoint,         // The endpoint of your Azure OpenAI service
                apiKey: _settings.OpenAIKey )         // The API key of your Azure OpenAI service
                                                     //modelId: AzureOpenAIModelId  // The model ID of your Azure OpenAI service)
                   .Build();




            // Function defined using few-shot design pattern
            string promptTemplate = @"
You are an analyst helps summarize only Math transactions by processing a list of transactions. 
            You are provided the list of transactions in the JSON format, as well as the query submitted by the user.
            You can return your results in JSON or the format specified by the user in the query. 
            
           

            Provide your response by completing the following bullet:
            - Result: 2

            +++

            [INPUT]
            Transaction Data:
            {{$transactionData}}

           
            [END INPUT]

            Provide your response by completing the following bullet on a new line:
            - ResultNew:
            
";

            
       

            builder.ImportPluginFromType<MathPlugin>();
            //builder.ImportPluginFromPromptDirectory(folder, "SummarizePlugin");




            

            var transactionData = JsonSerializer.Serialize(transactions);


          


#pragma warning disable SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var planner = new HandlebarsPlanner
                (
                new HandlebarsPlannerOptions 
                { AllowLoops = true, MaxTokens=2000});
#pragma warning restore SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


          


            // Act
#pragma warning disable SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            promptTemplate =  promptTemplate.Replace("{{$transactionData}}", transactionData);
            var plan = await planner.CreatePlanAsync(builder, promptTemplate +   Environment.NewLine + query);
#pragma warning restore SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            var inputsToPlanner = new Dictionary<string, object?>
            {
                //inputs.Add("query", query);
                { "transactionData", transactionData }
            };
            
            var plannerInputs = new KernelArguments((IDictionary<string, object?>)inputsToPlanner);

#pragma warning disable SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var result = plan.InvokeAsync(builder, plannerInputs);
#pragma warning restore SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            return result.Result;
           
        }
    }
}