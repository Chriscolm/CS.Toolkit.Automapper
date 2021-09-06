using CS.Toolkit.Automapper.Contracts;
using CS.Toolkit.Automapper.Contracts.Attributes;
using CS.Toolkit.Automapper.Datamodels;
using System;

namespace CS.Toolkit.Automapper
{
    [AutoMapping(typeof(Source), typeof(Target))]
    public class ObjectMapper : IObjectMapper
    {
        private readonly Lazy<ObjectAutomapper> _mapper = new();
        
        public TOut Map<TIn, TOut>(TIn source)
            where TIn : new()
            where TOut : new()
        {
            var res = _mapper.Value.Map(source);
            if (res is TOut outValue)
            {
                return outValue;
            }
            throw new NotImplementedException();
        }
    }
}
