using PX.Data;
using eGUICustomizations.Descriptor;

namespace PX.Objects.TX
{
    public class TaxExt : PXCacheExtension<PX.Objects.TX.Tax>
    {
        #region UsrTWNGUI
        [PXDBBool()]
        [PXUIField(DisplayName = "GUI Enabled")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrTWNGUI { get; set; }
        public abstract class usrTWNGUI : PX.Data.BQL.BqlBool.Field<usrTWNGUI> { }
        #endregion

        #region UsrGUIType
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI VAT Type")]
        [TWNStringList.TWNGUIVATType.List]
        public virtual string UsrGUIType { get; set; }
        public abstract class usrGUIType : PX.Data.BQL.BqlString.Field<usrGUIType> { }
        #endregion
    }
}