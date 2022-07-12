using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.TX;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace eGUICustomizations.Graph_Release
{
    public class TWNReleaseProcess : PXGraph<TWNReleaseProcess>, ITWNGUITran
    {
        #region Selects
        public SelectFrom<TWNGUITrans>.View ViewGUITrans;
        public SelectFrom<TWNGUIPrepayAdjust>.View PrepayAdjust;
        #endregion

        #region Property
        public int SequenceNo { get; set; }
        #endregion

        #region Methods
        public virtual void CreateGUITrans(STWNGUITran sGUITran)
        {
            TWNGUITrans row = ViewGUITrans.Cache.CreateInstance() as TWNGUITrans;

            row.GUIFormatcode = sGUITran.VATCode;
            row.GUINbr        = sGUITran.GUINbr;
            row.SequenceNo    = SequenceNo;

            row = ViewGUITrans.Insert(row);

            row.GUIStatus     = sGUITran.GUIStatus;
            row.Branch        = PXAccess.GetBranchCD(sGUITran.BranchID);
            row.GUIDirection  = sGUITran.GUIDirection;
            row.GUIDate       = sGUITran.GUIDate;
            row.GUIDecPeriod  = sGUITran.GUIDecPeriod ?? sGUITran.GUIDate;
            row.GUITitle      = sGUITran.GUITitle;
            row.TaxZoneID     = sGUITran.TaxZoneID;
            row.TaxCategoryID = sGUITran.TaxCategoryID;
            row.TaxID         = sGUITran.TaxID;
            row.VATType       = sGUITran.VATType ?? GetVATType(row.TaxID);
            row.TaxNbr        = sGUITran.TaxNbr;
            row.OurTaxNbr     = sGUITran.OurTaxNbr;
            row.NetAmount     = row.NetAmtRemain = sGUITran.NetAmount;
            row.TaxAmount     = row.TaxAmtRemain = sGUITran.TaxAmount;
            row.CustVend      = sGUITran.AcctCD;
            row.CustVendName  = sGUITran.AcctName;
            row.DeductionCode = sGUITran.DeductionCode;
            row.TransDate     = base.Accessinfo.BusinessDate;
            row.EGUIExcluded  = sGUITran.eGUIExcluded;
            row.Remark        = sGUITran.Remark;
            row.BatchNbr      = sGUITran.BatchNbr;
            row.DocType       = sGUITran.DocType;
            row.OrderNbr      = sGUITran.OrderNbr;
            row.CarrierType   = sGUITran.CarrierType;
            row.CarrierID     = sGUITran.CarrierID;
            row.NPONbr        = sGUITran.NPONbr;
            row.B2CPrinted    = sGUITran.B2CPrinted;
            //row.QREncrypter   = sGUITran.GUIDirection.Equals(TWNGUIDirection.Issue) && sGUITran.NetAmount > 0 && sGUITran.eGUIExcluded.Equals(false) ? GetQREncrypter(sGUITran) : null;

            ViewGUITrans.Update(row);

            this.Actions.PressSave();
        }

        public virtual void CreateGUIPrepayAdjust(bool create2Record = false, Tuple<string, int?, decimal?, decimal?, decimal?, decimal?> tuple = null)
        {
            TWNGUITrans trans = ViewGUITrans.Current;

            TWNGUIPrepayAdjust prepay = PrepayAdjust.Cache.CreateInstance() as TWNGUIPrepayAdjust;

            prepay.AppliedGUINbr = trans.GUINbr;
            prepay.PrepayGUINbr  = tuple?.Item1 ?? trans.GUINbr;
            prepay.SequenceNo    = tuple?.Item2 ?? trans.SequenceNo;
            prepay.Reason        = create2Record == false ? trans.GUIStatus : TWNStringList.TWNGUIStatus.Used;

            prepay = PrepayAdjust.Insert(prepay);

            decimal? netAmt = tuple?.Item3 ?? trans.NetAmount;
            decimal? taxAmt = tuple?.Item4 ?? trans.TaxAmount;

            prepay.NetAmt          = prepay.Reason != TWNStringList.TWNGUIStatus.Voided ? netAmt : -1 * netAmt;
            prepay.TaxAmt          = prepay.Reason != TWNStringList.TWNGUIStatus.Voided ? taxAmt : -1 * taxAmt;
            prepay.NetAmtUnapplied = tuple?.Item5 ?? netAmt;
            prepay.TaxAmtUnapplied = tuple?.Item6 ?? taxAmt;
            prepay.Remark          = trans.Remark;

            PrepayAdjust.Update(prepay);

            this.Actions.PressSave();
        }

        public virtual void GenerateVoidedGUI(bool isDeleted, ARRegister register)
        {
            int count = isDeleted == true ? 2 : 1;

            ARRegisterExt regisExt = register.GetExtension<ARRegisterExt>();

            if (string.IsNullOrEmpty(regisExt.UsrGUINbr)) { return; }

            using (PXTransactionScope ts = new PXTransactionScope())
            {
                new TWNGUIValidation().VerifyGUIPrepayAdjust(this.PrepayAdjust.Cache, register);

                TWNReleaseProcess rp = PXGraph.CreateInstance<TWNReleaseProcess>();

                var trans = rp.InitAndCheckOnAR(regisExt.UsrGUINbr, regisExt.UsrVATOutCode);

                Tuple<string, string, string, decimal?, decimal?> tuple = new ARReleaseProcess_Extension().RetrieveTaxDetails(this, register);

                if (isDeleted == true)
                {
                    Customer customer = Customer.PK.Find(this, register.CustomerID);

                    rp.CreateGUITrans(new STWNGUITran()
                    {
                        VATCode       = regisExt.UsrVATOutCode,
                        GUINbr        = regisExt.UsrGUINbr,
                        GUIStatus     = TWNStringList.TWNGUIStatus.Voided,
                        BranchID      = register.BranchID,
                        GUIDirection  = TWNStringList.TWNGUIDirection.Issue,
                        GUIDate       = regisExt.UsrGUIDate.Value.Date.Add(register.CreatedDateTime.Value.TimeOfDay),
                        GUITitle      = customer.AcctName,
                        TaxZoneID     = tuple.Item1,
                        TaxCategoryID = tuple.Item3,
                        TaxID         = tuple.Item2,
                        TaxNbr        = regisExt.UsrTaxNbr,
                        OurTaxNbr     = regisExt.UsrOurTaxNbr,
                        NetAmount     = tuple.Item4,
                        TaxAmount     = tuple.Item5,
                        AcctCD        = customer.AcctCD,
                        AcctName      = customer.AcctName,
                        Remark        = register.DocDesc,
                        OrderNbr      = register.RefNbr
                    });
                }
                else
                {
                    rp.ViewGUITrans.Current = SelectFrom<TWNGUITrans>.Where<TWNGUITrans.gUIFormatcode.IsEqual<@P.AsString>
                                                                            .And<TWNGUITrans.gUINbr.IsEqual<@P.AsString>
                                                                                 .And<TWNGUITrans.orderNbr.IsEqual<@P.AsString>>>>.View
                                                                            .SelectSingleBound(this, null, regisExt.UsrVATOutCode, regisExt.UsrGUINbr, register.RefNbr);

                    rp.ViewGUITrans.Current.GUIStatus = TWNStringList.TWNGUIStatus.Voided;
                    rp.ViewGUITrans.UpdateCurrent();
                }

                for (int i = 1; i <= count; i++)
                {
                    bool isVoided = i != 2; // First reocrd is voided and the second one is used.

                    rp.CreateGUIPrepayAdjust(isDeleted && !isVoided, Tuple.Create<string, int?, decimal?, decimal?, decimal?, decimal?>(null, null, null, null,
                                                                                                                                        isVoided == true ? 0m : tuple.Item4 ,
                                                                                                                                        isVoided == true ? 0m : tuple.Item5));
                }

                ts.Complete();
            }
        }

        public virtual TWNGUITrans InitAndCheckOnAP(string gUINbr, string vATInCode)
        {
            SequenceNo = (int)PXSelectGroupBy<TWNGUITrans, Where<TWNGUITrans.gUINbr, Equal<Required<TWNGUITrans.gUINbr>>,
                                                                 And<TWNGUITrans.gUIFormatcode, Equal<Required<TWNGUITrans.gUIFormatcode>>>>,
                                              Aggregate<Count>>.Select(this, gUINbr, vATInCode).RowCount;

            if (vATInCode.EndsWith("3") == true) { ++SequenceNo; }

            TWNGUIValidation gUIValidation = new TWNGUIValidation();

            gUIValidation.CheckCorrespondingInv(this, gUINbr, vATInCode);

            return gUIValidation.tWNGUITrans;
        }

        public virtual TWNGUITrans InitAndCheckOnAR(string gUINbr, string vATOutCode)
        {
            SequenceNo = (int)PXSelectGroupBy<TWNGUITrans, Where<TWNGUITrans.gUINbr, Equal<Required<TWNGUITrans.gUINbr>>,
                                                                 And<TWNGUITrans.gUIFormatcode, Equal<Required<TWNGUITrans.gUIFormatcode>>>>,
                                              Aggregate<Count>>.Select(this, gUINbr, vATOutCode).RowCount;
            
            if (vATOutCode.EndsWith("3") == true) { ++SequenceNo; }

            TWNGUIValidation gUIValidation = new TWNGUIValidation();

            gUIValidation.CheckCorrespondingInv(this, gUINbr, vATOutCode);

            return gUIValidation.tWNGUITrans;
        }

        private string GetVATType(string taxID) => SelectFrom<Tax>.Where<Tax.taxID.IsEqual<@P.AsString>>.View.ReadOnly.SelectSingleBound(this, null, taxID)?.TopFirst?.GetExtension<TaxExt>().UsrGUIType;
       
        //private string GetQREncrypter(STWNGUITran sGUITran)
        //{
        //    com.tradevan.qrutil.QREncrypter qrEncrypter = new com.tradevan.qrutil.QREncrypter();

        //    string result;

        //    try
        //    {
        //        string[][] abc = new string[1][];

        //        TWNGUIPreferences gUIPreferences = PXSelect<TWNGUIPreferences>.Select(this);

        //        if (string.IsNullOrEmpty(gUIPreferences.AESKey))
        //        {
        //            throw new MissingFieldException(string.Format("{0} {1}", nameof(TWNGUIPreferences.AESKey), PX.Data.InfoMessages.IsNull));
        //        }

        //        // a) Invoice number = GUITrans.GUINbr
        //        result = qrEncrypter.QRCodeINV(sGUITran.GUINbr,
        //                                       // b) Invoice date = GUITrans.GUIDate(If it is 2019 / 12 / 01, please change to 1081201.  107 = YYYY – 1911)
        //                                       TWNGenZeroTaxRateMedFile.GetTWNDate(sGUITran.GUIDate.Value),
        //                                       // c) Invoice time = “hhmmss” of GUITrans.GUIDate
        //                                       sGUITran.GUIDate.Value.ToString("hhmmss"),
        //                                       // d) Random number = If GUITrans.BatchNbr is not null then Right(Guitrans.bachNbrr,4) else Right(Guitrans.OrderNbrr, 4)
        //                                       string.IsNullOrEmpty(sGUITran.BatchNbr) ? sGUITran.BatchNbr.Substring(sGUITran.BatchNbr.Length - 4) : sGUITran.OrderNbr.Substring(sGUITran.OrderNbr.Length - 4),
        //                                       // e) Sales amount = GUITrans.Amount (No thousands separator, no decimal places)
        //                                       (int)sGUITran.NetAmount.Value,
        //                                       // f) Tax amount = GUITrans.Taxamount  (No thousands separator, no decimal places)
        //                                       (int)sGUITran.TaxAmount.Value,
        //                                       // g) Total amount = GUITrans.Amount + GUITrans.TaxAmount(No thousands separator, no decimal places)
        //                                       (int)(sGUITran.NetAmount + sGUITran.TaxAmount).Value,
        //                                       // h) Buyer identifier = GUITrans.TaxNbr(If it's blank or null, please use “00000000”)
        //                                       string.IsNullOrEmpty(sGUITran.TaxNbr) ? "00000000" : sGUITran.TaxNbr,
        //                                       // i) Representative identifier = “00000000”
        //                                       "00000000",
        //                                       // j) Sales identifier = GUITrans.OurTaxNbr
        //                                       sGUITran.OurTaxNbr,
        //                                       // k) Business identifier = GUITrans.OurTaxNbr
        //                                       sGUITran.OurTaxNbr,
        //                                       // l) AESKEY = GUIParameters.AESKEY
        //                                       gUIPreferences.AESKey);
        //    }
        //    catch
        //    {
        //        throw;
        //    }

        //    return result;
        //}
        #endregion
    }

    public interface ITWNGUITran
    {
        int SequenceNo { get; set; }
    }

    public struct STWNGUITran
    {
        public string VATCode;
        public string GUINbr;
        public string GUIDirection;
        public string GUIStatus;
        public string GUITitle;
        public string TaxZoneID;
        public string TaxCategoryID;
        public string TaxID;
        public string VATType;
        public string TaxNbr;
        public string AcctCD;
        public string AcctName;
        public string OurTaxNbr;
        public string DeductionCode;
        public string Remark;
        public string BatchNbr;
        public string DocType;
        public string OrderNbr;
        public string CarrierType;
        public string CarrierID;
        public string NPONbr;
        public string QREncrypter;
        
        public int? BranchID;

        public bool eGUIExcluded;
        public bool B2CPrinted;

        public DateTime? GUIDate;
        public DateTime? GUIDecPeriod;

        public decimal? NetAmount;
        public decimal? TaxAmount;

        //public STWNGUITran(string a, string b, string c, string d )
        //{
        //    VATCode = a;
        //    GUINbr = b;
        //    GUIDirection = c;
        //    GUIStatus = d;
        //}
    }
}
