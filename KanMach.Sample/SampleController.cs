using KanMach.Core;
using KanMach.Veldrid;
using KanMach.Veldrid.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Sample
{
    public class SampleController : KanGameController
    {
        private KanSystemCollection _systems;
        private readonly IVeldridService _vs;

        public SampleController(IVeldridService vs)
        {
            _vs = vs;
        }

        public override void Init()
        {

            //_systems = new KanSystemCollection(Context);

            //_systems.Add<SampleSystem>(config =>
            //{
            //    config.InitAction = () => Console.WriteLine("System1 Init");
            //    config.RunAction = (delta) => Console.WriteLine("System1 Run");
            //});
            //_systems.Add<SampleSystem>(config =>
            //{
            //    config.InitAction = () => Console.WriteLine("System2 Init");
            //    config.RunAction = (delta) => Console.WriteLine("System2 Run");
            //});
            //_systems.Add<SampleSystem>(-1, config =>
            //{
            //    config.InitAction = () => Console.WriteLine("System3 Init");
            //    config.RunAction = (delta) => Console.WriteLine("System3 Run");
            //});

        }

        public override void Update(TimeSpan delta)
        {

            //_systems.Run(delta);
            _vs.PumpEvents();
            _vs.Draw();

        }

        public override void Dispose()
        {
            _vs.DisposeResources();
        }
    }
}
