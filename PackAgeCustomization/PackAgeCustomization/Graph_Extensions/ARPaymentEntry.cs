using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.SO;

namespace PX.Objects.AR
{
    ///<remarks> The primary DAC queried in the selector actually comes from the unbound DAC redefined in "ARAdjust", not PX.Objects.AR.ARInvoice. </remarks>
    using ARInvoice_Adj = PX.Objects.AR.ARAdjust.ARInvoice;

    public class ARPaymentEntry_Extension : PXGraphExtension<PX.Objects.AR.ARPaymentEntry>
    {
        #region Event Handlers
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [ARInvoiceType.AdjdRefNbr(typeof(Search2<ARInvoice_Adj.refNbr, LeftJoin<ARAdjust, On<ARAdjust.adjdDocType, Equal<ARInvoice.docType>,
                                                                              And<ARAdjust.adjdRefNbr, Equal<ARInvoice.refNbr>,
                                                                              And<ARAdjust.released, NotEqual<True>,
                                                                              And<ARAdjust.voided, NotEqual<True>,
                                                                              And<Where<ARAdjust.adjgDocType, NotEqual<Current<ARRegister.docType>>,
                                                                                        Or<ARAdjust.adjgRefNbr, NotEqual<Current<ARRegister.refNbr>>>>>>>>>,
                                                                     LeftJoin<ARAdjust2, On<ARAdjust2.adjgDocType, Equal<ARInvoice.docType>,
                                                                              And<ARAdjust2.adjgRefNbr, Equal<ARInvoice.refNbr>,
                                                                              And<ARAdjust2.released, NotEqual<True>,
                                                                              And<ARAdjust2.voided, NotEqual<True>>>>>,
                                                                     LeftJoin<Customer, On<ARInvoice.customerID, Equal<Customer.bAccountID>>,
                                                                     LeftJoin<SOInvoice, On<ARInvoice.docType, Equal<SOInvoice.docType>, And<ARInvoice.refNbr, Equal<SOInvoice.refNbr>>>,
                                                                     LeftJoin<SOOrderShipment, On<SOOrderShipment.invoiceType, Equal<ARInvoice.docType>,
                                                                                                  And<SOOrderShipment.invoiceNbr, Equal<ARInvoice.refNbr>>>>
                                                                         >>>>,
                                                                     Where<ARInvoice_Adj.docType, Equal<Optional<ARAdjust.adjdDocType>>,
                                                                           And2<Where<ARInvoice_Adj.released, Equal<True>,
                                                                                      Or<ARInvoice_Adj.origModule, Equal<BatchModule.moduleSO>,
                                                                                And<ARInvoice_Adj.docType, NotEqual<ARDocType.creditMemo>>>>,
                                                                           And<ARInvoice_Adj.openDoc, Equal<True>,
                                                                           And<ARInvoice_Adj.hold, Equal<False>,
                                                                           And<ARAdjust.adjgRefNbr, IsNull,
                                                                           And<ARAdjust2.adjdRefNbr, IsNull,
                                                                           And<ARInvoice_Adj.customerID, In2<Search<Override.BAccount.bAccountID,
                                                                               Where<Override.BAccount.bAccountID, Equal<Optional<ARRegister.customerID>>,
                                                                                     Or<Override.BAccount.consolidatingBAccountID, Equal<Optional<ARRegister.customerID>>>>>>,
                                                                           And2<Where<ARInvoice_Adj.pendingPPD, NotEqual<True>,
                                                                                      Or<Current<ARRegister.pendingPPD>, Equal<True>>>,
                                                                           And<Where<Current<ARSetup.migrationMode>, NotEqual<True>,
                                                                                     Or<ARInvoice_Adj.isMigratedRecord, Equal<Current<ARRegister.isMigratedRecord>>>>>>>>>>>>>>),
                                    typeof(ARRegister.branchID),
                                    typeof(ARRegister.refNbr),
                                    typeof(ARRegisterExt.usrGUINbr),
                                    typeof(SOOrderShipment.orderNbr),
                                    typeof(ARRegister.docDate),
                                    typeof(ARRegister.finPeriodID),
                                    typeof(ARRegister.customerID),
                                    typeof(ARRegister.customerLocationID),
                                    typeof(ARRegister.curyID),
                                    typeof(ARRegister.curyOrigDocAmt),
                                    typeof(ARRegister.curyDocBal),
                                    typeof(ARRegister.status),
                                    typeof(ARRegister.dueDate),
                                    typeof(ARAdjust.ARInvoice.invoiceNbr),
                                    typeof(ARRegister.docDesc),
                                    Filterable = true)]
        protected void _(Events.CacheAttached<ARAdjust.adjdRefNbr> e) { }
        #endregion
    }
}