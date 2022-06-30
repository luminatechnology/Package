using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.TX;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace PX.Objects.EP
{
    public class ExpenseClaimEntry_Extension : PXGraphExtension<ExpenseClaimEntry>
    {
        #region Selects
        [PXCopyPasteHiddenFields(typeof(TWNManualGUIExpense.gUINbr), typeof(TWNManualGUIExpense.refNbr))]
        public SelectFrom<TWNManualGUIExpense>
                         .Where<TWNManualGUIExpense.refNbr.IsEqual<EPExpenseClaim.refNbr.FromCurrent>>.View manGUIExpense;
        #endregion

        #region Static Methods
        public static bool IsActive() => TWNGUIValidation.ActivateTWGUI(new PXGraph());
        #endregion

        #region Event Handlers
        TWNGUIValidation tWNGUIValidation = new TWNGUIValidation();

        protected void _(Events.RowSelected<EPExpenseClaim> e, PXRowSelected InvokeBaseHandler)
        {
            InvokeBaseHandler?.Invoke(e.Cache, e.Args);

            manGUIExpense.Cache.AllowSelect = TWNGUIValidation.ActivateTWGUI(Base);
            manGUIExpense.Cache.AllowDelete = manGUIExpense.Cache.AllowInsert = manGUIExpense.Cache.AllowUpdate = !e.Row.Status.Equals(EPExpenseClaimStatus.ReleasedStatus);
        }

        protected void _(Events.RowPersisting<EPExpenseClaim> e, PXRowPersisting baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            if (Base.ExpenseClaimCurrent.Current != null)
            {
                decimal taxSum = 0;

                foreach (TWNManualGUIExpense row in manGUIExpense.Select())
                {
                    tWNGUIValidation.CheckCorrespondingInv(Base, row.GUINbr, row.VATInCode);

                    if (tWNGUIValidation.errorOccurred.Equals(true))
                    {
                        e.Cache.RaiseExceptionHandling<TWNManualGUIExpense.gUINbr>(e.Row, row.GUINbr, new PXSetPropertyException(tWNGUIValidation.errorMessage, PXErrorLevel.RowError));
                    }

                    taxSum += row.TaxAmt.Value;
                }

                if (!taxSum.Equals(Base.ExpenseClaimCurrent.Current.TaxTotal))
                {
                    throw new PXException(TWMessages.ChkTotalGUIAmt);
                }
            }
        }

        protected virtual void _(Events.FieldDefaulting<TWNManualGUIExpense.deduction> e)
        {
            var row = (TWNManualGUIExpense)e.Row;

            /// If user doesn't choose a vendor then bring the fixed default value from Attribure "DEDUCTCODE" first record.
            e.NewValue = row.VendorID == null ? "1" : e.NewValue;
        }

        protected virtual void _(Events.FieldDefaulting<TWNManualGUIExpense.ourTaxNbr> e)
        {
            var row = (TWNManualGUIExpense)e.Row;

            TWNGUIPreferences preferences = SelectFrom<TWNGUIPreferences>.View.Select(Base);

            e.NewValue = row.VendorID == null ? preferences.OurTaxNbr : e.NewValue;
        }

        protected virtual void _(Events.FieldVerifying<TWNManualGUIExpense.gUINbr> e)
        {
            var row = (TWNManualGUIExpense)e.Row;

            tWNGUIValidation.CheckGUINbrExisted(Base, (string)e.NewValue, row.VATInCode);
        }

        protected virtual void _(Events.FieldVerifying<TWNManualGUIExpense.taxAmt> e)
        {
            var row = (TWNManualGUIExpense)e.Row;

            e.Cache.RaiseExceptionHandling<TWNManualGUIExpense.taxAmt>(row, e.NewValue, tWNGUIValidation.CheckTaxAmount(e.Cache, row.NetAmt.Value, (decimal)e.NewValue));
        }

        protected virtual void _(Events.FieldUpdated<TWNManualGUIExpense.netAmt> e)
        {         
            var row = (TWNManualGUIExpense)e.Row;

            foreach (TaxRev taxRev in SelectFrom<TaxRev>.Where<TaxRev.taxID.IsEqual<@P.AsString>
                                                               .And<TaxRev.taxType.IsEqual<TaxRev.taxType>>>.View.Select(Base, row.TaxID, "P")) // P = Group type (Input)
            {
                row.TaxAmt = row.NetAmt * (taxRev.TaxRate / taxRev.NonDeductibleTaxRate);
            }
        }
        #endregion
    }
}