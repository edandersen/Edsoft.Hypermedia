# .NET Hypermedia Toolkit
A series of tools to enable .NET applications to produce and consume RESTful Hypermedia enabled APIs.

## Components

### Edsoft.Hypermedia

This is the core library, containing Serializers and Hypermedia Representors.

* [RepresentationBuilder][].

### Edsoft.Hypermedia.Client

A fully async/await supported client for traversing and navigating Hypermedia APIs. Supports .NET 4+, Xamarin, Windows Phone, Windows RT. Docs coming soon.

### Edsoft.Hypermedia.WebApi

Support for serving Hypermedia responses from ASP.NET WebApi. Docs are coming, but in the meantime there is sample code in the /samples folder.

#### Hypermedia format support

##### HAL+JSON

The following is supported in the HAL+JSON Serializer implemented by ```HalSerializer``` as defined by http://tools.ietf.org/html/draft-kelly-json-hal.

Spec | Support
--- | ---
4. Resource Objects | ✔
4.1. Reserved Properties | ✔
4.1.1. _links | ✔
4.1.2. _embedded | ✔
5. Link Objects | ✔
5.1. href | ✔
5.2. templated | ✔
5.3. type | ✔
5.4. deprecation | ✔
5.5. name | ✔
5.6. profile | ✔
5.7. title | ✔
5.8. hreflang | ✔
7. Media Type Parameters | ✔
7.1. profile | ✔
8. Recommendations | ✔
CURIE syntax | ❌

The HAL+JSON Serializer is complete apart from CURIEs.

##### HALE+JSON

The HALE+JSON Serializer supports everything the HAL+JSON Serializer does above. HALE+JSON is defined by the spec at https://github.com/mdsol/hale/blob/master/README.md. It is implemented by ```HaleSerializer```.

Spec | Support
--- | ---
4. Link Objects | ✔
4.1. method | ✔
4.2. data | ✔
4.3. render | ✔
4.4. enctype | ✔
4.5. target | ✔
5. Data Objects | ✔
5.1. Data Properties | ✔
5.1.1. type | ✔
5.1.2. data | ✔
5.1.3. scope | ✔
5.1.4. profile | ✔
5.1.5. value | ✔
5.2 Constraint Properties | ✔
5.2.1. options | ✔
5.2.2. in | ✔
5.2.3. min | ✔
5.2.4. minlength | ✔
5.2.5. max | ✔
5.2.6. maxlength | ✔
5.2.7. pattern | ✔
5.2.8. multi | ✔
5.2.9 required | ✔
5.3 Constraint Extensions | ❌
6. Resource Objects | ❌
6.1. Reserved Properties | ❌
6.1.1 _meta | ❌
7. Reference Objects | ❌
7.1. Reserved Properties | ❌
7.1.1. _ref | ❌
7.1.1.1. String values | ❌
7.1.1.2. Link Object values | ❌

The HALE+JSON Serializer supports everything apart from Meta and Reference Objects.

##### Other formats

We hope to support other formats such as Collection+JSON, Siren etc in the future.

## Contributing
See [CONTRIBUTING][] for details.

## Copyright
Copyright (c) 2014 Ed Andersen. Licensed under MIT. See [LICENSE][] for details.

All commits up to July 3rd 2014 and [this commit hash](https://github.com/edandersen/Edsoft.Hypermedia/commit/83d1be2299c1dce638d3c64881d9a44f2217b416)  Copyright (c) 2014 Medidata Solutions, Licensed under MIT. This project was forked from https://github.com/mdsol/crichton-dotnet on July 6th 2014.

## Authors

* [Ed Andersen](https://github.com/edandersen) - [@edandersen](https://twitter.com/edandersen)
* [Kenya Matsumoto](https://github.com/kenyamat)
* Based on the work by [Mark W. Foster](https://github.com/fosdev)

[CONTRIBUTING]: CONTRIBUTING.md
[LICENSE]: LICENSE.md
[RepresentationBuilder]: docs/representor_builder.md
[BenchmarkTool]: docs/representor_benchmark_tool.md
