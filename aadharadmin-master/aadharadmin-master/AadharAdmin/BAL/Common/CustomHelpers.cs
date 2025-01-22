namespace System.Web.Mvc.Html
{
    public static class CustomHelpers
    {
        public static MvcHtmlString Securitykey
 (this HtmlHelper helper, string name, object htmlAttributes = null)
        {
            string propName = string.Empty;
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var builder = new TagBuilder("input");
            //var text2 = text.ToString(); 
            if (htmlAttributes != null)
                builder.MergeAttributes(attributes);
            propName = Guid.NewGuid().ToString().Replace("-", "");
            HttpContext.Current.Session["Securitykey"] = propName;
            builder.Attributes.Add("type", "hidden");
            if (!attributes.ContainsKey("value"))
            {
                builder.Attributes.Add("value", propName);
            }
            builder.Attributes.Add("name", name);
            builder.Attributes.Add("id", name);
            return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}