using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MiniTFG
{
    public class Receta
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }

        [JsonIgnore] // para que no se envíe a la API
        public ImageSource ImagenSource { get; set; }

        public int Comensales { get; set; }
        public string OrigenDelPlato { get; set; }
        public string TiempoPreparacion { get; set; }
        public string TipoCocina { get; set; }
        public string IngredientePrincipal { get; set; }
        public bool Gluten { get; set; }
        public bool Lactosa { get; set; }
        public bool Huevo { get; set; }
        public bool FrutosSecos { get; set; }
        public bool Mariscos { get; set; }
        public bool Soja { get; set; }
        public bool Pescado { get; set; }
        public bool Cacahuetes { get; set; }
        public bool Sesamo { get; set; }
        public bool Sulfitos { get; set; }
        public bool Mostaza { get; set; }
        public bool Altramuces { get; set; }
        public bool Moluscos { get; set; }
        public bool Apio { get; set; }
        public bool Vegano { get; set; }
        public bool Vegetariano { get; set; }
    }
}
