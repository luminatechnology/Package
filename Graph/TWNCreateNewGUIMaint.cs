using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;
using static eGUICustomizations.Descriptor.TWNStringList;

namespace eGUICustomizations.Graph
{
    public class TWNCreateNewGUIMaint : PXGraph<TWNCreateNewGUIMaint, TWNGUIInvCredit>
    {
        #region Selects & Setup
        public PXSelect<TWNGUIInvCredit> Document;
        public SelectFrom<TWNGUITrans>.InnerJoin<Customer>.On<Customer.acctCD.IsEqual<TWNGUITrans.custVend>>
                                      //.InnerJoin<ARInvoice>.On<ARInvoice.refNbr.IsEqual<TWNGUITrans.orderNbr>
                                      //                         .And<ARInvoice.origRefNbr.IsNotNull>>
                                      .Where<TWNGUITrans.gUIStatus.IsEqual<TWNGUIStatus.used>
                                             .And<TWNGUITrans.gUIDirection.IsEqual<TWNGUIDirection.issue>
                                                  .And<TWNGUITrans.gUIFormatcode.IsEqual<TWNExpGUIInv2BankPro.VATOutCode35>
                                                       .And<Customer.bAccountID.IsEqual<TWNGUIInvCredit.customerID.FromCurrent>>>>>.View GUIInvoice;
        public SelectFrom<TWNGUITrans>.InnerJoin<Customer>.On<Customer.acctCD.IsEqual<TWNGUITrans.custVend>>
                                      //.InnerJoin<ARInvoice>.On<ARInvoice.refNbr.IsEqual<TWNGUITrans.orderNbr>
                                      //                         .And<ARInvoice.origRefNbr.IsNotNull>>
                                      .Where<TWNGUITrans.gUIStatus.IsEqual<TWNGUIStatus.used>
                                             .And<TWNGUITrans.gUIDirection.IsEqual<TWNGUIDirection.issue>
                                                  .And<TWNGUITrans.gUIFormatcode.IsEqual<ARRegisterExt.VATOut33Att>
                                                       .And<Customer.bAccountID.IsEqual<TWNGUIInvCredit.customerID.FromCurrent>>>>>.View GUICreditNote;
        public SelectFrom<TWNInvTranHist>.View InvTranHist;

        public PXSetup<TWNGUIPreferences> Preferences;
        #endregion

        #region Ctor
        public TWNCreateNewGUIMaint()
        {
            actions.AddMenuAction(createNewGUI);
            report.AddMenuAction(printGUIInvoice);

            Save.SetVisible(false);
        }
        #endregion

        #region Delegate Data Views
        public IEnumerable gUIInvoice()
        {
            PXView view = new PXView(this, false, GUIInvoice.View.BqlSelect);

            if (Document.Current != null && Document.Current.RequestID != PX.Objects.CS.AutoNumberAttribute.GetNewNumberSymbol<TWNGUIInvCredit.requestID>(Document.Cache, Document.Current))
            {
                view.WhereAnd<Where<TWNGUITrans.orderNbr, Equal<Current<TWNGUIInvCredit.requestID>>>>();
            }

            int totalrow = 0;
            int startrow = PXView.StartRow;

            List<object> result = view.Select(PXView.Currents, PXView.Parameters, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startrow, PXView.MaximumRows, ref totalrow);

            PXView.StartRow = 0;

            return result;
        }

        public IEnumerable gUICreditNote()
        {
            PXView view = new PXView(this, false, GUICreditNote.View.BqlSelect);

            if (Document.Current != null && Document.Current.RequestID != PX.Objects.CS.AutoNumberAttribute.GetNewNumberSymbol<TWNGUIInvCredit.requestID>(Document.Cache, Document.Current))
            {
                view.WhereAnd<Where<TWNGUITrans.orderNbr, Equal<Current<TWNGUIInvCredit.requestID>>>>();
            }

            int totalrow = 0;
            int startrow = PXView.StartRow;

            List<object> result = view.Select(PXView.Currents, PXView.Parameters, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startrow, PXView.MaximumRows, ref totalrow);

            PXView.StartRow = 0;

            return result;
        }
        #endregion

        #region Actions
        public PXAction<TWNGUIInvCredit> actions;
        [PXButton(SpecialType = PXSpecialButtonType.ActionsFolder, MenuAutoOpen = true)]
        [PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select)]
        protected new virtual IEnumerable Actions(PXAdapter adapter) => adapter.Get();

