using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NServiceStub.WCF
{
    public class MapInputArgumentHeuristic
    {
        readonly List<int> _indexOfRequiredArgumentVsIndexOfMethodParameter = new List<int>();

        public MapInputArgumentHeuristic(MethodInfo source, Delegate destination, int skipNumberOfDestinationArguments = 0)
        {
            ParameterInfo[] methodParameters = source.GetParameters();
            int indexOfLastScannedInputArgument = -1;

            foreach (var requiredArgument in destination.Method.GetParameters().Skip(skipNumberOfDestinationArguments).Select(param => param.ParameterType))
            {
                bool foundArgument = false;

                while (!foundArgument && ++indexOfLastScannedInputArgument < methodParameters.Length)
                {
                    if (requiredArgument == methodParameters[indexOfLastScannedInputArgument].ParameterType)
                    {
                        _indexOfRequiredArgumentVsIndexOfMethodParameter.Add(indexOfLastScannedInputArgument);
                        foundArgument = true;
                    }
                }

                if (!foundArgument)
                    throw new ArgumentException("Can not resolve the required arguments from the provided method");
            }
        }

        public IEnumerable<object> Map(object[] inputArguments)
        {
            return _indexOfRequiredArgumentVsIndexOfMethodParameter.Select(index => inputArguments[index]);
        }
    }
}