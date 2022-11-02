using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.GL;
using PX.Objects.SO;

namespace PX.Objects.AR
{
    [PXNonInstantiatedExtension]
    public class ARAdjustExt_Columns : PXCacheExtension<PX.Objects.AR.ARAdjust>
    {
        #region AdjdRefNbr	
        [PXRemoveBaseAttribute(typeof(ARInvoiceType.AdjdRefNbrAttribute))]
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [ARInvoiceType2.AdjdRefNbr2(typeof(Search2<ARInvoice.refNbr, LeftJoin<ARAdjust, On<ARAdjust.adjdDocType, Equal<ARInvoice.docType>,
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
                                                                                                 And<SOOrderShipment.invoiceNbr, Equal<ARInvoice.refNbr>>>>>>>>,
                                                                    Where<ARInvoice.docType, Equal<Optional<ARAdjust.adjdDocType>>,
                                                                          And2<Where<ARInvoice.released, Equal<True>,
                                                                            Or<ARInvoice.origModule, Equal<BatchModule.moduleSO>,
                                                                            And<ARInvoice.docType, NotEqual<ARDocType.creditMemo>>>>,
                                                                        And<ARInvoice.openDoc, Equal<True>,
                                                                        And<ARInvoice.hold, Equal<False>,
                                                                        And<ARAdjust.adjgRefNbr, IsNull,
                                                                        And<ARAdjust2.adjdRefNbr, IsNull,
                                                                        And<ARInvoice.customerID, In2<Search<Override.BAccount.bAccountID,
                                                                            Where<Override.BAccount.bAccountID, Equal<Optional<ARRegister.customerID>>,
                                                                                  Or<Override.BAccount.consolidatingBAccountID, Equal<Optional<ARRegister.customerID>>>>>>,
                                                                        And2<Where<ARInvoice.pendingPPD, NotEqual<True>,
                                                                                   Or<Current<ARRegister.pendingPPD>, Equal<True>>>,
                                                                        And<Where<Current<ARSetup.migrationMode>, NotEqual<True>,
                                                                                  Or<ARInvoice.isMigratedRecord, Equal<Current<ARRegister.isMigratedRecord>>>>>>>>>>>>>>), 
                                    Filterable = true)]
        public string AdjdRefNbr { get; set; }
        #endregion
    }

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

    #region Custom inherited Attribute
    /// <summary>
    /// Extend the standard Selector attibute
    /// </summary>
    public class ARInvoiceType2 : ARInvoiceType
    {
        public class AdjdRefNbr2Attribute : PXSelectorAttribute
        {
            /// <summary>
            /// Add two custom fields on search form.
            /// Copied from standard attribute.
            /// </summary>
            /// <param name="SearchType">Must be IBqlSearch, returning ARInvoice.refNbr</param>
            public AdjdRefNbr2Attribute(Type SearchType) : base(SearchType,
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
                                                                typeof(ARRegister.docDesc))
            { }
        }
    }
    #endregion
}
