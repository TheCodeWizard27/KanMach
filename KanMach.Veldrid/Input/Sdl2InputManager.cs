using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        public void StartListening()
        {
            _cancelToken = new CancellationTokenSource();
            var tmpToken = _cancelToken.Token;

            Gamepads.Init();

            Task.Factory.StartNew(() =>
            {
                do
                {
                    if (GamePadPollingEnabled) Gamepads.Poll();
                    Thread.Sleep(Interval);
                } while (!tmpToken.IsCancellationRequested);
            }, tmpToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void StopListening() => _cancelToken.Cancel();

    }
}
