using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using site.classes;
//using site.classesAdm;

namespace site.classesHelp
{
    /// <summary>Класс предназначен для проверки и установки данных авторизации и данных по заказу, хранящемуся в сессионной переменной или в куках по заказу</summary>
    public class CookieSession
    {
        //private int authExpiration = 6;     //время хранения кук аутентификации в браузере (месяцы)
        //private int trashExpiration = 6;    //время хранения кук корзины в браузере (месяцы)
        private int authAExpiration = 1;         //время хранения кук авторизации в консоли (дни)
        private int votingExpiration = 12;         //время хранения кук голосования (месяцы)

        /// <summary>Метод проверяет наличие сессионной переменной с данными авторизации. Если такой переменной нет, то она создаётся в случае нахождения данных авторизации
        /// в куках браузера.</summary>
        /// <param name="pag">объект страницы</param>
        public void AuthCheck(Page pag)
        {
            /*EncDecClass encdec = new EncDecClass();
            if (pag.Session["loginFizStruct"] != null)          //если в сессии существует авторизационная информация, то используем её
            {
                ((LinkButton)pag.Master.FindControl("ctl00$lBtnToPersonCab")).Text = ((UserFizStruct)pag.Session["loginFizStruct"]).Fam + " " +
                                                                                 ((UserFizStruct)pag.Session["loginFizStruct"]).Name + " " +
                                                                                 ((UserFizStruct)pag.Session["loginFizStruct"]).Otch;
                ((LinkButton)pag.Master.FindControl("ctl00$lBtnToPersonCab")).Visible = true;
                ((LinkButton)pag.Master.FindControl("ctl00$lBtnLoginLogout")).Text = "Выйти";
            }
            else                                            //если нет, то обращаемся к КУКАМ..
            {
                if (pag.Request.Cookies["auth"] != null) //если куки авторизации найдены, то..
                {
                    var ubwork = new UsersWorkClass(null);
                    pag.Session["loginFizStruct"] = ubwork.GetFizStructFromMail(encdec.Dectext(pag.Server.HtmlEncode(pag.Request.Cookies["auth"].Value)));
                    if (((UserFizStruct)pag.Session["loginFizStruct"]).ID != -1)   //если для такого mail существует учётка, то.. 
                    {
                        ((LinkButton)pag.Master.FindControl("ctl00$lBtnToPersonCab")).Text = ((UserFizStruct)pag.Session["loginFizStruct"]).Fam + " " +
                                                                                         ((UserFizStruct)pag.Session["loginFizStruct"]).Name + " " +
                                                                                         ((UserFizStruct)pag.Session["loginFizStruct"]).Otch;
                        ((LinkButton)pag.Master.FindControl("ctl00$lBtnToPersonCab")).Visible = true;
                        ((LinkButton)pag.Master.FindControl("ctl00$lBtnLoginLogout")).Text = "Выйти";
                    }
                    else                                                        //если нет, то..
                    {
                        ((LinkButton)pag.Master.FindControl("ctl00$lBtnToPersonCab")).Text = "";
                        ((LinkButton)pag.Master.FindControl("ctl00$lBtnToPersonCab")).Visible = false;
                    }
                }
                else                    //если нет, то..
                {
                    ((LinkButton)pag.Master.FindControl("ctl00$lBtnToPersonCab")).Text = "";
                    ((LinkButton)pag.Master.FindControl("ctl00$lBtnToPersonCab")).Visible = false;
                }
            }*/
        }

