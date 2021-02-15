using System.Text;

namespace FileManager
{
    public static class Extensions
    {
        public static Encoding GetEncoding(this EncodingEnums encoding)
        {
            switch(encoding)
            {
                case EncodingEnums.UTF7:
                    return Encoding.UTF7;

                case EncodingEnums.UTF8:
                    return Encoding.UTF8;

                case EncodingEnums.UTF32:
                    return Encoding.UTF32;

                case EncodingEnums.ASCII:
                    return Encoding.ASCII;

                case EncodingEnums.UNICODE:
                    return Encoding.Unicode;

                case EncodingEnums.BIG_ENDIAN_UNICODE:
                    return Encoding.BigEndianUnicode;
                
                default:
                    return Encoding.Default;
            }
        }
    }
}
