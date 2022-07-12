using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.TX;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;
using eGUICustomizations.Graph_Release;
using PX.Objects.CR;
using PX.Objects.CM;

namespace PX.Objects.AP
{
    public class APReleaseProcess_Extension : PXGraphExtension<APReleaseProcess>
    {
        #region Selects
        public SelectFrom<TWNWHTTran>.Where<TWNWHTTran.docType.IsEqual<APRegister.docType.FromCurrent>
                                            .And<TWNWHTTran.refNbr.IsEqual<APRegister.refNbr.FromCurrent>>>.View WHTTranView;
        #endregion

        #region Delegate Methods
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            baseMethod();

            APRegister doc = Base.APDocument.Current;

            // Check for document and released flag
            if (TWNGUIValidation.ActivateTWGUI(Base) == true &&
                doc != null && 
                doc.Released == true &&
                doc.DocType.IsIn(APDocType.Invoice, APDocType.DebitAdj) )
            {
                //if (Base.APTaxTran_TranType_RefNbr.Current == null)
                //{
                //    throw new PXException(TWMessages.NoInvTaxDtls);
                //}
                TWNReleaseProcess rp = PXGraph.CreateInstance<TWNReleaseProcess>();

                foreach (TWNManualGUIAPBill row in SelectFrom<TWNManualGUIAPBill>.Where<TWNManualGUIAPBill.docType.IsEqual<@P.AsString>
                                                                                        .And<TWNManualGUIAPBill.refNbr.IsEqual<@P.AsString>>>.View.Select(Base, doc.DocType, doc.RefNbr))
                {
                    // Avoid standard logic calling this method twice and inserting duplicate records into TWNGUITrans.
                    if (CountExistedRec(Base, row.GUINbr, row.VATInCode, doc.RefNbr) >= 1) { return; }

                    if (Tax.PK.Find(Base, row.TaxID).GetExtension<TaxExt>().UsrTWNGUI != true) { continue; }

                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        TWNGUITrans tWNGUITrans = rp.InitAndCheckOnAP(row.GUINbr, row.VATInCode);

                        Vendor vendor = Vendor.PK.Find(Base, row.VendorID);

                        rp.CreateGUITrans(new STWNGUITran()
                        {    
                            VATCode       = row.VATInCode,
                            GUINbr        = row.GUINbr,
                            GUIStatus     = TWNStringList.TWNGUIStatus.Used,
                            BranchID      = doc.BranchID,
                            GUIDirection  = TWNStringList.TWNGUIDirection.Receipt,
                            GUIDate       = row.GUIDate,
                            GUITitle      = vendor?.AcctName,
                            TaxZoneID     = row.TaxZoneID,
                            TaxCategoryID = row.TaxCategoryID,
                            TaxID         = row.TaxID,
                            TaxNbr        = row.TaxNbr,
                            OurTaxNbr     = row.OurTaxNbr,
                            NetAmount     = row.NetAmt,
                            TaxAmount     = row.TaxAmt,
                            AcctCD        = vendor?.AcctCD,
                            AcctName      = vendor?.AcctName,
                            DeductionCode = row.Deduction,
                            Remark        = row.Remark,
                            BatchNbr      = doc.BatchNbr,
                            DocType       = doc.DocType,
                            OrderNbr      = doc.RefNbr
                        });

                        ///<remarks>This is only for Cottingham's request.</remarks>
                        rp.ViewGUITrans.Current.GUIDecPeriod = doc.DocDate;
                        rp.ViewGUITrans.UpdateCurrent();

                        if (tWNGUITrans != null)
                        {
                            if (tWNGUITrans.NetAmtRemain < row.NetAmt)
                            {
                                throw new PXException(TWMessages.RemainAmt);
                            }

                            rp.ViewGUITrans.SetValueExt<TWNGUITrans.netAmtRemain>(tWNGUITrans, (tWNGUITrans.NetAmtRemain -= row.NetAmt));
                            rp.ViewGUITrans.SetValueExt<TWNGUITrans.taxAmtRemain>(tWNGUITrans, (tWNGUITrans.TaxAmtRemain -= row.TaxAmt));

                            tWNGUITrans = rp.ViewGUITrans.Update(tWNGUITrans);
                        }

                        // Manually Saving as base code will not call base graph persis.
                        rp.ViewGUITrans.Cache.Persist(PXDBOperation.Insert);
                        rp.ViewGUITrans.Cache.Persist(PXDBOperation.Update);

                        ts.Complete(Base);
                    }
                }

                // Triggering after save events.
                rp.ViewGUITrans.Cache.Persisted(false);
            }

