# Representation Builder

The ```RepresentationBuilder``` class allows you to build ```HypermediaRepresentation``` objects using a simple interface, instead of worrying about manually setting properties on the ```HypermediaRepresentation``` object. This is the preferred and recommended way of constructing ```HypermediaRepresentation``` objects.

Example usage:

```csharp

IRepresentationBuilder builder = new RepresentationBuilder();

builder.SetSelfLink("/api/objects/1"); // set the self link
builder.SetAttributesFromObject(myObject); // myObject is a model class
builder.AddTransition("parent", "/api/parent_objects/2"); // adds a transition

// create an embedded resource
IRepresentationBuilder embeddedResourceBuilder = new RepresentationBuilder();
embeddedResourceBuilder.SetAttributesFromObject(myEmbeddedResource);
embeddedResourceBuilder.SetSelfLink("/api/embedded_resources/3");

// add the embedded resource to the builder
builder.AddEmbeddedResource("child", embeddedResourceBuilder.ToRepresentation());

// get the Representation object
var Representation = builder.ToRepresentation();

// you can then use a serializer
var serializer = new HalSerializer();
var json = serializer.Serialize(Representation);

```

You should whenever possible use the ```IRepresentationBuilder``` interface and not depend on the concrete class. Try hooking up your favorite DI framework.

## ```IRepresentationBuilder``` methods

The following methods are available on IRepresentationBuilder.

### ```HypermediaRepresentation ToRepresentation()```
Create a HypermediaRepresentation object from this builder instance. Returns a HypermediaRepresentation object.

### ```void SetSelfLink(string self)```
Sets the Self Link on the HypermediaRepresentation object that you are building

Param | Description
--- | ---
self | The Self Link Uri 

### ```void SetAttributes(JObject attributes)```
Sets the HypermediaRepresentation attributes from a JSON.NET JObject.

Param | Description
--- | ---
attributes | A JSON.NET JObject 

### ```void SetAttributesFromObject(object data)```
Sets the HypermediaRepresentation attributes from an abitrary object. This object will be internally converted into a JObject using JSON.NET, so all standard JSON.NET attributes apply.

Param | Description
--- | ---
data | An object, Model, ViewModel etc 

### ```void AddTransition(HypermediaTransition transition)```
Adds a HypermediaTransition to the current HypermediaRepresentation that you are building.

Param | Description
--- | ---
transition | A HypermediaTransition object 


### ```void AddTransition(string rel, string uri = null, string title = null, string type = null, bool uriIsTemplated = false, string depreciationUri = null, string name = null, string profileUri = null, string languageTag = null)```
Adds a transition to the current HypermediaRepresentation that you are building.

Param | Description
--- | ---
rel | The link relation. 
uri | The Uri of the transition 
title | The title of the transition 
type | The type of the transition 
uriIsTemplated | True if the Uri is a templated Uri, false if not. 
depreciationUri | If the transition has been deprecated, a link to a Uri explaining the deprecation 
name | The name of the transition. Can be used as an alternative or subcategory of title. 
profileUri | Uri to an http://alps.io/ or similar profile. 
languageTag | Language of the transition, as per RFC 5988 http://tools.ietf.org/html/rfc5988 


### ```void AddEmbeddedResource(string key, HypermediaRepresentation resource)```
Adds an embedded resource. There can be multiple resources with the same key, in which case a collection of resources will be built.

Param | Description
--- | ---
key | The embedded resource key 
resource | The embedded resource as represented by a HypermediaRepresentation 


### ```void SetCollection(IEnumerable<HypermediaRepresentation> Representations;```
Sets the HypermediaRepresentation you are building to be a collection instead of a single object.

Param | Description
--- | ---
Representations | An enumerable of HypermediaRepresentations 


### ```void SetCollection<T>(IEnumerable<T> collection, Func<T, string> selfLinkFunc)```
Sets the HypermediaRepresentation you are building to be a collection instead of a single object.

Example use:

```csharp

public class MyObject {
    public int Id { get; set;}
    public string Name { get; set;}
}

var myObjectCollection = new List<MyObject> { 
    new MyObject { Id = 1, Name = "Brian"}, 
    new MyObject { Id = 2, Name = "Terrance"}
};

IRepresentationBuilder builder = new RepresentationBuilder();
builder.SetCollection(myObjectCollection, o => "api/objects/" + o.Id);

```

Param | Description
--- | ---
Type T | The type of the object contained in the collection, such as a Model or ViewModel type
collection | The collection of objects that represent the collection. JSON.NET will be used to serialize the objects, so the objects can have standard JSON.NET attributes. 
selfLinkFunc | A function that defines the Self Link. This will be called on each object to populate the Self Link of the HypermediaRepresentation. 