        /// <summary>Метод проверяет наличие сессионной переменной с данными авторизации (метод для страниц личного кабинета). Если такой переменной нет, 
        /// то она создаётся в случае нахождения данных авторизации в куках браузера.</summary>
        /// <param name="pag">объект страницы</param>
        public void AuthCheckAcc(Page pag)
        {
            /*EncDecClass encdec = new EncDecClass();
            if (pag.Session["loginFizStruct"] != null)          //если в сессии существует авторизационная информация, то используем её
            {
                ((LinkButton)pag.Master.Master.FindControl("ctl00$lBtnToPersonCab")).Text = ((UserFizStruct)pag.Session["loginFizStruct"]).Fam + " " +
                                                                                 ((UserFizStruct)pag.Session["loginFizStruct"]).Name + " " +
                                                                                 ((UserFizStruct)pag.Session["loginFizStruct"]).Otch;
                ((LinkButton)pag.Master.Master.FindControl("ctl00$lBtnToPersonCab")).Visible = true;
                ((LinkButton)pag.Master.Master.FindControl("ctl00$lBtnLoginLogout")).Text = "Выйти";
            }
            else                                            //если нет, то обращаемся к КУКАМ..
            {
                if (pag.Request.Cookies["auth"] != null) //если куки авторизации найдены, то..
                {
                    var ubwork = new UsersWorkClass(null);
                    pag.Session["loginFizStruct"] = ubwork.GetFizStructFromMail(encdec.Dectext(pag.Server.HtmlEncode(pag.Request.Cookies["auth"].Value)));
                    if (((UserFizStruct)pag.Session["loginFizStruct"]).ID != -1)   //если для такого mail существует учётка, то.. 
                    {
                        ((LinkButton)pag.Master.Master.FindControl("ctl00$lBtnToPersonCab")).Text = ((UserFizStruct)pag.Session["loginFizStruct"]).Fam + " " +
                                                                                         ((UserFizStruct)pag.Session["loginFizStruct"]).Name + " " +
                                                                                         ((UserFizStruct)pag.Session["loginFizStruct"]).Otch;
                        ((LinkButton)pag.Master.Master.FindControl("ctl00$lBtnToPersonCab")).Visible = true;
                        ((LinkButton)pag.Master.Master.FindControl("ctl00$lBtnLoginLogout")).Text = "Выйти";
                    }
                    else                                                        //если нет, то..
                    {
                        ((LinkButton)pag.Master.Master.FindControl("ctl00$lBtnToPersonCab")).Text = "";
                        ((LinkButton)pag.Master.Master.FindControl("ctl00$lBtnToPersonCab")).Visible = false;
                    }
                }
                else                    //если нет, то..
                {
                    ((LinkButton)pag.Master.Master.FindControl("ctl00$lBtnToPersonCab")).Text = "";
                    ((LinkButton)pag.Master.Master.FindControl("ctl00$lBtnToPersonCab")).Visible = false;
                }
            }*/
        }

        /// <summary>Создать в браузере набор кук авторизации</summary>
        /// <param name="mail">в эту переменную передаётся почтовый адрес</param>
        public void AuthAdd(string mail)
        {
            /*EncDecClass encdec = new EncDecClass();
            if (HttpContext.Current.Request.Cookies["auth"] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["auth"];
                cookie.Value = encdec.Enctext(mail);
                cookie.Expires = DateTime.Now.AddMonths(authExpiration);
                //cookie.Domain = HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            else
            {
                HttpCookie authCookie = new HttpCookie("auth");
                authCookie.Value = encdec.Enctext(mail);
                authCookie.Expires = DateTime.Now.AddMonths(authExpiration);
                //authCookie.Domain = HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
                HttpContext.Current.Response.Cookies.Add(authCookie);
            }*/
        }

        /// <summary>Удалить из браузера набор кук авторизации</summary>
        public void AuthDel()
        {
            /*if (HttpContext.Current.Request.Cookies["auth"] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["auth"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }*/
        }



        /// <summary>Метод проверяет наличие сессионной переменной с данными авторизации в консоли. Если такой переменной нет, то она создаётся в случае нахождения данных авторизации
        /// в куках браузера.</summary>
        /// <param name="pag">объект страницы</param>
        public void AuthACheck(Page pag)
        {
            if (pag.Session["authperson"] == null)          //если в сессии не существует данных об авторизации, то проверяем куки
            {
                EncDecClass encdec = new EncDecClass();
                var loginwork = new LoginWorkClass(pag);
                if (pag.Request.Cookies["autha"] != null) //если куки авторизации найдены, то..
                {
                    HttpCookie cookie = pag.Request.Cookies["autha"];
                    string l = encdec.Dectext(cookie["l"]);
                    string p = encdec.Dectext(cookie["p"]);
                    loginwork.CheckAuth(l, p);  //если данные авторизации верны, то эта же функция заполнит сессионную переменную Session["authperson"]
                }
            }
        }

