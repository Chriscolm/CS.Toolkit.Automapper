using Microsoft.VisualStudio.TestTools.UnitTesting;
using CS.Toolkit.Automapper.Datamodels;

namespace CS.Toolkit.Automapper.Tests
{
    [TestClass()]
    public class ObjectAutomapperTests
    {
        private Source _source;

        [TestInitialize]
        public void Up()
        {
            _source = new Source()
            {
                Id = 1,
                Name = "Foo",
                Age = "42 y"
                // wie im ersten Teil (dotnetpro 9/21) zum Thema Objektmapping dargestellt, gibt es derartige Konstrukte tatsächlich in der wirklichen Welt
            };
        }

        [TestMethod()]
        public void ItCreatesInstanceOfTargetObject()
        {
            var mapper = new ObjectMapper();
            var res = mapper.Map<Source, Target>(_source);
            Assert.IsInstanceOfType(res, typeof(Target));
        }

        [TestMethod()]
        public void ItMapsPropertiesWithEqualNames()
        {
            var mapper = new ObjectMapper();
            var res = mapper.Map<Source, Target>(_source);
            Assert.AreEqual(_source.Name, res.Name);
        }

        [TestMethod()]
        public void ItMapsPropertiesWithDifferentNames()
        {
            var mapper = new ObjectMapper();
            var res = mapper.Map<Source, Target>(_source);
            Assert.AreEqual(_source.Id, res.Key);
        }

        [TestMethod()]
        public void ItMapsPropertiesWithConverter()
        {
            var mapper = new ObjectMapper();
            var res = mapper.Map<Source, Target>(_source);
            Assert.AreEqual(42d, res.Age);
        }
    }
}