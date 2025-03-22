# CollectionExtensions
```csharp
public static class CollectionExtensions
```
Extensões para coleções.<br />
## HasMin
```csharp
public static bool HasMin<T>(System.Collections.Generic.IEnumerable<T> enumerable, int count) { }
```
Checa se a coleção tem pelo menos X itens.<br />
## Has
```csharp
public static bool Has<T>(System.Collections.Generic.IEnumerable<T> enumerable, int count) { }
```
Checa se a coleção tem exatamente X itens.<br />
## HasMinMax
```csharp
public static bool HasMinMax<T>(System.Collections.Generic.IEnumerable<T> enumerable, int min, int max) { }
```
Checa se a coleção tem entre X e Y itens.<br />
## HasMax
```csharp
public static bool HasMax<T>(System.Collections.Generic.IEnumerable<T> enumerable, int count) { }
```
Checa se a coleção tem no máximo X itens.<br />
## IsEmpty
```csharp
public static bool IsEmpty<T>(System.Collections.Generic.IEnumerable<T> enumerable) { }
```
Checa se a coleção é nula ou está vazia.<br />
## Swap
```csharp
public static void Swap<T>(T[] array, int index1, int index2) { }
```
Troca dois elementos de posição em um array.<br />
## Swap
```csharp
public static void Swap<T>(System.Collections.Generic.IList<T> list, int index1, int index2) { }
```
Troca dois elementos de posição em uma lista.<br />
## Swap
```csharp
public static void Swap<TKey,TValue>(System.Collections.Generic.IDictionary<TKey,TValue> map, TKey index1, TKey index2) { }
```
Troca dois elementos de posição em um dicionário.<br />
## Get
```csharp
public static TValue Get<TKey,TValue>(System.Collections.Generic.IDictionary<TKey,TValue> map, TKey index, TValue defaultValue) { }
```
Pega um valor no index, se ele existir. Senão, retorna um valor padrão.<br />
