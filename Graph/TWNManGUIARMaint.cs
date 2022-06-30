using System.Collections;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.TX;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;
using eGUICustomizations.Graph_Release;

namespace eGUICustomizations.Graph
{
    public class TWNManGUIAREntry : PXGraph<TWNManGUIAREntry>
    {
        public TWNReleaseProcess rp = PXGraph.CreateInstance<TWNReleaseProcess>();

        #region Selects & Setup
        public PXSave<TWNManualGUIAR> Save;
        public PXCancel<TWNManualGUIAR> Cancel;

        [PXImport(typeof(TWNManualGUIAR))]
        [PXFilterable]
        public SelectFrom<TWNManualGUIAR>
                          .Where<TWNManualGUIAR.status.IsEqual<TWNStringList.TWNGUIManualStatus.open>>.View manualGUIAR_Open;
        public SelectFrom<TWNManualGUIAR>
                          .Where<TWNManualGUIAR.status.IsEqual<TWNStringList.TWNGUIManualStatus.released>>.View.ReadOnly manualGUIAR_Released;

        public SelectFrom<TWNGUITrans>
                         .Where<TWNGUITrans.gUINbr.IsEqual<TWNManualGUIAR.gUINbr.FromCurrent>>.View ViewGUITrans;

        public PXSetup<TWNGUIPreferences> GUIPreferences;
        #endregion

        #region Overrie Methods
        public override void Persist()
        {
            foreach (TWNManualGUIAR row in manualGUIAR_Open.Cache.Deleted)
            {
                if (tWNGUIValidation.isCreditNote == false && !string.IsNullOrEmpty(row.GUINbr))
                {
                    Customer customer = Customer.PK.Find(this, row.CustomerID);

                    rp.CreateGUITrans(new STWNGUITran()
                    {
                        VATCode       = row.VatOutCode,
                        GUINbr        = row.GUINbr,
                        GUIStatus     = TWNStringList.TWNGUIStatus.Voided,
                        GUIDirection  = TWNStringList.TWNGUIDirection.Issue,
                        GUIDate       = row.GUIDate,
                        TaxZoneID     = row.TaxZoneID,
                        TaxCategoryID = row.TaxCategoryID,
                        TaxID         = row.TaxID,
                        TaxNbr        = row.TaxNbr,
                        OurTaxNbr     = row.OurTaxNbr,
                        NetAmount     = 0,
                        TaxAmount     = 0,
                        AcctCD        = customer.AcctCD,
                        AcctName      = customer.AcctName,
                        Remark        = string.Format(TWMessages.DeleteInfo, this.Accessinfo.UserName),
                    });
                }
            }

            base.Persist();
        }
        #endregion

        #region Actions
        public PXAction<TWNManualGUIAR> Release;
        [PXProcessButton()]
        [PXUIField(DisplayName = ActionsMessages.Release)]
        public IEnumerable release(PXAdapter adapter)
        {
            TWNManualGUIAR manualGUIAR = this.manualGUIAR_Open.Current;

            if (manualGUIAR_Open.Current == null || string.IsNullOrEmpty(manualGUIAR.GUINbr))
            {
                throw new PXException(TWMessages.GUINbrIsMandat);
            }
            else
            {
                Customer customer = Customer.PK.Find(this, manualGUIAR.CustomerID);

                //TaxExt taxExt = PXCache<Tax>.GetExtension<TaxExt>(SelectFrom<Tax>
                //                                                            .Where<Tax.taxID.IsEqual<@P.AsString>>.View.Select(this, manualGUIAR.TaxID) );

                PXLongOperation.StartOperation(this, delegate
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        TWNGUITrans tWNGUITrans = rp.InitAndCheckOnAR(manualGUIAR.GUINbr, manualGUIAR.VatOutCode);

                        rp.CreateGUITrans(new STWNGUITran()
                        {
                            VATCode       = manualGUIAR.VatOutCode,
                            GUINbr        = manualGUIAR.GUINbr,
                            GUIStatus     = TWNStringList.TWNGUIStatus.Used,
                            GUIDirection  = TWNStringList.TWNGUIDirection.Issue,
                            GUIDate       = manualGUIAR.GUIDate,
                            GUITitle      = customer.AcctName,
                            TaxZoneID     = manualGUIAR.TaxZoneID,
                            TaxCategoryID = manualGUIAR.TaxCategoryID,
                            TaxID         = manualGUIAR.TaxID,
                            TaxNbr        = manualGUIAR.TaxNbr,
                            OurTaxNbr     = manualGUIAR.OurTaxNbr,
                            NetAmount     = manualGUIAR.NetAmt,
                            TaxAmount     = manualGUIAR.TaxAmt,
                            AcctCD        = customer.AcctCD,
                            AcctName      = customer.AcctName,
                            eGUIExcluded  = true,
                            Remark        = manualGUIAR.Remark
                        });

                        manualGUIAR.Status = TWNStringList.TWNGUIManualStatus.Released;
                        manualGUIAR_Open.Cache.MarkUpdated(manualGUIAR);

                        Actions.PressSave();

                        if (tWNGUITrans != null)
                        {
                            ViewGUITrans.SetValueExt<TWNGUITrans.netAmtRemain>(tWNGUITrans, (tWNGUITrans.NetAmtRemain -= manualGUIAR.NetAmt));
                            ViewGUITrans.SetValueExt<TWNGUITrans.taxAmtRemain>(tWNGUITrans, (tWNGUITrans.TaxAmtRemain -= manualGUIAR.TaxAmt));
                            ViewGUITrans.Update(tWNGUITrans);
                        }

                        ts.Complete();
                    }
                });
            }

