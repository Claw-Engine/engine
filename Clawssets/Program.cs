using System;
using System.IO;
using Clawssets.Builder;

namespace Clawssets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string configFile = string.Empty;

#if DEBUG
            configFile = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../Tests/Assets/Tests.cb"));

            Console.WriteLine("Compilando \"{0}\"...", configFile);
#else
            if (args.Length > 0 && args[0].Length > 0) configFile = args[0];
            else
            {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());

                for (int i = 0; i < files.Length; i++)
                {
                    if (Path.GetExtension(files[i]).ToLower() == AssetBuilder.ConfigExtension)
                    {
                        configFile = files[i];

                        return;
                    }
                }
            }

            if (configFile.Length == 0)
            {
                Console.WriteLine("Digite o local do arquivo:");

                configFile = Console.ReadLine();
            }
#endif

            if (configFile.Length > 0)
            {
                AssetBuilder builder = new AssetBuilder(configFile);

                builder.Build();
                builder.CopyToOutputs();
            }

#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}