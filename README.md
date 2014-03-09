# GeoPattern C# #

*This is a C# port of [Jason Long's GeoPattern](https://github.com/jasonlong/geo_pattern):*

>Generate beautiful tiling SVG patterns from a string. The string is converted into a SHA and a color and pattern are determined based on the values in the hash. The color is determined by shifting the hue and saturation from a default (or passed in) base color. One of 16 patterns is used (or you can specify one) and the sizing of the pattern elements is also determined by the hash values.

>You can use the generated pattern as the `background-image` for a container. Using the `base64` representation of the pattern still results in SVG rendering, so it looks great on retina displays.

>See the [GitHub Guides](http://guides.github.com) site as an example of this library in action.

##Usage

The code for the API is simple. `GeoPattern.Patterns.Pattern` is the abstract base class for all pattern generators, and it has the following child classes:

+ **OctagonsPattern:** *octagons*
+ **OverlappingCirclesPattern:** *overlapping_circles*
+ **PlusSignsPattern:** *plus_signs*
+ **XesPattern:** *xes*
+ **SineWavesPattern:** *sine_waves*
+ **HexagonsPattern:** *hexagons*
+ **OverlappingRingsPattern:** *overlapping_rings*
+ **PlaidPattern:** *plaid*
+ **TrianglesPattern:** *triangles*
+ **SquaresPattern:** *squares*
+ **ConcentricCirclesPattern:** *concentric_circles*
+ **DiamondsPattern:** *diamonds*
+ **TessellationPattern:** *tessellation*
+ **NestedSquaresPattern:** *nested_squares*
+ **MosaicSquaresPattern:** *mosaic_squares*
+ **TrianglesRotatedPattern:** *triangles_rotated*
+ **ChevronsPattern:** *chevrons*

And `GeoPattern.GeoPatterns` is the factory for instantiating those pattern generators:

```csharp
var image = GeoPatterns.Generate("GitHub", "triangles_rotated").UriImage();
File.WriteAllText("GeoPattern.html", "<!DOCTYPE html><html><title></title></head><body style='background: " + image + "'></body></html>");
```

Loop through all available pattern generators and generate a data URI for each one:

```csharp
foreach (var patternName in GeoPatterns.PatternNames())
{
    var image = GeoPatterns.Generate("GitHub", patternName);
    // ...
}
```

Change the available options:

```csharp
GeoPatterns.Options(baseColor: "#2244aa");
var image = GeoPatterns.Generate("GitHub").UriImage();
File.WriteAllText("GeoPattern.html", "<!doctype html><html><title></title></head><body style='background: " + image + "'></body></html>");
```

Results in:

![geopattern example](https://raw.github.com/sgbj/GeoPattern/master/examples/geopattern.png)

And register your own custom pattern generators with the API:

```csharp
GeoPatterns.Register<MyCustomPattern>("my_custom_pattern");
var image = GeoPatterns.Generate("My Name", "my_custom_pattern").UriImage();
// ...
```

##Available patterns

###octagons
![octagons example](https://raw.github.com/sgbj/GeoPattern/master/examples/octagons.png)

###overlapping_circles
![overlapping_circles example](https://raw.github.com/sgbj/GeoPattern/master/examples/overlapping_circles.png)

###plus_signs
![plus_signs example](https://raw.github.com/sgbj/GeoPattern/master/examples/plus_signs.png)

###xes
![xes example](https://raw.github.com/sgbj/GeoPattern/master/examples/xes.png)

###sine_waves
![sine_waves example](https://raw.github.com/sgbj/GeoPattern/master/examples/sine_waves.png)

###hexagons
![hexagons example](https://raw.github.com/sgbj/GeoPattern/master/examples/hexagons.png)

###overlapping_rings
![overlapping_rings example](https://raw.github.com/sgbj/GeoPattern/master/examples/overlapping_rings.png)

###plaid
![plaid example](https://raw.github.com/sgbj/GeoPattern/master/examples/plaid.png)

###triangles
![triangles example](https://raw.github.com/sgbj/GeoPattern/master/examples/triangles.png)

###squares
![squares example](https://raw.github.com/sgbj/GeoPattern/master/examples/squares.png)

###concentric_circles
![concentric_circles example](https://raw.github.com/sgbj/GeoPattern/master/examples/concentric_circles.png)

###diamonds
![diamonds example](https://raw.github.com/sgbj/GeoPattern/master/examples/diamonds.png)

###tessellation
![tessellation example](https://raw.github.com/sgbj/GeoPattern/master/examples/tessellation.png)

###nested_squares
![nested_squares example](https://raw.github.com/sgbj/GeoPattern/master/examples/nested_squares.png)

###mosaic_squares
![mosaic_squares example](https://raw.github.com/sgbj/GeoPattern/master/examples/mosaic_squares.png)

###triangles_rotated
![triangles_rotated example](https://raw.github.com/sgbj/GeoPattern/master/examples/triangles_rotated.png)

###chevrons
![chevrons example](https://raw.github.com/sgbj/GeoPattern/master/examples/chevrons.png)

