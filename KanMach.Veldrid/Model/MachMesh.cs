using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Model
{
    public class MachMesh
    {
        MachPolygon[] Polygons;
        public MachMesh(MachPolygon[] p)
        {
            Polygons = p;
        }
    }
}
