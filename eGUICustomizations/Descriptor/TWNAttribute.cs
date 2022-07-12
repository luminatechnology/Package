using System;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.TX;
using eGUICustomizations.DAC;

namespace eGUICustomizations.Descriptor
{
    #region ChineseAmountAttribute
    /// <summary>
    /// Convert numbers to Chinese tranditional word.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class ChineseAmountAttribute : PXStringAttribute, IPXFieldSelectingSubscriber
    {
        void IPXFieldSelectingSubscriber.FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            ARInvoice invoice = sender.Graph.Caches[typeof(ARInvoice)].Current as ARInvoice;

            if (invoice != null)
            {
                e.ReturnValue = ARInvoiceEntry_Extension.AmtInChinese((int)invoice.CuryDocBal);
            }
        }
    }
    #endregion

    #region ARGUINbrAutoNumAttribute
    public class ARGUINbrAutoNumAttribute : PX.Objects.CS.AutoNumberAttribute
    {
        public ARGUINbrAutoNumAttribute(Type doctypeField, Type dateField) : base(doctypeField, dateField) { }

        public ARGUINbrAutoNumAttribute(Type doctypeField, Type dateField, string[] doctypeValues, Type[] setupFields) : base(doctypeField, dateField, doctypeValues, setupFields) { }

        public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e.Operation == PXDBOperation.Delete) { return; }

            string vATOutCode = string.Empty;

            if (this.BqlTable.Name == nameof(DAC.TWNManualGUIAR))
            {
                var row = (TWNManualGUIAR)e.Row;

                vATOutCode = row.VatOutCode;
            }
            else
            {
                var row = (ARRegister)e.Row;

                vATOutCode = PXCache<ARRegister>.GetExtension<ARRegisterExt>(row).UsrVATOutCode;
            }

            if (vATOutCode != TWGUIFormatCode.vATOutCode33 && vATOutCode != TWGUIFormatCode.vATOutCode34 && vATOutCode != null)
            {
                base.RowPersisting(sender, e);
            }

            sender.SetValue(e.Row, _FieldName, (string)sender.GetValue(e.Row, _FieldName));
        }
    }
    #endregion

    #region GUINumberAttribute
    public class GUINumberAttribute : PXDBStringAttribute, IPXFieldVerifyingSubscriber
    {
        public GUINumberAttribute(int length) : base(length) { }

        public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (string.IsNullOrEmpty((string)e.NewValue) || TWNGUIValidation.ActivateTWGUI(new PXGraph()) == false) { return; }

            bool reverse = false;
            string vATCode = null;
            string erroMsg = null;

            switch (this.BqlTable.Name)
            {
                case nameof(ARRegister):
                    vATCode = (string)sender.GetValue<ARRegisterExt.usrVATOutCode>(e.Row);
                    reverse = sender.GetValue<ARRegister.docType>(e.Row).Equals(ARDocType.CreditMemo);
                    break;
                case nameof(TWNGUITrans):
                    vATCode = (string)sender.GetValue<TWNGUITrans.gUIFormatcode>(e.Row);
                    break;
                case nameof(TWNManualGUIAP):
                    vATCode = (string)sender.GetValue<TWNManualGUIAP.vATInCode>(e.Row);
                    break;
                case nameof(TWNManualGUIAR):
                    vATCode = (string)sender.GetValue<TWNManualGUIAR.vatOutCode>(e.Row);
                    break;
                case nameof(TWNManualGUIBank):
                    vATCode = (string)sender.GetValue<TWNManualGUIBank.vATInCode>(e.Row);
                    break;
                case nameof(TWNManualGUIExpense):
                    vATCode = (string)sender.GetValue<TWNManualGUIExpense.vATInCode>(e.Row);
                    break;
                case nameof(TWNManualGUIAPBill):
                    vATCode = (string)sender.GetValue<TWNManualGUIAPBill.vATInCode>(e.Row);
                    break;
            }

            if (!vATCode.IsIn(TWGUIFormatCode.vATOutCode33, TWGUIFormatCode.vATOutCode34))
            {
                erroMsg = (vATCode.IsIn(TWGUIFormatCode.vATInCode21, TWGUIFormatCode.vATInCode23, TWGUIFormatCode.vATInCode25) && e.NewValue.ToString().Length != 10) ? TWMessages.GUINbrLength :
                                                                                                                                                                        (e.NewValue.ToString().Length < 10) ? TWMessages.GUINbrMini :
                                                                                                                                                                                                              null;
            }

            if (!string.IsNullOrEmpty(erroMsg))
            {
                throw new PXSetPropertyException(erroMsg, PXErrorLevel.Error);
            }

            if (reverse == false)
            {
                new TWNGUIValidation().CheckGUINbrExisted(sender.Graph, (string)e.NewValue, vATCode);
            }
        }
    }
    #endregion

    #region TaxNbrVerifyAttribute
    public class TaxNbrVerifyAttribute : PXDBStringAttribute, IPXFieldVerifyingSubscriber
    {
        public TaxNbrVerifyAttribute(int length) : base(length) { }

        public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (e.NewValue != null)
            {
                string vATInCode = (string)sender.GetValue(e.Row, nameof(TWNGUITrans.GUIFormatcode));

                if (string.IsNullOrEmpty(vATInCode))
                {
                    vATInCode = (string)sender.GetValue(e.Row, nameof(TWNManualGUIAP.VATInCode));
                }

                ///<remarks>VAT in code = 27 匯總申報發票, Tax Nbr輸入四碼張數檢核</remarks>
                if (vATInCode != TWGUIFormatCode.vATInCode27)
                {
                    TWNGUIValidation validation = new TWNGUIValidation();

                    validation.CheckTabNbr(e.NewValue.ToString());

                    if (validation.errorOccurred == true)
                    {
                        throw new PXSetPropertyException(validation.errorMessage, (PXErrorLevel)validation.errorLevel);
                    }
                }
            }
        }
    }
    #endregion

    #region TWNetAmountAttribute
    public class TWNetAmountAttribute : PXDBDecimalAttribute, IPXFieldVerifyingSubscriber
    {
        public TWNetAmountAttribute(int percision) : base(percision) { }

        public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if ((decimal)e.NewValue < 0)
            {
                // Throwing an exception to cancel assignment of the new value to the field
                throw new PXSetPropertyException(TWMessages.NetAmtNegError);
            }
        }
    }
    #endregion

    #region TWTaxAmountAttribute
    public class TWTaxAmountAttribute : PXDBDecimalAttribute, IPXFieldVerifyingSubscriber
    {
        protected Type _NetAmt;

        public TWTaxAmountAttribute(int percision) : base(percision) { }

        public TWTaxAmountAttribute(int percision, Type netAmt) : base(percision)
        {
            _NetAmt = netAmt;
        }

        public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if ((decimal)e.NewValue < 0)
            {
                throw new PXSetPropertyException(TWMessages.TaxAmtNegError);
            }

            if (_NetAmt != null)
            {
                throw new TWNGUIValidation().CheckTaxAmount(sender, (decimal)sender.GetValue(e.Row, _NetAmt.Name), (decimal)e.NewValue);
            }
        }
    }
    #endregion

    #region TWTaxAmountCalcAttribute
    public class TWTaxAmountCalcAttribute : TWNetAmountAttribute, IPXFieldUpdatedSubscriber
    {
        protected Type _TaxID;
        protected Type _NetAmt;
        protected Type _TaxAmt;

        public TWTaxAmountCalcAttribute(int percision, Type taxID, Type netAmt, Type taxAmt) : base(percision)
        {
            _TaxID = taxID;
            _NetAmt = netAmt;
            _TaxAmt = taxAmt;
        }

        public virtual void FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            string taxID = (string)sender.GetValue(e.Row, _TaxID.Name);
            decimal netAmt = (decimal)sender.GetValue(e.Row, _NetAmt.Name);

            foreach (TaxRev taxRev in SelectFrom<TaxRev>.Where<TaxRev.taxID.IsEqual<@P.AsString>
                                                               .And<TaxRev.taxType.IsEqual<@P.AsString>>>.View.Select(sender.Graph, taxID, "P")) // P = Group type (Input)
            {
                decimal taxAmt = Math.Round(netAmt * (taxRev.TaxRate.Value / taxRev.NonDeductibleTaxRate.Value), 0);

                sender.SetValue(e.Row, _TaxAmt.Name, taxAmt);
            }
        }
    }
    #endregion

    #region TypeOfInSelectorAttribute
    public class TypeOfInSelectorAttribute : PXSelectorAttribute
    {
        public TypeOfInSelectorAttribute() : base(typeof(Search<CSAttributeDetail.valueID, Where<CSAttributeDetail.attributeID, Equal<TWNWHT.TypeOfInAtt>>>),
                                                  typeof(CSAttributeDetail.description))
        {
            Filterable = true;
            DirtyRead = true;
            DescriptionField = typeof(CSAttributeDetail.description);
        }
    }
    #endregion

    #region WHTFmtCodeSelectorAttribute
    public class WHTFmtCodeSelectorAttribute : PXSelectorAttribute
    {
        public WHTFmtCodeSelectorAttribute() : base(typeof(Search<CSAttributeDetail.valueID, Where<CSAttributeDetail.attributeID, Equal<TWNWHT.WHTFmtCodeAtt>>>),
                                                    typeof(CSAttributeDetail.description))
        {
            Filterable = true;
            DirtyRead = true;
            DescriptionField = typeof(CSAttributeDetail.description);
        }
    }
    #endregion

    #region WHTFmtSubSelectorAttribute
    public class WHTFmtSubSelectorAttribute : PXSelectorAttribute
    {
        public WHTFmtSubSelectorAttribute() : base(typeof(Search<CSAttributeDetail.valueID, Where<CSAttributeDetail.attributeID, Equal<TWNWHT.WHTFmtSubAtt>>>),
                                                   typeof(CSAttributeDetail.description))
        {
            Filterable = true;
            DirtyRead = true;
            DescriptionField = typeof(CSAttributeDetail.description);
        }
    }
    #endregion

    #region SecNHICodeSelectorAttribute
    public class SecNHICodeSelectorAttribute : PXSelectorAttribute
    {
        public SecNHICodeSelectorAttribute() : base(typeof(Search<CSAttributeDetail.valueID, Where<CSAttributeDetail.attributeID, Equal<TWNWHT.SecNHICodeAtt>>>),
                                                    typeof(CSAttributeDetail.description))
        {
            Filterable = true;
            DirtyRead = true;
            DescriptionField = typeof(CSAttributeDetail.description);
        }
    }
    #endregion
}