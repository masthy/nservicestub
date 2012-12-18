using System;

namespace NServiceStub.Rest.Configuration
{
    public class ParameterConfiguration<T>
    {
        private readonly ParameterLocation _parameterLocation;
        private readonly string _parameterName;

        public ParameterConfiguration(ParameterLocation parameterLocation, string parameterName)
        {
            _parameterLocation = parameterLocation;
            _parameterName = parameterName;
        }

        public LogicalCombinablePredicate Equals(T value)
        {
            return new LogicalCombinablePredicate(new ParameterEqualsValueConfiguration<T>(value, _parameterName, _parameterLocation));
        }

        public LogicalCombinablePredicate Equals(Func<T, bool> predicate)
        {
            return new LogicalCombinablePredicate(new ParameterEvaluatedConfiguration<T>(predicate, _parameterName, _parameterLocation));
        }

        public LogicalCombinablePredicate Any()
        {
            return new LogicalCombinablePredicate(new ParameterEvaluatedConfiguration<T>(value => true, _parameterName, _parameterLocation));
        }
    }
}