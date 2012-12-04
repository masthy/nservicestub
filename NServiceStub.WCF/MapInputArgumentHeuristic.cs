using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NServiceStub.WCF
{
    public class MapInputArgumentHeuristic
    {
        readonly List<int> _indexOfRequiredArgumentVsIndexOfMethodParameter = new List<int>();

        public MapInputArgumentHeuristic(MethodInfo method, IEnumerable<Type> requiredArguments)
        {
            ParameterInfo[] methodParameters = method.GetParameters();
            int indexOfLastScannedInputArgument = -1;

            foreach(var requiredArgument in requiredArguments)
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