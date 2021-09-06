using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CS.Toolkit.Generator
{
    public class AutomapperSyntaxReceiver : ISyntaxContextReceiver
    {
        public string SourceTypeName { get; private set; }
        public string TargetTypeName { get; private set; }
        public Dictionary<string, string> PropertyMap { get; }
        public Dictionary<string, (string typeName, string targetPropertyName, string targetPropertyTypeName)> ConversionMap { get; }

        public AutomapperSyntaxReceiver()
        {            
            PropertyMap = new();
            ConversionMap = new();
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {            
            if (!Debugger.IsAttached)
            {
                // Debugger.Launch();
            }

            var model = context.SemanticModel;
            var node = context.Node;
            if(node is ClassDeclarationSyntax)
            {
                var mapAttributeSymbol = model.Compilation.GetTypeByMetadataName("CS.Toolkit.Automapper.Contracts.Attributes.AutoMappingAttribute");
                var symbol = model.GetDeclaredSymbol(node) as INamedTypeSymbol;
                if(symbol is not null)
                {                    
                    foreach(var attribute in symbol.GetAttributes())
                    {
                        if(SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mapAttributeSymbol))
                        {
                            var arr = new string[2];
                            for(var i = 0; i < arr.Length; i++)
                            {
                                var arg = attribute.ConstructorArguments[i];
                                var argValueSymbol = arg.Value as INamedTypeSymbol;
                                var argTypeName = argValueSymbol.Name;

                                CreatePropertyMap(model, argValueSymbol);

                                var n = argTypeName;
                                n = GetFullName(argValueSymbol, n); // Ermittlung des vollständig qualifizierten Typnamens
                                arr[i] = n;
                            }

                            SourceTypeName = arr[0];
                            TargetTypeName = arr[1];                            
                        }
                    }
                }
            }
        }       

        private void CreatePropertyMap(SemanticModel model, INamedTypeSymbol argValueSymbol)
        {
            // Diese Methode wird zweimal aufgerufen. 
            // 1. Analyse des Quellobjektes (das hat die Attribute)
            // 2. Analyse des Zielobjektes (ggf. Hinzufügen von Typinformationen)
            var mapAttributeSymbol = model.Compilation.GetTypeByMetadataName("CS.Toolkit.Automapper.Contracts.Attributes.MappingAttribute");

            var members = argValueSymbol.GetMembers();
            foreach(var member in members)
            {                
                if (member is IPropertySymbol propertySymbol)
                {
                    AddTypeInformation(propertySymbol);
                    var attributes = propertySymbol.GetAttributes();
                    foreach (var attribute in attributes)
                        if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mapAttributeSymbol))
                        {                            
                            var ctorArgs = attribute.ConstructorArguments;
                            var n = propertySymbol.Name;
                            if(ctorArgs.Length < 1)
                            {
                                PropertyMap.Add(n, n);
                            } else if(ctorArgs.Length == 1){
                                AddToMap(n, ctorArgs[0]);                                                          
                            } else if(ctorArgs.Length == 2)
                            {
                                AddToConverterMap(n, ctorArgs[0], ctorArgs[1]);                                
                            }
                        }
                }
            }
        }

        private void AddToMap(string sourcePropertyName, TypedConstant ctorArgument)
        {
            var arg = ctorArgument;
            var v = arg.Value;
            if (v is string s)
            {
                PropertyMap.Add(sourcePropertyName, s);
            }
        }

        private void AddToConverterMap(string sourcePropertyName, TypedConstant ctorArgument, TypedConstant converterArgument)
        {
            var arg0 = ctorArgument;
            var v0 = arg0.Value;
            var arg1 = converterArgument;
            var v1 = arg1.Value;            
            if (v0 is string s && v1 is INamedTypeSymbol converterSymbol)
            {
                var fullConverterType = GetFullName(converterSymbol, converterSymbol.Name);
                ConversionMap.Add(sourcePropertyName, (typeName: fullConverterType, targetPropertyName: s, targetPropertyTypeName: string.Empty));
            }
        }

        private void AddTypeInformation(IPropertySymbol propertySymbol)
        {
            var targetPropertyName = propertySymbol.Name;
            var fromConversionMap = ConversionMap.SingleOrDefault(p => p.Value.targetPropertyName.Equals(targetPropertyName, StringComparison.InvariantCulture));
            var key = fromConversionMap.Key;
            if(key != null)
            {
                var typeName = propertySymbol.Type.Name;
                var conversionMapValue = ConversionMap[key];
                conversionMapValue.targetPropertyTypeName = typeName;
                ConversionMap[key] = conversionMapValue;
            }
        }

        /// <summary>
        /// Ermittelt den vollständig qualifizierten Typnamen
        /// </summary>
        /// <param name="argValueSymbol"></param>
        /// <param name="n">unqualifizierter Typname</param>
        /// <returns></returns>
        private static string GetFullName(INamedTypeSymbol argValueSymbol, string n)
        {
            var p = argValueSymbol.ContainingSymbol;
            while (p != null)
            {
                n = $"{p.MetadataName}.{n}";
                p = p.ContainingSymbol;
                if (p is INamespaceSymbol s && s.IsGlobalNamespace)
                {
                    break;
                }
            }
            return n;
        }
    }
}