            foreach (TWNWHT tWNWHT in SelectFrom<TWNWHT>.Where<TWNWHT.docType.IsEqual<@P.AsString>.And<TWNWHT.refNbr.IsEqual<@P.AsString>>>.View.Select(Base, doc.DocType, doc.RefNbr))
            {
                if (doc != null && doc.Released == true && doc.DocType == APDocType.Invoice)
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        TWNWHTTran wHTTran = new TWNWHTTran()
                        {
                            DocType = doc.DocType,
                            RefNbr = doc.RefNbr,
                        };

                        wHTTran = WHTTranView.Insert(wHTTran);

                        wHTTran.BatchNbr   = doc.BatchNbr;
                        wHTTran.TranDate   = doc.DocDate;
                        wHTTran.PaymDate   = Base.APInvoice_DocType_RefNbr.Current?.DueDate;
                        wHTTran.PersonalID = tWNWHT.PersonalID;
                        wHTTran.PropertyID = tWNWHT.PropertyID;
                        wHTTran.TypeOfIn   = tWNWHT.TypeOfIn;
                        wHTTran.WHTFmtCode = tWNWHT.WHTFmtCode;
                        wHTTran.WHTFmtSub  = tWNWHT.WHTFmtSub;
                        wHTTran.PayeeName  = (string)PXSelectorAttribute.GetField(Base.APDocument.Cache, doc, nameof(doc.VendorID), doc.VendorID, nameof(Vendor.acctName));
                        wHTTran.PayeeAddr  = SelectFrom<Address>.InnerJoin<Vendor>.On<Vendor.bAccountID.IsEqual<Address.bAccountID>
                                                                                     .And<Vendor.defAddressID.IsEqual<Address.addressID>>>
                                                               .Where<Vendor.bAccountID.IsEqual<@P.AsInt>>.View.ReadOnly.SelectSingleBound(Base, null, doc.VendorID).TopFirst?.AddressLine1;
                        wHTTran.WHTTaxPct  = string.IsNullOrEmpty(tWNWHT.WHTTaxPct) ? 0m : System.Convert.ToDecimal(tWNWHT.WHTTaxPct);
                        wHTTran.WHTAmt     = PXDBCurrencyAttribute.BaseRound(Base, (doc.CuryDocBal * wHTTran.WHTTaxPct).Value);
                        wHTTran.NetAmt     = doc.CuryOrigDocAmt;
                        wHTTran.SecNHIPct  = tWNWHT.SecNHIPct;
                        wHTTran.SecNHICode = tWNWHT.SecNHICode;
                        wHTTran.SecNHIAmt  = PXDBCurrencyAttribute.BaseRound(Base, (doc.CuryDocBal * wHTTran.SecNHIPct).Value);

                        WHTTranView.Update(wHTTran);

                        // Manually Saving as base code will not call base graph persis.
                        WHTTranView.Cache.Persist(PXDBOperation.Insert);
                        WHTTranView.Cache.Persist(PXDBOperation.Update);

                        ts.Complete(Base);
                    }

                    // Triggering after save events.
                    WHTTranView.Cache.Persisted(false);
                }
            }

            foreach (APAdjust adjust in Base.APAdjust_AdjgDocType_RefNbr_VendorID.Cache.Cached)
            {
                if (doc != null && doc.Released == true && doc.DocType == APDocType.Check && Base.APPayment_DocType_RefNbr.Current != null)
                {
                    PXUpdate<Set<TWNWHTTran.paymDate, Required<APAdjust.adjgDocDate>,
                                 Set<TWNWHTTran.paymRef, Required<APAdjust.adjgRefNbr>>>,
                             TWNWHTTran,
                             Where<TWNWHTTran.docType, Equal<Required<APAdjust.adjdDocType>>,
                                   And<TWNWHTTran.refNbr, Equal<Required<APAdjust.adjdRefNbr>>>>>.Update(Base, Base.APPayment_DocType_RefNbr.Current.AdjDate, Base.APPayment_DocType_RefNbr.Current.ExtRefNbr,
                                                                                                               adjust.AdjdDocType, adjust.AdjdRefNbr);
                }
            }
        }
        #endregion

        #region Static Methods
        public static int CountExistedRec(PXGraph graph, string gUINbr, string gUIFmtCode, string refNbr)
        {
            return SelectFrom<TWNGUITrans>.Where<TWNGUITrans.gUINbr.IsEqual<@P.AsString>
                                                 .And<TWNGUITrans.gUIFormatcode.IsEqual<@P.AsString>>
                                                      .And<TWNGUITrans.orderNbr.IsEqual<@P.AsString>>>
                                          .View.ReadOnly.Select(graph, gUINbr, gUIFmtCode, refNbr).Count;
        }

        public static TaxTran SelectTaxTran(PXGraph graph, string tranType, string refNbr, string module)
        {
            return SelectFrom<TaxTran>.Where<TaxTran.tranType.IsEqual<@P.AsString>
                                             .And<TaxTran.refNbr.IsEqual<@P.AsString>>
                                                  .And<TaxTran.module.IsEqual<@P.AsString>>>
                                      .View.ReadOnly.Select(graph, tranType, refNbr, module);
        }
        #endregion
    }
}