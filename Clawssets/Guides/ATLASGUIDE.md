# Como os texture atlas são salvos
Os texture atlas representam um compilado de sprites, no seguinte formato:

```
[bool:ÉLittleEndian][int:LarguraDoAtlas][int:AlturaDoAtlas][byte[]:BytesDoAtlas][int:NumeroDeSprites][Sprite[]:Sprites]
```

**Importante:** Apesar das texturas na Claw estarem no formato ABGR, no arquivo elas podem estar em ARGB (Big Endian) ou BGRA (Little Endian).<br>

## Formato da Sprite

```
[string:NomeDaSprite][int:XDaSprite][int:YDaSprite][int:LarguraDaSprite][int:AlturaDaSprite]
```

# Atlas gerado

Os primeiros dois pixels, da primeira linha, são um pixel branco e um pixel vazio.<br>
O primeiro pixel branco é para não quebrar o batching. O segundo é um padding para evitar problemas com o source rectangle.<br>

O tamanho máximo de um atlas é definido pelo Clawssets/Resources/BaseAtlas.png.<br>
As imagens no atlas terão sempre pelo menos um pixel de espaço entre elas.<br>
Apesar do tamanho (2048x2048), o atlas possui espaços não-aproveitados, para uma compilação rápida. Por isso, pode ser que suas sprites estourarem o limite, mesmo com espaços vazios no atlas.<br>

O nome das sprites do seu atlas será [NomeDoGrupo] + "/" + [NomeDaImagem], sem extensão.