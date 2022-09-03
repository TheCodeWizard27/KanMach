using KanMach.Core;
using KanMach.Core.Interfaces;
using KanMach.Veldrid;
using KanMach.Veldrid.Input;
using System;

namespace KanMach.Sample
{
    public class SystemSampleController : KanGameController
    {
        private KanSystemCollection _systemCollection;

        public SystemSampleController()
        {            
        }

        public override void Init()
        {
            _systemCollection = new KanSystemCollection(Context);
            _systemCollection.Add<SampleSystem>(config =>
            {
                config.InitAction = () => Console.WriteLine("System1 Init");
                config.RunAction = (delta) => Console.WriteLine("System1 Run");
            });
            _systemCollection.Add<SampleSystem>(config =>
            {
                config.InitAction = () => Console.WriteLine("System2 Init");
                config.RunAction = (delta) => Console.WriteLine("System2 Run");
            });
            _systemCollection.Add<SampleSystem>(-1, config =>
            {
                config.InitAction = () => Console.WriteLine("System3 Init");
                config.RunAction = (delta) => Console.WriteLine("System3 Run");
            });
        }

        public override void Update(FrameTime delta)
        {
            _systemCollection.Run(delta);
        }

        public override void Dispose()
        {
        }
    }
}
