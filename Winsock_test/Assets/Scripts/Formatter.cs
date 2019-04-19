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
            byte[] arr = new byte[size + 5]; //Package size + unsigned id size and byte flag size

            arr[0] = flag;
            arr[1] = id[0];
            arr[2] = id[1];
            arr[3] = id[2];
            arr[4] = id[3];

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(p, ptr, true);
            Marshal.Copy(ptr, arr, 5, size);
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
