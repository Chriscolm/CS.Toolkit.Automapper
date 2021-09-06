using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS.Toolkit.Generator
{
    public class AutomapperBuilder
    {
        private readonly AutomapperSyntaxReceiver _syntaxReceiver;

        public AutomapperBuilder(AutomapperSyntaxReceiver syntaxReceiver)
        {
            _syntaxReceiver = syntaxReceiver;
        }

        /// <summary>
        /// Erzeugt die erforderlichen Methoden. 
        /// Gibt den Aufruf als Schlüssel und die Methode selbst als Wert zurück
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> Build()
        {
            var result = new Dictionary<string, string>();

            var simple = BuildSimple();
            result.Add(simple.methodCall, simple.methodContent);

            foreach(var pair in _syntaxReceiver.ConversionMap)
            {
                var conversion = BuildWithConverter(pair.Key, pair.Value.targetPropertyName);
                result.Add(conversion.methodCall, conversion.methodContent);
            }

            return result;
        }

        private (string methodCall, string methodContent) BuildSimple()
        {
            var name = "MapSimple";
            var sourceArgName = "source";
            var targetArgName = "target";
            var call = $"{name}({sourceArgName}, {targetArgName});";
            var simpleMappingBegin = BeginMethod(name, "void", sourceArgName, targetArgName);
            var body = WriteBody(sourceArgName, targetArgName);
            var simpleMappingEnd = EndMethod();
            return (methodCall: call, methodContent: $"{simpleMappingBegin}{body}{simpleMappingEnd}");
        }

        private (string methodCall, string methodContent) BuildWithConverter(string sourcePropertyName, string targetPropertyName)
        {
            var name = $"Map{sourcePropertyName}PropertyWithConverter";
            var sourceArgName = "source";
            var targetArgName = "target";
            var call = $"{name}({sourceArgName}, {targetArgName});";
            var conversionMappingBegin = BeginMethod(name, "void", sourceArgName, targetArgName);
            var body = WriteConversionBody(sourceArgName, sourcePropertyName, targetArgName, targetPropertyName);
            var conversionMappingEnd = EndMethod();
            return (methodCall: call, methodContent: $"{conversionMappingBegin}{body}{conversionMappingEnd}");
        }

        private string BeginMethod(string name, string returnType, string sourceArgument, string targetArgument)
        {
            var sourceType = _syntaxReceiver.SourceTypeName;
            var targetType = _syntaxReceiver.TargetTypeName;
            var s = $@"
private {returnType} {name}({sourceType} {sourceArgument}, {targetType} {targetArgument})
{{
";
            return s;
        }

        private string WriteBody(string sourceArgName, string targetArgName)
        {            
            var map = _syntaxReceiver.PropertyMap;
            var sb = new StringBuilder();
            foreach(var pair in map)
            {
                var line = $"{targetArgName}.{pair.Value} = {sourceArgName}.{pair.Key};";
                sb.AppendLine(line);
            }
            return sb.ToString();
        }

        private string WriteConversionBody(string sourceArgName, string sourcePropertyName, string targetArgName, string targetPropertyName)
        {
            var map = _syntaxReceiver.ConversionMap[sourcePropertyName];
            var sb = new StringBuilder();

            var converterCtorLine = $"var converter = new {map.typeName}();";             
            var converterLine = $"var convertedValue = converter.Convert({sourceArgName}.{sourcePropertyName});";
            var typeCheckLine = $"if(convertedValue is {map.targetPropertyTypeName} typedValue)";
            var open = "{";
            var mappingLine = $"{targetArgName}.{targetPropertyName} = typedValue;";
            var close = "}";

            sb.AppendLine(converterCtorLine);
            sb.AppendLine(converterLine);
            sb.AppendLine(typeCheckLine);
            sb.AppendLine(open);            
            sb.AppendLine(mappingLine);
            sb.AppendLine(close);

            return sb.ToString();
        }

        private static string EndMethod()
        {
            var s = $@"
}}
";
            return s;
        }
    }
}
