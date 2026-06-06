using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace Database.models;

public partial class ai_analyze_cv
{
    [NotMapped] public Vector? embedding { get; set; }
}
