using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.Database
{
    public class DatabaseManager<T>
    {
        private string ruta;
        private List<T> data = new List<T>();

        public DatabaseManager(string rut)
        {
            ruta = rut;
        }

        public void Guardar()
        {
            string texto = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(ruta, texto);
        }

        public void Actualizar(Func<T, bool> criterio, T nuevo)
        {
            data = data.Select(x =>
            {
                if (criterio(x)) x = nuevo;
                return x;
            }).ToList();

            Guardar();
        }

        public List<T> getData()
        {
            return data;
        }

        public void Cargar()
        {
            try
            {
                string archivo = File.ReadAllText(ruta);
                data = JsonConvert.DeserializeObject<List<T>>(archivo);
            }
            catch (Exception) { }
        }

        public void Insertar(T Nuevo)
        {
            data.Add(Nuevo);
            Guardar();
        }

        public List<T> Buscar(Func<T, bool> criterio)
        {
            return data.Where(criterio).ToList();
        }

        public void Eliminar(Func<T, bool> criterio)
        {
            data = data.Where(x => !criterio(x)).ToList();
            Guardar();
        }

    }
}
