namespace NServiceStub.Rest.Configuration
{
    public class LogicalCombinablePredicate : IGetOrPostInvocationConfiguration
    {
        private IGetOrPostInvocationConfiguration _lastStep;

        public LogicalCombinablePredicate(IGetOrPostInvocationConfiguration lastStep)
        {
            _lastStep = lastStep;
        }

        IInvocationMatcher IGetInvocationConfiguration.CreateInvocationInspector(IGetTemplate routeToConfigure)
        {
            return _lastStep.CreateInvocationInspector(routeToConfigure);
        }

        IInvocationMatcher IPostInvocationConfiguration.CreateInvocationInspector(IPostTemplate routeToConfigure)
        {
            return _lastStep.CreateInvocationInspector(routeToConfigure);
        }

        public LogicalCombinablePredicate And(IGetOrPostInvocationConfiguration inspection)
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

        public IGetInvocationConfiguration Or(IGetOrPostInvocationConfiguration inspection)
        {
            _lastStep = new LogicalOrPredicateConfiguration(_lastStep, inspection);

            return this;
        }

    }
}