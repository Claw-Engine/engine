using System;
using System.IO;
using Clawssets.Builder;

namespace Clawssets
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            Debug();
#else
            Release(args);
#endif
        }

        /// <summary>
        /// Código executado no programa publicado.
        /// </summary>
        private static void Release(string[] args)
        {
            string configFile = string.Empty;
            
            if (args.Length > 0 && args[0].Length > 1) configFile = args[0];
            else
            {
                string[] files = Directory.GetFiles(Environment.CurrentDirectory);

                for (int i = 0; i < files.Length; i++)
                {
                    if (Path.GetExtension(files[i]).ToLower() == AssetBuilder.ConfigExtension)
                    {
                        configFile = files[i];

                        break;
                    }
                }
            }

            if (configFile.Length == 0)
            {
                Console.WriteLine("Digite o local do arquivo:");

                configFile = Console.ReadLine();
            }

            if (configFile.Length > 0)
            {
                Console.WriteLine("Compilando \"{0}\"...\n", configFile);
                AssetBuilder.BuildAssets(configFile);
            }
        }
        /// <summary>
        /// Código executado nos testes.
        /// </summary>
        private static void Debug()
        {
            string configFile = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../Tests/Assets/Tests.cb"));

            Console.WriteLine("Compilando \"{0}\"...\n", configFile);
            AssetBuilder.BuildAssets(configFile);
            Console.ReadLine();
        }
    }
}