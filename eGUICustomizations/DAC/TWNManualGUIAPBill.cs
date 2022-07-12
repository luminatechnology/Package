using System;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.TX;
using eGUICustomizations.Descriptor;

namespace eGUICustomizations.DAC
{
    [Serializable]
    [PXCacheName("TWN Manual GUI AP Bill")]
    public class TWNManualGUIAPBill : IBqlTable
    {
        #region DocType
        [PXDBString(3, IsKey = true, IsFixed = true)]
        [PXUIField(DisplayName = "Doc Type", Visible = false)]
        [APDocType.List()]
        [PXDBDefault(typeof(APInvoice.docType))]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsKey = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Ref Nbr", Visible = false)]
        [PXDBDefault(typeof(APInvoice.refNbr))]
        [PXParent(typeof(Select<ARInvoice, Where<ARInvoice.docType, Equal<Current<TWNManualGUIAPBill.docType>>,
                                                 And<ARInvoice.refNbr, Equal<Current<TWNManualGUIAPBill.refNbr>>>>>))]
        [ARInvoiceType.RefNbr(typeof(Search2<PX.Objects.AR.Standalone.ARRegisterAlias.refNbr, InnerJoinSingleTable<ARInvoice, On<ARInvoice.docType, Equal<PX.Objects.AR.Standalone.ARRegisterAlias.docType>,
                                                                                                                                 And<ARInvoice.refNbr, Equal<PX.Objects.AR.Standalone.ARRegisterAlias.refNbr>>>,
                                                                                              InnerJoinSingleTable<Customer, On<PX.Objects.AR.Standalone.ARRegisterAlias.customerID, Equal<Customer.bAccountID>>>>,
                                                                                              Where<PX.Objects.AR.Standalone.ARRegisterAlias.docType, Equal<Optional<ARInvoice.docType>>,
                                                                                                    And2<Where<PX.Objects.AR.Standalone.ARRegisterAlias.origModule, Equal<BatchModule.moduleAR>,
                                                                                                               Or<PX.Objects.AR.Standalone.ARRegisterAlias.origModule, Equal<BatchModule.moduleEP>,
                                                                                                                  Or<PX.Objects.AR.Standalone.ARRegisterAlias.released, Equal<True>>>>,
                                                                                                         And<Match<Customer, Current<AccessInfo.userName>>>>>,
                                                                                              OrderBy<Desc<PX.Objects.AR.Standalone.ARRegisterAlias.refNbr>>>), 
                               ValidateValue = false)]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region GUINbr
        [GUINumber(15, IsKey = true, IsUnicode = true, InputMask = ">aaaaaaaaaaaaaa")]
        [PXUIField(DisplayName = "GUI Nbr")]
        [PXDefault()]
        public virtual string GUINbr { get; set; }
        public abstract class gUINbr : PX.Data.BQL.BqlString.Field<gUINbr> { }
        #endregion

        #region Status
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [TWNStringList.TWNGUIManualStatus.List]
        [PXDefault(TWNStringList.TWNGUIManualStatus.Open)]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region VendorID
        [Vendor]
        [PXDefault(typeof(APInvoice.vendorID), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region VATInCode
        public const string VATINFRMTName = "VATINFRMT";
        public class VATINFRMTNameAtt : PX.Data.BQL.BqlString.Constant<VATINFRMTNameAtt>
        {
            public VATINFRMTNameAtt() : base(VATINFRMTName) { }
        }

        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "VAT In Code", Required = true)]
        [PXSelector(typeof(Search<CSAttributeDetail.valueID,
                                  Where<CSAttributeDetail.attributeID, Equal<VATINFRMTNameAtt>>>),
                    typeof(CSAttributeDetail.description))]
        [PXDefault(typeof(Search2<CSAnswers.value, InnerJoin<Vendor, On<Vendor.noteID, Equal<CSAnswers.refNoteID>,
                                                                       And<CSAnswers.attributeID, Equal<VATINFRMTNameAtt>>>>,
                                                   Where<Vendor.bAccountID, Equal<Current<vendorID>>>>))]
        [PXFormula(typeof(Default<vendorID>))]
        public virtual string VATInCode { get; set; }
        public abstract class vATInCode : PX.Data.BQL.BqlString.Field<vATInCode> { }
        #endregion

        #region GUIDate
        [PXDBDate()]
        [PXUIField(DisplayName = "GUI Date")]
        public virtual DateTime? GUIDate { get; set; }
        public abstract class gUIDate : PX.Data.BQL.BqlDateTime.Field<gUIDate> { }
        #endregion

