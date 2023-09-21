using System;
using PX.Data;
using PX.Objects.SO;

namespace PX.Objects.AR
{
    public class ARReleaseProcess_Extension : PXGraphExtension<ARReleaseProcess>
    {
        #region Override Methods
        [PXOverride]
        public virtual void ProcessInvoiceDetailDiscount(ARRegister doc, Customer customer, ARTran tran, SOOrderType sotype, ARInvoice ardoc,
                                                         Action<ARRegister, Customer, ARTran, SOOrderType, ARInvoice> baseMethod)
        {
            baseMethod(doc, customer, tran, sotype, ardoc);

            var curTran = Base.ARTran_TranType_RefNbr.Current;

            if (sotype.GetExtension<SOOrderTypeExt>().UsrUseDiscountAcctFromSalesAcct == true && curTran != null)
            {
                Base.ARTran_TranType_RefNbr.SetValueExt<ARTran.accountID>(curTran, SOLine.PK.Find(Base, curTran.SOOrderType, curTran.SOOrderNbr, curTran.SOOrderLineNbr)?.SalesAcctID);
                Base.ARTran_TranType_RefNbr.Update(curTran);
            }
        }
        #endregion
    }
}
