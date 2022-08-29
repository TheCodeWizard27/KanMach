using KanMach.Core;
using KanMach.Veldrid;
using KanMach.Veldrid.Input;
using System;

namespace KanMach.Sample
{
    public class InputSampleController : KanGameController
    {
        private readonly IVeldridService _veldridService;
        private readonly IVeldridInputManager _inputManager;

        public InputSampleController(IVeldridService veldridService, IVeldridInputManager inputManager)
        {
            _veldridService = veldridService;
            _inputManager = inputManager;
        }

        public override void Init()
        {
            _inputManager.Gamepads.OnConnect += (gamepad) =>
            {
                Console.WriteLine($"{gamepad.GamepadMap.Name} has been connected");

                gamepad.OnButtonDown += (button) =>
                {
                    Console.WriteLine($"{Enum.GetName(button)} is down");
                };
            };

            _inputManager.Gamepads.OnDisconnect += (gamepad) =>
            {
                Console.WriteLine($"{gamepad.GamepadMap.Name} has been disconnected");
            };

        }

        public override void Update(TimeSpan delta)
        {

            //_veldridService.Draw();

        }

        public override void Dispose()
        {
            _veldridService.DisposeResources();
        }
    }
}
