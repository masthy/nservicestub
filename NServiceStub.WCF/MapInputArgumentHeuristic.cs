using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NServiceStub.WCF
{
    public class MapInputArgumentHeuristic
    {
        readonly List<int> _map = new List<int>();

        public MapInputArgumentHeuristic(MethodInfo source, Delegate destination, int skipNumberOfDestinationArguments = 0)
        {
            ParameterInfo[] sourceParameters = source.GetParameters();

            ParameterInfo[] destinationParametersToMap = destination.Method.GetParameters().Skip(skipNumberOfDestinationArguments).ToArray();
            if (destinationParametersToMap.Length == 0)
                return;

            var map = new List<int>();
            bool mappedSuccessfullyByName = MapByArgumentName(sourceParameters, destinationParametersToMap[0], map);

            if (mappedSuccessfullyByName)
            {
                foreach (var argument in destinationParametersToMap.Skip(1))
                {
                    mappedSuccessfullyByName &= MapByArgumentName(sourceParameters, argument, map);
                }
            }

            if (!mappedSuccessfullyByName)
            {
                map.Clear();
                MapByPositionAndType(sourceParameters, destinationParametersToMap, map);
            }

            _map = map;
        }

        private static void MapByPositionAndType(ParameterInfo[] sourceParameters, IEnumerable<ParameterInfo> destinationParametersToMap, List<int> map)
        {
            int indexOfLastScannedInputArgument = -1;

            foreach (var requiredArgument in destinationParametersToMap.Select(param => param.ParameterType))
            {
                bool foundArgument = false;

                while (!foundArgument && ++indexOfLastScannedInputArgument < sourceParameters.Length)
                {
                    if (requiredArgument == sourceParameters[indexOfLastScannedInputArgument].ParameterType)
                    {
                        map.Add(indexOfLastScannedInputArgument);
                        foundArgument = true;
                    }
                }

                if (!foundArgument)
                    throw new ArgumentException("Can not resolve the required arguments from the provided method");
            }
        }

        private static bool MapByArgumentName(ParameterInfo[] sourceParameters, ParameterInfo destinationParameter, List<int> map)
        {
            for (int sourceParameterIndex = 0; sourceParameterIndex < sourceParameters.Length; sourceParameterIndex++)
            {
                if (sourceParameters[sourceParameterIndex].Name == destinationParameter.Name)
                {
                    map.Add(sourceParameterIndex);
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<object> Map(object[] inputArguments)
        {
            return _map.Select(index => inputArguments[index]);
        }
    }
}