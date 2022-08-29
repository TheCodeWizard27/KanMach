using KanMach.Veldrid.Util.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Input
{
    public class VeldridInputManager : IVeldridInputManager
    {
        private IVeldridService _veldridService;

        public int Interval { get; set; } = 1000 / 60;
        public bool GamePadPollingEnabled { get; set; } = true;

        protected CancellationTokenSource _cancelToken;

        public Keyboard Keyboard { get; protected set; } = new Keyboard();
        public Mouse Mouse { get; protected set; } = new Mouse();
        public Gamepads Gamepads { get; protected set; } = new Gamepads();

        public VeldridInputManager(VeldridInputManagerOptions options, IVeldridService veldridService)
        {
            Interval = options.Interval;
            GamePadPollingEnabled = options.GamePadPollingEnabled;

            _veldridService = veldridService;
            veldridService.OnUpdate += VeldridService_OnUpdate;
            veldridService.OnInit += VeldridService_OnInit;
        }

        public void StartListening()
        {
            if (_cancelToken != null) throw new Exception("Gamepad polling already enabled.");

            if (!GamePadPollingEnabled) return;

            _cancelToken = new CancellationTokenSource();
            var tmpToken = _cancelToken.Token;

            Gamepads.Init();

            Task.Factory.StartNew(() =>
            {
                do
                {
                    Gamepads.Poll();
                    Thread.Sleep(Interval);
                } while (!tmpToken.IsCancellationRequested);
            }, tmpToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void StopListening() => _cancelToken.Cancel();

        private void VeldridService_OnInit()
        {
            StartListening();
        }

        private void VeldridService_OnUpdate(TimeSpan delta)
        {
            Keyboard.Update(_veldridService.CurrentInputSnapshot);
            Mouse.Update(_veldridService.CurrentInputSnapshot);
        }

    }
}
