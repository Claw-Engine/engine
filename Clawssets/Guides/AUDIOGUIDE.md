# Como os áudios são salvos
O compilador suporta as extensões `.wav(PCM)`.

Os áudios são salvos num formato cru, sem nenhum tipo de compressão:

```
[byte:Canais][long:NúmeroDeSamples][float[]:Samples]
```

**Importante**: Todos os áudios precisam ter o sample rate de até 48kHz. Os que tiverem menos que isso serão convertidos. <br />
Além disso, áudios só podem ser Mono ou Stereo.