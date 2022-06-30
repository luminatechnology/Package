using PX.Data;

namespace eGUICustomizations.Descriptor
{
    public class TWNStringList
    {
        #region TWNGUIVATType
        public class TWNGUIVATType
        {
            public const string Zero    = "00";
            public const string Five    = "05";
            public const string Exclude = "EX";
            public static readonly string[] Values = new string[3]
            {
                Zero, Five, Exclude
            };
            public static readonly string[] Labels = new string[3]
            {
                "VAT00", "VAT05", "VATEX"
            };

            public class ListAttribute : PXStringListAttribute
            {
                public ListAttribute() : base(TWNGUIVATType.Values, TWNGUIVATType.Labels) { }
            }

            public class zero : PX.Data.BQL.BqlString.Constant<zero>
            {
                public zero() : base(Zero) { }
            }

            public class five : PX.Data.BQL.BqlString.Constant<five>
            {
                public five() : base(Five) { }
            }

            public class exclude : PX.Data.BQL.BqlString.Constant<exclude>
            {
                public exclude() : base(Exclude) { }
            }
        }
        #endregion

        #region TWNGUIStatus
        public class TWNGUIStatus
        {
            public const string Used   = "U";
            public const string Voided = "V";
            public const string Blank  = "N";

            public static readonly string[] Values = new string[3]
            {
                Used, Voided, Blank
            };
            public static readonly string[] Labels = new string[3]
            {
                nameof(Used), nameof(Voided), nameof(Blank)
            };

            public class ListAttribute : PXStringListAttribute
            {
                public ListAttribute() : base (TWNGUIStatus.Values, TWNGUIStatus.Labels) { }
            }

            public class used : PX.Data.BQL.BqlString.Constant<used>
            {
                public used() : base(Used) { }
            }

            public class voided : PX.Data.BQL.BqlString.Constant<voided>
            {
                public voided() : base(Voided) { }
            }

            public class blank : PX.Data.BQL.BqlString.Constant<blank>
            {
                public blank() : base(Blank) { }
            }
        }
        #endregion

        #region TWNGUIManualStatus
        public class TWNGUIManualStatus
        {
            public const string Open = "0";
            public const string Released = "1";

            public static readonly string[] Values = new string[2]
            {
                Open, Released
            };
            public static readonly string[] Labels = new string[2]
            {
                nameof(Open), nameof(Released)
            };

            public class ListAttribute : PXStringListAttribute
            {
                public ListAttribute() : base(TWNGUIManualStatus.Values, TWNGUIManualStatus.Labels) { }
            }

            public class open : PX.Data.BQL.BqlString.Constant<open>
            {
                public open() : base(Open) { }
            }

            public class released : PX.Data.BQL.BqlString.Constant<released>
            {
                public released() : base(Released) { }
            }
        }
        #endregion

        #region TWNGUIDirection
        public class TWNGUIDirection
        {
            public const string Issue   = "Issue";
            public const string Receipt = "Receipt";

            public static readonly string[] Values = new string[2]
            {
                Issue, Receipt
            };
            public static readonly string[] Labels = new string[2]
            {
                "GUI Issue", "GUI Receipt"
            };

            public class ListAttribute : PXStringListAttribute
            {
                public ListAttribute() : base(TWNGUIDirection.Values, TWNGUIDirection.Labels) { }
            }

            public class issue : PX.Data.BQL.BqlString.Constant<issue>
            {
                public issue() : base(Issue) { }
            }

            public class receipt : PX.Data.BQL.BqlString.Constant<receipt>
            {
                public receipt() : base(Receipt) { }
            }
        }
        #endregion

        #region TWNGUICustomType
        public class TWNGUICustomType
        {
            public const string NotThruCustom = "1";
            public const string ThruCustom = "2";
            public static readonly string[] Values = new string[2]
            {
                NotThruCustom, ThruCustom
            };
            public static readonly string[] Labels = new string[2]
            {
                "Not Thru Custom", "Thru Custom"
            };

            public class ListAttribute : PXStringListAttribute
            {
                public ListAttribute() : base(TWNGUICustomType.Values, TWNGUICustomType.Labels) { }
            }
        }
        #endregion

        #region TWNCreditAction
        public class TWNCreditAction
        {
            public const string NO = "None";
            public const string CN = "CreditNote";
            public const string VG = "VoidedGUI";

            public static readonly string[] Values = new string[3]
            {
                NO, CN, VG
            };
            public static readonly string[] Labels = new string[3]
            {
               "None", "Credit Note", "Voided GUI"
            };

            public class ListAttribute : PXStringListAttribute
            { 
                public ListAttribute() : base(TWNCreditAction.Values, TWNCreditAction.Labels) { }
            }
        }
        #endregion

        #region TWNB2CType
        public class TWNB2CType
        {
            public const string DEF = "NA";
            public const string MC  = "MC";
            public const string NPO = "NPO";

            public static readonly string[] Values = new string[3]
            {
                DEF, MC, NPO
            };
            public static readonly string[] Labels = new string[3]
            {
                nameof(DEF), nameof (MC), nameof (NPO)
            };

            public class ListAttribute : PXStringListAttribute
            {
                public ListAttribute() : base(TWNB2CType.Values, TWNB2CType.Labels) { }
            }
        }
        #endregion
    }
}
