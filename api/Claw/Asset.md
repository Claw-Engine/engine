# Asset
```csharp
public static class Asset
```
Classe responsável pelo carregamento de assets.<br />
## AssetExtension
```csharp
public const string AssetExtension;
```
## RootDirectory
```csharp
public static string RootDirectory { get; set; } 
```
Diretório base dos assets ("Assets", por padrão).<br />
## FullPath
```csharp
public static string FullPath { get; private set; } 
```
Diretório em que os assets estão ([caminho]/ [Asset.RootDirectory](/api/Claw/Asset.md#RootDirectory) ).<br />
## AddReader
```csharp
public static void AddReader<T>(System.Func<string,T> reader) { }
```
Define uma função que carregará determinado tipo de asset.<br />
**T**: O tipo de asset.<br />
**reader**: A função, que recebe um arquivo e retorna um asset ou nulo.<br />
## Load
```csharp
public static T Load<T>(string path) { }
```
Carrega um asset.<br />
**T**: O tipo de asset.<br />
**path**: Caminho relativo do arquivo, sem a extensão.<br />
