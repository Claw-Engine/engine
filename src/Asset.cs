using System.Reflection;
using Claw.Graphics;
using Claw.Audio;

namespace Claw;

/// <summary>
/// Classe responsável pelo carregamento de assets.
/// </summary>
public static class Asset
{
	/// <summary>
	/// Diretório base dos assets ("Assets", por padrão).
	/// </summary>
	public static string RootDirectory = "Assets";
	private const string AssetExtension = ".ca";
	private static string currentDirectory;
	private static string fullPath => Path.Combine(currentDirectory, RootDirectory);
	private static Dictionary<Type, Func<string, object>> readers;

	static Asset()
	{
		currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		readers = new Dictionary<Type, Func<string, object>>()
		{
			{ typeof(Music), Music.Load },
			{ typeof(SoundEffect), SoundEffect.Load },
			{ typeof(Texture), Texture.Load },
			{ typeof(TextureAtlas), TextureAtlas.Load }
		};
	}

	/// <summary>
	/// Define uma função que carregará determinado tipo de asset.
	/// </summary>
	/// <typeparam name="T">O tipo de asset.</typeparam>
	/// <param name="reader">A função, que recebe um arquivo e retorna um asset ou nulo.</param>
	public static void AddReader<T>(Func<string, T> reader) where T : class
	{
		Type type = typeof(T);

		if (readers.ContainsKey(type)) throw new ArgumentException("Esse tipo já tem um leitor definido!");

		readers.Add(type, reader);
	}

	/// <summary>
	/// Carrega um asset através de um arquivo.
	/// </summary>
	/// <typeparam name="T">O tipo de asset.</typeparam>
	/// <param name="path">Caminho relativo do arquivo, sem a extensão.</param>
	public static T Load<T>(string path) where T : class
	{
		path = Path.Combine(fullPath, path + AssetExtension);

		if (!File.Exists(path)) throw new Exception("Arquivo \"{0}\" não encontrado!");

		return (T)readers[typeof(T)](path);
	}
}
