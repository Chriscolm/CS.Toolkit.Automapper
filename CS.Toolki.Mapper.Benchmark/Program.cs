using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CS.Toolkit.Automapper.Datamodels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CS.Toolki.Mapper.Benchmark
{
    [SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.CoreRt50)]
    [RPlotExporter]
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Moin");
            var summary = BenchmarkRunner.Run<Runner>();
            Console.ReadLine();
        }
    }

    public class Runner
    {
        private readonly IEnumerable<Source> _sourceCollection;

        public Runner()
        {
            _sourceCollection = CreateSourceCollection(1000000);
        }

        [Benchmark]
        public void RunMappingByReflection()
        {
            var mapper = new CS.Toolkit.Mapper.ObjectMapper();
            var res = from q in _sourceCollection
                      select mapper.Map<Source, Target>(q);
            var arr = res.ToArray();
        }

        [Benchmark]
        public void RunMappingByGeneration()
        {
            var mapper = new CS.Toolkit.Automapper.ObjectMapper();
            var res = from q in _sourceCollection
                      select mapper.Map<Source, Target>(q);
            var arr = res.ToArray();
        }

        private IEnumerable<Source> CreateSourceCollection(int cnt)
        {
            var r = new Random();
            var result = new List<Source>(cnt);
            for (var i = 0; i < cnt; i++)
            {
                var source = new Source()
                {
                    Id = i,
                    Name = r.Next(0, int.MaxValue).ToString(),
                    Age = $"{r.Next(18, 43)} y"
                };
                result.Add(source);
            }

            return result;
        }
    }
}