        #region TaxZoneID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tax Zone ID")]
        [PXDefault(typeof(APInvoice.taxZoneID), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search3<TaxZone.taxZoneID,
                                   OrderBy<Asc<TaxZone.taxZoneID>>>),
                    CacheGlobal = true)]
        public virtual string TaxZoneID { get; set; }
        public abstract class taxZoneID : PX.Data.BQL.BqlString.Field<taxZoneID> { }
        #endregion

        #region TaxCategoryID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tax Category ID")]
        [PXDefault(typeof(APTran.taxCategoryID), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>),
                      PX.Objects.TX.Messages.InactiveTaxCategory,
                      typeof(TaxCategory.taxCategoryID))]
        public virtual string TaxCategoryID { get; set; }
        public abstract class taxCategoryID : PX.Data.BQL.BqlString.Field<taxCategoryID> { }
        #endregion

        #region TaxID
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tax ID")]
        [PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr),  DirtyRead = true)]
        [PXDefault(typeof(Search<TaxZoneDet.taxID,
                                 Where<TaxZoneDet.taxZoneID, Equal<Current<TWNManualGUIAPBill.taxZoneID>>>>))]
        public virtual string TaxID { get; set; }
        public abstract class taxID : PX.Data.BQL.BqlString.Field<taxID> { }
        #endregion

        #region TaxNbr
        public const string TaxNbrName = "TAXNbr";
        public class TaxNbrNameAtt : PX.Data.BQL.BqlString.Constant<TaxNbrNameAtt>
        {
            public TaxNbrNameAtt() : base(TaxNbrName) { }
        }

        [TaxNbrVerify(8, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tax Nbr")]
        [PXDefault(typeof(Search2<CSAnswers.value, InnerJoin<Vendor, On<Vendor.noteID, Equal<CSAnswers.refNoteID>,
                                                                       And<CSAnswers.attributeID, Equal<TaxNbrNameAtt>>>>,
                                                   Where<Vendor.bAccountID, Equal<Current<vendorID>>>>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<vendorID>))]
        public virtual string TaxNbr { get; set; }
        public abstract class taxNbr : PX.Data.BQL.BqlString.Field<taxNbr> { }
        #endregion

        #region OurTaxNbr
        public const string OurTaxNbrName = "OURTAXNBR";
        public class OurTaxNbrNameAtt : PX.Data.BQL.BqlString.Constant<OurTaxNbrNameAtt>
        {
            public OurTaxNbrNameAtt() : base(OurTaxNbrName) { }
        }

        [TaxNbrVerify(8, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Our Tax Nbr")]
        [PXDefault(typeof(Search2<CSAnswers.value, InnerJoin<Vendor, On<Vendor.noteID, Equal<CSAnswers.refNoteID>,
                                                                       And<CSAnswers.attributeID, Equal<OurTaxNbrNameAtt>>>>,
                                                   Where<Vendor.bAccountID, Equal<Current<vendorID>>>>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<vendorID>))]
        public virtual string OurTaxNbr { get; set; }
        public abstract class ourTaxNbr : PX.Data.BQL.BqlString.Field<ourTaxNbr> { }
        #endregion

        #region Deduction
        public const string DeductionName = "DEDUCTCODE";
        public class DeductionNameAtt : PX.Data.BQL.BqlString.Constant<DeductionNameAtt>
        {
            public DeductionNameAtt() : base(DeductionName) { }
        }

        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Deduction")]
        [PXSelector(typeof(Search<CSAttributeDetail.valueID,
                                  Where<CSAttributeDetail.attributeID, Equal<DeductionNameAtt>>>),
                    typeof(CSAttributeDetail.description))]
        [PXDefault(typeof(Search2<CSAnswers.value, InnerJoin<Vendor, On<Vendor.noteID, Equal<CSAnswers.refNoteID>,
                                                                       And<CSAnswers.attributeID, Equal<DeductionNameAtt>>>>,
                                                   Where<Vendor.bAccountID, Equal<Current<vendorID>>>>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<vendorID>))]
        public virtual string Deduction { get; set; }
        public abstract class deduction : PX.Data.BQL.BqlString.Field<deduction> { }
        #endregion

        #region NetAmt
        [TWTaxAmountCalc(0, typeof(TWNManualGUIAPBill.taxID), typeof(TWNManualGUIAPBill.netAmt), typeof(TWNManualGUIAPBill.taxAmt))]
        [PXUIField(DisplayName = "Net Amt")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual decimal? NetAmt { get; set; }
        public abstract class netAmt : PX.Data.BQL.BqlDecimal.Field<netAmt> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Tax Amt")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
        #endregion

        #region Remark
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}