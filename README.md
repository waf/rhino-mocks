Rhino Mocks

Forked from https://github.com/ayende/rhino-mocks/tree/d4a046bdacdd9d93a813c61f287f3f4cf0c7420f

With the following changes applied:

1. Upgrade dependencies (Castle.DynamicProxy -> Castle.Core) - commit 472cbeae
1. Upgrade the core RhinoMocks assembly to .NET Standard 2.0 - commit cc501fe4
    - This required removing all the "remoting" functionality from RhinoMocks
1. Upgrade xunit test project so unit tests can be run (though a number of them fail) - 27b75e0b and ebdbfe81
1. Apply thread-safety fix from this [blog post](http://r00t.dk/post/2011/03/26/thread-safe-version-of-rhino-mocks/) - commit b3e01f01


