TestTypeInference
=================

I was trying to make a fluent API with generics and descriptors that extend each other.
In this repo I've demonstrated 5 steps I took to create an API that supports fluent syntax.
Program5 contains the ultimate attempt to do the fluent API with generic descriptors.

For a real example check out Nest's facet descriptors: 
* https://github.com/Mpdreamz/NEST/blob/master/src/Nest/DSL/Facets/BaseFacetDescriptor.cs
* https://github.com/Mpdreamz/NEST/blob/master/src/Nest/DSL/Facets/RangeFacetDescriptor.cs

I tried to work out a way to get rid of all the "new" methods:

    public new RangeFacetDescriptor<T, K> Global()
    {
      this._IsGlobal = true; // _IsGlobal is internal so Nest's inspectors of the descriptor can access it
      return this;
    }

The problem is that we can't put the Global() method in the BaseFacetDescriptor, because it will break the fluentness
of the descriptor:

    var rangeFacetDescriptor = new RangeFacetDescriptor<MyType, int>();
    rangeFacetDescriptor
      .Global() // this method comes from the BaseFacetDescriptor, but needs to be newed in RangeFacetDescriptor
      .Ranges() // because we want to call e.g. Ranges() on it which belongs to RangeFacetDescriptor only

A way to solve this for Global() is to take the implementation to an extension method, the extension method could be made
generic, the type param would be need to have a constraint on a non-generic base class. For BaseFacetDescriptor this was easy (
https://github.com/q42jaap/NEST/blob/descriptors-extensionmethods/src/Nest/DSL/Facets/BaseFacetDescriptor.cs)
 
    public static class BaseFacetDescriptorExtensions
    {
		  public static TDescriptor Global<TDescriptor>(this TDescriptor descriptor)
	  		where TDescriptor : BaseFacetDescriptor
      {
        descriptor._IsGlobal = true;
        return descriptor;
      }
    }

The problem arises with the FacetFilter() method because it actualy uses the type parameter <T> from BaseFacetDescriptor

	  public new DateHistogramFacetDescriptor<T> FacetFilter(
      Func<FilterDescriptor<T>, BaseFilter> facetFilter
    )
    {
      var filter = facetFilter(new FilterDescriptor<T>());
      this._FacetFilter = filter;
      return this;
    }

The extension method would look like this:

	  public static TDescriptor FacetFilter<TDescriptor, T>(this TDescriptor descriptor, Func<FilterDescriptor<T>, BaseFilter> facetFilter)
	  		where TDescriptor : BaseFacetDescriptor
        where T : class
    {
      var filter = facetFilter(new FilterDescriptor<T>());
      descriptor._FacetFilter = filter;
      return descriptor;
    }

However, when calling the extension method like this:

    var rangeFacetDescriptor = new RangeFacetDescriptor<MyType, int>();
    rangeFacetDescriptor
      .FacetFilter(f => f.Terms(...))

you end up with the following compiler error:

    The type arguments for method 'Nest.BaseFacetDescriptorExtensions.FacetFilter<TDescriptor,T>(TDescriptor, Func<FilterDescriptor<T>, BaseFilter>)' cannot be inferred from the usage. Try specifying the type arguments explicitly.

Program5 from this repository demonstrates this. You can use the extension method, but it is very ugly:

     .Favorite<ExperiencedZooKeeper<Reptile>, Reptile>(...)
     
which is definitely not fluent at all.

The problem is the declaration of FacetFilter<TDescriptor, T>, because the T parameter has to be a part of the signature of the method.
There is no way to relate the T to the TDescriptor in a type inference way of speaking :(
As a human it seems evident, but the compiler is not a human...