using KanMach.Veldrid.Util.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Input
{
    public class Sdl2InputManager
    {

        public int Interval { get; set; } = 1000 / 60;
        public bool GamePadPollingEnabled { get; set; } = true;

        protected CancellationTokenSource _cancelToken;

        public Gamepads Gamepads { get; protected set; } = new Gamepads();

        public Sdl2InputManager(Sdl2InputManagerOptions options)
        {
            Interval = options.Interval;
            GamePadPollingEnabled = options.GamePadPollingEnabled;
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

    }
}
