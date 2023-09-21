using PX.Data;

namespace PX.Objects.SO
{
    public class SOOrderTypeExt : PXCacheExtension<PX.Objects.SO.SOOrderType>
    {
        #region UsrUseDiscountAcctFromSalesAcct
        [PXDBBool]
        [PXUIField(DisplayName = "Use Discount Account from Sales Account")]
        public virtual bool? UsrUseDiscountAcctFromSalesAcct { get; set; }
        public abstract class usrUseDiscountAcctFromSalesAcct : PX.Data.BQL.BqlBool.Field<usrUseDiscountAcctFromSalesAcct> { }
        #endregion
    }
}