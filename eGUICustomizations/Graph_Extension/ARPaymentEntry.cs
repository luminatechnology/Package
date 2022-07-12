using System;
using System.Collections;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.WorkflowAPI;
using PX.Objects.TX;
using eGUICustomizations.Descriptor;
using eGUICustomizations.DAC;
using eGUICustomizations.Graph_Release;

namespace PX.Objects.AR
{
    public class ARPaymentEntry_Extension : PXGraphExtension<ARPaymentEntry_Workflow, ARPaymentEntry>
    {
        public bool activateGUI = TWNGUIValidation.ActivateTWGUI(new PXGraph());
        public TWNReleaseProcess rp = PXGraph.CreateInstance<TWNReleaseProcess>();

        #region Selects
        [PXCopyPasteHiddenFields(typeof(ARRegisterExt.usrGUINbr))]
        public SelectFrom<ARRegister>.Where<ARRegister.docType.IsEqual<ARPayment.docType.FromCurrent>
                                            .And<ARRegister.refNbr.IsEqual<ARPayment.refNbr.FromCurrent>>>.View Register;
        #endregion

        #region Override/Delegate Methods

        #region ARPaymentEntry
        public delegate void PersistDelgate();
        [PXOverride]
        public void Persist(PersistDelgate baseMethod)
        {
            foreach (ARPayment row in Base.CurrentDocument.Cache.Deleted)
            {
                if (row.DocType == ARDocType.Prepayment)
                {
                    rp.GenerateVoidedGUI(true, (ARRegister)row);
                }
            }

            baseMethod();
        }
        #endregion

        #region ARPaymentEntry_Workflow
        public override void Configure(PXScreenConfiguration config)
        {
            Configure(config.GetScreenConfigurationContext<ARPaymentEntry, ARPayment>());
        }

        protected virtual void Configure(WorkflowContext<ARPaymentEntry, ARPayment> context)
        {
            var processingCategory = context.Categories.Get(AR.ARPaymentEntry_Workflow.CategoryID.Processing);

            context.UpdateScreenConfigurationFor(screen =>
            {
                return screen.UpdateDefaultFlow(flow => flow.WithFlowStates(states =>
                                                                            {
                                                                                states.Update<ARDocStatus.open>(flowState =>
                                                                                {
                                                                                    return flowState.WithActions(actions =>
                                                                                                                 {
                                                                                                                     actions.Add<ARPaymentEntry_Extension>(g => g.generateGUI/*, a => a.IsDuplicatedInToolbar()*/);
                                                                                                                 });
                                                                                                    //.WithEventHandlers(handlers =>
                                                                                                    //{
                                                                                                    //    handlers.Add(a => a.OnReleaseDocument);
                                                                                                    //});
                                                                                });
                                                                                states.Update<ARDocStatus.closed>(flowState =>
                                                                                {
                                                                                    return flowState.WithActions(actions =>
                                                                                                                 {
                                                                                                                     actions.Add<ARPaymentEntry_Extension>(g => g.generateGUI);
                                                                                                                 });
                                                                                                    //.WithEventHandlers(handlers =>
                                                                                                    //{
                                                                                                    //    handlers(a => a.OnReleaseDocument);
                                                                                                    //});
                                                                                });
                                                                            }))
                                .WithActions(actions =>
                                {
                                    actions.Add<ARPaymentEntry_Extension>(g => g.generateGUI, c => c.WithCategory(processingCategory));
                                });
            });
        }
        #endregion

        #endregion

