# Funcionamento dos arquivos **.csf**
Os arquivos **.csf** são arquivos **json**, que descrevem uma SpriteFont baseada numa fonte real (instalada na sua máquina).

```json
{
    "FontName": "Arial",
    "Size": 12,
    "Spacing": 0,
    "Style": "Regular",

    "CharacterRegions": [
        { "Start": 32, "End": 126 },
        { "Start": 127, "End": 252 }
    ]
}
```

Esse tipo de arquivo é uma descrição direta de um SpriteFont, onde a Sprite é uma textura própria para a fonte. Na compilação ele é convertido para:

## SpriteFont
```
[bool:ÉLittleEndian][int:LarguraDaTextura][int:AlturaDaTextura][byte[]:BytesDaTextura][string:NomeDaFonte][float:EspaçoEntreCaracteres][int:NúmeroDeCaracteres][Glyph[]:DescriçãoDosCaracteres]
```

O nome da fonte será [NomeDoGrupo] + "/" + [NomeDoArquivo], sem extensão.

## Formato do Glyph

```
[char:Caractere][int:X][int:Y][int:Largura][int:Altura][int:NúmeroDePares][KerningPair[]:Pares]
```

## Formato do KerningPair

```
[char:Par][float:ValorDeKerning]
```

**Importante:**
* Tenha em mente que fontes assim podem ser mais pesadas (em compilação ou espaço no disco) e que elas terão uma textura dedicada só para elas;
* O atlas da fonte respeita o tamanho máximo de um atlas normal.