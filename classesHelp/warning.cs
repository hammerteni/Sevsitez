using System.Web.UI;
using System.Web.UI.WebControls;

namespace site.classesHelp
{
    /// <summary>класс отвечает за включение предупреждений вверху страницы</summary>
    public class WarnClass
    {
        /// <summary>метод отображает информационную строку на мастер-странице</summary>
        /// <param name="txt">текст предупреждения(оповещения)</param>
        /// <param name="pag"></param>
        public void ShowWarning(string txt, MasterPage pag)
        {
            if (((Label)pag.FindControl("attentionContent")) != null)
                ((Label)pag.FindControl("attentionContent")).Text = txt;
            if (pag.FindControl("divWarnWrapper") != null)
                pag.FindControl("divWarnWrapper").Visible = true;
        }

        /// <summary>метод скрывает панель предупреждения на мастер-странице</summary>
        /// <param name="pag"></param>
        public void HideWarning(MasterPage pag)
        {
            if (((Label)pag.FindControl("attentionContent")) != null)
                 ((Label)pag.FindControl("attentionContent")).Text = "";
            if (pag.FindControl("divWarnWrapper") != null)
                pag.FindControl("divWarnWrapper").Visible = false;
        }

        /// <summary>метод отображает информационную строку на мастер-странице</summary>
        /// <param name="txt">текст предупреждения(оповещения)</param>
        /// <param name="pag"></param>
        public void ShowPageWarning(string txt, Page pag)
        {
            if (((Label)pag.FindControl("attentionContent")) != null)
                ((Label)pag.FindControl("attentionContent")).Text = txt;
            if (pag.FindControl("divWarnWrapper") != null)
                pag.FindControl("divWarnWrapper").Visible = true;
        }

        /// <summary>метод скрывает панель предупреждения на мастер-странице</summary>
        /// <param name="pag"></param>
        public void HidePageWarning(Page pag)
        {
            if (((Label)pag.FindControl("attentionContent")) != null)
                ((Label)pag.FindControl("attentionContent")).Text = "";
            if (pag.FindControl("divWarnWrapper") != null)
                pag.FindControl("divWarnWrapper").Visible = false;
        }
    }
}