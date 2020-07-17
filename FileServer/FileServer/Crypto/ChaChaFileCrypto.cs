using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.IO;

namespace FileServer.Crypto
{
    public class ChaChaFileCrypto
    {
        public byte[] Key { get; set; }
        public byte[] Iv { get; set; }
        public Stream BcEncryptStream(Stream inputStream)
        {
            CipherKeyGenerator gen = GeneratorUtilities.GetKeyGenerator("ChaCha20-Poly1305");
            var cipher = CipherUtilities.GetCipher("ChaCha20-Poly1305");
            Key = gen.GenerateKey();
            SecureRandom random = new SecureRandom();
            Iv = new byte[12];
            random.NextBytes(Iv);
            cipher.Init(true, new ParametersWithIV(new KeyParameter(Key), Iv));
            var cs = new CipherStream(inputStream, cipher, null);
            return cs;
        }

        public Stream BcDecryptStream(Stream inputStream)
        {
            var cipher = CipherUtilities.GetCipher("ChaCha20-Poly1305");
            cipher.Init(false, new ParametersWithIV(new KeyParameter(Key), Iv));
            var cs = new CipherStream(inputStream, cipher, cipher);
            return cs;
        }
    }
}
