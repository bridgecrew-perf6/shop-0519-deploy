﻿using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        public string Title { get; set; }

        [MaxLength(1024, ErrorMessage = "Este campo decontar entre 3 e 60 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Range(0.1, int.MaxValue, ErrorMessage = "O preço do produto deve ser maior que zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "A categoria informada é inválida")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
