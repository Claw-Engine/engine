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
	public static string RootDirectory
	{
		get => _rootDirectory;
		set
		{
			_rootDirectory = value;
			FullPath = Path.Combine(currentDirectory, value);
		}
	}
	/// <summary>
	/// Diretório em que os assets estão ([caminho]/<see cref="RootDirectory" />).
	/// </summary>
	public static string FullPath { get; private set; }
	public const string AssetExtension = ".ca";
	private static string currentDirectory;
	private static string _rootDirectory;
	private static Dictionary<Type, Func<string, object>> readers;

	static Asset()
	{
		currentDirectory = Path.GetDirectoryName(AppContext.BaseDirectory);
		RootDirectory = "Assets";
		readers = new Dictionary<Type, Func<string, object>>()
		{
			{ typeof(Music), Music.Load },
			{ typeof(SoundEffect), SoundEffect.Load },
			{ typeof(Texture), Texture.Load },
			{ typeof(TextureAtlas), TextureAtlas.Load },
			{ typeof(SpriteFont), SpriteFont.Load }
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
	/// Carrega um asset.
	/// </summary>
	/// <typeparam name="T">O tipo de asset.</typeparam>
	/// <param name="path">Caminho relativo do arquivo, sem a extensão.</param>
	public static T Load<T>(string path) where T : class
	{
		path = Path.Combine(FullPath, path + AssetExtension);

		if (!File.Exists(path)) throw new Exception(string.Format("Arquivo \"{0}\" não encontrado!", path));

		return (T)readers[typeof(T)](path);
	}
}
