using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Veldrid.Model
{
    public class MachPolygon
    {
        Vector3[] Vertices { get; set; }
        public MachPolygon(Vector3[] v)
        {
            Vertices = v;
        }

    }
}