        /// <summary>Создать в браузере набор кук авторизации для консоли</summary>
        /// <param name="l">логин</param>
        /// <param name="p">пароль</param>
        public void AuthAAdd(string l, string p)
        {
            EncDecClass encdec = new EncDecClass();
            if (HttpContext.Current.Request.Cookies["autha"] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["autha"];
                cookie["l"] = encdec.Enctext(l);
                cookie["p"] = encdec.Enctext(p);
                cookie.Path = "/; SameSite=None; Secure";
                cookie.Expires = DateTime.Now.AddDays(authAExpiration);
                HttpContext.Current.Response.SetCookie(cookie);
            }
            else
            {
                HttpCookie authCookie = new HttpCookie("autha");
                authCookie["l"] = encdec.Enctext(l);
                authCookie["p"] = encdec.Enctext(p);
                authCookie.Path = "/; SameSite=None; Secure";
                authCookie.Expires = DateTime.Now.AddDays(authAExpiration);
                HttpContext.Current.Response.SetCookie(authCookie);
            }
        }

        /// <summary>Удалить из браузера набор кук авторизации для консоли</summary>
        public void AuthADel()
        {
            if (HttpContext.Current != null) {
                if (HttpContext.Current.Response.Cookies["autha"] != null)
                {
                    //HttpCookie cookie = HttpContext.Current.Request.Cookies["autha"];
                    //cookie.Expires = DateTime.Now.AddDays(-1);
                    //HttpContext.Current.Response.SetCookie(cookie);
                    HttpContext.Current.Response.Cookies["autha"].Expires = DateTime.Now.AddDays(-1);
                }
                //HttpContext.Current.Request.Cookies.Clear();
            }
        }


        /// <summary>Метод проверяет наличие сессионной переменной с данными авторизации. Если такой переменной нет, то она создаётся в случае нахождения данных авторизации
        /// в куках браузера.</summary>
        /// <param name="pag">объект страницы</param>
        public void TrashCheck(Page pag)
        {
            /*if (pag.Session["OrderList"] != null)           //если существует сессионная переменная
            {
                var orderWork = new OrderWorkClass(null);
                ((Label)pag.Master.FindControl("ctl00$countInTrash")).Text = ((List<ProductStruct>)pag.Session["OrderList"]).Count.ToString();
                ((Label)pag.Master.FindControl("ctl00$sumInTrash")).Text = orderWork.GetSummFromTrashList();
            }
            else                                            //если нет, то обращаемся к КУКАМ..
            {
                EncDecClass encdec = new EncDecClass();
                if (pag.Request.Cookies["trash"] != null)   //если куки содержимого корзины найдены, то..
                {
                    var orderWork = new OrderWorkClass(pag.Master);
                    HttpCookie cookie = pag.Request.Cookies["trash"];
                    string[] strSplit;
                    for (int i = 0; i < cookie.Values.Count; i++)
                    {
                        strSplit = cookie["prod" + i].Split(new[] { '|' });
                        //артикул|кол-во|выбранный размер|цена
                        orderWork.AddProductToTrashList(strSplit[0], int.Parse(strSplit[1]), encdec.Dectext(strSplit[2]), int.Parse(strSplit[3]));
                    }
                    ((Label)pag.Master.FindControl("ctl00$countInTrash")).Text = ((List<ProductStruct>)pag.Session["OrderList"]).Count.ToString();
                    ((Label)pag.Master.FindControl("ctl00$sumInTrash")).Text = orderWork.GetSummFromTrashList();
                }
                else                                        //если нет, то..
                {
                    ((Label)pag.Master.FindControl("ctl00$countInTrash")).Text = "0";
                    ((Label)pag.Master.FindControl("ctl00$sumInTrash")).Text = "0";
                }
            }*/
        }

