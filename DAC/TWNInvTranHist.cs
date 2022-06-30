using System;
using PX.Data;

namespace eGUICustomizations.DAC
{
    [Serializable]
    [PXCacheName("Invoice Transaction History")]
    public class TWNInvTranHist : IBqlTable
    {
        #region RequestID
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Request ID")]
        public virtual string RequestID { get; set; }
        public abstract class requestID : PX.Data.BQL.BqlString.Field<requestID> { }
        #endregion
    
        #region InvoiceType
        [PXDBString(3, IsKey = true, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Invoice Type")]
        public virtual string InvoiceType { get; set; }
        public abstract class invoiceType : PX.Data.BQL.BqlString.Field<invoiceType> { }
        #endregion
    
        #region InvoiceNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Invoice Nbr.")]
        public virtual string InvoiceNbr { get; set; }
        public abstract class invoiceNbr : PX.Data.BQL.BqlString.Field<invoiceNbr> { }
        #endregion
    
        #region InvTranLineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Tran Line Nbr")]
        public virtual int? InvTranLineNbr { get; set; }
        public abstract class invTranLineNbr : PX.Data.BQL.BqlInt.Field<invTranLineNbr> { }
        #endregion
    
        #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion
    
        #region Qty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Qty.")]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion
    
        #region UnitPrice
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Unit Price")]
        public virtual Decimal? UnitPrice { get; set; }
        public abstract class unitPrice : PX.Data.BQL.BqlDecimal.Field<unitPrice> { }
        #endregion
    
        #region TranAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tran Amt")]
        public virtual Decimal? TranAmt { get; set; }
        public abstract class tranAmt : PX.Data.BQL.BqlDecimal.Field<tranAmt> { }
        #endregion
    
        #region Tstamp
        [PXDBTimestamp()]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}