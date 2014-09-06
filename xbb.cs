using System;
using System.IO;
using System.Text;

namespace HM3DSGCR.Archive
{
    internal class xbb
    {
        private string _path = "";
        private FileStream _stream;
        private long _filecount= 0;
        private long _blockASize = 32;
        private long _blockBSize = 0;
        private long _blockCSize = 0;
        private XbbFileEntry[] _fileentry;
        private CheckSumEntrySort[] _checksumentry;
        private long _lastoffset = 0;
        

        public xbb(){}

        public void setPath(string a)
        {
            _path = a;
            Open();
        }

        #region Getting FileOffset
        private void Open()
        {
            if (_path == "")
            {
                throw new Exception("Please Give Path of File.");
            }

            getCount();
            getLastOffset();
            blockB();
            _stream.Close();
        }

        private void getCount()
        {
            _stream = new FileStream(_path, FileMode.Open);
            _stream.Position = 4;

            byte[] bi = new byte[4];

            _stream.Read(bi, 0, 4);
            _filecount = BitConverter.ToInt32(bi, 0);
        }

        private void getLastOffset()
        {
            byte[] bi = new byte[4];

            _stream.Position = _blockASize;

            _stream.Read(bi, 0, 4);

            _lastoffset = BitConverter.ToInt32(bi, 0);
        }

        private void blockB()
        {
            _stream.Position = _blockASize;
            _blockBSize = _filecount*16;
            _blockCSize = _filecount*8;

            _fileentry = new XbbFileEntry[_filecount];
            _checksumentry = new CheckSumEntrySort[_filecount];

            byte[] bi = new byte[4];

            for (int i = 0; i < (int) _filecount; i++)
            {
                bi = new byte[] {(byte) _stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte()};

                _fileentry[i].FileOffset = BitConverter.ToInt32(bi, 0);

                bi = new byte[] { (byte)_stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte() };

                _fileentry[i].FileSize = BitConverter.ToInt32(bi, 0);

                bi = new byte[] { (byte)_stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte() };

                _fileentry[i].FileNameOffset = BitConverter.ToInt32(bi, 0);

                bi = new byte[] { (byte)_stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte() };

                _fileentry[i].FileCheckSum = BitConverter.ToInt32(bi, 0);
            }

            for (int i = 0; i < (int) _filecount; i++)
            {
                bi = new byte[] { (byte)_stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte() };

                _checksumentry[i].FileCheckSum = BitConverter.ToInt32(bi, 0);

                bi = new byte[] { (byte)_stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte(), (byte)_stream.ReadByte() };

                _checksumentry[i].FileID = BitConverter.ToInt32(bi, 0);
            }
        }
        #endregion

        #region Getting Data
        public byte[] getFile(int id)
        {
            if (_fileentry.Length == 0)
            {
                throw new Exception("XbbFileEntry is not have any children.");
            }

            _stream = new FileStream(_path, FileMode.Open);

            byte[] data = new byte[_fileentry[id].FileSize];

            _stream.Position = _fileentry[id].FileOffset;
            _stream.Read(data, 0, (int) _fileentry[id].FileSize);
            _stream.Close();

            return data;
        }

        public string getFileName(int id)
        {
            if (_fileentry.Length == 0)
            {
                throw new Exception("XbbFileEntry is not have any children.");
            }

            long FileNameLenght = 0;

            if (id == (_fileentry.Length - 1))
            {
                FileNameLenght = _lastoffset - _fileentry[id].FileNameOffset;
            }
            else
            {
                FileNameLenght = _fileentry[id+1].FileNameOffset - _fileentry[id].FileNameOffset;
            }

            byte[] data = new byte[FileNameLenght];

            _stream = new FileStream(_path, FileMode.Open);
            _stream.Position = _fileentry[id].FileNameOffset;

            _stream.Read(data, 0, (int) FileNameLenght);
            _stream.Close();

            return RemoveSpecialCharacters(System.Text.Encoding.UTF8.GetString(data));
        }

        internal static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        #endregion

        #region Getting Properties
        public long getFileSelectedOffset(int id)
        {
            return _fileentry[id].FileOffset;
        }

        public long getFileSelectedSize(int id)
        {
            return _fileentry[id].FileSize;
        }

        public long getFileNameSelectedOffset(int id)
        {
            return _fileentry[id].FileNameOffset;
        }

        public long getFileChecksum(int id)
        {
            return _fileentry[id].FileCheckSum;
        }

        public long getFileCount()
        {
            return _filecount;
        }
        #endregion
    }
}
