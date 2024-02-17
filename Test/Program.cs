using PhysX;
using System.Drawing;

namespace Test
{
    internal class Program
    {
        private static void Main()
        {
            const uint TargetFrames = 1000;

            Enviroment.LoadJson("startup_file.json");

            for (int i = 0; i < TargetFrames; i++)
            {
                Enviroment.Log($"=== Step {i + 1} ===");
                foreach (var @object in Enviroment.Objects)
                {
                    @object.Tick();
                    Enviroment.Log(@object);
                }
            }
        }
    }
}