using System;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace PX.Objects.AR
{
    public class ARRegisterExt : PXCacheExtension<PX.Objects.AR.ARRegister>
    {
        #region UsrVATOutCode
        public const string VATOUTFRMTName = "VATOUTFRMT";
        public class VATOUTFRMTNameAtt : PX.Data.BQL.BqlString.Constant<VATOUTFRMTNameAtt>
        {
            public VATOUTFRMTNameAtt() : base(VATOUTFRMTName) { }
        }
        
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "VAT Out Code")]
        [PXSelector(typeof(Search<CSAttributeDetail.valueID, Where<CSAttributeDetail.attributeID, Equal<VATOUTFRMTNameAtt>>>),
                    DescriptionField = typeof(CSAttributeDetail.description), 
                    ValidateValue = false)]
        public virtual string UsrVATOutCode { get; set; }
        public abstract class usrVATOutCode : IBqlField { }
        #endregion

        #region UsrTaxNbr
        public const string TaxNbrName = "TAXNBR";
        public class TaxNbrNameAtt : PX.Data.BQL.BqlString.Constant<TaxNbrNameAtt>
        {
           public TaxNbrNameAtt() : base(TaxNbrName) { }
        }

        [TaxNbrVerify(8, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Nbr.")]
        [PXDefault(typeof(Search2<CSAnswers.value, InnerJoin<Customer, On<Customer.noteID, Equal<CSAnswers.refNoteID>>>,
                                                   Where<Customer.bAccountID, Equal<Current<ARRegister.customerID>>,
                                                         And<CSAnswers.attributeID, Equal<TaxNbrNameAtt>>>>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<ARInvoice.customerID>))]
        public virtual string UsrTaxNbr { get; set; }
        public abstract class usrTaxNbr : IBqlField { }
        #endregion

        #region UsrOurTaxNbr
        public const string OurTaxNbrName = "OURTAXNBR";
        public class OurTaxNbrNameAtt : PX.Data.BQL.BqlString.Constant<OurTaxNbrNameAtt>
        {
           public OurTaxNbrNameAtt() : base(OurTaxNbrName) { }
        }

        [TaxNbrVerify(8, IsUnicode = true)]
        [PXUIField(DisplayName = "Our Tax Nbr.")]
        [PXDefault(typeof(Search2<CSAnswers.value, InnerJoin<Customer, On<Customer.noteID, Equal<CSAnswers.refNoteID>>>,
                                                   Where<Customer.bAccountID, Equal<Current<ARRegister.customerID>>,
                                                         And<CSAnswers.attributeID, Equal<OurTaxNbrNameAtt>>>>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<ARInvoice.customerID>))]      
        public virtual string UsrOurTaxNbr { get; set; }
        public abstract class usrOurTaxNbr : IBqlField { }
        #endregion

        #region UsrGUINbr
        public class VATOut33Att : PX.Data.BQL.BqlString.Constant<VATOut33Att>
        {
            public VATOut33Att() : base(TWGUIFormatCode.vATOutCode33) { }
        }

        public class VATOut34Att : PX.Data.BQL.BqlString.Constant<VATOut34Att>
        {
            public VATOut34Att() : base(TWGUIFormatCode.vATOutCode34) { }
        }

        //[GUINumber(1000, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCC")]
        [PXDBString(10000, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI Nbr.")]
        [PXSelector(typeof(Search2<TWNGUITrans.gUINbr, InnerJoin<BAccount, On<BAccount.acctCD, Equal<TWNGUITrans.custVend>>>,
                                                       Where<BAccount.bAccountID, Equal<Current<ARRegister.customerID>>,
                                                           And<TWNGUITrans.gUIStatus,Equal<TWNStringList.TWNGUIStatus.used>,
                                                               And<TWNGUITrans.gUIDirection, Equal<TWNStringList.TWNGUIDirection.issue>,
                                                                   And<TWNGUITrans.netAmtRemain, Greater<decimal0>,
                                                                       And<Where<TWNGUITrans.gUIFormatcode, NotEqual<VATOut33Att>,
                                                                                 And<TWNGUITrans.gUIFormatcode, NotEqual<VATOut34Att>>>>>>>>>),
                    typeof(TWNGUITrans.gUIFormatcode),
                    typeof(TWNGUITrans.gUINbr),
                    typeof(TWNGUITrans.orderNbr),
                    typeof(TWNGUITrans.netAmtRemain),
                    typeof(TWNGUITrans.taxAmtRemain),
                    ValidateValue = false,
                    Filterable = true)]
        public virtual string UsrGUINbr { get; set; }
        public abstract class usrGUINbr : IBqlField { }
        #endregion

        #region UsrGUIDate
        [PXDBDate]
        [PXUIField(DisplayName = "GUI Date")]
        [PXDefault(typeof(AccessInfo.businessDate),  PersistingCheck = PXPersistingCheck.Nothing)]        
        public virtual DateTime? UsrGUIDate { get; set; }
        public abstract class usrGUIDate : IBqlField { }
        #endregion

        #region UsrB2CType
        [PXDBString(3, IsUnicode = true)]
        [PXUIField(DisplayName = "B2C Type")]
        [PXDefault(TWNStringList.TWNB2CType.DEF, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrB2CType { get; set; }
        public abstract class usrB2CType : PX.Data.BQL.BqlString.Field<usrB2CType> { }
        #endregion

        #region UsrCarrierID
        [PXDBString(64, IsUnicode = true)]
        [PXUIField(DisplayName = "Carrier ID")]
        public virtual string UsrCarrierID { get; set; }
        public abstract class usrCarrierID : PX.Data.BQL.BqlString.Field<usrCarrierID> { }
        #endregion

        #region UsrNPONbr
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "NPO Nbr.", Visibility = PXUIVisibility.SelectorVisible, IsDirty = true)]
        [NPONbrSelector]
        public virtual string UsrNPONbr { get; set; }
        public abstract class usrNPONbr : PX.Data.BQL.BqlString.Field<usrNPONbr> { }
        #endregion

        #region UsrCreditAction
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Credit Action")]
        [PXDefault(TWNStringList.TWNCreditAction.NO, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrCreditAction { get; set; }
        public abstract class usrCreditAction : PX.Data.BQL.BqlString.Field<usrCreditAction> { }
        #endregion

        #region UsrGUITitle
        [PXDBString(80, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI Title")]
        public virtual string UsrGUITitle { get; set; }
        public abstract class usrGUITitle : PX.Data.BQL.BqlString.Field<usrGUITitle> { }
        #endregion

        #region UsrVATType
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "VAT Type")]
        [TWNStringList.TWNGUIVATType.List]
        public virtual string UsrVATType { get; set; }
        public abstract class usrVATType : PX.Data.BQL.BqlString.Field<usrVATType> { }
        #endregion

        #region UsrSummaryPrint
        [PXDBBool()]
        [PXUIField(DisplayName = "Summary Printing")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrSummaryPrint { get; set; }
        public abstract class usrSummaryPrint : PX.Data.BQL.BqlBool.Field<usrSummaryPrint> { }
        #endregion

        #region UsrGUISummary
        public const string GUISummary = "GUISUMMARY";
        public class GUISmryAttr : PX.Data.BQL.BqlString.Constant<GUISmryAttr>
        {
            public GUISmryAttr() : base(GUISummary) { }
        }

        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Invoice Summary")]
        [PXSelector(typeof(Search<CSAttributeDetail.valueID, Where<CSAttributeDetail.attributeID, Equal<GUISmryAttr>>>),
                    typeof(CSAttributeDetail.valueID),
                    DescriptionField = typeof(CSAttributeDetail.description))]
        public virtual string UsrGUISummary { get; set; }
        public abstract class usrGUISummary : PX.Data.BQL.BqlString.Field<usrGUISummary> { }
        #endregion

        #region UsrShowTWGUITab
        [PXBool()]
        [PXUIField(Visible = false)]
        [PXFormula(typeof(Switch<Case<Where<ARRegister.docType, Equal<ARDocType.prepayment>, Or<ARRegister.docType, Equal<ARDocType.voidPayment>>>, True>, False>))]
        public virtual bool? UsrShowTWGUITab { get; set; }
        public abstract class usrShowTWGUITab : PX.Data.BQL.BqlBool.Field<usrShowTWGUITab> { }
        #endregion
    }
}