using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjToFacade
{
    public static class ObjParser
    {
        public static Obj ParseFromBytes(byte[] bytes)
        {
            Obj obj = new Obj();

            List<float> verticesList = new List<float>();
            List<float> vtList = new List<float>();
            List<float> vnList = new List<float>();
            List<Face> facesList = new List<Face>();

            using (MemoryStream ms = new MemoryStream(bytes))
            using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                        continue;

                    string[] tokens = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (tokens.Length == 0) continue;

                    string prefix = tokens[0];

                    switch (prefix)
                    {
                        case "o": // object name
                            if (tokens.Length > 1)
                            {
                                obj.Name = tokens[1];
                            }
                            break;

                        case "v": // vertex (x,y,z)
                            if (tokens.Length >= 4)
                            {
                                verticesList.Add(float.Parse(tokens[1], CultureInfo.InvariantCulture));
                                verticesList.Add(float.Parse(tokens[2], CultureInfo.InvariantCulture));
                                verticesList.Add(float.Parse(tokens[3], CultureInfo.InvariantCulture));
                            }
                            break;

                        case "vt": // texture coordinates (u,v)
                            if (tokens.Length >= 3)
                            {
                                vtList.Add(float.Parse(tokens[1], CultureInfo.InvariantCulture));
                                vtList.Add(float.Parse(tokens[2], CultureInfo.InvariantCulture));
                            }
                            break;

                        case "vn": // vertex normal (x, y,z)
                            if (tokens.Length >= 4)
                            {
                                vnList.Add(float.Parse(tokens[1], CultureInfo.InvariantCulture));
                                vnList.Add(float.Parse(tokens[2], CultureInfo.InvariantCulture));
                                vnList.Add(float.Parse(tokens[3], CultureInfo.InvariantCulture));
                            }
                            break;

                        case "f": //face (f 2/1/1 3/2/1 4/3/1)
                            if (tokens.Length >= 4)
                            {
                                int idxA = ParseFaceVertexIndex(tokens[1]);
                                int idxB = ParseFaceVertexIndex(tokens[2]);
                                int idxC = ParseFaceVertexIndex(tokens[3]);

                                facesList.Add(new Face
                                {
                                    A = idxA - 1,
                                    B = idxB - 1,
                                    C = idxC - 1
                                });


                                if (tokens.Length == 5)
                                {
                                    int idxD = ParseFaceVertexIndex(tokens[4]);

                                    facesList.Add(new Face
                                    {
                                        A = idxA - 1,
                                        B = idxC - 1,
                                        C = idxD - 1
                                    });
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            obj.Vertices = verticesList.ToArray();
            obj.Vt = vtList.ToArray();
            obj.Vn = vnList.ToArray();
            obj.Faces = facesList.ToArray();

            return obj;
        }

        private static int ParseFaceVertexIndex(string token)
        {
            if (token.Contains("/"))
            {
                string[] parts = token.Split('/');

                return int.Parse(parts[0]);
            }

            return int.Parse(token);
        }
    }
}
