namespace Chess.Engine.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class ZobristKeyTable
    {
        private ulong[] _keys;

        public ZobristKeyTable(int keyCount)
        {
            this._keys = new ulong[keyCount];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            for (int i = 0; i < keyCount; i++)
            {
                byte[] bytes = new byte[8];
                rng.GetNonZeroBytes(bytes);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes);
                }

                this._keys[i] = BitConverter.ToUInt64(bytes, 0);
            }

        }

        public void Save(string filename)
        {
            using (FileStream fs = File.Create(filename))
            {
                foreach (ulong key in this._keys)
                {
                    byte[] bytes = BitConverter.GetBytes(key);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytes);
                    }

                    fs.Write(bytes, 0, 8);
                }
            }
        }
    }
}
