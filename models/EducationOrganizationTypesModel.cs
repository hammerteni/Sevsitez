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

    #region Класс EducationOrganizationTypes
    /// <summary>Класс представляет структуру данных по типам организаций</summary>
    [Table("EducationOrganizationTypes")]
    public class EducationOrganizationTypes
    {
        #region Поля
        [Key]
        [Column("Name", TypeName = "VARCHAR")]
        [StringLength(512)]
        public string Name { get; set; }

        #endregion

    }
    #endregion

}
