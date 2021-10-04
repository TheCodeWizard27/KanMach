using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs
{
    public class EcsConfig
    {

        public static EcsConfig Default = new EcsConfig();

        public int WorldEntitiesCacheSize { get; set; }
        public int WorldComponentPoolsCacheSize { get; set; }
        public int EntityComponentCacheSize { get; set; }
        public int ViewEntityCacheSize { get; set; }
        public int ViewCacheSize { get; set; }

        public EcsConfig()
        {
            WorldEntitiesCacheSize = 1024;
            WorldComponentPoolsCacheSize = 64;
            EntityComponentCacheSize = 8;
            ViewCacheSize = 8;
            ViewEntityCacheSize = 8;
        }

    }
}
