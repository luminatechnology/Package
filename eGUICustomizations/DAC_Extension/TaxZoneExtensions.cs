using PX.Data;

namespace PX.Objects.TX
{
    public class TaxZoneExt : PXCacheExtension<PX.Objects.TX.TaxZone>
    {
        #region UsrWHTTaxRelated
        [PXDBBool]
        [PXUIField(DisplayName = "Withholding Tax Related")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrWHTTaxRelated { get; set; }
        public abstract class usrWHTTaxRelated : PX.Data.BQL.BqlBool.Field<usrWHTTaxRelated> { }
        #endregion
    }
}