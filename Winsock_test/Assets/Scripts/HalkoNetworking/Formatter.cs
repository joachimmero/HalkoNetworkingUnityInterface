using System.Runtime.InteropServices;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

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

        public byte[] SerializeMethod(byte flag, int index)
        {
           
            Method method = new Method
            {
                MethodIndex = index
                //Parameter = Encoding.ASCII.GetBytes((string)parameter);
            };

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

        public int DeSerializeMethod(byte[] bytes)
        {
            Debug.Log(Encoding.ASCII.GetString(bytes, 0, 5));
            int size = Marshal.SizeOf(bytes.Length - 1);

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(bytes, 1, ptr, size);

            Method method = new Method();

            method = (Method)Marshal.PtrToStructure(ptr, method.GetType());
            
            return method.MethodIndex;
        }

        public byte[] SerializeMethod2(byte flag, int index, object[] parameters)
        {
            //Temporary list for the parameters.
            List<byte> tempBytes = new List<byte>();

            uint paramsCount = (uint)parameters.Length;
            for(uint i = 0; i < paramsCount; i++)
            {
                if (parameters[i].GetType() == typeof(int))
                {
                    tempBytes.Add((byte)'i');
                    byte[] bytes = BitConverter.GetBytes((int)parameters[i]);
                    Debug.Log("Int bytes lenght: " + bytes.Length);
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        tempBytes.Add(bytes[j]);
                    }
                }
                else if (parameters[i].GetType() == typeof(uint))
                {
                    tempBytes.Add((byte)'u');
                    byte[] bytes = BitConverter.GetBytes((uint)parameters[i]);
                    Debug.Log("UInt bytes lenght: " + bytes.Length);
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        tempBytes.Add(bytes[j]);
                    }
                }
                else if (parameters[i].GetType() == typeof(char))
                {
                    Debug.Log("Char bytes lenght: " + 1);
                    tempBytes.Add((byte)'c');
                    tempBytes.Add((byte)(char)parameters[i]);
                }
                else if (parameters[i].GetType() == typeof(string))
                {
                    tempBytes.Add((byte)'s');
                    byte[] stringLen = BitConverter.GetBytes((uint)((string)parameters[i]).Length);
                    for(int j = 0; j < stringLen.Length; j++)
                    {
                        tempBytes.Add(stringLen[j]);
                    }

                    byte[] bytes = Encoding.ASCII.GetBytes((string)parameters[i]);
                    Debug.Log("String bytes lenght: " + bytes.Length);
                    for (int k = 0; k < bytes.Length; k++)
                    {
                        tempBytes.Add(bytes[k]);
                    }
                }
                else if (parameters[i].GetType() == typeof(bool))
                {
                    tempBytes.Add((byte)'b');
                    Debug.Log("Bool bytes lenght: " + 1);
                    if ((bool)parameters[i] == true)
                    {
                        tempBytes.Add((byte)1);
                    }
                    else
                    {
                        tempBytes.Add((byte)0);
                    }
                }
                else if (parameters[i].GetType() == typeof(float))
                {
                    tempBytes.Add((byte)'f');
                    byte[] bytes = BitConverter.GetBytes((float)parameters[i]);
                    Debug.Log("Float bytes lenght: " + bytes.Length);
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        tempBytes.Add(bytes[j]);
                    }
                }
                else if (parameters[i].GetType() == typeof(double))
                {
                    tempBytes.Add((byte)'d');
                    byte[] bytes = BitConverter.GetBytes((double)parameters[i]);
                    Debug.Log("Double bytes lenght: " + bytes.Length);
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        tempBytes.Add(bytes[j]);
                    }
                }
                else if (parameters[i].GetType() == typeof(Vector2))
                {
                    tempBytes.Add((byte)'x');
                    //Vector2 bytes array.
                    byte[] bytes = new byte[8];
                    byte[] xBytes = BitConverter.GetBytes(((Vector2)parameters[i]).x);
                    byte[] yBytes = BitConverter.GetBytes(((Vector2)parameters[i]).y);

                    for(int j = 0; j < xBytes.Length; j++)
                    {
                        bytes[j] = xBytes[j];
                        bytes[j + 4] = yBytes[j];
                    }
                    for (int k = 0; k < bytes.Length; k++)
                    {
                        tempBytes.Add(bytes[k]);
                    }
                }
                else if (parameters[i].GetType() == typeof(Vector3))
                {
                    tempBytes.Add((byte)'y');
                    //Vector3 bytes array.
                    byte[] bytes = new byte[12];
                    byte[] xBytes = BitConverter.GetBytes(((Vector3)parameters[i]).x);
                    byte[] yBytes = BitConverter.GetBytes(((Vector3)parameters[i]).y);
                    byte[] zBytes = BitConverter.GetBytes(((Vector3)parameters[i]).z);

                    for (int j = 0; j < xBytes.Length; j++)
                    {
                        bytes[j] = xBytes[j];
                        bytes[j + 4] = yBytes[j];
                        bytes[j + 8] = zBytes[j];
                    }
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        tempBytes.Add(bytes[j]);
                    }
                }
                else if (parameters[i].GetType() == typeof(Vector4))
                {
                    tempBytes.Add((byte)'z');
                    //Vector4 bytes array.
                    byte[] bytes = new byte[16];
                    byte[] xBytes = BitConverter.GetBytes(((Vector4)parameters[i]).x);
                    byte[] yBytes = BitConverter.GetBytes(((Vector4)parameters[i]).y);
                    byte[] zBytes = BitConverter.GetBytes(((Vector4)parameters[i]).z);
                    byte[] wBytes = BitConverter.GetBytes(((Vector4)parameters[i]).w);

                    for (int j = 0; j < xBytes.Length; j++)
                    {
                        bytes[j] = xBytes[j];
                        bytes[j + 4] = yBytes[j];
                        bytes[j + 8] = zBytes[j];
                        bytes[j + 12] = wBytes[j];
                    }
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        tempBytes.Add(bytes[j]);
                    }
                }
            }

            byte[] streamLength = BitConverter.GetBytes((uint)(9 + tempBytes.Count));
            //unsigned int streamSize + byte flag size + paramsAmount (uint) + method name size + parameters size
            byte[] arr = new byte[13 + tempBytes.Count];

            byte[] indexBytes = BitConverter.GetBytes((uint)index);

            byte[] paramsBytes = BitConverter.GetBytes(paramsCount);

            //Add the size of the stream to the first four indexes of arr.
            arr[0] = streamLength[0];
            arr[1] = streamLength[1];
            arr[2] = streamLength[2];
            arr[3] = streamLength[3];

            arr[4] = flag;

            arr[5] = paramsBytes[0];
            arr[6] = paramsBytes[1];
            arr[7] = paramsBytes[2];
            arr[8] = paramsBytes[3];
            
            arr[9] = indexBytes[0];
            arr[10] = indexBytes[1];
            arr[11] = indexBytes[2];
            arr[12] = indexBytes[3];

            for (int i = 0; i < tempBytes.Count; i++)
            {
                arr[i + 13] = tempBytes[i];
            }
            Debug.Log(tempBytes.Count);
            Debug.Log(arr.Length);
            return arr;
        }

        public KeyValuePair<int, object[]> DeSerializeMethod2(byte[] bytes)
        {
            uint paramsLength = BitConverter.ToUInt32(bytes, 1);
            object[] parameters = new object[paramsLength];
            int i = 9;
            for(int j = 0; j < paramsLength; j++)
            {
                byte flag = bytes[i];
                switch (flag)
                {
                    case (byte)'i':
                        parameters[j] = (object)BitConverter.ToInt32(bytes, i + 1);
                        i += 5;
                        break;
                    case (byte)'u':
                        parameters[j] = (object)BitConverter.ToUInt32(bytes, i + 1);  
                        i += 5;
                        break;
                    case (byte)'c':
                        parameters[j] = (object)(char)bytes[i + 1];
                        i += 2;
                        break;
                    case (byte)'s':
                        uint stringLen = BitConverter.ToUInt32(bytes, i + 1);
                        string paramString = Encoding.ASCII.GetString(bytes, i + 5, (int)stringLen);
                        parameters[j] = (object)paramString;
                        i += 5 + (int)stringLen;
                        break;
                    case (byte)'b':
                        bool temp = true;
                        if (bytes[i + 1] == (byte)0)
                        {
                            temp = false;
                        }
                        parameters[j] = (object)temp;
                        i += 2;
                        break;
                    case (byte)'f':
                        parameters[j] = (object)BitConverter.ToSingle(bytes, i + 1);
                        i += 5;
                        break;
                    case (byte)'d':
                        parameters[j] = (object)BitConverter.ToDouble(bytes, i + 1);
                        i += 9;
                        break;
                    case (byte)'x':
                        Vector2 v2 = new Vector2(BitConverter.ToSingle(bytes, i + 1), BitConverter.ToSingle(bytes, i + 5));
                        parameters[j] = (object)v2;
                        i += 9;
                        break;
                    case (byte)'y':
                        Vector3 v3 = new Vector3(BitConverter.ToSingle(bytes, i + 1), BitConverter.ToSingle(bytes, i + 5), BitConverter.ToSingle(bytes, i + 9));
                        parameters[j] = (object)v3;
                        i += 13;
                        break;
                    case (byte)'z':
                        Vector4 v4 = new Vector4(BitConverter.ToSingle(bytes, i + 1), BitConverter.ToSingle(bytes, i + 5), BitConverter.ToSingle(bytes, i + 9), BitConverter.ToSingle(bytes, i + 13));
                        parameters[j] = (object)v4;
                        i += 17;
                        break;
                }
            }

            int methodIndex = BitConverter.ToInt32(bytes, 5);
            KeyValuePair<int, object[]> method = new KeyValuePair<int, object[]>(methodIndex, parameters);

            return method;
        }
    }

}
