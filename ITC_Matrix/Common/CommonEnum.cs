using System;
using System.Reflection;

namespace ITC_Matrix.Common
{
    public static class CommonEnum
    {
        #region --Inner Class--

        /// <summary>
        /// Class used for identifying string enumerations
        /// </summary>
        public class StringValue : System.Attribute
        {
            #region Class Variables

            private string _value;

            #endregion

            #region Class Properties

            public StringValue(string value)
            {
                _value = value;
            }

            public string Value
            {
                get { return _value; }
            }

            #endregion
        }

        /// <summary>
        /// Class used to get string value of enumerations
        /// </summary>
        public static class StringEnum
        {
            /// <summary>
            /// Gets the string value from enum.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            public static string GetStringValue(Enum value)
            {
                string output = null;
                Type type = value.GetType();

                FieldInfo fi = type.GetField(value.ToString());
                StringValue[] attrs = fi.GetCustomAttributes(typeof(StringValue), false) as StringValue[];

                if (attrs.Length > 0)
                {
                    output = attrs[0].Value;
                }

                return output;
            }
        }

        #endregion

        public enum SearchMethod
        {
            Code = 1,
            Description = 2,
            Given_Name = 3,
            Family_Given_Name = 4,
            Given_Name_Family_Name = 5,
            IPAddress = 6,
            Enabled = 7,
            RegisterType = 8,
            deviceTypeName = 9,
            status = 10,
            GroupDesc = 11,
            Login = 12,
            Family = 13,
            Given = 14,
            Comments = 15,
            Email = 16,
            Name = 17
        }

        public enum SearchDeletedClient
        {
            Code = 1,
            Family_Name = 2,
            Given_Name = 3,
            Family_Given_Name = 4,
            Given_Name_Family_Name = 5
        }

        // enum for week days
        public enum DayOfWeek
        {
            Sunday = 1,
            Monday = 2,
            Tuesday = 3,
            Wednesday = 4,
            Thursday = 5,
            Friday = 6,
            Saturday = 7
        }

        public enum SearchByPeriod
        {
            This_Month = 1,
            Last_Month = 2,
            All_Periods = 3
        }

        public enum MainMenu
        {
            [StringValue("Clients")]
            Clients = 1,
            [StringValue("Accounts")]
            Accounts = 2,
            [StringValue("Privileges")]
            plans = 3,
            [StringValue("Devices")]
            Device = 4,
            [StringValue("Reports")]
            Reports = 5,
            [StringValue("System Administration")]
            Sys = 6,
            [StringValue("Tools")]
            Tools = 7
        }

        public enum SubMenus
        {
            // Clients
            [StringValue("Clients")]
            clients_node = 1,
            [StringValue("Client Profiles")]
            clients_profiles = 2,
            [StringValue("Deleted Client")]
            clients_deleted = 3,
            [StringValue("Departments")]
            clients_departments = 4,
            [StringValue("Cost Centers")]
            clients_costcenters = 5,

            // Account Types
            [StringValue("Account Types")]
            accounts_types = 6,
            [StringValue("Payment Methods")]
            accounts_paymentTypes = 7,

            // Devices
            [StringValue("Devices")]
            device_node = 8,
            [StringValue("Device Groups")]
            device_groups = 9,

            // Devices
            [StringValue("Meal Plans")]
            plans_mealplans = 10,
            [StringValue("Meal Types")]
            plans_types = 11,

            // System Administration
            [StringValue("Operators")]
            sys_operators = 12,
            [StringValue("Operator Roles")]
            sys_roles = 13,
            [StringValue("Defaults")]
            sys_defaults = 14,
            [StringValue("Themes")]
            sys_themes = 15,
            [StringValue("Languages")]
            sys_languages = 16,
            [StringValue("Translations")]
            sys_translations = 17,
            [StringValue("Manage Menus")]
            sys_menus = 18,

            //Tools
            [StringValue("Tools")]
            tools_node = 19,

            //Reports
            [StringValue("Reports")]
            reports_node = 20
        }

        public enum Command
        {
            [StringValue("Set Pin")]
            SetPin = 1,
            [StringValue("Issue Card")]
            IssueCard = 2
        }

