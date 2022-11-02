using PX.Data;
using PX.Data.BQL.Fluent;

namespace PX.Objects.SO.DAC.Projections
{
    public class SOLineForDirectInvoiceExt : PXCacheExtension<PX.Objects.SO.DAC.Projections.SOLineForDirectInvoice>
    {
		#region UsrCustomerOrderNbr
		[PXString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "Customer Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBScalar(typeof(SelectFrom<SOOrder>.Where<SOOrder.orderType.IsEqual<SOLineForDirectInvoice.orderType>
													 .And<SOOrder.orderNbr.IsEqual<SOLineForDirectInvoice.orderNbr>>>.SearchFor<SOOrder.customerOrderNbr>))]
		public virtual string UsrCustomerOrderNbr { get; set; }
		public abstract class usrCustomerOrderNbr : PX.Data.BQL.BqlString.Field<usrCustomerOrderNbr> { }
		#endregion
	}
}
