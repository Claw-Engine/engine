# ModuleManager
```csharp
public sealed class ModuleManager
```
Gerenciador de [Module](api/Claw/Modules/Module.md#Module) s.<br />
## ModuleManager
```csharp
public ModuleManager(Claw.Utils.ModuleLayer[] stepLayers, Claw.Utils.ModuleLayer[] renderLayers) { }
```
Cria o gerenciador, com camadas separadas para [ModuleManager.Step](api/Claw/Utils/ModuleManager.md#Step) e [ModuleManager.Render](api/Claw/Utils/ModuleManager.md#Render) .<br />
## Tilemaps
```csharp
public System.Collections.Generic.List<Claw.Maps.Tilemap> Tilemaps;
```
Variável de suporte, indicando os mapas presentes neste gerenciador (null por padrão).<br />
### Observações
O mapa estar presente nesta lista não significa, necessariamente, que suas camadas estejam em [ModuleManager.StepLayers](api/Claw/Utils/ModuleManager.md#StepLayers) ou [ModuleManager.RenderLayers](api/Claw/Utils/ModuleManager.md#RenderLayers) .<br />
## StepLayers
```csharp
public readonly Claw.Utils.ModuleLayer[] StepLayers;
```
## RenderLayers
```csharp
public readonly Claw.Utils.ModuleLayer[] RenderLayers;
```
## GetStepLayer
```csharp
public Claw.Utils.ModuleLayer GetStepLayer(string name) { }
```
Procura por uma camada em [ModuleManager.StepLayers](api/Claw/Utils/ModuleManager.md#StepLayers) com determinado nome.<br />
## GetRenderLayer
```csharp
public Claw.Utils.ModuleLayer GetRenderLayer(string name) { }
```
Procura por uma camada em [ModuleManager.RenderLayers](api/Claw/Utils/ModuleManager.md#RenderLayers) com determinado nome.<br />
## Exists
```csharp
public Claw.Utils.ModuleLayer Exists(Claw.Modules.Module module) { }
```
Verifica se um módulo existe em pelo menos uma das camadas.<br />
**Retorna**: A camada em que o módulo existe.<br />
## ExistsOnStep
```csharp
public Claw.Utils.ModuleLayer ExistsOnStep(Claw.Modules.Module module) { }
```
Verifica se um módulo existe em pelo menos uma das camadas em [ModuleManager.StepLayers](api/Claw/Utils/ModuleManager.md#StepLayers) .<br />
**Retorna**: A camada em que o módulo existe.<br />
## ExistsOnRender
```csharp
public Claw.Utils.ModuleLayer ExistsOnRender(Claw.Modules.Module module) { }
```
Verifica se um módulo existe em pelo menos uma das camadas em [ModuleManager.RenderLayers](api/Claw/Utils/ModuleManager.md#RenderLayers) .<br />
**Retorna**: A camada em que o módulo existe.<br />
## AddTo
```csharp
public void AddTo(Claw.Modules.Module module, string stepLayer, string renderLayer) { }
```
Insere um módulo em [ModuleManager.StepLayers](api/Claw/Utils/ModuleManager.md#StepLayers) e [ModuleManager.RenderLayers](api/Claw/Utils/ModuleManager.md#RenderLayers) , baseado no nome das camadas.<br />
### Observações
Não há nenhuma verificação interna para garantir que o módulo não exista em outras camadas.
            Se necessário, você pode utilizar [ModuleManager.Exists](api/Claw/Utils/ModuleManager.md#Exists) , [ModuleManager.ExistsOnStep](api/Claw/Utils/ModuleManager.md#ExistsOnStep) ou [ModuleManager.ExistsOnRender](api/Claw/Utils/ModuleManager.md#ExistsOnRender) para uma verificação genérica.<br />
## AddToStep
```csharp
public void AddToStep(Claw.Modules.Module module, string stepLayer) { }
```
Insere um módulo em [ModuleManager.StepLayers](api/Claw/Utils/ModuleManager.md#StepLayers) , baseado no nome da camada.<br />
### Observações
Não há nenhuma verificação interna para garantir que o módulo não exista em outras camadas.
            Se necessário, você pode utilizar [ModuleManager.Exists](api/Claw/Utils/ModuleManager.md#Exists) , [ModuleManager.ExistsOnStep](api/Claw/Utils/ModuleManager.md#ExistsOnStep) ou [ModuleManager.ExistsOnRender](api/Claw/Utils/ModuleManager.md#ExistsOnRender) para uma verificação genérica.<br />
## AddToRender
```csharp
public void AddToRender(Claw.Modules.Module module, string renderLayer) { }
```
Insere um módulo em [ModuleManager.RenderLayers](api/Claw/Utils/ModuleManager.md#RenderLayers) , baseado no nome da camada.<br />
### Observações
Não há nenhuma verificação interna para garantir que o módulo não exista em outras camadas.
            Se necessário, você pode utilizar [ModuleManager.Exists](api/Claw/Utils/ModuleManager.md#Exists) , [ModuleManager.ExistsOnStep](api/Claw/Utils/ModuleManager.md#ExistsOnStep) ou [ModuleManager.ExistsOnRender](api/Claw/Utils/ModuleManager.md#ExistsOnRender) para uma verificação genérica.<br />
## Step
```csharp
public void Step() { }
```
## Render
```csharp
public void Render() { }
```
