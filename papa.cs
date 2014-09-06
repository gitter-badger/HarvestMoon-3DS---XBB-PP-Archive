using System;
using System.IO;

namespace HM3DSGCR.Archive
{
    public class papa
    {
        private string _path = "";
        private Stream _stream;
        private long _filecount = 0;
        private const long _filezerosize = 0; // constant, header size too
        private long _finaloffsetbig = 0;
        private long _finaloffsetsmall = 0;
        private long _filezerooffset = 0;
        private PapaFileEntry[] _entry;
        private long modefile = 0;

        public papa() { }

        public void setPath(string a)
        {
            _path = a;
            Open();
        }

        private void Open()
        {
            if (_path == "")
            {
                throw new Exception("Please Give Path of File.");
            }

            getCount();
            getZeroOffset();
            getOffsettable();
            _stream.Close();
        }

        public void Open(Stream str)
        {
            if (str.Length == 0)
            {
                throw new Exception("Please Give Path of File.");
            }

            modefile = 1;
            byte[] bi = new byte[4];

            _stream = str;

            _stream.Position = 8;
            _stream.Read(bi, 0, 4);
            _finaloffsetsmall = BitConverter.ToInt32(bi, 0);

            _stream.Position = 12;
            _stream.Read(bi, 0, 4);
            _finaloffsetbig = BitConverter.ToInt32(bi, 0);

            _stream.Position = 16;
            _stream.Read(bi, 0, 4);
            _filecount = BitConverter.ToInt32(bi, 0);

            getZeroOffset();
            getOffsettable();
            //_stream.Close();
        }

        private void getOffsettable()
        {
            _entry = new PapaFileEntry[_filecount];

            long[] hitung1 = new long[_filecount];
            byte[] bi = new byte[4];

            _stream.Position = 20;

            for (int i = 0; i < _filecount; i++)
            {
                bi = new byte[] {(byte) _stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte()};

                hitung1[i] = BitConverter.ToInt32(bi, 0);
            }

            _entry[0].FileOffset = _filezerooffset;
            _entry[0].FileSize = _filezerosize;

            for (int i = 1; i < _filecount; i++)
            {
                long _countlenght = hitung1[i] - hitung1[i - 1];

                _entry[i].FileOffset = _entry[i - 1].FileOffset + _entry[i-1].FileSize;
                _entry[i].FileSize = _countlenght;
            }
        }

        private void getCount()
        {
            byte[] bi = new byte[4];

            _stream = new FileStream(_path, FileMode.Open);

            _stream.Position = 8;
            _stream.Read(bi, 0, 4);
            _finaloffsetsmall = BitConverter.ToInt32(bi, 0);

            _stream.Position = 12;
            _stream.Read(bi, 0, 4);
            _finaloffsetbig = BitConverter.ToInt32(bi, 0);

            _stream.Position = 16;
            _stream.Read(bi, 0, 4);
            _filecount = BitConverter.ToInt32(bi, 0);
        }

        private void getZeroOffset()
        {
            byte[] bi = new byte[4];
            _stream.Position = 20;
            _stream.Read(bi, 0, 4);
            _filezerooffset = BitConverter.ToInt32(bi, 0);
        }

        public byte[] getFile(int id)
        {
            if (_entry.Length == 0)
            {
                throw new Exception("XbbFileEntry is not have any children.");
            }

            if (modefile == 0)
            {
                _stream = new FileStream(_path, FileMode.Open);
            }

            byte[] data = new byte[_entry[id].FileSize];

            _stream.Position = _entry[id].FileOffset;
            _stream.Read(data, 0, (int)_entry[id].FileSize);

            if (modefile == 0)
            {
                _stream.Close();
            }

            return data;
        }

        public long getFileCount()
        {
            return _filecount;
        }
    }
}
