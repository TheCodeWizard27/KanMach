using KanMach.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace KanMach.Core
{
    public class KanGameEngine
    {
        private readonly ILogger _logger;
        private bool _run = true;

        public delegate void OnUpdateHandler(TimeSpan delta);
        public event OnUpdateHandler OnUpdate;

        public delegate void OnExitHandler();
        public event OnExitHandler OnExit;

        public IKanContext Context { get; set; }
        public KanGameController CurrentController { internal set; get; }

        internal KanGameEngine(ILogger logger)
        {
            _logger = logger;
        }

        public void Exit()
        {
            _run = false;
        }

        public void Run<ControllerType>() where ControllerType : KanGameController
        {
            var startController = (ControllerType) ActivatorUtilities.CreateInstance(Context.Provider, typeof(ControllerType));
            Run(startController);
        }

        public void Run(KanGameController kanGameController)
        {
            kanGameController.Context = Context.CreateNewScope();
            kanGameController.Init();
            CurrentController = kanGameController;

            var previous = DateTime.Now;
            DateTime current;
            while(_run)
            {
                current = DateTime.Now;
                Update(current - previous);
                previous = current;
            }

            OnExit?.Invoke();
            CurrentController.Dispose();
        }

        public void Update(TimeSpan delta)
        {
            OnUpdate?.Invoke(delta);
            CurrentController?.Update(delta);
        }

        public void SwapGameControllers(KanGameController controller, bool keepOld = false)
        {
            if(!keepOld)
            {
                CurrentController.Dispose();
            }

            CurrentController = controller;
            CurrentController.Init();
        }

    }
}
