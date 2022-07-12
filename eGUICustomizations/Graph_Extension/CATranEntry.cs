using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.TX;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace PX.Objects.CA
{
    public class CATranEntry_Extension : PXGraphExtension<CATranEntry>
    {
        #region Select & Setup
        // Retrieves detail records by CAAdj.adjRefNbr of the current master record.
        public SelectFrom<TWNManualGUIBank>
                         .Where<TWNManualGUIBank.adjRefNbr.IsEqual<CAAdj.adjRefNbr.FromCurrent>>.View ManGUIBank;

        public PXSetup<TWNGUIPreferences> GUIPreferences;
        #endregion

        #region Event Handlers
        public bool activateGUI = TWNGUIValidation.ActivateTWGUI(new PXGraph());

        TWNGUIValidation tWNGUIValidation = new TWNGUIValidation();

        protected void _(Events.RowSelected<CAAdj> e, PXRowSelected InvokeBaseHandler)
        {
            InvokeBaseHandler?.Invoke(e.Cache, e.Args);

            ManGUIBank.Cache.AllowSelect = activateGUI;
            ManGUIBank.Cache.AllowDelete = ManGUIBank.Cache.AllowInsert = ManGUIBank.Cache.AllowUpdate = !e.Row.Status.Equals(CATransferStatus.Released);
        }

        protected void _(Events.RowPersisting<CAAdj> e, PXRowPersisting baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            if (Base.CurrentDocument.Current == null || activateGUI.Equals(false)) { return; }

            decimal taxSum = 0;

            foreach (TWNManualGUIBank row in ManGUIBank.Select())
            {
                tWNGUIValidation.CheckCorrespondingInv(Base, row.GUINbr, row.VATInCode);

                if (tWNGUIValidation.errorOccurred.Equals(true))
                {
                    e.Cache.RaiseExceptionHandling<TWNManualGUIExpense.gUINbr>(e.Row, row.GUINbr, new PXSetPropertyException(tWNGUIValidation.errorMessage, PXErrorLevel.RowError));
                }

                taxSum += row.TaxAmt.Value;
            }

            if (taxSum != Base.CurrentDocument.Current.TaxTotal)
            {
                throw new PXException(TWMessages.ChkTotalGUIAmt);
            }
        }

        protected virtual void _(Events.FieldDefaulting<TWNManualGUIBank.deduction> e)
        {
            var row = (TWNManualGUIBank)e.Row;

            /// If user doesn't choose a vendor then bring the fixed default value from Attribure "DEDUCTCODE" first record.
            e.NewValue = row.VendorID == null ? "1" : e.NewValue;
        }

        protected void _(Events.FieldDefaulting<TWNManualGUIBank.ourTaxNbr> e)
        {
            var row = e.Row as TWNManualGUIBank;

            e.NewValue = row.VendorID == null ? GUIPreferences.Current.OurTaxNbr : e.NewValue;
        }

        protected virtual void _(Events.FieldVerifying<TWNManualGUIBank.gUINbr> e)
        {
            var row = (TWNManualGUIBank)e.Row;

            tWNGUIValidation.CheckGUINbrExisted(Base, (string)e.NewValue, row.VATInCode);
        }

        protected virtual void _(Events.FieldVerifying<TWNManualGUIBank.taxAmt> e)
        {
            var row = (TWNManualGUIBank)e.Row;

            e.Cache.RaiseExceptionHandling<TWNManualGUIBank.taxAmt>(row, e.NewValue, tWNGUIValidation.CheckTaxAmount(e.Cache, row.NetAmt.Value, (decimal)e.NewValue));
        }

        protected virtual void _(Events.FieldUpdated< TWNManualGUIBank.netAmt> e)
        {        
            var row = (TWNManualGUIBank)e.Row;

            foreach (TaxRev taxRev in SelectFrom<TaxRev>.Where<TaxRev.taxID.IsEqual<@P.AsString>
                                                               .And<TaxRev.taxType.IsEqual<TaxRev.taxType>>>.View.Select(Base, row.TaxID, "P")) // P = Group type (Input)
            {
                row.TaxAmt = row.NetAmt * (taxRev.TaxRate / taxRev.NonDeductibleTaxRate);
            }
        }
        #endregion
    }
}