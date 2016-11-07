using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Autentificacion
{
    class Program
    {

        const string archivoPasswords = "archivo.txt";
        static void Main(string[] args)
        {
            String respuesta;

            if (!(File.Exists(archivoPasswords)))
            {
                using (File.Create(archivoPasswords))
                {

                }               
            }


            respuesta = "0";
            while (respuesta != "3") 
            {
                Console.WriteLine("Que quieres hacer?\n");
                Console.WriteLine("1. Darme de alta.\n");
                Console.WriteLine("2. Entrar mi login.\n");
                Console.WriteLine("3. Cerrar el programa.\n");

                respuesta = Console.ReadLine();
                switch (respuesta)
                {
                    case "1":
                        registrarse();
                        break;
                    case "2":
                        autentificar();
                        break;
                }
            }
        }




        public static void registrarse() {
            //Inicializamos las variable
            string password;
            string nombre;
            String nombreExiste;
            string[] fichero;

            //Le preguntamos su nombre y lo guardamos en el archivo.text (con una , al final)
            Console.WriteLine("Escribe tu nombre. \n");
            nombre = Console.ReadLine();

            string apartado = LlegiUsuari(nombre);

            fichero = apartado.Split(',');
            nombreExiste = (fichero[0]);

            if (nombreExiste == "404" || nombreExiste == null)  
            { 
                //Creamos el salt
                byte[] salt;
                new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
                                       
                //Le preguntamos su password, calculamos el hash y lo guardamos en el archivo.text (con una , al final)
                Console.WriteLine("Escribe tu contraseña. \n");
                password = Console.ReadLine();

                //Llamamos a la funcion calcular hash, con el password y el salt concatenado
                string hashString = CalculaHash(password, salt);

                //Aqui usamos la constante (Inicializada arriba con el nombre del archivo.) para escribir en el archivo.txt el nombre, el salt(convertida a String) y el hash del password y el salt
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(archivoPasswords, true))
                {
                    file.WriteLine(nombre + ',' + Convert.ToBase64String(salt) + ',' + hashString);
                }
                Console.WriteLine("Usuario registrado.\n");
            }
            else
            {
                Console.WriteLine("El nombre de este usuario ya existe.\n");
            }
        }




        



        public static void autentificar(){
            //Inicializamos las variable
            string password;
            string nombre;
            byte[] saltLeido;
            
            string[] fichero;
            

            //Le preguntamos su nombre y lo guardamos en el archivo.text (con una , al final)
            Console.WriteLine("Escribe tu nombre. \n");
            nombre = Console.ReadLine();

            //Le preguntamos su password,
            Console.WriteLine("Escribe tu contraseña. \n");
            password = Console.ReadLine();

            /////Cojo el salt del archivo y calculo el hash con la contraseña que introduzca y la comparo con el hash del archivo
            string apartado = LlegiUsuari(nombre);

            
            fichero = apartado.Split(',');
            saltLeido = Convert.FromBase64String(fichero[1]);
            

            //Llamamos a la funcion calcular hash, con el password y el salt concatenado
            string hashString = CalculaHash(password, saltLeido);

            

            if (hashString == fichero[2])
            {
                Console.WriteLine("Usuario autentificado.\n");
            }
            else
            {
                Console.WriteLine("Usuario o la contraseña no son validos.\n");
            }
        }





        public static string LlegiUsuari(string nomUsuari)
        {
            try
            {
                using (StreamReader lector = new StreamReader(archivoPasswords))
                {
                    while (lector.Peek() > -1)
                    {
                        string linia = lector.ReadLine();
                        string[] user;
                        if (!String.IsNullOrEmpty(linia))
                        {
                            //Separem nom i hash
                            user = linia.Split(',');
                            //Comprueba que el nombre del usuario exista
                            if (user[0].Equals(nomUsuari)) return user[0] +',' + user[1] + ',' + user[2];
                        }
                    }

                }
                // retorn d'usuari no trobat
                return "404";

            }
            catch
            {
                // Retornem null per indicar que no s'ha pogut llegir el fitxer
                return null;
            }

        }



        /////retorna un string amb el HASH resultat o null si hi ha error
        static string CalculaHash(string textIn, byte[] sal)
        {

            try
            {
                //calculamos el hash con el password y el sal juntos
                var pbkdf2 = new Rfc2898DeriveBytes(textIn, sal, 10000);
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hash);
            }
            catch (Exception)
            {
                Console.WriteLine("Error calculant el hash");
                Console.ReadKey(true);
                return null;
            }
        }

    }
}
