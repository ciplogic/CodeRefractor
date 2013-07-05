#!/bin/sh
g++ System_Console.cpp -c -std=c++11
rm ../libCRRuntimeLinux.a
ar -r -c -s ../libCRRuntimeLinux.a System_Console.o
ranlib ../libCRRuntimeLinux.a
