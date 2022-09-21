using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Finbourne.Notifications.Sdk.Client;

namespace Finbourne.Notifications.Sdk.Extensions
{
    /// <summary>
    /// Factory to provide instances of the autogenerated Api
    /// </summary>
    public interface IApiFactory
    {
        /// <summary>
        /// Return the specific autogenerated Api
        /// </summary>
        TApi Api<TApi>() where TApi : class, IApiAccessor;
    }

    /// <inheritdoc />
    public class ApiFactory : IApiFactory
    {
        private static readonly IEnumerable<Type> ApiTypes = Assembly.GetAssembly(typeof(ApiClient))
            .GetTypes()
            .Where(t => typeof(IApiAccessor).IsAssignableFrom(t) && t.IsClass);

        private readonly IReadOnlyDictionary<Type, IApiAccessor> _apis;

        /// <summary>
        /// Create a new factory using the specified configuration
        /// </summary>
        /// <param name="apiConfiguration">Configuration for the ClientCredentialsFlowTokenProvider, usually sourced from a "secrets.json" file</param>
        public ApiFactory(ApiConfiguration apiConfiguration)
        {
            if (apiConfiguration == null) throw new ArgumentNullException(nameof(apiConfiguration));

            // Validate Uris
            // note: could employ a factory pattern here to create ITokenProvider in case more branching is required in the future:
            ITokenProvider tokenProvider;
            if (!string.IsNullOrWhiteSpace(apiConfiguration.PersonalAccessToken)) // the personal access token takes precedence over other methods of authentication
            {
                tokenProvider = new PersonalAccessTokenProvider(apiConfiguration.PersonalAccessToken);
            }
            else {
                if (!Uri.TryCreate(apiConfiguration.TokenUrl, UriKind.Absolute, out var _))
                {
                    throw new UriFormatException($"Invalid Token Uri: {apiConfiguration.TokenUrl}");
                }
                tokenProvider = new ClientCredentialsFlowTokenProvider(apiConfiguration); 
            }

            if (!Uri.TryCreate(apiConfiguration.NotificationsUrl, UriKind.Absolute, out var _))
            {
                if (string.IsNullOrWhiteSpace(apiConfiguration.NotificationsUrl))
                    throw new ArgumentNullException(
                        nameof(apiConfiguration.NotificationsUrl),
                        $"Notifications Uri missing. Please specify either FBN_NOTIFICATIONS_API_URL environment variable or notificationsUrl in secrets.json.");

                throw new UriFormatException($"Invalid Uri: {apiConfiguration.NotificationsUrl}");
            }

            // Create configuration
            var configuration = new TokenProviderConfiguration(tokenProvider)
            {
                BasePath = apiConfiguration.NotificationsUrl,
            };
            
            configuration.DefaultHeaders.Add("X-LUSID-Application", apiConfiguration.ApplicationName);

            _apis = Init(configuration);
        }

        /// <summary>
        /// Create a new factory using the specified configuration
        /// </summary>
        /// <param name="configuration">A set of configuration settings</param>
        public ApiFactory(Client.Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _apis = Init(configuration);
        }

        /// <inheritdoc />
        public TApi Api<TApi>() where TApi : class, IApiAccessor
        {
            _apis.TryGetValue(typeof(TApi), out var api);

            if (api == null)
            {
                throw new InvalidOperationException($"Unable to find api: {typeof(TApi)}");
            }

            return api as TApi;
        }

        private static Dictionary<Type, IApiAccessor> Init(Client.Configuration configuration)
        {
            // If some retry policy has already been assigned, use it.
            // Users can combine their own policy with the default policy by using the .Wrap() method.
            RetryConfiguration.RetryPolicy =
                RetryConfiguration.RetryPolicy ?? PollyApiRetryHandler.DefaultRetryPolicyWithFallback;

            // If some async retry policy has already been assigned, use it.
            // Users can combine their own policy with the default policy by using the .WrapAsync() method.
            RetryConfiguration.AsyncRetryPolicy =
                RetryConfiguration.AsyncRetryPolicy ?? PollyApiRetryHandler.DefaultRetryPolicyWithFallbackAsync;

            var dict = new Dictionary<Type, IApiAccessor>();
            foreach (Type api in ApiTypes)
            {
                if (!(Activator.CreateInstance(api, configuration) is IApiAccessor impl))
                {
                    throw new Exception($"Unable to create type {api}");
                }

                // Replace the default implementation of the ExceptionFactory with a custom one defined by FINBOURNE
                impl.ExceptionFactory = ExceptionHandler.CustomExceptionFactory;
                var @interface = api.GetInterfaces()
                    .First(i => typeof(IApiAccessor).IsAssignableFrom(i));

                dict[api] = impl;
                dict[@interface] = impl;
            }

            return dict;
        }
    }
}