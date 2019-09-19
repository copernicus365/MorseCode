``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18362
Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview9-014004
  [Host]     : .NET Core 3.0.0-preview9-19423-09 (CoreCLR 4.700.19.42102, CoreFX 4.700.19.42104), 64bit RyuJIT
  DefaultJob : .NET Core 3.0.0-preview9-19423-09 (CoreCLR 4.700.19.42102, CoreFX 4.700.19.42104), 64bit RyuJIT


```
|       Method |     Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------- |---------:|----------:|----------:|-------:|------:|------:|----------:|
|  DecodeWDict | 35.23 us | 0.3792 us | 0.3547 us | 4.3945 |     - |     - |  13.58 KB |
| DecodeNormal | 18.66 us | 0.3204 us | 0.2840 us | 0.7629 |     - |     - |   2.41 KB |
