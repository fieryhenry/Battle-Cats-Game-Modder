using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Battle_Cats_Game_Modder.Program;


namespace Battle_Cats_Game_Modder.Mods.LibnativeModding
{
    public class PatchLibnative
    {
        static readonly List<Tuple<string, byte[], int>> architectures = new()
        {
            Tuple.Create("x86", new byte[] { 0x55, 0x89, 0xe5, 0x83, 0xe4, 0xfc, 0xb8, 5, 0, 0, 0, 0x89, 0xec, 0x5d, 0xc3 }, 7),
            Tuple.Create("x86_64", new byte[] { 0xB8, 5, 0, 0, 0, 0xC3, 0x66, 0x2E, 0x0F, 0x1F, 0x84 }, 1),
            Tuple.Create("armeabi-v7a", new byte[] { 0x05, 0, 0xA0, 0xE3, 0x1E, 0xFF, 0x2F, 0xE1 }, 0),
            Tuple.Create("arm64-v8a", new byte[] { 0xA0, 0x00, 0x80, 0x52, 0xC0, 0x03, 0x5F, 0xD6 }, 0),
        };
        public static int Search_pos(string path)
        {
            List<string> architecture_str = new();
            foreach (Tuple<string, byte[], int> architecture in architectures)
            {
                architecture_str.Add(architecture.Item1);
            }
            ColouredText($"&What architecture are you using?:\n&{CreateOptionsList<string>(architecture_str.ToArray())}");
            int id = Inputed() - 1;
            Tuple<string, byte[], int> curr_architecture = architectures[id];
            int pos = Search(path, curr_architecture.Item2)[0] + curr_architecture.Item3;
            if (pos < 5000)
            {

                Error("Error. You must have already done this fix, or this is a bug. If it is a bug, please report this on discord in #bug-reports");
            }
            return pos;
        }
        public static void MD5Lib()
        {
            string path;
            Console.WriteLine("Please select an so file");
            OpenFileDialog fd = new()
            {
                Filter = "files (*.so)|*.so"
            };
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .so files");
                Main();
            }
            path = fd.FileName;

            int pos = Search_pos(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            stream.Position = pos;
            stream.WriteByte(0);
            Console.WriteLine("Successfully patched game code to not check the md5 hashes of .pack and .list files.\nYou shouldn't need to re-hack this after you edit the .pack and .lists again, the patch should remain until the game updates.");
        }
    }
}
