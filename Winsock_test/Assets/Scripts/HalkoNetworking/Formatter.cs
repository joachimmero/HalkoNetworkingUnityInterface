using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Collections.Generic;
namespace HalkoNetworking
{
    public class Formatter
    {
        public byte[] SerializePackage(byte[] id, byte flag, Package p)
        {
            int size = Marshal.SizeOf(p);

            //unsigned int streamSize + Package size + unsigned id size and byte flag size
            byte[] arr = new byte[size + 9];

            //Get the length of the stream "arr" in uints and convert it to bytes.
            byte[] streamLength = BitConverter.GetBytes((uint)arr.Length);

            //Add the size of the stream to the first four indexes of arr.
            arr[0] = streamLength[0];
            arr[1] = streamLength[1];
            arr[2] = streamLength[2];
            arr[3] = streamLength[3];

            arr[4] = flag;
            arr[5] = id[0];
            arr[6] = id[1];
            arr[7] = id[2];
            arr[8] = id[3];

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(p, ptr, true);
            Marshal.Copy(ptr, arr, 9, size);
            Marshal.FreeHGlobal(ptr);
          
            return arr;
        }

        public Package DeSerializePackage(byte[] bytes)
        {
            Package p = new Package();

            int size = Marshal.SizeOf(p);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(bytes, 5, ptr, size);

            p = (Package)Marshal.PtrToStructure(ptr, p.GetType());
            Marshal.FreeHGlobal(ptr);

            return p;
        }

        public byte[] SerializeMethod(byte flag, string methodName, object[] parameters)
        {
            KeyValuePair<string, object[]> method = new KeyValuePair<string, object[]>(methodName, parameters);

            int size = Marshal.SizeOf(method);
            
            //unsigned int streamSize + byte flag size + paramsAmount (uint) + method name size + parameters size
            byte[] arr = new byte[5 + size];

            //Get the length of the stream "arr" in uints and convert it to bytes.
            byte[] streamLength = BitConverter.GetBytes((uint)arr.Length);

            //Add the size of the stream to the first four indexes of arr.
            arr[0] = streamLength[0];
            arr[1] = streamLength[1];
            arr[2] = streamLength[2];
            arr[3] = streamLength[3];

            arr[4] = flag;

            //Allocate paramsSize of space in the memory.
            IntPtr ptr = Marshal.AllocHGlobal(size);

            //Move the parameters-data to the location of the pointer-ptr.
            Marshal.StructureToPtr(method, ptr, true);

            //Copy the data from the memory location of prt to the byte-array arr.
            Marshal.Copy(ptr, arr, 5, size);
            //Free the memory of ptr.
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public KeyValuePair<string, object[]> DeSerializeMethod(byte[] bytes)
        {

            int size = Marshal.SizeOf(bytes.Length - 5);

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(bytes, 5, ptr, size);

            KeyValuePair<string, object[]> method;

            Marshal.PtrToStructure(ptr, method);
            
            return method;
        }
    }

}
