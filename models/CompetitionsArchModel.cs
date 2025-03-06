using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Configuration;
using static System.Net.Mime.MediaTypeNames;

namespace site.models
{
    #region Класс CompetitionsArchModel
    /// <summary>Класс представляет структуру данных по организациям с привязкой к региону, областям, городам </summary>
    [Table("competitions_arch")]
    public class CompetitionsArchModel
    {
        #region Поля

        [Key]
        [Column("_id", TypeName = "INTEGER")]
        public int _id { get; set; }

        [Column("OldId", TypeName = "INTEGER")]
        public int OldId { get; set; } = 0;

       
        [Column("Fio", TypeName = "VARCHAR")]
        public string Fio { get; set; } = "";

        [Column("Class_", TypeName = "VARCHAR")]
        public string Class_ { get; set; } = "";

        [Column("Age", TypeName = "VARCHAR")]
        public string Age { get; set; } = "";

        [Column("WorkName", TypeName = "VARCHAR")]
        public string WorkName { get; set; } = "";

        [Column("WorkComment", TypeName = "VARCHAR")]
        public string WorkComment { get; set; } = "";

        [Column("EducationalOrganization", TypeName = "VARCHAR")]
        public string EducationalOrganization { get; set; } = "";

        [Column("EducationalOrganizationShort", TypeName = "VARCHAR")]
        public string EducationalOrganizationShort { get; set; } = "";

        [Column("Email", TypeName = "VARCHAR")]
        public string Email { get; set; } = "";

        [Column("Telephone", TypeName = "VARCHAR")]
        public string Telephone { get; set; } = "";

        [Column("District", TypeName = "VARCHAR")]
        public string District { get; set; } = "";

        [Column("Region", TypeName = "VARCHAR")]
        public string Region { get; set; } = "";

        [Column("Area", TypeName = "VARCHAR")]
        public string Area { get; set; } = "";

        [Column("City", TypeName = "VARCHAR")]
        public string City { get; set; } = "";

        [Column("ChiefFio", TypeName = "VARCHAR")]
        public string ChiefFio { get; set; } = "";

        [Column("ChiefPosition", TypeName = "VARCHAR")]
        public string ChiefPosition { get; set; } = "";

        [Column("ChiefEmail", TypeName = "VARCHAR")]
        public string ChiefEmail { get; set; } = "";

        [Column("ChiefTelephone", TypeName = "VARCHAR")]
        public string ChiefTelephone { get; set; } = "";

        [Column("SubsectionName", TypeName = "VARCHAR")]
        public string SubsectionName { get; set; } = "";

        [Column("Fios", TypeName = "VARCHAR")]
        public string Fios { get; set; } = "";

        [Column("Agies", TypeName = "VARCHAR")]
        public string Agies { get; set; } = "";

        [Column("AgeСategories", TypeName = "VARCHAR")]
        public string AgeСategories { get; set; } = "";


        [Column("Kvalifications", TypeName = "VARCHAR")]
        public string Kvalifications { get; set; } = "";

        [Column("Programms", TypeName = "VARCHAR")]
        public string Programms { get; set; } = "";

        [Column("Links", TypeName = "VARCHAR")]
        public string Links { get; set; } = "";

        [Column("CompetitionName", TypeName = "VARCHAR")]
        public string CompetitionName { get; set; } = "";

        [Column("DateReg", TypeName = "INTEGER")]
        public int DateReg { get; set; } = 0;

        [Column("Likes", TypeName = "INTEGER")]
        public int Likes { get; set; } = 0;

        [Column("Nolikes", TypeName = "INTEGER")]
        public int Nolikes { get; set; } = 0;

        [Column("Approved", TypeName = "INTEGER")]
        public int Approved { get; set; } = 0;

        [Column("PdnProcessing", TypeName = "INTEGER")]
        public int PdnProcessing { get; set; } = 0;

        [Column("PublicAgreement", TypeName = "INTEGER")]
        public int PublicAgreement { get; set; } = 0;

        [Column("ProcMedicine", TypeName = "INTEGER")]
        public int ProcMedicine { get; set; } = 0;

        [Column("SummaryLikes", TypeName = "INTEGER")]
        public int SummaryLikes { get; set; } = 0;

        [Column("ClubsName", TypeName = "VARCHAR")]
        public string ClubsName { get; set; } = "";

        [Column("Weight", TypeName = "INTEGER")]
        public int Weight { get; set; } = 0;

        [Column("Weights", TypeName = "VARCHAR")]
        public string Weights { get; set; } = "";

        [Column("Result", TypeName = "VARCHAR")]
        public string Result { get; set; } = "";

        [Column("Results", TypeName = "VARCHAR")]
        public string Results { get; set; } = "";

        [Column("ProtocolFile", TypeName = "VARCHAR")]
        public string ProtocolFile { get; set; } = "";

        [Column("ProtocolPartyCount", TypeName = "INTEGER")]
        public int ProtocolPartyCount { get; set; } = 0;

        [Column("TechnicalInfo", TypeName = "VARCHAR")]
        public string TechnicalInfo { get; set; } = "";

        [Column("Timing_min", TypeName = "INTEGER")]
        public int Timing_min { get; set; } = 0;

        [Column("Timing_sec", TypeName = "INTEGER")]
        public int Timing_sec { get; set; } = 0;

        [Column("ChiefFios", TypeName = "VARCHAR")]
        public string ChiefFios { get; set; } = "";

        [Column("ChiefPositions", TypeName = "VARCHAR")]
        public string ChiefPositions { get; set; } = "";

        [Column("AthorsFios", TypeName = "VARCHAR")]
        public string AthorsFios { get; set; } = "";

        [Column("AgeСategory", TypeName = "VARCHAR")]
        public string AgeСategory { get; set; } = "";

        [Column("PartyCount", TypeName = "INTEGER")]
        public int PartyCount { get; set; } = 0;

        [Column("Addr", TypeName = "VARCHAR")]
        public string Addr { get; set; } = "";

        [Column("Addr1", TypeName = "VARCHAR")]
        public string Addr1 { get; set; } = "";

        [Column("CheckedAdmin", TypeName = "INTEGER")]
        public int CheckedAdmin { get; set; } = 0;

        [Column("Points", TypeName = "INTEGER")]
        public int Points { get; set; } = 0;

        [Column("Schools", TypeName = "VARCHAR")]
        public string Schools { get; set; } = "";

        [Column("ClassRooms", TypeName = "VARCHAR")]
        public string ClassRooms { get; set; } = "";

        [Column("ProtocolFileDoc", TypeName = "VARCHAR")]
        public string ProtocolFileDoc { get; set; } = "";

        [Column("Fios_1", TypeName = "VARCHAR")]
        public string Fios_1 { get; set; } = "";

        [Column("Agies_1", TypeName = "VARCHAR")]
        public string Agies_1 { get; set; } = "";

        [Column("Schools_1", TypeName = "VARCHAR")]
        public string Schools_1 { get; set; } = "";

        [Column("ClassRooms_1", TypeName = "VARCHAR")]
        public string ClassRooms_1 { get; set; } = "";

        [Column("Weights_1", TypeName = "VARCHAR")]
        public string Weights_1 { get; set; } = "";

        [Column("IsApply", TypeName = "INTEGER")]
        public int IsApply { get; set; } = 0;

        [Column("DateApply", TypeName = "INTEGER")]
        public int DateApply { get; set; } = 0;

        [Column("Division", TypeName = "VARCHAR")]
        public string Division { get; set; } = "";

        #endregion

    }
    #endregion

}
