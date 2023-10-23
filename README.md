# Claw Engine
Claw é uma engine low-end para o desenvolvimento de jogos 2D pixel art.

# Código aberto, contribuição fechada
A Claw é um projeto pessoal que eu faço por diversão. Mas sinta-se livre para olhar o source ou até mesmo fazer um fork.

# Como usar
Além dos comentários de documentação em XML, você pode encontrar instruções mais específicas em [ClawDocs](https://github.com/tomateuso/ClawDocs).

# Dependências
## Claw
A única dependência da Claw é o **SDL2**, para o back-end.

## Clawssets
* Claw:
    * Para acessar as estruturas do Claw.Tiled;
    * Para acessar o Claw.Graphics.SpriteFont.Glyph;
	* Para acessar o Claw.Audio.AudioManager.SampleRate.
* Newtonsoft.Json:
    * Para carregar os mapas **.json** e **.tmj** do Tiled;
    * Para carregar as descrições de fonte **.csf**;
	* Para escrever e carregar os arquivos de cachê **.cache**.