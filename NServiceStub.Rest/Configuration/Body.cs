using System;

namespace NServiceStub.Rest.Configuration
{
    public class Body
    {
        public static AsDynamicConfiguration AsDynamic()
        {
            return new AsDynamicConfiguration();
        }
    }

    public class AsDynamicConfiguration
    {
        public LogicalCombinablePredicate IsEqualTo(Func<dynamic, bool> bodyEvaluator)
        {
            return new LogicalCombinablePredicate(new BodyAsDynamicEqualsConfiguration(bodyEvaluator));
        }        
    }
}