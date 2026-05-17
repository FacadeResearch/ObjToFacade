using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjToFacade
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Obj 2 Facade Model Object";

            if (args.Length == 0 || args.Length > 1)
            {
                Console.WriteLine("Failed to convert .obj to .fmo\nProgram expects a path to the .obj\nPlease try again with ObjToFacade.exe [objectPath] like ObjToFacade.exe cube.obj");
                Console.ReadKey();
                return;
            }

            try
            {
                string path = args[0];
                string FileName = Path.GetFileNameWithoutExtension(path);

                Obj obj = ObjParser.ParseFromBytes(File.ReadAllBytes(path));

                if (obj.Faces.Length == 0 && obj.Vertices.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to convert .obj to .fmo\n0 Vertexes and Faces were parsed from the .obj. Try another model.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.ReadKey();
                    return;
                }

                File.WriteAllText($"{FileName}.fmo", FmConverter.ToFacadeModelText(obj));

                Console.WriteLine($"Successfully converted {FileName}.obj to {FileName}.fmo\nOUTPUT:\n\nName: {obj.Name}\nVertices: {obj.Vertices.Length / 3}\nFaces: {obj.Faces.Length}\nIf you have the Facade Modding SDK, use this .fmo file with your project.");
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to convert .obj to .fmo\nError:\n\n" + e.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            Console.ReadKey();
        }
    }
}
