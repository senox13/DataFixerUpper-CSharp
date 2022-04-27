# **DataFixerUpper C#** #

A ***heavily work-in-progress*** C# port of Mojang's DataFixerUpper library. Currently,
only the types in the `com.mojang.serialization` package and their
dependencies have been implemented. These types allow for format-agnostic
serialization, including a single `DynamicOps` implementation for
converting to/from JSON (`JsonOps`), via the library [Json.NET](https://www.newtonsoft.com/json).
Tests are included that broadly cover the current features of this library,
but are far from comprehensive. ***Bugs are to be expected.***

## Contact

I can be reached at, in descending order of preference:
* The issue tracker on [this repo](https://github.com/senox13/DataFixerUpper-CSharp)
* Discord: `senox13#8134`
* Email: `senox13@gmail.com`


## References

The ported Java library:  
https://github.com/Mojang/DataFixerUpper

A fork of the above library, with added javadoc comments:  
https://github.com/kvverti/Documented-DataFixerUpper
