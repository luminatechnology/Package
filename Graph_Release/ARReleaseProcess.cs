using System;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.TX;
using eGUICustomizations.DAC;
using eGUICustomizations.Graph_Release;
using eGUICustomizations.Descriptor;
using System.Collections.Generic;

namespace PX.Objects.AR
{
    public class ARReleaseProcess_Extension : PXGraphExtension<ARReleaseProcess>
    {
        public static bool IsActive() => TWNGUIValidation.ActivateTWGUI(new PXGraph());

        #region Delegate Methods
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            baseMethod();

            CreateGUITranAndPrepay();
        }
        #endregion

        #region Methods
        public void CreateGUITranAndPrepay()
        {
            ARRegister    doc    = Base.ARDocument.Current;
            ARRegisterExt docExt = doc.GetExtension<ARRegisterExt>();

            if (doc != null &&
                doc.Released == true &&
                doc.DocType.IsIn(ARDocType.Invoice, ARDocType.CreditMemo, ARDocType.CashSale, ARDocType.Prepayment) &&
                ((string.IsNullOrEmpty(docExt.UsrGUINbr) && docExt.UsrVATOutCode == TWGUIFormatCode.vATOutCode36) || !string.IsNullOrEmpty(docExt.UsrVATOutCode)) &&
                doc.OpenDoc == true)
            {
                if (docExt.UsrVATOutCode.IsIn(TWGUIFormatCode.vATOutCode33, TWGUIFormatCode.vATOutCode34) &&
                    docExt.UsrCreditAction == TWNStringList.TWNCreditAction.NO)
                {
                    throw new PXException(TWMessages.CRACIsNone);
                }
                else if (docExt.UsrVATOutCode == TWGUIFormatCode.vATOutCode37)
                {
                    throw new PXException(TWMessages.VATOutCodeIs37);
                }

                #region Tax Details
                string taxZoneID, taxID, taxCateID = null;
                decimal? remainNet, remainTax, settledNet = 0m, settledTax = 0m, taxRate = 0m;

                if (doc.DocType != ARDocType.Prepayment)
                { 
                    TaxTran xTran = APReleaseProcess_Extension.SelectTaxTran(Base, doc.DocType, doc.RefNbr, BatchModule.AR);

                    if (xTran == null) { throw new PXException(TWMessages.NoInvTaxDtls); }

                    if (Tax.PK.Find(Base, xTran.TaxID).GetExtension<TaxExt>().UsrTWNGUI != true) { return; }

                    taxID     = xTran.TaxID;
                    taxZoneID = Base.ARInvoice_DocType_RefNbr.Current.TaxZoneID;
                    remainNet = xTran.TaxableAmt + xTran.RetainedTaxableAmt;
                    remainTax = xTran.TaxAmt + xTran.RetainedTaxAmt;
                    taxRate   = xTran.TaxRate / xTran.NonDeductibleTaxRate;
                }
                else
                {
                    Tuple<string, string, string, decimal?, decimal?> tuple = RetrieveTaxDetails(Base, doc);

                    taxZoneID = tuple.Item1;
                    taxID     = tuple.Item2;
                    taxCateID = tuple.Item3;
                    remainNet = tuple.Item4;
                    remainTax = tuple.Item5;
                }
                #endregion

                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    TWNReleaseProcess rp = PXGraph.CreateInstance<TWNReleaseProcess>();

                    string[] gUINbrs = !string.IsNullOrEmpty(docExt.UsrGUINbr) ? docExt.UsrGUINbr.Split(';') : new string[1] { string.Empty };

                    for (int i = 0; i < gUINbrs.Length; i++)
                    {
                        string gUINbr = gUINbrs[i].TrimStart();

                        // Avoid standard logic calling this method twice and inserting duplicate records into TWNGUITrans.
                        if (APReleaseProcess_Extension.CountExistedRec(Base, gUINbr, docExt.UsrVATOutCode, doc.RefNbr) > 0) { return; }

                        TWNGUITrans tWNGUITrans = rp.InitAndCheckOnAR(gUINbr, docExt.UsrVATOutCode);

                        foreach (ARTran row in Base.ARTran_TranType_RefNbr.Cache.Cached)
                        {
                            taxCateID = row.TaxCategoryID;

                            if (!string.IsNullOrEmpty(taxCateID)) { goto CreatGUI; }
                        }
                        
                    CreatGUI:
                        if (docExt.UsrCreditAction.IsIn(TWNStringList.TWNCreditAction.CN, TWNStringList.TWNCreditAction.NO))
                        {
                            if (tWNGUITrans != null)
                            {
                                settledNet = (tWNGUITrans.NetAmtRemain < remainNet) ? tWNGUITrans.NetAmtRemain : remainNet;
                                settledTax = (tWNGUITrans.TaxAmtRemain < remainTax) ? tWNGUITrans.TaxAmtRemain : remainTax;

                                remainNet = settledNet - remainNet;
                                remainTax = settledTax - remainTax;
                            }
                            else
                            {
                                settledNet = remainNet;
                                settledTax = remainTax;
                            }

                            Customer customer = Customer.PK.Find(Base, doc.CustomerID);

                            Tuple<string, string, string> tuple = GetB2CTypeValue(string.IsNullOrEmpty(docExt.UsrTaxNbr), docExt.UsrCarrierID, docExt.UsrNPONbr);

                            rp.CreateGUITrans(new STWNGUITran()
                            {
                                VATCode       = docExt.UsrVATOutCode,
                                GUINbr        = gUINbr ?? string.Empty,
                                GUIStatus     = doc.CuryOrigDocAmt == 0m ? TWNStringList.TWNGUIStatus.Voided : TWNStringList.TWNGUIStatus.Used,
                                BranchID      = doc.BranchID,
                                GUIDirection  = TWNStringList.TWNGUIDirection.Issue,
                                GUIDate       = docExt.UsrGUIDate.Value.Date.Add(doc.CreatedDateTime.Value.TimeOfDay),
                                GUITitle      = docExt.UsrGUITitle ?? ARContact.PK.Find(Base, (doc as ARInvoice).BillContactID)?.FullName, // As David's email [GUI Form Adjustment] # 6
                                TaxZoneID     = taxZoneID,
                                TaxCategoryID = taxCateID,
                                TaxID         = taxID,
                                TaxNbr        = docExt.UsrTaxNbr,
                                OurTaxNbr     = docExt.UsrOurTaxNbr,
                                NetAmount     = settledNet < 0 ? 0 : settledNet,
                                TaxAmount     = settledTax < 0 ? 0 : settledTax,
                                AcctCD        = customer.AcctCD,
                                AcctName      = customer.AcctName,
                                Remark        = doc.DocDesc,//(appointment is null) ? string.Empty : appointment.RefNbr,
                                BatchNbr      = doc.BatchNbr,
                                DocType       = doc.DocType,
                                OrderNbr      = doc.RefNbr,
                                CarrierType   = tuple.Item1,
                                CarrierID     = docExt.UsrB2CType == TWNStringList.TWNB2CType.MC ? tuple.Item2 : null,
                                NPONbr        = docExt.UsrB2CType == TWNStringList.TWNB2CType.NPO ? tuple.Item3 : null,
                                B2CPrinted    = docExt.UsrB2CType == TWNStringList.TWNB2CType.DEF && string.IsNullOrEmpty(docExt.UsrTaxNbr),
                            });

                            #region Prepayment Adjust
                            decimal? adjustNetTol = null, adjustTaxTol = null;

                            foreach (ARAdjust adjust in Base.ARAdjust_AdjdDocType_RefNbr_CustomerID.Cache.Cached)
                            {
                                string prepayGUINbr = ARRegister.PK.Find(Base, adjust.AdjgDocType, adjust.AdjgRefNbr).GetExtension<ARRegisterExt>().UsrGUINbr;

                                decimal adjustNet = PXDBCurrencyAttribute.BaseRound(Base, (adjust.CuryAdjdAmt / (1 + taxRate)).Value);
                                decimal adjustTax = PXDBCurrencyAttribute.BaseRound(Base, (adjust.CuryAdjdAmt - adjustNet).Value);

                                foreach (var keyValues in GetGUIPrepayAdjustAmounts(prepayGUINbr, adjustNet, adjustTax))
                                {
                                    rp.CreateGUIPrepayAdjust(false, Tuple.Create<string, int?, decimal?, decimal?, decimal?, decimal?>(prepayGUINbr, 0, -adjustNet, -adjustTax, keyValues.Value.Item1, keyValues.Value.Item2));

                                    adjustNetTol = (adjustNetTol ?? 0m) + adjustNet;
                                    adjustTaxTol = (adjustTaxTol ?? 0m) + adjustTax;
                                }
                            }

                            if (adjustNetTol != null)
                            {
                                var gUITran = rp.ViewGUITrans.Current;

                                rp.ViewGUITrans.Cache.SetValue<TWNGUITrans.netAmount>(gUITran, gUITran.NetAmount - adjustNetTol);
                                rp.ViewGUITrans.Cache.SetValue<TWNGUITrans.taxAmount>(gUITran, gUITran.TaxAmount - adjustTaxTol);
                                rp.ViewGUITrans.Cache.SetValue<TWNGUITrans.netAmtRemain>(gUITran, gUITran.NetAmtRemain - adjustNetTol);
                                rp.ViewGUITrans.Cache.SetValue<TWNGUITrans.taxAmtRemain>(gUITran, gUITran.TaxAmtRemain -  adjustTaxTol);

                                rp.ViewGUITrans.UpdateCurrent();
                            }

                            if (doc.DocType == ARDocType.Prepayment)
                            {
                                rp.CreateGUIPrepayAdjust();
                            }
                            #endregion
                        }

                        #region GUI Tran Credit Action
                        if (tWNGUITrans != null)
                        {
                            if (docExt.UsrCreditAction == TWNStringList.TWNCreditAction.VG)
                            {
                                rp.ViewGUITrans.SetValueExt<TWNGUITrans.gUIStatus>(tWNGUITrans, TWNStringList.TWNGUIStatus.Voided);
                                rp.ViewGUITrans.SetValueExt<TWNGUITrans.eGUIExported>(tWNGUITrans, false);
                            }
                            else
                            {
                                if (remainNet < 0 && (i + 1) == gUINbrs.Length) { throw new PXException(TWMessages.RemainAmt); }

                                rp.ViewGUITrans.SetValueExt<TWNGUITrans.netAmtRemain>(tWNGUITrans, tWNGUITrans.NetAmtRemain -= settledNet);
                                rp.ViewGUITrans.SetValueExt<TWNGUITrans.taxAmtRemain>(tWNGUITrans, tWNGUITrans.TaxAmtRemain -= settledTax);
                            }

                            rp.ViewGUITrans.Update(tWNGUITrans);
                        }
                        #endregion

                        #region GUI Prepay Credit Action
                        if (doc.DocType == ARDocType.CreditMemo && docExt.UsrCreditAction.IsIn(TWNStringList.TWNCreditAction.CN, TWNStringList.TWNCreditAction.VG) && doc.ReleasedToVerify != null)
                        {
                            bool isVoided = docExt.UsrCreditAction == TWNStringList.TWNCreditAction.VG;

                            decimal taxable = (doc as ARInvoice).CuryVatTaxableTotal.Value;
                            decimal taxTotl = (doc as ARInvoice).CuryTaxTotal.Value;

                            foreach (var keyValues in GetGUIPrepayAdjustAmounts(gUINbr, taxable, taxTotl, isVoided) )
                            {
                                if (keyValues.Value.Item3 == null && isVoided == true) { continue; }

                                rp.CreateGUIPrepayAdjust(false, Tuple.Create<string, int?, decimal?, decimal?, decimal?, decimal?>(keyValues.Key,//gUINbr,
                                                                                                                                   isVoided == false ? (keyValues.Value.Item3 ?? 0) + 1 : keyValues.Value.Item3,
                                                                                                                                   isVoided == false ? -taxable : keyValues.Value.Item1,
                                                                                                                                   isVoided == false ? -taxTotl : keyValues.Value.Item2,
                                                                                                                                   Math.Abs(keyValues.Value.Item1),
                                                                                                                                   Math.Abs(keyValues.Value.Item2)) );
                            }
                        }
                        #endregion
                    }

                    // Manually Saving as base code will not call base graph persist.
                    rp.ViewGUITrans.Cache.Persist(PXDBOperation.Insert);
                    rp.ViewGUITrans.Cache.Persist(PXDBOperation.Update);

                    ts.Complete(Base);

                    #region Print eGUI invoice
                    //if (doc.DocType == ARDocType.Invoice && gUINbr != null && rp.ViewGUITrans.Current.GUIStatus == TWNGUIStatus.Used && docExt.UsrB2CType ==TWNB2CType.DEF)
                    //{
                    //    Base.ARTran_TranType_RefNbr.WhereAnd<Where<ARTran.curyExtPrice, Greater<decimal0>>>();
                    //    PXGraph.CreateInstance<eGUIInquiry>().PrintReport(Base.ARTran_TranType_RefNbr.Select(doc.DocType, doc.RefNbr), rp.ViewGUITrans.Current, false);
                    //}
                    #endregion

                    // Triggering after save events.
                    rp.ViewGUITrans.Cache.Persisted(false);
                }
            }

