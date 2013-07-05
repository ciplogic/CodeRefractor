SET PATH=%PATH%;C:\Oss\Dev-Cpp\MinGW64\bin
g++ -c -std=c++11 System_Console.cpp
rm ../libCodeRefactorRuntime.a
ar -r -c -s ../libCodeRefactorRuntime.a System_Console.o
ranlib ../libCodeRefactorRuntime.a
