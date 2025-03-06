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
    #region Класс RegionAreaCities
    /// <summary>Класс представляет структуру данных по региону, областям, городам</summary>
    [Table("RegionAreaCities")]
    public class RegionAreaCities
    {
        #region Поля
        [Key, Column("District", TypeName = "VARCHAR", Order = 1)]
        [StringLength(512)]
        public string District { get; set; }

        [Key, Column("Region", TypeName = "VARCHAR", Order = 2)]
        [StringLength(512)]
        public string Region { get; set; }

        [Key, Column("Area", TypeName = "VARCHAR", Order = 3)]
        [StringLength(512)]
        public string Area { get; set; }

        [Key, Column("City", TypeName = "VARCHAR", Order = 4)]
        [StringLength(512)]
        public string City { get; set; }

        [NotMapped]
        public int Priority { get; set; } = 0;

        #endregion

    }
    #endregion
}
