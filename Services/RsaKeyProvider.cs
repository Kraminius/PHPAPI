using System.Security.Cryptography;

namespace PHPAPI.Services
{
    public static class RsaKeyProvider
    {
        private static readonly Lazy<RSA> _publicKey = new Lazy<RSA>(GetPublicKey);

        public static RSA PublicKey => _publicKey.Value;

        private static RSA GetPublicKey()
        {
            var publicKeyPath = "/container-secure-dir/public_key.pem";
            RSA rsa = RSA.Create();
            string publicKeyContent = File.ReadAllText(publicKeyPath);
            rsa.ImportFromPem(publicKeyContent.ToCharArray());

            Console.WriteLine("Public Key: " + publicKeyContent);

            return rsa;
        }
    }
}
