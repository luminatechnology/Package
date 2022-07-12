using System;
using PX.Data;
using PX.Objects.AP;
using eGUICustomizations.Graph;
using eGUICustomizations.Descriptor;
using PX.Objects.GL;

namespace eGUICustomizations.DAC
{
    [Serializable]
    [PXCacheName("Withholding Tax Trans")]
    [PXPrimaryGraph(typeof(TWNWHTInquiry))]
    public class TWNWHTTran : IBqlTable
    {
        #region Selected       
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected", Visible = true, Enabled = false, Visibility = PXUIVisibility.Invisible)]
        public bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region DocType
        [PXDBString(3, IsKey = true, IsFixed = true)]
        [PXUIField(DisplayName = "Doc Type")]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ref. Nbr.")]
        [PXSelector(typeof(Search<APInvoice.refNbr, Where<APInvoice.docType, Equal<Current<docType>>,
                                                          And<APInvoice.refNbr, Equal<Current<refNbr>>>>>))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region BatchNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Batch Nbr.")]
        [PXSelector(typeof(Search<Batch.batchNbr, Where<Batch.module, Equal<BatchModule.moduleAP>>>))]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
        #endregion

        #region TranDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Tran Date")]
        public virtual DateTime? TranDate { get; set; }
        public abstract class tranDate : PX.Data.BQL.BqlDateTime.Field<tranDate> { }
        #endregion

        #region PaymDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Payment Date")]
        public virtual DateTime? PaymDate { get; set; }
        public abstract class paymDate : PX.Data.BQL.BqlDateTime.Field<paymDate> { }
        #endregion

        #region PaymRef
        [PXDBString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "Payment Ref.")]
        public virtual string PaymRef { get; set; }
        public abstract class paymRef : PX.Data.BQL.BqlString.Field<paymRef> { }
        #endregion

        #region PersonalID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Personal Tax ID")]
        public virtual string PersonalID { get; set; }
        public abstract class personalID : PX.Data.BQL.BqlString.Field<personalID> { }
        #endregion

        #region PropertyID
        [PXDBString(12, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Property ID")]
        public virtual string PropertyID { get; set; }
        public abstract class propertyID : PX.Data.BQL.BqlString.Field<propertyID> { }
        #endregion

        #region TypeOfIn
        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Type Of Income")]
        [TypeOfInSelector]
        public virtual string TypeOfIn { get; set; }
        public abstract class typeOfIn : PX.Data.BQL.BqlString.Field<typeOfIn> { }
        #endregion

        #region WHTFmtCode
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Format Code")]
        [WHTFmtCodeSelector]
        public virtual string WHTFmtCode { get; set; }
        public abstract class wHTFmtCode : PX.Data.BQL.BqlString.Field<wHTFmtCode> { }
        #endregion

        #region WHTFmtSub
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Format Sub Code")]
        [WHTFmtSubSelector]
        public virtual string WHTFmtSub { get; set; }
        public abstract class wHTFmtSub : PX.Data.BQL.BqlString.Field<wHTFmtSub> { }
        #endregion

        #region PayeeName
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payee Name")]
        public virtual string PayeeName { get; set; }
        public abstract class payeeName : PX.Data.BQL.BqlString.Field<payeeName> { }
        #endregion

        #region PayeeAddr
        [PXDBString(70, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payee Address")]
        public virtual string PayeeAddr { get; set; }
        public abstract class payeeAddr : PX.Data.BQL.BqlString.Field<payeeAddr> { }
        #endregion

        #region SecNHIPct
        [PXDBDecimal(3)]
        [PXUIField(DisplayName = "2GNHI %")]
        public virtual decimal? SecNHIPct { get; set; }
        public abstract class secNHIPct : PX.Data.BQL.BqlDecimal.Field<secNHIPct> { }
        #endregion

        #region SecNHICode
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "2GNHI Code")]
        [SecNHICodeSelector]
        public virtual string SecNHICode { get; set; }
        public abstract class secNHICode : PX.Data.BQL.BqlString.Field<secNHICode> { }
        #endregion

        #region SecNHIAmt
        [Descriptor.TWNetAmount(0)]
        [PXUIField(DisplayName = "2GNHI Amount")]
        public virtual decimal? SecNHIAmt { get; set; }
        public abstract class secNHIAmt : PX.Data.BQL.BqlDecimal.Field<secNHIAmt> { }
        #endregion

        #region WHTTaxPct
        [PXDBDecimal()]
        [PXUIField(DisplayName = "WHT Tax %")]
        public virtual decimal? WHTTaxPct { get; set; }
        public abstract class wHTTaxPct : PX.Data.BQL.BqlDecimal.Field<wHTTaxPct> { }
        #endregion

        #region WHTAmt
        [Descriptor.TWNetAmount(0)]
        [PXUIField(DisplayName = "WHT Amount")]
        public virtual decimal? WHTAmt { get; set; }
        public abstract class wHTAmt : PX.Data.BQL.BqlDecimal.Field<wHTAmt> { }
        #endregion

        #region NetAmt
        [Descriptor.TWNetAmount(0)]
        [PXUIField(DisplayName = "Net Amount")]
        public virtual decimal? NetAmt { get; set; }
        public abstract class netAmt : PX.Data.BQL.BqlDecimal.Field<netAmt> { }
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

        #region Noteid
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