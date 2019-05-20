using System.Runtime.InteropServices;
using System.IO;
using System;

namespace HalkoNetworking
{
    public class Formatter
    {
        public byte[] Serialize(byte[] id, byte flag, Package p)
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

        public Package DeSerialize(byte[] bytes)
        {
            Package p = new Package();

            int size = Marshal.SizeOf(p);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(bytes, 5, ptr, size);

            p = (Package)Marshal.PtrToStructure(ptr, p.GetType());
            Marshal.FreeHGlobal(ptr);

            return p;
        }
    }

}
