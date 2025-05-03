# Claw Engine
Claw Engine é uma engine 2D para a criação de jogos com .NET, utilizando SDL como back-end.<br>
Este é um projeto pessoal e contribuições de código não são aceitas.

# Estado da Engine
Atualmente, a engine está na versão 3.0, e não há planos para nenhuma grande atualização.
Se a Claw fizer sentido para você, é recomendável que você faça um fork do último release e bagunce da forma que quiser.

# Como Utilizar
A API Reference pode ser encontrada em [API](./api). O compilador de assets pode ser encontrado em [Clawssets](../../../clawssets).<br>
Para utilizar, basta referenciar o projeto no seu `.csproj`.
```xml
<!-- ... -->
<ItemGroup>
    <ProjectReference Include="/caminho/da/engine/claw.csproj" />
</ItemGroup>
<!-- ... -->
```
Ao compilar, os arquivos `SDL3.dll` e `SDL3.so` serão copiados para o caminho do seu projeto. Esses arquivos são dependências necessárias para Windows e Linux, respectivamente.
