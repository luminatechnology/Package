using PX.Data;

namespace PX.Objects.SO
{
    public class SOOrderTypeMaint_Extension : PXGraphExtension<SOOrderTypeMaint>
    {
        #region Event Handlers
        protected void _(Events.RowSelected<SOOrderType> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            SOOrderType ordertype = e.Row;

            if (ordertype != null) 
            {
                PXUIFieldAttribute.SetEnabled<SOOrderTypeExt.usrUseDiscountAcctFromSalesAcct>(e.Cache, ordertype, ordertype.PostLineDiscSeparately == true);

                if (ordertype.Behavior == SOBehavior.BL)
                {
                    e.Cache.Adjust<PXUIFieldAttribute>(ordertype)
                    //Order settings
                    .For<SOOrderType.creditHoldEntry>(a =>
                    {
                        a.Visible = a.Enabled = false;
                    })
                    .SameFor<SOOrderType.daysToKeep>()
                    .SameFor<SOOrderType.billSeparately>()
                    .SameFor<SOOrderType.shipSeparately>()
                    .SameFor<SOOrderType.calculateFreight>()
                    .SameFor<SOOrderType.shipFullIfNegQtyAllowed>()
                    .SameFor<SOOrderType.disableAutomaticDiscountCalculation>()
                    .SameFor<SOOrderType.recalculateDiscOnPartialShipment>()
                    .SameFor<SOOrderType.allowRefundBeforeReturn>()
                    .SameFor<SOOrderType.copyNotes>()
                    .SameFor<SOOrderType.copyFiles>()
                    .SameFor<SOOrderType.copyLineNotesToShipment>()
                    .SameFor<SOOrderType.copyLineFilesToShipment>()
                    .SameFor<SOOrderType.copyLineNotesToInvoice>()
                    .SameFor<SOOrderType.copyLineNotesToInvoiceOnlyNS>()
                    .SameFor<SOOrderType.copyLineFilesToInvoice>()
                    .SameFor<SOOrderType.copyLineFilesToInvoiceOnlyNS>()
                    //Accounts Receipvable Settings
                    .SameFor<SOOrderType.invoiceNumberingID>()
                    .SameFor<SOOrderType.markInvoicePrinted>()
                    .SameFor<SOOrderType.markInvoiceEmailed>()
                    .SameFor<SOOrderType.invoiceHoldEntry>()
                    .SameFor<SOOrderType.useCuryRateFromSO>()
                    //Posting Settings
                    .SameFor<SOOrderType.salesAcctDefault>()
                    .SameFor<SOOrderType.salesSubMask>()
                    .SameFor<SOOrderType.freightAcctID>()
                    .SameFor<SOOrderType.freightAcctDefault>()
                    .SameFor<SOOrderType.freightSubID>()
                    .SameFor<SOOrderType.freightSubMask>()
                    .SameFor<SOOrderType.discountAcctID>()
                    .SameFor<SOOrderType.discAcctDefault>()
                    .SameFor<SOOrderType.discountSubID>()
                    .SameFor<SOOrderType.discSubMask>()
                    .SameFor<SOOrderType.postLineDiscSeparately>()
                    .SameFor<SOOrderType.useDiscountSubFromSalesSub>()
                    // New custom field
                    .SameFor<SOOrderTypeExt.usrUseDiscountAcctFromSalesAcct>()
                    .SameFor<SOOrderType.autoWriteOff>()
                    .SameFor<SOOrderType.useShippedNotInvoiced>()
                    .SameFor<SOOrderType.shippedNotInvoicedAcctID>()
                    .SameFor<SOOrderType.shippedNotInvoicedSubID>()
                    //Intercompany Posting Settings
                    .SameFor<SOOrderType.intercompanySalesAcctDefault>()
                    .SameFor<SOOrderType.intercompanyCOGSAcctDefault>();
                }
            }
        }
        #endregion
    }
}
