using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.models
{
    public partial class cvup00001Context : DbContext
    {

        public virtual DbSet<IdNameModel> idNameModelDB { get; set; } = null!;
        public virtual DbSet<CvsToIndexModel> cvsToIndexDB { get; set; } = null!;

    }
}
