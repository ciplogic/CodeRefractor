Compilation
===========

In order to compile your generated source, the minimal GCC options you need to pass is just C++11.

This assumes a platform that runs on bare metal, so it will disable many things not expected to be present there:

```sh
g++ -DBARE_METAL -std=c++11 output.cpp -o main
```

For a regular application compilation simply run:
```sh
g++ -Ofast -fomit-frame-pointer -ffast-math -std=c++11 -static-libgcc -fpermissive output.cpp -ldl -o main
```

This allows also dynamic loading of external libraries, and calls with PInvoke.

