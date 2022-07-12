using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.TX;
using eGUICustomizations.DAC;
using eGUICustomizations.Graph_Release;
using eGUICustomizations.Descriptor;

namespace PX.Objects.EP
{
    public class EPReleaseProcess_Extension : PXGraphExtension<EPReleaseProcess>
    {
        public static bool IsActive() => TWNGUIValidation.ActivateTWGUI(new PXGraph());

        #region Delegate Methods
        public delegate void ReleaseDocProcDelegate(EPExpenseClaim claim);
        [PXOverride]
        public void ReleaseDocProc(EPExpenseClaim claim, ReleaseDocProcDelegate baseMethod)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                baseMethod(claim);

                if (TWNGUIValidation.ActivateTWGUI(Base) == true &&
                    claim != null && claim.Released == true)
                {
                    TWNReleaseProcess rp = PXGraph.CreateInstance<TWNReleaseProcess>();

                    Vendor vendor = new Vendor();

                    foreach (TWNManualGUIExpense manualGUIExp in SelectFrom<TWNManualGUIExpense>.Where<TWNManualGUIExpense.refNbr.IsEqual<@P.AsString>>.View.Select(Base, claim.RefNbr))
                    {
                        if (PXCache<Tax>.GetExtension<TaxExt>(Tax.PK.Find(Base, manualGUIExp.TaxID)).UsrTWNGUI == false) { continue; }
                    
                        if (manualGUIExp.VendorID != null)
                        {
                            vendor = Vendor.PK.Find(Base, manualGUIExp.VendorID);
                        }

                        rp.CreateGUITrans(new STWNGUITran()
                        {
                            VATCode       = manualGUIExp.VATInCode,
                            GUINbr        = manualGUIExp.GUINbr,
                            GUIStatus     = TWNStringList.TWNGUIStatus.Used,
                            BranchID      = claim.BranchID,
                            GUIDirection  = TWNStringList.TWNGUIDirection.Receipt,
                            GUIDate       = manualGUIExp.GUIDate,
                            GUITitle      = vendor.AcctName,
                            TaxZoneID     = manualGUIExp.TaxZoneID,
                            TaxCategoryID = manualGUIExp.TaxCategoryID,
                            TaxID         = manualGUIExp.TaxID,
                            TaxNbr        = manualGUIExp.TaxNbr,
                            OurTaxNbr     = manualGUIExp.OurTaxNbr,
                            NetAmount     = manualGUIExp.NetAmt,
                            TaxAmount     = manualGUIExp.TaxAmt,
                            AcctCD        = vendor.AcctCD,
                            AcctName      = vendor.AcctName,
                            DeductionCode = manualGUIExp.Deduction,
                            Remark        = manualGUIExp.Remark,
                            OrderNbr      = manualGUIExp.RefNbr
                        });
                    }
                }

                ts.Complete(Base);
            }
        }
        #endregion
    }
}