            return adapter.Get();
        }
        #endregion

        #region Event Handlers
        TWNGUIValidation tWNGUIValidation = new TWNGUIValidation();

        protected virtual void _(Events.RowPersisting<TWNManualGUIAR> e)
        {
            if (e.Row.VatOutCode == TWGUIFormatCode.vATOutCode31)
            {
                AutoNumberAttribute.SetNumberingId<TWNManualGUIAR.gUINbr>(e.Cache, GUIPreferences.Current.GUI3CopiesManNumbering);
            }
            else if (e.Row.VatOutCode == TWGUIFormatCode.vATOutCode32)
            {
                AutoNumberAttribute.SetNumberingId<TWNManualGUIAR.gUINbr>(e.Cache, GUIPreferences.Current.GUI2CopiesNumbering);
            }
        }

        //protected virtual void _(Events.FieldVerifying<TWNManualGUIAR.gUINbr> e)
        //{
        //    var row = (TWNManualGUIAR)e.Row;

        //    tWNGUIValidation.CheckGUINbrExisted(this, row.GUINbr, row.VatOutCode);
        //}

        //protected virtual void _(Events.FieldVerifying<TWNManualGUIAR.taxAmt> e)
        //{
        //    var row = (TWNManualGUIAR)e.Row;

        //    e.Cache.RaiseExceptionHandling<TWNManualGUIAR.taxAmt>(row, e.NewValue, tWNGUIValidation.CheckTaxAmount(e.Cache, row.NetAmt.Value, (decimal)e.NewValue));
        //}

        protected virtual void _(Events.FieldUpdated<TWNManualGUIAR.netAmt> e)
        {       
            var row = (TWNManualGUIAR)e.Row;

            foreach (TaxRev taxRev in SelectFrom<TaxRev>.Where<TaxRev.taxID.IsEqual<@P.AsString>
                                                               .And<TaxRev.taxType.IsEqual<@P.AsString>>>
                                                        .View.ReadOnly.Select(this, row.TaxID, "S"))     // S = Group type (Output)
            { 
                row.TaxAmt = row.NetAmt * (taxRev.TaxRate / taxRev.NonDeductibleTaxRate); 
            }
        }
          
        protected virtual void _(Events.FieldUpdated<TWNManualGUIAR.customerID> e)
        {
            var row = (TWNManualGUIAR)e.Row;

            PXResult<Location> result = SelectFrom<Location>.InnerJoin<Customer>.On<Location.bAccountID.IsEqual<Customer.bAccountID>
                                                                                    .And<Location.locationID.IsEqual<Customer.defLocationID>>>
                                                            .InnerJoin<TaxZone>.On<TaxZone.taxZoneID.IsEqual<Location.cTaxZoneID>>
                                                            .InnerJoin<TaxZoneDet>.On<TaxZoneDet.taxZoneID.IsEqual<Location.cTaxZoneID>>
                                                            .Where<Customer.bAccountID.IsEqual<@P.AsInt>>.View.Select(this, row.CustomerID);
            if (result != null)
            {
                row.TaxZoneID     = result.GetItem<Location>().CTaxZoneID;
                row.TaxCategoryID = result.GetItem<TaxZone>().DfltTaxCategoryID;
                row.TaxID         = result.GetItem<TaxZoneDet>().TaxID;

                foreach (CSAnswers cS in SelectFrom<CSAnswers>.Where<CSAnswers.refNoteID.IsEqual<@P.AsGuid>>.View.ReadOnly.Select(this, result.GetItem<Customer>().NoteID))
                {
                    switch (cS.AttributeID)
                    {
                        case ARRegisterExt.VATOUTFRMTName :
                            row.VatOutCode = cS.Value;
                            break;

                        case ARRegisterExt.OurTaxNbrName :
                            row.OurTaxNbr = cS.Value;
                            break;

                        case ARRegisterExt.TaxNbrName :
                            row.TaxNbr = cS.Value;
                            break;
                    }
                }
            }
        }
        #endregion
    }
}