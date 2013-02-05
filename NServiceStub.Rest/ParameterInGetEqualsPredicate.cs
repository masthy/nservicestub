﻿using System;
using System.Net;

namespace NServiceStub.Rest
{
    public class ParameterInGetEqualsPredicate<T> : IInvocationMatcher
    {
        private readonly IGetTemplate _routeOwningUrl;
        private readonly Func<T, bool> _predicate;
        private readonly ParameterLocation _parameterLocation;
        private readonly string _parameterName;

        public ParameterInGetEqualsPredicate(IGetTemplate routeOwningUrl, Func<T, bool> predicate, ParameterLocation parameterLocation, string parameterName)
        {
            _routeOwningUrl = routeOwningUrl;
            _predicate = predicate;
            _parameterLocation = parameterLocation;
            _parameterName = parameterName;
        }

        public bool Matches(RequestWrapper request)
        {
            var parameterValue = _routeOwningUrl.Route.GetParameterValue<T>(request.Request, _parameterName, _parameterLocation);

            return _predicate(parameterValue);
        }
    }
}