using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;
using eGUICustomizations.Descriptor;
using eGUICustomizations.Graph;

namespace eGUICustomizations.DAC
{
    [Serializable]
    [PXCacheName("GUI Invoice/Credit")]
    [PXPrimaryGraph(typeof(TWNCreateNewGUIMaint))]
    public class TWNGUIInvCredit : IBqlTable
    {
        #region RequestID
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Request ID")]
        [AutoNumber(typeof(TWNGUIPreferences.requestNumbering), typeof(AccessInfo.businessDate))]
        [PXSelector(typeof(Search<TWNGUIInvCredit.requestID>), Filterable = true, ValidateValue = false)]
        public virtual string RequestID { get; set; }
        public abstract class requestID : PX.Data.BQL.BqlString.Field<requestID> { }
        #endregion
    
        #region CustomerID
        [CustomerActive()]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion

        #region GUINbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "GUI Nbr.")]
        [PXSelector(typeof(Search2<TWNGUITrans.gUINbr, InnerJoin<Customer, On<Customer.acctCD, Equal<TWNGUITrans.custVend>>>,
                                                       Where<TWNGUITrans.gUIDirection, Equal<TWNStringList.TWNGUIDirection.issue>,
                                                             And<TWNGUITrans.gUIFormatcode, Equal<TWNExpGUIInv2BankPro.VATOutCode35>,
                                                                 And<TWNGUITrans.gUIStatus, Equal<TWNStringList.TWNGUIStatus.used>,
                                                                     And<Customer.bAccountID, Equal<Current<TWNGUIInvCredit.customerID>>>>>>>),
                    typeof(TWNGUITrans.gUIStatus),
                    typeof(TWNGUITrans.gUIDate),
                    typeof(TWNGUITrans.orderNbr))]
        public virtual string GUINbr { get; set; }
        public abstract class gUINbr : PX.Data.BQL.BqlString.Field<gUINbr> { }
        #endregion
    
        #region VATOutCode
        [PXDBString(2, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "VAT Out Code")]
        [PXDefault()]
        public virtual string VATOutCode { get; set; }
        public abstract class vATOutCode : PX.Data.BQL.BqlString.Field<vATOutCode> { }
        #endregion

        #region GUINetAmt
        [TWNetAmount(0)]
        [PXUIField(DisplayName = "GUI Amount")]
        public virtual Decimal? GUINetAmt { get; set; }
        public abstract class gUINetAmt : PX.Data.BQL.BqlDecimal.Field<gUINetAmt> { }
        #endregion
    
        #region GUITaxAmt
        [TWTaxAmount(0)]
        [PXUIField(DisplayName = "GUI Tax")]
        public virtual Decimal? GUITaxAmt { get; set; }
        public abstract class gUITaxAmt : PX.Data.BQL.BqlDecimal.Field<gUITaxAmt> { }
        #endregion
    
        #region CNGUINbr
        [PXDBString(1000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Credit Note Nbr.")]
        public virtual string CNGUINbr { get; set; }
        public abstract class cNGUINbr : PX.Data.BQL.BqlString.Field<cNGUINbr> { }
        #endregion
    
        #region CNVATOutCode
        [PXDBString(2, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "CN VAT Out Code")]
        public virtual string CNVATOutCode { get; set; }
        public abstract class cNVATOutCode : PX.Data.BQL.BqlString.Field<cNVATOutCode> { }
        #endregion

        #region CNNetAmt
        [TWNetAmount(0)]
        [PXUIField(DisplayName = "Credit Note Amt")]
        public virtual Decimal? CNNetAmt { get; set; }
        public abstract class cNNetAmt : PX.Data.BQL.BqlDecimal.Field<cNNetAmt> { }
        #endregion

        #region CNTaxAmt
        [TWTaxAmount(0)]
        [PXUIField(DisplayName = "Credit Note Tax")]
        public virtual Decimal? CNTaxAmt { get; set; }
        public abstract class cNTaxAmt : PX.Data.BQL.BqlDecimal.Field<cNTaxAmt> { }
        #endregion
    
        #region NewGUINbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "New GUI Nbr.")]
        public virtual string NewGUINbr { get; set; }
        public abstract class newGUINbr : PX.Data.BQL.BqlString.Field<newGUINbr> { }
        #endregion
    
        #region NewGUIDate
        [PXDBDate()]
        [PXUIField(DisplayName = "New GUI Date")]
        [PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? NewGUIDate { get; set; }
        public abstract class newGUIDate : PX.Data.BQL.BqlDateTime.Field<newGUIDate> { }
        #endregion

        #region CalcNetAmt
        [TWNetAmount(0)]
        [PXUIField(DisplayName = "Calculation Net Amt")]
        public virtual Decimal? CalcNetAmt { get; set; }
        public abstract class calcNetAmt : PX.Data.BQL.BqlDecimal.Field<calcNetAmt> { }
        #endregion

        #region CalcTaxAmt
        [TWTaxAmount(0)]
        [PXUIField(DisplayName = "Calculation Tax Amt")]
        public virtual Decimal? CalcTaxAmt { get; set; }
        public abstract class calcTaxAmt : PX.Data.BQL.BqlDecimal.Field<calcTaxAmt> { }
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