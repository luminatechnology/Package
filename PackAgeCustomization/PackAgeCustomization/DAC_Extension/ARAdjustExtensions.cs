using PX.Data;
using PX.Data.BQL.Fluent;

namespace PX.Objects.AR
{
    public class ARAdjustExt : PXCacheExtension<PX.Objects.AR.ARAdjust>
    {
        #region UsrSOOrderNbr
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Order Nbr. [*]", Enabled = false)]
        [PXDBScalar(typeof(SelectFrom<ARTran>.Where<ARTran.tranType.IsEqual<ARAdjust.adjdDocType>
                                                    .And<ARTran.refNbr.IsEqual<ARAdjust.adjdRefNbr>>>.SearchFor<ARTran.sOOrderNbr>))]
        [PXUnboundDefault(typeof(SelectFrom<ARTran>.Where<ARTran.tranType.IsEqual<ARAdjust.adjdDocType.FromCurrent>
                                                         .And<ARTran.refNbr.IsEqual<ARAdjust.adjdRefNbr.FromCurrent>>>.SearchFor<ARTran.sOOrderNbr>))]
        public virtual string UsrSOOrderNbr { get; set; }
        public abstract class usrSOOrderNbr : PX.Data.BQL.BqlString.Field<usrSOOrderNbr> { }
        #endregion
    }
}
