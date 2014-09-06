using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HM3DSGCR.Archive
{
    public class papagameconfig
    {
        public int FileBlockSize = 0;
        public long BlockCount = 0;
        public int[] FileBlockOffset;

        public enum EncodingType
        {
            UTF8,ASCII,UNICODE
        }

        public papagameconfig (Stream _str)
        {
            byte[] data = new byte[4];
            _str.Position = 0;

            data = new byte[] { (byte)_str.ReadByte(), (byte)_str.ReadByte(), (byte)_str.ReadByte(), (byte)_str.ReadByte() };
            FileBlockSize = BitConverter.ToInt32(data, 0);

            data = new byte[] { (byte)_str.ReadByte(), (byte)_str.ReadByte(), (byte)_str.ReadByte(), (byte)_str.ReadByte() };
            BlockCount = BitConverter.ToInt32(data, 0);

            FileBlockOffset = new int[BlockCount];

            for (int i = 0; i < FileBlockOffset.Length; i++)
            {
                data = new byte[] { (byte)_str.ReadByte(), (byte)_str.ReadByte(), (byte)_str.ReadByte(), (byte)_str.ReadByte() };
                FileBlockOffset[i] = BitConverter.ToInt32(data, 0);
            }
        }

        public string getText(int objectid, Stream str, EncodingType typenc = EncodingType.UNICODE,bool custom = false, params int[] input)
        {
            byte[] data = new byte[2];
            long offset;

            if (custom == true)
            {
                if (input[1] != 999)
                {
                    data = new byte[FileBlockOffset[input[1]] - FileBlockOffset[input[0]]];
                }
                else
                {
                    data = new byte[FileBlockSize - FileBlockOffset[input[0]]];
                }

                offset = input[0];
            }
            else
            {
                if (objectid >= BlockCount - 1)
                {
                    data = new byte[FileBlockSize - FileBlockOffset[objectid]];
                    offset = FileBlockOffset[objectid];
                }
                else
                {
                    data = new byte[FileBlockOffset[objectid + 1] - FileBlockOffset[objectid]];
                    offset = FileBlockOffset[objectid];
                }
            }
            

            str.Position = offset;
            str.Read(data, 0, data.Length);

            if (typenc == EncodingType.UNICODE)
            {
                return Encoding.Unicode.GetString(data);
            }
            else if (typenc == EncodingType.UTF8)
            {
                return Encoding.UTF8.GetString(data);
            }
            else
            {
                return Encoding.ASCII.GetString(data);
            }
        }
    }
}
