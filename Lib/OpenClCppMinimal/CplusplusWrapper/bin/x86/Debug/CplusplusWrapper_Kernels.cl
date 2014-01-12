
float compute(float a, float b)
{
	int intA = (int) a;
	int intB= (int) b;



	float result = 0.0f;
	if(intA %2 == 0)
		goto label_else;
	
	result = intA + intB;
	goto label_final;
	
	label_else:
	result = intA+intB+3;

	label_final:
	return result;
}

__kernel 
void vectorAdd(__global float *output,
                __global float *inputA,		
                __global float *inputB,
				__global float toAdd,
				__global float toRemove)  
            
{

    int gid = get_global_id(0);

	float a =inputA[gid];
	float b =inputB[gid];

    output[gid] = compute(a,b);
    
}