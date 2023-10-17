# Funcionamento dos arquivos **.cb**
Os arquivos **.cb** são arquivos que indicarão ao compilador os assets que ele deverá compilar e para onde mandar esses assets.<br>
Os assets gerados pelo compilador terão a extensão **.ca**.

```
| Output
../Game.Windows/bin/Debug
../Game.Windows/bin/Release
../Game.Linux/bin/Release
/builded

| Atlas geral
#Texture:Atlas1
place_holder.png
Folder/player.jpg

| Atlas dos meus projéteis
#Texture:Folder/Atlas2
Folder/SubFolder/bullet_sheet.bmp

| Sons do meu jogo
#Audio:AudioFolder
music.ogg

| Fontes do meu jogo
#SpriteFont:Font Folder/Font SubFolder
font.csf

| Mapas Tiled do meu jogo
#Map:Maps
level1.tmj
level2.json
```

As primeiras linhas são para os diretórios de saída. <br />
Em seguida, o caractere "#" indica um grupo de assets, no formato "[Tipo]:[NomeDoGrupo]". <br />
Qualquer linha que comece com "|" será ignorada. <br />
A utilização do nome do grupo vai variar entre os tipos. Em geral, o nome do grupo diz respeito a pasta (caminho relativo) em que os arquivos ficarão nos diretórios de exportação. Porém, no caso de texturas, esse nome vai determinar o caminho do arquivo para o texture atlas.