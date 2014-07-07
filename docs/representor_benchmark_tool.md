### Edsoft.Hypermedia.Benchmark

Edsoft.Hypermedia.Benchmark is a console application for benchmarking HalSerializer.DeserializeToNewBuilder().

Expected usage:

    Edsoft.Hypermedia.Benchmark.exe deserializer <options>
    <options> available:
    -f, --filepath=FILEPATH    The FILEPATH of json to serialize.
    -i, --iterations=TIMES     The number of TIMES to repeat the benchmark. Default value is 5

Sample command:
```
    Edsoft.Hypermedia.Benchmark.exe deserializer --filepath "tests\Edsoft.Hypermedia.Tests\Integration\TestData\Hal\AllLinkObjectProperties.json"
```

You can find a sample batch file(benchmark.bat) in the project root.