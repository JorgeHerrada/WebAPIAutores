using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using WebAppAutores.DTOs;

namespace WebAppAutores.Servicios
{
    public class HashService
    {
        public ResultadoHashDTO Hash(string textoPlano)
        {
            var sal = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(sal);
            }

            return Hash(textoPlano, sal);
        }

        public ResultadoHashDTO Hash(string textoPlano, byte[] sal)
        {
            var llaveDerivada = KeyDerivation.Pbkdf2(
                password: textoPlano,
                salt: sal,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32
            );

            var hash = Convert.ToBase64String(llaveDerivada);

            return new ResultadoHashDTO
            {
                Hash = hash,
                Sal = sal
            };
        }
    }
}
