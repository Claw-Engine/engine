# Clawssets
Clawssets é um compilador de assets de linha de comando para a Claw.

# Como usar
Baixe algum **release** que corresponda à sua versão da Claw. A pasta terá o executável, as dependências e um arquivo batch. Para facilitar o uso do compilador, altere o seu **PATH**, inserindo o diretório do Clawssets.

```
> setx /M PATH "%PATH%;C:\Local\Do Meu\Clawssets"
```

Já para compilar os assets, basta utilizar o comando "Clawssets" em uma dessas formas:

```
> Clawssets [config].cb
> Clawssets
```

Caso não digite o nome do arquivo de configuração, o compilador irá procurar um arquivo **.cb** na pasta. Se não encontrar, ele pedirá que você especifique o caminho inteiro.

# Como os arquivos funcionam?
A pasta "Guides" tem uma explicação para o arquivo de configuração e para os arquivos exportáveis, incluindo explicações sobre como eles são exportados.

# Dependências
Abaixo, a lista de dependências, com os motivos:
* Claw:
    * Para acessar as estruturas do Claw.Tiled;
    * Para acessar o Claw.Graphics.SpriteFont.Glyph;
	* Para acessar o Claw.Audio.AudioManager.SampleRate.
* Newtonsoft.Json:
    * Para carregar os mapas **.json** e **.tmj** do Tiled;
    * Para carregar as descrições de fonte **.csf**.