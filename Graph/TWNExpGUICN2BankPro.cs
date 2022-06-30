using System;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.AR;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;
using static eGUICustomizations.Graph.TWNExpGUIInv2BankPro;

namespace eGUICustomizations.Graph
{
    public class TWNExpGUICN2BankPro : PXGraph<TWNExpGUICN2BankPro>
    {
        #region Process & Setup
        public PXCancel<TWNGUITrans> Cancel;
        public PXProcessing<TWNGUITrans,
                            Where<TWNGUITrans.eGUIExcluded.IsNotEqual<True>
                                  .And<TWNGUITrans.eGUIExported.IsNotEqual<True>
                                      .And<TWNGUITrans.gUIFormatcode.IsEqual<ARRegisterExt.VATOut33Att>
                                          .And<TWNGUITrans.taxNbr.IsNotNull>>>>> GUITranProc;
                            //Where<TWNGUITrans.eGUIExcluded, Equal<False>,
                            //      And2<Where<TWNGUITrans.eGUIExported, Equal<False>,
                            //                 Or<TWNGUITrans.eGUIExported, IsNull>>,
                            //          And<TWNGUITrans.gUIFormatcode, Equal<ARRegisterExt.VATOut33Att>>>>> GUITranProc;
        public PXSetup<TWNGUIPreferences> gUIPreferSetup;
        #endregion

        #region Ctor
        public TWNExpGUICN2BankPro()
        {
            GUITranProc.SetProcessCaption(ActionsMessages.Upload);
            GUITranProc.SetProcessAllCaption(TWMessages.UploadAll);
            GUITranProc.SetProcessDelegate(Upload);
        }
        #endregion

