using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TrabalhoFinal.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O	campo	nome	é	obrigatório")]
        public string nome { get; set; }
        public string descricao { get; set; }
        public string cor { get; set; }
        [Required(ErrorMessage = "O	campo	modelo	é	obrigatório")]
        public string modelo { get; set; }
        [Required(ErrorMessage = "O	campo	código	é	obrigatório")]
        public string codigo { get; set; }
        [Range(10, 999, ErrorMessage = "O	preço	deverá	ser	entre  10  e   999.")]
        public decimal preco { get; set; }
        public string peso { get; set; }
        public string altura { get; set; }
        public string largura { get; set; }
        public string comprimento { get; set; }
        public string diametro { get; set; }
        public string Url { get; set; }
    }
}