using FileManager;
using System;
using System.Diagnostics;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var mgr = new LocalFileManager();

            // Hashes and saving text and binary files
            Md5Test(mgr);
            Sha256Test(mgr);
            Sha512Test(mgr);
            StringBase64Test(mgr);
            TextFileBase64Test(mgr);
            BinaryFileBase64Test(mgr);

            // Moving and copying files and directories recursively
            var newDirectory = mgr.CreateDirectory("new folder");
            string x = mgr.GetProposedPathForFile(newDirectory, "test.txt");
            mgr.CopyFile("test.txt", x);

            string x2 = mgr.GetProposedPathForFile(newDirectory, "test.txt");
            mgr.CopyFile("test.txt", x2);

            var foobar = mgr.CreateDirectory(newDirectory.FullName, "foobar");
            var binary = mgr.CreateDirectory(foobar.FullName, "binary");
            var text = mgr.CreateDirectory(foobar.FullName, "text");
            mgr.MoveFile("test.txt", text.FullName, "test.txt");
            mgr.CopyFile("Untitled 1.odt", binary.FullName, "Untitled 1.odt");

            var copy = mgr.CopyDirectory(newDirectory, mgr.CreateDirectory(mgr.GetProposedPathForDirectory("new folder")), overwrite: false);

            // Create zip folder in a temporary directory
            var tempDir = mgr.GetTemporaryDirectory();
            var zipName = mgr.GetProposedPathForFile(tempDir, "test.zip");
            var zip = mgr.CreateZipFile(zipName, overwrite: true, copy.FullName, "Untitled 1.odt");

            mgr.ExtractZipFile(zip, "baz", true);
        }

        static void Sha256Test(IFileManager mgr)
        {
            string test = "The quick brown fox jumps over the lazy dog";
            Debug.Assert(mgr.GetTextSha256Hash(test) == "d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592");
            var textFileHash = mgr.GetFileSha256Hash("test.txt");
            Debug.Assert(textFileHash == "9eebf869675fb762f5b9ef72a6e5933f99db6859201b13ae48bd8ce5e955b900");
            var binaryFileHash = mgr.GetFileSha256Hash("Untitled 1.odt");
            Debug.Assert(binaryFileHash == "84f64b6b39ad1713a4a43f9b8722b48e07c8fc720042dc38d13eb4c36c1543d9");
        }

        static void Sha512Test(IFileManager mgr)
        {
            string test = "The quick brown fox jumps over the lazy dog";
            Debug.Assert(mgr.GetTextSha512Hash(test) == "07e547d9586f6a73f73fbac0435ed76951218fb7d0c8d788a309d785436bbb642e93a252a954f23912547d1e8a3b5ed6e1bfd7097821233fa0538f3db854fee6");
            var textFileHash = mgr.GetFileSha512Hash("test.txt");
            Debug.Assert(textFileHash == "be3f8825ebcb39c5ee382c7ade5dff4d704d28d666734c5da306295418753386e6840001a29379b00d8141230a77ad92072e176e913b7a0e35a2cae545bdb6cf");
            var binaryFileHash = mgr.GetFileSha512Hash("Untitled 1.odt");
            Debug.Assert(binaryFileHash == "d4917728c9d77b53f3ddcabd8f7740b550a4c9092facf70a78f2c4f23bfb4c467deb1805843e70d90bd6398f89249c2ed431a3cd4d46bec8fbbe0695ad1f0ecb");
        }

        static void Md5Test(IFileManager mgr)
        {
            string test = "The quick brown fox jumps over the lazy dog";
            Debug.Assert(mgr.GetTextMd5Hash(test) == "9e107d9d372bb6826bd81d3542a419d6");
            var textFileHash = mgr.GetFileMd5Hash("test.txt");
            Debug.Assert(textFileHash == "091bf4e55b6bc0687319e551c181153f");
            var binaryFileHash = mgr.GetFileMd5Hash("Untitled 1.odt");
            Debug.Assert(binaryFileHash == "bd28e0e8c28f6b7e1eb200c8664394b1");
        }

        static void StringBase64Test(IFileManager mgr)
        {
            string test = "The quick brown fox jumps over the lazy dog";
            var base64 = mgr.EncodeTextBase64String(test);
            Debug.Assert(base64 == "VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZw==");
            var decoded = mgr.DecodeTextBase64String(base64);
            Debug.Assert(decoded == test);
        }

        static void TextFileBase64Test(IFileManager mgr)
        {
            var base64 = mgr.EncodeFileBase64String("test.txt");
            Debug.Assert(base64 == "77u/VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZw==");
            byte[] decoded = mgr.DecodeFileBase64String(base64);
            mgr.WriteTextFile("test2.txt", decoded, EncodingEnums.UTF8, true);
        }

        static void BinaryFileBase64Test(IFileManager mgr)
        {
            var base64 = mgr.EncodeFileBase64String("Untitled 1.odt");
            Debug.Assert(base64 == "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OrsuuaXbOtrYtisfmh3xaoOlnyq6d24qOV+WhS03DGzPs6IoeJrzPE+8IsHVOHFIDMa0jggJHo1CafXkfBo5zuIJZldRdOugkHn3ID2L3TqpoLIKYbZSvYe2IJGBQI0JTMqEdIMcuk5LxTOW81E5waHt4sdgvdODojxg8CuOz9jeiAym5V7gvbDuXIPffJVoeu5jenXTxfHfI5RgnDLuT+q7O3n/5/4uz/8Z4q+0dkRsQM6jZ/qQ57TyH1VHr1BLBwi092jSBQEAAIMDAABQSwMEFAAACAAA43lUUpyvr70xBAAAMQQAAAgAAABtZXRhLnhtbDw/eG1sIHZlcnNpb249IjEuMCIgZW5jb2Rpbmc9IlVURi04Ij8+CjxvZmZpY2U6ZG9jdW1lbnQtbWV0YSB4bWxuczpvZmZpY2U9InVybjpvYXNpczpuYW1lczp0YzpvcGVuZG9jdW1lbnQ6eG1sbnM6b2ZmaWNlOjEuMCIgeG1sbnM6eGxpbms9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGxpbmsiIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6bWV0YT0idXJuOm9hc2lzOm5hbWVzOnRjOm9wZW5kb2N1bWVudDp4bWxuczptZXRhOjEuMCIgeG1sbnM6b29vPSJodHRwOi8vb3Blbm9mZmljZS5vcmcvMjAwNC9vZmZpY2UiIHhtbG5zOmdyZGRsPSJodHRwOi8vd3d3LnczLm9yZy8yMDAzL2cvZGF0YS12aWV3IyIgeG1sbnM6dGV4dG9vbz0iaHR0cDovL29wZW5vZmZpY2Uub3JnLzIwMTMvb2ZmaWNlIiBvZmZpY2U6dmVyc2lvbj0iMS4yIj48b2ZmaWNlOm1ldGE+PG1ldGE6aW5pdGlhbC1jcmVhdG9yPlN1bmRlciBLaHViY2hhbmRhbmk8L21ldGE6aW5pdGlhbC1jcmVhdG9yPjxtZXRhOmNyZWF0aW9uLWRhdGU+MjAyMS0wMi0yMFQxNToxNDo0Mi43MTwvbWV0YTpjcmVhdGlvbi1kYXRlPjxtZXRhOmRvY3VtZW50LXN0YXRpc3RpYyBtZXRhOnRhYmxlLWNvdW50PSIwIiBtZXRhOmltYWdlLWNvdW50PSIwIiBtZXRhOm9iamVj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");
            byte[] decoded = mgr.DecodeFileBase64String(base64);
            mgr.WriteBinaryFile("Untitled 2.odt", decoded, true);
        }
    }
}