        public enum ThemeSetting
        {
            [StringValue("#171A29")]
            SiteBackgroundColor = 1,
            [StringValue("/Assets/images/body-bg.jpg")]
            SiteBackgroundImage = 2,
            [StringValue("/Assets/images/logo.png")]
            SiteLogo = 3,
            [StringValue("#00D6B0")]
            HeaderBackgroundColor = 4,

            [StringValue("Header Font Style")]
            HeaderFontStyle = 5,
            [StringValue("18px")]
            HeaderFontStyleFontSize = 6,
            [StringValue("400")]
            HeaderFontStyleFontWight = 7,
            [StringValue("Raleway, sans-serif")]
            HeaderFontStyleFontFamily = 8,
            [StringValue("#fff")]
            HeaderFontStyleFontColor = 9,

            [StringValue("#242128")]
            MenuBackgroundColor = 10,

            [StringValue("Menu Font Style")]
            MenuFontStyle = 11,
            [StringValue("15px")]
            MenuFontStyleFontSize = 12,
            [StringValue("400")]
            MenuFontStyleFontWight = 13,
            [StringValue("Raleway, sans-serif")]
            MenuFontStyleFontFamily = 14,
            [StringValue("#c4c4c4")]
            MenuFontStyleFontColor = 15,

            [StringValue("#242128")]
            SubmenuBackgroundColor = 16,

            [StringValue("Submenu Font Style")]
            SubmenuFontStyle = 17,
            [StringValue("13px")]
            SubmenuFontStyleFontSize = 18,
            [StringValue("400")]
            SubmenuFontStyleFontWight = 19,
            [StringValue("Raleway, sans-serif")]
            SubmenuFontStyleFontFamily = 20,
            [StringValue("#fff")]
            SubmenuFontStyleFontColor = 21,

            [StringValue("/Assets/images/Clients_bg.png")]
            ClientTileBackgroundImage = 22,
            [StringValue("/Assets/images/dash_accounts.png")]
            AccountsTileBackgroundImage = 23,
            [StringValue("/Assets/images/dash_Meal_Plans.png")]
            PrivilegesTileBackgroundImage = 24,
            [StringValue("/Assets/images/Devices_bg.png")]
            DevicesTileBackgroundImage = 25,
            [StringValue("/Assets/images/reports_bg.png")]
            ReportsTileBackgroundImage = 26,
            [StringValue("/Assets/images/Roles_bg.png")]
            RolesTileBackgroundImage = 27,
            [StringValue("/Assets/images/System_Admin_bg.png")]
            AdministrationTileBackgroundImage = 28,
            [StringValue("/Assets/images/dash_schedules.png")]
            ToolsTileBackgroundImage = 29,
            [StringValue("/Assets/images/Appearance_bg.png")]
            AppearanceTileBackgroundImage = 30,

            [StringValue("Form Header Font Style")]
            FormHeaderFontStyle = 31,
            [StringValue("13px")]
            FormHeaderFontStyleFontSize = 32,
            [StringValue("600")]
            FormHeaderFontStyleFontWight = 33,
            [StringValue("Raleway, sans-serif")]
            FormHeaderFontStyleFontFamily = 34,
            [StringValue("#fff")]
            FormHeaderFontStyleFontColor = 35,

            [StringValue("Form Lable Font Style")]
            FormLableFontStyle = 36,
            [StringValue("700")]
            FormLableFontStyleFontWight = 37,
            [StringValue("Raleway, sans-serif")]
            FormLableFontStyleFontFamily = 38,
            [StringValue("#fff")]
            FormLableFontStyleFontColor = 39,

            [StringValue("Form Control Font Style")]
            FormControlFontStyle = 40,
            [StringValue("13px")]
            FormControlFontStyleFontSize = 41,
            [StringValue("400")]
            FormControlFontStyleFontWight = 42,
            [StringValue("Raleway, sans-serif")]
            FormControlFontStyleFontFamily = 43,
            [StringValue("#F8DED1")]
            FormControlFontStyleFontColor = 44,

