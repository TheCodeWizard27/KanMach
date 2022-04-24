using KanMach.Core;
using KanMach.Veldrid;
using KanMach.Veldrid.Input;
using System;

namespace KanMach.Sample
{
    public class GraphicsSampleController : KanGameController
    {
        private readonly IVeldridService _veldridService;

        public GraphicsSampleController(IVeldridService veldridService, Sdl2InputManager inputManager)
        {
            _veldridService = veldridService;
        }

        public override void Init()
        {

        }

        public override void Update(TimeSpan delta)
        {

            _veldridService.PumpEvents();
            _veldridService.Draw();

        }

        public override void Dispose()
        {
            _veldridService.DisposeResources();
        }
    }
}
