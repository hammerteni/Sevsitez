using System;
using site.classes;
using site.classesHelp;
using Secure;

namespace site.adm
{
    /// <summary>страница авторизации в консоль управления</summary>
    public partial class _as : System.Web.UI.Page
    {
        #region Описание некоторых сессионных переменных

        //Session["tempNewsStruct"] - содержит структуру данных одной новости (NewsStruct). Инициализируется в классе NewsWork. Переменная нужна для хранения информации о новости во время её добавления или редактирования
        //Session["TempAccount"] - содержит структуру данных одной новости (AdmPersonStruct). Инициализируется в классе UsersForm. Переменная нужна для хранения информации об одной учётной записи во время её добавления или редактирования
        //Session["tempRowsList"] - содержит список структур данных строк телефонного справочника(RefRowStruct). Инициализируется в классе ReferenceForm. Переменная нужна для хранения списка строк справочника во время его редактирования.
        //Session["tempOneRowStruct"] - содержит структуру данных одной строки телефонного справочника(RefRowStruct). Инициализируется в классе ReferenceForm. Переменная нужна для хранения структуры строки справочника во время её редактирования.
        //Session["inverseSrch"] - содержит булево значение, которое показывает на инверсный поиск. Если оно равно true, то фильтр по событиям работает наоборот, отыскивая всё, кроме указанного в поисковой строке (переменная - Session["srchStrEvent"])

        #endregion

        #region События страницы

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ActionValidator.Initialize(base.IsPostBack);
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {

        }
        protected void Page_Init(object sender, EventArgs e)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            #region COOKIE

            CookieSession cookie = new CookieSession();
            cookie.AuthACheck(this);

            #endregion
            
            if (Session["authperson"] == null)
            {
                PageFill();
            }
            else
            {
                if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.admin) || 
                    ((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.pagesEditor))
                {
                    Response.Redirect("~/adm/pageseditor.aspx");
                }
                else if (((AdmPersonStruct)Session["authperson"]).Writes == EnumsHelper.GetWritesValue(Writes.newsEditor))
                {
                    Response.Redirect("~/adm/news.aspx");
                }
                else if (EnumsHelper.GetWritesCodeFromValue(((AdmPersonStruct)Session["authperson"]).Writes) != "")
                {
                    Response.Redirect("~/adm/competitions.aspx");
                }
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {

        }

        #endregion

        /// <summary>метод наполнения страницы</summary>
        private void PageFill()
        {
            var usersForm = new UsersForm(this);
            divWindowContent.Controls.Add(usersForm.GetAdmLoginPanel());
        }
    }
}