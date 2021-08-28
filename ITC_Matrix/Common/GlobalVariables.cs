using System.Configuration;

/// <summary>
///  This is common classes namespace 
/// </summary>
namespace ITC_Matrix.Common
{
    /// <summary>
    /// Summary description for GlobalValues
    /// </summary>
    public class GlobalVariables
    {
        #region --Class Variables--
       
        public static string DateFormat = ConfigurationManager.AppSettings["DateFormat"];
        public static string templateFile = ConfigurationManager.AppSettings["templateFile"];
        public static string ThemeFolder = ConfigurationManager.AppSettings["ThemeFolder"];
        public static string defaultThemeFile = ConfigurationManager.AppSettings["defaultThemeFile"];
        public static string defaultImagePath = ConfigurationManager.AppSettings["defaultImagePath"];
        public static string defaultCSSPath = ConfigurationManager.AppSettings["defaultCSSPath"];
        public static string Themes = ConfigurationManager.AppSettings["Themes"];
        public static string ConnectionSetting = ConfigurationManager.AppSettings["ConnectionSetting"];
        public static string reportsPath = ConfigurationManager.AppSettings["reportsPath"];

        #endregion

        #region --Class Constructor--
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalVariables"/> class.
        /// </summary>
        public GlobalVariables()
        {
            if (ConfigurationManager.AppSettings["DateFormat"] != null)
            {
                DateFormat = ConfigurationManager.AppSettings["DateFormat"];
            }
            else
            {
                DateFormat = "yyyy/MM/dd";
            }

            if (ConfigurationManager.AppSettings["templateFile"] != null)
            {
                templateFile = ConfigurationManager.AppSettings["templateFile"];
            }
            else
            {
                templateFile = "Template.css";
            }

            if (ConfigurationManager.AppSettings["defaultThemeFile"] != null)
            {
                defaultThemeFile = ConfigurationManager.AppSettings["defaultThemeFile"];
            }
            else
            {
                defaultThemeFile = "default-theme.css";
            }

            if (ConfigurationManager.AppSettings["ThemeFolder"] != null)
            {
                ThemeFolder = ConfigurationManager.AppSettings["ThemeFolder"];
            }
            else
            {
                ThemeFolder = "../../_themes/";
            }

            if (ConfigurationManager.AppSettings["defaultImagePath"] != null)
            {
                defaultImagePath = ConfigurationManager.AppSettings["defaultImagePath"];
            }
            else
            {
                defaultImagePath = "/Assets/images/";
            }

            if (ConfigurationManager.AppSettings["defaultCSSPath"] != null)
            {
                defaultCSSPath = ConfigurationManager.AppSettings["defaultCSSPath"];
            }
            else
            {
                defaultCSSPath = "~/Assets/CSS/";
            }

            if (ConfigurationManager.AppSettings["Themes"] != null)
            {
                Themes = ConfigurationManager.AppSettings["Themes"];
            }
            else
            {
                Themes = "~/_themes/";
            }

            if (ConfigurationManager.AppSettings["ConnectionSetting"] != null)
            {
                ConnectionSetting = ConfigurationManager.AppSettings["ConnectionSetting"];
            }
            else
            {
                ConnectionSetting = "~/Assets/Install/Install.txt";
            }

            if (ConfigurationManager.AppSettings["reportsPath"] != null)
            {
                reportsPath = ConfigurationManager.AppSettings["reportsPath"];
            }
            else
            {
                reportsPath = "http://localhost:81/";
            }
        }

        #endregion        
    }
}