using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Configuration;

namespace site.models
{
    #region Класс EducationOrganizations
    /// <summary>Класс представляет структуру данных по организациям с привязкой к региону, областям, городам </summary>
    [Table("EducationOrganizations")]
    public class EducationOrganizations
    {
        #region Поля
        [Key]
        [Column("FullName", TypeName = "VARCHAR", Order = 1)]
        [StringLength(512)]
        public string FullName { get; set; }

        [Key]
        [Column("Name", TypeName = "VARCHAR", Order = 2)]
        [StringLength(512)]
        public string Name { get; set; }

        [Column("District", TypeName = "VARCHAR")]
        [StringLength(512)]
        public string District { get; set; }

        [Column("Region", TypeName = "VARCHAR")]
        [StringLength(512)]
        public string Region { get; set; }

        [Column("Area", TypeName = "VARCHAR")]
        [StringLength(512)]
        public string Area { get; set; }

        [Column("City", TypeName = "VARCHAR")]
        [StringLength(512)]
        public string City { get; set; }

        [Column("TypeName", TypeName = "VARCHAR")]
        [StringLength(512)]
        public string TypeName { get; set; }

        [Column("MRSD", TypeName = "VARCHAR")]
        [StringLength(512)]
        public string MRSD { get; set; }

        [NotMapped]
        public int Priority { get; set; } = 0;
        #endregion

    }
    #endregion

    
}