        public PXAction<TWNGUITrans> createNewGUI;
        [PXProcessButton()]
        [PXUIField(DisplayName = "Create New GUI Nbr.")]
        protected virtual IEnumerable CreateNewGUI(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, delegate ()
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    CreateGUIInvCredit();

                    CreateInvTranHist();

                    this.Save.Press();

                    ts.Complete();
                }
            });

            return adapter.Get();
        }

        public PXAction<TWNGUIInvCredit> report;
        [PXButton(SpecialType = PXSpecialButtonType.ReportsFolder, MenuAutoOpen = true)]
        [PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
        protected new virtual IEnumerable Report(PXAdapter adapter, [PXString(8, InputMask = "CC.CC.CC.CC")] string reportID) => adapter.Get();

        public PXAction<TWNGUIInvCredit> printGUIInvoice;
        [PXLookupButton]
        [PXUIField(DisplayName = "Print GUI Invoice", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual void PrintGUIInvoice()
        {
            if (Document.Current != null)
            {
                throw new PXReportRequiredException(new Dictionary<string, string>()
                {
                    ["RequestID"] = Document.Current.RequestID
                }, "TW601001");
            }
        }
        #endregion

        #region Event Handlers
        protected void _(Events.RowSelected<TWNGUIInvCredit> e)
        {
            bool EmptyNewGUI = string.IsNullOrEmpty(e.Row.NewGUINbr);

            GUICreditNote.AllowSelect = EmptyNewGUI;

            createNewGUI.SetEnabled(EmptyNewGUI);
            printGUIInvoice.SetEnabled(!EmptyNewGUI);
        }

        protected void _(Events.RowSelected<TWNGUITrans> e)
        {
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.gUINbr>       (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.gUIFormatcode>(e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.gUIDirection> (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.gUIDate>      (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.custVend>     (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.custVendName> (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.taxNbr>       (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.ourTaxNbr>    (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.taxID>        (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.taxZoneID>    (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.taxCategoryID>(e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.orderNbr>     (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.transDate>    (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.netAmount>    (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.taxAmount>    (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.gUITitle>     (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.batchNbr>     (e.Cache, e.Row, false);
        }
        #endregion

        #region Methods
        public virtual void CreateGUIInvCredit()
        {
            TWNGUITrans gUITran = new TWNGUITrans();
            TWNGUIInvCredit invCredit = Document.Current;

            List<TWNGUITrans> lists = GUIInvoice.Cache.Updated.Cast<TWNGUITrans>().ToList();

            lists.Sort((x, y) =>
            {
                if (Convert.ToInt32(x.GUIFormatcode) < Convert.ToInt32(y.GUIFormatcode))
                {
                    return 0;
                }
                else
                {
                    return ((IComparable)x.GUIFormatcode).CompareTo(y.GUIFormatcode);
                }
            });

            int index = lists.FindIndex(tran => tran.Selected == false);
            
            if (index >= 0)
            {
                lists.RemoveAt(index);
            }

            for (int i = 0; i < lists.Count; i++)
            {
                TWNGUITrans row = lists[i];

                if (row.GUIFormatcode == TWGUIFormatCode.vATOutCode35)
                {
                    invCredit.GUINbr     = row.GUINbr;
                    invCredit.VATOutCode = row.GUIFormatcode;
                    invCredit.GUINetAmt  = row.NetAmount;
                    invCredit.GUITaxAmt  = row.TaxAmount;
                    invCredit.CNNetAmt   = invCredit.CNTaxAmt = 0;
                    invCredit.NewGUINbr  = ARGUINbrAutoNumAttribute.GetNextNumber(Document.Cache, invCredit, Preferences.Current.GUI3CopiesNumbering, Document.Current.NewGUIDate);

                    gUITran = row;
                }
                else
                {
                    invCredit.CNGUINbr    += (row.GUINbr + ";");
                    invCredit.CNVATOutCode = row.GUIFormatcode;
                    invCredit.CNNetAmt    += row.NetAmount;
                    invCredit.CNTaxAmt    += row.TaxAmount;
                    invCredit.CalcNetAmt   = invCredit.GUINetAmt - invCredit.CNNetAmt;
                    invCredit.CalcTaxAmt   = invCredit.GUITaxAmt - invCredit.CNTaxAmt;
                }

                row.GUIStatus    = TWNGUIStatus.Voided;
                row.EGUIExported = false;

                GUIInvoice.Cache.Update(row);
            }

            invCredit.CNGUINbr = invCredit.CNGUINbr.Substring(0, invCredit.CNGUINbr.Length - 1);

            this.Save.Press();

            CreateNewGUITran(gUITran);
        }

        public virtual void CreateNewGUITran(TWNGUITrans refTran)
        {
            TWNGUITrans newTran = PXCache<TWNGUITrans>.CreateCopy(refTran);

            newTran.GUIStatus            = TWNGUIStatus.Used;
            newTran.GUINbr               = Document.Current.NewGUINbr;
            newTran.GUIDate              = Document.Current.NewGUIDate;
            newTran.OrderNbr             = Document.Current.RequestID;
            newTran.NetAmount            = newTran.NetAmtRemain = Document.Current.CalcNetAmt;
            newTran.TaxAmount            = newTran.TaxAmtRemain = Document.Current.CalcTaxAmt;
            newTran.IdentityID           = null;
            newTran.BatchNbr             = null;
            newTran.EGUIExported         = null;
            newTran.EGUIExportedDateTime = null;
            newTran.NoteID               = null;

            GUIInvoice.Insert(newTran);
        }

        /// <summary>
        /// The new GUI number comes from a GUI invoice and multiple GUI credit notes, merge the same inventory, and then summarize the quantity.
        /// Insert the merged result into the new history table.
        /// </summary>
        public virtual void CreateInvTranHist()
        {
            Dictionary<string, decimal?> dic1 = new Dictionary<string, decimal?>();

            foreach (ARTran row in RetriveARTran(this, Document.Current.VATOutCode, Document.Current.GUINbr))
            {
                dic1.Add(string.Format("{0}-{1}", row.InventoryID, row.LineNbr), row.Qty);
            }

            Dictionary<string, decimal?> dic2 = new Dictionary<string, decimal?>();

            string[] cNGUINbrs = !string.IsNullOrEmpty(Document.Current.CNGUINbr) ? Document.Current.CNGUINbr.Split(';') : new string[1] { string.Empty };

            for (int i = 0; i < cNGUINbrs.Length; i++)
            {
                string cNGUINbr = cNGUINbrs[i].TrimStart();

                foreach (ARTran row in SelectFrom<ARTran>.InnerJoin<TWNGUITrans>.On<ARTran.refNbr.IsEqual<TWNGUITrans.orderNbr>>
                                                         .Where<TWNGUITrans.gUIFormatcode.IsEqual<@P.AsString>
                                                                .And<TWNGUITrans.gUINbr.IsEqual<@P.AsString>
                                                                     .And<TWNGUITrans.sequenceNo.IsEqual<@P.AsInt>>>>
                                                         .OrderBy<Asc<ARTran.inventoryID>>.View.Select(this, Document.Current.CNVATOutCode, cNGUINbr, i))
                {
                    dic2.Add(string.Format("{0}-{1}", row.InventoryID, string.IsNullOrEmpty(row.OrigInvoiceNbr) ? row.SOOrderLineNbr : row.OrigInvoiceLineNbr), -row.Qty);
                }
            }

            var resDict = dic1.Concat(dic2).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.Value));

            foreach (ARTran row in RetriveARTran(this, Document.Current.VATOutCode, Document.Current.GUINbr))
            {
                int? lineNbr = string.IsNullOrEmpty(row.OrigInvoiceNbr) ? row.SOOrderLineNbr : row.OrigInvoiceLineNbr;

                TWNInvTranHist tranHist = new TWNInvTranHist();

                tranHist.RequestID      = Document.Current.RequestID;
                tranHist.InvoiceType    = row.TranType;
                tranHist.InvoiceNbr     = row.RefNbr;
                tranHist.InvTranLineNbr = row.LineNbr;
                tranHist.InventoryID    = row.InventoryID;
                if (resDict.ContainsKey(string.Format("{0}-{1}", tranHist.InventoryID, row.LineNbr)) )
                {
                    tranHist.Qty = resDict[string.Format("{0}-{1}", tranHist.InventoryID, row.LineNbr)];
                }
                tranHist.UnitPrice      = row.CuryUnitPrice;
                tranHist.TranAmt        = tranHist.Qty * tranHist.UnitPrice;

                InvTranHist.Insert(tranHist);
            }
        }
        #endregion

        #region Static Method
        public static PXResultset<ARTran> RetriveARTran(PXGraph graph, string formatCode, string gUINbr)
        {
            return SelectFrom<ARTran>.InnerJoin<TWNGUITrans>.On<ARTran.refNbr.IsEqual<TWNGUITrans.orderNbr>>
                                     .Where<TWNGUITrans.gUIFormatcode.IsEqual<@P.AsString>
                                            .And<TWNGUITrans.gUINbr.IsEqual<@P.AsString>>>
                                     .OrderBy<Asc<ARTran.inventoryID>>.View.Select(graph, formatCode, gUINbr);
        }
        #endregion
    }
}