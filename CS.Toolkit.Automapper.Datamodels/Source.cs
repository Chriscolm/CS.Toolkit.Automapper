using CS.Toolkit.Automapper.Contracts.Attributes;
using CS.Toolkit.Automapper.Contracts.Converters;

namespace CS.Toolkit.Automapper.Datamodels
{
    public class Source
    {
        [Mapping(targetPropertyPath:nameof(Target.Key))]
        public int Id { get; set; }
        [Mapping]
        public string Name { get; set; }
        [Mapping(nameof(Target.Age), typeof(CompoundStringToDoubleConverter))]
        public string Age { get; set; }
    }
}