        /// <summary>Метод проверяет наличие сессионной переменной с данными авторизации (метод для страниц личного кабинета). Если такой переменной нет, 
        /// то она создаётся в случае нахождения данных авторизации в куках браузера.</summary>
        /// <param name="pag">объект страницы</param>
        public void TrashCheckAcc(Page pag)
        {
            /*if (pag.Session["OrderList"] != null)           //если существует сессионная переменная
            {
                var orderWork = new OrderWorkClass(null);
                ((Label)pag.Master.Master.FindControl("ctl00$countInTrash")).Text = ((List<ProductStruct>)pag.Session["OrderList"]).Count.ToString();
                ((Label)pag.Master.Master.FindControl("ctl00$sumInTrash")).Text = orderWork.GetSummFromTrashList();
            }
            else                                            //если нет, то обращаемся к КУКАМ..
            {
                EncDecClass encdec = new EncDecClass();
                if (pag.Request.Cookies["trash"] != null)   //если куки содержимого корзины найдены, то..
                {
                    var orderWork = new OrderWorkClass(pag.Master);
                    HttpCookie cookie = pag.Request.Cookies["trash"];
                    string[] strSplit;
                    for (int i = 0; i < cookie.Values.Count; i++)
                    {
                        strSplit = cookie["prod" + i].Split(new[] { '|' });
                        //артикул|кол-во|выбранный размер|цена
                        orderWork.AddProductToTrashList(strSplit[0], int.Parse(strSplit[1]), encdec.Dectext(strSplit[2]), int.Parse(strSplit[3]));
                    }
                    ((Label)pag.Master.Master.FindControl("ctl00$countInTrash")).Text = ((List<ProductStruct>)pag.Session["OrderList"]).Count.ToString();
                    ((Label)pag.Master.Master.FindControl("ctl00$sumInTrash")).Text = orderWork.GetSummFromTrashList();
                }
                else                                        //если нет, то..
                {
                    ((Label)pag.Master.Master.FindControl("ctl00$countInTrash")).Text = "0";
                    ((Label)pag.Master.Master.FindControl("ctl00$sumInTrash")).Text = "0";
                }
            }*/
        }

        /// <summary>Создать в браузере набор кук корзины с товарами</summary>
        /// <param name="artikul">артикул товара</param>
        /// <param name="count">кол-во позиций товара</param>
        /// <param name="selectsize">выбранный размер товара</param>
        /// <param name="price">цена за штуку</param>
        public void TrashAdd(string artikul, string count, string selectsize, string price)
        {
            /* EncDecClass encdec = new EncDecClass();
             if (HttpContext.Current.Request.Cookies["trash"] != null)
             {
                 string key = HttpContext.Current.Request.Cookies["trash"].Values.Count.ToString();
                 HttpCookie cookie = HttpContext.Current.Request.Cookies["trash"];
                 cookie["prod" + key] = artikul + "|" + count + "|" + encdec.Enctext(selectsize) + "|" + price;      //selectsize кодируется по причине присутствия русских букв
                 cookie.Expires = DateTime.Now.AddMonths(trashExpiration);
                 //cookie.Domain = HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
                 HttpContext.Current.Response.Cookies.Add(cookie);
             }
             else
             {
                 HttpCookie trashCookie = new HttpCookie("trash");
                 trashCookie["prod0"] = artikul + "|" + count + "|" + encdec.Enctext(selectsize) + "|" + price;      //артикул|кол-во|выбранный размер|цена
                 trashCookie.Expires = DateTime.Now.AddMonths(trashExpiration);
                 //trashCookie.Domain = HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
                 HttpContext.Current.Response.Cookies.Add(trashCookie);
             }*/
        }

        /// <summary>Удалить из браузера набор кук с содержимым корзины покупок</summary>
        public void TrashDel(Page pag = null)
        {
            /*if (HttpContext.Current.Request.Cookies["trash"] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["trash"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
                ((Label)pag.Master.FindControl("ctl00$countInTrash")).Text = "0";
                ((Label)pag.Master.FindControl("ctl00$sumInTrash")).Text = "0";
            }*/
        }


        #region Методы для класса CompetitionsForm (применяются в событиях голосания за конкурсные работы)

