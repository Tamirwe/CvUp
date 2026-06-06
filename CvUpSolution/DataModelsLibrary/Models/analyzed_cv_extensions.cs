using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace Database.models;

public partial class analyzed_cv
{
    [NotMapped] public Vector? titles_embedding { get; set; }
    [NotMapped] public Vector? skills_embedding { get; set; }
    [NotMapped] public Vector? summary_embedding { get; set; }
    [NotMapped] public Vector? companies_embedding { get; set; }
}
