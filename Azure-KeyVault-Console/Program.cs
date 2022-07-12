namespace Microshaoft
{
    using System;
    using Azure.Core;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    class Program
    {
        static void Main(string[] args)
        {
            string secretName = "microshaoft-secret-002";
            string keyVaultName = "microshaoft-keyvault-001";
            var kvUri = $"https://{keyVaultName}.vault.azure.net/";
            SecretClientOptions options = new SecretClientOptions()
            {
                Retry =
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                 }
            };

            DefaultAzureCredential credential = new DefaultAzureCredential
                                                    (
                                                        //true
                                                    );
            var client = new SecretClient(new Uri(kvUri), credential, options);
            var input = string.Empty;
            var i = 0;
            do
            {
                if (i > 0)
                {
                    Console.Write($"Setting a secret in {keyVaultName} called '{secretName}' with the value '{input}' ...");
                    client.SetSecret(secretName, input);
                    Console.WriteLine(" done.");
                }
                KeyVaultSecret secret = client.GetSecret(secretName);
                Console.WriteLine($"Retrieving your secret from {keyVaultName}.{secretName}.value:{secret.Value}");
                Console.Write("press q to exit or Input the new value of your secret > ");
                i ++;
            }
            while ((input = Console.ReadLine()) != "q");
            Console.Write("Deleting your secret from " + keyVaultName + " ...");

            client.StartDeleteSecret(secretName);

            //System.Threading.Thread.Sleep(5000);
            Console.WriteLine(" bye.");

        }
    }
}