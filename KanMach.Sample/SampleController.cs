using KanMach.Core;
using KanMach.Veldrid;
using KanMach.Veldrid.Input;
using KanMach.Veldrid.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Sdl2;

namespace KanMach.Sample
{
    public class SampleController : KanGameController
    {
        private KanSystemCollection _systems;
        private readonly VeldridService _vs;

        public Sdl2InputManager InputManager { get; set; }

        public SampleController(IVeldridService vs)
        {
            _vs = (VeldridService) vs;

            InputManager = new Sdl2InputManager();
        }

        public override void Init()
        {
            InputManager.StartListening();
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
