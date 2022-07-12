using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.SO;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace eGUICustomizations.Graph
{
    public class TWNExpGUIInv2BankPro : PXGraph<TWNExpGUIInv2BankPro>
    {
        public const decimal fixedRate = (decimal)1.05;
        public const string  verticalBar = "|";

        #region String Constant Classes
        public class VATOutCode31 : PX.Data.BQL.BqlString.Constant<VATOutCode31>
        {
            public VATOutCode31() : base(TWGUIFormatCode.vATOutCode31) { }
        }

        public class VATOutCode32 : PX.Data.BQL.BqlString.Constant<VATOutCode32>
        {
            public VATOutCode32() : base(TWGUIFormatCode.vATOutCode32) { }
        }

        public class VATOutCode35 : PX.Data.BQL.BqlString.Constant<VATOutCode35>
        {
            public VATOutCode35() : base(TWGUIFormatCode.vATOutCode35) { }
        }
        #endregion

        #region Features & Setup
        public PXCancel<TWNGUITrans> Cancel;
        public PXProcessing<TWNGUITrans,
                            Where<TWNGUITrans.eGUIExcluded.IsNotEqual<True>
                                  .And<TWNGUITrans.eGUIExported.IsNotEqual<True>
                                      .And<TWNGUITrans.gUIFormatcode.IsIn<VATOutCode31, VATOutCode32, VATOutCode35>
                                          .And<TWNGUITrans.taxNbr.IsNotNull>>>>> GUITranProc;
        //And2<Where<TWNGUITrans.eGUIExported, Equal<False>,
        //           Or<TWNGUITrans.eGUIExported, IsNull>>,
        //     And<Where<TWNGUITrans.gUIFormatcode, Equal<VATOutCode31>,
        //               Or<TWNGUITrans.gUIFormatcode, Equal<VATOutCode32>,
        //                  Or<TWNGUITrans.gUIFormatcode, Equal<VATOutCode35>>>>>>>> GUITranProc;
        public PXSetup<TWNGUIPreferences> gUIPreferSetup;
        #endregion

        #region Ctor
        public TWNExpGUIInv2BankPro()
        {
            GUITranProc.SetProcessCaption(ActionsMessages.Upload);
            GUITranProc.SetProcessAllCaption(TWMessages.UploadAll);
            GUITranProc.SetProcessDelegate(Upload);
        }
        #endregion

        #region Methods
        public void UpdateGUITran(List<TWNGUITrans> tWNGUITrans)
        {
            foreach (TWNGUITrans trans in tWNGUITrans)
            {
                trans.EGUIExported = true;
                trans.EGUIExportedDateTime = DateTime.UtcNow;

                GUITranProc.Cache.Update(trans);
            }

            this.Actions.PressSave();
        }

        public void UploadFile2FTP(string fileName, string content)
        {
            ///<remarks>
            ///ftpes:// is not a standard protocol prefix. But it is recognized by some FTP clients to mean "Explicit FTP over TLS/SSL".
            ///With FtpWebRequest you specify that by using standard ftp:// protocol prefix and setting EnableSsl = true
            ///</remarks>
            string uRL = gUIPreferSetup.Current.Url.Replace("ftps", "ftp");

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(uRL + fileName));

            request.Credentials = new NetworkCredential(gUIPreferSetup.Current.UserName, gUIPreferSetup.Current.Password);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            //request.EnableSsl = true;

            byte[] data = System.Text.Encoding.UTF8.GetBytes(content);

            request.ContentLength = data.Length;

            Stream requestStream = request.GetRequestStream();

            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            requestStream.Dispose();

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                response.Close();
                /// Close FTP
                request.Abort();

                //throw new PXOperationCompletedException(message);
            }
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

                fileName = preferences.OurTaxNbr + "-InvoiceMD--Paper-" + DateTime.Today.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("hhmmss") + ".txt";

                foreach (TWNGUITrans gUITrans in tWNGUITrans)
                {
                    // File Type
                    lines += "M" + verticalBar;
                    // Bill type
                    lines += GetBillType(gUITrans) + verticalBar;
                    // Invoice No
                    lines += gUITrans.GUINbr + verticalBar;
                    // Invoice Date Time
                    lines += gUITrans.GUIDate.Value.ToString("yyyy/MM/dd HH:mm:ss") + verticalBar;
                    // Allowance Date
                    // Cancel Date
                    lines += verticalBar + GetCancelDate(gUITrans) + verticalBar;
                    // Bill Attribute
                    // Seller Ban
                    lines += verticalBar + gUITrans.OurTaxNbr + verticalBar;
                    // Seller Code
                    lines += verticalBar;
                    // Buyer Ban
                    lines += gUITrans.TaxNbr + verticalBar;
                    // Buyer Code
                    lines += verticalBar;
                    // Buyer CName
                    lines += gUITrans.GUITitle + verticalBar;
                    // Sales Amount
                    lines += GetSalesAmt(gUITrans) + verticalBar;
                    // Tax Type
                    lines += GetTaxType(gUITrans.VATType) + verticalBar;
                    // Tax Rate
                    lines += GetTaxRate(gUITrans.VATType) + verticalBar;
                    // Tax Amount
                    lines += GetTaxAmt(gUITrans) + verticalBar;
                    // Total Amount
                    lines += (gUITrans.NetAmount + gUITrans.TaxAmount).Value + verticalBar;
                    // Health Tax
                    lines += "0" + verticalBar;
                    // Buyer Remark
                    lines += verticalBar;
                    // Main Remark
                    lines += verticalBar;
                    // Order No = Relate Number1
                    lines += gUITrans.OrderNbr + verticalBar;
                    // Relate Number2
                    // Relate Number3
                    // Relate Number4
                    // Relate Number5
                    // Group Mark
                    // Customs Clearance Mark
                    lines += new string(char.Parse(verticalBar), 5) + GetCustomClearance(gUITrans) + verticalBar;
                    // Bonded Area Enum
                    lines += verticalBar;
                    // Random Number
                    lines += (gUITrans.BatchNbr != null) ? gUITrans.BatchNbr.Substring(gUITrans.BatchNbr.Length - 4, 4) : null;
                    lines += verticalBar;
                    // Carrier Type
                    Tuple<string, string, string> tuple = new ARReleaseProcess_Extension().GetB2CTypeValue(string.IsNullOrEmpty(gUITrans.TaxNbr), gUITrans.CarrierID, gUITrans.NPONbr);
                    lines += tuple.Item1 + verticalBar;
                    // Carrier ID
                    lines += tuple.Item2 + verticalBar;
                    // NPOBAN
                    lines += tuple.Item3 + verticalBar;
                    // Request Paper
                    lines += gUITrans.B2CPrinted == true ? "Y" : "N" + verticalBar;
                    // Void Reason
                    // Project Number Void Approved
                    lines += new string(char.Parse(verticalBar), 2) + "\r\n";

                    // The following method is only for voided invoice.
                    if (gUITrans.GUIStatus == TWNStringList.TWNGUIStatus.Voided)
                    {
                        CreateVoidedDetailLine(verticalBar, gUITrans.OrderNbr, ref lines);
                    }
                    else
                    {
                        foreach (PXResult<ARTran> result in graph.RetrieveARTran(gUITrans.OrderNbr))
                        {
                            ARTran aRTran = result;

                            string taxCalcMode = SelectFrom<ARRegister>.Where<ARRegister.docType.IsEqual<@P.AsString>
                                                                              .And<ARRegister.refNbr.IsEqual<@P.AsString>>>
                                                                       .View.ReadOnly.Select(graph, aRTran.TranType, aRTran.RefNbr).TopFirst.TaxCalcMode;
                            // File Type
                            lines += "D" + verticalBar;
                            // Description
                            lines += aRTran.TranDesc + verticalBar;
                            // Quantity
                            lines += (aRTran.Qty ?? 1) + verticalBar;
                            // Unit Price
                            // Amount
                            #region Convert design spec logic to code.
                            //if (aRTran.CuryDiscAmt == 0m)
                            //{
                            //    if (taxCalcMode != PX.Objects.TX.TaxCalculationMode.Gross)
                            //    {
                            //        if (!string.IsNullOrEmpty(gUITrans.TaxNbr))
                            //        {
                            //            lines += aRTran.UnitPrice + verticalBar;
                            //        }
                            //        else
                            //        {
                            //            lines += aRTran.UnitPrice * fixedRate + verticalBar;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (!string.IsNullOrEmpty(gUITrans.TaxNbr))
                            //        {
                            //            lines += aRTran.UnitPrice / fixedRate + verticalBar;
                            //        }
                            //        else
                            //        {
                            //            lines += aRTran.UnitPrice + verticalBar;
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    if (taxCalcMode != PX.Objects.TX.TaxCalculationMode.Gross)
                            //    {
                            //        if (!string.IsNullOrEmpty(gUITrans.TaxNbr))
                            //        {
                            //            lines += aRTran.TranAmt / aRTran.Qty + verticalBar;
                            //        }
                            //        else
                            //        {
                            //            lines += aRTran.TranAmt / aRTran.Qty * fixedRate + verticalBar;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (!string.IsNullOrEmpty(gUITrans.TaxNbr))
                            //        {
                            //            lines += aRTran.TranAmt / aRTran.Qty / fixedRate + verticalBar;
                            //        }
                            //        else
                            //        {
                            //            lines += aRTran.TranAmt / aRTran.Qty + verticalBar;
                            //        }
                            //    }
                            //}
                            #endregion
                            decimal? unitPrice = (aRTran.CuryDiscAmt == 0m) ? aRTran.UnitPrice : (aRTran.TranAmt / aRTran.Qty);
                            decimal? tranAmt = aRTran.TranAmt;

                            if (string.IsNullOrEmpty(gUITrans.TaxNbr) && taxCalcMode != PX.Objects.TX.TaxCalculationMode.Gross)
                            {
                                unitPrice *= fixedRate;
                                tranAmt *= fixedRate;
                            }
                            else if (!string.IsNullOrEmpty(gUITrans.TaxNbr) && taxCalcMode == PX.Objects.TX.TaxCalculationMode.Gross)
                            {
                                unitPrice /= fixedRate;
                                tranAmt /= fixedRate;
                            }
                            lines += string.Format("{0:0.####}", unitPrice) + verticalBar;
                            lines += string.Format("{0:0.####}", tranAmt) + verticalBar;
                            // Unit
                            lines += verticalBar;
                            // Package
                            lines += "0" + verticalBar;
                            // Gift Number 1 (Box)
                            lines += "0" + verticalBar;
                            // Gift Number 2 (Piece)
                            lines += "0" + verticalBar;
                            // Order No
                            lines += gUITrans.OrderNbr;
                            // Buyer Barcode
                            // Buyer Prod No
                            // Seller Prod No
                            // Seller Account No
                            // Seller Shipping No
                            // Remark
                            // Relate Number1
                            // Relate Number2 (Invoice No)
                            // Relate Number3 (Invoice Date)
                            // Relate Number4
                            // Relate Number5
                            lines += new string(char.Parse(verticalBar), 11) + "\r\n";
                        }
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

        public static string GetBillType(TWNGUITrans gUITran)
        {
            string billType = null;

            switch (gUITran.GUIFormatcode)
            {
                case TWGUIFormatCode.vATOutCode35:
                case TWGUIFormatCode.vATOutCode31:
                    if (gUITran.GUIStatus == TWNStringList.TWNGUIStatus.Used) { billType = "O"; }
                    else if (gUITran.GUIStatus == TWNStringList.TWNGUIStatus.Voided) { billType = "C"; }
                    break;

                case TWGUIFormatCode.vATOutCode33:
                    if (gUITran.GUIStatus == TWNStringList.TWNGUIStatus.Used) { billType = "A2"; }
                    else if (gUITran.GUIStatus == TWNStringList.TWNGUIStatus.Voided) { billType = "D"; }
                    break;
            }

            return billType;
        }

        public static string GetTaxType(string vATType)
        {
            if (vATType == TWNStringList.TWNGUIVATType.Five) { return "1"; }
            else if (vATType == TWNStringList.TWNGUIVATType.Zero) { return "2"; }
            else { return "3"; }
        }

        public static string GetTaxRate(string vATType)
        {
            return (vATType == TWNStringList.TWNGUIVATType.Five) ? "0.05" : "0";
        }

        public static string GetCustomClearance(TWNGUITrans gUITran)
        {
            if (gUITran.CustomType == TWNStringList.TWNGUICustomType.NotThruCustom &&
                gUITran.VATType == TWNStringList.TWNGUIVATType.Five)
            {
                return "1";
            }
            if (gUITran.CustomType == TWNStringList.TWNGUICustomType.ThruCustom &&
                gUITran.VATType == TWNStringList.TWNGUIVATType.Zero)
            {
                return "2";
            }
            else
            {
                return null;
            }
        }

        public static string GetCancelDate(TWNGUITrans gUITran)
        {
            return gUITran.GUIStatus ==TWNStringList.TWNGUIStatus.Voided ? gUITran.GUIDate.Value.ToString("yyyyMMdd") : string.Empty;
        }

        public static decimal GetSalesAmt(TWNGUITrans gUITran)
        {
            return string.IsNullOrEmpty(gUITran.TaxNbr) ? (gUITran.NetAmount + gUITran.TaxAmount).Value : gUITran.NetAmount.Value;
        }

        public static decimal GetTaxAmt(TWNGUITrans gUITran)
        {
            return string.IsNullOrEmpty(gUITran.TaxNbr) ? 0 : gUITran.TaxAmount.Value;
        }

        public static void CreateVoidedDetailLine(string verticalBar, string refNbr, ref string lines)
        {
            // File Type
            lines += "D" + verticalBar;
            // Description
            lines += "Service" + verticalBar;
            // Quantity
            lines += "0" + verticalBar;
            // Unit Price
            lines += "0" + verticalBar;
            // Amount
            lines += "0" + verticalBar;
            // Unit
            lines += verticalBar;
            // Package
            lines += "0" + verticalBar;
            // Gift Number 1 (Box)
            lines += "0" + verticalBar;
            // Gift Number 2 (Piece)
            lines += "0" + verticalBar;
            // Order No
            lines += refNbr;
            // Buyer Barcode
            // Buyer Prod No
            // Seller Prod No
            // Seller Account No
            // Seller Shipping No
            // Remark
            // Relate Number1
            // Relate Number2 (Invoice No)
            // Relate Number3 (Invoice Date)
            // Relate Number4
            // Relate Number5
            lines += new string(char.Parse(verticalBar), 11) + "\r\n";
        }
        #endregion

        #region Search Result
        public PXResultset<ARTran> RetrieveARTran(string orderNbr)
        {
            return SelectFrom<ARTran>.Where<ARTran.refNbr.IsEqual<@P.AsString>
                                            .And<Where<ARTran.lineType.IsNotEqual<SOLineType.discount>
                                                       .Or<ARTran.lineType.IsNull>>>>.View.ReadOnly.Select(this, orderNbr);
        }
        #endregion
    }
}