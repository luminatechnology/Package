using PX.Data;
using PX.Data.BQL.Fluent;

namespace PX.Objects.SO
{
    public class SOOrderShipmentExt : PXCacheExtension<PX.Objects.SO.SOOrderShipment>
    {
		#region UsrCustomerOrderNbr
		[PXString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "Customer Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBScalar(typeof(SelectFrom<SOOrder>.Where<SOOrder.noteID.IsEqual<SOOrderShipment.orderNoteID>>.SearchFor<SOOrder.customerOrderNbr>))]
		public virtual string UsrCustomerOrderNbr { get; set; }
		public abstract class usrCustomerOrderNbr : PX.Data.BQL.BqlString.Field<usrCustomerOrderNbr> { }
		#endregion
	}
}