        #region Actions
        public PXAction<ARPayment> generateGUI;
        [PXProcessButton(CommitChanges = true), PXUIField(DisplayName = "Generate GUI", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable GenerateGUI(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(Base, delegate ()
            {
                var register = Base.CurrentDocument.Current?.GetExtension<ARRegisterExt>();

                new TWNGUIValidation().CheckGUINbrExisted(Base, register?.UsrGUINbr, register?.UsrVATOutCode);

                ///<remarks>It is enforced again whether it is disabled or not.</remarks>
                Base.release.Press();
            });

            return adapter.Get();
        }
        #endregion

        #region Event Handlers
        protected void _(Events.RowSelected<ARPayment> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            if (e.Row == null) { return; }

            bool isPrepayment = e.Row.DocType.IsIn(ARDocType.Prepayment, ARDocType.VoidPayment);

            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrGUIDate>   (e.Cache, null, activateGUI && isPrepayment);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrGUINbr>    (e.Cache, null, activateGUI && isPrepayment);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrOurTaxNbr> (e.Cache, null, activateGUI && isPrepayment);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrTaxNbr>    (e.Cache, null, activateGUI && isPrepayment);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrVATOutCode>(e.Cache, null, activateGUI && isPrepayment);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrB2CType>   (e.Cache, null, activateGUI && isPrepayment);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrCarrierID> (e.Cache, null, activateGUI && isPrepayment);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrNPONbr>    (e.Cache, null, activateGUI && isPrepayment);
            PXUIFieldAttribute.SetVisible< ARRegisterExt.usrVATType>  (e.Cache, null, activateGUI && isPrepayment);

            generateGUI.SetVisible(activateGUI);

            ARRegisterExt regisExt = e.Row.GetExtension<ARRegisterExt>();

            bool taxNbrBlank = string.IsNullOrEmpty(regisExt.UsrTaxNbr);
            bool noGUINbr    = string.IsNullOrEmpty(regisExt.UsrGUINbr);
            bool hasReleased = e.Row.Status.IsIn(ARDocStatus.Open, ARDocStatus.Closed);

            PXUIFieldAttribute.SetEnabled<ARRegisterExt.usrB2CType>   (e.Cache, e.Row, !hasReleased && taxNbrBlank);
            PXUIFieldAttribute.SetEnabled<ARRegisterExt.usrCarrierID> (e.Cache, e.Row, !hasReleased && taxNbrBlank && regisExt.UsrB2CType == TWNStringList.TWNB2CType.MC);
            PXUIFieldAttribute.SetEnabled<ARRegisterExt.usrNPONbr>    (e.Cache, e.Row, !hasReleased && taxNbrBlank && regisExt.UsrB2CType == TWNStringList.TWNB2CType.NPO);
            PXUIFieldAttribute.SetEnabled<ARRegisterExt.usrVATOutCode>(e.Cache, e.Row, noGUINbr);
            PXUIFieldAttribute.SetEnabled<ARRegisterExt.usrGUIDate>   (e.Cache, e.Row, noGUINbr);

            generateGUI.SetEnabled(hasReleased == true);
        }

        protected void _(Events.RowPersisting<ARPayment> e, PXRowPersisting baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
            {
                ARRegisterExt regisExt = e.Row.GetExtension<ARRegisterExt>();

                if (string.IsNullOrEmpty(regisExt.UsrGUINbr) && regisExt.UsrVATOutCode.IsIn(TWGUIFormatCode.vATOutCode31, TWGUIFormatCode.vATOutCode32, TWGUIFormatCode.vATOutCode35))
                {
                    TWNGUIPreferences pref = SelectFrom<TWNGUIPreferences>.View.Select(Base);

                    regisExt.UsrGUINbr = ARGUINbrAutoNumAttribute.GetNextNumber(e.Cache, e.Row, regisExt.UsrVATOutCode == TWGUIFormatCode.vATOutCode32 ? pref.GUI2CopiesNumbering : pref.GUI3CopiesNumbering, regisExt.UsrGUIDate);

                    new TWNGUIValidation().CheckGUINbrExisted(Base, regisExt.UsrGUINbr, regisExt.UsrVATOutCode);
                }
            }
        }

        protected void _(Events.FieldUpdated<ARPayment.customerID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            e.Cache.SetDefaultExt<ARRegisterExt.usrVATOutCode>(e.Row);
            e.Cache.SetDefaultExt<ARRegisterExt.usrTaxNbr>(e.Row);
            e.Cache.SetDefaultExt<ARRegisterExt.usrOurTaxNbr>(e.Row);
            e.Cache.SetDefaultExt<ARRegisterExt.usrVATType>(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<ARRegisterExt.usrVATOutCode> e)
        {
            Guid? custNoteID = Base.customer.Current?.NoteID;

            if (custNoteID != null && Base.CurrentDocument.Current?.DocType == ARDocType.Prepayment)
            {
                var value = CS.CSAnswers.PK.Find(Base, custNoteID, "PRINTPREPA")?.Value;

                e.NewValue = Convert.ToBoolean(Convert.ToInt32(value)) == true ? CS.CSAnswers.PK.Find(Base, custNoteID, ARRegisterExt.VATOUTFRMTName)?.Value : null;
            }
        }

        protected virtual void _(Events.FieldDefaulting<ARRegisterExt.usrVATType> e)
        {
            ARPayment row = e.Row as ARPayment;

            if (row == null) { return; }

            ///<remarks>Determined by Tax Zone and Tax Category. Tax Zone is default from customer master (Location.CTaxZoneID). TaxCategory is default form Tax Zone(TaxZone.Dflttaxcategory)</remarks>
            Tax tax = SelectFrom<Tax>.InnerJoin<TaxZoneDet>.On<TaxZoneDet.FK.Tax>
                                     .InnerJoin<CR.Location>.On<CR.Location.cTaxZoneID.IsEqual<TaxZoneDet.taxZoneID>>
                                     .Where<CR.Location.bAccountID.IsEqual<@P.AsInt>>.View.SelectSingleBound(Base, null, row.CustomerID);

            e.NewValue = tax?.GetExtension<TaxExt>().UsrGUIType;
        }
        #endregion
    }
}