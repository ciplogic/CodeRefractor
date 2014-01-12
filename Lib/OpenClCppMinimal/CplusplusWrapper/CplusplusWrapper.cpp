/**********************************************************************
Copyright ©2013 Advanced Micro Devices, Inc. All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

•	Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
•	Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or
 other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY
 DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS
 OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
********************************************************************/

#define  _VARIADIC_MAX 10
#define CL_USE_DEPRECATED_OPENCL_1_1_APIS
#define __CL_ENABLE_EXCEPTIONS
#include <CL/cl.hpp>
#include <functional>
#include <time.h>
#include <iostream>
#include <fstream>
#include <exception>
#include <cstdlib>
#include <vector>

#define SUCCESS 0
#define FAILURE 1
#define EXPECTED_FAILURE 2

const int NUM_ELEMENTS = 64;

/**
 * Write random number
 * @param vector stores the random sequence
 * @param width and height the size of arrayPtr
 * @param the range of the element
 * @param seed
 */
int fillRandom(
         std::vector<float> &vec, 
         const int width,
         const int height,
         const float rangeMin,
         const float rangeMax,
         unsigned int seed = 123);

/**
 * Print the vector
 * @param header 
 * @param data the vector
 */
void printVector(
    std::string header, 
    const std::vector<float> vec);

/**
 * Read a file to string
 * @param filename the file name 
 * @param s stores the file data
 */
int convertToString(
        const char *filename, 
        std::string& s);

int 
main(int argc, char * argv[])
{
    cl_int status = 0;
    // create a CL program using the kernel source
    const char *filename = "CplusplusWrapper_Kernels.cl";
    std::string sourceStr;

    status = convertToString(filename, sourceStr);
    if (status != SUCCESS) 
    {
        std::cout << "Failed to open " << filename << std::endl;
        return FAILURE;
    }

    // create program through .cl file
    cl::Program vectorAddProgram(std::string(sourceStr), false);
    try
    {
        vectorAddProgram.build("");
    } catch(cl::Error e) {
        std::cout << e.what() << std::endl;
        std::cout << "Build Status: " << vectorAddProgram.getBuildInfo<CL_PROGRAM_BUILD_STATUS>(cl::Device::getDefault()) << std::endl;
        std::cout << "Build Options:\t" << vectorAddProgram.getBuildInfo<CL_PROGRAM_BUILD_OPTIONS>(cl::Device::getDefault()) << std::endl;
        std::cout << "Build Log:\t " << vectorAddProgram.getBuildInfo<CL_PROGRAM_BUILD_LOG>(cl::Device::getDefault()) << std::endl;
        
        return FAILURE;
    }
    typedef cl::make_kernel<
            cl::Buffer&,
            cl::Buffer&,
            cl::Buffer&
            > KernelType;
   
    // create kernel as a functor
    KernelType vectorAddKernel(vectorAddProgram, "vectorAdd" );

    // create host memory inputA.
    std::vector<float> inputA(NUM_ELEMENTS);
    fillRandom(inputA, NUM_ELEMENTS, 1, 0, 255);
    printVector("inputA:", inputA);

    // create host memory inputB.
    std::vector<float> inputB(NUM_ELEMENTS);
    fillRandom(inputB, NUM_ELEMENTS, 1, 0, 255, 100);
    printVector("inputB:", inputB);

    // create host memory output.
    std::vector<float> output(NUM_ELEMENTS, 0);    
    
    // create memory objects.
    bool isReadOnly = true;
    cl::Buffer inputABuffer(inputA.begin(), inputA.end(), isReadOnly);
    cl::Buffer inputBBuffer(inputB.begin(), inputB.end(), isReadOnly);
    cl::Buffer outputBuffer(output.begin(), output.end(), !isReadOnly);
    
    cl::Event e;
	cl::Platform platform = cl::Platform::getDefault();
	/*
	if(strcmp(platform.getInfo<CL_PLATFORM_VENDOR>().c_str(), "Advanced Micro Devices, Inc."))
	{
		std::cout<<"Default platform should be Advanced Micro Devices, Inc. to run this sample\n"<<std::endl;
		exit(FAILURE);
	}
	*/
    // set arguments for kernel, and execute it.
    cl::NDRange ndrg(NUM_ELEMENTS);
    cl::NDRange ndrl(NUM_ELEMENTS);
    cl::EnqueueArgs arg(ndrg,  ndrl);

    // execute the kernel by calling the kernel functor
    e = vectorAddKernel(                
        arg,
        outputBuffer,
        inputABuffer,
        inputBBuffer);
        
     // transfer data to host memory from device memory.
    cl::copy(outputBuffer, output.begin(), output.end()); 
    printVector("output:", output);
	std::cout<<"Passed\n";
    return SUCCESS;
}

int convertToString(const char *filename, std::string& s)
{
    size_t size;
    char*  str;

    // create a file stream object by filename
    std::fstream f(filename, (std::fstream::in | std::fstream::binary));


    if(!f.is_open())
    {
     	return FAILURE;   
    }
    else
    {
        size_t fileSize;
        f.seekg(0, std::fstream::end);
        size = fileSize = (size_t)f.tellg();
        f.seekg(0, std::fstream::beg);

        str = new char[size+1];
        if(!str)
        {
            f.close();
            return FAILURE;
        }

        f.read(str, fileSize);
        f.close();
        str[size] = '\0';

        s = str;
        delete[] str;
        return SUCCESS;
    }
}


void printVector(
    std::string header, 
    const std::vector<float> vec)
{
    std::cout<<"\n"<<header<<"\n";

    // print all the elements of the data 
    for(std::vector<float>::size_type ix = 0; ix != vec.size(); ++ix)
    {
        std::cout<<vec[ix]<<" ";
    }
    std::cout<<"\n";
}

int fillRandom(
         std::vector<float> &vec, 
         const int width,
         const int height,
         const float rangeMin,
         const float rangeMax,
         unsigned int seed)
{
    if(vec.empty())
    {
        std::cout << "Cannot fill vector." << std::endl;
        return FAILURE;
    }

    // set seed
    if(!seed)
        seed = (unsigned int)time(NULL);

    srand(seed);

    // set the range
    double range = double(rangeMax - rangeMin) + 1.0; 

    /* random initialisation of input */
    for(int i = 0; i < height; i++)
        for(int j = 0; j < width; j++)
        {
            int index = i*width + j;
            vec[index] = rangeMin + float(range*rand()/(RAND_MAX + 1.0)); 
        }

    return SUCCESS;
}
