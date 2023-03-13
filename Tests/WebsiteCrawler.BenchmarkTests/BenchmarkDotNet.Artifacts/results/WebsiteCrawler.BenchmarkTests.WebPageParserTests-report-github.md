``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22598.1)
Apple Part 000 r0p0, 1 CPU, 4 logical and 4 physical cores
.NET SDK=7.0.201
  [Host]     : .NET 7.0.3 (7.0.323.6910), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 7.0.3 (7.0.323.6910), Arm64 RyuJIT AdvSIMD


```
|         Method |    Mean |    Error |   StdDev |
|--------------- |--------:|---------:|---------:|
| HttpClientTest | 1.971 s | 0.0862 s | 0.2528 s |
|  WebClientTest | 1.990 s | 0.1130 s | 0.3315 s |
|     WebRequest | 1.887 s | 0.0774 s | 0.2195 s |
