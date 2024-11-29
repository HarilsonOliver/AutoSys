using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Domain.Entities;

public class Veiculo{

    [System.ComponentModel.DataAnnotations.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;

    [Required]
    [StringLength(155)]
    public string Nome { get; set; } = default!;

    [Required]
    [StringLength(100)]
    public string Marca { get; set; } = default!;

    [Required]
    public int  Ano { get; set; } = default!;


}