            [StringValue("#f8ded1")]
            FormControlFontColor = 45,
            [StringValue("#a09383")]
            FormControlBorderColor = 46,
            [StringValue("#171A29")]
            GridHeaderBackgroundColor = 47,
            [StringValue("")]
            GridFooterBackgroundColor = 48,
            [StringValue("")]
            GridRowBackgroundColor = 49,
            [StringValue("")]
            GridRowAlternateBackgroundColor = 50,

            // tiles background color
            [StringValue("")]
            ClientTileBackgroundColor = 51,
            [StringValue("")]
            AccountsTileBackgroundColor = 52,
            [StringValue("")]
            PrivilegesTileBackgroundColor = 53,
            [StringValue("")]
            DevicesTileBackgroundColor = 54,
            [StringValue("")]
            ReportsTileBackgroundColor = 55,
            [StringValue("")]
            RolesTileBackgroundColor = 56,
            [StringValue("")]
            AdministrationTileBackgroundColor = 57,
            [StringValue("")]
            ToolsTileBackgroundColor = 58,
            [StringValue("")]
            AppearanceTileBackgroundColor = 59,
        }

        public enum FontWeight
        {
            [StringValue("Normal")]
            Normal = 1,
            [StringValue("Bold")]
            Bold = 2,
            [StringValue("300")]
            FontWeight300 = 3,
            [StringValue("400")]
            FontWeight400 = 4,
            [StringValue("500")]
            FontWeight500 = 5,
            [StringValue("600")]
            FontWeight600 = 6,
            [StringValue("700")]
            FontWeight700 = 7
        }

        public enum ModulePermission
        {
            manageClientData = 1,
            operator_list = 2,
            operator_edit = 3,
            manageTasks = 4,
            manageDefaults = 5,
            manageThemes = 6,
            manageLanguages = 7,
            manageTranslations = 8,
            seeDebug = 9,
            manageRoles = 10,
            manageMenus = 11,
            seeClientMenu = 12,
            seeAccountMenu = 13,
            seeDeviceMenu = 14,
            seePlansMenu = 15,
            seeReportsMenu = 16,
            seeSysAdminMenu = 17,
            manage_devices = 18,
            view_devices = 19,
            edit_device_group = 20,
            view_device_group = 21,
            manage_pmt_methods = 22,
            view_pmt_methods = 23,
            manage_acct_types = 24,
            su_acct_types = 25,
            view_acct_types = 26,
            manage_meal_plans = 27,
            edit_meal_plans = 28,
            view_meal_plans = 29,
            manage_meal_types = 30,
            view_meal_types = 31,
            acc_addMoney = 32,
            acc_deductMoney = 33,
            acc_balance = 34,
            acc_statement = 35,
            acc_setCredit = 36,
            view_department = 37,
            manage_department = 38,
            view_cost_centre = 39,
            manage_cost_centre = 40,
            manage_profiles = 41,
            view_profiles = 42,
            manage_client_card = 43,
            new_client_card = 44,
            return_client_card = 45,
            manage_client_card_pin = 46,
            view_client = 47,
            manage_client = 48,
            manage_deleted_clients = 49,
            delete_clients = 50,
            view_plans = 51,
            manage_plans = 52,
            manage_printergroups = 53,
            manage_printers = 54
        }

        public enum Unlimitedaccounts
        {
            addMoney = 1,
            deductMoney = 2,
            balance = 3,
            statement = 4,
            setCredit = 5
        }

        public enum MessageGroup
        {
            Netlink = 1,
            Web = 2,
            WebLabel = 3
        }

