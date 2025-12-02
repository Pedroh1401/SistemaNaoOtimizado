namespace SistemaNaoOtimizado.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }

        // Chaves Estrangeiras
        public int CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }
    }
}