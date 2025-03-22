# Asset
```csharp
public static class Asset
```
Classe responsável pelo carregamento de assets.<br />
## RootDirectory
```csharp
public static string RootDirectory;
```
Diretório base dos assets ("Assets", por padrão).<br />
## GetFullPath
```csharp
public static string GetFullPath(string path) { }
```
Obtem o caminho completo para o asset, incluindo a extensão.<br />
**path**: Caminho relativo do arquivo, sem a extensão.<br />
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
