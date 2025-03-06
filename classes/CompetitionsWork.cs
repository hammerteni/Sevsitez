// Файл с классами для работы с КОНКУРСАМИ. Файлы располагаются так:
// Материалы для конкурсов - 
// ~/files/competitionfiles/; 
// ~/files/competitionfiles/foto; 
// ~/files/competitionfiles/literary; 
// ~/files/competitionfiles/theatre
// База данных - 
// ~/files/sqlitedb/konkurses.db

#region Using

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using NameCaseLib;
using site.adm;
using site.classesHelp;
using site.dbContext;
using site.models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.UI;
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using Constants = site.classesHelp.Constants;

#endregion

namespace site.classes
{
    #region Код работы с данными

    #region CompetitonWorkCommon - общий класс для работы с заяками

    public class CompetitonWorkCommon
    {
        #region GetProjectName
        public static string projectName() {
            return "«Москва - Крым - Территория талантов». ";
        }
        #endregion

        #region GetDocFileName
        public static string GetDocFileName(DocumentType docType, string id)
        {
            Random rnd = new Random();      // для добавления случайной цифры в конец имени файла. Нужно для избежания ошибок при генерации файла.
            var fileName = "";

            if (docType == DocumentType.Diplom)              //диплом
            {
                fileName = "diplom_" + id + "_" + rnd.Next(100, 9999) + ".pdf";
            }
            else if (docType == DocumentType.Certificate)   //сертификат
            {
                fileName = "certificate_" + id + "_" + rnd.Next(100, 9999) + ".pdf";
            }
            else if (docType == DocumentType.Blagodarnost)  //благодарственное письмо
            {
                fileName = "thankletter_" + id + "_" + rnd.Next(100, 9999) + ".pdf";
            }
            else
                fileName = "unknown_" + id + "_" + rnd.Next(100, 9999) + ".pdf";

            return fileName;
        }

        #endregion

        #region GetBckgrdFileName
        public static string GetBckgrdFileName(DocumentType docType, long DateReg)
        {
            string pathToImg = HttpContext.Current.Server.MapPath("~") + @"images\";
            string bckgrdFileName = docType == DocumentType.Diplom ? "diplom" : docType == DocumentType.Certificate ? "certificate" : "thankletter";

            DateTime dt = new DateTime(DateReg);

            string pref = "_a";
            if (dt.Month < 10)
            {
                pref = "_b";
            }

            bckgrdFileName += pref + "_" + dt.Year + ".jpg";

            //если файла нет, используем шаблон по умолчанию
            if (!File.Exists(pathToImg + bckgrdFileName))
            {
                bckgrdFileName = bckgrdFileName.Replace(pref + "_" + dt.Year, "");
            }
        
            return bckgrdFileName;
        }
        #endregion

        #region YouTubeLinkParse
        public static string youTubeLinkParser(string link)
        {
            var result = link.IndexOf("/embed/") > 0 ? link : link.Replace("/watch?v=", "/embed/").Replace("/v/", "/embed/").Replace("youtu.be/", "youtube.com/embed/");
            if (result.IndexOf("&t=") > 0)
                result = result.Substring(0, result.IndexOf("&t="));

            return result;
        }
        #endregion

        #region Универсальный метод получения объекта заявки
        public static CompetitionRequest GetCompetitionRequest(string reqId) {

            var reqNew = new CompetitionsWork().GetOneRequest(reqId);
            object req = new object();

            if (reqNew != null && reqNew.Id != 0)
                req = reqNew;
            else
                req = new CompetitionsWork_Arch().GetOneRequest(reqId.ToString());

            return (CompetitionRequest)req;
        }
        #endregion

        public static string phonePattern = "^((\\+7|8)+([0-9]){10})$";
        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static string fioPattern1() { return "^[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+$"; }                    //Асов Ас Асович
        public static string fioPattern2() { return "^[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+-?[А-ЯЁ][а-яё]+$"; }   //Асов Ас Асович-Абдулаев
        public static string fioPattern3() { return "^[А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+$"; } //Асов-Абдулин Ас Асович-Абдулаев
        public static string fioPattern4() { return "^[А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+$"; }                   //Асов-Абдулин Ас Асович
        public static string fioPattern5() { return "^[А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+$"; }    //Асов-Абдулин-Заде Ас Асович

        public static string fioPattern6() { return "^[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+$"; }    //Асов Ева-Ангелина Асович
        public static string fioPattern7() { return "^[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+$"; }    //Асов Ева-Ангелина Асович-Абдулаев
        public static string fioPattern8() { return "^[А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+$"; }    //Асов-Абдулин Ева-Ангелина Асович
        public static string fioPattern9() { return "^[А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+-[А-ЯЁ][а-яё]+$"; }    //Асов-Абдулин Ева-Ангелина Асович-Абдулаев
        public static bool IsFioOk(string fio)
        {
            if (!Regex.IsMatch(fio.Trim(), fioPattern1()) && !Regex.IsMatch(fio.Trim(), fioPattern2()) && !Regex.IsMatch(fio.Trim(), fioPattern3()) && !Regex.IsMatch(fio.Trim(), fioPattern4()) && !Regex.IsMatch(fio.Trim(), fioPattern5()) && !Regex.IsMatch(fio.Trim(), fioPattern6()) && !Regex.IsMatch(fio.Trim(), fioPattern7()) && !Regex.IsMatch(fio.Trim(), fioPattern8()) && !Regex.IsMatch(fio.Trim(), fioPattern9()))
            {
                return false;
            }
            else
                return true;
        }

        // Номинации, которыми управляет админ editorPhotoCompGraphic
        public static string[] subnames_editorPhotoCompGraphic() {
            return new string[] {
                EnumsHelper.GetPhotoValue(Photo.computerGraphic),
                EnumsHelper.GetPhotoValue(Photo.computer_risunok),
                EnumsHelper.GetPhotoValue(Photo.collazh_fotomontazh)
            };
        }

        //// Номинации, которыми управляет админ editorPhotoCompGraphic (для архива)
        //public static string[] subnames_editorPhotoCompGraphic_arch() {
        //    return new string[] { 
        //        EnumsHelper.GetPhotoValue(Photo.computerGraphic),
        //        EnumsHelper.GetPhotoValue(Photo.computer_risunok),
        //        EnumsHelper.GetPhotoValue(Photo.collazh_fotomontazh) 
        //    };
        //}

        //Номинации, которыми управляет админ editorDPI1
        public static string[] subnames_editorDPI1() {
            return new string[]{
                EnumsHelper.GetPhotoValue(Photo.DPT1_avtor_igrushka),
                EnumsHelper.GetPhotoValue(Photo.DPT1_biseropletenie),
                EnumsHelper.GetPhotoValue(Photo.DPT1_hud_vyazanie),
                EnumsHelper.GetPhotoValue(Photo.DPT1_bumagoplastika),
                EnumsHelper.GetPhotoValue(Photo.DPT1_voilokovalyanie),

                //Объединение DPT1 и DPT2
                EnumsHelper.GetPhotoValue(Photo.DPT2_batik),
                EnumsHelper.GetPhotoValue(Photo.DPT2_dekupazh),
                EnumsHelper.GetPhotoValue(Photo.DPT2_keramika),
                EnumsHelper.GetPhotoValue(Photo.DPT2_rospis_poderevu),
                EnumsHelper.GetPhotoValue(Photo.DPT1_combitehnics),
                EnumsHelper.GetPhotoValue(Photo.DPT1_plastilonografiya),
            };
        }

        //Номинации, которыми управляет админ editorDPI1 (для архива)
        public static string[] subnames_editorDPI1_arch()
        {
            return new string[] {

                EnumsHelper.GetPhotoValue(Photo.DPT1_makrame),
                EnumsHelper.GetPhotoValue(Photo.DPT1_gobelen),
                EnumsHelper.GetPhotoValue(Photo.DPT1_hud_vishivka),
                EnumsHelper.GetPhotoValue(Photo.DPT1_hud_vyazanie),
                EnumsHelper.GetPhotoValue(Photo.DPT1_bumagoplastika),
                EnumsHelper.GetPhotoValue(Photo.DPT1_loskut_shitie),
                EnumsHelper.GetPhotoValue(Photo.DPT1_avtor_igrushka),
                EnumsHelper.GetPhotoValue(Photo.DPT1_voilokovalyanie),
                EnumsHelper.GetPhotoValue(Photo.DPT1_biseropletenie),
                EnumsHelper.GetPhotoValue(Photo.DPT1_fitodisign),
                EnumsHelper.GetPhotoValue(Photo.DPT1_combitehnics),
                EnumsHelper.GetPhotoValue(Photo.DPT1_plastilonografiya),

                //Объединение DPT1 и DPT2
                EnumsHelper.GetPhotoValue(Photo.DPT2_keramika),
                EnumsHelper.GetPhotoValue(Photo.DPT2_hud_obr_stekla),
                EnumsHelper.GetPhotoValue(Photo.DPT2_hud_obr_kozhi),
                EnumsHelper.GetPhotoValue(Photo.DPT2_narod_igrush_isglini),
                EnumsHelper.GetPhotoValue(Photo.DPT2_batik),
                EnumsHelper.GetPhotoValue(Photo.DPT2_dekupazh),
                EnumsHelper.GetPhotoValue(Photo.DPT2_rospis_poderevu)
            };
        }

        ////Номинации, которыми управляет админ editorDPI2
        //public string[] subnames_editorDPI2 = {
        //  EnumsHelper.GetPhotoValue(Photo.DPT2_batik),
        //  EnumsHelper.GetPhotoValue(Photo.DPT2_dekupazh),
        //  EnumsHelper.GetPhotoValue(Photo.DPT2_keramika),
        //  EnumsHelper.GetPhotoValue(Photo.DPT2_rospis_poderevu)
        //};

        ////Номинации, которыми управляет админ editorDPI2 (для архива)
        //public string[] subnames_editorDPI2_arch = {
        //  EnumsHelper.GetPhotoValue(Photo.DPT2_keramika),
        //  EnumsHelper.GetPhotoValue(Photo.DPT2_hud_obr_stekla),
        //  EnumsHelper.GetPhotoValue(Photo.DPT2_hud_obr_kozhi),
        //  EnumsHelper.GetPhotoValue(Photo.DPT2_narod_igrush_isglini),
        //  EnumsHelper.GetPhotoValue(Photo.DPT2_batik),
        //  EnumsHelper.GetPhotoValue(Photo.DPT2_dekupazh),
        //  EnumsHelper.GetPhotoValue(Photo.DPT2_rospis_poderevu)
        //};

        //Номинации, которыми управляет админ editorTheatreInstrumZanr
        public static string[] subnames_editorTheatreInstrumZanr()
        {
            return new string[] {
                EnumsHelper.GetTheatreValue(Theatre.insrumZanrAnsambli),
                EnumsHelper.GetTheatreValue(Theatre.insrumZanrDuhovieUdarnInstrum),
                EnumsHelper.GetTheatreValue(Theatre.insrumZanrFortepiano),
                EnumsHelper.GetTheatreValue(Theatre.insrumZanrGitara),
                EnumsHelper.GetTheatreValue(Theatre.insrumZanrNarodnieInstrum),
                EnumsHelper.GetTheatreValue(Theatre.insrumZanrOrkestri),
                EnumsHelper.GetTheatreValue(Theatre.insrumZanrSintezator),
                EnumsHelper.GetTheatreValue(Theatre.insrumZanrStrunnoSmichkovieInstrumenti)
            };
        }
        //Номинации, которыми управляет админ editorTheatreInstrumZanr
        public static string[] subnames_editorTheatreVokal()
        {
            return new string[] {
                EnumsHelper.GetTheatreValue(Theatre.vokalAkademVokal),
                EnumsHelper.GetTheatreValue(Theatre.vokalEstradVokal),
                EnumsHelper.GetTheatreValue(Theatre.vokalFolklor)
            };
        }
        //Номинации, которыми управляет админ editorTheatre
        public static string[] subnames_editorTheatre()
        {
            return new string[] { EnumsHelper.GetTheatreValue(Theatre.teatrIskusLitMuzKom),
                EnumsHelper.GetTheatreValue(Theatre.teatrIskusSpekt),
                EnumsHelper.GetTheatreValue(Theatre.teatrIskusMultiGanr)};
        }
        //Номинации, которыми управляет админ editorTheatreHoreo
        public static string[] subnames_editorTheatreHoreo()
        {
            return new string[] { EnumsHelper.GetTheatreValue(Theatre.xoreoBalniyTanets),
                EnumsHelper.GetTheatreValue(Theatre.xoreoClassichTanets),
                EnumsHelper.GetTheatreValue(Theatre.xoreoEstradTanets),
                EnumsHelper.GetTheatreValue(Theatre.xoreoNarodTanets),
                EnumsHelper.GetTheatreValue(Theatre.xoreoStilNarodTanets),
                EnumsHelper.GetTheatreValue(Theatre.xoreoSovremTanets),
                EnumsHelper.GetTheatreValue(Theatre.xoreoCircIskus),
                EnumsHelper.GetTheatreValue(Theatre.xoreoKadeti),
                EnumsHelper.GetTheatreValue(Theatre.xoreoCherliding)
            };
        }
        //Номинации, которыми управляет админ editorTheatreModa
        public static string[] subnames_editorTheatreModa()
        {
            return new string[] { EnumsHelper.GetClothesValue(Clothes.tmavtorcollect),
                EnumsHelper.GetClothesValue(Clothes.tmcollectpokaz),
                EnumsHelper.GetClothesValue(Clothes.tmindividpokaz),
                EnumsHelper.GetClothesValue(Clothes.tmnetradicmaterial),
               //EnumsHelper.GetClothesValue(Clothes.tmissledovproject)
            };
        }
        //Номинации, которыми управляет админ editorLiterary
        public static string[] subnames_editorLiterary()
        {
            return new string[] { EnumsHelper.GetLiteraryValue(Literary.stihi),
                EnumsHelper.GetLiteraryValue(Literary.esse),
                EnumsHelper.GetLiteraryValue(Literary.rasskaz),
                EnumsHelper.GetLiteraryValue(Literary.sochinenie)};
        }
        //Номинации, которыми управляет админ editorKultura
        public static string[] subnames_editorKultura()
        {
            return new string[] { //EnumsHelper.GetKulturaValue(Kultura.iSeeCrimea),
                //EnumsHelper.GetKulturaValue(Kultura.vossoedCrimaSRossiei),
                //EnumsHelper.GetKulturaValue(Kultura.zashitaobsihgranits),
                //EnumsHelper.GetKulturaValue(Kultura.kultrnNasledieCrima),
                //EnumsHelper.GetKulturaValue(Kultura.krimskyMostVSZ),
                EnumsHelper.GetKulturaValue(Kultura.presentaionEn),
                EnumsHelper.GetKulturaValue(Kultura.iSeeCrimeaEn),
                EnumsHelper.GetKulturaValue(Kultura.publishVkontakte),
                EnumsHelper.GetKulturaValue(Kultura.audioGaid),
                EnumsHelper.GetKulturaValue(Kultura.intellektualKviz),
            };
        }
        //Номинации, которыми управляет админ editorToponim
        public static string[] subnames_editorToponim()
        {
            return new string[] { EnumsHelper.GetToponimValue(Toponim.toponimika) };
        }
        // Номинации, которыми управляет админ editorRobotech
        public static string[] subnames_editorRobotech()
        {
            return new string[] { EnumsHelper.GetRobotechValue(Robotech.robototechnika),
                                EnumsHelper.GetRobotechValue(Robotech.robototechnikaproject),
                                EnumsHelper.GetRobotechValue(Robotech.robototechnika3dmodel),
                                EnumsHelper.GetRobotechValue(Robotech.tinkercad),
                                EnumsHelper.GetRobotechValue(Robotech.programmproject),
                            };
        }
        //// Номинации, которыми управляет админ editorRobotech (для архива)
        //public static string[] subnames_editorRobotech_arch()
        //{
        //  return new string[] { EnumsHelper.GetRobotechValue(Robotech.robotech),
        //      EnumsHelper.GetRobotechValue(Robotech.robototechnika),
        //      EnumsHelper.GetRobotechValue(Robotech.robototechnikaproject),
        //      EnumsHelper.GetRobotechValue(Robotech.robototechnika3dmodel) ,
        //      EnumsHelper.GetRobotechValue(Robotech.tinkercad)};
        //}
        // Номинации, которыми управляет админ editorVmesteSila
        public static string[] subnames_editorVmesteSila()
        {
            return new string[] { 
                EnumsHelper.GetVmesteSilaValue(VmesteSila.hudSlovoPoeziya),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.hudSlovoProza),

                EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaBalniyTanets),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaClassichTanets),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaEstradTanets),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaNarodTanets),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaSovremenTanets),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaOstalnieGanri),

                EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalAkademVokal),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalEstradVokal),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalFolklor),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalZest),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalOstalnieGanri),

                EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrFortepiano),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrSintezator),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrStrunnoSmichkovieInstrumenti),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrDuhovieUdarnInstrum),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrNarodnieInstrum),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrGitara),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrAnsambli),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrOstalnieGanri),

                EnumsHelper.GetVmesteSilaValue(VmesteSila.theatreSpektakl),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.theatreScenka),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.theatreLiteraturnoMusikalnaya),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.theatreDrama),

                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupDay),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupNight),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupStsena),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupFantasy),

                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairPletenie),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairDay),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairNight),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairFantasy),
            };
        }

        //// Номинации, которыми управляет админ editorVmesteSila (для архива)
        //public static string[] subnames_editorVmesteSila_arch()
        //{
        //    return new string[] { EnumsHelper.GetVmesteSilaValue(VmesteSila.hudSlovo),
        //    EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographia),
        //    EnumsHelper.GetVmesteSilaValue(VmesteSila.vokal),
        //    EnumsHelper.GetVmesteSilaValue(VmesteSila.instrumental),
        //    EnumsHelper.GetVmesteSilaValue(VmesteSila.theatre),};
        //}

        public static string[] subnames_editorVmesteSilaMakeUp()
        {
            return new string[] { 

                //EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeup),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupDay),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupNight),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupStsena),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupFantasy),

            };
        }

        public static string[] subnames_editorVmesteSilaShair()
        {
            return new string[] {

                //EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShair),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairPletenie),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairDay),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairNight),
                EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairFantasy),
            };
        }

        // Номинации, которыми управляет админ editoreditorClothes
        public static string[] subnames_editorClothes() {
            return new string[]
{
                //EnumsHelper.GetClothesValue(Clothes.uniyKuturie),
                EnumsHelper.GetClothesValue(Clothes.uniyKuturieTkan),
                EnumsHelper.GetClothesValue(Clothes.uniyKuturieNetradicMaterial),
                EnumsHelper.GetClothesValue(Clothes.uniyKuturieFashion),
                EnumsHelper.GetClothesValue(Clothes.uniyKuturieTechRisunok),
                EnumsHelper.GetClothesValue(Clothes.uniyKuturieFoodArt),
                EnumsHelper.GetClothesValue(Clothes.uniyKuturieOgorod),
                EnumsHelper.GetClothesValue(Clothes.uniyKuturieBeauty),

                EnumsHelper.GetClothesValue(Clothes.chudoLoskutkiIgrushkiKukliTvorRisunok),
                EnumsHelper.GetClothesValue(Clothes.chudoLoskutkiIgrushkiKukli),


         

                //EnumsHelper.GetClothesValue(Clothes.eskiziModelier),
                //EnumsHelper.GetClothesValue(Clothes.eskiziModelierTvorRisunok),
                //EnumsHelper.GetClothesValue(Clothes.eskiziModelierFashion),
                //EnumsHelper.GetClothesValue(Clothes.eskiziModelierTechRisunok),

                //EnumsHelper.GetClothesValue(Clothes.sedobnayaModa),
                //EnumsHelper.GetClothesValue(Clothes.sedobnayaModaFoodArt),
                //EnumsHelper.GetClothesValue(Clothes.sedobnayaModaOgorod),
                //EnumsHelper.GetClothesValue(Clothes.sedobnayaModaBeauty)
            };
        }

        //Номинации, которыми управляет админ editoreditorClothes(для архива)
        //public static string[] subnames_editorClothes_arch()
        //{
        //    return new string[] {
        //      EnumsHelper.GetClothesValue(Clothes.uniyKuturie),
        //      EnumsHelper.GetClothesValue(Clothes.chudoLoskutki),
        //      EnumsHelper.GetClothesValue(Clothes.eskiziModelier),
        //      EnumsHelper.GetClothesValue(Clothes.sedobnayaModa),
        //   };
        //}

        // Номинации, которыми управляет админ editorMultimedia
        public static string[] subnames_editorMultimedia()
        {
            return new string[]{
                //EnumsHelper.GetMultimediaValue(Multimedia.uniygeroi),
                //EnumsHelper.GetMultimediaValue(Multimedia.uniyzashitnik)
                //EnumsHelper.GetMultimediaValue(Multimedia.characteristica),
                //EnumsHelper.GetMultimediaValue(Multimedia.prichinietapi),
                //EnumsHelper.GetMultimediaValue(Multimedia.rolvistorii)

                //EnumsHelper.GetMultimediaValue(Multimedia.podandreevskimflagom),
                //EnumsHelper.GetMultimediaValue(Multimedia.pomoryampovolnam),
                //EnumsHelper.GetMultimediaValue(Multimedia.korablimorykimore),
                //EnumsHelper.GetMultimediaValue(Multimedia.chernomorskomurossiiposvyashaetsa),
                //EnumsHelper.GetMultimediaValue(Multimedia.metodicheskierazrabotki),

                //EnumsHelper.GetMultimediaValue(Multimedia.morepobedkolibelsmelchakov),
                //EnumsHelper.GetMultimediaValue(Multimedia.netzapyatihtolkotochki),
                //EnumsHelper.GetMultimediaValue(Multimedia.vashipodvigineumrut),
                //EnumsHelper.GetMultimediaValue(Multimedia.sevastopol44),
                //EnumsHelper.GetMultimediaValue(Multimedia.nadevaemmitelnyashku),
                //EnumsHelper.GetMultimediaValue(Multimedia.klyatvudaemsevastopolvernem),
                //EnumsHelper.GetMultimediaValue(Multimedia.hranitalbomsemeinipamyat),
                //EnumsHelper.GetMultimediaValue(Multimedia.ihpamaytuzivushiipoklonis),
                //EnumsHelper.GetMultimediaValue(Multimedia.multimediinieizdaniya),

                EnumsHelper.GetMultimediaValue(Multimedia.yarisuupobedy),
                EnumsHelper.GetMultimediaValue(Multimedia.spesneirpobede),
                EnumsHelper.GetMultimediaValue(Multimedia.geroyamotserdca),
                EnumsHelper.GetMultimediaValue(Multimedia.plechomkplechu),
                EnumsHelper.GetMultimediaValue(Multimedia.pamyatsilneevremeni),

            };
        }
        //// Номинации, которыми управляет админ editorMultimedia (для архива)
        //public static string[] subnames_editorMultimedia_arch()
        //{
        //    return new string[] {
        //        EnumsHelper.GetMultimediaValue(Multimedia.uniygeroi),
        //        EnumsHelper.GetMultimediaValue(Multimedia.uniyzashitnik),
        //        EnumsHelper.GetMultimediaValue(Multimedia.characteristica),
        //        EnumsHelper.GetMultimediaValue(Multimedia.prichinietapi),
        //        EnumsHelper.GetMultimediaValue(Multimedia.rolvistorii)
        //     };
        //}

        // Номинации, которыми управляет админ editorKorablik
        public static string[] subnames_editorKorablik()
        {
            return new string[]{
                EnumsHelper.GetKorablikValue(Korablik.hudSlovo),
            };
        }

        // Номинации, которыми управляет админ editorKorablikVokal
        public static string[] subnames_editorKorablikVokal()
        {
            return new string[]{
                //EnumsHelper.GetKorablikValue(Korablik.vokal),
                EnumsHelper.GetKorablikValue(Korablik.vokalSolo),
                EnumsHelper.GetKorablikValue(Korablik.vokalMalieFormi),
                EnumsHelper.GetKorablikValue(Korablik.vokalAnsambli)
            };
        }

        // Номинации, которыми управляет админ editorKorablik
        public static string[] subnames_editorKorablikHoreo()
        {
            return new string[]{
                EnumsHelper.GetKorablikValue(Korablik.horeographia),
            };
        }

        //// Номинации, которыми управляет админ editorKorablik (для архива)
        //public static string[] subnames_editorKorablik_arch()
        //{
        //    return new string[] { 
        //        EnumsHelper.GetKorablikValue(Korablik.hudSlovo),
        //        EnumsHelper.GetKorablikValue(Korablik.horeographia),
        //        EnumsHelper.GetKorablikValue(Korablik.vokal) 
        //    };
        //}

        // Номинации, которыми управляет админ editorCrimroute
        public static string[] subnames_editorCrimroute()
        {
            return new string[] {
                EnumsHelper.GetCrimrouteValue(Crimroute.historyplace),
                EnumsHelper.GetCrimrouteValue(Crimroute.militaryhistoryplace),
                EnumsHelper.GetCrimrouteValue(Crimroute.literaturehistoryplace)
            };
        }
        //// Номинации, которыми управляет админ editorCrimroute (для архива)
        //public static string[] subnames_editorCrimroute_arch()
        //{
        //    return new string[] { 
        //        EnumsHelper.GetCrimrouteValue(Crimroute.historyplace),
        //        EnumsHelper.GetCrimrouteValue(Crimroute.militaryhistoryplace),
        //        EnumsHelper.GetCrimrouteValue(Crimroute.literaturehistoryplace) 
        //    };
        //}

        // Номинации, которыми управляет админ editorMathbattle
        public static string[] subnames_editorMathbattle()
        {
            return new string[] { EnumsHelper.GetMathbattleValue(Mathbattle.battle) };
        }

        //// Номинации, которыми управляет админ editorMathbattle (для архива)
        //public static string[] subnames_editorMathbattle_arch()
        //{
        //    return new string[] { EnumsHelper.GetMathbattleValue(Mathbattle.battle) };
        //}

        // Номинации, которыми управляет админ editorKosmos
        public static string[] subnames_editorKosmos()
        {
            return new string[] { EnumsHelper.GetKosmosValue(Kosmos.kosmos) };
        }

        //// Номинации, которыми управляет админ editorMathbattle (для архива)
        //public static string[] subnames_editorKosmos_arch()
        //{
        //    return new string[] { EnumsHelper.GetKosmosValue(Kosmos.kosmos) };
        //}

        // Номинации, которыми управляет админ subnames_editorScience
        public static string[] subnames_editorScience()
        {
            return new string[] {
                EnumsHelper.GetScienceValue(Science.ekologia_ochno),
                EnumsHelper.GetScienceValue(Science.ekologia_zaochno),
                EnumsHelper.GetScienceValue(Science.himiya_ochno),
                EnumsHelper.GetScienceValue(Science.himiya_zaochno),
                EnumsHelper.GetScienceValue(Science.fizika_ochno),
                EnumsHelper.GetScienceValue(Science.fizika_zaochno),
                EnumsHelper.GetScienceValue(Science.biologiya_ochno),
                EnumsHelper.GetScienceValue(Science.biologiya_zaochno)
            };
        }
        //// Номинации, которыми управляет админ subnames_editorScience (для архива)
        //public static string[] subnames_editorScience_arch()
        //{
        //    return new string[] {
        //        EnumsHelper.GetScienceValue(Science.ekologia_ochno),
        //        EnumsHelper.GetScienceValue(Science.ekologia_zaochno),
        //        EnumsHelper.GetScienceValue(Science.himiya_ochno),
        //        EnumsHelper.GetScienceValue(Science.himiya_zaochno),
        //        EnumsHelper.GetScienceValue(Science.fizika_ochno),
        //        EnumsHelper.GetScienceValue(Science.fizika_zaochno),
        //        EnumsHelper.GetScienceValue(Science.biologiya_ochno),
        //        EnumsHelper.GetScienceValue(Science.biologiya_zaochno)
        //    };
        //}
    }




    #endregion

    #region Класс CompetitionsWork

    /// <summary>Класс формирования данных по конкурсам. Обслуживает класс CompetitionsForm</summary>
    public class CompetitionsWork
    {
        #region Поля

        private HttpContext _context;

        private string _pathToDb;
        private string _tableName;

        private string _pathToMainFolder;
        private string _pathToFotoFolder;
        private string _pathToLiteraryFolder;
        private string _pathToTheatreFolder;
        private string _pathToVmesteSilaFolder;
        private string _pathToClothesFolder;
        private string _pathToSportFolder;
        private string _pathToKulturaFolder;
        private string _pathToToponimFolder;
        private string _pathToProtocolFolder;
        //private string _pathToTempFolder;
        private string _pathToMultimediaFolder;
        private string _pathToKorablikFolder;
        private string _pathToCrimrouteFolder;
        private string _pathToMathbattleFolder;
        private string _pathToKosmosFolder;
        private string _pathToScienceFolder;

        private string _imgUrlPathFoto;
        private string _imgUrlPathLiterary;
        private string _imgUrlPathTheatre;
        private string _imgUrlPathVmesteSila;
        private string _imgUrlPathClothes;
        private string _imgUrlPathSport;
        private string _imgUrlPathKultura;
        private string _imgUrlPathToponim;
        private string _imgUrlPathMultimedia;
        private string _imgUrlPathProtocol;
        private string _imgUrlPathKorablik;
        private string _imgUrlPathCrimroute;
        private string _imgUrlPathMathbattle;
        private string _imgUrlPathKosmos;
        private string _imgUrlPathScience;

        #endregion

        #region Конструктор класса

        /// <summary>Конструктор класса. Добавляет в БД таблицу конкурсов, если её ещё не существует.
        /// Так же инициализирует поля.</summary>
        public CompetitionsWork()
        {
            _context = HttpContext.Current;
            _pathToDb = _context.Server.MapPath("~") + @"files\sqlitedb\konkurses.db";
            _tableName = "competitions";

            _pathToMainFolder = _context.Server.MapPath("~") + @"files\competitionfiles\";

            _pathToFotoFolder = _pathToMainFolder + @"foto\";
            _pathToLiteraryFolder = _pathToMainFolder + @"literary\";
            _pathToTheatreFolder = _pathToMainFolder + @"theatre\";
            _pathToVmesteSilaFolder = _pathToMainFolder + @"vmestesila\";
            _pathToClothesFolder = _pathToMainFolder + @"clothes\";
            _pathToSportFolder = _pathToMainFolder + @"sport\";
            _pathToKulturaFolder = _pathToMainFolder + @"kultura\";
            _pathToToponimFolder = _pathToMainFolder + @"toponim\";
            _pathToProtocolFolder = _pathToMainFolder + @"protocols\";
            _pathToMultimediaFolder = _pathToMainFolder + @"multimedia\";
            _pathToKorablikFolder = _pathToMainFolder + @"korablik\";
            _pathToCrimrouteFolder = _pathToMainFolder + @"crimroute\";
            _pathToMathbattleFolder = _pathToMainFolder + @"mathbattle\";
            _pathToKosmosFolder = _pathToMainFolder + @"kosmos\";
            _pathToScienceFolder = _pathToMainFolder + @"science\";

            _imgUrlPathFoto = "~/files/competitionfiles/foto/";
            _imgUrlPathLiterary = "~/files/competitionfiles/literary/";
            _imgUrlPathTheatre = "~/files/competitionfiles/theatre/";
            _imgUrlPathVmesteSila = "~/files/competitionfiles/vmestesila/";
            _imgUrlPathClothes = "~/files/competitionfiles/clothes/";
            _imgUrlPathSport = "~/files/competitionfiles/sport/";
            _imgUrlPathKultura = "~/files/competitionfiles/kultura/";
            _imgUrlPathToponim = "~/files/competitionfiles/toponim/";
            _imgUrlPathProtocol = "~/files/competitionfiles/protocols/";
            _imgUrlPathMultimedia = "~/files/competitionfiles/multimedia/";
            _imgUrlPathKorablik = "~/files/competitionfiles/korablik/";
            _imgUrlPathCrimroute = "~/files/competitionfiles/crimroute/";
            _imgUrlPathMathbattle = "~/files/competitionfiles/mathbattle/";
            _imgUrlPathKosmos = "~/files/competitionfiles/kosmos/";
            _imgUrlPathScience = "~/files/competitionfiles/science/";

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            string sqlString = "CREATE TABLE IF NOT EXISTS " + _tableName + " (" +
                " '_id' INTEGER PRIMARY KEY AUTOINCREMENT, " +
                " 'Fio' TEXT NOT NULL DEFAULT ('')," +
                " 'Class_' TEXT NOT NULL DEFAULT ('')," +
                " 'Age' TEXT NOT NULL DEFAULT ('')," +
                " 'WorkName' TEXT NOT NULL DEFAULT ('')," +
                " 'WorkComment' TEXT NOT NULL DEFAULT ('')," +
                " 'EducationalOrganization' TEXT NOT NULL DEFAULT ('')," +
                " 'Email' TEXT NOT NULL DEFAULT ('')," +
                " 'Telephone' TEXT NOT NULL DEFAULT ('')," +
                " 'Addr' TEXT NOT NULL DEFAULT ('')," +
                " 'Addr1' TEXT NOT NULL DEFAULT ('')," +
                " 'PostIndex' INTEGER NOT NULL DEFAULT (0)," +
                " 'Region' TEXT NOT NULL DEFAULT ('')," +
                " 'City' TEXT NOT NULL DEFAULT ('')," +
                " 'Street' TEXT NOT NULL DEFAULT ('')," +
                " 'Home' TEXT NOT NULL DEFAULT ('')," +
                " 'Room' TEXT NOT NULL DEFAULT ('')," +
                " 'ChiefFio' TEXT NOT NULL DEFAULT ('')," +
                " 'ChiefPosition' TEXT NOT NULL DEFAULT ('')," +
                " 'ChiefEmail' TEXT NOT NULL DEFAULT ('')," +
                " 'ChiefTelephone' TEXT NOT NULL DEFAULT ('')," +
                " 'SubsectionName' TEXT NOT NULL DEFAULT ('')," +
                " 'Fios' TEXT NOT NULL DEFAULT ('')," +
                " 'Agies' TEXT NOT NULL DEFAULT ('')," +
                " 'Links' TEXT NOT NULL DEFAULT ('')," +
                " 'CompetitionName' TEXT NOT NULL DEFAULT ('')," +
                " 'DateReg' INTEGER NOT NULL DEFAULT (0)," +
                " 'Likes' INTEGER NOT NULL DEFAULT (0)," +
                " 'Nolikes' INTEGER NOT NULL DEFAULT (0)," +
                " 'Approved' INTEGER NOT NULL DEFAULT (0)," +
                " 'PdnProcessing' INTEGER NOT NULL DEFAULT (0)," +
                " 'PublicAgreement' INTEGER NOT NULL DEFAULT (0)," +
                " 'ProcMedicine' INTEGER NOT NULL DEFAULT (0)," +
                " 'SummaryLikes' INTEGER NOT NULL DEFAULT (0)," +
                " 'ClubsName' TEXT NOT NULL DEFAULT ('')," +
                " 'Weight' INTEGER NOT NULL DEFAULT ('')," +
                " 'Weights' TEXT NOT NULL DEFAULT ('')," +
                " 'AgeСategories' TEXT NOT NULL DEFAULT ('')," +
                " 'Kvalifications' TEXT NOT NULL DEFAULT ('')," +
                " 'Programms' TEXT NOT NULL DEFAULT ('')," +
                " 'Result' TEXT NOT NULL DEFAULT ('')," +
                " 'Results' TEXT NOT NULL DEFAULT ('')," +
                " 'ProtocolFile' TEXT NOT NULL DEFAULT ('')," +
                " 'ProtocolPartyCount' INTEGER NOT NULL DEFAULT (0)," +
                " 'TechnicalInfo' TEXT NOT NULL DEFAULT ('')," +
                " 'Timing_min' INTEGER NOT NULL DEFAULT (0)," +
                " 'Timing_sec' INTEGER NOT NULL DEFAULT (0)," +
                " 'ChiefFios' TEXT NOT NULL DEFAULT ('')," +
                " 'ChiefPositions' TEXT NOT NULL DEFAULT ('')," +
                " 'AthorsFios' TEXT NOT NULL DEFAULT ('')," +
                " 'AgeСategory' TEXT NOT NULL DEFAULT ('')," +
                " 'PartyCount' INTEGER NOT NULL DEFAULT (0)," +
                " 'CheckedAdmin' INTEGER NOT NULL DEFAULT (0)," +
                " 'Points' INTEGER NOT NULL DEFAULT (0), " +
                " 'Schools' TEXT NOT NULL DEFAULT (''), " +
                " 'ClassRooms' TEXT NOT NULL DEFAULT (''), " +
                " 'ProtocolFileDoc' TEXT NOT NULL DEFAULT (''), " +
                " 'Fios_1' TEXT NOT NULL DEFAULT ('')," +
                " 'Agies_1' TEXT NOT NULL DEFAULT ('')," +
                " 'Schools_1' TEXT NOT NULL DEFAULT (''), " +
                " 'ClassRooms_1' TEXT NOT NULL DEFAULT (''), " +
                " 'Weights_1' TEXT NOT NULL DEFAULT (''), " +
                " 'IsApply' INTEGER NOT NULL DEFAULT (0)," +
                " 'DateApply' INTEGER NOT NULL DEFAULT (0)," +
                " 'Division' TEXT NOT NULL DEFAULT ('')" +
                ")";
            sqlite.ExecuteNonQuery(sqlString);
            sqlite.ConnectionClose();
        }

        #endregion

        #region GetPropertyValue
        public static string GetPropertyValue(Object obj, string propertyName)
        {
            Type t = obj.GetType();
            PropertyInfo[] props = t.GetProperties();
            PropertyInfo prop1 = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (props != null && prop1 != null)
            {
                foreach (var prop in props)
                {
                    try
                    {
                        if (prop != null && prop.Name != null && prop.Name == propertyName)
                            return prop.GetValue(obj).ToString();
                    }
                    catch (Exception ex1)
                    {
                        try
                        {
                            DebugLog.Log(ErrorEvents.err, "CompetitionsWork", MethodBase.GetCurrentMethod().Name, "Текст ошибки: " + ex1.Message + ". Строка: " + ex1.StackTrace);
                        }
                        catch { }

                        break;
                    }
                }
            }
            return "";
        }
        #endregion

        #region SetPropertyValue
        public static void SetPropertyValue(ref CompetitionRequest req, string propertyName, string propertyValue)
        {
            Type examType = typeof(CompetitionRequest);
            PropertyInfo piInstance = examType.GetProperty(propertyName);
            //object pValue = (piInstance.PropertyType.Name == "Int32" ? int.Parse(propertyValue) : propertyValue);
            piInstance.SetValue(req, propertyValue);
        }
        #endregion

        #region Метод int InsertOneRequest(CompetitionRequest req)

        /// <summary>Метод добавляет в БД одну запись с данными из переданного объекта CompetitionRequest.</summary>
        /// <param name="req">объекта CompetitionRequest с данными по заявке участника конкурса</param>
        /// <returns>Метод возвращает номер принятой заявки или -1 в случае какой-либо ошибки</returns>
        public int InsertOneRequest(ref CompetitionRequest req)
        {
            int result = 0;

            try
            {
                #region Код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "INSERT INTO " + _tableName + " (" +
                                                           "Fio, " +
                                                           "Class_, " +
                                                           "Age, " +
                                                           "WorkName, " +
                                                           "WorkComment, " +
                                                           "EducationalOrganization, " +
                                                           "EducationalOrganizationShort, " +
                                                           "Email, " +
                                                           "Telephone, " +
                                                           "Addr, " +
                                                           "Addr1, " +
                                                           "District, " +
                                                           "Region, " +
                                                           "Area, " +
                                                           "City, " +
                                                           "ChiefFio, " +
                                                           "ChiefPosition, " +
                                                           "ChiefEmail, " +
                                                           "ChiefTelephone, " +
                                                           "SubsectionName, " +
                                                           "Fios, " +
                                                           "Agies, " +
                                                           "Links, " +
                                                           "CompetitionName, " +
                                                           "DateReg, " +
                                                           "Likes, " +
                                                           "Nolikes, " +
                                                           "Approved, " +
                                                           "PdnProcessing, " +
                                                           "PublicAgreement, " +
                                                           "ProcMedicine, " +
                                                           "SummaryLikes, " +
                                                           "ClubsName, " +
                                                           "Weight, " +
                                                           "Weights, " +
                                                           "AgeСategories, " +
                                                           "Kvalifications, " +
                                                           "Programms, " +
                                                           "Result, " +
                                                           "Results, " +
                                                           "ProtocolFile, " +
                                                           "ProtocolPartyCount, " +
                                                           "TechnicalInfo, " +
                                                           "Timing_min, " +
                                                           "Timing_sec, " +
                                                           "ChiefFios, " +
                                                           "ChiefPositions, " +
                                                           "AthorsFios, " +
                                                           "AgeСategory, " +
                                                           "PartyCount," +
                                                           "CheckedAdmin," +
                                                           "Points," +
                                                           "Schools," +
                                                           "ClassRooms," +
                                                           "ProtocolFileDoc," +
                                                           "Fios_1," +
                                                           "Agies_1," +
                                                           "Schools_1," +
                                                           "ClassRooms_1," +
                                                           "Weights_1," +
                                                           "IsApply," +
                                                           "DateApply," +
                                                           "Division" +
                                                           ") " +
                                                "VALUES (" +
                                                           "@Fio, " +
                                                           "@Class_, " +
                                                           "@Age, " +
                                                           "@WorkName, " +
                                                           "@WorkComment, " +
                                                           "@EducationalOrganization, " +
                                                           "@EducationalOrganizationShort, " +
                                                           "@Email, " +
                                                           "@Telephone, " +
                                                           "@Addr, " +
                                                           "@Addr1, " +
                                                           "@District, " +
                                                           "@Region, " +
                                                           "@Area, " +
                                                           "@City, " +
                                                           "@ChiefFio, " +
                                                           "@ChiefPosition, " +
                                                           "@ChiefEmail, " +
                                                           "@ChiefTelephone, " +
                                                           "@SubsectionName, " +
                                                           "@Fios, " +
                                                           "@Agies, " +
                                                           "@Links, " +
                                                           "@CompetitionName, " +
                                                           "@DateReg, " +
                                                           "@Likes, " +
                                                           "@Nolikes, " +
                                                           "@Approved, " +
                                                           "@PdnProcessing, " +
                                                           "@PublicAgreement, " +
                                                           "@ProcMedicine, " +
                                                           "@SummaryLikes, " +
                                                           "@ClubsName, " +
                                                           "@Weight, " +
                                                           "@Weights, " +
                                                           "@AgeСategories, " +
                                                           "@Kvalifications, " +
                                                           "@Programms, " +
                                                           "@Result, " +
                                                           "@Results, " +
                                                           "@ProtocolFile, " +
                                                           "@ProtocolPartyCount, " +
                                                           "@TechnicalInfo, " +
                                                           "@Timing_min, " +
                                                           "@Timing_sec, " +
                                                           "@ChiefFios, " +
                                                           "@ChiefPositions, " +
                                                           "@AthorsFios, " +
                                                           "@AgeСategory, " +
                                                           "@PartyCount," +
                                                           "@CheckedAdmin, " +
                                                           "@Points," +
                                                           "@Schools," +
                                                           "@ClassRooms," +
                                                           "@ProtocolFileDoc," +
                                                           "@Fios_1," +
                                                           "@Agies_1," +
                                                           "@Schools_1," +
                                                           "@ClassRooms_1," +
                                                           "@Weights_1," +
                                                           "@IsApply," +
                                                           "@DateApply," +
                                                           "@Division" +
                                                        ")";

                cmd.Parameters.Add(new SQLiteParameter("@Fio", req.Fio));
                cmd.Parameters.Add(new SQLiteParameter("@Class_", req.Class_));
                cmd.Parameters.Add(new SQLiteParameter("@Age", req.Age));
                cmd.Parameters.Add(new SQLiteParameter("@WorkName", req.WorkName));
                cmd.Parameters.Add(new SQLiteParameter("@WorkComment", req.WorkComment));
                cmd.Parameters.Add(new SQLiteParameter("@EducationalOrganization", req.EducationalOrganization));
                cmd.Parameters.Add(new SQLiteParameter("@EducationalOrganizationShort", req.EducationalOrganizationShort));
                cmd.Parameters.Add(new SQLiteParameter("@Email", req.Email));
                cmd.Parameters.Add(new SQLiteParameter("@Telephone", req.Telephone));
                cmd.Parameters.Add(new SQLiteParameter("@Addr", req.Addr));
                cmd.Parameters.Add(new SQLiteParameter("@Addr1", req.Addr1));
                cmd.Parameters.Add(new SQLiteParameter("@District", req.District));
                cmd.Parameters.Add(new SQLiteParameter("@Region", req.Region));
                cmd.Parameters.Add(new SQLiteParameter("@Area", req.Area));
                cmd.Parameters.Add(new SQLiteParameter("@City", req.City));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefFio", req.ChiefFio));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefPosition", req.ChiefPosition));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefEmail", req.ChiefEmail));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefTelephone", req.ChiefTelephone));
                cmd.Parameters.Add(new SQLiteParameter("@SubsectionName", req.SubsectionName));
                cmd.Parameters.Add(new SQLiteParameter("@Fios", req.Fios));
                cmd.Parameters.Add(new SQLiteParameter("@Agies", req.Agies));
                cmd.Parameters.Add(new SQLiteParameter("@ProcMedicine", (req.ProcMedicine) ? 1 : 0));
                StringBuilder sb = new StringBuilder();
                int counter = 0;
                foreach (string str in req.Links)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("^" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@Links", sb.ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@CompetitionName", req.CompetitionName));
                cmd.Parameters.Add(new SQLiteParameter("@DateReg", req.DateReg));
                cmd.Parameters.Add(new SQLiteParameter("@Likes", req.Likes));
                cmd.Parameters.Add(new SQLiteParameter("@Nolikes", req.Nolikes));
                cmd.Parameters.Add(new SQLiteParameter("@Approved", (req.Approved) ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@PdnProcessing", (req.PdnProcessing) ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@PublicAgreement", (req.PublicAgreement) ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@SummaryLikes", req.SummaryLikes));
                cmd.Parameters.Add(new SQLiteParameter("@ClubsName", req.ClubsName));
                cmd.Parameters.Add(new SQLiteParameter("@Weight", req.Weight));
                cmd.Parameters.Add(new SQLiteParameter("@Weights", req.Weights));
                cmd.Parameters.Add(new SQLiteParameter("@AgeСategories", req.AgeСategories));
                cmd.Parameters.Add(new SQLiteParameter("@Kvalifications", req.Kvalifications));
                cmd.Parameters.Add(new SQLiteParameter("@Programms", req.Programms));
                cmd.Parameters.Add(new SQLiteParameter("@Result", req.Result));
                cmd.Parameters.Add(new SQLiteParameter("@Results", req.Results));
                cmd.Parameters.Add(new SQLiteParameter("@ProtocolFile", req.ProtocolFile));
                cmd.Parameters.Add(new SQLiteParameter("@ProtocolPartyCount", req.ProtocolPartyCount));
                cmd.Parameters.Add(new SQLiteParameter("@TechnicalInfo", req.TechnicalInfo));
                cmd.Parameters.Add(new SQLiteParameter("@Timing_min", req.Timing_min));
                cmd.Parameters.Add(new SQLiteParameter("@Timing_sec", req.Timing_sec));

                sb = new StringBuilder();
                counter = 0;
                foreach (string str in req.ChiefFios)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@ChiefFios", sb.ToString()));
                sb = new StringBuilder();
                counter = 0;
                foreach (string str in req.ChiefPositions)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@ChiefPositions", sb.ToString()));
                sb = new StringBuilder();
                counter = 0;
                foreach (string str in req.AthorsFios)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@AthorsFios", sb.ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@AgeСategory", req.AgeСategory));
                cmd.Parameters.Add(new SQLiteParameter("@PartyCount", req.PartyCount));
                cmd.Parameters.Add(new SQLiteParameter("@CheckedAdmin", req.CheckedAdmin));
                cmd.Parameters.Add(new SQLiteParameter("@Points", req.Points));

                cmd.Parameters.Add(new SQLiteParameter("@Schools", (string.IsNullOrEmpty(req.Schools) ? "" : req.Schools)));
                cmd.Parameters.Add(new SQLiteParameter("@ClassRooms", (string.IsNullOrEmpty(req.ClassRooms) ? "" : req.ClassRooms)));
                cmd.Parameters.Add(new SQLiteParameter("@ProtocolFileDoc", (string.IsNullOrEmpty(req.ProtocolFileDoc) ? "" : req.ProtocolFileDoc)));
                cmd.Parameters.Add(new SQLiteParameter("@Fios_1", (string.IsNullOrEmpty(req.Fios_1) ? "" : req.Fios_1)));
                cmd.Parameters.Add(new SQLiteParameter("@Agies_1", (string.IsNullOrEmpty(req.Agies_1) ? "" : req.Agies_1)));
                cmd.Parameters.Add(new SQLiteParameter("@Schools_1", (string.IsNullOrEmpty(req.Schools_1) ? "" : req.Schools_1)));
                cmd.Parameters.Add(new SQLiteParameter("@ClassRooms_1", (string.IsNullOrEmpty(req.ClassRooms_1) ? "" : req.ClassRooms_1)));
                cmd.Parameters.Add(new SQLiteParameter("@Weights_1", (string.IsNullOrEmpty(req.Weights_1) ? "" : req.Weights_1)));
                cmd.Parameters.Add(new SQLiteParameter("@IsApply", req.IsApply));
                cmd.Parameters.Add(new SQLiteParameter("@DateApply", req.DateApply));
                cmd.Parameters.Add(new SQLiteParameter("@Division", (string.IsNullOrEmpty(req.Division) ? "" : req.Division)));

                if (sqlite.ExecuteNonQueryParams(cmd) == -1)
                {
                    result = -1;
                }
                else
                {
                    sqlite.ConnectionClose();
                }
                cmd.Dispose();

                if (result != -1)
                {
                    // определим номер последней добавленной строки, он и будет номером добавленной заявки
                    sqlite = new SqliteHelper(_pathToDb);
                    cmd = new SQLiteCommand();
                    cmd.CommandText = "SELECT MAX(_id) FROM " + _tableName;
                    result = sqlite.ExecuteScalarParams(cmd);
                    cmd.Dispose();
                    if (sqlite != null)
                    {
                        sqlite.ConnectionClose();
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                result = -1;
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
            req.Id = result;
            return result;
        }

        #endregion
        #region Метод long UpdateOneRequest(CompetitionRequest obj)

        /// <summary>Метод обновляет в БД данные по одной структуре</summary>
        /// <param name="obj">объект с данными по одной структуре</param>
        /// <returns>Метод возвращает кол-во обновлённых строк или -1 в случае какой-либо ошибки</returns>
        public long UpdateOneRequest(CompetitionRequest obj)
        {
            long result = -1;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                cmd.CommandText = "UPDATE " + _tableName + " SET " +
                                                                "Fio=@Fio, " +
                                                                "Class_=@Class_, " +
                                                                "Age=@Age, " +
                                                                "WorkName=@WorkName, " +
                                                                "WorkComment=@WorkComment, " +
                                                                "EducationalOrganization=@EducationalOrganization, " +
                                                                "EducationalOrganizationShort=@EducationalOrganizationShort, " +
                                                                "Email=@Email, " +
                                                                "Telephone=@Telephone, " +
                                                                "Addr=@Addr, " +
                                                                "Addr1=@Addr1, " +
                                                                "District=@District, " +
                                                                "Region=@Region, " +
                                                                "Area=@Area, " +
                                                                "City=@City, " +
                                                                "ChiefFio=@ChiefFio, " +
                                                                "ChiefPosition=@ChiefPosition, " +
                                                                "ChiefEmail=@ChiefEmail, " +
                                                                "ChiefTelephone=@ChiefTelephone, " +
                                                                "SubsectionName=@SubsectionName, " +
                                                                "Fios=@Fios, " +
                                                                "Agies=@Agies, " +
                                                                "ProcMedicine=@ProcMedicine, " +
                                                                "Links=@Links, " +
                                                                "CompetitionName=@CompetitionName, " +
                                                                "DateReg=@DateReg, " +
                                                                "Likes=@Likes, " +
                                                                "Nolikes=@Nolikes, " +
                                                                "Approved=@Approved, " +
                                                                "PdnProcessing=@PdnProcessing, " +
                                                                "PublicAgreement=@PublicAgreement, " +
                                                                "SummaryLikes=@SummaryLikes, " +
                                                                "ClubsName=@ClubsName, " +
                                                                "Weight=@Weight, " +
                                                                "Weights=@Weights, " +
                                                                "AgeСategories=@AgeСategories, " +
                                                                "Kvalifications=@Kvalifications, " +
                                                                "Programms=@Programms, " +
                                                                "Result=@Result, " +
                                                                "Results=@Results, " +
                                                                "ProtocolFile=@ProtocolFile, " +
                                                                "ProtocolPartyCount=@ProtocolPartyCount, " +
                                                                "TechnicalInfo=@TechnicalInfo, " +
                                                                "Timing_min=@Timing_min, " +
                                                                "Timing_sec=@Timing_sec, " +
                                                                "ChiefFios=@ChiefFios, " +
                                                                "ChiefPositions=@ChiefPositions, " +
                                                                "AthorsFios=@AthorsFios, " +
                                                                "AgeСategory=@AgeСategory, " +
                                                                "PartyCount=@PartyCount, " +
                                                                "CheckedAdmin=@CheckedAdmin, " +
                                                                "Points=@Points," +
                                                                "Schools=@Schools," +
                                                                "ClassRooms=@ClassRooms," +
                                                                "ProtocolFileDoc=@ProtocolFileDoc," +
                                                                "Fios_1=@Fios_1, " +
                                                                "Agies_1=@Agies_1, " +
                                                                "Schools_1=@Schools_1," +
                                                                "ClassRooms_1=@ClassRooms_1," +
                                                                "Weights_1=@Weights_1," +
                                                                "IsApply=@IsApply," +
                                                                "DateApply=@DateApply," +
                                                                "Division = @Division" +
                                                            " WHERE _id=@_id";

                cmd.Parameters.Add(new SQLiteParameter("@_id", obj.Id));
                cmd.Parameters.Add(new SQLiteParameter("@Fio", obj.Fio));
                cmd.Parameters.Add(new SQLiteParameter("@Class_", obj.Class_));
                cmd.Parameters.Add(new SQLiteParameter("@Age", obj.Age));
                cmd.Parameters.Add(new SQLiteParameter("@WorkName", obj.WorkName));
                cmd.Parameters.Add(new SQLiteParameter("@WorkComment", obj.WorkComment));
                cmd.Parameters.Add(new SQLiteParameter("@EducationalOrganization", obj.EducationalOrganization));
                cmd.Parameters.Add(new SQLiteParameter("@EducationalOrganizationShort", obj.EducationalOrganizationShort));
                cmd.Parameters.Add(new SQLiteParameter("@Email", obj.Email));
                cmd.Parameters.Add(new SQLiteParameter("@Telephone", obj.Telephone));
                cmd.Parameters.Add(new SQLiteParameter("@Addr", obj.Addr));
                cmd.Parameters.Add(new SQLiteParameter("@Addr1", obj.Addr1));
                cmd.Parameters.Add(new SQLiteParameter("@District", obj.District));
                cmd.Parameters.Add(new SQLiteParameter("@Region", obj.Region));
                cmd.Parameters.Add(new SQLiteParameter("@Area", obj.Area));
                cmd.Parameters.Add(new SQLiteParameter("@City", obj.City));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefFio", obj.ChiefFio));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefPosition", obj.ChiefPosition));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefEmail", obj.ChiefEmail));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefTelephone", obj.ChiefTelephone));
                cmd.Parameters.Add(new SQLiteParameter("@SubsectionName", obj.SubsectionName));
                cmd.Parameters.Add(new SQLiteParameter("@Fios", obj.Fios));
                cmd.Parameters.Add(new SQLiteParameter("@Agies", obj.Agies));
                cmd.Parameters.Add(new SQLiteParameter("@ProcMedicine", (obj.ProcMedicine) ? 1 : 0));
                StringBuilder sb = new StringBuilder();
                int counter = 0;
                foreach (string str in obj.Links)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("^" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@Links", sb.ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@CompetitionName", obj.CompetitionName));
                cmd.Parameters.Add(new SQLiteParameter("@DateReg", obj.DateReg));
                cmd.Parameters.Add(new SQLiteParameter("@Likes", obj.Likes));
                cmd.Parameters.Add(new SQLiteParameter("@Nolikes", obj.Nolikes));
                cmd.Parameters.Add(new SQLiteParameter("@Approved", (obj.Approved) ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@PdnProcessing", (obj.PdnProcessing) ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@PublicAgreement", (obj.PublicAgreement) ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@SummaryLikes", obj.SummaryLikes));
                cmd.Parameters.Add(new SQLiteParameter("@ClubsName", obj.ClubsName));
                cmd.Parameters.Add(new SQLiteParameter("@Weight", obj.Weight));
                cmd.Parameters.Add(new SQLiteParameter("@Weights", obj.Weights));
                cmd.Parameters.Add(new SQLiteParameter("@AgeСategories", obj.AgeСategories));
                cmd.Parameters.Add(new SQLiteParameter("@Kvalifications", obj.Kvalifications));
                cmd.Parameters.Add(new SQLiteParameter("@Programms", obj.Programms));
                cmd.Parameters.Add(new SQLiteParameter("@Result", obj.Result));
                cmd.Parameters.Add(new SQLiteParameter("@Results", obj.Results));
                cmd.Parameters.Add(new SQLiteParameter("@ProtocolFile", obj.ProtocolFile));
                cmd.Parameters.Add(new SQLiteParameter("@ProtocolPartyCount", obj.ProtocolPartyCount));
                cmd.Parameters.Add(new SQLiteParameter("@TechnicalInfo", obj.TechnicalInfo));
                cmd.Parameters.Add(new SQLiteParameter("@Timing_min", obj.Timing_min));
                cmd.Parameters.Add(new SQLiteParameter("@Timing_sec", obj.Timing_sec));

                sb = new StringBuilder();
                counter = 0;
                foreach (string str in obj.ChiefFios)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@ChiefFios", sb.ToString()));
                sb = new StringBuilder();
                counter = 0;
                foreach (string str in obj.ChiefPositions)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@ChiefPositions", sb.ToString()));
                sb = new StringBuilder();
                counter = 0;
                foreach (string str in obj.AthorsFios)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@AthorsFios", sb.ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@AgeСategory", obj.AgeСategory));
                cmd.Parameters.Add(new SQLiteParameter("@PartyCount", obj.PartyCount));
                cmd.Parameters.Add(new SQLiteParameter("@CheckedAdmin", obj.CheckedAdmin));
                cmd.Parameters.Add(new SQLiteParameter("@Points", obj.Points));

                cmd.Parameters.Add(new SQLiteParameter("@Schools", (string.IsNullOrEmpty(obj.Schools) ? "" : obj.Schools)));
                cmd.Parameters.Add(new SQLiteParameter("@ClassRooms", (string.IsNullOrEmpty(obj.ClassRooms) ? "" : obj.ClassRooms)));
                cmd.Parameters.Add(new SQLiteParameter("@ProtocolFileDoc", (string.IsNullOrEmpty(obj.ProtocolFileDoc) ? "" : obj.ProtocolFileDoc)));
                cmd.Parameters.Add(new SQLiteParameter("@Fios_1", (string.IsNullOrEmpty(obj.Fios_1) ? "" : obj.Fios_1)));
                cmd.Parameters.Add(new SQLiteParameter("@Agies_1", (string.IsNullOrEmpty(obj.Agies_1) ? "" : obj.Agies_1)));
                cmd.Parameters.Add(new SQLiteParameter("@Schools_1", (string.IsNullOrEmpty(obj.Schools_1) ? "" : obj.Schools_1)));
                cmd.Parameters.Add(new SQLiteParameter("@ClassRooms_1", (string.IsNullOrEmpty(obj.ClassRooms_1) ? "" : obj.ClassRooms_1)));
                cmd.Parameters.Add(new SQLiteParameter("@Weights_1", (string.IsNullOrEmpty(obj.Weights_1) ? "" : obj.Weights_1)));
                cmd.Parameters.Add(new SQLiteParameter("@IsApply", obj.IsApply));
                cmd.Parameters.Add(new SQLiteParameter("@DateApply", obj.DateApply));
                cmd.Parameters.Add(new SQLiteParameter("@Division", (string.IsNullOrEmpty(obj.Division) ? "" : obj.Division)));

                result = sqlite.ExecuteNonQueryParams(cmd);

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;

                #endregion
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
            }

            return result;
        }

        #endregion

        #region GetListOfRequests
        /// <summary>
        /// Метод GetListOfRequests
        /// </summary>
        /// <param name="competition"></param>
        /// <param name="approved"></param>
        /// <param name="subnames"></param>
        /// <returns></returns>
        public List<CompetitionRequest> GetListOfRequests(string competition, string approved, string[] subnames)
        {
            if (competition == null) competition = "";
            if (approved == null) approved = "";
            if (subnames == null) subnames = new string[] { "" };

            var resultList = new List<CompetitionRequest>();
            var tmpList = new List<CompetitionRequest>();

            foreach (string subname in subnames)
            {
                tmpList = new CompetitionsWork().GetListOfRequests(competition, approved, subname);
                if (tmpList != null)
                {
                    resultList.AddRange(tmpList);
                }
            }

            return resultList;
        }

        #endregion
        #region Метод List<CompetitionRequest> GetListOfRequests(...)

        /// <summary>Метод возвращает готовый список структур запросов за участие в конкурсе.
        /// Может возвращать запросы, касающиеся одного, отдельного взятого конкурса.</summary>
        /// <param name="cName">условное название конкурса. Может быть - foto, literary, theatre</param>
        /// <param name="approved">значение для выборки опубликованных или неопубликованных заявок (можно передать 0 - выбрать неопубликованные или 1 - выбрать опубликованные).</param>
        /// <param name="subname">номинация</param>
        /// <param name="thisyear">архивная метка - если 0 то архив, если 1 то новое, если пусто то условие не используется</param>
        /// Если не передавать в это значие ничего, то метод не будет производить фильтрацию по этому полю</param>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<CompetitionRequest> GetListOfRequests(string cName = "", string approved = "", string subname = "")
        {
            //string sdacmd;
            var resultList = new List<CompetitionRequest>();

            var cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);

                #region Формирование строки запроса

                cmd.CommandText = "SELECT * FROM " + _tableName + " WHERE 1=1";

                if (cName != "")
                {
                    cmd.CommandText += " AND CompetitionName = '" + cName + "'";
                }


                if (approved != "") // если дополнительно нужно отфильтровать на предмет публикации то
                {
                    if (approved == "0" || approved == "1") // условие-подстраховка (approved должно быть - 0 или 1, других вариантов нет)
                    {
                        cmd.CommandText += " AND Approved=" + approved + " AND IsApply=1 ";
                    }
                }


                //if (thisyear != "") //проверка, если 0 то архив, если 1 то новое, если пусто то условие не используется.
                //{
                //    if ((cName == "") && (approved == "")) cmd.CommandText += " WHERE";
                //    else cmd.CommandText += " AND";

                //    cmd.CommandText += " Compitition_year=";
                //    cmd.CommandText += thisyear;
                //}



                //if (subname != "") // выбор по номинации
                //{
                //    if (subname.Contains("%"))
                //    {
                //        cmd.CommandText += " AND SubsectionName like '" + subname + "'";
                //    }
                //    else
                //    {
                //        cmd.CommandText += " AND SubsectionName = '" + subname + "'";
                //    }
                //}


                if (subname != "") // выбор по номинации
                {
                    cmd.CommandText += " AND SubsectionName like '" + subname + "'";
                }

                #endregion

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    #region Запись в лог

                    DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Произошла ошибка чтения данных из БД по запросу: " + cmd.CommandText);

                    #endregion

                    cmd.Dispose(); sqlite.ConnectionClose();
                    return null;
                }

                try
                {
                    #region Код заполнения списка

                    CompetitionRequest req = new CompetitionRequest();
                    while (reader.Read())
                    {
                        req = new CompetitionRequest();
                        FillRequest(req, reader);
                        resultList.Add(req);
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                }
                finally
                {
                    reader.Close(); reader.Dispose();
                    cmd.Dispose(); sqlite.ConnectionClose();
                }

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                resultList = null;
            }

            return resultList;
        }

        #endregion
        #region Метод List<CompetitionRequest> GetReqsForFields(string[] fieldNames)

        /// <summary>Функция возвращает список структур запросов на участие в конкурсе, в котором заполнены только нужные поля.</summary>
        /// <param name="fieldNames">названия полей, которые нужно выбрать из запросов</param>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<CompetitionRequest> GetReqsForFields(string[] fieldNames)
        {
            var resultList = new List<CompetitionRequest>();

            try
            {
                #region Основной код

                #region Инициализации и проверки

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT ";
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    if (i == 0) { cmd.CommandText += fieldNames[i]; }
                    else { cmd.CommandText += ", " + fieldNames[i]; }
                }
                cmd.CommandText += " FROM " + _tableName;

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    cmd.Dispose(); sqlite.ConnectionClose();
                    return null;
                }
                if (!reader.HasRows)
                {
                    cmd.Dispose(); sqlite.ConnectionClose();
                    return null;
                }

                #endregion 

                try
                {
                    #region Код заполнения списка

                    CompetitionRequest req = new CompetitionRequest();
                    while (reader.Read())
                    {
                        req = new CompetitionRequest();
                        FillRequestPartly(req, reader, fieldNames);
                        resultList.Add(req);
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    #region Код

                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                    #endregion
                }
                finally
                {
                    reader.Close(); reader.Dispose();
                    cmd.Dispose(); sqlite.ConnectionClose();
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                resultList = null;

                #endregion
            }

            return resultList;
        }

        #endregion
        #region Метод SelectWhere(string[] fieldsNamesSel, string[] fieldsNamesWhere, string[] fieldsValueWhere, SqlLogic sqlLogic = SqlLogic.AND)

        /// <summary>Метод возвращает готовый список структур по переданным параметрам</summary>
        /// <param name="fieldsNamesSel">имена полей, которые нужно выбрать. Если указать null, будут выбраны все поля</param>
        /// <param name="fieldsNamesWhere">имена полей, по которым нужно отфильтровать. Если указать null, то фильтрации не будет</param>
        /// <param name="fieldsValueWhere">значения полей, по которым нужно отфильтровать. Если указать null, то фильтрации не будет. Количество аргументов должно совпадать с кол-вом аргументов в fielsNamesWhere</param>
        /// <param name="sqlLogic">логический оператор, по которому будут складываться фильтрация по именам и значениям фильтрующих полей</param>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<CompetitionRequest> SelectWhere(string[] fieldsNamesSel, string[] fieldsNamesWhere, string[] fieldsValueWhere, SqlLogic sqlLogic = SqlLogic.AND)
        {
            List<CompetitionRequest> resultList = new List<CompetitionRequest>();

            string sqlLog = EnumsHelper.GetSqlLogic(sqlLogic);
            if (sqlLog == "no result") return resultList;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            #region Формирование строки запроса

            cmd.CommandText = "SELECT ";
            if (fieldsNamesSel == null)
            {
                cmd.CommandText += "* ";
            }
            else
            {
                for (int i = 0; i < fieldsNamesSel.Length; i++)
                {
                    if (i == 0) cmd.CommandText += fieldsNamesSel[i];
                    else cmd.CommandText += ", " + fieldsNamesSel[i];
                }
            }
            cmd.CommandText += " FROM " + _tableName;
            if (fieldsNamesWhere != null && fieldsNamesWhere != null)
            {
                cmd.CommandText += " WHERE ";
                for (int i = 0; i < fieldsNamesWhere.Length; i++)
                {
                    if (i == 0) cmd.CommandText += fieldsNamesWhere[i] + "=@" + fieldsNamesWhere[i];
                    else cmd.CommandText += " " + sqlLog + " " + fieldsNamesWhere[i] + "=@" + fieldsNamesWhere[i];
                    cmd.Parameters.Add(new SQLiteParameter("@" + fieldsNamesWhere[i], fieldsValueWhere[i]));
                }
            }
            #endregion

            SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
            if (reader == null)
            {
                cmd.Dispose();
                resultList = null;
            }
            else if (!reader.HasRows)
            {
                cmd.Dispose();
            }
            else
            {
                try
                {
                    #region Код заполнения списка

                    CompetitionRequest obj = new CompetitionRequest();
                    while (reader.Read())
                    {
                        obj = new CompetitionRequest();
                        FillRequestPartly(obj, reader, fieldsNamesSel);
                        resultList.Add(obj);
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    #region Код

                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    resultList = null;

                    #endregion
                }
                finally
                {
                    #region Код

                    sqlite.ConnectionClose();
                    if (cmd != null) cmd.Dispose();

                    #endregion
                }
            }

            return resultList;
        }

        #endregion
        #region GetSortedListOfRequests(string competition = "", string srchString = "", string[] subnames = null, bool isArchive = false)
        /// <summary>
        /// Метод-обёртка над методом GetListOfRequests. Сортирует список структур по номеру заявки
        /// </summary>
        /// <param name="competition">условное название конкурса. Может быть - foto, literary, theatre</param>
        /// <param name="srchString">строка поискового запроса</param>
        /// <param name="subname">условное наименование номинации</param>
        /// <returns></returns>
        public List<CompetitionRequest> GetSortedListOfRequests(string competition = "", string srchString = "", string[] subnames = null)
        {
            if (competition == null) competition = "";
            if (srchString == null) srchString = "";
            if (subnames == null) subnames = new string[] { "" };

            var resultList = new List<CompetitionRequest>();
            var tmpList = new List<CompetitionRequest>();

            foreach (string subname in subnames)
            {
                tmpList = GetSortedListOfRequests(competition, srchString, subname);
                if (tmpList != null)
                {
                    resultList.AddRange(tmpList);
                }
            }

            return resultList;
        }
        #endregion
        #region Метод List<CompetitionRequest> GetSortedListOfRequests(...)

        /// <summary>Метод-обёртка над методом GetListOfRequests. Сортирует список структур по номеру заявки</summary>
        /// <param name="competition">условное название конкурса. Может быть - foto, literary, theatre</param>
        /// <param name="srchString">строка поискового запроса</param>
        /// <param name="subname">условное наименование номинации</param>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<CompetitionRequest> GetSortedListOfRequests(string competition = "", string srchString = "", string subname = "")
        {
            var tempList = GetListOfRequests(competition, "", subname);
            if (tempList == null) return null;
            tempList = tempList.OrderBy(x => x.Id * -1).ToList();

            #region Фильтрация по поисковой строке

            var resultList = new List<CompetitionRequest>();
            var mainMatchesList = new List<CompetitionRequest>();  //совпадения содержимого поля
            var fullMatchesList = new List<CompetitionRequest>();  //полные совпадения всего содержимого поля
            DateTime dt = new DateTime();
            srchString = srchString.Trim().ToLower();
            bool ChiefFiosChecker = false;
            bool ChiefPositionsChecker = false;
            bool AthorsFiosChecker = false;

            if (srchString != "")
            {
                foreach (CompetitionRequest req in tempList)
                {
                    #region Код

                    #region Код проверки полных совпадений всего содержимого поля

                    ChiefFiosChecker = false;
                    foreach (string item in req.ChiefFios)
                    {
                        if (item.ToLower() == srchString)
                        {
                            ChiefFiosChecker = true; break;
                        }
                    }
                    ChiefPositionsChecker = false;
                    foreach (string item in req.ChiefPositions)
                    {
                        if (item.ToLower() == srchString)
                        {
                            ChiefPositionsChecker = true; break;
                        }
                    }
                    AthorsFiosChecker = false;
                    foreach (string item in req.AthorsFios)
                    {
                        if (item.ToLower() == srchString)
                        {
                            AthorsFiosChecker = true; break;
                        }
                    }

                    if (
                            req.Id.ToString() == srchString ||
                            req.Fio.ToLower() == srchString ||
                            req.Age.ToLower() == srchString ||
                            req.Class_.ToLower() == srchString ||
                            req.WorkName.ToLower() == srchString ||
                            req.WorkComment.ToLower() == srchString ||
                            req.EducationalOrganization.ToLower() == srchString ||
                            req.EducationalOrganizationShort.ToLower() == srchString ||
                            req.Email.ToLower() == srchString ||
                            req.Telephone.ToLower() == srchString ||
                            req.Addr.ToLower() == srchString ||
                            req.Addr1.ToLower() == srchString ||
                            req.District.ToLower() == srchString ||
                            req.Region.ToLower() == srchString ||
                            req.Area.ToLower() == srchString ||
                            req.City.ToLower() == srchString ||
                            req.ChiefFio.ToLower() == srchString ||
                            req.ChiefPosition.ToLower() == srchString ||
                            req.ChiefEmail.ToLower() == srchString ||
                            req.ChiefTelephone.ToLower() == srchString ||
                            req.SubsectionName.ToLower() == srchString ||
                            req.Fios.ToLower().Contains(srchString) ||
                            req.Agies.ToLower().Contains(srchString) ||
                            req.Schools.ToLower().Contains(srchString) ||
                            req.ClassRooms.ToLower().Contains(srchString) ||
                            req.CompetitionName.ToLower() == srchString ||
                            req.ClubsName.ToLower() == srchString ||
                            req.Result.ToLower() == srchString ||
                            dt.ToShortDateString() == srchString ||
                            req.TechnicalInfo.ToLower() == srchString ||
                            req.Fios_1.ToLower().Contains(srchString) ||
                            req.Age_1.ToLower().Contains(srchString) ||
                            req.Schools_1.ToLower().Contains(srchString) ||
                            req.ClassRooms_1.ToLower().Contains(srchString) ||
                            ChiefFiosChecker || ChiefPositionsChecker || AthorsFiosChecker
                       )
                    {
                        fullMatchesList.Add(req);
                    }

                    #endregion

                    if (srchString.Contains(" "))
                    {
                        #region Код поиска в случае поискового запроса, состоящего из нескольких слов, разделённых пробелами

                        string[] srchArr = srchString.Split(new[] { ' ' });
                        dt = new DateTime(req.DateReg);
                        bool checker = false;

                        #region Сначала проверим вхождение целой фразы в полях,

                        ChiefFiosChecker = false;
                        foreach (string item in req.ChiefFios)
                        {
                            if (item.ToLower().Contains(srchString))
                            {
                                ChiefFiosChecker = true; break;
                            }
                        }
                        ChiefPositionsChecker = false;
                        foreach (string item in req.ChiefPositions)
                        {
                            if (item.ToLower().Contains(srchString))
                            {
                                ChiefPositionsChecker = true; break;
                            }
                        }
                        AthorsFiosChecker = false;
                        foreach (string item in req.AthorsFios)
                        {
                            if (item.ToLower().Contains(srchString))
                            {
                                AthorsFiosChecker = true; break;
                            }
                        }

                        if (req.Id.ToString().Contains(srchString) ||
                            req.Fio.ToLower().Contains(srchString) ||
                            req.Age.ToLower().Contains(srchString) ||
                            req.Class_.ToLower().Contains(srchString) ||
                            req.WorkName.ToLower().Contains(srchString) ||
                            req.WorkComment.ToLower().Contains(srchString) ||
                            req.EducationalOrganization.ToLower().Contains(srchString) ||
                            req.EducationalOrganizationShort.ToLower().Contains(srchString) ||
                            req.Email.ToLower().Contains(srchString) ||
                            req.Telephone.ToLower().Contains(srchString) ||
                            req.Addr.ToLower().Contains(srchString) ||
                            req.Addr1.ToLower().Contains(srchString) ||
                            req.District.ToLower().Contains(srchString) ||
                            req.Region.ToLower().Contains(srchString) ||
                            req.Area.ToLower().Contains(srchString) ||
                            req.City.ToLower().Contains(srchString) ||
                            req.ChiefFio.ToLower().Contains(srchString) ||
                            req.ChiefPosition.ToLower().Contains(srchString) ||
                            req.ChiefEmail.ToLower().Contains(srchString) ||
                            req.ChiefTelephone.ToLower().Contains(srchString) ||
                            req.SubsectionName.ToLower().Contains(srchString) ||
                            req.Fios.ToLower().Contains(srchString) ||
                            req.Agies.ToLower().Contains(srchString) ||
                            req.Schools.ToLower().Contains(srchString) ||
                            req.ClassRooms.ToLower().Contains(srchString) ||
                            req.CompetitionName.ToLower() == srchString ||
                            req.ClubsName.ToLower() == srchString ||
                            req.Result.ToLower() == srchString ||
                            dt.ToShortDateString() == srchString ||
                            req.TechnicalInfo.ToLower() == srchString ||
                            req.Fios_1.ToLower().Contains(srchString) ||
                            req.Age_1.ToLower().Contains(srchString) ||
                            req.Schools_1.ToLower().Contains(srchString) ||
                            req.ClassRooms_1.ToLower().Contains(srchString) ||
                            ChiefFiosChecker || ChiefPositionsChecker || AthorsFiosChecker
                            )
                        {
                            mainMatchesList.Add(req);
                            checker = true;
                        }

                        #endregion

                        #region Потом проверим отдельно вхождение каждого слова в поисковой строке

                        if (!checker)   //если вхождение поисковой фразы целиком не нашлось, то ищем вхождение слова из фразы
                        {
                            foreach (string srch in srchArr)
                            {
                                ChiefFiosChecker = false;
                                foreach (string item in req.ChiefFios)
                                {
                                    if (item.ToLower().Contains(srch))
                                    {
                                        ChiefFiosChecker = true; break;
                                    }
                                }
                                ChiefPositionsChecker = false;
                                foreach (string item in req.ChiefPositions)
                                {
                                    if (item.ToLower().Contains(srch))
                                    {
                                        ChiefPositionsChecker = true; break;
                                    }
                                }
                                AthorsFiosChecker = false;
                                foreach (string item in req.AthorsFios)
                                {
                                    if (item.ToLower().Contains(srch))
                                    {
                                        AthorsFiosChecker = true; break;
                                    }
                                }

                                if (req.Id.ToString().Contains(srch) ||
                                    req.Fio.ToLower().Contains(srch) ||
                                    req.Age.ToLower().Contains(srch) ||
                                    req.Class_.ToLower().Contains(srch) ||
                                    req.WorkName.ToLower().Contains(srch) ||
                                    req.WorkComment.ToLower().Contains(srch) ||
                                    req.EducationalOrganization.ToLower().Contains(srch) ||
                                    req.EducationalOrganizationShort.ToLower().Contains(srch) ||
                                    req.Email.ToLower().Contains(srch) ||
                                    req.Telephone.ToLower().Contains(srch) ||
                                    req.Addr.ToLower().Contains(srch) ||
                                    req.Addr1.ToLower().Contains(srch) ||
                                    req.District.ToLower().Contains(srch) ||
                                    req.Region.ToLower().Contains(srch) ||
                                    req.Area.ToLower().Contains(srch) ||
                                    req.City.ToLower().Contains(srch) ||
                                    req.ChiefFio.ToLower().Contains(srch) ||
                                    req.ChiefPosition.ToLower().Contains(srch) ||
                                    req.ChiefEmail.ToLower().Contains(srch) ||
                                    req.ChiefTelephone.ToLower().Contains(srch) ||
                                    req.SubsectionName.ToLower().Contains(srch) ||
                                    req.Fios.ToLower().Contains(srch) ||
                                    req.CompetitionName.ToLower().Contains(srch) ||
                                    req.ClubsName.ToLower().Contains(srch) ||
                                    req.Result.ToLower().Contains(srch) ||
                                    dt.ToShortDateString().Contains(srch) ||
                                    req.TechnicalInfo.ToLower().Contains(srch) ||
                                    ChiefFiosChecker || ChiefPositionsChecker || AthorsFiosChecker)
                                {
                                    resultList.Add(req);
                                    break;
                                }
                            }
                        }

                        #endregion

                        #endregion
                    }
                    else
                    {
                        #region Код поиска в случае отсутствия пробелов в поисковом запросе

                        ChiefFiosChecker = false;
                        foreach (string item in req.ChiefFios)
                        {
                            if (item.ToLower().Contains(srchString))
                            {
                                ChiefFiosChecker = true; break;
                            }
                        }
                        ChiefPositionsChecker = false;
                        foreach (string item in req.ChiefPositions)
                        {
                            if (item.ToLower().Contains(srchString))
                            {
                                ChiefPositionsChecker = true; break;
                            }
                        }
                        AthorsFiosChecker = false;
                        foreach (string item in req.AthorsFios)
                        {
                            if (item.ToLower().Contains(srchString))
                            {
                                AthorsFiosChecker = true; break;
                            }
                        }

                        dt = new DateTime(req.DateReg);
                        if (req.Id.ToString().Contains(srchString) ||
                            req.Fio.ToLower().Contains(srchString) ||
                            req.Age.ToLower().Contains(srchString) ||
                            req.Class_.ToLower().Contains(srchString) ||
                            req.WorkName.ToLower().Contains(srchString) ||
                            req.WorkComment.ToLower().Contains(srchString) ||
                            req.EducationalOrganization.ToLower().Contains(srchString) ||
                            req.EducationalOrganizationShort.ToLower().Contains(srchString) ||
                            req.Email.ToLower().Contains(srchString) ||
                            req.Telephone.ToLower().Contains(srchString) ||
                            req.Addr.ToLower().Contains(srchString) ||
                            req.Addr1.ToLower().Contains(srchString) ||
                            req.District.ToLower().Contains(srchString) ||
                            req.Region.ToLower().Contains(srchString) ||
                            req.Area.ToLower().Contains(srchString) ||
                            req.City.ToLower().Contains(srchString) ||
                            req.ChiefFio.ToLower().Contains(srchString) ||
                            req.ChiefPosition.ToLower().Contains(srchString) ||
                            req.ChiefEmail.ToLower().Contains(srchString) ||
                            req.ChiefTelephone.ToLower().Contains(srchString) ||
                            req.SubsectionName.ToLower().Contains(srchString) ||
                            req.Fios.ToLower().Contains(srchString) ||
                            req.CompetitionName.ToLower().Contains(srchString) ||
                            req.ClubsName.ToLower().Contains(srchString) ||
                            req.Result.ToLower().Contains(srchString) ||
                            dt.ToShortDateString().Contains(srchString) ||
                            req.TechnicalInfo.ToLower().Contains(srchString) ||
                            ChiefFiosChecker || ChiefPositionsChecker || AthorsFiosChecker
                            )
                        {
                            mainMatchesList.Add(req);
                        }

                        #endregion
                    }

                    #endregion
                }

                #region Выставление точных совпадений по поиску на первое место

                fullMatchesList.AddRange(mainMatchesList);

                if (fullMatchesList.Count > 0)
                {
                    resultList.InsertRange(0, fullMatchesList);
                }

                //Удаление повторяющихся заявок
                resultList = resultList.Distinct().ToList();

                #endregion
            }
            else
            {
                resultList = tempList;
            }

            return resultList;

            #endregion
        }

        #endregion
        #region Метод GetOneRequest(string reqNum)

        /// <summary>Метод возвращает структуру заявки по её номеру</summary>
        /// <param name="reqNum">номер заявки</param>
        /// <returns>Возвращает null в случае ошибки</returns>
        public CompetitionRequest GetOneRequest(string reqNum)
        {
            CompetitionRequest req = new CompetitionRequest();

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();

                cmd.CommandText = "SELECT * FROM " + _tableName + " WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", reqNum));

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                    return null;
                }

                try
                {
                    while (reader.Read())
                    {
                        FillRequest(req, reader);
                    }
                }
                catch (Exception ex)
                {
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    req = null;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                }

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                req = null;
            }

            return (req.Id == 0 ? null : req);
        }

        #endregion
        #region Метод int UpdateApproved(long id, int approved)

        /// <summary>Метод обновляет значение утверждения заявки в ТОП 30</summary>
        /// <param name="id">id заявки</param>
        /// <param name="approved">новое значения для утверждения в ТОП 30</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateApproved(long id, int approved)
        {
            int result = -1;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "UPDATE " + _tableName + " SET Approved=@approved WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", id));
                cmd.Parameters.Add(new SQLiteParameter("approved", approved));
                result = sqlite.ExecuteNonQueryParams(cmd);

                cmd.Dispose(); sqlite.ConnectionClose();

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }

            return result;
        }

        #endregion
        #region Метод int UpdateCheckedAdmin(long id, int checkedAdmin)

        /// <summary>Метод обновляет значение утверждения заявки в ТОП 30</summary>
        /// <param name="id">id заявки</param>
        /// <param name="checkedAdmin">проверено администратором</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateCheckedAdmin(long id, int checkedAdmin)
        {
            int result = -1;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "UPDATE " + _tableName + " SET CheckedAdmin=@checkedAdmin WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", id));
                cmd.Parameters.Add(new SQLiteParameter("checkedAdmin", checkedAdmin));
                result = sqlite.ExecuteNonQueryParams(cmd);

                cmd.Dispose(); sqlite.ConnectionClose();

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }

            return result;
        }

        #endregion
        #region Метод int UpdateField(string id, string fieldName, string fieldValue)

        /// <summary>Метод обновляет значение одного поля в таблице заявок</summary>
        /// <param name="id">id заявки</param>
        /// <param name="fieldName">название поля</param>
        /// <param name="fieldValue">значение поля</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateField(string id, string fieldName, string fieldValue)
        {
            int result = -1;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                cmd.CommandText = "UPDATE " + _tableName + " SET " + fieldName + "=@value WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", id));
                cmd.Parameters.Add(new SQLiteParameter("value", fieldValue));
                result = sqlite.ExecuteNonQueryParams(cmd);

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }
            finally
            {
                cmd.Dispose(); sqlite.ConnectionClose();
            }

            return result;
        }

        #endregion
        #region Метод int UpdateFieldGroup(string id, string fieldName, string position)

        /// <summary>Метод обновляет значение одного поля в таблице заявок. Значения переменных, передаваемых в метод должны быть перепроверены перед передачей в него.</summary>
        /// <param name="id">id заявки</param>
        /// <param name="fieldName">название поля</param>
        /// <param name="fieldValue">значение поля</param>
        /// <param name="position">позиция для вставки нового значения в списке вида - параметр|параметр|параметр|параметр</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateFieldGroup(string id, string fieldName, string fieldValue, int position)
        {
            int result = -1;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                #region Сформируем значение, которое необходимо будет обновить в заявке

                string resFieldValue = "";
                CompetitionRequest req = GetOneRequest(id);
                if (req != null)
                {
                    #region Формирование значения

                    if (fieldName == "Fios") resFieldValue = req.Fios;
                    else if (fieldName == "Agies") resFieldValue = req.Agies;
                    else if (fieldName == "Weights") resFieldValue = req.Weights;
                    else if (fieldName == "Schools") resFieldValue = req.Schools;
                    else if (fieldName == "ClassRooms") resFieldValue = req.ClassRooms;
                    else if (fieldName == "Fios_1") resFieldValue = req.Fios_1;
                    else if (fieldName == "Agies_1") resFieldValue = req.Agies_1;
                    else if (fieldName == "Schools_1") resFieldValue = req.Schools_1;
                    else if (fieldName == "ClassRooms_1") resFieldValue = req.ClassRooms_1;
                    else if (fieldName == "Weights_1") resFieldValue = req.Weights_1;
                    else if (fieldName == "ChiefFios")
                    {
                        StringBuilder sb = new StringBuilder();
                        int counter = 0;
                        foreach (string str in req.ChiefFios)
                        {
                            if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                            counter++;
                        }
                        resFieldValue = sb.ToString();
                    }
                    else if (fieldName == "ChiefPositions")
                    {
                        StringBuilder sb = new StringBuilder();
                        int counter = 0;
                        foreach (string str in req.ChiefPositions)
                        {
                            if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                            counter++;
                        }
                        resFieldValue = sb.ToString();
                    }
                    else if (fieldName == "AthorsFios")
                    {
                        StringBuilder sb = new StringBuilder();
                        int counter = 0;
                        foreach (string str in req.AthorsFios)
                        {
                            if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                            counter++;
                        }
                        resFieldValue = sb.ToString();
                    }
                    else if (fieldName == "Links")
                    {
                        StringBuilder sb = new StringBuilder();
                        int counter = 0;
                        foreach (string str in req.Links)
                        {
                            if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                            counter++;
                        }
                        resFieldValue = sb.ToString();
                    }

                    resFieldValue = ReplaceFieldValues(resFieldValue, fieldValue, position);

                    if (fieldName == "Links")
                    {
                        resFieldValue = resFieldValue.Replace("|", "^");
                    }
                    #endregion
                }

                #endregion

                if (req != null)
                {

                    cmd.CommandText = "UPDATE " + _tableName + " SET " + fieldName + "=@value WHERE _id=@id";
                    cmd.Parameters.Add(new SQLiteParameter("id", id));
                    cmd.Parameters.Add(new SQLiteParameter("value", resFieldValue));
                    result = sqlite.ExecuteNonQueryParams(cmd);

                }

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }
            finally
            {
                cmd.Dispose(); sqlite.ConnectionClose();
            }

            return result;
        }

        #endregion
        #region Метод int UpdateLikes(long id, bool like)

        /// <summary>Метод увеличивает значение счётчика голосов ЗА или ПРОТИВ по одной заявке.</summary>
        /// <param name="id">id заявки</param>
        /// <param name="like">какой счётчик необходимо увеличить на один. Если true - увеличить кол-во голосов ЗА, если false - увеличить кол-во голосов ПРОТИВ</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateLikes(long id, bool like)
        {
            int result = -1;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                if (like)
                {
                    cmd.CommandText = "UPDATE " + _tableName + " SET Likes=Likes+1 WHERE _id=@id";
                }
                else
                {
                    cmd.CommandText = "UPDATE " + _tableName + " SET Nolikes=Nolikes+1 WHERE _id=@id";
                }
                cmd.Parameters.Add(new SQLiteParameter("id", id));
                result = sqlite.ExecuteNonQueryParams(cmd);



                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }
            finally
            {
                cmd.Dispose(); sqlite.ConnectionClose();
            }

            return result;
        }

        #endregion
        #region Метод int UpdateSummaryLikes(long id)

        /// <summary>Метод увеличивает значение счётчика голосов по итоговому голосованию.</summary>
        /// <param name="id">id заявки</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateSummaryLikes(long id)
        {
            int result = -1;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                cmd.CommandText = "UPDATE " + _tableName + " SET SummaryLikes=SummaryLikes+1 WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", id));
                result = sqlite.ExecuteNonQueryParams(cmd);

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }
            finally
            {
                cmd.Dispose(); sqlite.ConnectionClose();
            }

            return result;
        }

        #endregion
        #region Метод int UpdateApply(long id)

        /// <summary>Метод устанавливает значения флага принятия заявки куратором и дату принятия</summary>
        /// <param name="id">id заявки</param>

        public void UpdateApply(long id)
        {
            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            long ticks = DateTime.Now.Ticks;

            try
            {
                #region Основной код

                cmd.CommandText = "UPDATE " + _tableName + " SET IsApply=1, DateApply=" + ticks.ToString() + " WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", id));
                sqlite.ExecuteNonQueryParams(cmd);

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
            finally
            {
                cmd.Dispose();
                sqlite.ConnectionClose();
            }
        }

        #endregion
        #region Метод int DeleteOneRequest(...)

        /// <summary>Метод удаляет одну заявку из БД</summary>
        /// <param name="reqNum">id заявки</param>
        /// <param name="withWorkFiles">true - нужно ещё удалить файлы работ к этой заявке (НЕ ИСПОЛЬЗОВАТЬ, ПОТОМУ ЧТО ПРИ УДАЛЕНИИ ДУБЛИКАТОВ ЗАЯВОК УДАЛЯТСЯ ВСЕ ФАЙЛЫ РАБОТ!!!)</param>
        /// <param name="req">объект заявки (нужно передавать, если вы хотите дополнительно удалить файлы работ)</param>
        /// <returns>Возвращает -1 в случае какой-либо ошибки</returns>
        public int DeleteOneRequest(string reqNum, bool withWorkFiles = false, CompetitionRequest req = null)
        {
            int result = -1;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                cmd.CommandText = "DELETE FROM " + _tableName + " WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", reqNum));
                result = sqlite.ExecuteNonQueryParams(cmd);

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }
            finally
            {
                cmd.Dispose(); sqlite.ConnectionClose();
            }


            if (withWorkFiles && req != null)
            {
                #region Асинхронный код удаления файла работы и файла протокола

                UniObjParams objParams = new UniObjParams()
                {
                    obj1 = req,
                };
                Thread thread = new Thread(new ParameterizedThreadStart((object data) =>
                {
                    #region Код

                    UniObjParams objParams1 = (UniObjParams)data;
                    CompetitionRequest request = (CompetitionRequest)objParams1.obj1;
                    string mainPath = System.Web.Hosting.HostingEnvironment.MapPath("~") + @"files\competitionfiles\";
                    string pathToKulturaDir = mainPath + @"kultura\";
                    string pathToLiteraryDir = mainPath + @"literary\";
                    string pathToFotoDir = mainPath + @"foto\";
                    string pathToTheatreDir = mainPath + @"theatre\";
                    string pathToProtocolDir = mainPath + @"protocols\";
                    string pathToToponimDir = mainPath + @"toponim\";

                    string pathToDir = "";

                    if (request.CompetitionName == EnumsHelper.GetKulturaCode(Kultura.self)) pathToDir = pathToKulturaDir;
                    if (request.CompetitionName == EnumsHelper.GetLiteraryCode(Literary.self)) pathToDir = pathToLiteraryDir;
                    if (request.CompetitionName == EnumsHelper.GetPhotoCode(Photo.self)) pathToDir = pathToFotoDir;
                    if (request.CompetitionName == EnumsHelper.GetTheatreCode(Theatre.self)) pathToDir = pathToTheatreDir;
                    if (request.CompetitionName == EnumsHelper.GetToponimCode(Toponim.self)) pathToDir = pathToToponimDir;

                    if (pathToDir != "")
                    {
                        string name = "";
                        foreach (string fileName in request.Links)
                        {
                            #region Тело цикла

                            name = fileName.ToLower();
                            if (name.Contains(".jpg") ||
                                name.Contains(".jpeg") ||
                                name.Contains(".png") ||
                                name.Contains(".doc") ||
                                name.Contains(".docx") ||
                                name.Contains(".rar") ||
                                name.Contains(".zip") ||
                                name.Contains(".mp3"))
                            {
                                //Удаление файлов
                                if (File.Exists(pathToDir + fileName))
                                {
                                    File.Delete(pathToDir + fileName);
                                }
                                else
                                {
                                    DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: При удалении не найден файл - " + pathToDir + fileName);
                                }
                                //Удаление маленьких фотографий
                                string fileNameL = "";
                                if (name.Contains(".jpg"))
                                {
                                    fileNameL = name.Replace(".jpg", "") + "_l.jpg";
                                    if (File.Exists(pathToDir + fileNameL))
                                    {
                                        File.Delete(pathToDir + fileNameL);
                                    }
                                }
                                if (name.Contains(".jpeg"))
                                {
                                    fileNameL = name.Replace(".jpeg", "") + "_l.jpeg";
                                    if (File.Exists(pathToDir + fileNameL))
                                    {
                                        File.Delete(pathToDir + fileNameL);
                                    }
                                }
                                if (name.Contains(".png"))
                                {
                                    fileNameL = name.Replace(".png", "") + "_l.png";
                                    if (File.Exists(pathToDir + fileNameL))
                                    {
                                        File.Delete(pathToDir + fileNameL);
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: Не удалось определить путь к папке для удаления файла работы к заявке № " + request.Id);
                    }
                    
                    //Удаление файла протокола
                    if (request.ProtocolFile != "")
                    {
                        if (File.Exists(pathToProtocolDir + request.ProtocolFile))
                        {
                            try
                            {
                                File.Delete(pathToProtocolDir + request.ProtocolFile);
                            }
                            catch
                            {

                            }
                        }
                    }

                    #endregion
                }));
                thread.Start(objParams);

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод bool DeleteUnnecessaryFiles()

        /// <summary>Метод удаляет все файлы из папок конкурсов, которые не принадлежат ни одной зарегистрированной в БД заявке</summary>
        /// <returns>Возвращает true в случае успеха, и false - в случае возникновения какой-либо ошибки</returns>
        public bool DeleteUnnecessaryFiles()
        {
            bool result = true;

            try
            {
                #region Основной код

                var allList = GetListOfRequests();
                var listOffileNames = new List<string>();
                var listOfProtocolsNames = new List<string>();
                foreach (CompetitionRequest request in allList)
                {
                    if (!request.SubsectionName.Contains("Театральное искусство:") && !request.SubsectionName.Contains("Художественное слово")) //в этих номинация в Links хранятся не имена файлов, а ссылки на ютуб-ролики
                    {
                        listOffileNames.AddRange(request.Links);
                    }
                    if (request.ProtocolFile != "")
                    {
                        listOfProtocolsNames.Add(request.ProtocolFile);
                    }
                }

                string[] pathToFilesFoto = Directory.GetFiles(_pathToFotoFolder, "*", SearchOption.TopDirectoryOnly);
                string[] pathToFilesLiterary = Directory.GetFiles(_pathToLiteraryFolder, "*", SearchOption.TopDirectoryOnly);
                string[] pathToFilesKultura = Directory.GetFiles(_pathToKulturaFolder, "*", SearchOption.TopDirectoryOnly);
                string[] pathToFilesToponim = Directory.GetFiles(_pathToToponimFolder, "*", SearchOption.TopDirectoryOnly);
                string[] pathToFilesTheatre = Directory.GetFiles(_pathToTheatreFolder, "*", SearchOption.TopDirectoryOnly);
                string[] pathToProtocolFiles = Directory.GetFiles(_pathToProtocolFolder, "*", SearchOption.TopDirectoryOnly);

                string fName = "";
                foreach (string path in pathToFilesFoto)
                {
                    fName = Path.GetFileName(path);
                    if (!listOffileNames.Contains(fName))
                    {
                        File.Delete(path);
                    }
                }
                foreach (string path in pathToFilesLiterary)
                {
                    fName = Path.GetFileName(path);
                    if (!listOffileNames.Contains(fName))
                    {
                        File.Delete(path);
                    }
                }
                foreach (string path in pathToFilesKultura)
                {
                    fName = Path.GetFileName(path);
                    if (!listOffileNames.Contains(fName))
                    {
                        File.Delete(path);
                    }
                }
                foreach (string path in pathToFilesToponim)
                {
                    fName = Path.GetFileName(path);
                    if (!listOffileNames.Contains(fName))
                    {
                        File.Delete(path);
                    }
                }
                foreach (string path in pathToFilesTheatre)
                {
                    fName = Path.GetFileName(path);
                    if (!listOffileNames.Contains(fName))
                    {
                        File.Delete(path);
                    }
                }
                foreach (string path in pathToProtocolFiles)
                {
                    fName = Path.GetFileName(path);
                    if (!listOfProtocolsNames.Contains(fName))
                    {
                        File.Delete(path);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = false;

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод List<string[]> GetCityAndCounts(string cName = "")

        /// <summary>Метод возвращает список массивов с названиями городов и кол-вом участников по каждому городу (список сортирован по названиям городов).
        /// Метод может вернуть такой список по всем конкурсам или по каждому конкурсу отдельно.</summary>
        /// <param name="cName">условное наименование конкурса, может быть - foto, literary, theatre</param>
        /// <returns>Возвращает списк с массивами, 1-й элемент - название города, 2-й элемент - количество конкурсантов в этом городе. 
        /// Возвращается пустой список, если не найдено городов по данному конкурсу или в случае какой-либо ошибки.</returns>
        public List<string[]> GetCityAndCounts(string cName = "")
        {
            var resuList = new List<string[]>();

            var reqList = GetListOfRequests(cName);    // получил список всех заявок по данному конкурсу
            if (reqList == null) return resuList;

            var groupedList = reqList.Select(a => a).Where(a => a.City.Trim() != "").GroupBy(a => a.City)
                                     .OrderBy(a => a.Key)
                                     .Select(a => new string[]
                                                               {
                                                                    a.Key,a.Count().ToString()
                                                               }).ToArray();                   // "превратил" его в список массивов
            resuList.AddRange(groupedList);

            return resuList;

        }

        #endregion
        #region Метод void FillRequest(CompetitionRequest req, SQLiteDataReader reader)

        /// <summary>Вспомогательный метод, который заполняет переданный в него объект типа CompetitionRequest данными из SQLiteDataReader</summary>
        /// <param name="req">объект</param>
        /// <param name="reader">объект</param>
        private void FillRequest(CompetitionRequest req, SQLiteDataReader reader)
        {
            //System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            try
            {
                #region Код

                req.Fio = reader["Fio"].ToString();
                req.Class_ = reader["Class_"].ToString();
                req.Age = reader["Age"].ToString();
                req.WorkName = reader["WorkName"].ToString();
                req.WorkComment = reader["WorkComment"].ToString();
                req.EducationalOrganization = reader["EducationalOrganization"].ToString();
                req.EducationalOrganizationShort = reader["EducationalOrganizationShort"].ToString();
                req.Email = reader["Email"].ToString();
                req.Telephone = reader["Telephone"].ToString();
                req.Addr = reader["Addr"].ToString();
                req.Addr1 = reader["Addr1"].ToString();
                req.District = reader["District"].ToString();
                req.Region = reader["Region"].ToString();
                req.Area = reader["Area"].ToString();
                req.City = reader["City"].ToString();
                req.ChiefFio = reader["ChiefFio"].ToString();
                req.ChiefPosition = reader["ChiefPosition"].ToString();
                req.ChiefEmail = reader["ChiefEmail"].ToString();
                req.ChiefTelephone = reader["ChiefTelephone"].ToString();
                req.SubsectionName = reader["SubsectionName"].ToString();
                req.Fios = reader["Fios"].ToString();
                req.Agies = reader["Agies"].ToString();
                req.Id = long.Parse(reader["_id"].ToString());
                string[] strSplit = reader["Links"].ToString().Split('^');
                if (strSplit.Length == 1 && strSplit[0] == "")
                {
                    strSplit = new string[] { };
                }
                req.Links = strSplit.ToList();
                req.CompetitionName = reader["CompetitionName"].ToString();
                req.DateReg = long.Parse(reader["DateReg"].ToString());
                req.Likes = long.Parse(reader["Likes"].ToString());
                req.Nolikes = long.Parse(reader["Nolikes"].ToString());
                req.Approved = int.Parse(reader["Approved"].ToString()) == 1;

                req.PdnProcessing = int.Parse(reader["PdnProcessing"].ToString()) == 1;
                req.PublicAgreement = int.Parse(reader["PublicAgreement"].ToString()) == 1;
                req.ProcMedicine = int.Parse(reader["ProcMedicine"].ToString()) == 1;
                req.SummaryLikes = long.Parse(reader["SummaryLikes"].ToString());

                req.ClubsName = reader["ClubsName"].ToString();
                req.Weight = int.Parse(reader["Weight"].ToString());
                req.Weights = reader["Weights"].ToString();
                req.AgeСategories = reader["AgeСategories"].ToString();
                req.Kvalifications = reader["Kvalifications"].ToString();
                req.Programms = reader["Programms"].ToString();
                req.Result = reader["Result"].ToString();
                req.Results = reader["Results"].ToString();

                req.ProtocolFile = reader["ProtocolFile"].ToString();
                req.ProtocolPartyCount = int.Parse(reader["ProtocolPartyCount"].ToString());
                req.TechnicalInfo = reader["TechnicalInfo"].ToString();
                req.Timing_min = int.Parse(reader["Timing_min"].ToString());
                req.Timing_sec = int.Parse(reader["Timing_sec"].ToString());
                strSplit = reader["ChiefFios"].ToString().Split('|');
                if (strSplit.Length == 1 && strSplit[0] == "")
                {
                    strSplit = new string[] { };
                }
                req.ChiefFios = strSplit.ToList();
                strSplit = reader["ChiefPositions"].ToString().Split('|');
                if (strSplit.Length == 1 && strSplit[0] == "")
                {
                    strSplit = new string[] { };
                }
                req.ChiefPositions = strSplit.ToList();
                strSplit = reader["AthorsFios"].ToString().Split('|');
                if (strSplit.Length == 1 && strSplit[0] == "")
                {
                    strSplit = new string[] { };
                }
                req.AthorsFios = strSplit.ToList();
                req.AgeСategory = reader["AgeСategory"].ToString();
                req.PartyCount = int.Parse(reader["PartyCount"].ToString());
                req.CheckedAdmin = int.Parse(reader["CheckedAdmin"].ToString());
                req.Points = int.Parse(reader["Points"].ToString());

                req.Schools = reader["Schools"].ToString();
                req.ClassRooms = reader["ClassRooms"].ToString();
                req.ProtocolFileDoc = reader["ProtocolFileDoc"].ToString();

                req.Fios_1 = reader["Fios_1"].ToString();
                req.Agies_1 = reader["Agies_1"].ToString();
                req.Schools_1 = reader["Schools_1"].ToString();
                req.ClassRooms_1 = reader["ClassRooms_1"].ToString();
                req.Weights_1 = reader["Weights_1"].ToString();

                req.IsApply = int.Parse(reader["IsApply"].ToString()) == 1;
                req.DateApply = long.Parse(reader["DateApply"].ToString());
                req.Division = reader["Division"].ToString();

                if (!string.IsNullOrEmpty(req.ChiefFio) && !req.ChiefFios.Exists(x => x == req.ChiefFio))
                {
                    req.ChiefFios.Add(req.ChiefFio);

                    if (!string.IsNullOrEmpty(req.ChiefPosition))
                        req.ChiefPositions.Add(req.ChiefPosition);
                }


                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }
        }

        #endregion
        #region Метод FillRequestPartly(...)

        /// <summary>Метод наполняет переданный в него объект req данными поле, имена которых передаются в переменной fieldNames</summary>
        /// <param name="req">объект заявки</param>
        /// <param name="reader">ридер</param>
        /// <param name="fieldNames">имена полей, данные по которым можно взять</param>
        private void FillRequestPartly(CompetitionRequest req, SQLiteDataReader reader, string[] fieldNames)
        {
            foreach (string fieldName in fieldNames)
            {
                if (fieldName == "Fio")
                {
                    req.Fio = reader[fieldName].ToString();
                }
                else if (fieldName == "Class_")
                {
                    req.Class_ = reader[fieldName].ToString();
                }
                else if (fieldName == "Age")
                {
                    req.Age = reader[fieldName].ToString();
                }
                else if (fieldName == "WorkName")
                {
                    req.WorkName = reader[fieldName].ToString();
                }
                else if (fieldName == "WorkComment")
                {
                    req.WorkComment = reader[fieldName].ToString();
                }
                else if (fieldName == "EducationalOrganization")
                {
                    req.EducationalOrganization = reader[fieldName].ToString();
                }
                else if (fieldName == "EducationalOrganizationShort")
                {
                    req.EducationalOrganizationShort = reader[fieldName].ToString();
                }
                else if (fieldName == "Email")
                {
                    req.Email = reader[fieldName].ToString();
                }
                else if (fieldName == "Telephone")
                {
                    req.Telephone = reader[fieldName].ToString();
                }
                else if (fieldName == "Addr")
                {
                    req.Addr = reader[fieldName].ToString();
                }
                else if (fieldName == "Addr1")
                {
                    req.Addr1 = reader[fieldName].ToString();
                }
                else if (fieldName == "District")
                {
                    req.District = reader[fieldName].ToString();
                }
                else if (fieldName == "Region")
                {
                    req.Region = reader[fieldName].ToString();
                }
                else if (fieldName == "Area")
                {
                    req.Area = reader[fieldName].ToString();
                }
                else if (fieldName == "City")
                {
                    req.City = reader[fieldName].ToString();
                }
                else if (fieldName == "ChiefFio")
                {
                    req.ChiefFio = reader[fieldName].ToString();
                }
                else if (fieldName == "ChiefPosition")
                {
                    req.ChiefPosition = reader[fieldName].ToString();
                }
                else if (fieldName == "ChiefEmail")
                {
                    req.ChiefEmail = reader[fieldName].ToString();
                }
                else if (fieldName == "ChiefTelephone")
                {
                    req.ChiefTelephone = reader[fieldName].ToString();
                }
                else if (fieldName == "SubsectionName")
                {
                    req.SubsectionName = reader[fieldName].ToString();
                }
                else if (fieldName == "Fios")
                {
                    req.Fios = reader[fieldName].ToString();
                }
                else if (fieldName == "Agies")
                {
                    req.Agies = reader[fieldName].ToString();
                }
                else if (fieldName == "_id")
                {
                    req.Id = long.Parse(reader[fieldName].ToString());
                }
                else if (fieldName == "Links")
                {
                    string[] strSplit = reader[fieldName].ToString().Split('^');
                    req.Links = strSplit.ToList();
                }
                else if (fieldName == "CompetitionName")
                {
                    req.CompetitionName = reader[fieldName].ToString();
                }
                else if (fieldName == "DateReg")
                {
                    req.DateReg = long.Parse(reader[fieldName].ToString());
                }
                else if (fieldName == "Likes")
                {
                    req.Likes = long.Parse(reader[fieldName].ToString());
                }
                else if (fieldName == "Nolikes")
                {
                    req.Nolikes = long.Parse(reader[fieldName].ToString());
                }
                else if (fieldName == "Approved")
                {
                    req.Approved = int.Parse(reader[fieldName].ToString()) == 1;
                }
                else if (fieldName == "PdnProcessing")
                {
                    req.PdnProcessing = int.Parse(reader[fieldName].ToString()) == 1;
                }
                else if (fieldName == "PublicAgreement")
                {
                    req.PublicAgreement = int.Parse(reader[fieldName].ToString()) == 1;
                }
                else if (fieldName == "ProcMedicine")
                {
                    req.ProcMedicine = int.Parse(reader[fieldName].ToString()) == 1;
                }
                else if (fieldName == "SummaryLikes")
                {
                    req.SummaryLikes = long.Parse(reader[fieldName].ToString());
                }
                else if (fieldName == "ClubsName")
                {
                    req.ClubsName = reader[fieldName].ToString();
                }
                else if (fieldName == "Weight")
                {
                    req.Weight = int.Parse(reader[fieldName].ToString());
                }
                else if (fieldName == "Weights")
                {
                    req.Weights = reader[fieldName].ToString();
                }
                else if (fieldName == "AgeСategories")
                {
                    req.AgeСategories = reader[fieldName].ToString();
                }
                else if (fieldName == "Kvalifications")
                {
                    req.Kvalifications = reader[fieldName].ToString();
                }
                else if (fieldName == "Programms")
                {
                    req.Programms = reader[fieldName].ToString();
                }
                else if (fieldName == "Result")
                {
                    req.Result = reader[fieldName].ToString();
                }
                else if (fieldName == "Results")
                {
                    req.Results = reader[fieldName].ToString();
                }
                else if (fieldName == "ProtocolFile")
                {
                    req.ProtocolFile = reader[fieldName].ToString();
                }
                else if (fieldName == "ProtocolPartyCount")
                {
                    req.ProtocolPartyCount = int.Parse(reader[fieldName].ToString());
                }
                else if (fieldName == "TechnicalInfo")
                {
                    req.TechnicalInfo = reader[fieldName].ToString();
                }
                else if (fieldName == "Timing_min")
                {
                    req.Timing_min = int.Parse(reader[fieldName].ToString());
                }
                else if (fieldName == "Timing_sec")
                {
                    req.Timing_sec = int.Parse(reader[fieldName].ToString());
                }
                else if (fieldName == "ChiefFios")
                {
                    string[] strSplit = reader[fieldName].ToString().Split('^');
                    req.ChiefFios = strSplit.ToList();
                }
                else if (fieldName == "ChiefPositions")
                {
                    string[] strSplit = reader[fieldName].ToString().Split('^');
                    req.ChiefPositions = strSplit.ToList();
                }
                else if (fieldName == "AthorsFios")
                {
                    string[] strSplit = reader[fieldName].ToString().Split('^');
                    req.AthorsFios = strSplit.ToList();
                }
                else if (fieldName == "AgeСategory")
                {
                    req.AgeСategory = reader[fieldName].ToString();
                }
                else if (fieldName == "PartyCount")
                {
                    req.PartyCount = int.Parse(reader[fieldName].ToString());
                }
                else if (fieldName == "Schools")
                {
                    req.Schools = reader[fieldName].ToString();
                }
                else if (fieldName == "ClassRooms")
                {
                    req.ClassRooms = reader[fieldName].ToString();
                }
                else if (fieldName == "ProtocolFileDoc")
                {
                    req.ProtocolFileDoc = reader[fieldName].ToString();
                }
                else if (fieldName == "Fios_1")
                {
                    req.Fios_1 = reader[fieldName].ToString();
                }
                else if (fieldName == "Agies_1")
                {
                    req.Agies_1 = reader[fieldName].ToString();
                }
                else if (fieldName == "Schools_1")
                {
                    req.Schools_1 = reader[fieldName].ToString();
                }
                else if (fieldName == "ClassRooms_1")
                {
                    req.ClassRooms_1 = reader[fieldName].ToString();
                }
                else if (fieldName == "Weights_1")
                {
                    req.Weights_1 = reader[fieldName].ToString();
                }
                else if (fieldName == "IsApply")
                {
                    req.IsApply = int.Parse(reader[fieldName].ToString()) == 1;
                }
                else if (fieldName == "DateApply")
                {
                    req.DateApply = long.Parse(reader[fieldName].ToString());
                }
                else if (fieldName == "Division")
                {
                    req.Division = reader[fieldName].ToString();
                }
            }
        }

        #endregion
        #region Метод bool AddPartyToRequest(...)

        /// <summary>Метод добавляет одного участника в заявку</summary>
        /// <param name="reqId">id заявки</param>
        /// <param name="fio">ФИО</param>
        /// <param name="age">возраст (дата рождения)</param>
        /// <param name="weight">вес</param>
        /// <param name="cname">условное наименование конкурса</param>
        /// <returns>Возвращает true - в случае успеха, false - в случае ошибки или неверно переданных параметров</returns>
        public bool AddPartyToRequest(string reqId, string fio, string age, string cname, string schools, string classRooms, string protocolType, string weight = null)
        {
            bool result = false;

            #region Проверки переменных

            if (StringToNum.ParseLong(reqId) == -1) return result;
            if (fio == "") return result;
            DateTime dtTmp;
            if (!DateTime.TryParse(age, out dtTmp))
            {
                return result;
            }
            if (cname == EnumsHelper.GetSportCode(Sport.self) && weight != null)
            {
                if (StringToNum.ParseInt(weight) == -1) return result;
            }

            #endregion

            #region Код

            CompetitionRequest request = GetOneRequest(reqId);
            if (request == null) return result;

            if (cname == EnumsHelper.GetSportCode(Sport.self) && weight != null)
            {
                #region Для спортивного конкурса
                if (protocolType == "" || protocolType == "2")
                {
                    if (request.Fios == "")
                    {
                        if (request.Fio == "")
                        {
                            request.Fio = fio;
                            request.Age = age;
                            request.Weight = StringToNum.ParseInt(weight);
                            request.Schools = !string.IsNullOrEmpty(schools) ? schools : "";
                            request.ClassRooms = !string.IsNullOrEmpty(classRooms) ? classRooms : "";
                        }
                        else
                        {
                            var tFio = request.Fio;
                            var tAge = request.Age;
                            var tWeight = request.Weight;
                            request.Fios = tFio + "|" + fio;
                            request.Agies = tAge + "|" + age;
                            request.Weights = tWeight.ToString() + "|" + weight;
                            request.Schools = (!string.IsNullOrEmpty(request.Schools) ? request.Schools : "") + "|" + schools;
                            request.ClassRooms = (!string.IsNullOrEmpty(request.Schools) ? request.ClassRooms : "") + "|" + classRooms;
                            request.Fio = "";
                            request.Age = "";
                            request.Weight = 0;
                        }
                    }
                    else
                    {
                        request.Fios += "|" + fio;
                        request.Agies += "|" + age;
                        request.Weights += "|" + weight;
                        request.Schools += "|" + schools;
                        request.ClassRooms += "|" + classRooms;
                    }
                }
                else
                {
                    if (request.Fios_1 == "")
                    {
                        request.Fios_1 = fio;
                        request.Agies_1 = age;
                        request.Weights_1 = weight;
                        request.Schools_1 = !string.IsNullOrEmpty(schools) ? schools : "";
                        request.ClassRooms_1 = !string.IsNullOrEmpty(classRooms) ? classRooms : "";
                    }
                    else
                    {
                        request.Fios_1 += "|" + fio;
                        request.Agies_1 += "|" + age;
                        request.Weights_1 += "|" + weight;
                        request.Schools_1 += "|" + schools;
                        request.ClassRooms_1 += "|" + classRooms;
                    }
                }
                #endregion
            }
            else
            {
                #region Для всех остальных конкурсов

                if (protocolType == "" || protocolType == "2")
                {
                    if (request.Fios == "")
                    {
                        if (request.Fio == "")
                        {
                            request.Fio = fio;
                            request.Age = age;
                            request.Schools = !string.IsNullOrEmpty(schools) ? schools : "";
                            request.ClassRooms = !string.IsNullOrEmpty(classRooms) ? classRooms : "";
                        }
                        else
                        {
                            var tFio = request.Fio;
                            var tAge = request.Age;
                            request.Fios = tFio + "|" + fio;
                            request.Agies = tAge + "|" + age;
                            request.Schools = (!string.IsNullOrEmpty(request.Schools) ? request.Schools : "") + "|" + schools;
                            request.ClassRooms = (!string.IsNullOrEmpty(request.Schools) ? request.ClassRooms : "") + "|" + classRooms;
                            request.Fio = "";
                            request.Age = "";
                        }
                    }
                    else
                    {
                        request.Fios += "|" + fio;
                        request.Agies += "|" + age;
                        request.Schools += "|" + schools;
                        request.ClassRooms += "|" + classRooms;
                    }
                }
                else
                {
                    if (request.Fios_1 == "")
                    {
                        request.Fios_1 = fio;
                        request.Agies_1 = age;
                        request.Schools_1 = !string.IsNullOrEmpty(schools) ? schools : "";
                        request.ClassRooms_1 = !string.IsNullOrEmpty(classRooms) ? classRooms : "";
                    }
                    else
                    {
                        request.Fios_1 += "|" + fio;
                        request.Agies_1 += "|" + age;
                        request.Schools_1 += "|" + schools;
                        request.ClassRooms_1 += "|" + classRooms;
                    }
                }

                #endregion
            }

            #region Запись изменённой информации в БД

            long res = UpdateOneRequest(request);
            if (res > 0) result = true;

            #endregion

            #endregion

            return result;
        }

        #endregion
        #region Метод bool DelPartyFromRequest(...)

        /// <summary>Метод удаляет одного участника из заявки. Причём он может удалить как группового, так и индивидуального участника (в position должно быть -9)</summary>
        /// <param name="reqId">id заявки</param>
        /// <param name="position">номер позиции удаляемого участника в списке (если это значение -9, значит удаляется индивидуальный участник)</param>
        /// <param name="cname">условное наименование конкурса</param>
        /// <returns>Возвращает true - в случае успеха, false - в случае ошибки или неверно переданных параметров</returns>
        public bool DelPartyFromRequest(string reqId, string position, string cname, string protocolType)
        {
            bool result = false;
            cname = cname.Trim();

            #region Проверки переменных

            if (StringToNum.ParseLong(reqId) == -1) return result;
            if (StringToNum.ParseInt(position) == -1) return result;
            if (cname == "") return result;

            #endregion

            #region Код

            CompetitionRequest request = GetOneRequest(reqId);
            if (request == null) return result;

            int pos = StringToNum.ParseInt(position);

            if (cname == EnumsHelper.GetSportCode(Sport.self))
            {
                #region Для спортивного конкурса

                if (pos == -9)    //если нужно удалить данные по индивидуальному участнику
                {
                    if (protocolType == "" || protocolType == "2")
                    {
                        request.Fio = "";
                        request.Age = "";
                        request.Weight = 0;
                        request.Schools = "";
                        request.ClassRooms = "";
                    }
                    else
                    {
                        request.Fio = "";
                        request.Age = "";
                        request.Weight = 0;
                        request.Schools_1 = "";
                        request.ClassRooms_1 = "";
                    }
                }
                else                    //если нужно удалить данные по групповому участнику
                {
                    if (protocolType == "" || protocolType == "2")
                    {
                        request.Fios = DeleteFieldValues(request.Fios, pos);
                        request.Agies = DeleteFieldValues(request.Agies, pos);
                        request.Weights = DeleteFieldValues(request.Weights, pos);
                        request.Schools = DeleteFieldValues(request.Schools, pos);
                        request.ClassRooms = DeleteFieldValues(request.ClassRooms, pos);
                        request.Weights = DeleteFieldValues(request.Weights, pos);
                    }
                    else
                    {
                        request.Fios_1 = DeleteFieldValues(request.Fios_1, pos);
                        request.Agies_1 = DeleteFieldValues(request.Agies_1, pos);
                        request.Schools_1 = DeleteFieldValues(request.Schools_1, pos);
                        request.ClassRooms_1 = DeleteFieldValues(request.ClassRooms_1, pos);
                        request.Weights_1 = DeleteFieldValues(request.Weights_1, pos);
                    }
                }

                #endregion
            }
            else
            {
                #region Для всех остальных конкурсов

                if (pos == -9)    //если нужно удалить данные по индивидуальному участнику
                {
                    if (protocolType == "" || protocolType == "2")
                    {
                        request.Fio = "";
                        request.Age = "";
                        request.Schools = "";
                        request.ClassRooms = "";
                    }
                    else
                    {
                        request.Fio = "";
                        request.Age = "";
                        request.Schools_1 = "";
                        request.ClassRooms_1 = "";
                    }
                }
                else                    //если нужно удалить данные по групповому участнику
                {
                    if (protocolType == "" || protocolType == "2")
                    {
                        request.Fios = DeleteFieldValues(request.Fios, pos);
                        request.Agies = DeleteFieldValues(request.Agies, pos);
                        request.Schools = DeleteFieldValues(request.Schools, pos);
                        request.ClassRooms = DeleteFieldValues(request.ClassRooms, pos);
                    }
                    else
                    {
                        request.Fios_1 = DeleteFieldValues(request.Fios_1, pos);
                        request.Agies_1 = DeleteFieldValues(request.Agies_1, pos);
                        request.Schools_1 = DeleteFieldValues(request.Schools_1, pos);
                        request.ClassRooms_1 = DeleteFieldValues(request.ClassRooms_1, pos);
                    }
                }

                #endregion
            }

            #region Запись изменённой информации в БД

            long res = UpdateOneRequest(request);
            if (res > 0) result = true;

            #endregion

            #endregion

            return result;
        }

        #endregion
        #region Метод bool AddChiefToRequest(...)

        /// <summary>Метод добавляет одного руководителя в заявку</summary>
        /// <param name="reqId">id заявки</param>
        /// <param name="fio">ФИО</param>
        /// <param name="position">должность</param>
        /// <returns>Возвращает true - в случае успеха, false - в случае ошибки или неверно переданных параметров</returns>
        public bool AddChiefToRequest(string reqId, string fio, string position)
        {
            bool result = false;
            fio = fio.Trim();
            position = position.Trim();

            #region Проверки переменных

            if (StringToNum.ParseLong(reqId) == -1) return result;
            if (fio == "") return result;
            if (position == "") return result;

            if (!CompetitonWorkCommon.IsFioOk(fio))
            {
                return result;
            }

            #endregion

            #region Код

            CompetitionRequest request = GetOneRequest(reqId);
            if (request == null) return result;

            request.ChiefFios.Add(fio);
            request.ChiefPositions.Add(position);

            #region Запись изменённой информации в БД

            long res = UpdateOneRequest(request);
            if (res > 0) result = true;

            #endregion

            #endregion

            return result;
        }

        #endregion
        #region Метод bool DelChiefFromRequest(...)

        /// <summary>Метод удаляет одного руководителя из заявки</summary>
        /// <param name="reqId">id заявки</param>
        /// <param name="position">номер позиции удаляемого руководителя в списке</param>
        /// <returns>Возвращает true - в случае успеха, false - в случае ошибки или неверно переданных параметров</returns>
        public bool DelChiefFromRequest(string reqId, string position)
        {
            bool result = false;

            #region Проверки переменных

            if (StringToNum.ParseLong(reqId) == -1) return result;
            if (StringToNum.ParseInt(position) == -1) return result;

            #endregion

            #region Код

            CompetitionRequest request = GetOneRequest(reqId);
            if (request == null) return result;

            int pos = StringToNum.ParseInt(position);

            string ChiefFio = "";
            //удаляем дубли всего, потому что в методе FillRequest(req, reader) в request.ChiefFios добавляется req.ChiefFio
            //определяем кого удаляем, если руководителя из карточки, то запоминаем
            if (!string.IsNullOrEmpty(request.ChiefFio))
            {
                if (request.ChiefFio == request.ChiefFios[pos])
                    ChiefFio = request.ChiefFio;

                var fio = request.ChiefFio;
                for (int i = 0; i < request.ChiefFios.Count; i++)
                {
                    if (fio == request.ChiefFios[i])
                    {
                        request.ChiefFios.RemoveAt(i);
                        if (i < request.ChiefPositions.Count())
                            request.ChiefPositions.RemoveAt(i);
                        i--;
                    }
                }
            }
            //если удалили из списка главного чифа, то берем оставшегося, иначе пусто
            if (!string.IsNullOrEmpty(ChiefFio))
            {
                if (request.ChiefFios.Count > 0)
                {
                    request.ChiefFio = request.ChiefFios[0];
                    request.ChiefPosition = request.ChiefPositions.Count() > 0 ? request.ChiefPositions[0] : "";

                    var fio = request.ChiefFio;
                    for (int i = 0; i < request.ChiefFios.Count; i++)
                    {
                        if (fio == request.ChiefFios[i])
                        {
                            request.ChiefFios.RemoveAt(i);
                            if (i < request.ChiefPositions.Count())
                                request.ChiefPositions.RemoveAt(i);
                            i--;
                        }
                    }
                }
                else
                {
                    request.ChiefFio = "";
                    request.ChiefPosition = "";
                }
            }
            //если нет главного, а просто удаляем педагогов
            if (pos < request.ChiefFios.Count())
            {
                request.ChiefFios.RemoveAt(pos);
                if (pos < request.ChiefPositions.Count())
                    request.ChiefPositions.RemoveAt(pos);
            }

            #region Запись изменённой информации в БД

            long res = UpdateOneRequest(request);
            if (res > 0) result = true;

            #endregion

            #endregion

            return result;
        }

        #endregion
        #region Метод List<PersonPair> GetChiefsFiosPositionList(CompetitionRequest req)
        /// <summary>
        /// Получение списка педагогов и должностей по заявке
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public List<PersonPair> GetChiefsFiosPositionList(CompetitionRequest req) {
            var fiosChiefList = new List<PersonPair>();
            if (req.ChiefFio != "" && req.ChiefFio.ToUpper().Trim() != "НЕТ")
            {
                fiosChiefList.Add(new PersonPair() { Name = req.ChiefFio, Position = req.ChiefPosition });
            }

            if (req.ChiefFios.Count > 0)
            {
                for (int i = 0; i < req.ChiefFios.Distinct().Count(); i++)
                {
                    if (!fiosChiefList.Exists(x => x.Name == req.ChiefFios[i]))
                        fiosChiefList.Add(new PersonPair()
                        {
                            Name = req.ChiefFios[i],
                            Position = (req.ChiefPositions.ElementAtOrDefault(i) != null ? req.ChiefPositions[i] : "")
                        });
                }
            }
            return fiosChiefList;
        }
        #endregion
        #region Метод bool AddAthorToRequest(...)

        /// <summary>Метод добавляет одного автора коллекции в заявку</summary>
        /// <param name="reqId">id заявки</param>
        /// <param name="fio">ФИО</param>
        /// <returns>Возвращает true - в случае успеха, false - в случае ошибки или неверно переданных параметров</returns>
        public bool AddAthorToRequest(string reqId, string fio)
        {
            bool result = false;
            fio = fio.Trim();

            #region Проверки переменных

            if (StringToNum.ParseLong(reqId) == -1) return result;
            if (fio == "") return result;

            string sPattern = "^[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+$";
            if (!Regex.IsMatch(fio, sPattern))
            {
                return result;
            }

            #endregion

            #region Код

            CompetitionRequest request = GetOneRequest(reqId);
            if (request == null) return result;

            request.AthorsFios.Add(fio);

            #region Запись изменённой информации в БД

            long res = UpdateOneRequest(request);
            if (res > 0) result = true;

            #endregion

            #endregion

            return result;
        }

        #endregion
        #region Метод bool DelAthorFromRequest(...)

        /// <summary>Метод удаляет одного автора коллекции из заявки</summary>
        /// <param name="reqId">id заявки</param>
        /// <param name="position">номер позиции удаляемого автора в списке</param>
        /// <returns>Возвращает true - в случае успеха, false - в случае ошибки или неверно переданных параметров</returns>
        public bool DelAthorFromRequest(string reqId, string position)
        {
            bool result = false;

            #region Проверки переменных

            if (StringToNum.ParseLong(reqId) == -1) return result;
            if (StringToNum.ParseInt(position) == -1) return result;

            #endregion

            #region Код

            CompetitionRequest request = GetOneRequest(reqId);
            if (request == null) return result;

            int pos = StringToNum.ParseInt(position);

            request.AthorsFios.RemoveAt(pos);

            #region Запись изменённой информации в БД

            long res = UpdateOneRequest(request);
            if (res > 0) result = true;

            #endregion

            #endregion

            return result;
        }

        #endregion
        #region Метод bool VacuumDb()

        /// <summary>Метод очищает пересобирает БД, удаляя все записи, помеченные на удаление. Или попросту уменьшает её размер удаляя ненужное.</summary>
        /// <returns>true - в случае успеха, false - в случае возникновения какой-либо ошибки</returns>
        public bool VacuumDb()
        {
            bool result = false;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            if (sqlite.Vacuum())
            {
                result = true;
            }
            sqlite.ConnectionClose();

            return result;
        }

        #endregion

        #region Метод long GetDbSize()

        /// <summary>Метод возвращает размер файла БД в килобайтах</summary>
        /// <returns></returns>
        public long GetDbSize()
        {
            long result = 0;

            #region Основной код

            if (File.Exists(_pathToDb))
            {
                FileInfo f = new FileInfo(_pathToDb);
                result = f.Length / 1024;
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод long GetFoldersSize()

        /// <summary>Метод возвращает размер всех папок с файлами, относящиеся к конкурсам</summary>
        /// <returns></returns>
        public long GetFoldersSize()
        {
            long result = 0;

            #region Основной код

            try
            {
                #region Основной код

                if (Directory.Exists(_pathToFotoFolder) && Directory.Exists(_pathToLiteraryFolder) && Directory.Exists(_pathToTheatreFolder))
                {
                    string[] arr1 = Directory.GetFiles(_pathToFotoFolder, "*", SearchOption.TopDirectoryOnly);
                    string[] arr2 = Directory.GetFiles(_pathToLiteraryFolder, "*", SearchOption.TopDirectoryOnly);
                    string[] arr3 = Directory.GetFiles(_pathToTheatreFolder, "*", SearchOption.TopDirectoryOnly);
                    string[] arr4 = Directory.GetFiles(_pathToKulturaFolder, "*", SearchOption.TopDirectoryOnly);
                    string[] arr5 = Directory.GetFiles(_pathToToponimFolder, "*", SearchOption.TopDirectoryOnly);
                    string[] arr6 = Directory.GetFiles(_pathToProtocolFolder, "*", SearchOption.TopDirectoryOnly);
                    List<string> list = new List<string>();
                    list.AddRange(arr1); list.AddRange(arr2); list.AddRange(arr3); list.AddRange(arr4); list.AddRange(arr5); list.AddRange(arr6);
                    FileInfo f;
                    foreach (string path in list)
                    {
                        f = new FileInfo(path);
                        result += f.Length;
                    }
                    result = result / 1024;
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return 0;

                #endregion
            }

            #endregion

            return result;
        }

        #endregion

        #region Метод string GetFiosAndAgesInOne(string Fios, string Ages, string separate = "")

        /// <summary>Объединение всех возрастов и фамилий</summary>
        /// <param name="Fios"></param>
        /// <param name="Ages"></param>
        /// <param name="separate"></param>
        /// <returns></returns>
        public static string GetFiosAndAgesInOne(string Fios, string Ages, string separate = "", bool withOutPatronymic = false)
        {
            string someone = "";

            try
            {
                string[] Fio = Fios.Split(new Char[] { '|' });
                string[] Age = Ages.Split(new Char[] { '|' });

                Fio = Fio.Where(x => x != "").ToArray();
                Age = Age.Where(x => x != "").ToArray();
                //это невероятно, но проверяем равенство кол-ва записей.
                if (Fio.Length == Age.Length)
                {
                    DateTime dt;
                    for (int i = 0; i < Fio.Length; i++)
                    {
                        someone += (withOutPatronymic ? FioReduceFI(Fio[i]) : Fio[i]);
                        someone += " / ";
                        if (DateTime.TryParse(Age[i], out dt))
                        {
                            someone += Convert.ToString(GetAgeFromBurth(dt));
                        }
                        else
                        {
                            someone += Age[i];
                        }
                        someone += separate;
                    }
                }
                else if (Fio.Length != Age.Length)
                {
                    for (int i = 0; i < Fio.Length; i++)
                    {
                        someone += (withOutPatronymic ? FioReduceFI(Fio[i]) : Fio[i]);
                        someone += separate;
                    }
                }
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "CompetitionsWork", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            return someone;
        }

        #endregion
        #region Метод string GetWeightsInOne(CompetitionRequest obj, string separate = "")

        /// <summary>Объединение всех весов в заявке</summary>
        /// <param name="obj">заявка</param>
        /// <param name="separate">разделитель</param>
        /// <returns></returns>
        public static string GetWeightsInOne(CompetitionRequest obj, string separate = " ")
        {
            StringBuilder result = new StringBuilder();

            if (obj.Weight > 0)
            {
                result.Append(obj.Weight.ToString());
            }

            if (obj.Weights.Trim() != "")
            {
                string[] strSplit = obj.Weights.Split(new[] { '|' });
                for (int i = 0; i < strSplit.Length; i++)
                {
                    if (i == 0)
                    {
                        if (obj.Weight > 0)
                        {
                            result.Append(separate + strSplit[i]);
                        }
                        else
                        {
                            result.Append(strSplit[i]);
                        }
                    }
                    else
                    {
                        result.Append(separate + strSplit[i]);
                    }
                }
            }

            return result.ToString();
        }

        #endregion
        #region Метод int GetAgeFromBurth(DateTime birthDate)

        /// <summary>Вычисление возраста по дате рождения</summary>
        /// <param name="birthDate"></param>
        /// <returns></returns>
        public static int GetAgeFromBurth(DateTime birthDate)
        {
            int result = 0;

            try
            {
                DateTime now = DateTime.Now;
                result = now.Year - birthDate.Year;
                if (birthDate > now.AddYears(-result)) result--;
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "CompetitionsWork", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод string AgeCathegory(string Age, string CompetitionName, string SubName)

        /// <summary>Oпределение средней возрастной категории коллектива</summary>
        /// <param name="Age">возраста в формате 'дата или возраст'|'дата или возраст'|'дата или возраст'</param>
        /// <param name="CompetitionName"> конкурс</param>
        /// <param name="SubName">номинация</param>
        /// <returns></returns>
        public static string AgeCathegory(string Age, string CompetitionName, string SubName)
        {
            string someage = "";
            string errVal = "";

            try
            {
                int agepre1down = 0;
                int agepredown = 0;
                int agedown = 0;
                int agemiddle = 0;
                int ageup = 0;
                int agemolodezh = 0;
                int ageover = 0;
                int mixed = 0;
                int group1 = 0;
                int group2 = 0;
                int group3 = 0;
                int group4 = 0;
                int group5 = 0;
                int group6 = 0;
                int profi = 0;
                int group2011 = 0;
                int group2012 = 0;
                int group2013 = 0;
                int group2014 = 0;
                int group2015 = 0;
                int group2016 = 0;

                string[] Ages = Age.Split('|');
                int tmpAge = 0;
                //DateTime dtTmp;
                foreach (string age in Ages)
                {
                    if (age != "")
                    {
                        tmpAge = StringToNum.ParseInt(age);
                        if (tmpAge == -1)   //если передана полноценная дата рождения
                        {
                            errVal = age;   //просто для отлова ошибки конвертирования
                            someage = JustLookAge(GetAgeFromBurth(Convert.ToDateTime(age)), CompetitionName, SubName);
                        }
                        else                //если передан возраст
                        {
                            someage = JustLookAge(tmpAge, CompetitionName, SubName);
                        }

                        if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.baybi)) agepre1down++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.doshkolnaya)) agepredown++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya)) agedown++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya)) agemiddle++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya)) ageup++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh)) agemolodezh++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya)) mixed++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group1)) group1++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2)) group2++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group3)) group3++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group4)) group4++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group5)) group5++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group6)) group6++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi)) profi++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2011)) group2011++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2012)) group2012++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2013)) group2013++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2014)) group2014++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2015)) group2015++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2016)) group2016++;

                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY)) ageover++;
                    }
                }

                //Просто отсортируем полученные кол-ва возрастов в списке (по возрастанию значения)
                List<UniObj> list = new List<UniObj>();
                list.Add(new UniObj() { Num = agepre1down, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.baybi) });
                list.Add(new UniObj() { Num = agepredown, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.doshkolnaya) });
                list.Add(new UniObj() { Num = agedown, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya) });
                list.Add(new UniObj() { Num = agemiddle, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya) });
                list.Add(new UniObj() { Num = ageup, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya) });
                list.Add(new UniObj() { Num = agemolodezh, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh) });
                list.Add(new UniObj() { Num = mixed, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya) });
                list.Add(new UniObj() { Num = group1, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group1) });
                list.Add(new UniObj() { Num = group2, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2) });
                list.Add(new UniObj() { Num = group3, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group3) });
                list.Add(new UniObj() { Num = group4, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group4) });
                list.Add(new UniObj() { Num = group5, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group5) });
                list.Add(new UniObj() { Num = group6, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group6) });
                list.Add(new UniObj() { Num = profi, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi) });
                list.Add(new UniObj() { Num = group2011, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2011) });
                list.Add(new UniObj() { Num = group2012, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2012) });
                list.Add(new UniObj() { Num = group2013, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2013) });
                list.Add(new UniObj() { Num = group2014, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2014) });
                list.Add(new UniObj() { Num = group2015, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2015) });
                list.Add(new UniObj() { Num = group2016, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2016) });
                list.Add(new UniObj() { Num = ageover, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY) });
                list = list.OrderBy(a => a.Num).ToList();

                //Если конкурс кораблик детства и для того чтобы получить смешанную возрастную категорию - хардкодим
                if ((CompetitionName == EnumsHelper.GetKorablikValue(Korablik.self) &&
                    ((SubName == EnumsHelper.GetKorablikValue(Korablik.vokalSolo) ||
                    SubName == EnumsHelper.GetKorablikValue(Korablik.vokalMalieFormi) ||
                    SubName == EnumsHelper.GetKorablikValue(Korablik.vokalAnsambli)) && agepre1down > 0 && agedown > 0)
                    ||
                    ((
                    SubName == EnumsHelper.GetKorablikValue(Korablik.horeographia) ||
                    SubName == EnumsHelper.GetKorablikValue(Korablik.horeographiaBalniyTanets) ||
                    SubName == EnumsHelper.GetKorablikValue(Korablik.horeographiaClassichTanets) ||
                    SubName == EnumsHelper.GetKorablikValue(Korablik.horeographiaEstradTanets) ||
                    SubName == EnumsHelper.GetKorablikValue(Korablik.horeographiaNarodTanets)) && agepre1down > 0 && agedown > 0)
                    ) 
                    ||
                    //Если конкурс Мастер сцены и для того чтобы получить смешанную возрастную категорию - хардкодим
                    (CompetitionName == EnumsHelper.GetTheatreValue(Theatre.self) &&
                    ((SubName == EnumsHelper.GetTheatreValue(Theatre.vokalAkademVokal) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.vokalEstradVokal) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.vokalFolklor)) && ((agedown > 0 && agemiddle > 0) ||
                                                                                      (agedown > 0 && ageup > 0) ||
                                                                                       (agedown > 0 && agemolodezh > 0) ||
                                                                                       (agemiddle > 0 && ageup > 0) ||
                                                                                       (agemiddle > 0 && agemolodezh > 0) ||
                                                                                        (ageup > 0 && agemolodezh > 0)))
                    ||
                    ((SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoClassichTanets) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoNarodTanets) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoEstradTanets) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoBalniyTanets) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoStilNarodTanets) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoSovremTanets) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoCircIskus) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoKadeti)
                                                                                        //|| SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoCherliding)
                                                                                        ) && (agedown > 0 && agemiddle > 0) ||
                                                                                             (agedown > 0 && ageup > 0) ||
                                                                                             (agemiddle > 0 && ageup > 0))
                    ||
                    ((SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrAnsambli) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrDuhovieUdarnInstrum) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrFortepiano) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrGitara) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrNarodnieInstrum) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrOrkestri) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrSintezator) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrStrunnoSmichkovieInstrumenti)) && ((agedown > 0 && agemiddle > 0) ||
                                                                                      (agedown > 0 && ageup > 0) ||
                                                                                       (agedown > 0 && agemolodezh > 0) ||
                                                                                       (agemiddle > 0 && ageup > 0) ||
                                                                                       (agemiddle > 0 && agemolodezh > 0) ||
                                                                                        (ageup > 0 && agemolodezh > 0)))

                     ||
                     ((SubName == EnumsHelper.GetTheatreValue(Theatre.teatrIskusLitMuzKom) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.teatrIskusSpekt) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.teatrIskusMultiGanr)) && (
                                                                                        (agepredown > 0 && agedown > 0) ||
                                                                                      (agepredown > 0 && agemiddle > 0) ||
                                                                                       (agepredown > 0 && ageup > 0) || 
                                                                                       (agedown > 0 && agemiddle > 0) ||
                                                                                      (agedown > 0 && ageup > 0) ||
                                                                                       (agemiddle > 0 && ageup > 0) 
                                                                                        ))
                    || (
                    ((SubName == EnumsHelper.GetTheatreValue(Theatre.hudSlovo))
                                     && (agedown > 0 && agemiddle > 0) ||
                                        (agedown > 0 && ageup > 0) ||
                                        (agemiddle > 0 && ageup > 0)
                                        ))
                    )
                    //Если конкурс вместе мы сила и для того чтобы получить смешанную возрастную категорию - хардкодим
                    || (CompetitionName == EnumsHelper.GetVmesteSilaValue(VmesteSila.self) && ((agedown > 0 && agemiddle > 0) ||
                                                                                      (agedown > 0 && ageup > 0) ||
                                                                                       (agedown > 0 && agemolodezh > 0) ||
                                                                                       (agemiddle > 0 && ageup > 0) ||
                                                                                       (agemiddle > 0 && agemolodezh > 0) ||
                                                                                        (ageup > 0 && agemolodezh > 0)))
                    )
                {
                    someage = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                }
                else //Возмем последнее значение из него с наибольшим кол-вом возрастов
                {
                    var last = list[list.Count - 1];
                    someage = (last.Num > 0 ? list[list.Count - 1].Str : "");
                }
                    
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "CompetitionsWork", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            return someage;
        }

        #endregion
        #region Метод string JustLookAge(int someage, string SubName)

        /// <summary>Метод определения возростной категрии</summary>
        /// <param name="someage"></param>
        /// <param name="CompetitionName">конкурс</param>
        /// <param name="SubName">Номинация</param>
        /// <returns></returns>
        public static string JustLookAge(int someage, string CompetitionName, string SubName)
        {
            string substr = "";

            try
            {
                #region Код

                if (CompetitionName == EnumsHelper.GetPhotoValue(Photo.self) &&
                    (
                    (SubName == EnumsHelper.GetPhotoValue(Photo.izo)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_avtor_igrushka)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_biseropletenie)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_fitodisign)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_gobelen)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_hud_vishivka)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_hud_vyazanie)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_bumagoplastika)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_loskut_shitie)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_makrame)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_voilokovalyanie)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_batik)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_dekupazh)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_hud_obr_kozhi)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_hud_obr_stekla)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_keramika)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_narod_igrush_isglini)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_rospis_poderevu)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_combitehnics)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_plastilonografiya))
                    ))
                {
                    if ((someage >= 7) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 12) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 19) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetPhotoValue(Photo.self) &&
                    (
                        (SubName == EnumsHelper.GetPhotoValue(Photo.computerGraphic)) ||
                        (SubName == EnumsHelper.GetPhotoValue(Photo.computer_risunok)) ||
                        (SubName == EnumsHelper.GetPhotoValue(Photo.collazh_fotomontazh)) ||
                        (SubName == EnumsHelper.GetPhotoValue(Photo.photo))
                    ))
                {
                    if ((someage >= 9) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 12) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 19) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetLiteraryValue(Literary.self) &&
                    (SubName == EnumsHelper.GetLiteraryValue(Literary.stihi) ||
                    SubName == EnumsHelper.GetLiteraryValue(Literary.esse) ||
                    SubName == EnumsHelper.GetLiteraryValue(Literary.rasskaz) ||
                    SubName == EnumsHelper.GetLiteraryValue(Literary.sochinenie)))
                {
                    if ((someage >= 9) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 12) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage < 21)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetTheatreValue(Theatre.self) &&
                    (SubName == EnumsHelper.GetTheatreValue(Theatre.vokalAkademVokal) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.vokalEstradVokal) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.vokalFolklor)))
                {
                    if ((someage >= 8) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 16) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetTheatreValue(Theatre.self) &&
                   (SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoClassichTanets) ||
                   SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoNarodTanets) ||
                   SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoEstradTanets) ||
                   SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoBalniyTanets) ||
                   SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoStilNarodTanets) ||
                   SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoSovremTanets) ||
                   SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoCircIskus) ||
                   SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoKadeti)
                   //|| SubName == EnumsHelper.GetTheatreValue(Theatre.xoreoCherliding)
                   ))
                {
                    if ((someage >= 7) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 11) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 14) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetTheatreValue(Theatre.self) &&
                    (SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrAnsambli) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrDuhovieUdarnInstrum) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrFortepiano) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrGitara) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrNarodnieInstrum) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrOrkestri) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrSintezator) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrStrunnoSmichkovieInstrumenti)))
                {
                    if ((someage >= 8) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 16) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetTheatreValue(Theatre.self) &&
                    (SubName == EnumsHelper.GetTheatreValue(Theatre.teatrIskusLitMuzKom) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.teatrIskusSpekt) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.teatrIskusMultiGanr)))
                {
                    if ((someage >= 4) && (someage <= 6)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.doshkolnaya);
                    else if ((someage >= 7) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 11) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 14) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetTheatreValue(Theatre.self) &&
                    (SubName == EnumsHelper.GetTheatreValue(Theatre.hudSlovo)))
                {
                    if ((someage >= 8) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 11) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    //else if ((someage >= 19) && (someage <= 20)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetKulturaValue(Kultura.self) &&
                    (SubName == EnumsHelper.GetKulturaValue(Kultura.presentaionEn) ||
                    SubName == EnumsHelper.GetKulturaValue(Kultura.iSeeCrimeaEn) ||
                    SubName == EnumsHelper.GetKulturaValue(Kultura.publishVkontakte) ||
                    SubName == EnumsHelper.GetKulturaValue(Kultura.audioGaid) ||
                    SubName == EnumsHelper.GetKulturaValue(Kultura.intellektualKviz)))
                {
                    if ((someage >= 7) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 12) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 16) && (someage <= 21)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetClothesValue(Clothes.self) &&
                    (SubName == EnumsHelper.GetClothesValue(Clothes.tmindividpokaz) ||
                    SubName == EnumsHelper.GetClothesValue(Clothes.tmavtorcollect) ||
                    SubName == EnumsHelper.GetClothesValue(Clothes.tmcollectpokaz) ||
                    SubName == EnumsHelper.GetClothesValue(Clothes.tmnetradicmaterial)
                    //|| SubName == EnumsHelper.GetClothesValue(Clothes.tmissledovproject)
                    ))
                {
                    if (SubName == EnumsHelper.GetClothesValue(Clothes.tmcollectpokaz))
                    {
                        if ((someage >= 6) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                        else if ((someage >= 10) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                        else if ((someage >= 14) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                        else if ((someage >= 19) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi);
                        else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                    }
                    else if (SubName == EnumsHelper.GetClothesValue(Clothes.tmindividpokaz))
                    {
                        if ((someage >= 10) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                        else if ((someage >= 14) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                        else if ((someage >= 18) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi);
                        else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                    }
                    else if (SubName == EnumsHelper.GetClothesValue(Clothes.tmavtorcollect))
                    {
                        if ((someage >= 10) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                        else if ((someage >= 14) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                        else if ((someage >= 18) && (someage <= 21)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi);
                        else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                    }
                    else if (SubName == EnumsHelper.GetClothesValue(Clothes.tmnetradicmaterial))
                    {
                        if ((someage >= 10) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                        else if ((someage >= 14) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                        else if ((someage >= 18) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi);
                        else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                    }
                    //else if (SubName == EnumsHelper.GetClothesValue(Clothes.tmissledovproject))
                    //{
                    //    if ((someage >= 8) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                    //    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                    //}
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.prostEdinoborstva)))
                {
                    if ((someage >= 6) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group1);
                    else if ((someage >= 8) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2);
                    else if ((someage >= 10) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group3);
                    else if ((someage >= 12) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group4);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.boks)))
                {
                    if ((someage >= 10) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 12) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 14) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 16) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.kungfu)))
                {
                    if ((someage >= 6) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.doshkolnaya);
                    else if ((someage >= 8) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 16) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.stendStrelba)))
                {
                    if ((someage >= 8) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 11) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 14) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.shahmaty)))
                {
                    if ((someage >= 6) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group1);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group3);
                    else if ((someage >= 6) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group4);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.shashki)))
                {
                    if ((someage >= 6) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group1);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group3);
                    else if ((someage >= 6) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group4);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.football)))
                {
                    //if ((someage >= 7) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2016);
                    //else if ((someage >= 8) && (someage <= 8)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2015);
                    //else if ((someage >= 9) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2014);
                    //else if ((someage >= 10) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2013);
                    //else if ((someage >= 11) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2012);
                    //else if ((someage >= 12) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2011);
                    //else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                    substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.basketball)))
                {
                    if ((someage >= 13) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.volleyball)))
                {
                    if ((someage >= 11) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 13) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetToponimValue(Toponim.self) &&
                    (SubName == EnumsHelper.GetToponimValue(Toponim.toponimika)))
                {
                    if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetRobotechValue(Robotech.self) &&
                    (SubName == EnumsHelper.GetRobotechValue(Robotech.robototechnika) ||
                    SubName == EnumsHelper.GetRobotechValue(Robotech.robototechnikaproject) ||
                    SubName == EnumsHelper.GetRobotechValue(Robotech.robototechnika3dmodel) ||
                    SubName == EnumsHelper.GetRobotechValue(Robotech.tinkercad) ||
                    SubName == EnumsHelper.GetRobotechValue(Robotech.programmproject)))
                {
                    if ((someage >= 6) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetVmesteSilaValue(VmesteSila.self) &&
                    (SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.hudSlovoPoeziya) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.hudSlovoProza) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaBalniyTanets) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaClassichTanets) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaEstradTanets) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaNarodTanets) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaSovremenTanets) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaOstalnieGanri) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalAkademVokal) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalEstradVokal) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalFolklor) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalZest) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalOstalnieGanri) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrFortepiano) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrSintezator) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrStrunnoSmichkovieInstrumenti) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrDuhovieUdarnInstrum) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrNarodnieInstrum) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrGitara) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrAnsambli) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrOstalnieGanri) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.theatreSpektakl) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.theatreScenka) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.theatreLiteraturnoMusikalnaya) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.theatreDrama) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupDay) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupNight) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupStsena) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupFantasy) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairPletenie) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairDay) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairNight) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairFantasy)
                        )
                    )
                {
                    if ((someage >= 6) && (someage <= 8)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 9) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 13) && (someage <= 16)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 17) && (someage <= 21)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    //выбор смешанной категории складывается из того, что есть кто-то в бэйби и кто-то в младшей группе
                    //else if (someage <= 7) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                
                else if (CompetitionName == EnumsHelper.GetMultimediaValue(Multimedia.self) &&
                    (
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.yarisuupobedy) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.spesneirpobede) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.geroyamotserdca) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.plechomkplechu) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.pamyatsilneevremeni)
                    ))
                {
                    if ((someage >= 6) && (someage <= 21)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetMultimediaValue(Multimedia.self) &&
                    (
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.metodicheskierazrabotki)
                    ))
                {
                    if ((someage >= 6) && (someage <= 100)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                
                else if (CompetitionName == EnumsHelper.GetClothesValue(Clothes.self) &&
                   (
                   //(SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturie)) ||
                   (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieTkan)) ||
                   (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieNetradicMaterial)) ||
                   (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieFashion)) ||
                   (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieTechRisunok)) ||
                   (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieFoodArt)) ||
                   (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieOgorod)) ||
                   (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieBeauty)) ||

                   (SubName == EnumsHelper.GetClothesValue(Clothes.chudoLoskutkiIgrushkiKukliTvorRisunok)) ||
                   (SubName == EnumsHelper.GetClothesValue(Clothes.chudoLoskutkiIgrushkiKukli)) 

                   //(SubName == EnumsHelper.GetClothesValue(Clothes.eskiziModelier)) ||
                   //(SubName == EnumsHelper.GetClothesValue(Clothes.eskiziModelierTvorRisunok)) ||
                   //(SubName == EnumsHelper.GetClothesValue(Clothes.eskiziModelierFashion)) ||
                   //(SubName == EnumsHelper.GetClothesValue(Clothes.eskiziModelierTechRisunok)) 
                   )
                   )
                {
                    if ((someage >= 8) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 12) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 19) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetClothesValue(Clothes.self) &&
                    (
                    (SubName == EnumsHelper.GetClothesValue(Clothes.sedobnayaModa)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.sedobnayaModaFoodArt)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.sedobnayaModaOgorod)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.sedobnayaModaBeauty))
                    )
                    )
                {
                    if ((someage >= 6) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 10) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 14) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 19) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetKorablikValue(Korablik.self) &&
                    ((SubName == EnumsHelper.GetKorablikValue(Korablik.hudSlovo)))
                    )
                {
                    if ((someage >= 3) && (someage <= 5)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.baybi);
                    else if ((someage >= 6) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetKorablikValue(Korablik.self) &&
                    ((SubName == EnumsHelper.GetKorablikValue(Korablik.horeographia)) ||
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.horeographiaBalniyTanets)) ||
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.horeographiaClassichTanets)) ||
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.horeographiaEstradTanets)) ||
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.horeographiaNarodTanets))
                    ))
                {
                    if ((someage >= 3) && (someage <= 5)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.baybi);
                    else if ((someage >= 6) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    //выбор смешанной категории складывается из того, что есть кто-то в бэйби и кто-то в младшей группе
                    //else if (someage <= 7) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetKorablikValue(Korablik.self) &&
                    (
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.vokalSolo)) ||
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.vokalMalieFormi)) ||
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.vokalAnsambli)))
                    )
                {
                    if ((someage >= 3) && (someage <= 5)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.baybi);
                    else if ((someage >= 6) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    //выбор смешанной категории складывается из того, что есть кто-то в дошкольной и кто-то в младшей группе
                    //else if ((someage >= 3) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetCrimrouteValue(Crimroute.self) &&
                    (
                    (SubName == EnumsHelper.GetCrimrouteValue(Crimroute.historyplace)) ||
                    (SubName == EnumsHelper.GetCrimrouteValue(Crimroute.militaryhistoryplace)) ||
                    (SubName == EnumsHelper.GetCrimrouteValue(Crimroute.literaturehistoryplace)))
                    )
                {
                    if ((someage >= 6) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 11) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 16) && (someage <= 21)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                //else if (CompetitionName == EnumsHelper.GetMathbattleValue(Mathbattle.self) &&
                //        SubName == EnumsHelper.GetMathbattleValue(Mathbattle.battle)
                //    )
                //{
                //    if ((someage >= 7) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                //    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                //    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                //    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                //}
                else if (CompetitionName == EnumsHelper.GetKosmosValue(Kosmos.self) &&
                        SubName == EnumsHelper.GetKosmosValue(Kosmos.kosmos)
                    )
                {
                    if ((someage >= 7) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetScienceValue(Science.self) &&
                    (SubName == EnumsHelper.GetScienceValue(Science.ekologia_ochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.ekologia_zaochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.himiya_ochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.himiya_zaochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.fizika_ochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.fizika_zaochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.biologiya_ochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.biologiya_zaochno)))
                {
                    if ((someage >= 7) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 11) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "CompetitionsWork", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            return substr;
        }

        #endregion
        #region Метод GetAgeCategory(.)

        /// <summary>Метод получения значения возрастной категории прямо из объекта заявки</summary>
        /// <param name="obj">объект заявки</param>
        /// <returns></returns>
        public static string GetAgeCategory(CompetitionRequest obj)
        {
            string result = "";

            if (!string.IsNullOrEmpty(obj.AgeСategory.Trim()))
            {
                result = obj.AgeСategory;
                return result;
            }

            List<string> listOfAgies = new List<string>();
            DateTime dt;

            if (obj.Age != "" && obj.Age != "0")
            {
                if (DateTime.TryParse(obj.Age, out dt))
                {
                    listOfAgies.Add(GetAgeFromBurth(dt).ToString());
                }
                else
                {
                    if (StringToNum.ParseInt(obj.Age) != -1)
                    {
                        listOfAgies.Add(obj.Age);
                    }
                }
            }

            if (obj.Agies != "")
            {
                string[] Agies = obj.Agies.Split(new Char[] { '|' });
                Agies = Agies.Where(x => x != "" && x != "0").ToArray();
                foreach (string item in Agies)
                {
                    if (DateTime.TryParse(item, out dt))
                    {
                        listOfAgies.Add(GetAgeFromBurth(dt).ToString());
                    }
                    else
                    {
                        if (StringToNum.ParseInt(item) != -1)
                        {
                            listOfAgies.Add(item);
                        }
                    }
                }
            }

            if (listOfAgies.Count == 0)  //если в заявке вообще нет возрастов, значит ВНЕ КАТЕГОРИИ
            {
                result = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                return result;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < listOfAgies.Count; i++)
            {
                if (i == 0) sb.Append(listOfAgies[i]); else sb.Append("|" + listOfAgies[i]);
            }

            result = AgeCathegory(sb.ToString(), obj.CompetitionName, obj.SubsectionName);

            return result;
        }

        #endregion
        #region GetResultDocumentForParticipant
        /// <summary>
        /// Метод получает результат по номинации
        /// </summary>
        /// <returns></returns>
        public static string GetResultDocumentForParticipant(CompetitionRequest req, bool needEmpty = false)
        {
            string result = "";
            if (
                    req.CompetitionName == EnumsHelper.GetPhotoValue(Photo.self) &&
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.izo)
                )
            {
                result = GetPointsIzoText(req);
            }
            else if (
                req.CompetitionName == EnumsHelper.GetPhotoValue(Photo.self) &&
                (
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_avtor_igrushka) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_biseropletenie) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_fitodisign) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_gobelen) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_hud_vishivka) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_hud_vyazanie) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_bumagoplastika) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_loskut_shitie) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_makrame) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_voilokovalyanie) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_batik) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_dekupazh) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_hud_obr_kozhi) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_hud_obr_stekla) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_keramika) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_narod_igrush_isglini) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_rospis_poderevu) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_combitehnics) ||
                    req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_plastilonografiya)
                )
                )
            {
                result = GetPointsDPIText(req);
            }
            else if (
                    req.CompetitionName == EnumsHelper.GetPhotoValue(Photo.self) &&
                    (
                        req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.photo) ||
                        req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.collazh_fotomontazh) ||
                        req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.computerGraphic) ||
                        req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.computer_risunok)
                    )
                    )
            {
                result = GetPointsPhotoGraphicText(req);
            }
            else
            {
                if (!string.IsNullOrEmpty(req.Result))
                {
                    result = req.Result.Replace("\r", "").Replace("\n", "  ").Replace(";", ",").Replace("\"", " ");
                }
                else
                {
                    result = (needEmpty ? "" : "ДИПЛОМАНТ");
                }
            }
            return result;
        }
        #endregion
        #region Метод GetPointsDPIText(.)

        /// <summary>Метод получения значения текста исходя из кол-ва введенных баллов</summary>
        /// <param name="obj">объект заявки</param>
        /// <returns></returns>
        public static string GetPointsDPIText(CompetitionRequest obj)
        {
            string result = "";

            if (obj.Points <= 17)
                result = "УЧАСТНИК";
            else if (obj.Points >= 18 && obj.Points <= 22)
                result = "ДИПЛОМАНТ";
            else if (obj.Points >= 23 && obj.Points <= 27)
                result = "ЛАУРЕАТ III СТЕПЕНИ";
            else if (obj.Points >= 28 && obj.Points <= 32)
                result = "ЛАУРЕАТ II СТЕПЕНИ";
            else if (obj.Points >= 33 && obj.Points != 100)
                result = "ЛАУРЕАТ I СТЕПЕНИ";
            else if (obj.Points == 100)
                result = "ЛАУРЕАТ \"ПРИЗНАНИЕ ЗРИТЕЛЕЙ\"";

            return result;
        }

        #endregion

        #region Метод GetPointsIzoText(.)

        /// <summary>Метод получения значения текста исходя из кол-ва введенных баллов</summary>
        /// <param name="obj">объект заявки</param>
        /// <returns></returns>
        public static string GetPointsIzoText(CompetitionRequest obj)
        {
            string result = "";

            if (obj.Points >= 0 && obj.Points <= 12)
                result = "УЧАСТНИК";
            else if (obj.Points >= 13 && obj.Points <= 18)
                result = "ДИПЛОМАНТ III СТЕПЕНИ";
            else if (obj.Points >= 19 && obj.Points <= 21)
                result = "ДИПЛОМАНТ II СТЕПЕНИ";
            else if (obj.Points >= 22 && obj.Points <= 24)
                result = "ДИПЛОМАНТ I СТЕПЕНИ";
            else if (obj.Points >= 25 && obj.Points <= 27)
                result = "ЛАУРЕАТ III СТЕПЕНИ";
            else if (obj.Points >= 28 && obj.Points <= 32)
                result = "ЛАУРЕАТ II СТЕПЕНИ";
            else if (obj.Points >= 33 && obj.Points != 100)
                result = "ЛАУРЕАТ I СТЕПЕНИ";
            else if (obj.Points == 100)
                result = "ЛАУРЕАТ \"ПРИЗНАНИЕ ЗРИТЕЛЕЙ\"";

            return result;
        }

        #endregion

        #region Метод GetPointsPhotoGraphicText(.)

        /// <summary>Метод получения значения текста исходя из кол-ва введенных баллов</summary>
        /// <param name="obj">объект заявки</param>
        /// <returns></returns>
        public static string GetPointsPhotoGraphicText(CompetitionRequest obj)
        {
            string result = "";

            if (obj.Points <= 20)
                result = "УЧАСТНИК";
            else if (obj.Points >= 21 && obj.Points <= 29)
                result = "ДИПЛОМАНТ";
            else if (obj.Points >= 30 && obj.Points <= 35)
                result = "ЛАУРЕАТ III СТЕПЕНИ";
            else if (obj.Points >= 36 && obj.Points <= 41)
                result = "ЛАУРЕАТ II СТЕПЕНИ";
            else if (obj.Points >= 42 && obj.Points <= 45)
                result = "ЛАУРЕАТ I СТЕПЕНИ";

            return result;
        }

        #endregion

        #region Метод string FioReduce(string fio)

        /// <summary>Метод превращает строку вида 'Ситников Андрей Михайлович' в строку вида 'Ситников А.М.'</summary>
        /// <param name="fio">ФИО полностью</param>
        /// <returns></returns>
        public static string FioReduce(string fio)
        {
            string result = fio.Trim();

            #region Проверки

            if (fio == "") return "";
            if (!fio.Contains(" ")) return result;

            #endregion

            try
            {
                #region Код

                string[] tmpArr = result.Split(new[] { ' ' });
                if (tmpArr.Length == 3)
                {
                    result = tmpArr[0] + " " + tmpArr[1].Substring(0, 1).ToUpper() + "." + tmpArr[2].Substring(0, 1).ToUpper() + ".";
                }
                else if (tmpArr.Length == 2)    //проверка на всякий случай
                {
                    result = tmpArr[0] + " " + tmpArr[1].Substring(0, 1).ToUpper() + ".";
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "CompetitionsWork", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод string FioReduceFI(string fio)

        /// <summary>Метод превращает строку вида 'Ситников Андрей Михайлович' в строку вида 'Ситников Андрей'</summary>
        /// <param name="fio">ФИО полностью</param>
        /// <returns></returns>
        public static string FioReduceFI(string fio)
        {
            string result = fio.Trim();

            #region Проверки

            if (fio == "") return "";
            if (!fio.Contains(" ")) return result;

            #endregion

            try
            {
                #region Код

                string[] tmpArr = result.Split(new[] { ' ' });

                if (tmpArr.Length > 1)
                {
                    result = (tmpArr.ElementAtOrDefault(0) != null ? tmpArr[0] : "") + " " + (tmpArr.ElementAtOrDefault(1) != null ? tmpArr[1] : "");
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "CompetitionsWork", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            return result;
        }

        #endregion
        #region IsPartiesFromOneAgeCategory(.)

        /// <summary>Метод проверяет, все ли участники в заявке находятся в одной и той же возрастной категории</summary>
        /// <param name="obj">объект заявки</param>
        /// <returns>true - если все ли участники в заявке находятся в одной и той же возрастной категории</returns>
        public static bool IsPartiesFromOneAgeCategory(CompetitionRequest obj)
        {
            bool result = true;

            #region Проверки-подстраховки

            //если участник единственный в заявке, то проверять не нужно
            if (!string.IsNullOrEmpty(obj.Age.Trim()))
                return true;

            //если список возрастов пустой, то ошибка
            if (string.IsNullOrEmpty(obj.Agies.Trim()))
                return false;

            #endregion

            string[] birthDatesArr = obj.Agies.Split(new[] { '|' });
            string lastCategory = "";
            string currCategory = "";

            foreach (string item in birthDatesArr)
            {
                currCategory = AgeCathegory(item, obj.CompetitionName, obj.SubsectionName);
                if (lastCategory == "")
                {
                    lastCategory = currCategory;
                }
                if (currCategory != lastCategory)       // если возрастные категории не совпали, значит  содержит участников не одной возрастной категории
                {
                    result = false;
                    break;
                }
                lastCategory = currCategory;
            }

            return result;
        }

        #endregion
        #region Метод transformYoutubeLink(.)

        /// <summary>Метод проверяет переданную строку на видеоролик Ютуба на правильность и если нужно преобразовывается ее к нужному виду.
        /// Проверяется соответствие следующим образцам url-строк:
        /// https://www.youtube.com/watch?v=udKYsRO1njA
        /// https://youtu.be/udKYsRO1njA
        /// /iframe width="560" height="315" src="https://www.youtube.com/embed/eV-d47cDYKI" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen///iframe/
        /// Целевой вид url-строки:
        /// https://www.youtube.com/embed/udKYsRO1njA
        /// </summary>
        /// <param name="rawLink">ссылка, переданная заявителем</param>
        /// <returns>Возвращает массив из 2-х строковых значений. 1 значение - целевая ссылка, 2 значение - описание ошибки (если это значение пустое, значение ошибки не произошло)</returns>
        public static string[] transformYoutubeLink(string rawLink)
        {
            string[] result = new string[] { "", "" };

            string res = rawLink.Trim();

            #region Проверки-подстраховки

            if (res == "" || !res.StartsWith("https://"))
            {
                result[1] = "Добавьте ссылку на видеоролик согласно инструкции (текст выше)";
                return result;
            }
            if (res.StartsWith("https://") && !res.Contains("src=\"") && !res.Contains("src='"))
            {
                result[1] = "Вы пытаетесь добавить неправильную ссылку на видео, скопируйте ссылку согласно инструкции (текст выше)";
                return result;
            }

            #endregion

            #region Трансформации ссылки

            if (res.Contains("watch?v="))
            {
                res = res.Replace("watch?v=", "embed/");
            }
            else if (res.Contains("/embed/") && !res.Contains("src=\"") && !res.Contains("src='"))
            {
                string[] linkArrAll = res.Split(new string[] { "/embed/" }, StringSplitOptions.None);
                if (linkArrAll.Length == 2 && linkArrAll[0].Contains("youtube") && linkArrAll[1] != "")
                {
                    // все верно!
                }
                else
                {
                    result[1] = "Вы пытаетесь добавить некорректную ссылку на видео, скопируйте ссылку согласно инструкции (текст выше)";
                    return result;
                }
            }
            else if (res.Contains("src=\""))
            {
                string[] linkArrAll = res.Split(new string[] { "src=\"" }, StringSplitOptions.None);
                if (linkArrAll[1].Contains("\""))
                {
                    string[] linkArrRight = linkArrAll[1].Split(new string[] { "\"" }, StringSplitOptions.None);
                    res = linkArrRight[0];
                }
                else
                {
                    result[1] = "Неправильная строка со ссылкой на видео Youtube, скопируйте ссылку согласно инструкции (текст выше)";
                    return result;
                }
            }
            else if (res.Contains("src='"))
            {
                string[] linkArrAll = res.Split(new string[] { "src='" }, StringSplitOptions.None);
                if (linkArrAll[1].Contains("'"))
                {
                    string[] linkArrRight = linkArrAll[1].Split(new string[] { "'" }, StringSplitOptions.None);
                    res = linkArrRight[0];
                }
                else
                {
                    result[1] = "Неправильная строка со ссылкой на видео Youtube, скопируйте ссылку согласно инструкции (текст выше)";
                    return result;
                }
            }
            else
            {
                // Проверим на случай получения ссылки вида - https://youtu.be/udKYsRO1njA и если ссылка такая, то пересоберем её
                string[] linkArrAll = res.Split(new string[] { "/" }, StringSplitOptions.None);
                if (linkArrAll.Length == 4)
                {
                    res = "https://www.youtube.com/embed/" + linkArrAll[linkArrAll.Length - 1];
                }
                else
                {
                    result[1] = "Переданная строка не распознана, как ссылка на видео Youtube, скопируйте ссылку согласно инструкции (текст выше)";
                    return result;
                }
            }

            #endregion

            result[0] = res;

            return result;
        }

        #endregion
        #region Метод transformInstagramLink(.)

        /// <summary>Метод проверяет переданную строку на публикацию в Instagram на правильность и если нужно преобразовывается ее к нужному виду.
        /// Целевой вид url-строки:
        /// https://www.instagram.com/p/CXbccwYIcKn/embed
        /// </summary>
        /// <param name="rawLink">ссылка, переданная заявителем</param>
        /// <returns>Возвращает массив из 2-х строковых значений. 1 значение - целевая ссылка, 2 значение - описание ошибки (если это значение пустое, значение ошибки не произошло)</returns>
        public static string[] transformInstagramLink(string rawLink)
        {
            string[] result = new string[] { "", "" };

            string res = rawLink.Trim();

            #region Проверки-подстраховки

            if (res == "" || !res.ToLower().StartsWith("https://"))
            {
                result[1] = "Добавьте ссылку на публикацию Instagram согласно инструкции (текст выше)";
                return result;
            }
            if (res.ToLower().StartsWith("https://") && !res.ToLower().Contains("instagram"))
            {
                result[1] = "Вы пытаетесь добавить неправильную ссылку на публикацию, скопируйте ссылку согласно инструкции (текст выше)";
                return result;
            }

            #endregion

            #region Трансформации ссылки

            if (res.Contains("/?")) {
                res = res.Substring(0, res.IndexOf("/?"));
            }
            if (!res.EndsWith("/")) {
                res += "/";
            }

            res += "embed";

            #endregion

            result[0] = res;

            return result;
        }

        #endregion
        #region Метод transformYandexDiskLink(.)

        /// <summary>Метод проверяет переданную строку на файл в YandexDisk на правильность и если нужно преобразовывается ее к нужному виду.
        /// Целевой вид url-строки:
        /// https://disk.yandex.ru/d/CwtUiJ7NJLs45A
        /// </summary>
        /// <param name="rawLink">ссылка, переданная заявителем</param>
        /// <returns>Возвращает массив из 2-х строковых значений. 1 значение - целевая ссылка, 2 значение - описание ошибки (если это значение пустое, значение ошибки не произошло)</returns>
        public static string[] transformYandexDiskLink(string rawLink)
        {
            string[] result = new string[] { "", "" };

            string res = rawLink.Trim();

            #region Проверки-подстраховки

            if (res == "" || !res.ToLower().StartsWith("https://"))
            {
                result[1] = "Добавьте ссылку на файл ЯНДЕКС ДИСКА согласно инструкции (текст выше)";
                return result;
            }
            if (res.ToLower().StartsWith("https://") && !res.ToLower().Contains("disk.yandex"))
            {
                result[1] = "Вы пытаетесь добавить неправильную ссылку на файл ЯНДЕКС ДИСКА, скопируйте ссылку согласно инструкции (текст выше)";
                return result;
            }

            #endregion

            result[0] = res;

            return result;
        }

        #endregion
        #region Метод transformRutubeLink(.)

        /// <summary>Метод проверяет переданную строку на видео с RUTUBE на правильность и если нужно преобразовывается ее к нужному виду.
        /// Целевой вид url-строки:
        /// https://rutube.ru/video/269e9cc867b13e457c900097872fc74d/
        /// </summary>
        /// <param name="rawLink">ссылка, переданная заявителем</param>
        /// <returns>Возвращает массив из 2-х строковых значений. 1 значение - целевая ссылка, 2 значение - описание ошибки (если это значение пустое, значение ошибки не произошло)</returns>
        public static string[] transformRutubeLink(string rawLink)
        {
            string[] result = new string[] { "", "" };

            string res = rawLink.Trim();

            #region Проверки-подстраховки

            if (res == "" || !res.ToLower().StartsWith("https://"))
            {
                result[1] = "Добавьте ссылку на RUTUBE видео согласно инструкции (текст выше)";
                return result;
            }
            if (res.ToLower().StartsWith("https://") && !res.ToLower().Contains("rutube.ru"))
            {
                result[1] = "Вы пытаетесь добавить неправильную ссылку на видео с RUTUBE, скопируйте ссылку согласно инструкции (текст выше)";
                return result;
            }

            #endregion

            result[0] = res;

            return result;
        }

        #endregion
        #region Метод transformVkontakteLink(.)

        /// <summary>Метод проверяет переданную строку на публикацию в Vkontakte на правильность и если нужно преобразовывается ее к нужному виду.
        /// Целевой вид url-строки:
        /// https://vk.com/wall268756_535
        /// </summary>
        /// <param name="rawLink">ссылка, переданная заявителем</param>
        /// <returns>Возвращает массив из 2-х строковых значений. 1 значение - целевая ссылка, 2 значение - описание ошибки (если это значение пустое, значение ошибки не произошло)</returns>
        public static string[] transformVkontakteLink(string rawLink)
        {
            string[] result = new string[] { "", "" };

            string res = rawLink.Trim();

            #region Проверки-подстраховки

            if (res == "" || !res.ToLower().StartsWith("https://"))
            {
                result[1] = "Добавьте ссылку на публикацию согласно инструкции (текст выше)";
                return result;
            }
            if (res.ToLower().StartsWith("https://") && !res.ToLower().Contains("vk.com"))
            {
                result[1] = "Вы пытаетесь добавить неправильную ссылку на публикацию в Vkontakte, скопируйте ссылку согласно инструкции (текст выше)";
                return result;
            }

            #endregion

            result[0] = res;

            return result;
        }

        #endregion
        #region Метод void GetResultDocument_Common(...)

        /// <summary>Метод формирует требуемый документ для всех конкурсов (Диплом, Сертификат, Благодарственное письмо) и возвращает его браузеру для скачивания.</summary>
        /// <param name="reqId">номер заявки участника(ов)</param>
        /// <param name="page">объект страницы</param>
        /// <param name="docType">тип формируемого документа</param>
        /// <param name="warning">инициализированный объект Предупреждения (необязательный параметр)</param>
        /// <param name="personFio">фио участника полностью. Нужен для формирования файла с одним сертификатом участника, даже если он в коллективе</param>
        /// <param name="clubsForm">форма представления сертификата для Клуба, Коллектива (формируется сертификат на Коллектив без указания фамилий, он формируется даже если в заявке нет ни одной фамилии участника)</param>
        public void GetResultDocument_Common(string reqId, Page page, DocumentType docType, WarnClass warning = null, string personFio = "", bool clubsForm = false, int protocolType = 2)
        {
            #region Получение структуры данных

            var req = CompetitonWorkCommon.GetCompetitionRequest(reqId);

            if (req == null && warning != null)
            {
                warning.ShowWarning("Не удалось найти строку в БД. Попробуйте повторить..", page.Master);
                return;
            }

            #endregion

            #region Создание списка фамилий конкурсантов

            List<PersonPair> fiosList = new List<PersonPair>();
            if (protocolType == 2 && req.Fio != "" && req.Fio.ToUpper().Trim() != "НЕТ")
            {
                fiosList.Add(new PersonPair() { Name = FioReduceFI(req.Fio) });
            }

            string tmp = (protocolType == 2 ? req.Fios : req.Fios_1);
            if (!string.IsNullOrEmpty(tmp))
            {
                string[] strSplit = tmp.Split(new[] { '|' });
                if (strSplit.Length > 0)
                    fiosList = new List<PersonPair>();
                foreach (string item in strSplit.Distinct())
                {
                    if (!fiosList.Exists(x => FioReduceFI(x.Name) == item))
                        fiosList.Add(new PersonPair() { Name = FioReduceFI(item) });
                }
            }

            //проверку только на индивидуальные документы
            if (fiosList.Count == 0 && !clubsForm && warning != null)
            {
                warning.ShowWarning("Не удалось найти конкурсантов для печати документов. Попробуйте повторить..", page.Master);
                return;
            }

            //сортировка по алфавиту
            if (fiosList.Count > 0)
                fiosList = fiosList.OrderBy(x => x.Name).ToList();

            #endregion

            #region Сбор фамилий педагогов

            var fiosChiefList = GetChiefsFiosPositionList(req);

            #endregion

            var thankLetterText = new StringBuilder();
            //если выбрали благодарность, тогда печатаем благодарности для каждого учителя
            if (docType == DocumentType.Blagodarnost)
            {
                fiosList = fiosChiefList;

                if (req.CompetitionName == EnumsHelper.GetVmesteSilaCode(VmesteSila.self))
                {
                    thankLetterText.Append("За активное участие в подготовке конкурсантов; За профессионализм; ");
                    thankLetterText.Append("За большой вклад в развитие, образование, воспитание и раскрытие творческих ");
                    thankLetterText.Append("способностей детей с ограниченными возможностями;");
                    thankLetterText.Append("За индивидуальный подход к каждому ребенку;");
                    thankLetterText.Append("За ваш благородный труд;");
                }
                //if (((CompetitionRequest)req).CompetitionName == EnumsHelper.GetScienceCode(Science.self))
                //{
                //    thankLetterText.Append("За помощь в судействе и проведении конкурса научных работ \"В моей лаборатории вот что... \", который проходил 30 апреля 2023 года в рамках VIII Комплексного образовательного проекта \"Москва - Крым - Территория талантов\"");
                //}
                if (req.CompetitionName == EnumsHelper.GetPhotoCode(Photo.self)
                    &&
                       (
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.izo)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_avtor_igrushka)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_biseropletenie)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_fitodisign)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_gobelen)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_hud_vishivka)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_hud_vyazanie)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_bumagoplastika)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_loskut_shitie)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_makrame)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_voilokovalyanie)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_batik)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_dekupazh)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_hud_obr_kozhi)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_hud_obr_stekla)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_keramika)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_narod_igrush_isglini)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT2_rospis_poderevu)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_combitehnics)) ||
                           (req.SubsectionName == EnumsHelper.GetPhotoValue(Photo.DPT1_plastilonografiya))
                       )
                    )
                {
                    thankLetterText.Append("Выражаем благодарность за Ваш профессионализм в воспитании и раскрытии творческого потенциала обучающихся, индивидуальный подход к каждому ребёнку. За работу по подготовке победителей конкурса");
                }
                if (req.CompetitionName == EnumsHelper.GetKulturaCode(Kultura.self))
                {
                    thankLetterText.Append("За подготовку победителя");
                }

                if (req.CompetitionName == EnumsHelper.GetMathbattleCode(Mathbattle.self))
                {
                    thankLetterText.Append("За активное привлечение учащихся к участию в проекте, ");
                    thankLetterText.Append("За умение увлечь за собой, ");
                    thankLetterText.Append("За творческий подход к образовательному процессу, ");
                    thankLetterText.Append("За развитие познавательного интереса учащихся к математике. ");
                }
            }

            #region Определение имени файла

            string fileName = CompetitonWorkCommon.GetDocFileName(docType, reqId);

            #endregion

            string pdfFile = Constants.PATH_TO_MAINFOLDER + @"/temp/" + fileName;

            bool checker = true;        //в случае возникновения ошибки предотвращает генерацию файла

            try
            {
                #region Код

                FileStream stream = new FileStream(pdfFile, FileMode.Create);

                iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
                Document doc = new Document(rec, 0f, 0f, 0f, 0f);
                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                #region Подготовка парсера

                HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
                htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
                ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                IPipeline pipeline = new CssResolverPipeline(cssResolver,
                                     new HtmlPipeline(htmlContext,
                                     new PdfWriterPipeline(doc, pdfWriter)));


                XMLWorker worker = new XMLWorker(pipeline, true);
                XMLParser p = new XMLParser(true, worker, Encoding.UTF8);

                #endregion

                #region Подготовка HTML и CSS кода для преобразования в PDF

                #region Загрузка шрифтов

                FontFactory.Register(Constants.PATH_TO_MAIN + @"\fonts\Standarts\times.ttf");     //регистрация нужного шрифта для отображения в PDF Кириллицы
                FontFactory.Register(Constants.PATH_TO_MAIN + @"\fonts\Standarts\timesbd.ttf");     //регистрация нужного шрифта для отображения в PDF Кириллицы
                FontFactory.Register(Constants.PATH_TO_MAIN + @"\fonts\Standarts\timesbi.ttf");     //регистрация нужного шрифта для отображения в PDF Кириллицы
                FontFactory.Register(Constants.PATH_TO_MAIN + @"\fonts\Standarts\timesi.ttf");     //регистрация нужного шрифта для отображения в PDF Кириллицы
                FontFactory.Register(Constants.PATH_TO_MAIN + @"\fonts\Standarts\arial.ttf");     //регистрация нужного шрифта для отображения в PDF Кириллицы
                FontFactory.Register(Constants.PATH_TO_MAIN + @"\fonts\Standarts\arialbd.ttf");     //регистрация нужного шрифта для отображения в PDF Кириллицы
                FontFactory.Register(Constants.PATH_TO_MAIN + @"\fonts\Standarts\arialbi.ttf");     //регистрация нужного шрифта для отображения в PDF Кириллицы
                FontFactory.Register(Constants.PATH_TO_MAIN + @"\fonts\Standarts\ariali.ttf");     //регистрация нужного шрифта для отображения в PDF Кириллицы

                #endregion

                #region Получение названия конкурса для печатаемых документов

                StringBuilder sb = new StringBuilder();
                string competitionName = EnumsHelper.GetCompetitionValueFromCodeForDocument(req.CompetitionName);

                #endregion

                #region Признак по которому не показывать номинацию в документах

                bool showSubsectionName = true;
                if (req.CompetitionName == EnumsHelper.GetKosmosCode(Kosmos.self) ||
                    req.CompetitionName == EnumsHelper.GetMathbattleCode(Mathbattle.self))
                {
                    showSubsectionName = false;
                }

                #endregion

                #region Получение нужного имени файла диплома, сертификата, благодарственного письма

                string bckgrdFileName = CompetitonWorkCommon.GetBckgrdFileName(docType, req.DateReg);

                #endregion

                #region Если нужно отсечь по одной фамилии

                if (!string.IsNullOrEmpty(personFio) && fiosList.Count > 0) //если нужно получить индивидуальный сертификат участника
                {
                    fiosList = fiosList.Where(a => a.Name == FioReduceFI(personFio))
                                        .Select(x => new PersonPair() { Name = x.Name })
                                        .ToList();
                }

                #endregion

                #region Создание списка для групповых документов с переводом строки

                var list = new StringBuilder();
                bool newStroke = false;
                for (int i = 1; i <= fiosList.Count; i++)
                {
                    if (i == 1)             //первая фамилия в списке выставляется без запятой
                    {
                        list.Append(fiosList[i - 1].Name);
                    }
                    else
                    {
                        if (i % 5 == 0)      //перенос на новую строку после трёх фамилий
                        {
                            if (i == fiosList.Count)  //если фамилия является последней в списке и это конец строки, то запятая и перенос не нужны
                            {
                                list.Append(", " + fiosList[i - 1].Name);
                            }
                            else
                            {
                                //list.Append(", " + fiosList[i - 1] + "," + "<br />");
                                list.Append(", " + fiosList[i - 1].Name + ", ");
                            }
                            newStroke = true;
                        }
                        else if (newStroke) //для правильной расстановки запятых при переносе на новую строку
                        {
                            list.Append(fiosList[i - 1].Name);
                            newStroke = false;
                        }
                        else
                        {
                            list.Append(", " + fiosList[i - 1].Name);
                        }
                    }
                }
                #endregion

                #region Результат
                string result = "";
                if (docType == DocumentType.Diplom)
                {
                    result = GetResultDocumentForParticipant(req);
                }
                #endregion

                var ageCategory = GetAgeCategory(req);

                if (clubsForm)
                {
                    sb.Append("<html>");

                    sb.Append("<head>");
                    sb.Append("<style>");
                    sb.Append("table { border-collapse: collapse; border-spacing: 0; margin-top: 425px; } ");
                    sb.Append("table td { text-align: center; } ");
                    //sb.Append("span { font-family: 'Arial'; } ");
                    sb.Append("span { font-family: 'Times New Roman'; } ");
                    sb.Append("</style>");
                    sb.Append("</head>");

                    sb.Append("<body>");

                    sb.Append("<div style='background-image: url(" + Constants.PATH_TO_MAIN + "/images/" + bckgrdFileName + "); height:100%;'>");

                    sb.Append("<table align=center width=650>");

                    if (docType == DocumentType.Diplom)
                    {
                        #region Конкурс
                        sb.Append("<tr><td style='padding-top: 30px;'><span style='font-size: 24px; font-weight: normal; color: #000000;'>" + competitionName + "</span></td></tr>");
                        #endregion

                        #region Номинация
                        if (showSubsectionName && ((CompetitionRequest)req).SubsectionName != "" && ((CompetitionRequest)req).SubsectionName.ToUpper().Trim() != "НЕТ" && ((CompetitionRequest)req).SubsectionName.ToUpper().Trim() != "-")
                        {
                            sb.Append("<tr><td style='padding-top: 10px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>Номинация: «<b>" + ((CompetitionRequest)req).SubsectionName + "</b>»</span></td></tr>");
                        }
                        #endregion

                        #region Возрастная категория
                        if (!string.IsNullOrEmpty(ageCategory))
                        {
                            sb.Append("<tr><td style='padding-top: 10px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>Возрастная категория «" + ageCategory + "»</span></td></tr>");
                        }
                        #endregion

                        sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 30px; font-weight: bold; color: #000000; letter-spacing: 5px;'>НАГРАЖДАЕТСЯ</span></td></tr>");

                        #region Результат

                        sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 30px; font-weight: bold; color: #e11f27;'>" + result + "</span></td></tr>");

                        #endregion

                        #region Коллектив
                        if (req.ClubsName != "" && req.ClubsName.ToUpper().Trim() != "НЕТ" && req.ClubsName.ToUpper().Trim() != "-")                //Коллектив с названием и его состав
                        {
                            sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 20px; font-weight: bold; color: #000000;'>" + req.ClubsName + "</span></td></tr>");
                        }
                        else
                        {
                            sb.Append("<tr><td style='padding-top: 10px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>&nbsp;</span></td></tr>");
                        }
                        #endregion

                        #region Название работы
                        //только для Театральное искусство ( "Театр моды: Коллективный показ","Театр моды: Авторская коллекция","Театр моды: Индивидуальный показ"), Робототехника, Индустрия моды
                        if (new string[] { EnumsHelper.GetRobotechValue(Robotech.self), EnumsHelper.GetClothesValue(Clothes.self) }.Contains(((CompetitionRequest)req).CompetitionName)
                            //|| (((CompetitionRequest)req).CompetitionName == EnumsHelper.GetClothesValue(Clothes.self) && new string[] { EnumsHelper.GetClothesValue(Clothes.tmcollectpokaz), EnumsHelper.GetClothesValue(Clothes.tmavtorcollect), EnumsHelper.GetClothesValue(Clothes.tmindividpokaz) }.Contains(((CompetitionRequest)req).SubsectionName))
                            )
                        {
                            if (((CompetitionRequest)req).WorkName != "" && ((CompetitionRequest)req).WorkName.ToUpper().Trim() != "НЕТ" && ((CompetitionRequest)req).WorkName.ToUpper().Trim() != "-")                //Название спектакля
                            {
                                sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>" + ((CompetitionRequest)req).WorkName + "</span></td></tr>");
                            }
                        }
                        #endregion
                    }
                    else if (docType == DocumentType.Certificate)
                    {
                        sb.Append("<tr><td style='padding-top: 30px;'><span style='font-size: 24px; font-weight: normal; color: #000000;'>Настоящим подтверждается, что</span></td></tr>");

                        #region Коллектив
                        if (((CompetitionRequest)req).ClubsName != "" && ((CompetitionRequest)req).ClubsName.ToUpper().Trim() != "НЕТ" && ((CompetitionRequest)req).ClubsName.ToUpper().Trim() != "-")                //Коллектив с названием и его состав
                        {
                            sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 30px; font-weight: bold; color: #000000;'>" + ((CompetitionRequest)req).ClubsName + "</span></td></tr>");
                        }
                        else
                        {
                            sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 24px; font-weight: normal; color: #000000;'>Коллектив</span></td></tr>");
                        }
                        #endregion

                        sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 24px; font-weight: normal; color: #000000;'>Принимал(а) участие " + (protocolType == 2 ? "во II" : "в I") + " туре</span></td></tr>");

                        sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 26px; font-weight: bold; color: #000000;'>" + competitionName + "</span></td></tr>");

                        if (showSubsectionName && req.SubsectionName != "" && req.SubsectionName.ToUpper().Trim() != "НЕТ" && req.SubsectionName.ToUpper().Trim() != "-")
                        {
                            sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 24px; font-weight: normal; color: #000000;'>Номинация «<b>" + req.SubsectionName + "</b>»</span></td></tr>");
                        }
                    }

                    #region Состав коллектива

                    if (fiosList.Count > 0)
                        sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: " + (fiosList.Count <= 3 ? "24px" : (fiosList.Count > 30 ? "12px" : "18px")) + "; color: #000000;'>" + list.ToString() + "</span></td></tr>");

                    #endregion

                    if (req.ChiefFios.Count > 0)
                    {
                        sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 18px; font-weight: bold; color: #000000;'>" + req.GetFormChiefPositionsAndFios(", ") + "</span></td></tr>");
                    }
                    else
                    {
                        if (req.ChiefFio != "" && req.ChiefFio.ToUpper().Trim() != "НЕТ" && req.ChiefFio.ToUpper().Trim() != "-")
                        {
                            sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 18px; font-weight: bold; color: #000000;'>Педагог " + FioReduce(req.ChiefFio) + "</span></td></tr>");
                        }
                    }

                    if (req.EducationalOrganization != "" && req.EducationalOrganization.ToUpper().Trim() != "НЕТ" && req.EducationalOrganization.ToUpper().Trim() != "-")
                    {
                        sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 18px; font-weight: normal; color: #000000;'>" + req.EducationalOrganization + "</span></td></tr>");
                    }

                    if (docType == DocumentType.Certificate)
                    {
                        if (req.City != "" && req.City.ToUpper().Trim() != "НЕТ" && req.City.ToUpper().Trim() != "-")
                        {
                            sb.Append("<tr><td style='padding-top: 15px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>" + req.City + "</span></td></tr>");
                        }
                    }

                    sb.Append("</table>");

                    sb.Append("</div>");

                    sb.Append("</body>");

                    sb.Append("</html>");
                }
                else
                {
                    #region Код
                    foreach (var fi in fiosList)
                    {
                        #region Код создания одного документа

                        sb.Append("<html>");

                        sb.Append("<head>");
                        sb.Append("<style>");
                        sb.Append("table { border-collapse: collapse; border-spacing: 0; margin-top: 425px; } ");
                        sb.Append("table td { text-align: center; } ");
                        //sb.Append("span { font-family: 'Arial'; } ");
                        sb.Append("span { font-family: 'Times New Roman'; } ");
                        sb.Append("</style>");
                        sb.Append("</head>");

                        sb.Append("<body>");

                        sb.Append("<div style='background-image: url(" + Constants.PATH_TO_MAIN + "/images/" + bckgrdFileName + "); height:100%;'>");

                        sb.Append("<table align=center width=650>");

                        if (docType == DocumentType.Diplom || docType == DocumentType.Certificate)
                        {
                            if (docType == DocumentType.Diplom)
                            {
                                #region Конкурс
                                sb.Append("<tr><td style='padding-top: 30px;'><span style='font-size: 24px; font-weight: normal; color: #000000;'>" + competitionName + "</span></td></tr>");
                                #endregion

                                #region Номинация
                                if (showSubsectionName && req.SubsectionName != "" && req.SubsectionName.ToUpper().Trim() != "НЕТ" && req.SubsectionName.ToUpper().Trim() != "-")
                                {
                                    sb.Append("<tr><td style='padding-top: 10px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>Номинация: «<b>" + req.SubsectionName + "</b>»</span></td></tr>");
                                }
                                #endregion

                                #region Возрастная категория
                                if (!string.IsNullOrEmpty(ageCategory))
                                {
                                    sb.Append("<tr><td style='padding-top: 10px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>Возрастная категория «" + ageCategory + "»</span></td></tr>");
                                }
                                #endregion

                                sb.Append("<tr><td style='padding-top: 25px;'><span style='font-size: 30px; font-weight: bold; color: #000000; letter-spacing: 5px;'>НАГРАЖДАЕТСЯ</span></td></tr>");

                                #region Результат
                                sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 30px; font-weight: bold; color: #e11f27;'>" + result + "</span></td></tr>");
                                #endregion

                                #region Участник

                                sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 30px; font-weight: bold; color: #000000;'>" + fi.Name + "</span></td></tr>");

                                #endregion

                                #region Коллектив
                                if (req.ClubsName != "" && req.ClubsName.ToUpper().Trim() != "НЕТ" && req.ClubsName.ToUpper().Trim() != "-")                //Коллектив с названием и его состав
                                {
                                    sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>" + req.ClubsName + "</span></td></tr>");
                                }
                                else
                                {
                                    sb.Append("<tr><td style='padding-top: 5px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>&nbsp;</span></td></tr>");
                                }
                                #endregion

                                #region Название работы
                                //только для Театральное искусство ( "Театр моды: Коллективный показ","Театр моды: Авторская коллекция","Театр моды: Индивидуальный показ"), Робототехника, Индустрия моды
                                if (new string[] { EnumsHelper.GetRobotechValue(Robotech.self), EnumsHelper.GetClothesValue(Clothes.self) }.Contains(req.CompetitionName)
                                    //|| (req.CompetitionName == EnumsHelper.GetClothesValue(Clothes.self) && new string[] { EnumsHelper.GetClothesValue(Clothes.tmcollectpokaz), EnumsHelper.GetClothesValue(Clothes.tmavtorcollect), EnumsHelper.GetClothesValue(Clothes.tmindividpokaz) }.Contains(req.SubsectionName))
                                    )
                                {
                                    if (req.WorkName != "" && req.WorkName.ToUpper().Trim() != "НЕТ" && req.WorkName.ToUpper().Trim() != "-")                //Название спектакля
                                    {
                                        sb.Append("<tr><td style='padding-top: 10px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>" + req.WorkName + "</span></td></tr>");
                                    }
                                }
                                #endregion

                            }
                            else if (docType == DocumentType.Certificate)
                            {
                                sb.Append("<tr><td style='padding-top: 30px;'><span style='font-size: 24px; font-weight: normal; color: #000000;'>Настоящим подтверждается, что</span></td></tr>");

                                #region Участник

                                sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 30px; font-weight: bold; color: #000000;'>" + fi.Name + "</span></td></tr>");

                                #endregion

                                sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 24px; font-weight: normal; color: #000000;'>Принимал(а) участие " + (protocolType == 2 ? "во II" : "в I") + " туре</span></td></tr>");
                                sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 26px; font-weight: bold; color: #000000;'>" + competitionName + "</span></td></tr>");

                                if (showSubsectionName && req.SubsectionName != "" && req.SubsectionName.ToUpper().Trim() != "НЕТ" && req.SubsectionName.ToUpper().Trim() != "-")
                                {
                                    sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 24px; font-weight: normal; color: #000000;'>Номинация «<b>" + req.SubsectionName + "</b>»</span></td></tr>");
                                }
                            }

                            if (req.ChiefFios.Count > 0)
                            {
                                sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 18px; font-weight: bold; color: #000000;'>" + req.GetFormChiefPositionsAndFios(", ") + "</span></td></tr>");
                            }
                            else
                            {
                                if (req.ChiefFio != "" && req.ChiefFio.ToUpper().Trim() != "НЕТ" && req.ChiefFio.ToUpper().Trim() != "-")
                                {
                                    sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 18px; font-weight: bold; color: #000000;'>Педагог " + FioReduce(req.ChiefFio) + "</span></td></tr>");
                                }
                            }

                            if (req.EducationalOrganization != "" && req.EducationalOrganization.ToUpper().Trim() != "НЕТ" && req.EducationalOrganization.ToUpper().Trim() != "-")
                            {
                                sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 18px; font-weight: normal; color: #000000;'>" + req.EducationalOrganization + "</span></td></tr>");
                            }

                            if (docType == DocumentType.Certificate)
                            {
                                if (req.City != "" && req.City.ToUpper().Trim() != "НЕТ" && req.City.ToUpper().Trim() != "-")
                                {
                                    sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>" + req.City + "</span></td></tr>");
                                }
                            }
                        }
                        else if (docType == DocumentType.Blagodarnost) {

                            #region Конкурс

                            sb.Append("<tr><td style='padding-top: 60px;'><span style='font-size: 24px; font-weight: normal; color: #000000;'>" + competitionName + "</span></td></tr>");

                            #endregion

                            #region Номинация

                            if (showSubsectionName && req.SubsectionName != "" && req.SubsectionName.ToUpper().Trim() != "НЕТ" && req.SubsectionName.ToUpper().Trim() != "-")
                            {
                                sb.Append("<tr><td style='padding-top: 10px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>Номинация: «<b>" + req.SubsectionName + "</b>»</span></td></tr>");
                            }

                            #endregion

                            #region ФИО учителя

                            var fio = new Ru().Q(fi.Name, NameCaseLib.NCL.Padeg.DATELN);

                            sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 30px; font-weight: bold; color: #000000;'>" + fio + "</span></td></tr>");

                            #endregion

                            #region Должность учителя

                            sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 20px; font-weight: normal; color: #000000;'>" + fi.Position + "</span></td></tr>");

                            #endregion

                            if (req.EducationalOrganization != "" && req.EducationalOrganization.ToUpper().Trim() != "НЕТ" && req.EducationalOrganization.ToUpper().Trim() != "-")
                            {
                                sb.Append("<tr><td style='padding-top: 20px;'><span style='font-size: 18px; font-weight: normal; color: #000000;'>" + req.EducationalOrganization + "</span></td></tr>");
                            }
                            if (thankLetterText.Length > 0)
                            {
                                sb.Append("<tr><td style='padding-top: 30px;'><span style='font-size: 20px; font-weight: normal; color: #000000;font-style:italic;'>" + thankLetterText.ToString() + "</span></td></tr>");
                            }
                        }

                        sb.Append("</table>");

                        sb.Append("</div>");

                        sb.Append("</body>");

                        sb.Append("</html>");
                        #endregion
                    }
                    #endregion
                }

                #endregion 

                #region Применение парсера

                MemoryStream mStream = new MemoryStream();
                if (sb.ToString().Trim() != "")
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                    mStream = new MemoryStream(bytes);
                    p.Parse(mStream);
                }
                else
                {
                    checker = false;

                    if (docType == DocumentType.Diplom)
                    {
                        DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: PDF-файл Диплома по заявке " + reqId + " не сформировался, так как не удалось сформировать его содержимое");
                    }
                    else if (docType == DocumentType.Certificate)
                    {
                        DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: PDF-файл Сертификата по заявке " + reqId + " не сформировался, так как не удалось сформировать его содержимое");
                    }
                    else if (docType == DocumentType.Blagodarnost)
                    {
                        DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: PDF-файл Благодарности по заявке " + reqId + " не сформировался, так как не удалось сформировать его содержимое");
                    }
                }

                #endregion

                doc.Close();
                pdfWriter.Close();
                mStream.Close(); mStream.Dispose();
                stream.Close(); stream.Dispose();

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                checker = false;

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            #region Отправка данных

            if (checker)
            {
                #region Выкидываем файл в выходной поток для скачивания через браузер

                page.Response.Clear();
                page.Response.ContentType = "application/octet-stream";
                page.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                byte[] b = File.ReadAllBytes(pdfFile);       // если нужно передать из памяти, то так - //byte[] b = Encoding.Default.GetBytes(sb.ToString());

                #region Удаляем временный файл

                try
                {
                    #region Код

                    File.Delete(pdfFile);

                    #endregion
                }
                catch (Exception ex)
                {
                    #region Код

                    DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                    #endregion
                }

                #endregion

                page.Response.BinaryWrite(b);
                page.Response.Flush();
                page.Response.Close();
                //page.Response.End();
                _context.ApplicationInstance.CompleteRequest();

                #endregion
            }
            else
            {
                #region Отображение предупреждения

                if (warning != null)
                {
                    warning.ShowWarning("ОШИБКА во время генерации файла. Попробуйте повторить..", page.Master);
                }

                #endregion
            }

            #endregion
        }

        #endregion

        #region GetLinkList

        public Dictionary<int, string> GetLinkList(CompetitionRequest req)
        {
            var links = new Dictionary<int, string>();

            string fileUrlPath = GetUrlPathByReq(req);

            string baseUrl = "https://" + _context.Request.ServerVariables["SERVER_NAME"] + ":" +
                                    _context.Request.ServerVariables["SERVER_PORT"] + "/";
            int counter = 1;
            foreach (string link in req.Links)
            {
                if (link.StartsWith("http"))
                {
                    links.Add(counter, link);
                }
                else if (link != "")
                {
                    links.Add(counter, baseUrl + fileUrlPath + link);
                }
                counter++;
            }
            return links;
        }

        #endregion

        #region GetFilesList

        public Dictionary<int, string> GetFilesList(CompetitionRequest req)
        {
            var links = new Dictionary<int, string>();

            string pathToFolder = GetFolderNameByReq(req);

            int counter = 1;
            foreach (string link in req.Links)
            {
                if (link.StartsWith("http"))
                {
                    //links.Add(counter, link);
                }
                else if (link != "")
                {
                    var fi = new FileInfo(pathToFolder + link);
                    if (new FileInfo(fi.FullName).Exists) {
                        links.Add(counter, fi.FullName);
                    }
                    
                }
                counter++;
            }
            return links;
        }

        #endregion

        #region GetProtocolFile

        public Dictionary<int, string> GetProtocolFile(CompetitionRequest req)
        {
            var links = new Dictionary<int, string>();

            if (!string.IsNullOrEmpty(req.ProtocolFile))     //если к заявке загружен файл протокола
            {
                var fi = new FileInfo(_pathToProtocolFolder + req.ProtocolFile);
                if (fi.Exists)
                {
                    links.Add(1, fi.FullName);
                }
                
            }
            return links;
        }

        #endregion

        #region GetCompetitionSubsectionByWrites

        /// <summary>Получить название конкурса и номинации по коду правам в консоли</summary>
        /// <param name="fromEnum"></param>
        public KeyValuePair<string, string[]> GetCompetitionSubsectionByWrites(Writes fromEnum, bool isArchive = false)
        {
            KeyValuePair<string, string[]> result = new KeyValuePair<string, string[]>();

            switch (fromEnum)
            {
                //case Writes.admin: result = new KeyValuePair<string, string[]>(); break;
                case Writes.editorPhoto: {
                        var subnames = EnumsHelper.GetPhotoValue(Photo.photo);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetPhotoCode(Photo.self), new string[] { subnames }); 
                        break;
                    }
                case Writes.editorPhotoIzo: {
                        var subnames = EnumsHelper.GetPhotoValue(Photo.izo);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetPhotoCode(Photo.self), new string[] { subnames }); 
                        break;
                    } 
                case Writes.editorPhotoCompGraphic: {
                        var subnamesTemp = CompetitonWorkCommon.subnames_editorPhotoCompGraphic().ToList();
                        if (isArchive)
                        {
                            subnamesTemp.Add("");
                        }
                        var subnames = subnamesTemp.ToArray();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetPhotoCode(Photo.self), subnames); 
                        break;
                    } 
                case Writes.editorDPI1: {
                        var subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorDPI1();
                        else
                            subnames = CompetitonWorkCommon.subnames_editorDPI1_arch();

                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetPhotoCode(Photo.self), subnames); 
                        break;
                    } 
                //case Writes.editorDPI2: result = new KeyValuePair<string, string[]>(EnumsHelper.GetPhotoCode(Photo.self), CompetitonWorkCommon.subnames_editorDPI2()); break;
                case Writes.editorLiterary: {
                        var subnames = CompetitonWorkCommon.subnames_editorLiterary();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetLiteraryCode(Literary.self), subnames); 
                        break;
                    } 
                case Writes.editorTheatre: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorTheatre();
                        else
                            subnames = new string[] { "Театральное искусство: %" };
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetTheatreCode(Theatre.self), subnames);
                        break;
                    } 
                case Writes.editorTheatreHudSlovo: {
                        string subnames = EnumsHelper.GetTheatreValue(Theatre.hudSlovo);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetTheatreCode(Theatre.self), new string[] { subnames }); 
                        break;
                    } 
                case Writes.editorTheatreHoreo: {
                        var subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorTheatreHoreo();
                        else
                            subnames = new string[] { "Хореография: %" };
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetTheatreCode(Theatre.self), subnames); 
                        break;
                    } 
                case Writes.editorTheatreVokal: {
                        var subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorTheatreVokal();
                        else
                            subnames = new string[] { "Вокал: %" };
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetTheatreCode(Theatre.self), subnames); 
                        break;
                    } 
                case Writes.editorTheatreInstrumZanr: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorTheatreInstrumZanr();
                        else
                            subnames = new string[] { "Инструментальный жанр: %" };
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetTheatreCode(Theatre.self), subnames); 
                        break;
                    }
                    
                case Writes.editorTheatreModa: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorTheatreModa();
                        else
                            subnames = new string[] { "Театр моды: %" };
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetClothesCode(Clothes.self), subnames); 
                        break;
                    } 
                case Writes.editorKultura: {
                        string[] subnames = CompetitonWorkCommon.subnames_editorKultura();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetKulturaCode(Kultura.self), subnames); 
                        break;
                    } 
                case Writes.editorSport: {
                        var subnames = EnumsHelper.GetSportValue(Sport.prostEdinoborstva);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetSportCode(Sport.self), new string[] { subnames });
                        break;
                    } 
                case Writes.editorThekvo: {
                        var subnames = EnumsHelper.GetSportValue(Sport.thekvo);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetSportCode(Sport.self), new string[] { subnames });
                        break;
                    } 
                case Writes.editorBoks: {
                        var subnames = EnumsHelper.GetSportValue(Sport.boks);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetSportCode(Sport.self), new string[] { subnames });
                        break;
                    } 
                case Writes.editorKungfu: {
                        var subnames = EnumsHelper.GetSportValue(Sport.kungfu);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetSportCode(Sport.self), new string[] { subnames });
                        break;
                    } 
                case Writes.editorFootball: {
                        var subnames = EnumsHelper.GetSportValue(Sport.football);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetSportCode(Sport.self), new string[] { subnames });
                        break;
                    } 
                case Writes.editorBasketball: {
                        var subnames = EnumsHelper.GetSportValue(Sport.basketball);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetSportCode(Sport.self), new string[] { subnames });
                        break;
                    }
                case Writes.editorVolleyball: {
                        var subnames = EnumsHelper.GetSportValue(Sport.volleyball);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetSportCode(Sport.self), new string[] { subnames });
                        break;
                    }
                case Writes.editorPaintball: {
                        var subnames = EnumsHelper.GetSportValue(Sport.stendStrelba);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetSportCode(Sport.self), new string[] { subnames });
                        break;
                    } 
                case Writes.editorShahmaty: {
                        var subnames = EnumsHelper.GetSportValue(Sport.shahmaty);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetSportCode(Sport.self), new string[] { subnames });
                        break;
                    }
                case Writes.editorShashky: {
                        var subnames = EnumsHelper.GetSportValue(Sport.shashki);
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetSportCode(Sport.self), new string[] { subnames });
                        break;
                    }
                case Writes.editorToponim: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorToponim();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetToponimCode(Toponim.self), subnames); 
                        break;
                    } 
                case Writes.editorRobotech: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorRobotech();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetRobotechCode(Robotech.self), subnames); 
                        break;
                    } 
                case Writes.editorVmesteSila: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorVmesteSila();
                        else
                            subnames = CompetitonWorkCommon.subnames_editorVmesteSila();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetVmesteSilaCode(VmesteSila.self), subnames); 
                        break;
                    } 
                case Writes.editorVmesteSilaMakeUp: {
                        var subnames = CompetitonWorkCommon.subnames_editorVmesteSilaMakeUp();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetVmesteSilaCode(VmesteSila.self), subnames); 
                        break;
                    } 
                case Writes.editorVmesteSilaShair: {
                        var subnames = CompetitonWorkCommon.subnames_editorVmesteSilaShair();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetVmesteSilaCode(VmesteSila.self), subnames); 
                        break;
                    } 
                case Writes.editorClothes: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorClothes();
                        else
                            subnames = CompetitonWorkCommon.subnames_editorClothes();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetClothesCode(Clothes.self), subnames); 
                        break;
                    } 
                case Writes.editorMultimedia: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorMultimedia();
                        else
                            subnames = CompetitonWorkCommon.subnames_editorMultimedia();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetMultimediaCode(Multimedia.self), subnames); 
                        break;
                    } 
                case Writes.editorKorablik: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorKorablik();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetKorablikCode(Korablik.self), subnames); 
                        break;
                    }
                case Writes.editorKorablikVokal:
                    {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorKorablikVokal();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetKorablikCode(Korablik.self), subnames);
                        break;
                    }
                case Writes.editorKorablikHoreo: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorKorablikHoreo();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetKorablikCode(Korablik.self), subnames);
                        break;
                    } 
                case Writes.editorCrimroute: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorCrimroute();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetCrimrouteCode(Crimroute.self), subnames); 
                        break;
                    } 
                case Writes.editorMathbattle: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorMathbattle();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetMathbattleCode(Mathbattle.self), subnames); 
                        break;
                    } 
                case Writes.editorKosmos: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorKosmos();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetKosmosCode(Kosmos.self), subnames); 
                        break;
                    } 
                case Writes.editorScience: {
                        string[] subnames = new string[] { "" };
                        if (!isArchive)
                            subnames = CompetitonWorkCommon.subnames_editorScience();
                        result = new KeyValuePair<string, string[]>(EnumsHelper.GetScienceCode(Science.self), subnames); 
                        break;
                    } 
                default: break;
            }

            return result;
        }

        #endregion

        #region GetUrlPathByReq

        public string GetUrlPathByReq(CompetitionRequest req)
        {
            string fileUrlPath = "";
            if (req.CompetitionName == EnumsHelper.GetPhotoCode(Photo.self))
            {
                fileUrlPath = _imgUrlPathFoto.Replace("~/", "");
            }
            else if (req.CompetitionName == EnumsHelper.GetLiteraryCode(Literary.self))
            {
                fileUrlPath = _imgUrlPathLiterary.Replace("~/", "");
            }
            else if (req.CompetitionName == EnumsHelper.GetTheatreCode(Theatre.self))
            {
                fileUrlPath = _imgUrlPathTheatre.Replace("~/", "");     //на самом деле для этого конкурса эта переменная не применяется, так как там прямые ссылки на ролики в ютубе
            }
            else if (req.CompetitionName == EnumsHelper.GetKulturaCode(Kultura.self))
            {
                fileUrlPath = _imgUrlPathKultura.Replace("~/", "");
            }
            else if (req.CompetitionName == EnumsHelper.GetSportCode(Sport.self))
            {
                fileUrlPath = _imgUrlPathSport.Replace("~/", "");
            }
            else if (req.CompetitionName == EnumsHelper.GetToponimCode(Toponim.self))
            {
                fileUrlPath = _imgUrlPathToponim.Replace("~/", "");
            }
            else if (req.CompetitionName == EnumsHelper.GetClothesCode(Clothes.self))
            {
                fileUrlPath = _imgUrlPathClothes.Replace("~/", "");
            }
            else if (req.CompetitionName == EnumsHelper.GetMultimediaCode(Multimedia.self))
            {
                fileUrlPath = _imgUrlPathMultimedia.Replace("~/", "");
            }
            else if (req.CompetitionName == EnumsHelper.GetVmesteSilaCode(VmesteSila.self))
            {
                fileUrlPath = _imgUrlPathVmesteSila.Replace("~/", "");
            }
            else if (req.CompetitionName == EnumsHelper.GetKorablikCode(Korablik.self))
            {
                fileUrlPath = _imgUrlPathKorablik.Replace("~/", "");
            }
            else if (req.CompetitionName == EnumsHelper.GetCrimrouteCode(Crimroute.self))
            {
                fileUrlPath = _imgUrlPathCrimroute.Replace("~/", "");
            }
            else if (req.CompetitionName == EnumsHelper.GetKosmosCode(Kosmos.self))
            {
                fileUrlPath = _imgUrlPathKosmos.Replace("~/", "");
            }
            else if (req.CompetitionName == EnumsHelper.GetMathbattleCode(Mathbattle.self))
            {
                fileUrlPath = _imgUrlPathMathbattle.Replace("~/", "");
            }
            else if (req.CompetitionName == EnumsHelper.GetScienceCode(Science.self))
            {
                fileUrlPath = _imgUrlPathScience.Replace("~/", "");
            }
            return fileUrlPath;
        }

        #endregion

        #region GetFolderNameByReq 

        public string GetFolderNameByReq(CompetitionRequest req)
        {
            string pathToFolder = "";
            if (req.CompetitionName == EnumsHelper.GetPhotoCode(Photo.self))
            {
                pathToFolder = _pathToFotoFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetLiteraryCode(Literary.self))
            {
                pathToFolder = _pathToLiteraryFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetTheatreCode(Theatre.self))
            {
                pathToFolder = _pathToTheatreFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetKulturaCode(Kultura.self))
            {
                pathToFolder = _pathToKulturaFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetSportCode(Sport.self))
            {
                pathToFolder = _pathToSportFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetToponimCode(Toponim.self))
            {
                pathToFolder = _pathToToponimFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetClothesCode(Clothes.self))
            {
                pathToFolder = _pathToClothesFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetMultimediaCode(Multimedia.self))
            {
                pathToFolder = _pathToMultimediaFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetVmesteSilaCode(VmesteSila.self))
            {
                pathToFolder = _pathToVmesteSilaFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetKorablikCode(Korablik.self))
            {
                pathToFolder = _pathToKorablikFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetCrimrouteCode(Crimroute.self))
            {
                pathToFolder = _pathToCrimrouteFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetKosmosCode(Kosmos.self))
            {
                pathToFolder = _pathToKosmosFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetMathbattleCode(Mathbattle.self))
            {
                pathToFolder = _pathToMathbattleFolder;
            }
            else if (req.CompetitionName == EnumsHelper.GetScienceCode(Science.self))
            {
                pathToFolder = _pathToScienceFolder;
            }
            return pathToFolder;
        }

        #endregion

        #region Метод string ReplaceFieldValues(...)

        /// <summary>Метод заменяет значение в составных полях значений, например Fios, Agies, Weights</summary>
        /// <param name="oldFieldValues">старое составное значение поля</param>
        /// <param name="newFieldValue">новое значение, заменяемое в поле</param>
        /// <param name="position">индекс позии для нового значения</param>
        /// <returns>возвращает составное новое значение</returns>
        public string ReplaceFieldValues(string oldFieldValues, string newFieldValue, int position)
        {
            string result = "";

            string[] tmp = oldFieldValues.Split(new[] { '|' });
            if (position <= tmp.Length - 1)     //если штатная ситуация и нужно просто заменить значение..
            {
                tmp[position] = newFieldValue;
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (!string.IsNullOrEmpty(tmp[i])) result += tmp[i] + "|";
                }
            }
            else //если по каким-то причинам нужно добавить значение вне диапозона массива (этот вариант произошёл по причине недобавления в первых заявках весов спортсменов, возможны и другие подобные варианты)..
            {
                List<string> tmpList = new List<string>();
                foreach (string item in tmp)
                {
                    tmpList.Add(item);
                }
                int max = position - tmp.Length + 1;
                for (int i = 1; i <= max; i++)
                {
                    if (i < max) tmpList.Add(""); else tmpList.Add(newFieldValue);
                }

                for (int i = 0; i < tmpList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(tmpList[i])) result += tmpList[i] + "|";
                }
            }

            return result.Length > 0 ? result.Substring(0, result.Length - 1) : result;
        }

        #endregion
        #region Метод string DeleteFieldValues(...)

        /// <summary>Метод удаляет значение в составных полях значений, например Fios, Agies, Weights.</summary>
        /// <param name="oldFieldValues">старое составное значение поля</param>
        /// <param name="position">индекс позии удаляемого значения (если он выходит за границы значений поля, то ничего не удаляется)</param>
        /// <returns>возвращает новое составное значение</returns>
        public string DeleteFieldValues(string oldFieldValues, int position)
        {
            string result = "";

            string[] tmp = oldFieldValues.Split(new[] { '|' });

            if (position <= tmp.Length - 1)     //если штатная ситуация и нужно просто исключить значение..
            {
                List<string> tmpList = tmp.ToList();
                tmpList.RemoveAt(position);
                for (int i = 0; i < tmpList.Count; i++)
                {
                    if (i == 0) result = tmpList[i]; else result += "|" + tmpList[i];
                }
            }
            else                                //если по каким-то причинам нужно добавить значение вне диапозона массива (этот вариант произошёл по причине недобавления в первых заявках весов спортсменов, возможны и другие подобные варианты), то ничего не удаляем
            {
                result = new string(oldFieldValues.ToCharArray());
            }

            return result;
        }

        #endregion
    }

    #endregion

    #region Класс CompetitionsWork_Arch

    /// <summary>Класс формирования архивных данных по конкурсам. Обслуживает класс CompetitionsForm</summary>
    public class CompetitionsWork_Arch
    {
        #region Поля

        private bool _checkNeedFolders = true;
        private HttpContext _context;
        private string _pathToDb;
        private string _tableName;
        private string _pathToMainFolder;
        private string _pathToFotoFolder;
        private string _pathToLiteraryFolder;
        private string _pathToTheatreFolder;
        private string _pathToSportFolder;
        private string _pathToKulturaFolder;
        private string _pathToToponimFolder;
        private string _pathToProtocolFolder;
        public static bool _isArchivingFiles = false;   //показывает, производится ли асинхронный перенос в архив файлов заявок

        #endregion

        #region Конструктор класса

        /// <summary>Конструктор класса. Добавляет в архивную БД таблицу конкурсов, если её ещё не существует.
        /// Так же инициализирует поля.</summary>
        public CompetitionsWork_Arch()
        {
            _context = HttpContext.Current;
            _pathToDb = _context.Server.MapPath("~") + @"files\sqlitedb\konkurses_arch.db";
            _tableName = "competitions_arch";

            _pathToMainFolder = _context.Server.MapPath("~") + @"files\competitionfiles\";
            _pathToFotoFolder = _pathToMainFolder + @"foto\";
            _pathToLiteraryFolder = _pathToMainFolder + @"literary\";
            _pathToTheatreFolder = _pathToMainFolder + @"theatre\";
            _pathToSportFolder = _pathToMainFolder + @"sport\";
            _pathToKulturaFolder = _pathToMainFolder + @"kultura\";
            _pathToToponimFolder = _pathToMainFolder + @"toponim\";
            _pathToProtocolFolder = _context.Server.MapPath("~") + @"files\competitionfiles\protocols\";    //файлы протоколов не перемещаются в архив, а удаляются из исходной папки

            try
            {
                Directory.CreateDirectory(_pathToFotoFolder);
                Directory.CreateDirectory(_pathToLiteraryFolder);
                Directory.CreateDirectory(_pathToTheatreFolder);
                Directory.CreateDirectory(_pathToSportFolder);
                Directory.CreateDirectory(_pathToKulturaFolder);
                Directory.CreateDirectory(_pathToToponimFolder);
                Directory.CreateDirectory(_pathToProtocolFolder);
            }
            catch (Exception ex)
            {
                _checkNeedFolders = false;
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }

            if (_checkNeedFolders)
            {
                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                string sqlString = "CREATE TABLE IF NOT EXISTS " + _tableName + " (" +
                    " '_id' INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    " 'OldId' INTEGER NOT NULL DEFAULT (0), " +
                    " 'Fio' TEXT NOT NULL DEFAULT ('')," +
                    " 'Class_' TEXT NOT NULL DEFAULT ('')," +
                    " 'Age' TEXT NOT NULL DEFAULT ('')," +
                    " 'WorkName' TEXT NOT NULL DEFAULT ('')," +
                    " 'WorkComment' TEXT NOT NULL DEFAULT ('')," +
                    " 'EducationalOrganization' TEXT NOT NULL DEFAULT ('')," +
                    " 'Email' TEXT NOT NULL DEFAULT ('')," +
                    " 'Telephone' TEXT NOT NULL DEFAULT ('')," +
                    " 'Addr' TEXT NOT NULL DEFAULT ('')," +
                    " 'Addr1' TEXT NOT NULL DEFAULT ('')," +
                    " 'PostIndex' INTEGER NOT NULL DEFAULT (0)," +
                    " 'Region' TEXT NOT NULL DEFAULT ('')," +
                    " 'City' TEXT NOT NULL DEFAULT ('')," +
                    " 'Street' TEXT NOT NULL DEFAULT ('')," +
                    " 'Home' TEXT NOT NULL DEFAULT ('')," +
                    " 'Room' TEXT NOT NULL DEFAULT ('')," +
                    " 'ChiefFio' TEXT NOT NULL DEFAULT ('')," +
                    " 'ChiefPosition' TEXT NOT NULL DEFAULT ('')," +
                    " 'ChiefEmail' TEXT NOT NULL DEFAULT ('')," +
                    " 'ChiefTelephone' TEXT NOT NULL DEFAULT ('')," +
                    " 'SubsectionName' TEXT NOT NULL DEFAULT ('')," +
                    " 'Fios' TEXT NOT NULL DEFAULT ('')," +
                    " 'Agies' TEXT NOT NULL DEFAULT ('')," +
                    " 'Links' TEXT NOT NULL DEFAULT ('')," +
                    " 'CompetitionName' TEXT NOT NULL DEFAULT ('')," +
                    " 'DateReg' INTEGER NOT NULL DEFAULT (0)," +
                    " 'Likes' INTEGER NOT NULL DEFAULT (0)," +
                    " 'Nolikes' INTEGER NOT NULL DEFAULT (0)," +
                    " 'Approved' INTEGER NOT NULL DEFAULT (0)," +
                    " 'PdnProcessing' INTEGER NOT NULL DEFAULT (0)," +
                    " 'PublicAgreement' INTEGER NOT NULL DEFAULT (0)," +
                    " 'ProcMedicine' INTEGER NOT NULL DEFAULT (0)," +
                    " 'SummaryLikes' INTEGER NOT NULL DEFAULT (0)," +
                    " 'ClubsName' TEXT NOT NULL DEFAULT ('')," +
                    " 'Weight' INTEGER NOT NULL DEFAULT (0)," +
                    " 'Weights' TEXT NOT NULL DEFAULT ('')," +
                    " 'AgeСategories' TEXT NOT NULL DEFAULT ('')," +
                    " 'Kvalifications' TEXT NOT NULL DEFAULT ('')," +
                    " 'Programms' TEXT NOT NULL DEFAULT ('')," +
                    " 'Result' TEXT NOT NULL DEFAULT ('')," +
                    " 'Results' TEXT NOT NULL DEFAULT ('')," +
                    " 'ProtocolFile' TEXT NOT NULL DEFAULT ('')," +
                    " 'ProtocolPartyCount' INTEGER NOT NULL DEFAULT (0)," +
                    " 'TechnicalInfo' TEXT NOT NULL DEFAULT ('')," +
                    " 'Timing_min' INTEGER NOT NULL DEFAULT (0)," +
                    " 'Timing_sec' INTEGER NOT NULL DEFAULT (0)," +
                    " 'ChiefFios' TEXT NOT NULL DEFAULT ('')," +
                    " 'ChiefPositions' TEXT NOT NULL DEFAULT ('')," +
                    " 'AthorsFios' TEXT NOT NULL DEFAULT ('')," +
                    " 'AgeСategory' TEXT NOT NULL DEFAULT ('')," +
                    " 'PartyCount' INTEGER NOT NULL DEFAULT (0)," +
                    " 'CheckedAdmin' INTEGER NOT NULL DEFAULT (0)," +
                    " 'Points' INTEGER NOT NULL DEFAULT (0), " +
                    " 'Schools' TEXT NOT NULL DEFAULT (''), " +
                    " 'ClassRooms' TEXT NOT NULL DEFAULT (''), " +
                    " 'ProtocolFileDoc' TEXT NOT NULL DEFAULT (''), " +
                    " 'Fios_1' TEXT NOT NULL DEFAULT ('')," +
                    " 'Agies_1' TEXT NOT NULL DEFAULT ('')," +
                    " 'Schools_1' TEXT NOT NULL DEFAULT (''), " +
                    " 'ClassRooms_1' TEXT NOT NULL DEFAULT (''), " +
                    " 'Weights_1' TEXT NOT NULL DEFAULT (''), " +
                    " 'IsApply' INTEGER NOT NULL DEFAULT (0)," +
                    " 'DateApply' INTEGER NOT NULL DEFAULT (0)," +
                    " 'Division' TEXT NOT NULL DEFAULT ('')" +
                    ")";
                sqlite.ExecuteNonQuery(sqlString);
                sqlite.ConnectionClose();
            }
        }

        #endregion

        #region Метод int InsertOneRequest(CompetitionRequest_Arch req)

        /// <summary>Метод добавляет в БД одну запись с данными из переданного объекта</summary>
        /// <param name="req">объект CompetitionRequest_Arch с данными по заявке участника конкурса</param>
        /// <returns>Метод возвращает номер принятой заявки или -1 в случае какой-либо ошибки</returns>
        public int InsertOneRequest(CompetitionRequest_Arch req)
        {
            if (!_checkNeedFolders) return -1;
            int result = 0;

            try
            {
                #region Код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "INSERT INTO " + _tableName + " (" +
                                                           "OldId, " +
                                                           "Fio, " +
                                                           "Class_, " +
                                                           "Age, " +
                                                           "WorkName, " +
                                                           "WorkComment, " +
                                                           "EducationalOrganization, " +
                                                           "EducationalOrganizationShort, " +
                                                           "Email, " +
                                                           "Telephone, " +
                                                           "Addr, " +
                                                           "Addr1, " +
                                                           "District, " +
                                                           "Region, " +
                                                           "Area, " +
                                                           "City, " +
                                                           "ChiefFio, " +
                                                           "ChiefPosition, " +
                                                           "ChiefEmail, " +
                                                           "ChiefTelephone, " +
                                                           "SubsectionName, " +
                                                           "Fios, " +
                                                           "Agies, " +
                                                           "Links, " +
                                                           "CompetitionName, " +
                                                           "DateReg, " +
                                                           "Likes, " +
                                                           "Nolikes, " +
                                                           "Approved, " +
                                                           "PdnProcessing, " +
                                                           "PublicAgreement, " +
                                                           "ProcMedicine, " +
                                                           "SummaryLikes, " +
                                                           "ClubsName, " +
                                                           "Weight, " +
                                                           "Weights, " +
                                                           "AgeСategories, " +
                                                           "Kvalifications, " +
                                                           "Programms, " +
                                                           "Result, " +
                                                           "Results, " +
                                                           "ProtocolFile, " +
                                                           "ProtocolPartyCount, " +
                                                           "TechnicalInfo, " +
                                                           "Timing_min, " +
                                                           "Timing_sec, " +
                                                           "ChiefFios, " +
                                                           "ChiefPositions, " +
                                                           "AthorsFios, " +
                                                           "AgeСategory, " +
                                                           "PartyCount," +
                                                           "CheckedAdmin," +
                                                           "Points," +
                                                           "Schools," +
                                                           "ClassRooms," +
                                                           "ProtocolFileDoc," +
                                                           "Fios_1," +
                                                           "Agies_1," +
                                                           "Schools_1," +
                                                           "ClassRooms_1," +
                                                           "Weights_1," +
                                                           "IsApply," +
                                                           "DateApply," +
                                                           "Division" +
                                                           ") " +
                                                "VALUES (" +
                                                           "@OldId, " +
                                                           "@Fio, " +
                                                           "@Class_, " +
                                                           "@Age, " +
                                                           "@WorkName, " +
                                                           "@WorkComment, " +
                                                           "@EducationalOrganization, " +
                                                           "@EducationalOrganizationShort, " +
                                                           "@Email, " +
                                                           "@Telephone, " +
                                                           "@Addr, " +
                                                           "@Addr1, " +
                                                           "@District, " +
                                                           "@Region, " +
                                                           "@Area, " +
                                                           "@City, " +
                                                           "@ChiefFio, " +
                                                           "@ChiefPosition, " +
                                                           "@ChiefEmail, " +
                                                           "@ChiefTelephone, " +
                                                           "@SubsectionName, " +
                                                           "@Fios, " +
                                                           "@Agies, " +
                                                           "@Links, " +
                                                           "@CompetitionName, " +
                                                           "@DateReg, " +
                                                           "@Likes, " +
                                                           "@Nolikes, " +
                                                           "@Approved, " +
                                                           "@PdnProcessing, " +
                                                           "@PublicAgreement, " +
                                                           "@ProcMedicine, " +
                                                           "@SummaryLikes, " +
                                                           "@ClubsName, " +
                                                           "@Weight, " +
                                                           "@Weights, " +
                                                           "@AgeСategories, " +
                                                           "@Kvalifications, " +
                                                           "@Programms, " +
                                                           "@Result, " +
                                                           "@Results, " +
                                                           "@ProtocolFile, " +
                                                           "@ProtocolPartyCount, " +
                                                           "@TechnicalInfo, " +
                                                           "@Timing_min, " +
                                                           "@Timing_sec, " +
                                                           "@ChiefFios, " +
                                                           "@ChiefPositions, " +
                                                           "@AthorsFios, " +
                                                           "@AgeСategory, " +
                                                           "@PartyCount," +
                                                           "@CheckedAdmin, " +
                                                           "@Points," +
                                                           "@Schools," +
                                                           "@ClassRooms," +
                                                           "@ProtocolFileDoc," +
                                                           "@Fios_1," +
                                                           "@Agies_1," +
                                                           "@Schools_1," +
                                                           "@ClassRooms_1," +
                                                           "@Weights_1," +
                                                           "@IsApply," +
                                                           "@DateApply," +
                                                           "@Division" +
                                                        ")";

                cmd.Parameters.Add(new SQLiteParameter("@OldId", req.OldId));
                cmd.Parameters.Add(new SQLiteParameter("@Fio", req.Fio));
                cmd.Parameters.Add(new SQLiteParameter("@Class_", req.Class_));
                cmd.Parameters.Add(new SQLiteParameter("@Age", req.Age));
                cmd.Parameters.Add(new SQLiteParameter("@WorkName", req.WorkName));
                cmd.Parameters.Add(new SQLiteParameter("@WorkComment", req.WorkComment));
                cmd.Parameters.Add(new SQLiteParameter("@EducationalOrganization", req.EducationalOrganization));
                cmd.Parameters.Add(new SQLiteParameter("@EducationalOrganizationShort", req.EducationalOrganizationShort));
                cmd.Parameters.Add(new SQLiteParameter("@Email", req.Email));
                cmd.Parameters.Add(new SQLiteParameter("@Telephone", req.Telephone));
                cmd.Parameters.Add(new SQLiteParameter("@Addr", req.Addr));
                cmd.Parameters.Add(new SQLiteParameter("@Addr1", req.Addr1));
                cmd.Parameters.Add(new SQLiteParameter("@District", req.District));
                cmd.Parameters.Add(new SQLiteParameter("@Region", req.Region));
                cmd.Parameters.Add(new SQLiteParameter("@Area", req.Area));
                cmd.Parameters.Add(new SQLiteParameter("@City", req.City));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefFio", req.ChiefFio));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefPosition", req.ChiefPosition));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefEmail", req.ChiefEmail));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefTelephone", req.ChiefTelephone));
                cmd.Parameters.Add(new SQLiteParameter("@SubsectionName", req.SubsectionName));
                cmd.Parameters.Add(new SQLiteParameter("@Fios", req.Fios));
                cmd.Parameters.Add(new SQLiteParameter("@Agies", req.Agies));
                cmd.Parameters.Add(new SQLiteParameter("@ProcMedicine", (req.ProcMedicine) ? 1 : 0));
                StringBuilder sb = new StringBuilder();
                int counter = 0;
                foreach (string str in req.Links)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("^" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@Links", sb.ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@CompetitionName", req.CompetitionName));
                cmd.Parameters.Add(new SQLiteParameter("@DateReg", req.DateReg));
                cmd.Parameters.Add(new SQLiteParameter("@Likes", req.Likes));
                cmd.Parameters.Add(new SQLiteParameter("@Nolikes", req.Nolikes));
                cmd.Parameters.Add(new SQLiteParameter("@Approved", (req.Approved) ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@PdnProcessing", (req.PdnProcessing) ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@PublicAgreement", (req.PublicAgreement) ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@SummaryLikes", req.SummaryLikes));
                cmd.Parameters.Add(new SQLiteParameter("@ClubsName", req.ClubsName));
                cmd.Parameters.Add(new SQLiteParameter("@Weight", req.Weight));
                cmd.Parameters.Add(new SQLiteParameter("@Weights", req.Weights));
                cmd.Parameters.Add(new SQLiteParameter("@AgeСategories", req.AgeСategories));
                cmd.Parameters.Add(new SQLiteParameter("@Kvalifications", req.Kvalifications));
                cmd.Parameters.Add(new SQLiteParameter("@Programms", req.Programms));
                cmd.Parameters.Add(new SQLiteParameter("@Result", req.Result));
                cmd.Parameters.Add(new SQLiteParameter("@Results", req.Results));
                cmd.Parameters.Add(new SQLiteParameter("@ProtocolFile", req.ProtocolFile));
                cmd.Parameters.Add(new SQLiteParameter("@ProtocolPartyCount", req.ProtocolPartyCount));
                cmd.Parameters.Add(new SQLiteParameter("@TechnicalInfo", req.TechnicalInfo));
                cmd.Parameters.Add(new SQLiteParameter("@Timing_min", req.Timing_min));
                cmd.Parameters.Add(new SQLiteParameter("@Timing_sec", req.Timing_sec));

                sb = new StringBuilder();
                counter = 0;
                foreach (string str in req.ChiefFios)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@ChiefFios", sb.ToString()));
                sb = new StringBuilder();
                counter = 0;
                foreach (string str in req.ChiefPositions)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@ChiefPositions", sb.ToString()));
                sb = new StringBuilder();
                counter = 0;
                foreach (string str in req.AthorsFios)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@AthorsFios", sb.ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@AgeСategory", req.AgeСategory));
                cmd.Parameters.Add(new SQLiteParameter("@PartyCount", req.PartyCount));
                cmd.Parameters.Add(new SQLiteParameter("@CheckedAdmin", req.CheckedAdmin));
                cmd.Parameters.Add(new SQLiteParameter("@Points", req.Points));

                cmd.Parameters.Add(new SQLiteParameter("@Schools", (string.IsNullOrEmpty(req.Schools) ? "" : req.Schools)));
                cmd.Parameters.Add(new SQLiteParameter("@ClassRooms", (string.IsNullOrEmpty(req.ClassRooms) ? "" : req.ClassRooms)));
                cmd.Parameters.Add(new SQLiteParameter("@ProtocolFileDoc", (string.IsNullOrEmpty(req.ProtocolFileDoc) ? "" : req.ProtocolFileDoc)));
                cmd.Parameters.Add(new SQLiteParameter("@Fios_1", (string.IsNullOrEmpty(req.Fios_1) ? "" : req.Fios_1)));
                cmd.Parameters.Add(new SQLiteParameter("@Agies_1", (string.IsNullOrEmpty(req.Agies_1) ? "" : req.Agies_1)));
                cmd.Parameters.Add(new SQLiteParameter("@Schools_1", (string.IsNullOrEmpty(req.Schools_1) ? "" : req.Schools_1)));
                cmd.Parameters.Add(new SQLiteParameter("@ClassRooms_1", (string.IsNullOrEmpty(req.ClassRooms_1) ? "" : req.ClassRooms_1)));
                cmd.Parameters.Add(new SQLiteParameter("@Weights_1", (string.IsNullOrEmpty(req.Weights_1) ? "" : req.Weights_1)));
                cmd.Parameters.Add(new SQLiteParameter("@IsApply", req.IsApply));
                cmd.Parameters.Add(new SQLiteParameter("@DateApply", req.DateApply));
                cmd.Parameters.Add(new SQLiteParameter("@Division", (string.IsNullOrEmpty(req.Division) ? "" : req.Division)));

                if (sqlite.ExecuteNonQueryParams(cmd) == -1)
                {
                    result = -1;
                }
                else
                {
                    sqlite.ConnectionClose();
                }
                cmd.Dispose();

                if (result != -1)
                {
                    // определим номер последней добавленной строки, он и будет номером добавленной заявки
                    sqlite = new SqliteHelper(_pathToDb);
                    cmd = new SQLiteCommand();
                    cmd.CommandText = "SELECT MAX(_id) FROM " + _tableName;
                    result = sqlite.ExecuteScalarParams(cmd);
                    cmd.Dispose();
                    if (sqlite != null)
                    {
                        sqlite.ConnectionClose();
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                result = -1;
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }

            return result;
        }

        #endregion
        #region Метод int InsertRequests(CompetitionRequest_Arch req)

        /// <summary>Метод добавляет в БД список заявок</summary>
        /// <param name="reqs">список объектов CompetitionRequest_Arch с данными по заявке участника конкурса</param>
        /// <returns>Метод возвращает true в случае успешного добавления списка или false - в случае какой-либо ошибки</returns>
        /*public bool InsertRequests(List<CompetitionRequest_Arch> reqs)
        {
            bool result = true;
            if (!_checkNeedFolders) return false;

            try
            {
                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();

                int index = 0;

                cmd.CommandText = "INSERT INTO " + _tableName + " (" +
                                                               "OldId, " +
                                                               "Fio, " +
                                                               "Class_, " +
                                                               "Age, " +
                                                               "WorkName, " +
                                                               "WorkComment, " +
                                                               "EducationalOrganization, " +
                                                               "Email, " +
                                                               "Telephone, " +
                                                               "Addr, " +
                                                               "Addr1, " +
                                                               "PostIndex, " +
                                                               "Region, " +
                                                               "City, " +
                                                               "Street, " +
                                                               "Home, " +
                                                               "Room, " +
                                                               "ChiefFio, " +
                                                               "ChiefPosition, " +
                                                               "ChiefEmail, " +
                                                               "ChiefTelephone, " +
                                                               "SubsectionName, " +
                                                               "Fios, " +
                                                               "Agies, " +
                                                               "Links, " +
                                                               "CompetitionName, " +
                                                               "DateReg, " +
                                                               "Likes, " +
                                                               "Nolikes, " +
                                                               "Approved, " +
                                                               "PdnProcessing, " +
                                                               "PublicAgreement, " +
                                                               "ProcMedicine, " +
                                                               "SummaryLikes, " +
                                                               "ClubsName, " +
                                                               "Weight, " +
                                                               "Weights, " +
                                                               "Result" +
                                                               ") " +
                                                    "VALUES ";

                foreach (CompetitionRequest_Arch req in reqs)
                {
                    #region Код

                    cmd.CommandText += "(@OldId" + index + ", " +
                                           "@Fio" + index + ", " +
                                           "@Class_" + index + ", " +
                                           "@Age" + index + ", " +
                                           "@WorkName" + index + ", " +
                                           "@WorkComment" + index + ", " +
                                           "@EducationalOrganization" + index + ", " +
                                           "@Email" + index + ", " +
                                           "@Telephone" + index + ", " +
                                           "@Addr" + index + ", " +
                                           "@Addr1" + index + ", " +
                                           "@PostIndex" + index + ", " +
                                           "@Region" + index + ", " +
                                           "@City" + index + ", " +
                                           "@Street" + index + ", " +
                                           "@Home" + index + ", " +
                                           "@Room" + index + ", " +
                                           "@ChiefFio" + index + ", " +
                                           "@ChiefPosition" + index + ", " +
                                           "@ChiefEmail" + index + ", " +
                                           "@ChiefTelephone" + index + ", " +
                                           "@SubsectionName" + index + ", " +
                                           "@Fios" + index + ", " +
                                           "@Agies" + index + ", " +
                                           "@Links" + index + ", " +
                                           "@CompetitionName" + index + ", " +
                                           "@DateReg" + index + ", " +
                                           "@Likes" + index + ", " +
                                           "@Nolikes" + index + ", " +
                                           "@Approved" + index + ", " +
                                           "@PdnProcessing" + index + ", " +
                                           "@PublicAgreement" + index + ", " +
                                           "@ProcMedicine" + index + ", " +
                                           "@SummaryLikes" + index + ", " +
                                           "@ClubsName" + index + ", " +
                                           "@Weight" + index + ", " +
                                           "@Weights" + index + ", " +
                                           "@Result" + index + ")";

                    if (index == reqs.Count - 1)
                    {
                        cmd.CommandText += ";";
                    }
                    else
                    {
                        cmd.CommandText += ", ";
                    }

                    cmd.Parameters.Add(new SQLiteParameter("@OldId" + index, req.OldId));
                    cmd.Parameters.Add(new SQLiteParameter("@Fio" + index, req.Fio));
                    cmd.Parameters.Add(new SQLiteParameter("@Class_" + index, req.Class_));
                    cmd.Parameters.Add(new SQLiteParameter("@Age" + index, req.Age));
                    cmd.Parameters.Add(new SQLiteParameter("@WorkName" + index, req.WorkName));
                    cmd.Parameters.Add(new SQLiteParameter("@WorkComment" + index, req.WorkComment));
                    cmd.Parameters.Add(new SQLiteParameter("@EducationalOrganization" + index, req.EducationalOrganization));
                    cmd.Parameters.Add(new SQLiteParameter("@Email" + index, req.Email));
                    cmd.Parameters.Add(new SQLiteParameter("@Telephone" + index, req.Telephone));
                    cmd.Parameters.Add(new SQLiteParameter("@Addr" + index, req.Addr));
                    cmd.Parameters.Add(new SQLiteParameter("@Addr1" + index, req.Addr1));
                    cmd.Parameters.Add(new SQLiteParameter("@Region" + index, req.Region));
                    cmd.Parameters.Add(new SQLiteParameter("@Area" + index, req.Region));
                    cmd.Parameters.Add(new SQLiteParameter("@City" + index, req.City));
                    cmd.Parameters.Add(new SQLiteParameter("@ChiefFio" + index, req.ChiefFio));
                    cmd.Parameters.Add(new SQLiteParameter("@ChiefPosition" + index, req.ChiefPosition));
                    cmd.Parameters.Add(new SQLiteParameter("@ChiefEmail" + index, req.ChiefEmail));
                    cmd.Parameters.Add(new SQLiteParameter("@ChiefTelephone" + index, req.ChiefTelephone));
                    cmd.Parameters.Add(new SQLiteParameter("@SubsectionName" + index, req.SubsectionName));
                    cmd.Parameters.Add(new SQLiteParameter("@Fios" + index, req.Fios));
                    cmd.Parameters.Add(new SQLiteParameter("@Agies" + index, req.Agies));
                    cmd.Parameters.Add(new SQLiteParameter("@ProcMedicine" + index, (req.ProcMedicine) ? 1 : 0));
                    StringBuilder links = new StringBuilder();
                    int counter = 0;
                    foreach (string link in req.Links)
                    {
                        if (counter == 0) links.Append(link); else links.Append("^" + link);
                        counter++;
                    }
                    cmd.Parameters.Add(new SQLiteParameter("@Links" + index, links.ToString()));
                    cmd.Parameters.Add(new SQLiteParameter("@CompetitionName" + index, req.CompetitionName));
                    cmd.Parameters.Add(new SQLiteParameter("@DateReg" + index, req.DateReg));
                    cmd.Parameters.Add(new SQLiteParameter("@Likes" + index, req.Likes));
                    cmd.Parameters.Add(new SQLiteParameter("@Nolikes" + index, req.Nolikes));
                    cmd.Parameters.Add(new SQLiteParameter("@Approved" + index, (req.Approved) ? 1 : 0));
                    cmd.Parameters.Add(new SQLiteParameter("@PdnProcessing" + index, (req.PdnProcessing) ? 1 : 0));
                    cmd.Parameters.Add(new SQLiteParameter("@PublicAgreement" + index, (req.PublicAgreement) ? 1 : 0));
                    cmd.Parameters.Add(new SQLiteParameter("@SummaryLikes" + index, req.SummaryLikes));
                    cmd.Parameters.Add(new SQLiteParameter("@ClubsName" + index, req.ClubsName));
                    cmd.Parameters.Add(new SQLiteParameter("@Weight" + index, req.Weight));
                    cmd.Parameters.Add(new SQLiteParameter("@Weights" + index, req.Weights));
                    cmd.Parameters.Add(new SQLiteParameter("@Result" + index, req.Result));

                    index++;

                    #endregion
                }

                //DebugLog.Log(cmd.CommandText);

                if (sqlite.ExecuteNonQueryParams(cmd) == -1)
                {
                    result = false;
                }
                else
                {
                    sqlite.ConnectionClose();
                }
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                #region Код

                result = false;
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            return result;
        }*/

        #endregion
        #region Метод long UpdateOneRequest(CompetitionRequest_Arch obj)

        /// <summary>Метод обновляет в БД данные по одной структуре</summary>
        /// <param name="obj">объект с данными по одной структуре</param>
        /// <returns>Метод возвращает кол-во обновлённых строк или -1 в случае какой-либо ошибки</returns>
        public long UpdateOneRequest(CompetitionRequest_Arch obj)
        {
            long result = -1;
            if (!_checkNeedFolders) return result;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                cmd.CommandText = "UPDATE " + _tableName + " SET " +
                                                                "OldId=@OldId, " +
                                                                "Fio=@Fio, " +
                                                                "Class_=@Class_, " +
                                                                "Age=@Age, " +
                                                                "WorkName=@WorkName, " +
                                                                "WorkComment=@WorkComment, " +
                                                                "EducationalOrganization=@EducationalOrganization, " +
                                                                "Email=@Email, " +
                                                                "Telephone=@Telephone, " +
                                                                "Addr=@Addr, " +
                                                                "Addr1=@Addr1, " +
                                                                "PostIndex=@PostIndex, " +
                                                                "Region=@Region, " +
                                                                "City=@City, " +
                                                                "Street=@Street, " +
                                                                "Home=@Home, " +
                                                                "Room=@Room, " +
                                                                "ChiefFio=@ChiefFio, " +
                                                                "ChiefPosition=@ChiefPosition, " +
                                                                "ChiefEmail=@ChiefEmail, " +
                                                                "ChiefTelephone=@ChiefTelephone, " +
                                                                "SubsectionName=@SubsectionName, " +
                                                                "Fios=@Fios, " +
                                                                "Agies=@Agies, " +
                                                                "ProcMedicine=@ProcMedicine, " +
                                                                "Links=@Links, " +
                                                                "CompetitionName=@CompetitionName, " +
                                                                "DateReg=@DateReg, " +
                                                                "Likes=@Likes, " +
                                                                "Nolikes=@Nolikes, " +
                                                                "Approved=@Approved, " +
                                                                "PdnProcessing=@PdnProcessing, " +
                                                                "PublicAgreement=@PublicAgreement, " +
                                                                "SummaryLikes=@SummaryLikes, " +
                                                                "ClubsName=@ClubsName, " +
                                                                "Weight=@Weight, " +
                                                                "Weights=@Weights, " +
                                                                "AgeСategories=@AgeСategories, " +
                                                                "Kvalifications=@Kvalifications, " +
                                                                "Programms=@Programms, " +
                                                                "Result=@Result, " +
                                                                "Results=@Results, " +
                                                                "ProtocolFile=@ProtocolFile, " +
                                                                "ProtocolPartyCount=@ProtocolPartyCount, " +
                                                                "TechnicalInfo=@TechnicalInfo, " +
                                                                "Timing_min=@Timing_min, " +
                                                                "Timing_sec=@Timing_sec, " +
                                                                "ChiefFios=@ChiefFios, " +
                                                                "ChiefPositions=@ChiefPositions, " +
                                                                "AthorsFios=@AthorsFios, " +
                                                                "AgeСategory=@AgeСategory, " +
                                                                "PartyCount=@PartyCount, " +
                                                                "CheckedAdmin=@CheckedAdmin, " +
                                                                "Points=@Points," +
                                                                "Schools=@Schools," +
                                                                "ClassRooms=@ClassRooms," +
                                                                "ProtocolFileDoc=@ProtocolFileDoc," +
                                                                "Fios_1=@Fios_1, " +
                                                                "Agies_1=@Agies_1, " +
                                                                "Schools_1=@Schools_1," +
                                                                "ClassRooms_1=@ClassRooms_1," +
                                                                "Weights_1=@Weights_1," +
                                                                "IsApply=@IsApply," +
                                                                "DateApply=@DateApply," +
                                                                "Division=@Division" +
                                                            " WHERE _id=@_id";

                cmd.Parameters.Add(new SQLiteParameter("@_id", obj.Id));
                cmd.Parameters.Add(new SQLiteParameter("@OldId", obj.OldId));
                cmd.Parameters.Add(new SQLiteParameter("@Fio", obj.Fio));
                cmd.Parameters.Add(new SQLiteParameter("@Class_", obj.Class_));
                cmd.Parameters.Add(new SQLiteParameter("@Age", obj.Age));
                cmd.Parameters.Add(new SQLiteParameter("@WorkName", obj.WorkName));
                cmd.Parameters.Add(new SQLiteParameter("@WorkComment", obj.WorkComment));
                cmd.Parameters.Add(new SQLiteParameter("@EducationalOrganization", obj.EducationalOrganization));
                cmd.Parameters.Add(new SQLiteParameter("@EducationalOrganizationShort", obj.EducationalOrganizationShort));
                cmd.Parameters.Add(new SQLiteParameter("@Email", obj.Email));
                cmd.Parameters.Add(new SQLiteParameter("@Telephone", obj.Telephone));
                cmd.Parameters.Add(new SQLiteParameter("@Addr", obj.Addr));
                cmd.Parameters.Add(new SQLiteParameter("@Addr1", obj.Addr1));
                cmd.Parameters.Add(new SQLiteParameter("@District", obj.District));
                cmd.Parameters.Add(new SQLiteParameter("@Region", obj.Region));
                cmd.Parameters.Add(new SQLiteParameter("@Area", obj.Area));
                cmd.Parameters.Add(new SQLiteParameter("@City", obj.City));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefFio", obj.ChiefFio));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefPosition", obj.ChiefPosition));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefEmail", obj.ChiefEmail));
                cmd.Parameters.Add(new SQLiteParameter("@ChiefTelephone", obj.ChiefTelephone));
                cmd.Parameters.Add(new SQLiteParameter("@SubsectionName", obj.SubsectionName));
                cmd.Parameters.Add(new SQLiteParameter("@Fios", obj.Fios));
                cmd.Parameters.Add(new SQLiteParameter("@Agies", obj.Agies));
                cmd.Parameters.Add(new SQLiteParameter("@ProcMedicine", (obj.ProcMedicine) ? 1 : 0));
                StringBuilder sb = new StringBuilder();
                int counter = 0;
                foreach (string str in obj.Links)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("^" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@Links", sb.ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@CompetitionName", obj.CompetitionName));
                cmd.Parameters.Add(new SQLiteParameter("@DateReg", obj.DateReg));
                cmd.Parameters.Add(new SQLiteParameter("@Likes", obj.Likes));
                cmd.Parameters.Add(new SQLiteParameter("@Nolikes", obj.Nolikes));
                cmd.Parameters.Add(new SQLiteParameter("@Approved", (obj.Approved) ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@PdnProcessing", (obj.PdnProcessing) ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@PublicAgreement", (obj.PublicAgreement) ? 1 : 0));
                cmd.Parameters.Add(new SQLiteParameter("@SummaryLikes", obj.SummaryLikes));
                cmd.Parameters.Add(new SQLiteParameter("@ClubsName", obj.ClubsName));
                cmd.Parameters.Add(new SQLiteParameter("@Weight", obj.Weight));
                cmd.Parameters.Add(new SQLiteParameter("@Weights", obj.Weights));
                cmd.Parameters.Add(new SQLiteParameter("@AgeСategories", obj.AgeСategories));
                cmd.Parameters.Add(new SQLiteParameter("@Kvalifications", obj.Kvalifications));
                cmd.Parameters.Add(new SQLiteParameter("@Programms", obj.Programms));
                cmd.Parameters.Add(new SQLiteParameter("@Result", obj.Result));
                cmd.Parameters.Add(new SQLiteParameter("@Results", obj.Results));
                cmd.Parameters.Add(new SQLiteParameter("@ProtocolFile", obj.ProtocolFile));
                cmd.Parameters.Add(new SQLiteParameter("@ProtocolPartyCount", obj.ProtocolPartyCount));
                cmd.Parameters.Add(new SQLiteParameter("@TechnicalInfo", obj.TechnicalInfo));
                cmd.Parameters.Add(new SQLiteParameter("@Timing_min", obj.Timing_min));
                cmd.Parameters.Add(new SQLiteParameter("@Timing_sec", obj.Timing_sec));

                sb = new StringBuilder();
                counter = 0;
                foreach (string str in obj.ChiefFios)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@ChiefFios", sb.ToString()));
                sb = new StringBuilder();
                counter = 0;
                foreach (string str in obj.ChiefPositions)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@ChiefPositions", sb.ToString()));
                sb = new StringBuilder();
                counter = 0;
                foreach (string str in obj.AthorsFios)
                {
                    if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                    counter++;
                }
                cmd.Parameters.Add(new SQLiteParameter("@AthorsFios", sb.ToString()));
                cmd.Parameters.Add(new SQLiteParameter("@AgeСategory", obj.AgeСategory));
                cmd.Parameters.Add(new SQLiteParameter("@PartyCount", obj.PartyCount));
                cmd.Parameters.Add(new SQLiteParameter("@CheckedAdmin", obj.CheckedAdmin));
                cmd.Parameters.Add(new SQLiteParameter("@Points", obj.Points));

                cmd.Parameters.Add(new SQLiteParameter("@Schools", (string.IsNullOrEmpty(obj.Schools) ? "" : obj.Schools)));
                cmd.Parameters.Add(new SQLiteParameter("@ClassRooms", (string.IsNullOrEmpty(obj.ClassRooms) ? "" : obj.ClassRooms)));
                cmd.Parameters.Add(new SQLiteParameter("@ProtocolFileDoc", (string.IsNullOrEmpty(obj.ProtocolFileDoc) ? "" : obj.ProtocolFileDoc)));
                cmd.Parameters.Add(new SQLiteParameter("@Fios_1", (string.IsNullOrEmpty(obj.Fios_1) ? "" : obj.Fios_1)));
                cmd.Parameters.Add(new SQLiteParameter("@Agies_1", (string.IsNullOrEmpty(obj.Agies_1) ? "" : obj.Agies_1)));
                cmd.Parameters.Add(new SQLiteParameter("@Schools_1", (string.IsNullOrEmpty(obj.Schools_1) ? "" : obj.Schools_1)));
                cmd.Parameters.Add(new SQLiteParameter("@ClassRooms_1", (string.IsNullOrEmpty(obj.ClassRooms_1) ? "" : obj.ClassRooms_1)));
                cmd.Parameters.Add(new SQLiteParameter("@Weights_1", (string.IsNullOrEmpty(obj.Weights_1) ? "" : obj.Weights_1)));
                cmd.Parameters.Add(new SQLiteParameter("@IsApply", obj.IsApply));
                cmd.Parameters.Add(new SQLiteParameter("@DateApply", obj.DateApply));
                cmd.Parameters.Add(new SQLiteParameter("@Division", (string.IsNullOrEmpty(obj.Division) ? "" : obj.Division)));

                result = sqlite.ExecuteNonQueryParams(cmd);

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;

                #endregion
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
            }

            return result;
        }

        #endregion

        public List<CompetitionRequest_Arch> GetListOfRequests(string competition, string approved, string[] subnames)
        {
            if (competition == null) competition = "";
            if (approved == null) approved = "";
            if (subnames == null) subnames = new string[] { "" };

            var resultList = new List<CompetitionRequest_Arch>();
            var tmpList = new List<CompetitionRequest_Arch>();

            foreach (string subname in subnames)
            {
                tmpList = new CompetitionsWork_Arch().GetListOfRequests(competition, approved, subname);
                if (tmpList != null)
                {
                    resultList.AddRange(tmpList);
                }
            }

            return resultList;
        }

        #region Метод List<CompetitionRequest_Arch> GetListOfRequests(...)

        /// <summary>Метод возвращает готовый список структур запросов за участие в конкурсе.
        /// Может возвращать запросы, касающиеся одного, отдельного взятого конкурса.</summary>
        /// <param name="cName">условное название конкурса. Может быть - foto, literary, theatre</param>
        /// <param name="approved">значение для выборки опубликованных или неопубликованных заявок (можно передать 0 - выбрать неопубликованные или 1 - выбрать опубликованные).</param>
        /// <param name="subname">номинация</param>
        /// <param name="thisyear">архивная метка - если 0 то архив, если 1 то новое, если пусто то условие не используется</param>
        /// Если не передавать в это значие ничего, то метод не будет производить фильтрацию по этому полю</param>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<CompetitionRequest_Arch> GetListOfRequests(string cName = "", string approved = "", string subname = "")
        {
            if (!_checkNeedFolders) return null;

            //string sdacmd;
            var resultList = new List<CompetitionRequest_Arch>();

            var cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);

                #region Формирование строки запроса

                cmd.CommandText = "SELECT * FROM " + _tableName + " WHERE 1=1";

                if (cName != "")
                {
                    cmd.CommandText += " AND CompetitionName='" + cName + "'";
                }

                if (approved != "") // если дополнительно нужно отфильтровать на предмет публикации то
                {
                    if (approved == "0" || approved == "1") // условие-подстраховка (approved должно быть - 0 или 1, других вариантов нет)
                    {
                        cmd.CommandText += " AND Approved=" + approved + " AND IsApply=1 ";
                    }
                }

                //if (thisyear != "") //проверка, если 0 то архив, если 1 то новое, если пусто то условие не используется.
                //{
                //    if ((cName == "") && (approved == "")) cmd.CommandText += " WHERE";
                //    else cmd.CommandText += " AND";

                //    cmd.CommandText += " Compitition_year=";
                //    cmd.CommandText += thisyear;
                //}

                if (subname != "") // выбор по номинации
                {
                    if (subname.Contains("%"))
                    {
                        cmd.CommandText += " AND SubsectionName like '" + subname + "'";
                    }
                    else
                    {
                        cmd.CommandText += " AND SubsectionName = '" + subname + "'";
                    }
                }

                #endregion

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    #region Запись в лог

                    DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Произошла ошибка чтения данных из БД по запросу: " + cmd.CommandText);

                    #endregion

                    cmd.Dispose(); sqlite.ConnectionClose();
                    return null;
                }

                try
                {
                    #region Код заполнения списка

                    CompetitionRequest_Arch req = new CompetitionRequest_Arch();
                    while (reader.Read())
                    {
                        req = new CompetitionRequest_Arch();
                        FillRequest(req, reader);
                        resultList.Add(req);
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                }
                finally
                {
                    reader.Close(); reader.Dispose();
                    cmd.Dispose(); sqlite.ConnectionClose();
                }

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                resultList = null;
            }

            return resultList;
        }

        #endregion
        #region Метод List<CompetitionRequest_Arch> GetReqsForFields(string[] fieldNames)

        /// <summary>Функция возвращает список структур запросов на участие в конкурсе, в котором заполнены только нужные поля.</summary>
        /// <param name="fieldNames">названия полей, которые нужно выбрать из запросов</param>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<CompetitionRequest_Arch> GetReqsForFields(string[] fieldNames)
        {
            if (!_checkNeedFolders) return null;

            List<CompetitionRequest_Arch> resultList = new List<CompetitionRequest_Arch>();

            try
            {
                #region Основной код

                #region Инициализации и проверки

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT ";
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    if (i == 0) { cmd.CommandText += fieldNames[i]; }
                    else { cmd.CommandText += ", " + fieldNames[i]; }
                }
                cmd.CommandText += " FROM " + _tableName;

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    cmd.Dispose(); sqlite.ConnectionClose();
                    return null;
                }
                if (!reader.HasRows)
                {
                    cmd.Dispose(); sqlite.ConnectionClose();
                    return null;
                }

                #endregion 

                try
                {
                    #region Код заполнения списка

                    CompetitionRequest_Arch req = new CompetitionRequest_Arch();
                    while (reader.Read())
                    {
                        req = new CompetitionRequest_Arch();
                        FillRequestPartly(req, reader, fieldNames);
                        resultList.Add(req);
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    #region Код

                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                    #endregion
                }
                finally
                {
                    reader.Close(); reader.Dispose();
                    cmd.Dispose(); sqlite.ConnectionClose();
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                resultList = null;

                #endregion
            }

            return resultList;
        }

        #endregion
        #region Метод SelectWhere(....)

        /// <summary>Метод возвращает готовый список структур по переданным параметрам</summary>
        /// <param name="fieldsNamesSel">имена полей, которые нужно выбрать. Если указать null, будут выбраны все поля</param>
        /// <param name="fieldsNamesWhere">имена полей, по которым нужно отфильтровать. Если указать null, то фильтрации не будет</param>
        /// <param name="fieldsValueWhere">значения полей, по которым нужно отфильтровать. Если указать null, то фильтрации не будет. Количество аргументов должно совпадать с кол-вом аргументов в fielsNamesWhere</param>
        /// <param name="sqlLogic">логический оператор, по которому будут складываться фильтрация по именам и значениям фильтрующих полей</param>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<CompetitionRequest_Arch> SelectWhere(string[] fieldsNamesSel, string[] fieldsNamesWhere, string[] fieldsValueWhere, SqlLogic sqlLogic = SqlLogic.AND)
        {
            List<CompetitionRequest_Arch> resultList = new List<CompetitionRequest_Arch>();

            string sqlLog = EnumsHelper.GetSqlLogic(sqlLogic);
            if (sqlLog == "no result") return resultList;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            #region Формирование строки запроса

            cmd.CommandText = "SELECT ";
            if (fieldsNamesSel == null)
            {
                cmd.CommandText += "* ";
            }
            else
            {
                for (int i = 0; i < fieldsNamesSel.Length; i++)
                {
                    if (i == 0) cmd.CommandText += fieldsNamesSel[i];
                    else cmd.CommandText += ", " + fieldsNamesSel[i];
                }
            }
            cmd.CommandText += " FROM " + _tableName;
            if (fieldsNamesWhere != null && fieldsNamesWhere != null)
            {
                cmd.CommandText += " WHERE ";
                for (int i = 0; i < fieldsNamesWhere.Length; i++)
                {
                    if (i == 0) cmd.CommandText += fieldsNamesWhere[i] + "=@" + fieldsNamesWhere[i];
                    else cmd.CommandText += " " + sqlLog + " " + fieldsNamesWhere[i] + "=@" + fieldsNamesWhere[i];
                    cmd.Parameters.Add(new SQLiteParameter("@" + fieldsNamesWhere[i], fieldsValueWhere[i]));
                }
            }
            #endregion

            SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
            if (reader == null)
            {
                cmd.Dispose();
                resultList = null;
            }
            else if (!reader.HasRows)
            {
                cmd.Dispose();
            }
            else
            {
                try
                {
                    #region Код заполнения списка

                    CompetitionRequest_Arch obj = new CompetitionRequest_Arch();
                    while (reader.Read())
                    {
                        obj = new CompetitionRequest_Arch();
                        FillRequestPartly(obj, reader, fieldsNamesSel);
                        resultList.Add(obj);
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    #region Код

                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    resultList = null;

                    #endregion
                }
                finally
                {
                    #region Код

                    sqlite.ConnectionClose();
                    if (cmd != null) cmd.Dispose();

                    #endregion
                }
            }

            return resultList;
        }

        #endregion
        #region GetSortedListOfRequests(string competition = "", string srchString = "", string[] subnames = null, bool isArchive = false)
        /// <summary>
        /// Метод-обёртка над методом GetListOfRequests. Сортирует список структур по номеру заявки
        /// </summary>
        /// <param name="competition">условное название конкурса. Может быть - foto, literary, theatre</param>
        /// <param name="srchString">строка поискового запроса</param>
        /// <param name="subnames">условное наименование номинации</param>
        /// <returns></returns>
        public List<CompetitionRequest_Arch> GetSortedListOfRequests(string competition = "", string srchString = "", string[] subnames = null)
        {
            if (competition == null) competition = "";
            if (srchString == null) srchString = "";
            if (subnames == null) subnames = new string[] { "" };

            var resultList = new List<CompetitionRequest_Arch>();
            var tmpList = new List<CompetitionRequest_Arch>();

            foreach (string subname in subnames)
            {
                tmpList = GetSortedListOfRequests(competition, srchString, subname);
                if (tmpList != null)
                {
                    resultList.AddRange(tmpList);
                }
            }

            return resultList;
        }
        #endregion
        #region Метод List<CompetitionRequest_Arch> GetSortedListOfRequests(...)

        /// <summary>Метод-обёртка над методом GetListOfRequests. Сортирует список структур по номеру заявки</summary>
        /// <param name="competition">условное название конкурса. Может быть - foto, literary, theatre</param>
        /// <param name="srchString">строка поискового запроса</param>
        /// <param name="subname">условное наименование номинации</param>
        /// <returns>Возвращает список структур запросов или null - в случае какой-либо ошибки во время запроса.</returns>
        public List<CompetitionRequest_Arch> GetSortedListOfRequests(string competition = "", string srchString = "", string subname = "")
        {
            var tempList = GetListOfRequests(competition, "", subname);
            if (tempList == null) return null;
            tempList = tempList.OrderBy(x => x.Id * -1).ToList();

            #region Фильтрация по поисковой строке

            var resultList = new List<CompetitionRequest_Arch>();
            var mainMatchesList = new List<CompetitionRequest_Arch>();  //совпадения содержимого поля
            var fullMatchesList = new List<CompetitionRequest_Arch>();  //полные совпадения всего содержимого поля
            DateTime dt = new DateTime();
            srchString = srchString.Trim().ToLower();
            bool ChiefFiosChecker = false;
            bool ChiefPositionsChecker = false;
            bool AthorsFiosChecker = false;

            if (srchString != "")
            {
                foreach (CompetitionRequest_Arch req in tempList)
                {
                    #region Код

                    #region Код проверки полных совпадений всего содержимого поля

                    ChiefFiosChecker = false;
                    foreach (string item in req.ChiefFios)
                    {
                        if (item.ToLower() == srchString)
                        {
                            ChiefFiosChecker = true; break;
                        }
                    }
                    ChiefPositionsChecker = false;
                    foreach (string item in req.ChiefPositions)
                    {
                        if (item.ToLower() == srchString)
                        {
                            ChiefPositionsChecker = true; break;
                        }
                    }
                    AthorsFiosChecker = false;
                    foreach (string item in req.AthorsFios)
                    {
                        if (item.ToLower() == srchString)
                        {
                            AthorsFiosChecker = true; break;
                        }
                    }

                    if (
                            req.Id.ToString() == srchString ||
                            req.Fio.ToLower() == srchString ||
                            req.Age.ToLower() == srchString ||
                            req.Class_.ToLower() == srchString ||
                            req.WorkName.ToLower() == srchString ||
                            req.WorkComment.ToLower() == srchString ||
                            req.EducationalOrganization.ToLower() == srchString ||
                            req.EducationalOrganizationShort.ToLower() == srchString ||
                            req.Email.ToLower() == srchString ||
                            req.Telephone.ToLower() == srchString ||
                            req.Addr.ToLower() == srchString ||
                            req.Addr1.ToLower() == srchString ||
                            req.District.ToLower() == srchString ||
                            req.Region.ToLower() == srchString ||
                            req.Area.ToLower() == srchString ||
                            req.City.ToLower() == srchString ||
                            req.ChiefFio.ToLower() == srchString ||
                            req.ChiefPosition.ToLower() == srchString ||
                            req.ChiefEmail.ToLower() == srchString ||
                            req.ChiefTelephone.ToLower() == srchString ||
                            req.SubsectionName.ToLower() == srchString ||
                            req.Fios.ToLower().Contains(srchString) ||
                            req.Agies.ToLower().Contains(srchString) ||
                            req.Schools.ToLower().Contains(srchString) ||
                            req.ClassRooms.ToLower().Contains(srchString) ||
                            req.CompetitionName.ToLower() == srchString ||
                            req.ClubsName.ToLower() == srchString ||
                            req.Result.ToLower() == srchString ||
                            dt.ToShortDateString() == srchString ||
                            req.TechnicalInfo.ToLower() == srchString ||
                            req.Fios_1.ToLower().Contains(srchString) ||
                            req.Age_1.ToLower().Contains(srchString) ||
                            req.Schools_1.ToLower().Contains(srchString) ||
                            req.ClassRooms_1.ToLower().Contains(srchString) ||
                            ChiefFiosChecker || ChiefPositionsChecker || AthorsFiosChecker
                       )
                    {
                        fullMatchesList.Add(req);
                    }

                    #endregion

                    if (srchString.Contains(" "))
                    {
                        #region Код поиска в случае поискового запроса, состоящего из нескольких слов, разделённых пробелами

                        string[] srchArr = srchString.Split(new[] { ' ' });
                        dt = new DateTime(req.DateReg);
                        bool checker = false;

                        #region Сначала проверим вхождение целой фразы в полях,

                        ChiefFiosChecker = false;
                        foreach (string item in req.ChiefFios)
                        {
                            if (item.ToLower().Contains(srchString))
                            {
                                ChiefFiosChecker = true; break;
                            }
                        }
                        ChiefPositionsChecker = false;
                        foreach (string item in req.ChiefPositions)
                        {
                            if (item.ToLower().Contains(srchString))
                            {
                                ChiefPositionsChecker = true; break;
                            }
                        }
                        AthorsFiosChecker = false;
                        foreach (string item in req.AthorsFios)
                        {
                            if (item.ToLower().Contains(srchString))
                            {
                                AthorsFiosChecker = true; break;
                            }
                        }

                        if (req.Id.ToString().Contains(srchString) ||
                            req.Fio.ToLower().Contains(srchString) ||
                            req.Age.ToLower().Contains(srchString) ||
                            req.Class_.ToLower().Contains(srchString) ||
                            req.WorkName.ToLower().Contains(srchString) ||
                            req.WorkComment.ToLower().Contains(srchString) ||
                            req.EducationalOrganization.ToLower().Contains(srchString) ||
                            req.EducationalOrganizationShort.ToLower().Contains(srchString) ||
                            req.Email.ToLower().Contains(srchString) ||
                            req.Telephone.ToLower().Contains(srchString) ||
                            req.Addr.ToLower().Contains(srchString) ||
                            req.Addr1.ToLower().Contains(srchString) ||
                            req.District.ToLower().Contains(srchString) ||
                            req.Region.ToLower().Contains(srchString) ||
                            req.Area.ToLower().Contains(srchString) ||
                            req.City.ToLower().Contains(srchString) ||
                            req.ChiefFio.ToLower().Contains(srchString) ||
                            req.ChiefPosition.ToLower().Contains(srchString) ||
                            req.ChiefEmail.ToLower().Contains(srchString) ||
                            req.ChiefTelephone.ToLower().Contains(srchString) ||
                            req.SubsectionName.ToLower().Contains(srchString) ||
                            req.Fios.ToLower().Contains(srchString) ||
                            req.Agies.ToLower().Contains(srchString) ||
                            req.Schools.ToLower().Contains(srchString) ||
                            req.ClassRooms.ToLower().Contains(srchString) ||
                            req.CompetitionName.ToLower() == srchString ||
                            req.ClubsName.ToLower() == srchString ||
                            req.Result.ToLower() == srchString ||
                            dt.ToShortDateString() == srchString ||
                            req.TechnicalInfo.ToLower() == srchString ||
                            req.Fios_1.ToLower().Contains(srchString) ||
                            req.Age_1.ToLower().Contains(srchString) ||
                            req.Schools_1.ToLower().Contains(srchString) ||
                            req.ClassRooms_1.ToLower().Contains(srchString) ||
                            ChiefFiosChecker || ChiefPositionsChecker || AthorsFiosChecker
                            )
                        {
                            mainMatchesList.Add(req);
                            checker = true;
                        }

                        #endregion

                        #region Потом проверим отдельно вхождение каждого слова в поисковой строке

                        if (!checker)   //если вхождение поисковой фразы целиком не нашлось, то ищем вхождение слова из фразы
                        {
                            foreach (string srch in srchArr)
                            {
                                ChiefFiosChecker = false;
                                foreach (string item in req.ChiefFios)
                                {
                                    if (item.ToLower().Contains(srch))
                                    {
                                        ChiefFiosChecker = true; break;
                                    }
                                }
                                ChiefPositionsChecker = false;
                                foreach (string item in req.ChiefPositions)
                                {
                                    if (item.ToLower().Contains(srch))
                                    {
                                        ChiefPositionsChecker = true; break;
                                    }
                                }
                                AthorsFiosChecker = false;
                                foreach (string item in req.AthorsFios)
                                {
                                    if (item.ToLower().Contains(srch))
                                    {
                                        AthorsFiosChecker = true; break;
                                    }
                                }

                                if (req.Id.ToString().Contains(srch) ||
                                    req.Fio.ToLower().Contains(srch) ||
                                    req.Age.ToLower().Contains(srch) ||
                                    req.Class_.ToLower().Contains(srch) ||
                                    req.WorkName.ToLower().Contains(srch) ||
                                    req.WorkComment.ToLower().Contains(srch) ||
                                    req.EducationalOrganization.ToLower().Contains(srch) ||
                                    req.EducationalOrganizationShort.ToLower().Contains(srch) ||
                                    req.Email.ToLower().Contains(srch) ||
                                    req.Telephone.ToLower().Contains(srch) ||
                                    req.Addr.ToLower().Contains(srch) ||
                                    req.Addr1.ToLower().Contains(srch) ||
                                    req.District.ToLower().Contains(srch) ||
                                    req.Region.ToLower().Contains(srch) ||
                                    req.Area.ToLower().Contains(srch) ||
                                    req.City.ToLower().Contains(srch) ||
                                    req.ChiefFio.ToLower().Contains(srch) ||
                                    req.ChiefPosition.ToLower().Contains(srch) ||
                                    req.ChiefEmail.ToLower().Contains(srch) ||
                                    req.ChiefTelephone.ToLower().Contains(srch) ||
                                    req.SubsectionName.ToLower().Contains(srch) ||
                                    req.Fios.ToLower().Contains(srch) ||
                                    req.CompetitionName.ToLower().Contains(srch) ||
                                    req.ClubsName.ToLower().Contains(srch) ||
                                    req.Result.ToLower().Contains(srch) ||
                                    dt.ToShortDateString().Contains(srch) ||
                                    req.TechnicalInfo.ToLower().Contains(srch) ||
                                    ChiefFiosChecker || ChiefPositionsChecker || AthorsFiosChecker)
                                {
                                    resultList.Add(req);
                                    break;
                                }
                            }
                        }

                        #endregion

                        #endregion
                    }
                    else
                    {
                        #region Код поиска в случае отсутствия пробелов в поисковом запросе

                        ChiefFiosChecker = false;
                        foreach (string item in req.ChiefFios)
                        {
                            if (item.ToLower().Contains(srchString))
                            {
                                ChiefFiosChecker = true; break;
                            }
                        }
                        ChiefPositionsChecker = false;
                        foreach (string item in req.ChiefPositions)
                        {
                            if (item.ToLower().Contains(srchString))
                            {
                                ChiefPositionsChecker = true; break;
                            }
                        }
                        AthorsFiosChecker = false;
                        foreach (string item in req.AthorsFios)
                        {
                            if (item.ToLower().Contains(srchString))
                            {
                                AthorsFiosChecker = true; break;
                            }
                        }

                        dt = new DateTime(req.DateReg);
                        if (req.Id.ToString().Contains(srchString) ||
                            req.Fio.ToLower().Contains(srchString) ||
                            req.Age.ToLower().Contains(srchString) ||
                            req.Class_.ToLower().Contains(srchString) ||
                            req.WorkName.ToLower().Contains(srchString) ||
                            req.WorkComment.ToLower().Contains(srchString) ||
                            req.EducationalOrganization.ToLower().Contains(srchString) ||
                            req.EducationalOrganizationShort.ToLower().Contains(srchString) ||
                            req.Email.ToLower().Contains(srchString) ||
                            req.Telephone.ToLower().Contains(srchString) ||
                            req.Addr.ToLower().Contains(srchString) ||
                            req.Addr1.ToLower().Contains(srchString) ||
                            req.District.ToLower().Contains(srchString) ||
                            req.Region.ToLower().Contains(srchString) ||
                            req.Area.ToLower().Contains(srchString) ||
                            req.City.ToLower().Contains(srchString) ||
                            req.ChiefFio.ToLower().Contains(srchString) ||
                            req.ChiefPosition.ToLower().Contains(srchString) ||
                            req.ChiefEmail.ToLower().Contains(srchString) ||
                            req.ChiefTelephone.ToLower().Contains(srchString) ||
                            req.SubsectionName.ToLower().Contains(srchString) ||
                            req.Fios.ToLower().Contains(srchString) ||
                            req.CompetitionName.ToLower().Contains(srchString) ||
                            req.ClubsName.ToLower().Contains(srchString) ||
                            req.Result.ToLower().Contains(srchString) ||
                            dt.ToShortDateString().Contains(srchString) ||
                            req.TechnicalInfo.ToLower().Contains(srchString) ||
                            ChiefFiosChecker || ChiefPositionsChecker || AthorsFiosChecker
                            )
                        {
                            mainMatchesList.Add(req);
                        }

                        #endregion
                    }

                    #endregion
                }

                #region Выставление точных совпадений по поиску на первое место

                fullMatchesList.AddRange(mainMatchesList);

                if (fullMatchesList.Count > 0)
                {
                    resultList.InsertRange(0, fullMatchesList);
                }

                //Удаление повторяющихся заявок
                resultList = resultList.Distinct().ToList();

                #endregion
            }
            else
            {
                resultList = tempList;
            }

            return resultList;

            #endregion
        }

        #endregion
        #region Метод GetOneRequest(string reqNum)

        /// <summary>Метод возвращает структуру заявки по её номеру</summary>
        /// <param name="reqNum">номер заявки</param>
        /// <returns>Возвращает null в случае ошибки</returns>
        public CompetitionRequest_Arch GetOneRequest(string reqNum)
        {
            if (!_checkNeedFolders) return null;

            CompetitionRequest_Arch req = new CompetitionRequest_Arch();

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();

                cmd.CommandText = "SELECT * FROM " + _tableName + " WHERE OldId=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", reqNum));

                SQLiteDataReader reader = sqlite.ExecuteReader(cmd);
                if (reader == null)
                {
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                    return null;
                }

                try
                {
                    while (reader.Read())
                    {
                        FillRequest(req, reader);
                    }
                }
                catch (Exception ex)
                {
                    DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    req = null;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    sqlite.ConnectionClose();
                }

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                req = null;
            }

            return (req != null && req.OldId == 0 ? null : req);
        }

        #endregion
        #region Метод int UpdateApproved(long id, int approved)

        /// <summary>Метод обновляет значение утверждения заявки в ТОП 30</summary>
        /// <param name="id">id заявки</param>
        /// <param name="approved">новое значения для утверждения в ТОП 30</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateApproved(long id, int approved)
        {
            int result = -1;
            if (!_checkNeedFolders) return result;

            try
            {
                #region Основной код

                SqliteHelper sqlite = new SqliteHelper(_pathToDb);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "UPDATE " + _tableName + " SET Approved=@approved WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", id));
                cmd.Parameters.Add(new SQLiteParameter("approved", approved));
                result = sqlite.ExecuteNonQueryParams(cmd);

                cmd.Dispose(); sqlite.ConnectionClose();

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }

            return result;
        }

        #endregion
        #region Метод int UpdateField(string id, string fieldName, string fieldValue)

        /// <summary>Метод обновляет значение одного поля в таблице заявок</summary>
        /// <param name="id">id заявки</param>
        /// <param name="fieldName">название поля</param>
        /// <param name="fieldValue">значение поля</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateField(string id, string fieldName, string fieldValue)
        {
            int result = -1;
            if (!_checkNeedFolders) return result;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                cmd.CommandText = "UPDATE " + _tableName + " SET " + fieldName + "=@value WHERE OldId=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", id));
                cmd.Parameters.Add(new SQLiteParameter("value", fieldValue));
                result = sqlite.ExecuteNonQueryParams(cmd);

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }
            finally
            {
                cmd.Dispose(); sqlite.ConnectionClose();
            }

            return result;
        }

        #endregion
        #region Метод int UpdateFieldGroup(string id, string fieldName, string position)

        /// <summary>Метод обновляет значение одного поля в таблице заявок. Значения переменных, передаваемых в метод должны быть перепроверены перед передачей в него.</summary>
        /// <param name="id">id заявки</param>
        /// <param name="fieldName">название поля</param>
        /// <param name="fieldValue">значение поля</param>
        /// <param name="position">позиция для вставки нового значения в списке вида - параметр|параметр|параметр|параметр</param>
        /// <returns>Возвращает кол-во обновлённых строк или -1 в случае ошибки</returns>
        public int UpdateFieldGroup(string id, string fieldName, string fieldValue, int position)
        {
            int result = -1;
            if (!_checkNeedFolders) return result;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                #region Сформируем значение, которое необходимо будет обновить в заявке

                string resFieldValue = "";
                CompetitionRequest_Arch req = GetOneRequest(id);
                if (req != null)
                {
                    #region Формирование значения

                    if (fieldName == "Fios") resFieldValue = req.Fios;
                    else if (fieldName == "Agies") resFieldValue = req.Agies;
                    else if (fieldName == "Weights") resFieldValue = req.Weights;
                    else if (fieldName == "ChiefFios")
                    {
                        StringBuilder sb = new StringBuilder();
                        int counter = 0;
                        foreach (string str in req.ChiefFios)
                        {
                            if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                            counter++;
                        }
                        resFieldValue = sb.ToString();
                    }
                    else if (fieldName == "ChiefPositions")
                    {
                        StringBuilder sb = new StringBuilder();
                        int counter = 0;
                        foreach (string str in req.ChiefPositions)
                        {
                            if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                            counter++;
                        }
                        resFieldValue = sb.ToString();
                    }
                    else if (fieldName == "AthorsFios")
                    {
                        StringBuilder sb = new StringBuilder();
                        int counter = 0;
                        foreach (string str in req.AthorsFios)
                        {
                            if (counter == 0) sb.Append(str); else sb.Append("|" + str);
                            counter++;
                        }
                        resFieldValue = sb.ToString();
                    }

                    resFieldValue = ReplaceFieldValues(resFieldValue, fieldValue, position);

                    #endregion
                }

                #endregion

                if (req != null)
                {

                    cmd.CommandText = "UPDATE " + _tableName + " SET " + fieldName + "=@value WHERE _id=@id";
                    cmd.Parameters.Add(new SQLiteParameter("id", id));
                    cmd.Parameters.Add(new SQLiteParameter("value", resFieldValue));
                    result = sqlite.ExecuteNonQueryParams(cmd);

                }

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }
            finally
            {
                cmd.Dispose(); sqlite.ConnectionClose();
            }

            return result;
        }

        #endregion
        #region Метод int DeleteOneRequest(...)

        /// <summary>Метод удаляет одну заявку из БД</summary>
        /// <param name="reqNum">id заявки</param>
        /// <param name="withWorkFiles">true - нужно ещё удалить файлы работ к этой заявке</param>
        /// <param name="req">объект заявки (нужно передавать, если вы хотите дополнительно удалсть файл работ)</param>
        /// <returns></returns>
        public int DeleteOneRequest(string reqNum, bool withWorkFiles = false, CompetitionRequest_Arch req = null)
        {
            int result = -1;
            if (!_checkNeedFolders) return result;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            SQLiteCommand cmd = new SQLiteCommand();

            try
            {
                #region Основной код

                cmd.CommandText = "DELETE FROM " + _tableName + " WHERE _id=@id";
                cmd.Parameters.Add(new SQLiteParameter("id", reqNum));
                result = sqlite.ExecuteNonQueryParams(cmd);

                #endregion
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = -1;
            }
            finally
            {
                cmd.Dispose(); sqlite.ConnectionClose();
            }


            if (withWorkFiles && req != null)
            {
                #region Асинхронный код удаления файла работы

                UniObjParams objParams = new UniObjParams()
                {
                    obj1 = req,
                };
                Thread thread = new Thread(new ParameterizedThreadStart((object data) =>
                {
                    #region Код

                    UniObjParams objParams1 = (UniObjParams)data;
                    CompetitionRequest_Arch request = (CompetitionRequest_Arch)objParams1.obj1;
                    string mainPath = System.Web.Hosting.HostingEnvironment.MapPath("~") + @"files\competitionfiles\";
                    string pathToProtocolDir = mainPath + @"protocols\";

                    string pathToKulturaDir = mainPath + @"kultura\";
                    string pathToLiteraryDir = mainPath + @"literary\";
                    string pathToFotoDir = mainPath + @"foto\";
                    string pathToTheatreDir = mainPath + @"theatre\";
                    string pathToToponimDir = mainPath + @"toponim\";

                    string pathToDir = "";

                    if (request.CompetitionName == EnumsHelper.GetKulturaCode(Kultura.self)) pathToDir = pathToKulturaDir;
                    if (request.CompetitionName == EnumsHelper.GetLiteraryCode(Literary.self)) pathToDir = pathToLiteraryDir;
                    if (request.CompetitionName == EnumsHelper.GetPhotoCode(Photo.self)) pathToDir = pathToFotoDir;
                    if (request.CompetitionName == EnumsHelper.GetTheatreCode(Theatre.self)) pathToDir = pathToTheatreDir;
                    if (request.CompetitionName == EnumsHelper.GetToponimCode(Toponim.self)) pathToDir = pathToToponimDir;
                    
                    if (pathToDir != "")
                    {
                        string name = "";
                        foreach (string fileName in request.Links)
                        {
                            #region Тело цикла

                            name = fileName.ToLower();
                            if (name.Contains(".jpg") ||
                                name.Contains(".jpeg") ||
                                name.Contains(".png") ||
                                name.Contains(".doc") ||
                                name.Contains(".docx") ||
                                name.Contains(".rar") ||
                                name.Contains(".zip") ||
                                name.Contains(".mp3"))
                            {
                                //Удаление файлов
                                if (File.Exists(pathToDir + fileName))
                                {
                                    File.Delete(pathToDir + fileName);
                                }
                                else
                                {
                                    DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: При удалении не найден файл - " + pathToDir + fileName);
                                }
                                //Удаление маленьких фотографий
                                string fileNameL = "";
                                if (name.Contains(".jpg"))
                                {
                                    fileNameL = name.Replace(".jpg", "") + "_l.jpg";
                                    if (File.Exists(pathToDir + fileNameL))
                                    {
                                        File.Delete(pathToDir + fileNameL);
                                    }
                                }
                                if (name.Contains(".jpeg"))
                                {
                                    fileNameL = name.Replace(".jpeg", "") + "_l.jpeg";
                                    if (File.Exists(pathToDir + fileNameL))
                                    {
                                        File.Delete(pathToDir + fileNameL);
                                    }
                                }
                                if (name.Contains(".png"))
                                {
                                    fileNameL = name.Replace(".png", "") + "_l.png";
                                    if (File.Exists(pathToDir + fileNameL))
                                    {
                                        File.Delete(pathToDir + fileNameL);
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        DebugLog.Log(ErrorEvents.warn, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст: Не удалось определить путь к папке для удаления файла работы к заявке № " + request.Id);
                    }
                    
                    //Удаление файла протокола
                    if (request.ProtocolFile != "")
                    {
                        if (File.Exists(pathToProtocolDir + request.ProtocolFile))
                        {
                            try
                            {
                                File.Delete(pathToProtocolDir + request.ProtocolFile);
                            }
                            catch
                            {

                            }
                        }
                    }

                    #endregion
                }));
                thread.Start(objParams);

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод bool DeleteUnnecessaryFiles()

        /// <summary>Метод удаляет все файлы из папок конкурсов, которые не принадлежат ни одной зарегистрированной в БД заявке</summary>
        /// <returns>Возвращает true в случае успеха, и false - в случае возникновения какой-либо ошибки</returns>
        public bool DeleteUnnecessaryFiles()
        {
            bool result = true;
            if (!_checkNeedFolders) return false;

            try
            {
                #region Основной код

                var allList = GetListOfRequests();
                var listOffileNames = new List<string>();
                var listOfProtocolsNames = new List<string>();
                foreach (CompetitionRequest_Arch request in allList)
                {
                    if (!request.SubsectionName.Contains("Театральное искусство:") && !request.SubsectionName.Contains("Художественное слово")) //в этих номинация в Links хранятся не имена файлов, а ссылки на ютуб-ролики
                    {
                        listOffileNames.AddRange(request.Links);
                    }
                    if (request.ProtocolFile != "")
                    {
                        listOfProtocolsNames.Add(request.ProtocolFile);
                    }
                }

                string[] pathToFilesFoto = Directory.GetFiles(_pathToFotoFolder, "*", SearchOption.TopDirectoryOnly);
                string[] pathToFilesLiterary = Directory.GetFiles(_pathToLiteraryFolder, "*", SearchOption.TopDirectoryOnly);
                string[] pathToFilesKultura = Directory.GetFiles(_pathToKulturaFolder, "*", SearchOption.TopDirectoryOnly);
                string[] pathToFilesToponim = Directory.GetFiles(_pathToToponimFolder, "*", SearchOption.TopDirectoryOnly);
                string[] pathToFilesTheatre = Directory.GetFiles(_pathToTheatreFolder, "*", SearchOption.TopDirectoryOnly);
                string[] pathToProtocolFiles = Directory.GetFiles(_pathToProtocolFolder, "*", SearchOption.TopDirectoryOnly);

                string fName = "";//, fLName = "";
                foreach (string path in pathToFilesFoto)
                {
                    fName = Path.GetFileName(path);
                    if (fName.Contains("_l."))   //если это файл уменьшенного фото, то приведем его имя к имени оригинального фото, чтобы произвести проверку и не удалять его..
                    {
                        fName = Path.GetFileNameWithoutExtension(path).Replace("_l", "") + Path.GetExtension(path);
                    }
                    if (!listOffileNames.Contains(fName))
                    {
                        File.Delete(path);
                    }
                }
                foreach (string path in pathToFilesLiterary)
                {
                    fName = Path.GetFileName(path);
                    if (!listOffileNames.Contains(fName))
                    {
                        File.Delete(path);
                    }
                }
                foreach (string path in pathToFilesKultura)
                {
                    fName = Path.GetFileName(path);
                    if (!listOffileNames.Contains(fName))
                    {
                        File.Delete(path);
                    }
                }
                foreach (string path in pathToFilesToponim)
                {
                    fName = Path.GetFileName(path);
                    if (!listOffileNames.Contains(fName))
                    {
                        File.Delete(path);
                    }
                }
                foreach (string path in pathToFilesTheatre)
                {
                    fName = Path.GetFileName(path);
                    if (!listOffileNames.Contains(fName))
                    {
                        File.Delete(path);
                    }
                }
                foreach (string path in pathToProtocolFiles)
                {
                    fName = Path.GetFileName(path);
                    if (!listOfProtocolsNames.Contains(fName))
                    {
                        File.Delete(path);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                result = false;

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод void FillRequest(CompetitionRequest_Arch req, SQLiteDataReader reader)

        /// <summary>Вспомогательный метод, который заполняет переданный в него объект типа CompetitionRequest данными из SQLiteDataReader</summary>
        /// <param name="req">объект</param>
        /// <param name="reader">объект</param>
        private void FillRequest(CompetitionRequest_Arch req, SQLiteDataReader reader)
        {
            //System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            try
            {
                #region Код
                req.Id = long.Parse(reader["OldId"].ToString());
                req.OldId = long.Parse(reader["OldId"].ToString());
                req.Fio = reader["Fio"].ToString();
                req.Class_ = reader["Class_"].ToString();
                req.Age = reader["Age"].ToString();
                req.WorkName = reader["WorkName"].ToString();
                req.WorkComment = reader["WorkComment"].ToString();
                req.EducationalOrganization = reader["EducationalOrganization"].ToString();
                req.EducationalOrganizationShort = reader["EducationalOrganizationShort"].ToString();
                req.Email = reader["Email"].ToString();
                req.Telephone = reader["Telephone"].ToString();
                req.District = reader["District"].ToString();
                req.Region = reader["Region"].ToString();
                req.Area = reader["Area"].ToString();
                req.City = reader["City"].ToString();
                req.ChiefFio = reader["ChiefFio"].ToString();
                req.ChiefPosition = reader["ChiefPosition"].ToString();
                req.ChiefEmail = reader["ChiefEmail"].ToString();
                req.ChiefTelephone = reader["ChiefTelephone"].ToString();
                req.SubsectionName = reader["SubsectionName"].ToString();
                req.Fios = reader["Fios"].ToString();
                req.Agies = reader["Agies"].ToString();
                req.AgeСategories = reader["AgeСategories"].ToString();
                req.Kvalifications = reader["Kvalifications"].ToString();
                req.Programms = reader["Programms"].ToString();

                string[] strSplit = reader["Links"].ToString().Split('^');
                if (strSplit.Length == 1 && strSplit[0] == "")
                {
                    strSplit = new string[] { };
                }
                req.Links = strSplit.ToList();

                req.CompetitionName = reader["CompetitionName"].ToString();
                req.DateReg = long.Parse(reader["DateReg"].ToString());
                req.Likes = long.Parse(reader["Likes"].ToString());
                req.Nolikes = long.Parse(reader["Nolikes"].ToString());
                req.Approved = int.Parse(reader["Approved"].ToString()) == 1;

                req.PdnProcessing = int.Parse(reader["PdnProcessing"].ToString()) == 1;
                req.PublicAgreement = int.Parse(reader["PublicAgreement"].ToString()) == 1;
                req.ProcMedicine = int.Parse(reader["ProcMedicine"].ToString()) == 1;
                req.SummaryLikes = long.Parse(reader["SummaryLikes"].ToString());
                req.ClubsName = reader["ClubsName"].ToString();

                req.Addr = reader["Addr"].ToString();
                req.Addr1 = reader["Addr1"].ToString();

                req.Weight = int.Parse(reader["Weight"].ToString());
                req.Weights = reader["Weights"].ToString();

                req.Result = reader["Result"].ToString();
                req.Results = reader["Results"].ToString();

                req.ProtocolFile = reader["ProtocolFile"].ToString();
                req.ProtocolPartyCount = int.Parse(reader["ProtocolPartyCount"].ToString());
                req.TechnicalInfo = reader["TechnicalInfo"].ToString();
                req.Timing_min = int.Parse(reader["Timing_min"].ToString());
                req.Timing_sec = int.Parse(reader["Timing_sec"].ToString());
                strSplit = reader["ChiefFios"].ToString().Split('|');
                if (strSplit.Length == 1 && strSplit[0] == "")
                {
                    strSplit = new string[] { };
                }
                req.ChiefFios = strSplit.ToList();
                strSplit = reader["ChiefPositions"].ToString().Split('|');
                if (strSplit.Length == 1 && strSplit[0] == "")
                {
                    strSplit = new string[] { };
                }
                req.ChiefPositions = strSplit.ToList();
                strSplit = reader["AthorsFios"].ToString().Split('|');
                if (strSplit.Length == 1 && strSplit[0] == "")
                {
                    strSplit = new string[] { };
                }
                req.AthorsFios = strSplit.ToList();

                req.AgeСategory = reader["AgeСategory"].ToString();
                req.PartyCount = int.Parse(reader["PartyCount"].ToString());
                req.CheckedAdmin = int.Parse(reader["CheckedAdmin"].ToString());
                req.Points = int.Parse(reader["Points"].ToString());

                req.Schools = reader["Schools"].ToString();
                req.ClassRooms = reader["ClassRooms"].ToString();
                req.ProtocolFileDoc = reader["ProtocolFileDoc"].ToString();

                req.Fios_1 = reader["Fios_1"].ToString();
                req.Agies_1 = reader["Agies_1"].ToString();
                req.Schools_1 = reader["Schools_1"].ToString();
                req.ClassRooms_1 = reader["ClassRooms_1"].ToString();
                req.Weights_1 = reader["Weights_1"].ToString();

                req.IsApply = int.Parse(reader["IsApply"].ToString()) == 1;
                req.DateApply = long.Parse(reader["DateApply"].ToString());
                req.Division = reader["Division"].ToString();

                if (!string.IsNullOrEmpty(req.ChiefFio) && !req.ChiefFios.Exists(x => x == req.ChiefFio))
                {
                    req.ChiefFios.Add(req.ChiefFio);

                    if (!string.IsNullOrEmpty(req.ChiefPosition))
                        req.ChiefPositions.Add(req.ChiefPosition);
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }
        }

        #endregion
        #region Метод FillRequestPartly(...)

        /// <summary>Метод наполняет переданный в него объект req данными поле, имена которых передаются в переменной fieldNames</summary>
        /// <param name="req">объект заявки</param>
        /// <param name="reader">ридер</param>
        /// <param name="fieldNames">имена полей, данные по которым можно взять</param>
        private void FillRequestPartly(CompetitionRequest_Arch req, SQLiteDataReader reader, string[] fieldNames)
        {
            foreach (string fieldName in fieldNames)
            {
                if (fieldName == "Fio")
                {
                    req.Fio = reader[fieldName].ToString();
                }
                if (fieldName == "Class_")
                {
                    req.Class_ = reader[fieldName].ToString();
                }
                if (fieldName == "Age")
                {
                    req.Age = reader[fieldName].ToString();
                }
                if (fieldName == "WorkName")
                {
                    req.WorkName = reader[fieldName].ToString();
                }
                if (fieldName == "WorkComment")
                {
                    req.WorkComment = reader[fieldName].ToString();
                }
                if (fieldName == "EducationalOrganization")
                {
                    req.EducationalOrganization = reader[fieldName].ToString();
                }
                if (fieldName == "EducationalOrganizationShort")
                {
                    req.EducationalOrganizationShort = reader[fieldName].ToString();
                }
                if (fieldName == "Email")
                {
                    req.Email = reader[fieldName].ToString();
                }
                if (fieldName == "Telephone")
                {
                    req.Telephone = reader[fieldName].ToString();
                }
                if (fieldName == "Addr")
                {
                    req.Addr = reader[fieldName].ToString();
                }
                if (fieldName == "Addr1")
                {
                    req.Addr1 = reader[fieldName].ToString();
                }
                if (fieldName == "District")
                {
                    req.District = reader[fieldName].ToString();
                    //if (reader[fieldName].GetType() == typeof(DBNull)) req.Postindex = ""; else req.Postindex = int.Parse(reader[fieldName].ToString()).ToString();
                }
                if (fieldName == "Region")
                {
                    req.Region = reader[fieldName].ToString();
                }
                if (fieldName == "Area")
                {
                    req.Area = reader[fieldName].ToString();
                }
                if (fieldName == "City")
                {
                    req.City = reader[fieldName].ToString();
                }
                if (fieldName == "ChiefFio")
                {
                    req.ChiefFio = reader[fieldName].ToString();
                }
                if (fieldName == "ChiefPosition")
                {
                    req.ChiefPosition = reader[fieldName].ToString();
                }
                if (fieldName == "ChiefEmail")
                {
                    req.ChiefEmail = reader[fieldName].ToString();
                }
                if (fieldName == "ChiefTelephone")
                {
                    req.ChiefTelephone = reader[fieldName].ToString();
                }
                if (fieldName == "SubsectionName")
                {
                    req.SubsectionName = reader[fieldName].ToString();
                }
                if (fieldName == "Fios")
                {
                    req.Fios = reader[fieldName].ToString();
                }
                if (fieldName == "Agies")
                {
                    req.Agies = reader[fieldName].ToString();
                }
                if (fieldName == "_id")
                {
                    req.Id = long.Parse(reader[fieldName].ToString());
                }
                if (fieldName == "OldId")
                {
                    req.OldId = long.Parse(reader[fieldName].ToString());
                }
                if (fieldName == "Links")
                {
                    string[] strSplit = reader[fieldName].ToString().Split('^');
                    req.Links = strSplit.ToList();
                }
                if (fieldName == "CompetitionName")
                {
                    req.CompetitionName = reader[fieldName].ToString();
                }
                if (fieldName == "DateReg")
                {
                    req.DateReg = long.Parse(reader[fieldName].ToString());
                }
                if (fieldName == "Likes")
                {
                    req.Likes = long.Parse(reader[fieldName].ToString());
                }
                if (fieldName == "Nolikes")
                {
                    req.Nolikes = long.Parse(reader[fieldName].ToString());
                }
                if (fieldName == "Approved")
                {
                    req.Approved = int.Parse(reader[fieldName].ToString()) == 1;
                }
                if (fieldName == "PdnProcessing")
                {
                    req.PdnProcessing = int.Parse(reader[fieldName].ToString()) == 1;
                }
                if (fieldName == "PublicAgreement")
                {
                    req.PublicAgreement = int.Parse(reader[fieldName].ToString()) == 1;
                }
                if (fieldName == "ProcMedicine")
                {
                    if (reader[fieldName].GetType() == typeof(DBNull)) req.ProcMedicine = false; else req.ProcMedicine = int.Parse(reader[fieldName].ToString()) == 1;
                }
                if (fieldName == "SummaryLikes")
                {
                    req.SummaryLikes = long.Parse(reader[fieldName].ToString());
                }
                if (fieldName == "ClubsName")
                {
                    if (reader[fieldName].GetType() == typeof(DBNull)) req.ClubsName = ""; else req.ClubsName = reader[fieldName].ToString();
                }
                if (fieldName == "Weight")
                {
                    if (reader[fieldName].GetType() == typeof(DBNull)) req.Weight = 0; else req.Weight = int.Parse(reader[fieldName].ToString());
                }
                if (fieldName == "Weights")
                {
                    req.Weights = reader[fieldName].ToString();
                }
                if (fieldName == "AgeСategories")
                {
                    req.AgeСategories = reader[fieldName].ToString();
                }
                if (fieldName == "Kvalifications")
                {
                    req.Kvalifications = reader[fieldName].ToString();
                }
                if (fieldName == "Programms")
                {
                    req.Programms = reader[fieldName].ToString();
                }
                if (fieldName == "Result")
                {
                    req.Result = reader[fieldName].ToString();
                }
                if (fieldName == "Results")
                {
                    req.Results = reader[fieldName].ToString();
                }
                if (fieldName == "ProtocolFile")
                {
                    req.ProtocolFile = reader[fieldName].ToString();
                }
                if (fieldName == "ProtocolPartyCount")
                {
                    req.ProtocolPartyCount = int.Parse(reader[fieldName].ToString());
                }
                if (fieldName == "TechnicalInfo")
                {
                    req.TechnicalInfo = reader[fieldName].ToString();
                }
                if (fieldName == "Timing_min")
                {
                    req.Timing_min = int.Parse(reader[fieldName].ToString());
                }
                if (fieldName == "Timing_sec")
                {
                    req.Timing_sec = int.Parse(reader[fieldName].ToString());
                }
                if (fieldName == "ChiefFios")
                {
                    string[] strSplit = reader[fieldName].ToString().Split('^');
                    req.ChiefFios = strSplit.ToList();
                }
                if (fieldName == "ChiefPositions")
                {
                    string[] strSplit = reader[fieldName].ToString().Split('^');
                    req.ChiefPositions = strSplit.ToList();
                }
                if (fieldName == "AthorsFios")
                {
                    string[] strSplit = reader[fieldName].ToString().Split('^');
                    req.AthorsFios = strSplit.ToList();
                }
                if (fieldName == "AgeСategory")
                {
                    req.AgeСategory = reader[fieldName].ToString();
                }
                if (fieldName == "PartyCount")
                {
                    req.PartyCount = int.Parse(reader[fieldName].ToString());
                }
            }
        }

        #endregion
        #region Метод bool AddPartyToRequest(...)

        /// <summary>Метод добавляет одного участника в заявку</summary>
        /// <param name="reqId">id заявки</param>
        /// <param name="fio">ФИО</param>
        /// <param name="age">возраст (дата рождения)</param>
        /// <param name="cname">условное наименование конкурса</param>
        /// /// <param name="weight">вес</param>
        /// <returns>Возвращает true - в случае успеха, false - в случае ошибки или неверно переданных параметров</returns>
        public bool AddPartyToRequest(string reqId, string fio, string age, string cname, string weight = null)
        {
            bool result = false;
            if (!_checkNeedFolders) return result;

            #region Проверки переменных

            if (StringToNum.ParseLong(reqId) == -1) return result;
            if (fio == "") return result;
            DateTime dtTmp;
            if (!DateTime.TryParse(age, out dtTmp))
            {
                return result;
            }
            if (cname == EnumsHelper.GetSportCode(Sport.self) && weight != null)
            {
                if (StringToNum.ParseInt(weight) == -1) return result;
            }

            #endregion

            #region Код

            CompetitionRequest_Arch request = GetOneRequest(reqId);
            if (request == null) return result;

            if (cname == EnumsHelper.GetSportCode(Sport.self) && weight != null)
            {
                #region Для спортивного конкурса

                if (request.Fios == "")
                {
                    if (request.Fio == "")
                    {
                        request.Fio = fio;
                        request.Age = age;
                        request.Weight = StringToNum.ParseInt(weight);
                    }
                    else
                    {
                        request.Fios = fio;
                        request.Agies = age;
                        request.Weights = weight;
                    }
                }
                else
                {
                    request.Fios += "|" + fio;
                    request.Agies += "|" + age;
                    request.Weights += "|" + weight;
                }

                #endregion
            }
            else
            {
                #region Для всех остальных конкурсов

                if (request.Fios == "")
                {
                    if (request.Fio == "")
                    {
                        request.Fio = fio;
                        request.Age = age;
                    }
                    else
                    {
                        request.Fios = fio;
                        request.Agies = age;
                    }
                }
                else
                {
                    request.Fios += "|" + fio;
                    request.Agies += "|" + age;
                }

                #endregion
            }

            #region Запись изменённой информации в БД

            long res = UpdateOneRequest(request);
            if (res > 0) result = true;

            #endregion

            #endregion

            return result;
        }

        #endregion
        #region Метод bool DelPartyFromRequest(...)

        /// <summary>Метод удаляет одного участника из заявки. Причём он может удалить как группового, так и индивидуального участника (в position должно быть -9)</summary>
        /// <param name="reqId">id заявки</param>
        /// <param name="position">номер позиции удаляемого участника в списке (если это значение -9, значит удаляется индивидуальный участник)</param>
        /// <param name="cname">условное наименование конкурса</param>
        /// <returns>Возвращает true - в случае успеха, false - в случае ошибки или неверно переданных параметров</returns>
        public bool DelPartyFromRequest(string reqId, string position, string cname)
        {
            bool result = false;
            if (!_checkNeedFolders) return result;
            cname = cname.Trim();

            #region Проверки переменных

            if (StringToNum.ParseLong(reqId) == -1) return result;
            if (StringToNum.ParseInt(position) == -1) return result;
            if (cname == "") return result;

            #endregion

            #region Код

            CompetitionRequest_Arch request = GetOneRequest(reqId);
            if (request == null) return result;

            int pos = StringToNum.ParseInt(position);

            if (cname == EnumsHelper.GetSportCode(Sport.self))
            {
                #region Для спортивного конкурса

                if (pos == -9)    //если нужно удалить данные по индивидуальному участнику
                {
                    request.Fio = "";
                    request.Age = "";
                    request.Weight = 0;
                }
                else                    //если нужно удалить данные по групповому участнику
                {
                    request.Fios = DeleteFieldValues(request.Fios, pos);
                    request.Agies = DeleteFieldValues(request.Agies, pos);
                    request.Weights = DeleteFieldValues(request.Weights, pos);
                }

                #endregion
            }
            else
            {
                #region Для всех остальных конкурсов

                if (pos == -9)    //если нужно удалить данные по индивидуальному участнику
                {
                    request.Fio = "";
                    request.Age = "";
                }
                else                    //если нужно удалить данные по групповому участнику
                {
                    request.Fios = DeleteFieldValues(request.Fios, pos);
                    request.Agies = DeleteFieldValues(request.Agies, pos);
                }

                #endregion
            }

            #region Запись изменённой информации в БД

            long res = UpdateOneRequest(request);
            if (res > 0) result = true;

            #endregion

            #endregion

            return result;
        }

        #endregion
        #region Метод bool AddChiefToRequest(...)

        /// <summary>Метод добавляет одного руководителя в заявку</summary>
        /// <param name="reqId">id заявки</param>
        /// <param name="fio">ФИО</param>
        /// <param name="position">должность</param>
        /// <returns>Возвращает true - в случае успеха, false - в случае ошибки или неверно переданных параметров</returns>
        public bool AddChiefToRequest(string reqId, string fio, string position)
        {
            bool result = false;
            fio = fio.Trim();
            position = position.Trim();

            #region Проверки переменных

            if (StringToNum.ParseLong(reqId) == -1) return result;
            if (fio == "") return result;
            if (position == "") return result;

            if (!CompetitonWorkCommon.IsFioOk(fio))
            {
                return result;
            }

            #endregion

            #region Код

            CompetitionRequest_Arch request = GetOneRequest(reqId);
            if (request == null) return result;

            request.ChiefFios.Add(fio);
            request.ChiefPositions.Add(position);

            #region Запись изменённой информации в БД

            long res = UpdateOneRequest(request);
            if (res > 0) result = true;

            #endregion

            #endregion

            return result;
        }

        #endregion
        #region Метод bool DelChiefFromRequest(...)

        /// <summary>Метод удаляет одного руководителя из заявки</summary>
        /// <param name="reqId">id заявки</param>
        /// <param name="position">номер позиции удаляемого руководителя в списке</param>
        /// <returns>Возвращает true - в случае успеха, false - в случае ошибки или неверно переданных параметров</returns>
        public bool DelChiefFromRequest(string reqId, string position)
        {
            bool result = false;

            #region Проверки переменных

            if (StringToNum.ParseLong(reqId) == -1) return result;
            if (StringToNum.ParseInt(position) == -1) return result;

            #endregion

            #region Код

            CompetitionRequest_Arch request = GetOneRequest(reqId);
            if (request == null) return result;

            int pos = StringToNum.ParseInt(position);

            request.ChiefFios.RemoveAt(pos);
            request.ChiefPositions.RemoveAt(pos);

            #region Запись изменённой информации в БД

            long res = UpdateOneRequest(request);
            if (res > 0) result = true;

            #endregion

            #endregion

            return result;
        }

        #endregion
        #region Метод List<PersonPair> GetChiefsFiosPositionList(CompetitionRequest_Arch req)
        /// <summary>
        /// Получение списка педагогов и должностей по заявке
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public List<PersonPair> GetChiefsFiosPositionList(CompetitionRequest_Arch req)
        {
            var fiosChiefList = new List<PersonPair>();
            if (req.ChiefFio != "" && req.ChiefFio.ToUpper().Trim() != "НЕТ")
            {
                fiosChiefList.Add(new PersonPair() { Name = req.ChiefFio, Position = req.ChiefPosition });
            }

            if (req.ChiefFios.Count > 0)
            {
                for (int i = 0; i < req.ChiefFios.Distinct().Count(); i++)
                {
                    if (!fiosChiefList.Exists(x => x.Name == req.ChiefFios[i]))
                        fiosChiefList.Add(new PersonPair()
                        {
                            Name = req.ChiefFios[i],
                            Position = (req.ChiefPositions.ElementAtOrDefault(i) != null ? req.ChiefPositions[i] : "")
                        });
                }
            }
            return fiosChiefList;
        }
        #endregion
        #region Метод bool AddAthorToRequest(...)

        /// <summary>Метод добавляет одного автора коллекции в заявку</summary>
        /// <param name="reqId">id заявки</param>
        /// <param name="fio">ФИО</param>
        /// <returns>Возвращает true - в случае успеха, false - в случае ошибки или неверно переданных параметров</returns>
        public bool AddAthorToRequest(string reqId, string fio)
        {
            bool result = false;
            fio = fio.Trim();

            #region Проверки переменных

            if (StringToNum.ParseLong(reqId) == -1) return result;
            if (fio == "") return result;

            string sPattern = "^[А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+ [А-ЯЁ][а-яё]+$";
            if (!Regex.IsMatch(fio, sPattern))
            {
                return result;
            }

            #endregion

            #region Код

            CompetitionRequest_Arch request = GetOneRequest(reqId);
            if (request == null) return result;

            request.AthorsFios.Add(fio);

            #region Запись изменённой информации в БД

            long res = UpdateOneRequest(request);
            if (res > 0) result = true;

            #endregion

            #endregion

            return result;
        }

        #endregion
        #region Метод bool DelAthorFromRequest(...)

        /// <summary>Метод удаляет одного автора коллекции из заявки</summary>
        /// <param name="reqId">id заявки</param>
        /// <param name="position">номер позиции удаляемого автора в списке</param>
        /// <returns>Возвращает true - в случае успеха, false - в случае ошибки или неверно переданных параметров</returns>
        public bool DelAthorFromRequest(string reqId, string position)
        {
            bool result = false;

            #region Проверки переменных

            if (StringToNum.ParseLong(reqId) == -1) return result;
            if (StringToNum.ParseInt(position) == -1) return result;

            #endregion

            #region Код

            CompetitionRequest_Arch request = GetOneRequest(reqId);
            if (request == null) return result;

            int pos = StringToNum.ParseInt(position);

            request.AthorsFios.RemoveAt(pos);

            #region Запись изменённой информации в БД

            long res = UpdateOneRequest(request);
            if (res > 0) result = true;

            #endregion

            #endregion

            return result;
        }

        #endregion
        #region Метод bool VacuumDb()

        /// <summary>Метод очищает пересобирает БД, удаляя все записи, помеченные на удаление. Или попросту уменьшает её размер удаляя ненужное.</summary>
        /// <returns>true - в случае успеха, false - в случае возникновения какой-либо ошибки</returns>
        public bool VacuumDb()
        {
            bool result = false;
            if (!_checkNeedFolders) return result;

            SqliteHelper sqlite = new SqliteHelper(_pathToDb);
            if (sqlite.Vacuum())
            {
                result = true;
            }
            sqlite.ConnectionClose();

            return result;
        }

        #endregion

        #region Метод long GetDbSize()

        /// <summary>Метод возвращает размер файла БД в килобайтах</summary>
        /// <returns></returns>
        public long GetDbSize()
        {
            long result = 0;
            if (!_checkNeedFolders) return result;

            #region Основной код

            if (File.Exists(_pathToDb))
            {
                FileInfo f = new FileInfo(_pathToDb);
                result = f.Length / 1024;
            }

            #endregion

            return result;
        }

        #endregion
        #region Метод long GetFoldersSize()

        /// <summary>Метод возвращает размер всех папок с файлами, относящиеся к конкурсам</summary>
        /// <returns></returns>
        public long GetFoldersSize()
        {
            long result = 0;
            if (!_checkNeedFolders) return result;
            if (_isArchivingFiles) return result;

            #region Основной код

            try
            {
                #region Основной код

                string[] arr1 = Directory.GetFiles(_pathToFotoFolder, "*", SearchOption.TopDirectoryOnly);
                string[] arr2 = Directory.GetFiles(_pathToLiteraryFolder, "*", SearchOption.TopDirectoryOnly);
                string[] arr3 = Directory.GetFiles(_pathToTheatreFolder, "*", SearchOption.TopDirectoryOnly);
                string[] arr4 = Directory.GetFiles(_pathToKulturaFolder, "*", SearchOption.TopDirectoryOnly);
                string[] arr5 = Directory.GetFiles(_pathToToponimFolder, "*", SearchOption.TopDirectoryOnly);
                List<string> list = new List<string>();
                list.AddRange(arr1); list.AddRange(arr2); list.AddRange(arr3); list.AddRange(arr4); list.AddRange(arr5);
                FileInfo f;
                foreach (string path in list)
                {
                    f = new FileInfo(path);
                    result += f.Length;
                }
                result = result / 1024;

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return 0;

                #endregion
            }

            #endregion

            return result;
        }

        #endregion

        #region Метод string GetFiosAndAgesInOne(string Fios, string Ages, string separate = "")

        /// <summary>Объединение всех возрастов и фамилий</summary>
        /// <param name="Fios"></param>
        /// <param name="Ages"></param>
        /// <param name="separate"></param>
        /// <returns></returns>
        public static string GetFiosAndAgesInOne(string Fios, string Ages, string separate = "")
        {
            string someone = "";

            try
            {
                string[] Fio = Fios.Split(new Char[] { '|' });
                string[] Age = Ages.Split(new Char[] { '|' });

                Fio = Fio.Where(x => x != "").ToArray();
                Age = Age.Where(x => x != "").ToArray();
                //это невероятно, но проверяем равенсво кол-ва записей.
                if (Fio.Length == Age.Length)
                {
                    DateTime dt;
                    for (int i = 0; i < Fio.Length; i++)
                    {
                        someone += Fio[i];
                        someone += " / ";
                        if (DateTime.TryParse(Age[i], out dt))
                        {
                            someone += Convert.ToString(GetAgeFromBurth(dt));
                        }
                        else
                        {
                            someone += Age[i];
                        }
                        someone += separate;
                    }
                }
                else if (Fio.Length != Age.Length)
                {
                    for (int i = 0; i < Fio.Length; i++)
                    {
                        someone += Fio[i];
                        someone += separate;
                    }
                }
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "CompetitionsWork_Arch", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            return someone;
        }

        #endregion
        #region Метод string GetWeightsInOne(CompetitionRequest_Arch obj, string separate = " ")

        /// <summary>Объединение всех весов в заявке</summary>
        /// <param name="obj">заявка</param>
        /// <param name="separate">разделитель</param>
        /// <returns></returns>
        public static string GetWeightsInOne(CompetitionRequest_Arch obj, string separate = " ")
        {
            StringBuilder result = new StringBuilder();

            if (obj.Weight > 0)
            {
                result.Append(obj.Weight.ToString());
            }

            if (obj.Weights.Trim() != "")
            {
                string[] strSplit = obj.Weights.Split(new[] { '|' });
                for (int i = 0; i < strSplit.Length; i++)
                {
                    if (i == 0)
                    {
                        if (obj.Weight > 0)
                        {
                            result.Append(separate + strSplit[i]);
                        }
                        else
                        {
                            result.Append(strSplit[i]);
                        }
                    }
                    else
                    {
                        result.Append(separate + strSplit[i]);
                    }
                }
            }

            return result.ToString();
        }

        #endregion
        #region Метод int GetAgeFromBurth(DateTime birthDate)

        /// <summary>Вычисление возраста по дате рождения</summary>
        /// <param name="birthDate"></param>
        /// <returns></returns>
        public static int GetAgeFromBurth(DateTime birthDate)
        {
            int result = 0;

            try
            {
                DateTime now = DateTime.Now;
                result = now.Year - birthDate.Year;
                if (birthDate > now.AddYears(-result)) result--;
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "CompetitionsWork_Arch", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод string AgeCathegory(string Age, string SubName)

        /// <summary>Oпределение средней возрастной категории коллектива</summary>
        /// <param name="Age">возраста в формате 'дата или возраст'|'дата или возраст'|'дата или возраст'</param>
        /// <param name="SubName">номинация</param>
        /// <returns></returns>
        public static string AgeCathegory(string Age, string CompetitionName, string SubName)
        {
            string someage = "";

            try
            {
                int agepredown = 0;
                int agedown = 0;
                int agemiddle = 0;
                int ageup = 0;
                int agemolodezh = 0;
                int ageover = 0;
                int mixed = 0;
                int group1 = 0;
                int group2 = 0;
                int group3 = 0;
                int group4 = 0;
                int profi = 0;
                int group2011 = 0;
                int group2012 = 0;
                int group2013 = 0;
                int group2014 = 0;
                int group2015 = 0;
                int group2016 = 0;

                string[] Ages = Age.Split('|');
                int tmpAge = 0;
                foreach (string age in Ages)
                {
                    if (age != "")
                    {
                        tmpAge = StringToNum.ParseInt(age);
                        if (tmpAge == -1)   //если передана полноценная дата рождения
                        {
                            someage = JustLookAge(GetAgeFromBurth(Convert.ToDateTime(age)), CompetitionName, SubName);
                        }
                        else                //если передан возраст
                        {
                            someage = JustLookAge(tmpAge, CompetitionName, SubName);
                        }

                        if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.doshkolnaya)) agepredown++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya)) agedown++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya)) agemiddle++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya)) ageup++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh)) agemolodezh++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya)) mixed++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group1)) group1++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2)) group2++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group3)) group3++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group4)) group4++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi)) profi++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2011)) group2011++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2012)) group2012++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2013)) group2013++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2014)) group2014++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2015)) group2015++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2016)) group2016++;
                        else if (someage == EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY)) ageover++;
                    }

                }

                //Просто отсортируем полученные кол-ва возрастов в списке (по возрастанию значения)
                List<UniObj> list = new List<UniObj>();
                list.Add(new UniObj() { Num = agepredown, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.doshkolnaya) });
                list.Add(new UniObj() { Num = agedown, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya) });
                list.Add(new UniObj() { Num = agemiddle, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya) });
                list.Add(new UniObj() { Num = ageup, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya) });
                list.Add(new UniObj() { Num = agemolodezh, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh) });
                list.Add(new UniObj() { Num = mixed, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya) });
                list.Add(new UniObj() { Num = group1, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group1) });
                list.Add(new UniObj() { Num = group2, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2) });
                list.Add(new UniObj() { Num = group3, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group3) });
                list.Add(new UniObj() { Num = group4, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group4) });
                list.Add(new UniObj() { Num = profi, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi) });
                list.Add(new UniObj() { Num = group2011, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2011) });
                list.Add(new UniObj() { Num = group2012, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2012) });
                list.Add(new UniObj() { Num = group2013, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2013) });
                list.Add(new UniObj() { Num = group2014, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2014) });
                list.Add(new UniObj() { Num = group2015, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2015) });
                list.Add(new UniObj() { Num = group2016, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2016) });
                list.Add(new UniObj() { Num = ageover, Str = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY) });
                list = list.OrderBy(a => a.Num).ToList();
                //Возмем последнее значение из него с наибольшим кол-вом возрастов
                someage = list[list.Count - 1].Str;
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "CompetitionsWork_Arch", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            return someage;
        }

        #endregion
        #region Метод string JustLookAge(int someage, string SubName)

        /// <summary>Метод определения возростной категрии</summary>
        /// <param name="someage"></param>
        /// <param name="SubName"></param>
        /// <returns></returns>
        public static string JustLookAge(int someage, string CompetitionName, string SubName)
        {
            string substr = "";

            try
            {
                #region Код

                if (CompetitionName == EnumsHelper.GetPhotoValue(Photo.self) &&
                    (
                    (SubName == EnumsHelper.GetPhotoValue(Photo.izo)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_avtor_igrushka)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_biseropletenie)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_fitodisign)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_gobelen)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_hud_vishivka)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_hud_vyazanie)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_bumagoplastika)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_loskut_shitie)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_makrame)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_voilokovalyanie)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_batik)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_dekupazh)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_hud_obr_kozhi)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_hud_obr_stekla)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_keramika)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_narod_igrush_isglini)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT2_rospis_poderevu)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_combitehnics)) ||
                    (SubName == EnumsHelper.GetPhotoValue(Photo.DPT1_plastilonografiya))
                    ))
                {
                    if ((someage >= 7) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 12) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 19) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetPhotoValue(Photo.self) &&
                    (
                        (SubName == EnumsHelper.GetPhotoValue(Photo.computerGraphic)) ||
                        (SubName == EnumsHelper.GetPhotoValue(Photo.computer_risunok)) ||
                        (SubName == EnumsHelper.GetPhotoValue(Photo.collazh_fotomontazh)) ||
                        (SubName == EnumsHelper.GetPhotoValue(Photo.photo))
                    ))
                {
                    if ((someage >= 9) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 12) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 19) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetLiteraryValue(Literary.self) &&
                    (SubName == EnumsHelper.GetLiteraryValue(Literary.stihi) ||
                    SubName == EnumsHelper.GetLiteraryValue(Literary.esse) ||
                    SubName == EnumsHelper.GetLiteraryValue(Literary.rasskaz) ||
                    SubName == EnumsHelper.GetLiteraryValue(Literary.sochinenie)))
                {
                    if ((someage >= 9) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 12) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage < 21)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetTheatreValue(Theatre.self) &&
                    (SubName == EnumsHelper.GetTheatreValue(Theatre.vokalAkademVokal) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.vokalEstradVokal) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.vokalFolklor)))
                {
                    if ((someage >= 8) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 16) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetTheatreValue(Theatre.self) &&
                    (SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrAnsambli) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrDuhovieUdarnInstrum) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrFortepiano) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrGitara) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrNarodnieInstrum) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrOrkestri) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrSintezator) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.insrumZanrStrunnoSmichkovieInstrumenti)))
                {
                    if ((someage >= 8) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 16) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetTheatreValue(Theatre.self) &&
                    (SubName == EnumsHelper.GetTheatreValue(Theatre.teatrIskusLitMuzKom) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.teatrIskusSpekt) ||
                    SubName == EnumsHelper.GetTheatreValue(Theatre.teatrIskusMultiGanr)))
                {
                    if ((someage >= 4) && (someage <= 6)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.doshkolnaya);
                    else if ((someage >= 7) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 11) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 14) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetTheatreValue(Theatre.self) &&
                    (SubName == EnumsHelper.GetTheatreValue(Theatre.hudSlovo)))
                {
                    if ((someage >= 8) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 16) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetKulturaValue(Kultura.self) &&
                    (SubName == EnumsHelper.GetKulturaValue(Kultura.presentaionEn) ||
                    SubName == EnumsHelper.GetKulturaValue(Kultura.iSeeCrimeaEn) ||
                    SubName == EnumsHelper.GetKulturaValue(Kultura.publishVkontakte) ||
                    SubName == EnumsHelper.GetKulturaValue(Kultura.audioGaid) ||
                    SubName == EnumsHelper.GetKulturaValue(Kultura.intellektualKviz)))
                {
                    if ((someage >= 7) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 11) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 16) && (someage <= 21)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetClothesValue(Clothes.self) &&
                    (SubName == EnumsHelper.GetClothesValue(Clothes.tmindividpokaz) ||
                    SubName == EnumsHelper.GetClothesValue(Clothes.tmavtorcollect) ||
                    SubName == EnumsHelper.GetClothesValue(Clothes.tmcollectpokaz) ||
                    SubName == EnumsHelper.GetClothesValue(Clothes.tmnetradicmaterial) ||
                    SubName == EnumsHelper.GetClothesValue(Clothes.tmissledovproject)))
                {
                    if (SubName == EnumsHelper.GetClothesValue(Clothes.tmcollectpokaz))
                    {
                        if ((someage >= 6) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                        else if ((someage >= 10) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                        else if ((someage >= 14) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                        else if ((someage >= 19) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi);
                        else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                    }
                    else if (SubName == EnumsHelper.GetClothesValue(Clothes.tmindividpokaz))
                    {
                        if ((someage >= 10) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                        else if ((someage >= 14) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                        else if ((someage >= 18) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi);
                        else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                    }
                    else if (SubName == EnumsHelper.GetClothesValue(Clothes.tmavtorcollect))
                    {
                        if ((someage >= 10) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                        else if ((someage >= 14) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                        else if ((someage >= 18) && (someage <= 21)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi);
                        else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                    }
                    else if (SubName == EnumsHelper.GetClothesValue(Clothes.tmnetradicmaterial))
                    {
                        if ((someage >= 10) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                        else if ((someage >= 14) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                        else if ((someage >= 18) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.profi);
                        else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                    }
                    else if (SubName == EnumsHelper.GetClothesValue(Clothes.tmissledovproject))
                    {
                        if ((someage >= 8) && (someage <= 8)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                        else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                    }
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.prostEdinoborstva)))
                {
                    if ((someage >= 6) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group1);
                    else if ((someage >= 8) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2);
                    else if ((someage >= 10) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group3);
                    else if ((someage >= 12) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group4);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.boks)))
                {
                    if ((someage >= 10) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 12) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 14) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 16) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.kungfu)))
                {
                    if ((someage >= 6) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.doshkolnaya);
                    else if ((someage >= 8) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 16) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.stendStrelba)))
                {
                    if ((someage >= 8) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 11) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 14) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.shahmaty)))
                {
                    if ((someage >= 6) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group1);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group3);
                    else if ((someage >= 6) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group4);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.shashki)))
                {
                    if ((someage >= 6) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group1);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group3);
                    else if ((someage >= 6) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group4);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                     (SubName == EnumsHelper.GetSportValue(Sport.football)))
                {
                    if ((someage >= 7) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2016);
                    else if ((someage >= 8) && (someage <= 8)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2015);
                    else if ((someage >= 9) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2014);
                    else if ((someage >= 10) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2013);
                    else if ((someage >= 11) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2012);
                    else if ((someage >= 12) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.group2011);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.basketball)))
                {
                    if ((someage >= 13) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetSportValue(Sport.self) &&
                    (SubName == EnumsHelper.GetSportValue(Sport.volleyball)))
                {
                    if ((someage >= 11) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 13) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetToponimValue(Toponim.self) &&
                    (SubName == EnumsHelper.GetToponimValue(Toponim.toponimika)))
                {
                    if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetRobotechValue(Robotech.self) &&
                    (SubName == EnumsHelper.GetRobotechValue(Robotech.robototechnika) ||
                    SubName == EnumsHelper.GetRobotechValue(Robotech.robototechnikaproject) ||
                    SubName == EnumsHelper.GetRobotechValue(Robotech.robototechnika3dmodel) ||
                    SubName == EnumsHelper.GetRobotechValue(Robotech.tinkercad) ||
                    SubName == EnumsHelper.GetRobotechValue(Robotech.programmproject)))
                {
                    if ((someage >= 7) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetVmesteSilaValue(VmesteSila.self) &&
                    (
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.hudSlovoPoeziya) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.hudSlovoProza) ||

                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaBalniyTanets) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaClassichTanets) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaEstradTanets) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaNarodTanets) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaSovremenTanets) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.horeographiaOstalnieGanri) ||

                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalAkademVokal) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalEstradVokal) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalFolklor) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalZest) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.vokalOstalnieGanri) ||

                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrFortepiano) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrSintezator) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrStrunnoSmichkovieInstrumenti) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrDuhovieUdarnInstrum) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrNarodnieInstrum) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrGitara) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrAnsambli) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.insrumZanrOstalnieGanri) ||

                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.theatreSpektakl) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.theatreScenka) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.theatreLiteraturnoMusikalnaya) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.theatreDrama) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeup) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupDay) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupNight) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupStsena) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterMakeupFantasy) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShair) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairPletenie) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairDay) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairNight) ||
                        SubName == EnumsHelper.GetVmesteSilaValue(VmesteSila.masterShairFantasy))
                    )
                {
                    if ((someage >= 6) && (someage <= 8)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 9) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 13) && (someage <= 16)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 17) && (someage <= 21)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }

                else if (CompetitionName == EnumsHelper.GetMultimediaValue(Multimedia.self) &&
                    (
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.morepobedkolibelsmelchakov) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.netzapyatihtolkotochki) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.vashipodvigineumrut) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.sevastopol44) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.nadevaemmitelnyashku) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.klyatvudaemsevastopolvernem) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.hranitalbomsemeinipamyat) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.ihpamaytuzivushiipoklonis) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.multimediinieizdaniya) ||
                        
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.yarisuupobedy) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.spesneirpobede) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.geroyamotserdca) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.plechomkplechu) ||
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.pamyatsilneevremeni)
                    ))
                {
                    if ((someage >= 6) && (someage <= 21)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetMultimediaValue(Multimedia.self) &&
                    (
                        SubName == EnumsHelper.GetMultimediaValue(Multimedia.metodicheskierazrabotki)
                    ))
                {
                    if ((someage >= 6) && (someage <= 100)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }

                else if (CompetitionName == EnumsHelper.GetClothesValue(Clothes.self) &&
                    ((SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturie)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieTkan)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieNetradicMaterial)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieFashion)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieTechRisunok)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieFoodArt)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieOgorod)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.uniyKuturieBeauty)) ||

                    (SubName == EnumsHelper.GetClothesValue(Clothes.chudoLoskutki)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.chudoLoskutkiIgrushkiKukliTvorRisunok)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.chudoLoskutkiIgrushkiKukli)) ||

                    (SubName == EnumsHelper.GetClothesValue(Clothes.eskiziModelier)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.eskiziModelierTvorRisunok)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.eskiziModelierFashion)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.eskiziModelierTechRisunok))
                    )
                    )
                {
                    if ((someage >= 8) && (someage <= 11)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 12) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 19) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetClothesValue(Clothes.self) &&
                    (
                    (SubName == EnumsHelper.GetClothesValue(Clothes.sedobnayaModa)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.sedobnayaModaFoodArt)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.sedobnayaModaOgorod)) ||
                    (SubName == EnumsHelper.GetClothesValue(Clothes.sedobnayaModaBeauty))
                    )
                    )
                {
                    if ((someage >= 6) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 10) && (someage <= 13)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 14) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else if ((someage >= 19) && (someage <= 23)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.molodezh);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetKorablikValue(Korablik.self) &&
                    ((SubName == EnumsHelper.GetKorablikValue(Korablik.hudSlovo)))
                    )
                {
                    if ((someage >= 3) && (someage <= 5)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.baybi);
                    else if ((someage >= 6) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetKorablikValue(Korablik.self) &&
                    ((SubName == EnumsHelper.GetKorablikValue(Korablik.horeographia)) ||
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.horeographiaBalniyTanets)) ||
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.horeographiaClassichTanets)) ||
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.horeographiaEstradTanets)) ||
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.horeographiaNarodTanets))
                    ))
                {
                    if ((someage >= 3) && (someage <= 5)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.baybi);
                    else if ((someage >= 6) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    //else if (someage <= 7) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetKorablikValue(Korablik.self) &&
                    (
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.vokalSolo)) ||
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.vokalMalieFormi)) ||
                    (SubName == EnumsHelper.GetKorablikValue(Korablik.vokalAnsambli)))
                    )
                {
                    if ((someage >= 3) && (someage <= 5)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.baybi);
                    else if ((someage >= 6) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    //выбор смешанной категории складывается из того, что есть кто-то в дошкольной и кто-то в младшей группе
                    //else if ((someage >= 3) && (someage <= 7)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetCrimrouteValue(Crimroute.self) &&
                    (
                    (SubName == EnumsHelper.GetCrimrouteValue(Crimroute.historyplace)) ||
                    (SubName == EnumsHelper.GetCrimrouteValue(Crimroute.militaryhistoryplace)) ||
                    (SubName == EnumsHelper.GetCrimrouteValue(Crimroute.literaturehistoryplace)))
                    )
                {
                    if ((someage >= 6) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 11) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 16) && (someage <= 21)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                //else if (CompetitionName == EnumsHelper.GetMathbattleValue(Mathbattle.self) &&
                //        SubName == EnumsHelper.GetMathbattleValue(Mathbattle.battle)
                //    )
                //{
                //    if ((someage >= 7) && (someage <= 9)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                //    else if ((someage >= 10) && (someage <= 12)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                //    else if ((someage >= 13) && (someage <= 15)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                //    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                //}
                else if (CompetitionName == EnumsHelper.GetKosmosValue(Kosmos.self) &&
                        SubName == EnumsHelper.GetKosmosValue(Kosmos.kosmos)
                    )
                {
                    if ((someage >= 7) && (someage <= 17)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.smeshannaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                else if (CompetitionName == EnumsHelper.GetScienceValue(Science.self) &&
                    (SubName == EnumsHelper.GetScienceValue(Science.ekologia_ochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.ekologia_zaochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.himiya_ochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.himiya_zaochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.fizika_ochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.fizika_zaochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.biologiya_ochno) ||
                    SubName == EnumsHelper.GetScienceValue(Science.biologiya_zaochno)))
                {
                    if ((someage >= 7) && (someage <= 10)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.mladshaya);
                    else if ((someage >= 11) && (someage <= 14)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.srednaya);
                    else if ((someage >= 15) && (someage <= 18)) substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.starshaya);
                    else substr = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "CompetitionsWork", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

                #endregion
            }

            return substr;
        }

        #endregion
        #region Метод GetAgeCategory(...)

        /// <summary>Метод получения значения возрастной категории прямо из объекта заявки</summary>
        /// <param name="obj">объект заявки</param>
        /// <returns></returns>
        public static string GetAgeCategory(CompetitionRequest_Arch obj)
        {
            string result = "";

            if (obj.AgeСategory != "")
            {
                result = obj.AgeСategory;
                return result;
            }

            List<string> listOfAgies = new List<string>();
            DateTime dt;

            if (obj.Age != "" && obj.Age != "0")
            {
                if (DateTime.TryParse(obj.Age, out dt))
                {
                    listOfAgies.Add(GetAgeFromBurth(dt).ToString());
                }
                else
                {
                    if (StringToNum.ParseInt(obj.Age) != -1)
                    {
                        listOfAgies.Add(obj.Age);
                    }
                }
            }

            if (obj.Agies != "")
            {
                string[] Agies = obj.Agies.Split(new Char[] { '|' });
                Agies = Agies.Where(x => x != "" && x != "0").ToArray();
                foreach (string item in Agies)
                {
                    if (DateTime.TryParse(item, out dt))
                    {
                        listOfAgies.Add(GetAgeFromBurth(dt).ToString());
                    }
                    else
                    {
                        if (StringToNum.ParseInt(item) != -1)
                        {
                            listOfAgies.Add(item);
                        }
                    }
                }
            }

            if (listOfAgies.Count == 0)  //если в заявке вообще нет возрастов, то проверяем поле с предопределенным возрастом
            {
                result = EnumsHelper.GetAgeCategoriesValue(AgeCategories.VNEKATEGORY);
                return result;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < listOfAgies.Count; i++)
            {
                if (i == 0) sb.Append(listOfAgies[i]); else sb.Append("|" + listOfAgies[i]);
            }

            result = AgeCathegory(sb.ToString(), obj.CompetitionName, obj.SubsectionName);

            return result;
        }

        #endregion
        #region Метод string FioReduce(string fio)

        /// <summary>Метод превращает строку вида 'Ситников Андрей Михайлович' в строку вида 'Ситников А.М.'</summary>
        /// <param name="fio">ФИО полностью</param>
        /// <returns></returns>
        public static string FioReduce(string fio)
        {
            string result = fio.Trim();

            #region Проверки

            if (fio == "") return "";
            if (!fio.Contains(" ")) return result;

            #endregion

            try
            {
                #region Код

                string[] tmpArr = result.Split(new[] { ' ' });
                if (tmpArr.Length == 3)
                {
                    result = tmpArr[0] + " " + tmpArr[1].Substring(0, 1).ToUpper() + "." + tmpArr[2].Substring(0, 1).ToUpper() + ".";
                }
                else if (tmpArr.Length == 2)    //проверка на всякий случай
                {
                    result = tmpArr[0] + " " + tmpArr[1].Substring(0, 1).ToUpper() + ".";
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "CompetitionsWork", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());


                #endregion
            }

            return result;
        }

        #endregion
        #region Метод string FioReduceFI(string fio)

        /// <summary>Метод превращает строку вида 'Ситников Андрей Михайлович' в строку вида 'Ситников Андрей'</summary>
        /// <param name="fio">ФИО полностью</param>
        /// <returns></returns>
        public static string FioReduceFI(string fio)
        {
            string result = fio.Trim();

            #region Проверки

            if (fio == "") return "";
            if (!fio.Contains(" ")) return result;

            #endregion

            try
            {
                #region Код

                string[] tmpArr = result.Split(new[] { ' ' });
                if (tmpArr.Length > 1)
                {
                    result = tmpArr[0] + " " + tmpArr[1];
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, "CompetitionsWork", MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());


                #endregion
            }

            return result;
        }

        #endregion

        #region Метод string ReplaceFieldValues(...)

        /// <summary>Метод заменяет значение в составных полях значений, например Fios, Agies, Weights</summary>
        /// <param name="oldFieldValues">старое составное значение поля</param>
        /// <param name="newFieldValue">новое значение, заменяемое в поле</param>
        /// <param name="position">индекс позии для нового значения</param>
        /// <returns>возвращает составное новое значение</returns>
        private string ReplaceFieldValues(string oldFieldValues, string newFieldValue, int position)
        {
            string result = "";

            string[] tmp = oldFieldValues.Split(new[] { '|' });
            if (position <= tmp.Length - 1)     //если штатная ситуация и нужно просто заменить значение..
            {
                tmp[position] = newFieldValue;
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (i == 0) result = tmp[i]; else result += "|" + tmp[i];
                }
            }
            else                                //если по каким-то причинам нужно добавить значение вне диапозона массива (этот вариант произошёл по причине недобавления в первых заявках весов спортсменов, возможны и другие подобные варианты)..
            {
                List<string> tmpList = new List<string>();
                foreach (string item in tmp)
                {
                    tmpList.Add(item);
                }
                int max = position - tmp.Length + 1;
                for (int i = 1; i <= max; i++)
                {
                    if (i < max) tmpList.Add(""); else tmpList.Add(newFieldValue);
                }

                for (int i = 0; i < tmpList.Count; i++)
                {
                    if (i == 0) result = tmpList[i]; else result += "|" + tmpList[i];
                }
            }

            return result;
        }

        #endregion
        #region Метод string DeleteFieldValues(...)

        /// <summary>Метод удаляет значение в составных полях значений, например Fios, Agies, Weights.</summary>
        /// <param name="oldFieldValues">старое составное значение поля</param>
        /// <param name="position">индекс позии удаляемого значения (если он выходит за границы значений поля, то ничего не удаляется)</param>
        /// <returns>возвращает новое составное значение</returns>
        private string DeleteFieldValues(string oldFieldValues, int position)
        {
            string result = "";

            string[] tmp = oldFieldValues.Split(new[] { '|' });

            if (position <= tmp.Length - 1)     //если штатная ситуация и нужно просто исключить значение..
            {
                List<string> tmpList = tmp.ToList();
                tmpList.RemoveAt(position);
                for (int i = 0; i < tmpList.Count; i++)
                {
                    if (i == 0) result = tmpList[i]; else result += "|" + tmpList[i];
                }
            }
            else                                //если по каким-то причинам нужно добавить значение вне диапозона массива (этот вариант произошёл по причине недобавления в первых заявках весов спортсменов, возможны и другие подобные варианты), то ничего не удаляем
            {
                result = new string(oldFieldValues.ToCharArray());
            }

            return result;
        }

        #endregion

        #region Метод void MoveRequestsToArchive()

        /// <summary>Метод переноса всех заявок из рабочей БД в архивную. Если во время копирования заявки в архив происходит ошибка, то эта заявка и её файлы
        /// не удаляется из действующей БД (в лог записывается событие ошибки)</summary>
        public void MoveRequestsToArchive()
        {
            var work = new CompetitionsWork();
            var list = work.GetListOfRequests();

            if (list.Count > 0)
            {
                #region Добавим в архивную БД все заявки

                List<CompetitionRequest_Arch> listToArch = new List<CompetitionRequest_Arch>();
                CompetitionRequest_Arch reqArch;
                foreach (CompetitionRequest req in list)
                {
                    reqArch = new CompetitionRequest_Arch();
                    reqArch.CopyFrom(req);
                    listToArch.Add(reqArch);
                }

                List<CompetitionRequest_Arch> listFinal = new List<CompetitionRequest_Arch>();
                foreach (CompetitionRequest_Arch req in listToArch)
                {
                    if (InsertOneRequest(req) == -1)
                    {
                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: не удалось перенести в архив заявку с id - " + req.OldId +
                             ". Файлы к этой заявке в связи с этим не перенесы тоже.");
                    }
                    else
                    {
                        listFinal.Add(req);
                    }
                }

                #endregion

                if (listFinal.Count > 0)
                {
                    #region Удалим скопированные заявки из действующей БД

                    foreach (CompetitionRequest_Arch req in listFinal)
                    {
                        if (work.DeleteOneRequest(req.OldId.ToString()) == -1)
                        {
                            DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: не получилось удались заявку из действующей БД, которая была скопирована в архив. Id заявки - " + req.OldId +
                                 ". Файл(ы) к этой заявке будут перенесены в архивные папки.");
                        }
                    }

                    #endregion
                    #region Очистим действующую БД

                    work.VacuumDb();

                    #endregion
                    /*
                    #region Сформируем список файлов заявок для копирования и последующего удаления

                    List<string> listOfFileNames = new List<string>();
                    List<string> listOfProtocolsNames = new List<string>();
                    foreach (CompetitionRequest_Arch request in listFinal)
                    {
                        if (!request.SubsectionName.Contains("Театральное искусство:") && !request.SubsectionName.Contains("Художественное слово")) //в этих номинация в Links хранятся не имена файлов, а ссылки на ютуб-ролики
                        {
                            listOfFileNames.AddRange(request.Links);
                        }
                        if (request.ProtocolFile != "")
                        {
                            listOfProtocolsNames.Add(request.ProtocolFile);
                        }
                    }

                    #endregion
                    */

                    /*
                    if (listOfFileNames.Count > 0)
                    {
                        #region Асинхронно перенесём файлы архивировнных заявок в архивные папки

                        _isArchivingFiles = true;
                        UniObjParams objParams = new UniObjParams()
                        {
                            obj1 = listOfFileNames,
                            obj2 = listOfProtocolsNames
                        };
                        Thread thread = new Thread(new ParameterizedThreadStart((object data) =>
                        {
                            #region Код

                            UniObjParams objParams1 = (UniObjParams)data;
                            List<string> listFileNames = (List<string>)objParams1.obj1;
                            List<string> listProtocolsNames = (List<string>)objParams1.obj2;

                            string mainPath = System.Web.Hosting.HostingEnvironment.MapPath("~") + @"files\competitionfiles\";
                            string pathToKultura = mainPath + @"kultura\";
                            string pathToToponim = mainPath + @"toponim\";
                            string pathToLiterary = mainPath + @"literary\";
                            string pathToFoto = mainPath + @"foto\";
                            string pathToTheatre = mainPath + @"theatre\";
                            string pathToVmesteSila = mainPath + @"vmestesila\";
                            string pathToClothes = mainPath + @"clothes\";
                            string pathToMultimedia = mainPath + @"multimedia\";
                            string pathToProtocols = System.Web.Hosting.HostingEnvironment.MapPath("~") + @"files\competitionfiles\protocols\";    //файлы протоколов не перемещаются в архив, а удаляются из исходной папки

                            string mainPath_arch = System.Web.Hosting.HostingEnvironment.MapPath("~") + @"files\competitionfiles_arch\";
                            string pathToKultura_arch = mainPath_arch + @"kultura_arch\";
                            string pathToToponim_arch = mainPath_arch + @"toponim_arch\";
                            string pathToLiterary_arch = mainPath_arch + @"literary_arch\";
                            string pathToFoto_arch = mainPath_arch + @"foto_arch\";
                            string pathToTheatre_arch = mainPath_arch + @"theatre_arch\";
                            string pathToVmesteSila_arch = mainPath_arch + @"vmestesila_arch\";
                            string pathToClothes_arch = mainPath_arch + @"clothes_arch\";
                            string pathToMultimedia_arch = mainPath_arch + @"multimedia_arch\";

                            string[] pathsToFilesFoto = Directory.GetFiles(pathToFoto, "*", SearchOption.TopDirectoryOnly);
                            string[] pathsToFilesLiterary = Directory.GetFiles(pathToLiterary, "*", SearchOption.TopDirectoryOnly);
                            string[] pathsToFilesKultura = Directory.GetFiles(pathToKultura, "*", SearchOption.TopDirectoryOnly);
                            string[] pathsToFilesToponim = Directory.GetFiles(pathToToponim, "*", SearchOption.TopDirectoryOnly);
                            string[] pathsToFilesTheatre = Directory.GetFiles(pathToTheatre, "*", SearchOption.TopDirectoryOnly);
                            string[] pathsToFilesVmesteSila = Directory.GetFiles(pathToVmesteSila, "*", SearchOption.TopDirectoryOnly);
                            string[] pathsToFilesClothes = Directory.GetFiles(pathToClothes, "*", SearchOption.TopDirectoryOnly);
                            string[] pathsToFilesMultimedia = Directory.GetFiles(pathToMultimedia, "*", SearchOption.TopDirectoryOnly);
                            string[] pathsToProtocols = Directory.GetFiles(pathToProtocols, "*", SearchOption.TopDirectoryOnly);

                            string fName = "";
                            foreach (string path in pathsToFilesFoto)
                            {
                                fName = Path.GetFileName(path);
                                if (listFileNames.Contains(fName))
                                {
                                    try
                                    {
                                        File.Copy(path, pathToFoto_arch + fName);
                                        File.Delete(path);

                                        #region Перенос маленьких фотографий

                                        string fileNameL = "";
                                        fName = fName.ToLower();
                                        if (fName.Contains(".jpg"))
                                        {
                                            fileNameL = fName.Replace(".jpg", "") + "_l.jpg";
                                            if (File.Exists(pathToFoto + fileNameL))
                                            {
                                                File.Copy(pathToFoto + fileNameL, pathToFoto_arch + fileNameL);
                                                File.Delete(pathToFoto + fileNameL);
                                            }
                                        }
                                        if (fName.Contains(".jpeg"))
                                        {
                                            fileNameL = fName.Replace(".jpeg", "") + "_l.jpeg";
                                            if (File.Exists(pathToFoto + fileNameL))
                                            {
                                                File.Copy(pathToFoto + fileNameL, pathToFoto_arch + fileNameL);
                                                File.Delete(pathToFoto + fileNameL);
                                            }
                                        }
                                        if (fName.Contains(".png"))
                                        {
                                            fileNameL = fName.Replace(".png", "") + "_l.png";
                                            if (File.Exists(pathToFoto + fileNameL))
                                            {
                                                File.Copy(pathToFoto + fileNameL, pathToFoto_arch + fileNameL);
                                                File.Delete(pathToFoto + fileNameL);
                                            }
                                        }

                                        #endregion
                                    }
                                    catch (Exception ex)
                                    {
                                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                                    }
                                }
                            }
                            foreach (string path in pathsToFilesLiterary)
                            {
                                fName = Path.GetFileName(path);
                                if (listFileNames.Contains(fName))
                                {
                                    try
                                    {
                                        File.Copy(path, pathToLiterary_arch + fName);
                                        File.Delete(path);
                                    }
                                    catch (Exception ex)
                                    {
                                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                                    }
                                }
                            }
                            foreach (string path in pathsToFilesKultura)
                            {
                                fName = Path.GetFileName(path);
                                if (listFileNames.Contains(fName))
                                {
                                    try
                                    {
                                        File.Copy(path, pathToKultura_arch + fName);
                                        File.Delete(path);
                                    }
                                    catch (Exception ex)
                                    {
                                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                                    }
                                }
                            }
                            foreach (string path in pathsToFilesToponim)
                            {
                                fName = Path.GetFileName(path);
                                if (listFileNames.Contains(fName))
                                {
                                    try
                                    {
                                        File.Copy(path, pathToToponim_arch + fName);
                                        File.Delete(path);
                                    }
                                    catch (Exception ex)
                                    {
                                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                                    }
                                }
                            }
                            foreach (string path in pathsToFilesTheatre)
                            {
                                fName = Path.GetFileName(path);
                                if (listFileNames.Contains(fName))
                                {
                                    try
                                    {
                                        File.Copy(path, pathToTheatre_arch + fName);
                                        File.Delete(path);
                                    }
                                    catch (Exception ex)
                                    {
                                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                                    }
                                }
                            }
                            foreach (string path in pathsToFilesVmesteSila)
                            {
                                fName = Path.GetFileName(path);
                                if (listFileNames.Contains(fName))
                                {
                                    try
                                    {
                                        File.Copy(path, pathToVmesteSila_arch + fName);
                                        File.Delete(path);
                                    }
                                    catch (Exception ex)
                                    {
                                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                                    }
                                }
                            }
                            foreach (string path in pathsToFilesClothes)
                            {
                                fName = Path.GetFileName(path);
                                if (listFileNames.Contains(fName))
                                {
                                    try
                                    {
                                        File.Copy(path, pathToClothes_arch + fName);
                                        File.Delete(path);
                                    }
                                    catch (Exception ex)
                                    {
                                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                                    }
                                }
                            }
                            foreach (string path in pathsToFilesMultimedia)
                            {
                                fName = Path.GetFileName(path);
                                if (listFileNames.Contains(fName))
                                {
                                    try
                                    {
                                        File.Copy(path, pathToMultimedia_arch + fName);
                                        File.Delete(path);
                                    }
                                    catch (Exception ex)
                                    {
                                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                                    }
                                }
                            }
                            foreach (string path in pathsToProtocols)
                            {
                                fName = Path.GetFileName(path);
                                if (listProtocolsNames.Contains(fName))
                                {
                                    try
                                    {
                                        File.Delete(path);
                                    }
                                    catch (Exception ex)
                                    {
                                        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                                    }
                                }
                            }
                            _isArchivingFiles = false;

                            #endregion
                        }));
                        thread.Start(objParams);

                        #endregion
                    }
                    */
                }
            }
        }

        #endregion
    }

    #endregion

    #region Класс EducationOrganizationWork

    public class EducationOrganizationWork
    {
        #region Поля

        private EducationOrgAreasContext _dbContext;

        #endregion

        #region Конструктор класса

        /// <summary>Конструктор класса.
        /// Так же инициализирует поля.</summary>
        public EducationOrganizationWork()
        {
            //_dbContext = new EducationOrgAreasContext();
        }

        #endregion

        #region Метод async Task<List<EducationOrganizations>> GetEducationOrganizationList(...)

        /// <summary>
        /// Метод возвращает список образовательных учреждений с привязкой к региону, областям, городам.
        /// </summary>
        /// <param name="searchString">Наименование образовательного учреждения</param>
        /// <returns>Возвращает список структур образовательных учреждений с привязкой к региону, областям, городам</returns>
        public async Task<List<EducationOrganizations>> GetEducationOrganizationList(string searchString = "")
        {
            try
            {
                var resultList = new List<EducationOrganizations>();

                using (_dbContext = new EducationOrgAreasContext())
                {
                    var temp = await _dbContext.EducationOrganization
                            .Where(x => DbFunctions.Like(x.FullName.ToLower(), searchString.ToLower()) ||
                                    DbFunctions.Like(x.Name.ToLower(), searchString.ToLower()))
                            .ToListAsync();
                    if (temp.Count > 0)
                    {
                        temp.ForEach(x => x.Priority = 100);
                        resultList.AddRange(temp);
                    }
                }

                using (_dbContext = new EducationOrgAreasContext())
                {
                    var searchStringSplit = searchString.Split(' ');
                    foreach (var dd in searchStringSplit)
                    {
                        var temp = await _dbContext.EducationOrganization
                            .Where(x => DbFunctions.Like(x.FullName.ToLower(), "%" + dd.ToLower() + "%")
                                || DbFunctions.Like(x.Name.ToLower(), "%" + dd.ToLower() + "%"))
                            .ToListAsync();

                        temp.ForEach(x => x.Priority = 10);

                        if (resultList.Count > 0 && temp.Count > 0)
                        {
                            foreach (var r in resultList)
                            {
                                bool find = false;
                                for (int t = 0; t < temp.Count; t++)
                                {
                                    if (!find && r.Name == temp[t].Name && r.Region == temp[t].Region && r.Area == temp[t].Area && r.City == temp[t].City)
                                    {
                                        r.Priority += temp[t].Priority;
                                        find = true;
                                        temp.RemoveAt(t);
                                        t--;
                                    }
                                }
                            }
                        }
                        resultList.AddRange(temp);
                    }
                }

                return resultList.OrderByDescending(x => x.Priority).Take(15).ToList();
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
            return null;
        }

        #endregion

        #region Метод async Task<List<dynamic> GetListRegionAreaCities(...)

        /// <summary>
        /// Метод возвращает список регионов, областей, городов.
        /// </summary>
        /// <param name="searchString">Регион или область или город</param>
        /// <returns>Возвращает список структур регионов, областей, городов</returns>
        public async Task<List<RegionAreaCities>> GetRegionAreaCitiesList(string searchString = "")
        {
            try
            {
                var resultList = new List<RegionAreaCities>();

                using (_dbContext = new EducationOrgAreasContext())
                {
                    var temp = await _dbContext.RegionAreaCity
                        .Where(x => DbFunctions.Like(x.City.ToLower(), searchString.ToLower()))
                        .ToListAsync();
                    if (temp.Count > 0)
                    {
                        temp.ForEach(x => x.Priority = 100);
                        resultList.AddRange(temp);
                    }
                }

                using (_dbContext = new EducationOrgAreasContext())
                {
                    var searchStringSplit = searchString.Split(' ');
                    foreach (var dd in searchStringSplit)
                    {
                        var temp = await _dbContext.RegionAreaCity
                            .Where(x => DbFunctions.Like(x.City.ToLower(), "%" + dd.ToLower() + "%"))
                            .ToListAsync();
                        temp.ForEach(x => x.Priority = 10);

                        if (resultList.Count > 0 && temp.Count > 0)
                        {
                            foreach (var r in resultList)
                            {
                                bool find = false;
                                for (int t = 0; t < temp.Count; t++)
                                {
                                    if (!find && r.Region == temp[t].Region && r.Area == temp[t].Area && r.City == temp[t].City)
                                    {
                                        r.Priority += temp[t].Priority;
                                        find = true;
                                        temp.RemoveAt(t);
                                        t--;
                                    }
                                }
                            }
                        }
                        resultList.AddRange(temp);
                    }

                    foreach (var dd in searchStringSplit)
                    {
                        var temp = await _dbContext.RegionAreaCity
                            .Where(x => DbFunctions.Like(x.Area.ToLower(), "%" + dd.ToLower() + "%"))
                            .ToListAsync();
                        temp.ForEach(x => x.Priority = 10);

                        if (resultList.Count > 0 && temp.Count > 0)
                        {
                            foreach (var r in resultList)
                            {
                                bool find = false;
                                for (int t = 0; t < temp.Count; t++)
                                {
                                    if (!find && r.Region == temp[t].Region && r.Area == temp[t].Area && r.City == temp[t].City)
                                    {
                                        r.Priority += temp[t].Priority;
                                        find = true;
                                        temp.RemoveAt(t);
                                        t--;
                                    }
                                }
                            }
                        }
                        resultList.AddRange(temp);
                    }

                    foreach (var dd in searchStringSplit)
                    {
                        var temp = await _dbContext.RegionAreaCity
                            .Where(x => DbFunctions.Like(x.Region.ToLower(), "%" + dd.ToLower() + "%"))
                            .ToListAsync();
                        temp.ForEach(x => x.Priority = 10);

                        if (resultList.Count > 0 && temp.Count > 0)
                        {
                            foreach (var r in resultList)
                            {
                                bool find = false;
                                for (int t = 0; t < temp.Count; t++)
                                {
                                    if (!find && r.Region == temp[t].Region && r.Area == temp[t].Area && r.City == temp[t].City)
                                    {
                                        r.Priority += temp[t].Priority;
                                        find = true;
                                        temp.RemoveAt(t);
                                        t--;
                                    }
                                }
                            }
                        }
                        resultList.AddRange(temp);
                    }
                }
           
                return resultList.OrderByDescending(x => x.Priority).Take(15).ToList();
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
            return null;
        }

        #endregion


        public Dictionary<string, EducationOrganizations> EducationOrganizations()
        {
            var resultList = new Dictionary<string, EducationOrganizations>();
            try
            {
                using (_dbContext = new EducationOrgAreasContext())
                {
                    var edu = _dbContext.EducationOrganization.ToList();
                    for (int i = 0; i < edu.Count; i++)
                    {
                        if (!resultList.ContainsKey(edu[i].FullName.Trim() + edu[i].Name.Trim()))
                        {
                            resultList.Add(edu[i].FullName.Trim() + edu[i].Name.Trim(), edu[i]);
                        }
                        //else
                        //{
                        //    var err = true;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
            return resultList;
        }
    }

    #endregion

    #endregion

    #region Код с описанием структур данных

    #region Класс CompetitionRequest

    /// <summary>Класс представляет структуру данных одной универсальной заявки,
    /// которая подходит для любого конкурса</summary>
    [Serializable]
    public class CompetitionRequest
    {
        #region Поля

        private string fio = "";                        //ФИО *
        public string Fio
        {
            get { return fio; }
            set { fio = value; }
        }
        private string class_ = "";                     //Класс * (возможно НЕТ)
        public string Class_
        {
            get { return class_; }
            set { class_ = value; }
        }
        private string age = "";                        //Возраст *
        public string Age
        {
            get { return age; }
            set { age = value; }
        }
        private string groupofage = "";  //возростная группа ребенка.
        public string Groupofage
        {
            get { return groupofage; }
            set { groupofage = value; }
        }
        private string workName = "";                   //Название работы *
        public string WorkName
        {
            get { return workName; }
            set { workName = value; }
        }
        private string workComment = "";                //Комментарий к работе
        public string WorkComment
        {
            get { return workComment; }
            set { workComment = value; }
        }
        private string educationalOrganization = "";    //Образовательная организация (полностью) * (возможно НЕТ)
        public string EducationalOrganization
        {
            get { return educationalOrganization; }
            set { educationalOrganization = value; }
        }
        private string educationalOrganizationShort = "";    //Образовательная организация (короткое) * 
        public string EducationalOrganizationShort
        {
            get { return educationalOrganizationShort; }
            set { educationalOrganizationShort = value; }
        }
        private string email = "";                      //Электронная почта участника *
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        private string telephone = "";                  //Контактный телефон участника *
        public string Telephone
        {
            get { return telephone; }
            set { telephone = value; }
        }

        private string district = "";               // Федеральный округ
        public string District
        {
            get { return district; }
            set { district = value; }
        }

        private string region = "";                   //Регион *
        public string Region
        {
            get { return region; }
            set { region = value; }
        }
        private string area = "";                     //Область *
        public string Area
        {
            get { return area; }
            set { area = value; }
        }
        private string city = "";                       //Город *
        public string City
        {
            get { return city; }
            set { city = value; }
        }
        
        private string chiefFio = "";                   //ФИО руководителя * (возможно НЕТ)
        public string ChiefFio
        {
            get { return chiefFio; }
            set { chiefFio = value; }
        }
        private string chiefPosition = "";              //Должность руководителя * (возможно НЕТ)
        public string ChiefPosition
        {
            get { return chiefPosition; }
            set { chiefPosition = value; }
        }
        private string chiefEmail = "";                 //Электронная почта руководителя * (возможно НЕТ)
        public string ChiefEmail
        {
            get { return chiefEmail; }
            set { chiefEmail = value; }
        }
        private string chiefTelephone = "";             //Контактный телефон руководителя * (возможно НЕТ)
        public string ChiefTelephone
        {
            get { return chiefTelephone; }
            set { chiefTelephone = value; }
        }
        private string subsectionName = "";
        public string SubsectionName                // используется в театральном, спортивном, в вернисаже и конкурсе научных работ, хранит значения из выбора. (Хранители истории, Лучший сюжет, Творческий прорыв)
        {
            get { return subsectionName; }
            set { subsectionName = value; }
        }
        private string fios = "";
        public string Fios                          // используется в театральном конкурсе, хранит ФИО участников по одной заявке, в виде <фио>|<фио>|<фио>|...
        {
            get { return fios; }
            set { fios = value; }
        }
        private string ages = "";
        public string Agies                          // используется в театральном конкурсе, хранит возраста участников, соответствующих ФИО участников по одной заявке, в виде <возраст>|<возраст>|<возраст>|...
        {
            get { return ages; }
            set { ages = value; }
        }
        private string weights = "";
        public string Weights                         // Веса спортсменов для спортивного конкурса (или весовые категории в Тхэквондо), в виде <вес>|<вес>|<вес>|...
        {
            get { return weights; }
            set { weights = value; }
        }
        private string ageСategories = "";
        public string AgeСategories                   // Возрастные категории каждого из участников, применяется только в Тхэквондо, в виде <значение>|<значение>|<значение>|...
        {
            get { return ageСategories; }
            set { ageСategories = value; }
        }
        private string kvalifications = "";
        public string Kvalifications                   // Квалификация каждого из участников, применяется только в Тхэквондо, в виде <значение>|<значение>|<значение>|...
        {
            get { return kvalifications; }
            set { kvalifications = value; }
        }
        private string programms = "";
        public string Programms                         // Виды программ выступления для каждого из участников, применяется только в Тхэквондо, в виде <значение>|<значение>/..|<значение>... Значений может быть любое кол-во
        {
            get { return programms; }
            set { programms = value; }
        }
        private string protocolFile = "";
        public string ProtocolFile                         // Имя файла протокола - proto_<yyyyMMdd_HHmmssffff>_<random>.pdf. Путь к файлам ~/files/competitionfiles/protocols;. Файлы протоколов при архивации должны удаляться. Это поле при архивации не сохраняется!!!
        {
            get { return protocolFile; }
            set { protocolFile = value; }
        }
        private int protocolPartyCount = 0;
        public int ProtocolPartyCount                         // Количество участников I отборочного тура отборочного тура. По сути оно должно совпадать с кол-вом участников в файле протокола (ProtocolFile). Это поле при архивации не сохраняется!!!
        {
            get { return protocolPartyCount; }
            set { protocolPartyCount = value; }
        }
        private string technicalInfo = "";
        public string TechnicalInfo                         // Техническое сопровождение. Применяется в номинации "Художественное слово". Информация о наличии видеоряда, музыкального сопровождения и т.п.
        {
            get { return technicalInfo; }
            set { technicalInfo = value; }
        }
        private int timing_min = 0;
        public int Timing_min                         // Хронометраж - минуты. Сколько минут длится видео или аудио (добавляются секунды из Timing_sec). Для номинаций Вокала, Инструментала и Театр моды
        {
            get { return timing_min; }
            set { timing_min = value; }
        }
        private int timing_sec = 0;
        public int Timing_sec                         // Хронометраж - секунды. Сколько минут длится видео или аудио (добавляются к минутам из Timing_min). Для номинаций Вокала, Инструментала и Театр моды
        {
            get { return timing_sec; }
            set { timing_sec = value; }
        }
        private List<string> chiefFios = new List<string>();
        public List<string> ChiefFios                                   // ФИО руководителей. Используется совместно с полем ChiefPositions (в виде <>|<>|<>|...). Для добавления нескольких руководителей. Для номинаций - Вокала, Инструментала и Театр моды
        {
            get { return chiefFios; }
            set { chiefFios = value; }
        }
        private List<string> chiefPositions = new List<string>();
        public List<string> ChiefPositions                              // Должность руководителей. Используется совместно с полем ChiefFios (в виде <>|<>|<>|...). Для добавления нескольких должностей для нескольких руководителей. Для номинаций - Вокала, Инструментала и Театр моды
        {
            get { return chiefPositions; }
            set { chiefPositions = value; }
        }
        private string authorFio = "";
        public string AthorFio                                  // ФИО одного автора коллекции. В базе данных не сохраняется
        {
            get { return authorFio; }
            set { authorFio = value; }
        }
        private List<string> authorsFios = new List<string>();
        public List<string> AthorsFios                                  // ФИО авторов коллекции (в виде <>|<>|<>|...). Для номенации - Театр моды: Авторская коллекция
        {
            get { return authorsFios; }
            set { authorsFios = value; }
        }
        private string ageСategory = "";
        public string AgeСategory                         // Возрастная категория. Применяется в номинациях "Хореография", "Вокальный жанр", "Инструментальный жанр". Хранит название категории - дошкольники, младшая, средняя, старшая, смешанная
        {
            get { return ageСategory; }
            set { ageСategory = value; }
        }
        private int partyCount = 0;
        public int PartyCount                         // Количество участников I отборочного тура в заявке. Применяется в номинациях "Хореографии", потому что сами участники не добавляются в заявку.
        {
            get { return partyCount; }
            set { partyCount = value; }
        }
        private string addr = "";
        public string Addr                         // используется для полного адреса
        {
            get { return addr; }
            set { addr = value; }
        }
        private string addr1 = "";
        public string Addr1                         // используется для полного адреса
        {
            get { return addr1; }
            set { addr1 = value; }
        }

        private int checkedAdmin = 0;
        public int CheckedAdmin                        // проверено администратором конкурса
        {
            get { return checkedAdmin; }
            set { checkedAdmin = value; }
        }

        private int points = 0;
        public int Points                        // баллы
        {
            get { return points; }
            set { points = value; }
        }

        private string school = "";
        public string School                            //место учебы
        {
            get { return school; }
            set { school = value; }
        }

        private string classRoom = "";
        public string ClassRoom                         //класс
        {
            get { return classRoom; }
            set { classRoom = value; }
        }

        private string schools = "";
        public string Schools                            //место учебы
        {
            get { return schools; }
            set { schools = value; }
        }

        private string classRooms = "";
        public string ClassRooms                         //класс
        {
            get { return classRooms; }
            set { classRooms = value; }
        }

        private string fio_1 = "";                        //ФИО *
        public string Fio_1
        {
            get { return fio_1; }
            set { fio_1 = value; }
        }

        private string age_1 = "";                        //Возраст *
        public string Age_1
        {
            get { return age_1; }
            set { age_1 = value; }
        }

        private string fios_1 = "";
        public string Fios_1                          // используется в театральном конкурсе, хранит ФИО участников по одной заявке, в виде <фио>|<фио>|<фио>|...
        {
            get { return fios_1; }
            set { fios_1 = value; }
        }
        private string ages_1 = "";
        public string Agies_1                         // используется в театральном конкурсе, хранит возраста участников, соответствующих ФИО участников по одной заявке, в виде <возраст>|<возраст>|<возраст>|...
        {
            get { return ages_1; }
            set { ages_1 = value; }
        }

        private string school_1 = "";
        public string School_1                            //место учебы
        {
            get { return school_1; }
            set { school_1 = value; }
        }

        private string classRoom_1 = "";
        public string ClassRoom_1                         //класс
        {
            get { return classRoom_1; }
            set { classRoom_1 = value; }
        }

        private string schools_1 = "";
        public string Schools_1                           //место учебы
        {
            get { return schools_1; }
            set { schools_1 = value; }
        }

        private string classRooms_1 = "";
        public string ClassRooms_1                         //класс
        {
            get { return classRooms_1; }
            set { classRooms_1 = value; }
        }

        private string weights_1 = "";
        public string Weights_1                         //вес
        {
            get { return weights_1; }
            set { weights_1 = value; }
        }

        private string protocolFileDoc = "";
        public string ProtocolFileDoc                         //файл протокола в DOC
        {
            get { return protocolFileDoc; }
            set { protocolFileDoc = value; }
        }

        private bool isApply = false;                 // флаг принятия заявки
        public bool IsApply
        {
            get { return isApply; }
            set { isApply = value; }
        }

        private long dateApply = 0;                           // дата принятия заявки, представленная временным штампом - DateTime.Now.Ticks
        public long DateApply
        {
            get { return dateApply; }
            set { dateApply = value; }
        }

        private string division = "";
        public string Division                         //Структурное подразделение
        {
            get { return division; }
            set { division = value; }
        }
        #endregion

        #region Поля специального назначения :-)

        private long id = 0;                           // номер заявки
        public long Id
        {
            get { return id; }
            set { id = value; }
        }
        private bool archive = false;    // Переменная архивных материалов. Если значение 0 - то архив, если 1 - то данные этого года.
        private bool Archive
        {
            get { return archive; }
            set { archive = value; }
        }

        private List<string> links = new List<string>();    // список ссылок или имён файлов. Могут быть ссылки на Видеоролики на Ютубе, на файлы Ворд, на фотографии. Путь для хранения ссылок - ~/files/competitionfiles/; ~/files/competitionfiles/foto; ~/files/competitionfiles/literary; ~/files/competitionfiles/theatre
        public List<string> Links                           //Список вида - "<>^<>^<>"
        {
            get { return links; }
            set { links = value; }
        }

        private string competitionName = "";                // название конкурса. Может быть - foto (фотоконкурс), literary (литературный конкурс), theatre (театральный конкурс)
        public string CompetitionName
        {
            get { return competitionName; }
            set { competitionName = value; }
        }

        private long dateReg = 0;                           // дата подачи заявки, представленная временным штампом - DateTime.Now.Ticks
        public long DateReg
        {
            get { return dateReg; }
            set { dateReg = value; }
        }

        private long likes = 0;                             // поле будет содержать количество голосов ЗА (нравится)
        public long Likes
        {
            get { return likes; }
            set { likes = value; }
        }

        private long nolikes = 0;                           // поле будет содержать количество голосов НЕТ (не нравится)
        public long Nolikes
        {
            get { return nolikes; }
            set { nolikes = value; }
        }

        private long summaryLikes = 0;                      // поле будет содержать количество голосов ЗА (для итогового голосования по работам)
        public long SummaryLikes
        {
            get { return summaryLikes; }
            set { summaryLikes = value; }
        }

        private bool approved = false;                      // поле содержит флаг утверждения заявки в ТОП 30, которые будут отображаться на сайте для открытого голосования. true - утверждено, false - неутверждено
        public bool Approved
        {
            get { return approved; }
            set { approved = value; }
        }

        private bool pdnProcessing = false;                 // поле содержит флаг согласия на обработку персональных данных
        public bool PdnProcessing
        {
            get { return pdnProcessing; }
            set { pdnProcessing = value; }
        }

        private bool publicAgreement = false;               // поле содержит флаг согласия на публикацию своих работ с сохранением авторства
        public bool PublicAgreement
        {
            get { return publicAgreement; }
            set { publicAgreement = value; }
        }
        private bool procMedicine = false; // поле для спортсменов, о наличии на соревнованиях медицинской справки и копии страховки.
        public bool ProcMedicine
        {
            get { return procMedicine; }
            set { procMedicine = value; }
        }

        private string clubsname = ""; // наименование клуба.
        public string ClubsName
        {
            get { return clubsname; }
            set { clubsname = value; }
        }
        private int weight = 0; //Вес для спортивного соревнования.
        public int Weight
        {
            get { return weight; }
            set { weight = value; }
        }
        private int weight_1 = 0; //Вес для спортивного соревнования.
        public int Weight_1
        {
            get { return weight_1; }
            set { weight_1 = value; }
        }

        private string result = ""; // результат квалификационного отбора (Лауреат 1 степени и т.п. вводят ответственные через консоль)
        public string Result
        {
            get { return result; }
            set { result = value; }
        }
        private string results = ""; // результаты квалификационного отбора для каждого участника (для Тхэквондо) (Лауреат 1 степени и т.п. вводят ответственные через консоль). Формат <значение>|<значение>|<значение>
        public string Results
        {
            get { return results; }
            set { results = value; }
        }
        private string cityPref = ""; // тип города, применяется при приеме заявки, в БД не сохраняется
        public string CityPref
        {
            get { return cityPref; }
            set { cityPref = value; }
        }

        public string TypeName { get; set; } = string.Empty;
        public string MRSD { get; set; } = string.Empty;

        #endregion

        #region Метод GetFiosStr(string separator = " ")

        /// <summary>Метод преобразует список ФИО участников в строку, используя нужный разделитель</summary>
        /// <param name="separator">разделитель</param>
        /// <returns>возвращает пустую строку, если список пуст</returns>
        public string GetFiosStr(string separator = " ")
        {
            StringBuilder result = new StringBuilder();
            try
            {
                if (Fios != "" && Fio == "")
                {
                    string[] arr = Fios.Split(new[] { '|' });
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (i == 0)
                        {
                            result.Append(arr[i]);
                        }
                        else
                        {
                            result.Append(separator + arr[i]);
                        }
                    }
                }
                else if (Fios == "" && Fio != "")
                {
                    result.Append(Fio + separator);
                }
                else if (Fios != "" && Fio != "")
                {
                    result.Append(Fio);

                    string[] arr = Fios.Split(new[] { '|' });
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (i == 0)
                        {
                            result.Append(arr[i]);
                        }
                        else
                        {
                            result.Append(separator + arr[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
            }
            return result.ToString();
        }

        #endregion
        #region Метод GetAuthorsStr(string separator = " ")

        /// <summary>Метод преобразует список ФИО авторов в строку, используя нужный разделитель</summary>
        /// <param name="separator">разделитель</param>
        /// <returns>возвращает пустую строку, если список пуст</returns>
        public string GetAuthorsStr(string separator = " ")
        {
            StringBuilder result = new StringBuilder();
            try
            {
                if (AthorsFios.Count > 0)
                {
                    for (int i = 0; i < AthorsFios.Count; i++)
                    {
                        if (i == 0)
                        {
                            result.Append(AthorsFios[i]);
                        }
                        else
                        {
                            result.Append(separator + AthorsFios[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
            }
            return result.ToString();
        }

        #endregion
        #region Метод GetFormAuthorsStr(string separator = " ")

        /// <summary>Метод преобразует список ФИО авторов в строку, используя нужный разделитель. ФИО в формате Фамилия И.О.</summary>
        /// <param name="separator">разделитель</param>
        /// <returns>возвращает пустую строку, если список пуст</returns>
        public string GetFormAuthorsStr(string separator = " ")
        {
            StringBuilder result = new StringBuilder();
            try
            {
                if (AthorsFios.Count > 0)
                {
                    for (int i = 0; i < AthorsFios.Count; i++)
                    {
                        if (i == 0)
                        {
                            result.Append(CompetitionsWork.FioReduce(AthorsFios[i]).Trim());
                        }
                        else
                        {
                            result.Append(separator + CompetitionsWork.FioReduce(AthorsFios[i]).Trim());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
            }
            return result.ToString();
        }

        #endregion
        #region Метод GetChiefFiosAndPositionsStr(string separator = " ", bool needPosition = true)

        /// <summary>Метод преобразует списки ФИО и должностей педагогов в строку, используя нужный разделитель</summary>
        /// <param name="separator">разделитель</param>
        /// <returns>возвращает пустую строку, если списки пусты</returns>
        public string GetChiefFiosAndPositionsStr(string separator = " ", bool needPosition = true)
        {
            StringBuilder result = new StringBuilder();

            try
            {
                if (ChiefFios.Count > 0)
                {
                    for (int i = 0; i < ChiefFios.Count; i++)
                    {
                        if (i == 0)
                        {
                            result.Append(ChiefFios[i] + (needPosition ? (i < ChiefPositions.Count() ? " / " + ChiefPositions[i] : "") : ""));
                        }
                        else
                        {
                            result.Append(separator + ChiefFios[i] + (needPosition ? (i < ChiefPositions.Count() ? " / " + ChiefPositions[i] : "") : ""));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
            }

            return result.ToString();
        }

        #endregion
        #region Метод GetFormChiefPositionsAndFios(string separator = " ")

        /// <summary>Метод преобразует списки всех ФИО и должностей педагогов в строку вида 'ДОЛЖНОСТЬ Фамилия И.О.', используя нужный разделитель</summary>
        /// <param name="separator">разделитель</param>
        /// <returns>возвращает пустую строку, если списки пусты</returns>
        public string GetFormChiefPositionsAndFios(string separator = " ")
        {
            StringBuilder result = new StringBuilder();

            try
            {
                if (ChiefFio.Trim() != "" && ChiefFio.Trim().ToUpper() != "НЕТ" && ChiefFios.Count > 0 && !ChiefFios.Contains(ChiefFio.Trim()))
                {
                    result.Append((ChiefPosition.Trim() + " " + CompetitionsWork.FioReduce(ChiefFio)).Trim());
                }

                if (ChiefFios.Count > 0)
                {
                    for (int i = 0; i < ChiefFios.Distinct().Count(); i++)
                    {
                        if (result.ToString() != "" && i == 0)          //если уже был добавлен Педагог и это первый элемент списка
                        {
                            result.Append(separator + (ChiefPositions.ElementAtOrDefault(i) != null ? ChiefPositions[i].Trim() : "Педагог:") + " " + (ChiefFios.ElementAtOrDefault(i) != null ? CompetitionsWork.FioReduce(ChiefFios[i]).Trim() : ""));
                        }
                        else if (result.ToString() == "" && i == 0)     //если не был добавлен Педагог и это первый элемент списка
                        {
                            result.Append((ChiefPositions.ElementAtOrDefault(i) != null ? ChiefPositions[i].Trim() : "Педагог:") + " " + (ChiefFios.ElementAtOrDefault(i) != null ? CompetitionsWork.FioReduce(ChiefFios[i]).Trim() : ""));
                        }
                        else                                            //в иных случаях
                        {
                            result.Append(separator + (ChiefPositions.ElementAtOrDefault(i) != null ? ChiefPositions[i].Trim()  : "Педагог:") + " " + (ChiefFios.ElementAtOrDefault(i) != null ? CompetitionsWork.FioReduce(ChiefFios[i]).Trim() : ""));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
            }

            return result.ToString();
        }

        #endregion
        #region Метод GetFioBirthAgecatWeightKvalifProgamResult_All()

        /// <summary>Метод возвращает список массивов формы - 
        /// 'ФИО участника','Дата рождения','Возрастная катег-я','Весовая катег-я','Квалификация','Программа(ы через запятую) выступления', 'Результат'</summary>
        /// <returns></returns>
        public List<string[]> GetFioBirthAgecatWeightKvalifProgamResult_All()
        {
            List<string[]> result = new List<string[]>();
            string[] arr = new string[] { };

            try
            {
                #region Код

                string[] fiosArr = Fios.Split(new[] { '|' });
                string[] agiesArr = Agies.Split(new[] { '|' });
                string[] ageCatArr = AgeСategories.Split(new[] { '|' });
                string[] weightCatArr = Weights.Split(new[] { '|' });
                string[] kvalifArr = Kvalifications.Split(new[] { '|' });
                string[] programArr = Programms.Split(new[] { '|' }).Select(a => a.Replace("/", ", ")).ToArray();
                string[] resultArr = Results.Split(new[] { '|' });

                for (int i = 0; i < fiosArr.Length; i++)
                {
                    arr = new string[] { fiosArr[i], agiesArr[i], ageCatArr[i], weightCatArr[i], kvalifArr[i], programArr[i], resultArr[i] };
                    result.Add(arr);
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод GetFioBirthAgecatWeightKvalifProgamResult_One(.)

        /// <summary>Метод возвращает массив по форме - 
        /// 'ФИО участника','Дата рождения','Возрастная катег-я','Весовая катег-я','Квалификация','Программа(ы через запятую) выступления', 'Результат'</summary>
        /// <param name="fio">ФИО участника, по которому нужно вернуть массив данных</param>
        /// <returns></returns>
        public string[] GetFioBirthAgecatWeightKvalifProgamResult_One(string fio)
        {
            string[] result = new string[] { };

            try
            {
                #region Код

                string[] fiosArr = Fios.Split(new[] { '|' });
                string[] agiesArr = Agies.Split(new[] { '|' });
                string[] ageCatArr = AgeСategories.Split(new[] { '|' });
                string[] weightCatArr = Weights.Split(new[] { '|' });
                string[] kvalifArr = Kvalifications.Split(new[] { '|' });
                string[] programArr = Programms.Split(new[] { '|' }).Select(a => a.Replace("/", ", ")).ToArray();
                string[] resultArr = Results.Split(new[] { '|' });

                for (int i = 0; i < fiosArr.Length; i++)
                {
                    if (fiosArr[i] == fio)
                    {
                        result = new string[] { fiosArr[i], agiesArr[i], ageCatArr[i], weightCatArr[i], kvalifArr[i], programArr[i], resultArr[i] };
                        break;
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод GetFioBirthAgecatWeightKvalifProgamResult_One(.)

        /// <summary>Метод возвращает массив по форме - 
        /// 'ФИО участника','Дата рождения','Возрастная катег-я','Весовая катег-я','Квалификация','Программа(ы через запятую) выступления', 'Результат'</summary>
        /// <param name="index">индекс фио участника в форматированном списке Fios</param>
        /// <returns></returns>
        public string[] GetFioBirthAgecatWeightKvalifProgamResult_One(int index)
        {
            string[] result = new string[] { };

            try
            {
                #region Код

                string[] fiosArr = Fios.Split(new[] { '|' });
                string[] agiesArr = Agies.Split(new[] { '|' });
                string[] ageCatArr = AgeСategories.Split(new[] { '|' });
                string[] weightCatArr = Weights.Split(new[] { '|' });
                string[] kvalifArr = Kvalifications.Split(new[] { '|' });
                string[] programArr = Programms.Split(new[] { '|' }).Select(a => a.Replace("/", ", ")).ToArray();
                string[] resultArr = Results.Split(new[] { '|' });

                for (int i = 0; i < fiosArr.Length; i++)
                {
                    if (i == index)
                    {
                        result = new string[] { fiosArr[i], agiesArr[i], ageCatArr[i], weightCatArr[i], kvalifArr[i], programArr[i], resultArr[i] };
                        break;
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод AddParty(.......)

        /// <summary>Метод добавляет расширенные данные по одному участнику. 
        /// Значения, передаваемые в этот метод должны быть предварительно проверены и сформированы</summary>
        /// <param name="fio">Фамилия Имя Отчество</param>
        /// <param name="birthday">Дата рождения - ДД.ММ.ГГГГ</param>
        /// <param name="ageCat">возрастная категория</param>
        /// <param name="weight">весования категория</param>
        /// <param name="kvalif">техническая квалификация</param>
        /// <param name="program">программа выступления (форматированная строка вида [значение]/[значение]/..)</param>
        /// <param name="result">результат (опционально)</param>
        public void AddParty(string fio, string birthday, string ageCat, string weight, string kvalif, string program, string result = "")
        {
            try
            {
                if (Fios != "")
                {
                    Fios += "|" + fio;
                }
                else
                {
                    Fios += fio;
                }

                if (Agies != "")
                {
                    Agies += "|" + birthday;
                }
                else
                {
                    Agies += birthday;
                }

                if (AgeСategories != "")
                {
                    AgeСategories += "|" + ageCat;
                }
                else
                {
                    AgeСategories += ageCat;
                }

                if (Weights != "")
                {
                    Weights += "|" + weight;
                }
                else
                {
                    Weights += weight;
                }

                if (Kvalifications != "")
                {
                    Kvalifications += "|" + kvalif;
                }
                else
                {
                    Kvalifications += kvalif;
                }

                if (Programms != "")
                {
                    Programms += "|" + program;
                }
                else
                {
                    Programms += program;
                }

                if (Results != "")
                {
                    Results += "|" + result;
                }
                else
                {
                    if (Fios.Contains("|")) Results += "|" + result;
                    else Results += result;
                }
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
            }
        }

        #endregion
        #region Метод DeleteParty(.)

        /// <summary>Удаление данных по участнику из объекта. Удаляются следующие данные:
        /// 'ФИО участника','Дата рождения','Возрастная катег-я','Весовая катег-я','Квалификация',
        /// 'Программа(ы через запятую) выступления', 'Результат'</summary>
        /// <param name="index">индекс удаляемых элементов данных в строках данных</param>
        public void DeleteParty(int index)
        {
            try
            {
                #region Код

                // Для начала удалим данные (это нужно делать сначала, потому что теоритечески именно при этих операциях может
                //возникнуть ошибка)

                List<string> listFios = Fios.Split(new[] { '|' }).ToList();
                listFios.RemoveAt(index);

                List<string> listBirth = Agies.Split(new[] { '|' }).ToList();
                listBirth.RemoveAt(index);

                List<string> listAgeCat = AgeСategories.Split(new[] { '|' }).ToList();
                listAgeCat.RemoveAt(index);

                List<string> listWeights = Weights.Split(new[] { '|' }).ToList();
                listWeights.RemoveAt(index);

                List<string> listKvalif = Kvalifications.Split(new[] { '|' }).ToList();
                listKvalif.RemoveAt(index);

                List<string> listProgms = Programms.Split(new[] { '|' }).ToList();
                listProgms.RemoveAt(index);

                List<string> listResults = Results.Split(new[] { '|' }).ToList();
                if (listResults.Count == 0) listResults.Add("");
                listResults.RemoveAt(index);

                // Затем сохраним новые данные в свойства Объекта

                Fios = "";
                for (int i = 0; i < listFios.Count; i++)
                {
                    if (i == 0) Fios += listFios[i];
                    else Fios += "|" + listFios[i];
                }

                Agies = "";
                for (int i = 0; i < listBirth.Count; i++)
                {
                    if (i == 0) Agies += listBirth[i];
                    else Agies += "|" + listBirth[i];
                }

                AgeСategories = "";
                for (int i = 0; i < listAgeCat.Count; i++)
                {
                    if (i == 0) AgeСategories += listAgeCat[i];
                    else AgeСategories += "|" + listAgeCat[i];
                }

                Weights = "";
                for (int i = 0; i < listWeights.Count; i++)
                {
                    if (i == 0) Weights += listWeights[i];
                    else Weights += "|" + listWeights[i];
                }

                Kvalifications = "";
                for (int i = 0; i < listKvalif.Count; i++)
                {
                    if (i == 0) Kvalifications += listKvalif[i];
                    else Kvalifications += "|" + listKvalif[i];
                }

                Programms = "";
                for (int i = 0; i < listProgms.Count; i++)
                {
                    if (i == 0) Programms += listProgms[i];
                    else Programms += "|" + listProgms[i];
                }

                Results = "";
                for (int i = 0; i < listResults.Count; i++)
                {
                    if (i == 0) Results += listResults[i];
                    else Results += "|" + listResults[i];
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());

                #endregion
            }
        }

        #endregion
        #region Метод UpdateFormattedStringProperty(...)

        /// <summary>Метод заменяет значение в свойстве объекта, представляющим собой форматированную строку вида - 
        /// [значение]|[значение]|.. Все передаваемые в данный метод значения должны быть предварительно проверены на правильность.</summary>
        /// <param name="newValue">новое значение</param>
        /// <param name="fieldName">имя поля, в котором нужно произвести замену 
        /// (возможные значения - Fios, Agies, AgeСategories, Weights, Kvalifications, Programms, Results)</param>
        /// <param name="position">позиция заменяемого значения</param>
        /// <returns>true - в случае успешной замены, false - в случае какой-либо ошибки</returns>
        public bool UpdateFormattedStringProperty(string newValue, string fieldName, int position)
        {
            bool result = true;

            try
            {
                #region Код

                List<string> list;
                StringBuilder sb;

                if (fieldName == "Fios")
                {
                    list = Fios.Split(new[] { '|' }).ToList();
                    list[position] = newValue;
                    sb = new StringBuilder();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i == 0) sb.Append(list[i]);
                        else sb.Append("|" + list[i]);
                    }
                    Fios = sb.ToString();
                }
                else if (fieldName == "Agies")
                {
                    list = Agies.Split(new[] { '|' }).ToList();
                    list[position] = newValue;
                    sb = new StringBuilder();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i == 0) sb.Append(list[i]);
                        else sb.Append("|" + list[i]);
                    }
                    Agies = sb.ToString();
                }
                else if (fieldName == "AgeСategories")
                {
                    list = AgeСategories.Split(new[] { '|' }).ToList();
                    list[position] = newValue;
                    sb = new StringBuilder();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i == 0) sb.Append(list[i]);
                        else sb.Append("|" + list[i]);
                    }
                    AgeСategories = sb.ToString();
                }
                else if (fieldName == "Weights")
                {
                    list = Weights.Split(new[] { '|' }).ToList();
                    list[position] = newValue;
                    sb = new StringBuilder();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i == 0) sb.Append(list[i]);
                        else sb.Append("|" + list[i]);
                    }
                    Weights = sb.ToString();
                }
                else if (fieldName == "Kvalifications")
                {
                    list = Kvalifications.Split(new[] { '|' }).ToList();
                    list[position] = newValue;
                    sb = new StringBuilder();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i == 0) sb.Append(list[i]);
                        else sb.Append("|" + list[i]);
                    }
                    Kvalifications = sb.ToString();
                }
                else if (fieldName == "Programms")
                {
                    list = Programms.Split(new[] { '|' }).ToList();

                    #region Трансформация значения newValue (приходит с разделителем ',')

                    List<string> tmpList = newValue.Split(new[] { ',' }).ToList();
                    tmpList = tmpList.Select(a => a.Trim()).ToList();
                    sb = new StringBuilder();
                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        if (i == 0) sb.Append(tmpList[i]);
                        else sb.Append("/" + tmpList[i]);
                    }
                    newValue = sb.ToString();

                    #endregion

                    list[position] = newValue;
                    sb = new StringBuilder();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i == 0) sb.Append(list[i]);
                        else sb.Append("|" + list[i]);
                    }
                    Programms = sb.ToString();
                }
                else if (fieldName == "Results")
                {
                    list = Results.Split(new[] { '|' }).ToList();
                    if (list.Count == 0) list.Add("");
                    list[position] = newValue;
                    sb = new StringBuilder();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i == 0) sb.Append(list[i]);
                        else sb.Append("|" + list[i]);
                    }
                    Results = sb.ToString();
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Код

                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());

                #endregion
            }

            return result;
        }

        #endregion
        #region Метод GetPartyCount()

        /// <summary>Метод формирует и возвращает Количество участников I отборочного тура в заявке</summary>
        /// <returns>возвращает Количество участников I отборочного тура в заявке</returns>
        public int GetPartyCount()
        {
            int result = 0;
            try
            {
                if (PartyCount > 0)
                {
                    result = PartyCount;
                }
                else
                {
                    if (Fio.Trim() != "")
                    {
                        result += 1;
                    }
                    if (Fios.Trim() != "")
                    {
                        string[] arr = Fios.Split(new char[] { '|' });
                        result += arr.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
            }
            return result;
        }

        #endregion

    }

    #endregion

    #region Класс CompetitionRequest_Arch

    /// <summary>Класс представляет структуру данных одной универсальной заявки, хранящейся в архивной БД и
    /// которая подходит для любого конкурса</summary>
    [Serializable]
    public class CompetitionRequest_Arch : CompetitionRequest
    {
        #region Поля


        private long oldId = 0;                           //номер заявки, который она имела в рабочей БД
        public long OldId
        {
            get { return oldId; }
            set { oldId = value; }
        }

        #endregion

        #region CopyFrom(CompetitionRequest req)

        /// <summary>Метод копирования данных из текущей заявки (объекта CompetitionRequest)</summary>
        /// <param name="req">объект CompetitionRequest</param>
        public void CopyFrom(CompetitionRequest req)
        {
            OldId = req.Id;
            Fio = req.Fio;
            Class_ = req.Class_;
            Age = req.Age;
            WorkName = req.WorkName;
            WorkComment = req.WorkComment;
            EducationalOrganization = req.EducationalOrganization;
            EducationalOrganizationShort = req.EducationalOrganizationShort;
            Email = req.Email;
            Telephone = req.Telephone;
            District = req.District;
            Region = req.Region;
            Area = req.Area;
            City = req.City;
            ChiefFio = req.ChiefFio;
            ChiefPosition = req.ChiefPosition;
            ChiefEmail = req.ChiefEmail;
            ChiefTelephone = req.ChiefTelephone;
            SubsectionName = req.SubsectionName;
            Fios = req.Fios;
            Agies = req.Agies;
            AgeСategories = req.AgeСategories;
            Kvalifications = req.Kvalifications;
            Programms = req.Programms;
            Links = req.Links;
            CompetitionName = req.CompetitionName;
            DateReg = req.DateReg;
            Likes = req.Likes;
            Nolikes = req.Nolikes;
            Approved = req.Approved;
            PdnProcessing = req.PdnProcessing;
            PublicAgreement = req.PublicAgreement;
            ProcMedicine = req.ProcMedicine;
            SummaryLikes = req.SummaryLikes;
            ClubsName = req.ClubsName;
            Weight = req.Weight;
            Result = req.Result;
            Results = req.Results;
            Weights = req.Weights;
            ProtocolFile = req.ProtocolFile;
            ProtocolPartyCount = req.ProtocolPartyCount;
            TechnicalInfo = req.TechnicalInfo;
            Timing_min = req.Timing_min;
            Timing_sec = req.Timing_sec;
            ChiefFios = req.ChiefFios;
            ChiefPositions = req.ChiefPositions;
            AthorsFios = req.AthorsFios;
            AgeСategory = req.AgeСategory;
            PartyCount = req.PartyCount;
            Addr = req.Addr;
            Addr1 = req.Addr1;
            CheckedAdmin = req.CheckedAdmin;
            Points = req.Points;
            Schools = req.Schools;
            ClassRooms = req.ClassRooms;
            ProtocolFileDoc = req.ProtocolFileDoc;
            Fios_1 = req.Fios_1;
            Agies_1 = req.Agies_1;
            Schools_1 = req.Schools_1;
            ClassRooms_1 = req.ClassRooms_1;
            Weights_1 = req.Weights_1;
            IsApply = req.IsApply;
            DateApply = req.DateApply;
            Division = req.Division;
        }

        public static CompetitionRequest CopyFromArchReq(CompetitionRequest_Arch req)
        {
            var reqNew = new CompetitionRequest();
            reqNew.Id = req.OldId;
            reqNew.Fio = req.Fio;
            reqNew.Class_ = req.Class_;
            reqNew.Age = req.Age;
            reqNew.WorkName = req.WorkName;
            reqNew.WorkComment = req.WorkComment;
            reqNew.EducationalOrganization = req.EducationalOrganization;
            reqNew.EducationalOrganizationShort = req.EducationalOrganizationShort;
            reqNew.Email = req.Email;
            reqNew.Telephone = req.Telephone;
            reqNew.District = req.District;
            reqNew.Region = req.Region;
            reqNew.Area = req.Area;
            reqNew.City = req.City;
            reqNew.ChiefFio = req.ChiefFio;
            reqNew.ChiefPosition = req.ChiefPosition;
            reqNew.ChiefEmail = req.ChiefEmail;
            reqNew.ChiefTelephone = req.ChiefTelephone;
            reqNew.SubsectionName = req.SubsectionName;
            reqNew.Fios = req.Fios;
            reqNew.Agies = req.Agies;
            reqNew.AgeСategories = req.AgeСategories;
            reqNew.Kvalifications = req.Kvalifications;
            reqNew.Programms = req.Programms;
            reqNew.Links = req.Links;
            reqNew.CompetitionName = req.CompetitionName;
            reqNew.DateReg = req.DateReg;
            reqNew.Likes = req.Likes;
            reqNew.Nolikes = req.Nolikes;
            reqNew.Approved = req.Approved;
            reqNew.PdnProcessing = req.PdnProcessing;
            reqNew.PublicAgreement = req.PublicAgreement;
            reqNew.ProcMedicine = req.ProcMedicine;
            reqNew.SummaryLikes = req.SummaryLikes;
            reqNew.ClubsName = req.ClubsName;
            reqNew.Weight = req.Weight;
            reqNew.Result = req.Result;
            reqNew.Results = req.Results;
            reqNew.Weights = req.Weights;
            reqNew.ProtocolFile = req.ProtocolFile;
            reqNew.ProtocolPartyCount = req.ProtocolPartyCount;
            reqNew.TechnicalInfo = req.TechnicalInfo;
            reqNew.Timing_min = req.Timing_min;
            reqNew.Timing_sec = req.Timing_sec;
            reqNew.ChiefFios = req.ChiefFios;
            reqNew.ChiefPositions = req.ChiefPositions;
            reqNew.AthorsFios = req.AthorsFios;
            reqNew.AgeСategory = req.AgeСategory;
            reqNew.PartyCount = req.PartyCount;
            reqNew.Addr = req.Addr;
            reqNew.Addr1 = req.Addr1;
            reqNew.CheckedAdmin = req.CheckedAdmin;
            reqNew.Points = req.Points;
            reqNew.Schools = req.Schools;
            reqNew.ClassRooms = req.ClassRooms;
            reqNew.ProtocolFileDoc = req.ProtocolFileDoc;
            reqNew.Fios_1 = req.Fios_1;
            reqNew.Agies_1 = req.Agies_1;
            reqNew.Schools_1 = req.Schools_1;
            reqNew.ClassRooms_1 = req.ClassRooms_1;
            reqNew.Weights_1 = req.Weights_1;
            reqNew.IsApply = req.IsApply;
            reqNew.DateApply = req.DateApply;
            reqNew.Division = req.Division;
            return reqNew;
        }

        #endregion
        //#region Метод GetFiosStr(string separator = " ")

        ///// <summary>Метод преобразует список ФИО участников в строку, используя нужный разделитель</summary>
        ///// <param name="separator">разделитель</param>
        ///// <returns>возвращает пустую строку, если список пуст</returns>
        //public string GetFiosStr(string separator = " ")
        //{
        //    StringBuilder result = new StringBuilder();
        //    try
        //    {
        //        if (Fios != "" && Fio == "")
        //        {
        //            string[] arr = Fios.Split(new[] { '|' });
        //            for (int i = 0; i < arr.Length; i++)
        //            {
        //                if (i == 0)
        //                {
        //                    result.Append(arr[i]);
        //                }
        //                else
        //                {
        //                    result.Append(separator + arr[i]);
        //                }
        //            }
        //        }
        //        else if (Fios == "" && Fio != "")
        //        {
        //            result.Append(Fio + separator);
        //        }
        //        else if (Fios != "" && Fio != "")
        //        {
        //            result.Append(Fio);

        //            string[] arr = Fios.Split(new[] { '|' });
        //            for (int i = 0; i < arr.Length; i++)
        //            {
        //                if (i == 0)
        //                {
        //                    result.Append(arr[i]);
        //                }
        //                else
        //                {
        //                    result.Append(separator + arr[i]);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
        //    }
        //    return result.ToString();
        //}

        //#endregion
        //#region Метод GetAuthorsStr(string separator = " ")

        ///// <summary>Метод преобразует список ФИО авторов в строку, используя нужный разделитель</summary>
        ///// <param name="separator">разделитель</param>
        ///// <returns>возвращает пустую строку, если список пуст</returns>
        //public string GetAuthorsStr(string separator = " ")
        //{
        //    StringBuilder result = new StringBuilder();
        //    try
        //    {
        //        if (AthorsFios.Count > 0)
        //        {
        //            for (int i = 0; i < AthorsFios.Count; i++)
        //            {
        //                if (i == 0)
        //                {
        //                    result.Append(AthorsFios[i]);
        //                }
        //                else
        //                {
        //                    result.Append(separator + AthorsFios[i]);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
        //    }
        //    return result.ToString();
        //}

        //#endregion
        //#region Метод GetFormAuthorsStr(string separator = " ")

        ///// <summary>Метод преобразует список ФИО авторов в строку, используя нужный разделитель. ФИО в формате Фамилия И.О.</summary>
        ///// <param name="separator">разделитель</param>
        ///// <returns>возвращает пустую строку, если список пуст</returns>
        //public string GetFormAuthorsStr(string separator = " ")
        //{
        //    StringBuilder result = new StringBuilder();

        //    try
        //    {
        //        if (AthorsFios.Count > 0)
        //        {
        //            for (int i = 0; i < AthorsFios.Count; i++)
        //            {
        //                if (i == 0)
        //                {
        //                    result.Append(CompetitionsWork.FioReduce(AthorsFios[i]).Trim());
        //                }
        //                else
        //                {
        //                    result.Append(separator + CompetitionsWork.FioReduce(AthorsFios[i]).Trim());
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
        //    }

        //    return result.ToString();
        //}

        //#endregion
        //#region Метод GetChiefFiosAndPositionsStr(string separator = " ", bool needPosition = true)

        ///// <summary>Метод преобразует списки ФИО и должностей педагогов в строку, используя нужный разделитель</summary>
        ///// <param name="separator">разделитель</param>
        ///// <returns>возвращает пустую строку, если списки пусты</returns>
        //public string GetChiefFiosAndPositionsStr(string separator = " ", bool needPosition = true)
        //{
        //    StringBuilder result = new StringBuilder();

        //    try
        //    {
        //        if (ChiefFios.Count > 0)
        //        {
        //            for (int i = 0; i < ChiefFios.Count; i++)
        //            {
        //                if (i == 0)
        //                {
        //                    result.Append(ChiefFios[i] + (needPosition ? (i < ChiefPositions.Count() ? " / " + ChiefPositions[i] : "") : ""));
        //                }
        //                else
        //                {
        //                    result.Append(separator + ChiefFios[i] + (needPosition ? (i < ChiefPositions.Count() ? " / " + ChiefPositions[i] : "") : ""));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
        //    }

        //    return result.ToString();
        //}

        //#endregion
        //#region Метод GetFormChiefPositionsAndFios(string separator = " ")

        ///// <summary>Метод преобразует списки всех ФИО и должностей педагогов в строку вида 'ДОЛЖНОСТЬ Фамилия И.О.', используя нужный разделитель</summary>
        ///// <param name="separator">разделитель</param>
        ///// <returns>возвращает пустую строку, если списки пусты</returns>
        //public string GetFormChiefPositionsAndFios(string separator = " ")
        //{
        //    StringBuilder result = new StringBuilder();

        //    try
        //    {
        //        if (ChiefFio.Trim() != "" && ChiefFio.Trim().ToUpper() != "НЕТ")
        //        {
        //            result.Append((ChiefPosition.Trim() + " " + CompetitionsWork.FioReduce(ChiefFio)).Trim());
        //        }

        //        if (ChiefFios.Count > 0)
        //        {
        //            for (int i = 0; i < ChiefFios.Count; i++)
        //            {
        //                if (result.ToString() != "" && i == 0)          //если уже был добавлен Педагог и это первый элемент списка
        //                {
        //                    result.Append(separator + ChiefPositions[i].Trim() + " " + CompetitionsWork.FioReduce(ChiefFios[i]).Trim());
        //                }
        //                else if (result.ToString() == "" && i == 0)     //если не был добавлен Педагог и это первый элемент списка
        //                {
        //                    result.Append(ChiefPositions[i].Trim() + " " + CompetitionsWork.FioReduce(ChiefFios[i]).Trim());
        //                }
        //                else                                            //в иных случаях
        //                {
        //                    result.Append(separator + ChiefPositions[i].Trim() + " " + CompetitionsWork.FioReduce(ChiefFios[i]).Trim());
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
        //    }

        //    return result.ToString();
        //}

        //#endregion
        //#region Метод GetFioBirthAgecatWeightKvalifProgamResult_All()

        ///// <summary>Метод возвращает список массивов формы - 
        ///// 'ФИО участника','Дата рождения','Возрастная катег-я','Весовая катег-я','Квалификация','Программа(ы через запятую) выступления', 'Результат'</summary>
        ///// <returns></returns>
        //public List<string[]> GetFioBirthAgecatWeightKvalifProgamResult_All()
        //{
        //    List<string[]> result = new List<string[]>();
        //    string[] arr = new string[] { };

        //    try
        //    {
        //        #region Код

        //        string[] fiosArr = Fios.Split(new[] { '|' });
        //        string[] agiesArr = Agies.Split(new[] { '|' });
        //        string[] ageCatArr = AgeСategories.Split(new[] { '|' });
        //        string[] weightCatArr = Weights.Split(new[] { '|' });
        //        string[] kvalifArr = Kvalifications.Split(new[] { '|' });
        //        string[] programArr = Programms.Split(new[] { '|' }).Select(a => a.Replace("/", ", ")).ToArray();
        //        string[] resultArr = Results.Split(new[] { '|' });

        //        for (int i = 0; i < fiosArr.Length; i++)
        //        {
        //            arr = new string[] { fiosArr[i], agiesArr[i], ageCatArr[i], weightCatArr[i], kvalifArr[i], programArr[i], resultArr[i] };
        //            result.Add(arr);
        //        }

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        #region Код

        //        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());

        //        #endregion
        //    }

        //    return result;
        //}

        //#endregion
        //#region Метод GetFioBirthAgecatWeightKvalifProgamResult_One(.)

        ///// <summary>Метод возвращает массив по форме - 
        ///// 'ФИО участника','Дата рождения','Возрастная катег-я','Весовая катег-я','Квалификация','Программа(ы через запятую) выступления', 'Результат'</summary>
        ///// <param name="fio">ФИО участника, по которому нужно вернуть массив данных</param>
        ///// <returns></returns>
        //public string[] GetFioBirthAgecatWeightKvalifProgamResult_One(string fio)
        //{
        //    string[] result = new string[] { };

        //    try
        //    {
        //        #region Код

        //        string[] fiosArr = Fios.Split(new[] { '|' });
        //        string[] agiesArr = Agies.Split(new[] { '|' });
        //        string[] ageCatArr = AgeСategories.Split(new[] { '|' });
        //        string[] weightCatArr = Weights.Split(new[] { '|' });
        //        string[] kvalifArr = Kvalifications.Split(new[] { '|' });
        //        string[] programArr = Programms.Split(new[] { '|' }).Select(a => a.Replace("/", ", ")).ToArray();
        //        string[] resultArr = Results.Split(new[] { '|' });

        //        for (int i = 0; i < fiosArr.Length; i++)
        //        {
        //            if (fiosArr[i] == fio)
        //            {
        //                result = new string[] { fiosArr[i], agiesArr[i], ageCatArr[i], weightCatArr[i], kvalifArr[i], programArr[i], resultArr[i] };
        //                break;
        //            }
        //        }

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        #region Код

        //        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());

        //        #endregion
        //    }

        //    return result;
        //}

        //#endregion
        //#region Метод GetFioBirthAgecatWeightKvalifProgamResult_One(.)

        ///// <summary>Метод возвращает массив по форме - 
        ///// 'ФИО участника','Дата рождения','Возрастная катег-я','Весовая катег-я','Квалификация','Программа(ы через запятую) выступления', 'Результат'</summary>
        ///// <param name="index">индекс фио участника в форматированном списке Fios</param>
        ///// <returns></returns>
        //public string[] GetFioBirthAgecatWeightKvalifProgamResult_One(int index)
        //{
        //    string[] result = new string[] { };

        //    try
        //    {
        //        #region Код

        //        string[] fiosArr = Fios.Split(new[] { '|' });
        //        string[] agiesArr = Agies.Split(new[] { '|' });
        //        string[] ageCatArr = AgeСategories.Split(new[] { '|' });
        //        string[] weightCatArr = Weights.Split(new[] { '|' });
        //        string[] kvalifArr = Kvalifications.Split(new[] { '|' });
        //        string[] programArr = Programms.Split(new[] { '|' }).Select(a => a.Replace("/", ", ")).ToArray();
        //        string[] resultArr = Results.Split(new[] { '|' });

        //        for (int i = 0; i < fiosArr.Length; i++)
        //        {
        //            if (i == index)
        //            {
        //                result = new string[] { fiosArr[i], agiesArr[i], ageCatArr[i], weightCatArr[i], kvalifArr[i], programArr[i], resultArr[i] };
        //                break;
        //            }
        //        }

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        #region Код

        //        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());

        //        #endregion
        //    }

        //    return result;
        //}

        //#endregion
        //#region Метод AddParty(.......)

        ///// <summary>Метод добавляет расширенные данные по одному участнику. 
        ///// Значения, передаваемые в этот метод должны быть предварительно проверены и сформированы</summary>
        ///// <param name="fio">Фамилия Имя Отчество</param>
        ///// <param name="birthday">Дата рождения - ДД.ММ.ГГГГ</param>
        ///// <param name="ageCat">возрастная категория</param>
        ///// <param name="weight">весования категория</param>
        ///// <param name="kvalif">техническая квалификация</param>
        ///// <param name="program">программа выступления (форматированная строка вида [значение]/[значение]/..)</param>
        ///// <param name="result">результат (опционально)</param>
        //public void AddParty(string fio, string birthday, string ageCat, string weight, string kvalif, string program, string result = "")
        //{
        //    try
        //    {
        //        if (Fios != "")
        //        {
        //            Fios += "|" + fio;
        //        }
        //        else
        //        {
        //            Fios += fio;
        //        }

        //        if (Agies != "")
        //        {
        //            Agies += "|" + birthday;
        //        }
        //        else
        //        {
        //            Agies += birthday;
        //        }

        //        if (AgeСategories != "")
        //        {
        //            AgeСategories += "|" + ageCat;
        //        }
        //        else
        //        {
        //            AgeСategories += ageCat;
        //        }

        //        if (Weights != "")
        //        {
        //            Weights += "|" + weight;
        //        }
        //        else
        //        {
        //            Weights += weight;
        //        }

        //        if (Kvalifications != "")
        //        {
        //            Kvalifications += "|" + kvalif;
        //        }
        //        else
        //        {
        //            Kvalifications += kvalif;
        //        }

        //        if (Programms != "")
        //        {
        //            Programms += "|" + program;
        //        }
        //        else
        //        {
        //            Programms += program;
        //        }

        //        if (Results != "")
        //        {
        //            Results += "|" + result;
        //        }
        //        else
        //        {
        //            if (Fios.Contains("|")) Results += "|" + result;
        //            else Results += result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());
        //    }
        //}

        //#endregion
        //#region Метод DeleteParty(.)

        ///// <summary>Удаление данных по участнику из объекта. Удаляются следующие данные:
        ///// 'ФИО участника','Дата рождения','Возрастная катег-я','Весовая катег-я','Квалификация',
        ///// 'Программа(ы через запятую) выступления', 'Результат'</summary>
        ///// <param name="index">индекс удаляемых элементов данных в строках данных</param>
        //public void DeleteParty(int index)
        //{
        //    try
        //    {
        //        #region Код

        //        // Для начала удалим данные (это нужно делать сначала, потому что теоритечески именно при этих операциях может
        //        //возникнуть ошибка)

        //        List<string> listFios = Fios.Split(new[] { '|' }).ToList();
        //        listFios.RemoveAt(index);

        //        List<string> listBirth = Agies.Split(new[] { '|' }).ToList();
        //        listBirth.RemoveAt(index);

        //        List<string> listAgeCat = AgeСategories.Split(new[] { '|' }).ToList();
        //        listAgeCat.RemoveAt(index);

        //        List<string> listWeights = Weights.Split(new[] { '|' }).ToList();
        //        listWeights.RemoveAt(index);

        //        List<string> listKvalif = Kvalifications.Split(new[] { '|' }).ToList();
        //        listKvalif.RemoveAt(index);

        //        List<string> listProgms = Programms.Split(new[] { '|' }).ToList();
        //        listProgms.RemoveAt(index);

        //        List<string> listResults = Results.Split(new[] { '|' }).ToList();
        //        if (listResults.Count == 0) listResults.Add("");
        //        listResults.RemoveAt(index);

        //        // Затем сохраним новые данные в свойства Объекта

        //        Fios = "";
        //        for (int i = 0; i < listFios.Count; i++)
        //        {
        //            if (i == 0) Fios += listFios[i];
        //            else Fios += "|" + listFios[i];
        //        }

        //        Agies = "";
        //        for (int i = 0; i < listBirth.Count; i++)
        //        {
        //            if (i == 0) Agies += listBirth[i];
        //            else Agies += "|" + listBirth[i];
        //        }

        //        AgeСategories = "";
        //        for (int i = 0; i < listAgeCat.Count; i++)
        //        {
        //            if (i == 0) AgeСategories += listAgeCat[i];
        //            else AgeСategories += "|" + listAgeCat[i];
        //        }

        //        Weights = "";
        //        for (int i = 0; i < listWeights.Count; i++)
        //        {
        //            if (i == 0) Weights += listWeights[i];
        //            else Weights += "|" + listWeights[i];
        //        }

        //        Kvalifications = "";
        //        for (int i = 0; i < listKvalif.Count; i++)
        //        {
        //            if (i == 0) Kvalifications += listKvalif[i];
        //            else Kvalifications += "|" + listKvalif[i];
        //        }

        //        Programms = "";
        //        for (int i = 0; i < listProgms.Count; i++)
        //        {
        //            if (i == 0) Programms += listProgms[i];
        //            else Programms += "|" + listProgms[i];
        //        }

        //        Results = "";
        //        for (int i = 0; i < listResults.Count; i++)
        //        {
        //            if (i == 0) Results += listResults[i];
        //            else Results += "|" + listResults[i];
        //        }

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        #region Код

        //        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());

        //        #endregion
        //    }
        //}

        //#endregion
        //#region Метод UpdateFormattedStringProperty(...)

        ///// <summary>Метод заменяет значение в свойстве объекта, представляющим собой форматированную строку вида - 
        ///// [значение]|[значение]|.. Все передаваемые в данный метод значения должны быть предварительно проверены на правильность.</summary>
        ///// <param name="newValue">новое значение</param>
        ///// <param name="fieldName">имя поля, в котором нужно произвести замену 
        ///// (возможные значения - Fios, Agies, AgeСategories, Weights, Kvalifications, Programms, Results)</param>
        ///// <param name="position">позиция заменяемого значения</param>
        ///// <returns>true - в случае успешной замены, false - в случае какой-либо ошибки</returns>
        //public bool UpdateFormattedStringProperty(string newValue, string fieldName, int position)
        //{
        //    bool result = true;

        //    try
        //    {
        //        #region Код

        //        List<string> list;
        //        StringBuilder sb;

        //        if (fieldName == "Fios")
        //        {
        //            list = Fios.Split(new[] { '|' }).ToList();
        //            list[position] = newValue;
        //            sb = new StringBuilder();
        //            for (int i = 0; i < list.Count; i++)
        //            {
        //                if (i == 0) sb.Append(list[i]);
        //                else sb.Append("|" + list[i]);
        //            }
        //            Fios = sb.ToString();
        //        }
        //        else if (fieldName == "Agies")
        //        {
        //            list = Agies.Split(new[] { '|' }).ToList();
        //            list[position] = newValue;
        //            sb = new StringBuilder();
        //            for (int i = 0; i < list.Count; i++)
        //            {
        //                if (i == 0) sb.Append(list[i]);
        //                else sb.Append("|" + list[i]);
        //            }
        //            Agies = sb.ToString();
        //        }
        //        else if (fieldName == "AgeСategories")
        //        {
        //            list = AgeСategories.Split(new[] { '|' }).ToList();
        //            list[position] = newValue;
        //            sb = new StringBuilder();
        //            for (int i = 0; i < list.Count; i++)
        //            {
        //                if (i == 0) sb.Append(list[i]);
        //                else sb.Append("|" + list[i]);
        //            }
        //            AgeСategories = sb.ToString();
        //        }
        //        else if (fieldName == "Weights")
        //        {
        //            list = Weights.Split(new[] { '|' }).ToList();
        //            list[position] = newValue;
        //            sb = new StringBuilder();
        //            for (int i = 0; i < list.Count; i++)
        //            {
        //                if (i == 0) sb.Append(list[i]);
        //                else sb.Append("|" + list[i]);
        //            }
        //            Weights = sb.ToString();
        //        }
        //        else if (fieldName == "Kvalifications")
        //        {
        //            list = Kvalifications.Split(new[] { '|' }).ToList();
        //            list[position] = newValue;
        //            sb = new StringBuilder();
        //            for (int i = 0; i < list.Count; i++)
        //            {
        //                if (i == 0) sb.Append(list[i]);
        //                else sb.Append("|" + list[i]);
        //            }
        //            Kvalifications = sb.ToString();
        //        }
        //        else if (fieldName == "Programms")
        //        {
        //            list = Programms.Split(new[] { '|' }).ToList();

        //            #region Трансформация значения newValue (приходит с разделителем ',')

        //            List<string> tmpList = newValue.Split(new[] { ',' }).ToList();
        //            tmpList = tmpList.Select(a => a.Trim()).ToList();
        //            sb = new StringBuilder();
        //            for (int i = 0; i < tmpList.Count; i++)
        //            {
        //                if (i == 0) sb.Append(tmpList[i]);
        //                else sb.Append("/" + tmpList[i]);
        //            }
        //            newValue = sb.ToString();

        //            #endregion

        //            list[position] = newValue;
        //            sb = new StringBuilder();
        //            for (int i = 0; i < list.Count; i++)
        //            {
        //                if (i == 0) sb.Append(list[i]);
        //                else sb.Append("|" + list[i]);
        //            }
        //            Programms = sb.ToString();
        //        }
        //        else if (fieldName == "Results")
        //        {
        //            list = Results.Split(new[] { '|' }).ToList();
        //            if (list.Count == 0) list.Add("");
        //            list[position] = newValue;
        //            sb = new StringBuilder();
        //            for (int i = 0; i < list.Count; i++)
        //            {
        //                if (i == 0) sb.Append(list[i]);
        //                else sb.Append("|" + list[i]);
        //            }
        //            Results = sb.ToString();
        //        }

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        #region Код

        //        DebugLog.Log(ErrorEvents.err, GetType().Name, MethodBase.GetCurrentMethod().Name, "Текст исключения: " + ex.Message + ". Строка: " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber() + ". Id заявки: " + Id.ToString());

        //        #endregion
        //    }

        //    return result;
        //}

        //#endregion
    }

    #endregion


    /*
    #region Класс EducationOrganizations

    /// <summary>Класс представляет структуру данных по организациям с привязкой к региону, областям, городам </summary>
    [Serializable]
    public class EducationOrganizations
    {
        #region Поля

        private string name = "";                        //Наименование
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string region = "";                        //Регион
        public string Region
        {
            get { return region; }
            set { region = value; }
        }
        private string area = "";                        //Область
        public string Area
        {
            get { return area; }
            set { area = value; }
        }
        private string city = "";                        //Город
        public string City
        {
            get { return city; }
            set { city = value; }
        }
        private string typeName = "";                        //Город
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        #endregion

    }

    #endregion

    #region Класс RegionAreaCities

    /// <summary>Класс представляет структуру данных по региону, областям, городам</summary>
    [Serializable]
    public class RegionAreaCities
    {
        #region Поля

        private string region = "";                        //Регион
        public string Region
        {
            get { return region; }
            set { region = value; }
        }
        private string area = "";                        //Область
        public string Area
        {
            get { return area; }
            set { area = value; }
        }
        private string city = "";                        //Город
        public string City
        {
            get { return city; }
            set { city = value; }
        }
  
        #endregion

    }

    #endregion

    #region Класс Types

    /// <summary>Класс представляет структуру данных по типам организаций</summary>
    [Serializable]
    public class EducationOrganizationTypes
    {
        #region Поля

        private string name = "";                        //Наименование
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

    }

    #endregion

    */

    #region Класс ResultHelper

    /// <summary>Класс описывает структуру данных одного участника конкурса. Применяется при получении таблицы участников с кнопками скачивания сертификатов.</summary>
    public class ResultHelper
    {
        #region Fio

        private string fio = "";
        /// <summary>ФИО полностью</summary>
        public string Fio
        {
            get { return fio; }
            set { fio = value; }
        }

        #endregion
        #region ReqId

        private string reqId = "";
        /// <summary>id заявки</summary>
        public string ReqId
        {
            get { return reqId; }
            set { reqId = value; }
        }

        #endregion
        #region Diplom

        private bool diplom = false;
        /// <summary>true - участник дипломирован, false - участник не дипломирован</summary>
        public bool Diplom
        {
            get { return diplom; }
            set { diplom = value; }
        }

        #endregion
        #region Certificate

        private bool certificate = false;
        /// <summary>true - нужно печатать сертификаты для первого тура, false - не нужно печатать сертификаты для первого тура</summary>
        public bool Certificate
        {
            get { return certificate; }
            set { certificate = value; }
        }


        private int countParticipants = 0;
        /// <summary>
        /// Кол-во участников
        /// </summary>
        public int CountParticipants
        {
            get { return countParticipants; }
            set { countParticipants = value; }
        }
        #endregion
    }

    #endregion

    #region Класс UniObjParams

    /// <summary>Класс описывает структуру данных универсального объекта для передачи параметров в другой поток</summary>
    public class UniObjParams
    {
        #region obj1

        private object _obj1;
        /// <summary>какой-то объект с данными</summary>
        public object obj1
        {
            get { return _obj1; }
            set { _obj1 = value; }
        }

        #endregion
        #region obj2
        private object _obj2;
        /// <summary>какой-то объект с данными</summary>
        public object obj2
        {
            get { return _obj2; }
            set { _obj2 = value; }
        }
        #endregion
        #region obj3
        private object _obj3;
        /// <summary>какой-то объект с данными</summary>
        public object obj3
        {
            get { return _obj3; }
            set { _obj3 = value; }
        }
        #endregion
        #region obj4
        private object _obj4;
        /// <summary>какой-то объект с данными</summary>
        public object obj4
        {
            get { return _obj4; }
            set { _obj4 = value; }
        }
        #endregion
        #region obj5
        private object _obj5;
        /// <summary>какой-то объект с данными</summary>
        public object obj5
        {
            get { return _obj5; }
            set { _obj5 = value; }
        }
        #endregion
        #region obj6
        private object _obj6;
        /// <summary>какой-то объект с данными</summary>
        public object obj6
        {
            get { return _obj6; }
            set { _obj6 = value; }
        }
        #endregion
        #region obj7
        private object _obj7;
        /// <summary>какой-то объект с данными</summary>
        public object obj7
        {
            get { return _obj7; }
            set { _obj7 = value; }
        }
        #endregion
        #region obj8
        private object _obj8;
        /// <summary>какой-то объект с данными</summary>
        public object obj8
        {
            get { return _obj8; }
            set { _obj8 = value; }
        }
        #endregion
        #region obj9
        private object _obj9;
        /// <summary>какой-то объект с данными</summary>
        public object obj9
        {
            get { return _obj9; }
            set { _obj9 = value; }
        }
        #endregion
    }

    #endregion

    #region Класс UniObj

    /// <summary>Класс описывает структуру данных, применяемую например для сортировки по полю Num</summary>
    public class UniObj
    {
        #region Num

        private int num = 0;
        public int Num
        {
            get { return num; }
            set { num = value; }
        }

        #endregion
        #region Str

        private string str = "";
        public string Str
        {
            get { return str; }
            set { str = value; }
        }

        #endregion
    }

    #endregion

    #region PersonPair
    public class PersonPair
    {
        private string name = "";
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string position = "";
        public string Position
        {
            get { return position; }
            set { position = value; }
        }
        private string age = "";
        public string Age
        {
            get { return age; }
            set { age = value; }
        }
        private string school = "";
        public string School
        {
            get { return school; }
            set { school = value; }
        }
        private string classroom = "";
        public string Classroom
        {
            get { return classroom; }
            set { classroom = value; }
        }
        private string participantRound = "";
        public string ParticipantRound
        {
            get { return participantRound; }
            set { participantRound = value; }

        }
    }
    #endregion

    #endregion
}