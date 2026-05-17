using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjToFacade
{
    public class Face
    {
        public int A { get; set; }

        public int B { get; set; }

        public int C { get; set; }
    }

    public class Obj
    {
        public string Name { get; set; }

        public float[] Vertices { get; set; }

        public float[] Vt { get; set; }

        public float[] Vn { get; set; }

        public Face[] Faces { get; set; }
    }
}
