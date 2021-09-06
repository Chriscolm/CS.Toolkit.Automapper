using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CS.Toolkit.Generator
{
    [Generator]
    public class AutomapperGenerator : ISourceGenerator
    {
        private readonly List<string> _log = new();

        public void Initialize(GeneratorInitializationContext context)
        {
            if (!Debugger.IsAttached)
            {
                // Debugger.Launch();
            }
            
            _log.Add("Initialize");
            context.RegisterForSyntaxNotifications(() => new AutomapperSyntaxReceiver());
            context.RegisterForPostInitialization(a =>
            {
                _log.Add("RegisterForPostInitialization");
            });
        }

        public void Execute(GeneratorExecutionContext context)
        {            
            if (!Debugger.IsAttached)
            {
                // Debugger.Launch();
            }
            _log.Add($"Time: {DateTime.Now}");
            _log.Add($"SyntaxReceiver is {context.SyntaxReceiver?.GetType()?.Name ?? "null"}");
            _log.Add($"SyntaxContextReceiver is {context.SyntaxContextReceiver?.GetType()?.Name ?? "null"}");

            AutomapperSyntaxReceiver receiver;

            if (context.SyntaxReceiver is AutomapperSyntaxReceiver syntaxReceiver)
            {
                receiver = syntaxReceiver;
                //_log.AddRange(syntaxReceiver.Log);
            }
            else if (context.SyntaxContextReceiver is AutomapperSyntaxReceiver syntaxContextReceiver)
            {
                receiver = syntaxContextReceiver;
                //_log.AddRange(syntaxContextReceiver.Log);
            }
            else
            {
                return;
            }

            var sourceObject = receiver.SourceTypeName; // CS.Toolkit.Automapper.Datamodels.Source
            var targetObject = receiver.TargetTypeName; // CS.Toolkit.Automapper.Datamodels.Target 

            var mappingMethods = GetMappingMethods(receiver);

            var code = $@"
namespace CS.Toolkit.Automapper
{{
    using System;
    using CS.Toolkit.Automapper.Datamodels;
    public partial class ObjectAutomapper
    {{ 
        public object Map(object source)
        {{
            if(source is {sourceObject} o)
            {{
                var res = Map(o);
                return res;
            }}
            throw new System.InvalidOperationException();
        }}

        public {targetObject} Map({sourceObject} source)
        {{
            var target = new {targetObject}();
            {string.Join(Environment.NewLine, mappingMethods.Select(m=>m.Key))}
            return target;
        }}
        
        {string.Join(Environment.NewLine, mappingMethods.Select(m=>m.Value))}
    }}
}}
";
            context.AddSource("ObjectAutomapper.g", SourceText.From(code, Encoding.UTF8));            
            context.AddSource("Logs.Generator.g", SourceText.From($@"/*{ Environment.NewLine + string.Join(Environment.NewLine, _log) + Environment.NewLine}*/", Encoding.UTF8));
        }

        private static Dictionary<string, string> GetMappingMethods(AutomapperSyntaxReceiver receiver)
        {
            var builder = new AutomapperBuilder(receiver);
            var result = builder.Build();
            return result;
        }
    }
}
