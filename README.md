# Claw Engine
Claw Engine é uma engine 2D para a criação de jogos com .NET, utilizando SDL como back-end.<br>
Este é um projeto pessoal e contribuições de código não são aceitas.

# Estado da Engine
Atualmente, a versão **3.0** está sendo testada em um projeto real. Muitas coisas podem — e vão — ser alteradas.<br>
Use por sua conta e risco.


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
