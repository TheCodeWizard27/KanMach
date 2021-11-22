using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Interfaces
{
    public interface IKanContext
    {

        public IServiceProvider Provider { get; }

        public IKanContext CreateNewScope();
        public void SwapController(KanGameController controller, bool keepOld = false);

        public T Resolve<T>();
        public T ResolveController<T>() where T : KanGameController;

    }
}
