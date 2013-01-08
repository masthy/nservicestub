namespace NServiceStub.Rest.Configuration
{
    public class LogicalCombinablePredicate : IRouteInvocationConfiguration
    {
        private IRouteInvocationConfiguration _lastStep;

        public LogicalCombinablePredicate(IRouteInvocationConfiguration lastStep)
        {
            _lastStep = lastStep;
        }

        public IInvocationMatcher CreateInvocationInspector()
        {
            return _lastStep.CreateInvocationInspector();
        }

        public LogicalCombinablePredicate And(IRouteInvocationConfiguration inspection)
        {
            var andPredicate = _lastStep as LogicalAndPredicateConfiguration;

            if (andPredicate != null)
            {
                andPredicate.Add(inspection);
            }
            else
            {
                var orPredicate = _lastStep as LogicalOrPredicateConfiguration;

                if (orPredicate != null)
                {
                    var rightLeg = orPredicate.Right as LogicalAndPredicateConfiguration;

                    if (rightLeg != null)
                        rightLeg.Add(inspection);
                    else
                        orPredicate.Right = new LogicalAndPredicateConfiguration(orPredicate.Right, inspection);                    
                }
                else
                {
                    _lastStep = new LogicalAndPredicateConfiguration(_lastStep, inspection);
                }
            }

            return this;
        }

        public IRouteInvocationConfiguration Or(IRouteInvocationConfiguration inspection)
        {
            _lastStep = new LogicalOrPredicateConfiguration(_lastStep, inspection);

            return this;
        }

    }
}