        #region Static Methods
        public static void Upload(List<TWNGUITrans> tWNGUITrans)
        {
            try
            {
                // Avoid to create empty content file in automation schedule.
                if (tWNGUITrans.Count == 0) { return; }

                TWNExpGUIInv2BankPro graph = CreateInstance<TWNExpGUIInv2BankPro>();

                string lines = "", fileName = "";

                TWNGUIPreferences preferences = PXSelect<TWNGUIPreferences>.Select(graph);

                fileName = preferences.OurTaxNbr + "-AllowanceMD--Paper-" + DateTime.Today.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("hhmmss") + ".txt";

                foreach (TWNGUITrans gUITrans in tWNGUITrans)
                {
                    // File Type
                    lines += "M" + verticalBar;
                    // Bill type
                    lines += TWNExpGUIInv2BankPro.GetBillType(gUITrans) + verticalBar;
                    // Invoice No
                    lines += verticalBar;
                    // Invoice Date Time
                    lines += verticalBar;
                    // Allowance Date
                    lines += gUITrans.GUIDate.Value.ToString("yyyyMMdd") + verticalBar;
                    // Cancel Date
                    lines += TWNExpGUIInv2BankPro.GetCancelDate(gUITrans) + verticalBar;
                    // Bill Attribute
                    lines += verticalBar;
                    // Seller Ban
                    lines += gUITrans.OurTaxNbr + verticalBar;
                    // Seller Code
                    lines += verticalBar;
                    // Buyer Ban
                    lines += gUITrans.TaxNbr + verticalBar;
                    // Buyer Code
                    lines += verticalBar;
                    // Buyer CName
                    lines += gUITrans.GUITitle + verticalBar;
                    // Sales Amount
                    lines += TWNExpGUIInv2BankPro.GetSalesAmt(gUITrans) + verticalBar;
                    // Tax Type
                    lines += TWNExpGUIInv2BankPro.GetTaxType(gUITrans.VATType) + verticalBar;
                    // Tax Rate
                    lines += TWNExpGUIInv2BankPro.GetTaxRate(gUITrans.VATType) + verticalBar;
                    // Tax Amount
                    lines += TWNExpGUIInv2BankPro.GetTaxAmt(gUITrans) + verticalBar;
                    // Total Amount
                    lines += (gUITrans.NetAmount + gUITrans.TaxAmount).Value + verticalBar;
                    // Health Tax
                    lines += "0" + verticalBar;
                    // Buyer Remark
                    lines += verticalBar;
                    // Main Remark
                    lines += verticalBar;
                    // Order No = Relate Number1
                    lines += (gUITrans.OrderNbr.Length > 16) ? gUITrans.OrderNbr.Substring(0, 16) : gUITrans.OrderNbr + verticalBar;
                    // Relate Number2                           
                    // Relate Number3
                    // Relate Number4
                    // Relate Number5
                    // Group Mark
                    // Customs Clearance Mark
                    lines += new string(char.Parse(verticalBar), 5) + TWNExpGUIInv2BankPro.GetCustomClearance(gUITrans) + verticalBar;
                    // Bonded Area Enum
                    lines += verticalBar;
                    // Random Number
                    lines += (gUITrans.BatchNbr != null) ? gUITrans.BatchNbr.Substring(0, 4) : null;
                    // Carrier Type
                    // Carrier ID
                    // NPOBAN
                    // Request Paper
                    // Void Reason
                    // Project Number Void Approved
                    lines += new string(char.Parse(verticalBar), 6) + "\r\n";

                    foreach (PXResult<ARTran> result in graph.RetrieveARTran(gUITrans.OrderNbr))
                    {
                        ARTran aRTran = result;

                        // File Type
                        lines += "D" + verticalBar;
                        // Description
                        lines += aRTran.TranDesc + verticalBar;
                        // Quantity
                        lines += aRTran.Qty + verticalBar;
                        // Unit Price
                        // Amount
                        if (gUITrans.TaxNbr != null)
                        {
                            lines += aRTran.UnitPrice + verticalBar;
                            lines += aRTran.TranAmt + verticalBar;
                        }
                        else
                        {
                            lines += (aRTran.UnitPrice * TWNExpGUIInv2BankPro.fixedRate) + verticalBar;
                            lines += (aRTran.TranAmt * TWNExpGUIInv2BankPro.fixedRate) + verticalBar;
                        }
                        // Unit
                        lines += verticalBar;
                        // Package
                        lines += "0" + verticalBar;
                        // Gift Number 1 (Box)
                        lines += "0" + verticalBar;
                        // Gift Number 2 (Piece)
                        lines += "0" + verticalBar;
                        // Order No
                        lines += (gUITrans.OrderNbr.Length > 16) ? gUITrans.OrderNbr.Substring(0, 16) : gUITrans.OrderNbr + verticalBar;
                        // Buyer Barcode
                        // Buyer Prod No
                        // Seller Prod No
                        // Seller Account No
                        // Seller Shipping No
                        // Remark
                        // Relate Number1
                        // Relate Number2 (Invoice No)
                        lines += new string(char.Parse(verticalBar), 7) + gUITrans.GUINbr + verticalBar;
                        // Relate Number3 (Invoice Date)
                        // Relate Number4
                        // Relate Number5
                        lines += gUITrans.GUIDate.Value.ToString("yyyy/MM/dd HH:mm:ss");
                        lines += new string(char.Parse(verticalBar), 2) + "\r\n";
                    }

                    // The following method is only for voided invoice.
                    if (gUITrans.GUIStatus == TWNStringList.TWNGUIStatus.Voided)
                    {
                        TWNExpGUIInv2BankPro.CreateVoidedDetailLine(verticalBar, gUITrans.OrderNbr, ref lines);
                    }
                }

                // Total Records
                lines += tWNGUITrans.Count;

                graph.UpdateGUITran(tWNGUITrans);
                graph.UploadFile2FTP(fileName, lines);
            }
            catch (Exception ex)
            {
                PXProcessing<TWNGUITrans>.SetError(ex);
                throw;
            }
        }
        #endregion
    }
}