            ///<remarks> The following is for void prepayment only. </remarks>
            if ((doc as ARPayment)?.VoidAppl == true &&
                doc.DocType == ARDocType.VoidPayment &&
                doc.OrigDocType == ARDocType.Prepayment &&
                doc.Released == true)
             {
                PXGraph.CreateInstance<TWNReleaseProcess>().GenerateVoidedGUI(false, doc);
            }
        }

        public Tuple<string, string, string> GetB2CTypeValue(bool hasTaxNbr, string carrierID, string nPONbr)
        {
            // 手機條碼：3J0002, 自然人憑證：CQ0001
            const string Mobile = "3J0002", Citizen = "CQ0001";

            return Tuple.Create(string.IsNullOrEmpty(carrierID) ? null : carrierID.Substring(0, 1) == "/" ? Mobile : Citizen, hasTaxNbr == false ? carrierID : null, hasTaxNbr == false ? nPONbr : null);
        }

        public Tuple<string, string, string, decimal?, decimal?> RetrieveTaxDetails(PXGraph graph, ARRegister register)
        {
            string  taxZoneID = null, taxCateID = null;
            decimal? remainNet, remainTax;

            ARRegisterExt regisExt = register.GetExtension<ARRegisterExt>();

            string taxID = SelectFrom<Tax>.Where<TaxExt.usrGUIType.IsEqual<@P.AsString>>.View.SelectSingleBound(graph, null, regisExt.UsrVATType).TopFirst?.TaxID;

            foreach (PXResult<TaxZone, TaxZoneDet, CR.Location> result in SelectFrom<TaxZone>.InnerJoin<TaxZoneDet>.On<TaxZoneDet.FK.TaxZone>
                                                                                             .InnerJoin<CR.Location>.On<CR.Location.cTaxZoneID.IsEqual<TaxZoneDet.taxZoneID>>
                                                                                             .Where<CR.Location.bAccountID.IsEqual<@P.AsInt>>.View.SelectSingleBound(graph, null, register.CustomerID))
            {
                TaxZone taxZone = result;
                TaxZoneDet zoneDet = result;

                if (taxZone == null) { throw new PXException(TWMessages.CustNoTezZone); }

                taxZoneID = taxZone.TaxZoneID;
                taxCateID = taxZone.DfltTaxCategoryID;
                taxID     = taxID ?? zoneDet.TaxID;
            }

            TaxRev taxRev = SelectFrom<TaxRev>.Where<TaxRev.taxID.IsEqual<@P.AsString>
                                                     .And<TaxRev.taxType.IsEqual<@P.AsString>>
                                                          .And<TaxRev.startDate.IsLessEqual<@P.AsDateTime>>>.View.Select(graph, taxID, "S", regisExt.UsrGUIDate); // "S" -> Output

            if (taxRev == null) { throw new PXException(TX.Messages.TaxRateNotSpecified, taxID); }

            remainNet = register.CuryOrigDocAmt / (1 + (taxRev.TaxRate / taxRev.NonDeductibleTaxRate));
            remainNet = Math.Round(remainNet.Value, 0);
            remainTax = register.CuryOrigDocAmt - remainNet;

            return Tuple.Create(taxZoneID, taxID, taxCateID, remainNet, remainTax);
        }

        public Dictionary<string, ValueTuple<decimal, decimal, int?>> GetGUIPrepayAdjustAmounts(string gUINbr, decimal adjustNet, decimal adjustTax, bool isVoided = false)
        {
            Dictionary<string, ValueTuple<decimal, decimal, int?>> dic = new Dictionary<string, (decimal, decimal, int?)>();

            if (isVoided == false)
            {
                TWNGUIPrepayAdjust prepay = SelectFrom<TWNGUIPrepayAdjust>.Where<TWNGUIPrepayAdjust.prepayGUINbr.IsEqual<@P.AsString>>
                                                       .OrderBy<TWNGUIPrepayAdjust.createdDateTime.Desc>.View.SelectSingleBound(Base, null, gUINbr);

                if (prepay != null)
                {
                    decimal remainNet = prepay.NetAmtUnapplied ?? 0m;
                    decimal remainTax = prepay.TaxAmtUnapplied ?? 0m;

                    if (remainNet + remainTax < adjustNet + adjustTax) { throw new PXException(TWMessages.InsufPrepayCN); }

                    decimal settleNet = (remainNet + remainTax > adjustNet + adjustTax) ? remainNet - adjustNet : decimal.Zero;
                    decimal settleTax = (remainNet + remainTax > adjustNet + adjustTax) ? remainTax - adjustTax : decimal.Zero;

                    dic.Add(prepay.PrepayGUINbr, ValueTuple.Create(settleNet, settleTax, prepay.SequenceNo));
                }
            }
            else
            {
                foreach (TWNGUIPrepayAdjust applied in SelectFrom<TWNGUIPrepayAdjust>.Where<TWNGUIPrepayAdjust.appliedGUINbr.IsEqual<@P.AsString>
                                                                                            .And<TWNGUIPrepayAdjust.reason.IsEqual<TWNStringList.TWNGUIStatus.used>>
                                                                                                .And<TWNGUIPrepayAdjust.sequenceNo.IsEqual<CS.int0>>>.View.Select(Base, gUINbr))
                {
                    decimal settleNet = (applied.NetAmtUnapplied ?? 0m) + (applied.NetAmt ?? 0m);
                    decimal settleTax = (applied.TaxAmtUnapplied ?? 0m) + (applied.TaxAmt ?? 0m);

                    dic.Add(applied.PrepayGUINbr, ValueTuple.Create(settleNet, settleTax, applied.SequenceNo));
                }
            }

            return dic;
        }
        #endregion
    }
}