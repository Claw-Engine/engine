# Como os mapas são salvos
Os mapas são compilados com base em mapas do Tiled, exportados com as extensões .json e .tmj, com a seguinte configuração de exportação:<br>

![TiledPreferences](TiledExport.png)<br>

**Importante:** A última versão em que a Clawssets foi testada é o Tiled 1.10.1.

# Formato do Map

```
[int:NívelDeCompressão][int:Largura][int:Altura][int:IdDaPróximaCamada][int:IdDoPróximoObjeto][int:LarguraDosTiles][int:AlturaDosTiles][bool:ÉInfinito][string:Orientação][string:OrdemDeRenderização][string:VersãoDoTiled][string:Tipo][string:Versão][Layer[]:Camadas][Tileset[]:Tilesets]
```

# Formato da Layer

```
[int:Id][int:X][int:Y][int:Largura][int:Altura][bool:ÉVisível][float:Opacidade][string:Nome][string:Tintura][string:Cor][string:Tipo][string:OrdemDeDesenho][int:NúmeroDeTiles][int[]:Tiles][int:NúmeroDeChunks][Chunk[]:Chunks][int:NúmeroDeObjetos][Object[]:Objetos][int:NúmeroDeCamadas][Layer[]:Camadas][int:NúmeroDePropriedades][Property[]:Propriedades]
```

# Formato do Chunk

```
[int:X][int:Y][int:Largura][int:Altura][int:NúmeroDeTiles][int[]:Tiles]
```

# Formato do Object

```
[int:Id][int:Largura][int:Altura][bool:ÉVisível][float:X][float:Y][float:Rotação][string:Nome][string:Tipo][int:NúmeroDePropriedades][Property[]:Propriedades]
```

# Formato do Property

**Importante:** O Property.value é serializado para um array de bytes.

```
[string:Nome][string:Tipo][int:NúmeroDeBytes][byte[]:Bytes]
```

# Formato do Tileset

```
[int:Colunas][int:PrimeiroIdDeGrid][int:LarguraDaImagem][int:AlturaDaImagem][int:Margem][int:Espaçamento][int:NúmeroDeTiles][int:LarguraDosTiles][int:AlturaDosTiles][string:Imagem][string:Nome]
```