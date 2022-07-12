using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.TX;
using PX.Objects.AP;
using eGUICustomizations.DAC;
using eGUICustomizations.Graph_Release;
using eGUICustomizations.Descriptor;
using static eGUICustomizations.Descriptor.TWNStringList;

namespace PX.Objects.CA
{
    public class CAReleaseProcess_Extension : PXGraphExtension<CAReleaseProcess>
    {
        #region Delegate Functions
        public delegate void OnReleaseCompleteDelegate(ICADocument doc);
        [PXOverride]
        public void OnReleaseComplete(ICADocument doc, OnReleaseCompleteDelegate baseMethod)
        {
            CAAdj cAAdj = doc as CAAdj;

            if (TWNGUIValidation.ActivateTWGUI(Base) == true &&
                cAAdj != null &&
                cAAdj.Released == true &&
                cAAdj.AdjTranType == CATranType.CAAdjustment )
            {
                TWNReleaseProcess rp = PXGraph.CreateInstance<TWNReleaseProcess>();

                PXSelectBase<TWNManualGUIBank> ViewManGUIBank = new SelectFrom<TWNManualGUIBank>.Where<TWNManualGUIBank.adjRefNbr.IsEqual<@P.AsString>>.View(Base);

                foreach (TWNManualGUIBank manGUIBank in ViewManGUIBank.Cache.Cached)
                {
                    if (PXCache<Tax>.GetExtension<TaxExt>(Tax.PK.Find(Base, manGUIBank.TaxID)).UsrTWNGUI.Equals(false) ) { continue; }

                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        rp.CreateGUITrans(new STWNGUITran()
                        {
                            VATCode       = manGUIBank.VATInCode,
                            GUINbr        = manGUIBank.GUINbr,
                            GUIStatus     = TWNGUIStatus.Used,
                            BranchID      = Base.CATranCashTrans_Ordered.Current.BranchID,
                            GUIDirection  = TWNGUIDirection.Receipt,
                            GUIDate       = manGUIBank.GUIDate,
                            GUITitle      = (string)PXSelectorAttribute.GetField(ViewManGUIBank.Cache, manGUIBank,
                                                                                 typeof(APRegister.vendorID).Name, manGUIBank.VendorID,
                                                                                 typeof(Vendor.acctName).Name),
                            TaxZoneID     = manGUIBank.TaxZoneID,
                            TaxCategoryID = manGUIBank.TaxCategoryID,
                            TaxID         = manGUIBank.TaxID,
                            TaxNbr        = manGUIBank.TaxNbr,
                            OurTaxNbr     = manGUIBank.OurTaxNbr,
                            NetAmount     = manGUIBank.NetAmt,
                            TaxAmount     = manGUIBank.TaxAmt,
                            AcctCD        = (string)PXSelectorAttribute.GetField(ViewManGUIBank.Cache, manGUIBank,
                                                                                 typeof(APRegister.vendorID).Name, manGUIBank.VendorID,
                                                                                 typeof(Vendor.acctCD).Name),
                            AcctName      = (string)PXSelectorAttribute.GetField(ViewManGUIBank.Cache, manGUIBank,
                                                                                 typeof(APRegister.vendorID).Name, manGUIBank.VendorID,
                                                                                 typeof(Vendor.acctName).Name),
                            DeductionCode = manGUIBank.Deduction,
                            Remark        = manGUIBank.Remark,
                            DocType       = cAAdj.DocType,
                            OrderNbr      = manGUIBank.AdjRefNbr
                        });

                        ts.Complete(Base);
                    }
                }
            }

            baseMethod(doc);
        }
        #endregion
    }
}