using System;
using Microsoft.Graph;
using Azure.Identity;
using System.Threading.Tasks;

namespace CreateServicePrinciple {

    class Program {

        static async Task Main(string[] args) {


            // The client credentials flow requires that you request the
            // /.default scope, and preconfigure your permissions on the
            // app registration in Azure. An administrator must grant consent
            // to those permissions beforehand.
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            var tenantId = "{tenantId}";

            // Values from app registration
            var clientId = "{clientId}";
            var clientSecret = "{clientSecret}";

            // using Azure.Identity;
            var options = new TokenCredentialOptions {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            // https://docs.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);



            Random seed = new Random();
            String displayName = "KTA" + seed.Next().ToString();

            Console.WriteLine("New displayName = " + displayName);

            Application application = new Application
            {
                DisplayName = displayName //"KTA" + seed.Next().ToString()
            };
            await graphClient.Applications
                .Request()
                .AddAsync(application);

            var applications = await graphClient.Applications
                .Request()
                .Filter($"displayName eq '{displayName}'")
                .GetAsync();

            string appID = "";

            foreach(var app in applications)
            {
                Console.WriteLine(app.DisplayName);
                if (app.DisplayName.Equals(displayName)){ 
                    appID = app.AppId;
                } 
            }

            if (appID.Length > 0) {
                ServicePrincipal servicePrincipal = new ServicePrincipal {
                    AppId = appID
                };
                await graphClient.ServicePrincipals
                    .Request()
                    .AddAsync(servicePrincipal);
            }
            

            Console.WriteLine("New AppID = " + appID);
            Console.ReadLine();

        }


    }
}
