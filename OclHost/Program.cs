using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using OpenCL.Net;
using OpenCL.Net.Extensions;

namespace OclHost
{
    class Program
    {
        static void Main(string[] args)
        {
            uint platformCount;
            ErrorCode result = Cl.GetPlatformIDs(0, null, out platformCount);
            Console.WriteLine("{0} platforms found", platformCount);
            var platformIds = new Platform[platformCount];
            result = Cl.GetPlatformIDs(platformCount, platformIds, out platformCount);
            string firstKernel = string.Empty;
            foreach (Platform platformId in platformIds)
            {
                IntPtr paramSize;
                result = Cl.GetPlatformInfo(platformId, PlatformInfo.Name, IntPtr.Zero, InfoBuffer.Empty, out paramSize);

                using (var buffer = new InfoBuffer(paramSize))
                {
                    result = Cl.GetPlatformInfo(platformIds[0], PlatformInfo.Name, paramSize, buffer, out paramSize);

                    Console.WriteLine("Platform: {0}", buffer);
                    firstKernel = buffer.ToString();
                }
            }
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, @"TestKernels\TestKernel1.cl");
            var tasksKernel = new OpenCL.Net.Tasks.Kernel();
            var environment = firstKernel.CreateCLEnvironment();

            tasksKernel.Execute();
        
        }
    }

    internal class VirtualCrClFile : ITaskItem
    {
        public string GetMetadata(string metadataName)
        {
            throw new NotImplementedException();
        }

        public void SetMetadata(string metadataName, string metadataValue)
        {
            throw new NotImplementedException();
        }

        public void RemoveMetadata(string metadataName)
        {
            throw new NotImplementedException();
        }

        public void CopyMetadataTo(ITaskItem destinationItem)
        {
            throw new NotImplementedException();
        }

        public IDictionary CloneCustomMetadata()
        {
            throw new NotImplementedException();
        }

        public string ItemSpec
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ICollection MetadataNames
        {
            get { throw new NotImplementedException(); }
        }

        public int MetadataCount
        {
            get { throw new NotImplementedException(); }
        }
    }
}
