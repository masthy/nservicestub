using System;

namespace NServiceStub.Rest.Configuration
{
    public class ParameterEvaluatedConfiguration<T> : IGetOrPostInvocationConfiguration
    {
        private readonly Func<T, bool> _predicate;
        private readonly string _parameterName;
        private readonly ParameterLocation _parameterLocation;

        public ParameterEvaluatedConfiguration(Func<T, bool> predicate, string parameterName, ParameterLocation parameterLocation)
        {
            _predicate = predicate;
            _parameterName = parameterName;
            _parameterLocation = parameterLocation;
        }

        public IInvocationMatcher CreateInvocationInspector(IGetTemplate routeToConfigure)
        {
            return new ParameterInGetEqualsPredicate<T>(routeToConfigure, _predicate, _parameterLocation, _parameterName);
        }

        IInvocationMatcher IPostInvocationConfiguration.CreateInvocationInspector(IPostTemplate routeToConfigure)
        {
            return new ParameterInPostEqualsPredicate<T>(routeToConfigure, _predicate, _parameterLocation, _parameterName);
        }
    }
}