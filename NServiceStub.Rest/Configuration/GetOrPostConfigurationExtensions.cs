namespace NServiceStub.Rest.Configuration
{
    public static class GetOrPostConfigurationExtensions
    {
         public static IGetInvocationConfiguration AsGetConfiguration(this IGetOrPostInvocationConfiguration configuration)
         {
             return configuration;
         }

         public static IPostInvocationConfiguration AsPostConfiguration(this IGetOrPostInvocationConfiguration configuration)
         {
             return configuration;
         }

    }
}