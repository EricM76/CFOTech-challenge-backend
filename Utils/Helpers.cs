using XSystem.Security.Cryptography;

namespace ApiPeliculas.Utils
{
    public class Helpers
    {

        //Método para encriptar contraseña con MD5 se usa tanto en el Acceso como en el Registro
        public static string hashPassword(string valor)
        {
#pragma warning disable SYSLIB0021 // El tipo o el miembro están obsoletos
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
#pragma warning restore SYSLIB0021 // El tipo o el miembro están obsoletos
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
                resp += data[i].ToString("x2").ToLower();
            return resp;
        }
    }
}