        /// <summary>Метод возвращает true, если в браузере есть уже куки голосования по данной заявке. Дублируется это значение в сессионной переменной (List/string/)Session["voiting"]</summary>
        /// <param name="reqId">id заявки конкурсанта</param>
        /// <returns>возвращает true - если человек уже голосовал по данной заявке</returns>
        public bool VotingCookieCheck(string reqId)
        {
            if (HttpContext.Current.Request.Cookies["voting_" + reqId] == null)
            {
                return false;
            }
            else
            {
                if (HttpContext.Current.Session["voiting"] != null)     // нужно сохранить значение в сессионный список
                {
                    if (!((List<string>)HttpContext.Current.Session["voiting"]).Contains("_" + reqId + "_"))
                    {
                        ((List<string>)HttpContext.Current.Session["voiting"]).Add("_" + reqId + "_");
                    }
                }
            }
            if (HttpContext.Current.Session["voiting"] != null)     // дублирование проверки голосования в сессионной переменной
            {
                if (!((List<string>) HttpContext.Current.Session["voiting"]).Contains("_" + reqId + "_"))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Метод добавляет в браузер куки голосования по данной заявке. Дублируется это значение в сессионную переменную (List/string/)Session["voiting"]</summary>
        /// <param name="reqId">id заявки конкурсанта</param>
        public void VotingCookieAdd(string reqId)
        {
            if (HttpContext.Current.Request.Cookies["voting_" + reqId] == null)
            {
                HttpCookie cookie = new HttpCookie("voting_" + reqId,"");
                cookie.Expires = DateTime.Now.AddMonths(votingExpiration);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            if (HttpContext.Current.Session["voiting"] != null)     // дублирование сохранения голосования в сессионную переменную
            {
                ((List<string>)HttpContext.Current.Session["voiting"]).Add("_" + reqId + "_");
            }
        }

        /// <summary>Метод возвращает true, если в браузере есть уже куки голосования(итогового) по данной заявке. Дублируется это значение в сессионной переменной (List/string/)Session["voiting"]</summary>
        /// <param name="reqId">id заявки конкурсанта</param>
        /// <returns>возвращает true - если человек уже голосовал по данной заявке</returns>
        public bool VotingSumCookieCheck(string reqId)
        {
            if (HttpContext.Current.Request.Cookies["votingSum_" + reqId] == null)
            {
                return false;
            }
            else
            {
                if (HttpContext.Current.Session["voitingSum"] != null)     // нужно сохранить значение в сессионный список
                {
                    if (!((List<string>)HttpContext.Current.Session["voitingSum"]).Contains("_" + reqId + "_"))
                    {
                        ((List<string>)HttpContext.Current.Session["voitingSum"]).Add("_" + reqId + "_");
                    }
                }
            }
            if (HttpContext.Current.Session["voitingSum"] != null)     // дублирование проверки голосования в сессионной переменной
            {
                if (!((List<string>)HttpContext.Current.Session["voitingSum"]).Contains("_" + reqId + "_"))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Метод добавляет в браузер куки голосования(итогового) по данной заявке. Дублируется это значение в сессионную переменную (List/string/)Session["voiting"]</summary>
        /// <param name="reqId">id заявки конкурсанта</param>
        public void VotingSumCookieAdd(string reqId)
        {
            if (HttpContext.Current.Request.Cookies["votingSum_" + reqId] == null)
            {
                HttpCookie cookie = new HttpCookie("votingSum_" + reqId, "");
                cookie.Expires = DateTime.Now.AddMonths(votingExpiration);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            if (HttpContext.Current.Session["voitingSum"] != null)     // дублирование сохранения голосования в сессионную переменную
            {
                ((List<string>)HttpContext.Current.Session["voitingSum"]).Add("_" + reqId + "_");
            }
        }

        #endregion


        /// <summary>Функция возвращает хэш переданной в неё строки</summary>
        /// <param name="text">строка с текстом</param>
        /// <returns>возвращает хэш переданной в неё строки</returns>
        private string GetHash(string text)
        {
            byte[] hash = Encoding.ASCII.GetBytes(text);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hashenc = md5.ComputeHash(hash);
            string result = "";
            foreach (var b in hashenc)
            {
                result += b.ToString("x2");
            }
            return result;
        }
    }
}