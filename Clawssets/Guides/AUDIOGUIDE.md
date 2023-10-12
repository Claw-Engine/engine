# Como os áudios são salvos
O compilador suporta as extensões `.wav(PCM)`.

Os áudios são salvos num formato cru, sem nenhum tipo de compressão:

```
[int:TaxaDeSamples][byte:Canais][ushort:NúmeroDeSamples][int[]:Samples]
```