        public enum Translation
        {
            Login = 3000,
            ForgotPassword = 3001,
            Code = 3035,
            Clients = 3002,
            Filter = 3003,
            ClienID = 3004,
            NetworkID = 3005,
            Profile = 3006,
            CardNo = 3007,
            Manage = 3008,
            Add_a_Client = 3009,
            Personal_Information = 3010,
            First_Name = 3011,
            Last_Name = 3012,
            Date_of_Birth = 3013,
            Language = 3014,
            Upload_Image = 3015,
            Terms_and_conditions_Accepted = 3016,
            Allow_Undefined_Departmental_Codes = 3017,
            A_Free_Print = 3018,
            Easy_Convert_Admin = 3019,
            Comment = 3020,
            Contact = 3021,
            Faculty = 3022,
            Job_Title = 3023,
            Program_of_Study = 3024,
            Address = 3025,
            Phone = 3026,
            City = 3027,
            State_Province = 3028,
            Postal_Code = 3029,
            Email = 3030,
            Allow_Communication_By_Email = 3031,
            Res_Status = 3032,
            Grade_Level = 3033,
            Department_Codes = 3034,
            Client_Profiles = 3036,
            Sort_By = 3037,
            Description = 3038,
            Deleted_Clients = 3039,
            Family = 3040,
            Balance = 3042,
            Confirmation = 3043,
            Add_Department = 3044,
            General_Information = 3045,
            Cost_Center = 3046,
            Accounts = 3047,
            Departments = 3048,
            Add_Cost_Center = 3049,
            Centers = 3050,
            Account_Type = 3051,
            UP800_Total = 3052,
            Free_Print = 3053,
            Web_deposit = 3054,
            Payment_Methods = 3055,
            Type = 3056,
            Method = 3057,
            Devices = 3058,
            Group = 3059,
            IP_Address = 3060,
            Online = 3061,
            Enabled = 3062,
            Add_Device = 3063,
            Enable = 3064,
            Model = 3065,
            MAC_Address = 3066,
            Initialization = 3067,
            Not_Initialized = 3068,
            Allow_Initialization = 3069,
            Extended_Passback = 3070,
            Reset_With_Meal_Plan = 3071,
            Initialized_Accounts = 3072,
            Fetch_offline_transactions_on_this_schedule = 3074,
            Add_Device_Group = 3075,
            Device_Charging_Restriction = 3076,
            Departmental_Charging_Restriction = 3077,
            Meal_Plans = 3078,
            DAYS = 3079,
            Expiry = 3080,
            Price = 3081,
            Meal_Types = 3083,
            From = 3084,
            To = 3085,
            Edit_Meal_Type = 3086,
            From_Time = 3087,
            To_Time = 3088,
            Manage_Menu_Activations = 3089,
            Deleted_Client = 3092,
            Account_Types = 3094,
            System_Administration = 3098,
            Operators = 3099,
            Manage_Roles = 3100,
            Defaults = 3101,
            Themes = 3102,
            Languages = 3103,
            Translations = 3104,
            Manage_Menus = 3105,
            Tools = 3106,
            Given = 3110,
            Roles = 3111,
            Comments = 3112,
            Add_Operator = 3113,
            Details = 3114,
            Password = 3115,
            Confirm_Password = 3116,
            System_Defaults = 3118,
            Property = 3119,
            Value = 3120,
            Client_Account_Info = 3121,
            Account_Information = 3122,
            Account_Info = 3123,
            Deposit = 3124,
            Paid_By = 3126,
            Credit = 3127,
            Limited = 3128,
            Unlimited = 3129,
            Client_Name = 3130,
            Account = 3131,
            Period = 3132,
            status = 3133,
            expiry_date = 3134,
            Issued = 3135,
            Returned = 3136,
            Meal_Plan_Info = 3137,
            Plan = 3138,
            Start_Date = 3139,
            Last_Meal_Time = 3140,
            Week_Meal_Count = 3141,
            Taken = 3142,
            Max = 3143,
            amount = 3144,
            Profile_Create = 3145,
            General = 3146,
            Description_2 = 3147,
            Enable_Reset_Balance = 3148,
            Daily = 3149,
            Weekly = 3150,
            Monthly = 3151,
            Reset_Balance_Time_and_Value = 3152,
            Last_Reset = 3155,
            Discount_UP_700_online_only = 3157,
            Discount_percentage_of_transaction_amount_will_be_treated_as_Coupon_Loyalty_amount_Real_charge_will_be_reduced_with_Coupon_Loyalty_amount = 3158,
            Tax_Deletion_Feature_for_UDP800_only = 3159,
            Tax1 = 3160,
            Tax2 = 3161,
            Tax3 = 3162,
            Tax4 = 3163,
            Tax5 = 3164,
            Process_complete_transaction = 3165,
            To_taxable_account_for_insufficient_funds_on_non_taxable_account = 3166,
            Operator_Roles = 3167,
            Add_a_new_Role = 3168,
            To_be_assigned = 3169,
            Manage_ClientData = 3170,
            View_Clients = 3171,
            Manage_Clients = 3172,
            Manage_Deleted = 3173,
            Delete = 3174,
            View_Meal_Plans = 3175,
            Manage_Meal = 3176,
            List_Operators = 3177,
            Edit_Operators = 3178,
            Tasks = 3179,
            Debug_Visible = 3184,
            Menu_Defaults = 3186,
            menuAccess = 3187,
            devices = 3189,
            Plans = 3190,
            Reports = 3191,
            Manage_All_Devices = 3193,
            View_Device_Details = 3194,
            Device_Groups = 3195,
            View_Device_Groups = 3196,
            Printer_Groups = 3197,
            Printers = 3198,
            paymentMethods = 3199,
            Manage_Payment_Methods = 3200,
            View_Payment_Methods = 31201,
            Manage_Account_Types = 3203,
            Super_Operator_of = 3204,
            View_Account_Types = 3205,
            View_Schedules = 3207,
            departments = 3209,
            View_Departments = 3210,
            View_Cost_Centers = 3212,
            View_Client_Profiles = 3213,
            Client_Card = 3215,
            UnlimitedAccount = 3216,
            Device_Group = 3217,
            Allowed = 3218,
            Not_Allowed = 3219,
            Profile_Group = 3220,
            None = 3222,
            Cash = 3223,
            Cheque = 3224,
            Card = 3225,
            MessageID = 3227,
            DefaultMessage = 3228,
            CustomMessage = 3229,
            UseCustom = 3230,
            FamilyName = 3231,
            GivenName = 3232,
            General_Account_Balance = 3233,
            Browse = 3234,
            ClientPicture = 3235,
            ContactInformation = 3236,
            ClientAccountInfoAccountNo = 3237,
            Bank = 3238,
            Date = 3239,
            RegDate = 3240,
            Dev_Oper = 3241,
            Debit = 3242,
            DayoftheWeek = 3243,
            Name = 3244,
            Create_Cost_Center = 3245,
            Add = 3246,
            Deduct = 3247,
            Print = 3248,
            Meals_per_plan = 3249,
            Meals_per_week = 3250,
            Amount_perplan = 3251,
            Days_Per_Week = 3252,
            Add_Meal_Type = 3253,
            OperatorDetails = 3254,
            ChangePassword = 3255,
            OldPassword = 3256,
            NewPassword = 3257,
            LanguagesPack = 3258,
            Default = 3259,
            Edit_a_Theme = 3260,
            ThemeName = 3261,
            SetTheme = 3262,
            PicktheColor = 3263,
            Site_Background_Color = 3264,
            Site_Background_Image = 3265,
            Site_Logo = 3266,
            Header_Background_Color = 3267,
            HeaderFontStyle = 3268,
            NameSizeWeightColor = 3269,
            NavigationBar = 3270,
            MenuBackgroundColor = 3271,
            MenuFontStyle = 3272,
            SubMenuBackgroundColor = 3273,
            SubMenuFontStyle = 3274,
            BoxBackground = 3275,
            Box = 3276,
            Privileges = 3277,
            Appearance = 3278,
            Form_Contents = 3279,
            FormHeaderFontStyle = 3280,
            FormLabelFontStyle = 3281,
            FormControlFontStyle = 3282,
            FormControlforcolor = 3283,
            FormControlBordercolor = 3284,
            GridHeaderBackgroundcolor = 3285,
            GridFooterBackgroundcolor = 3286,
            GridRowBackgroundcolor = 3287,
            GridRowAlternateBackgroundcolor = 3288,
            FontName = 3289,
            FontSize = 3290,
            FontWeight = 3291,
            FontColor = 3292,
            Header = 3293,
            EditDepartment = 3294,
            DayoftheMonth = 3295,
            SelectAll = 3296,
            Initialized = 3297,
            PriceLine = 3298,
            CopierBudget = 3299,
            Modem = 3300,
            Accountsdiscounts = 3301,
            AssignedPrintQueues = 3302,
            PrintQueue = 3303,
            Noassignmentprintqueues = 3304,
            Welcome = 3305,
            ExclusivetoITCSystems = 3306,
            FastSimpleEfficient = 3307,
            OperatorSetting = 3308,
            Onlyimagesallowed = 3309,
            System_Admin = 3310,
            AntiPassback = 3311,
            EnableMultipleMeals = 3312,
            Profilesallowedtopurchaseplanonline = 3313,
            PurchaseNewPlan = 3314,
            WouldYouLikeToTransferConsumedMeal = 3315,
            Yes = 3316,
            No = 3317,
            Wouldyouliketodeductfundused = 3318,
            EndDate = 3319,
            PaidBy = 3320,
            Choosefile = 3321,
            Removecolorimage = 3322,
            EditTranslation = 3323,
            Home = 3324,
            NoAccess = 3325,
            Help = 3326,
            Credentials = 3327,
            Issue_Date = 3328,
            Return_Date = 3329,
            Primary = 3330,
            Edit_Credentials = 3331,
            Credential_Type = 3332,
            Manuel_Entry_Allowed = 3333,
            Auth_2_Required = 3334,
            MealsPerDay = 3335,
            TotalMeals = 3336,
            TotalAmount = 3337,
            MaxMealPrice = 3338,
            MealPlan_Details = 3339,
            Bonus = 3340,
            Selectthedaysandmealswhenthecardcannotbeused = 3341,
            Sun = 3342,
            Mon = 3343,
            Tue = 3344,
            Wed = 3345,
            Thu = 3346,
            Fri = 3347,
            Sat = 3348,
            MealPlanScheduler = 3349,
            Advanced_Restrictions = 3350,
            AllLanguage = 3351,
            CardInfoFor = 3352,
            Pin = 3353,
            ConfirmPin = 3354,
            AccountStatement = 3355,
            ShowAll_Plans = 3356,
            Meal = 3357,
            PlanDescription = 3358,
            PlanStartDate = 3359,
            LastMealTime = 3360,
            WeekMealCount = 3361,
            MealsTaken = 3362,
            MaxMeals = 3363,
            Maxamount = 3364,
            TimeRestrictions = 3365,
            UpdateProfile = 3366,
            EditPlan = 3367,
            Spent = 3368,
            AddaTheme = 3369,
            EditCostCenter = 3370,
            DeletedClients = 3371,
            ResetPassword = 3372,
            Setupofinitialparameters = 3373,
            MatrixKey = 3374,
            InitialMasterRole = 3375,
            InitialMasterUser = 3376,
            Reportsfromemail = 3377,
            BaseURL = 3378,
            Pathto_HTMLDOCexec = 3379,
            PathtoPDFStorage = 3380,
            SiteRoot = 3381,
            SessionName = 3382,
            SessionIdleTimeout = 3383,
            EditRole = 3384,
            AddaPaymentMethod = 3385,
            Edit = 3386,
            Find = 3387,
            Addnew = 3388,
            Clear = 3389,
            Save = 3390,
            Cancel = 3391,
            MakeCopyofPlan = 3392,
            SetPlanSchedule = 3393,
            MakeCopyofDevice = 3394,
            CreditLimit=3395,
            Statement=3396,
            Returntoclient=3397,
            Close=3398,
            ReturntoAccounts=3399,
            Create= 3400,
            ReturntoMealPlan= 3401,
            Preview=3402,
            OK=3403,         
           Sorryyoudonthavepermissionforthismodule=3404,
            ErrorInformation=3405,
            Moreinformation=3406,
            Sorryfortheinconvenience=3407,
            Pleasecontactyouradministrator=3408,
            Anerroroccurred=3409
        }

        public enum DBStructure
        {
            Unlimited = 15
        }
        public enum DatabaseEntities
        {
            [StringValue("New_ITC_WMMEntities")]
            New_ITC_WMMEntities = 1,
            [StringValue("New_ITC_MultiplanEntities")]
            New_ITC_MultiplanEntities = 2
        }
    }
}
