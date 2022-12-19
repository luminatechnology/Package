using System;
using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.AR;
using System.Collections;
using System.Collections.Generic;
using eGUICustomizations.Descriptor;

namespace PX.Objects.SO
{
    public class SOInvoiceEntry_Extension : PXGraphExtension<SOInvoiceEntry_Workflow, SOInvoiceEntry>
    {
        public const string GUIReportID = "TW641000";

        #region Override Methods
        public override void Configure(PXScreenConfiguration config)
        {
            Configure(config.GetScreenConfigurationContext<SOInvoiceEntry, ARInvoice>());
        }

        protected virtual void Configure(WorkflowContext<SOInvoiceEntry, ARInvoice> context)
        {
            context.UpdateScreenConfigurationFor(screen =>
            {
                return screen.WithActions(actions =>
                {
                    actions.Add<SOInvoiceEntry_Extension>(e => e.printGUIInvoice,
                                                          a => a.WithCategory((PredefinedCategory)FolderType.ReportsFolder).PlaceAfter(s => s.printInvoice));
                });
            });
        }
        #endregion

        #region Actions
        public PXAction<ARInvoice> printGUIInvoice;
        [PXButton()]
        [PXUIField(DisplayName = "GUI Invoice", MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable PrintGUIInvoice(PXAdapter adapter)
        {
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    [nameof(ARRegister.DocType)] = Base.Document.Current.DocType,
                    [nameof(ARRegister.RefNbr)] = Base.Document.Current.RefNbr
                };

                throw new PXReportRequiredException(parameters, GUIReportID, GUIReportID);
            }

            return adapter.Get();
        }
        #endregion

        #region Event Handlers
        protected void _(Events.RowSelected<SOInvoice> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            bool activateGUI = TWNGUIValidation.ActivateTWGUI(e.Cache.Graph);

            Base.report.SetVisible(nameof(PrintGUIInvoice), activateGUI);

            printGUIInvoice.SetEnabled(activateGUI && !string.IsNullOrEmpty(Base.Document.Current.GetExtension<ARRegisterExt>()?.UsrGUINbr));
        }

        protected void _(Events.RowUpdating<SOOrder> e, PXRowUpdating baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            var invoice = Base.Document.Current;

            Guid? customerNoteID = Base.customer.Current?.NoteID;

            Base.Document.Cache.SetValue<ARRegisterExt.usrSummaryPrint>(invoice, Convert.ToBoolean(Convert.ToInt32(CS.CSAnswers.PK.Find(Base, customerNoteID, "GUISUMPRNT")?.Value ?? "0")));
            Base.Document.Cache.SetValue<ARRegisterExt.usrGUISummary>(invoice, CS.CSAnswers.PK.Find(Base, customerNoteID, ARRegisterExt.GUISummary)?.Value);

            var customer_Attr = CS.CSAnswers.PK.Find(Base, customerNoteID, "GUIECINV");

            if (invoice != null && customer_Attr != null & Convert.ToBoolean(Convert.ToInt32(customer_Attr?.Value ?? "0")) == true)
            {
                Base.Document.Cache.SetValue<ARRegisterExt.usrTaxNbr>(invoice, GetSOrderUDFValue(e.Row, ARRegisterExt.TaxNbrName));
                Base.Document.Cache.SetValue<ARRegisterExt.usrGUITitle>(invoice, GetSOrderUDFValue(e.Row, "GUITITLE"));

                string carrierID = (string)GetSOrderUDFValue(e.Row, "GUICARRIER");
                Base.Document.Cache.SetValue<ARRegisterExt.usrCarrierID>(invoice, carrierID);

                string nPONbr = (string)GetSOrderUDFValue(e.Row, "GUINPONBR");
                Base.Document.Cache.SetValue<ARRegisterExt.usrNPONbr>(invoice, nPONbr);
                Base.Document.Cache.SetValue<ARRegisterExt.usrB2CType>(invoice, !string.IsNullOrEmpty(carrierID) ? TWNStringList.TWNB2CType.MC :
                                                                                                                   !string.IsNullOrEmpty(nPONbr) ? TWNStringList.TWNB2CType.NPO :
                                                                                                                                                   TWNStringList.TWNB2CType.DEF);
            }

            if (e.Row.CuryUnpaidBalance == 0m && Convert.ToBoolean(Convert.ToInt32(CS.CSAnswers.PK.Find(Base, customerNoteID, ARPaymentEntry_Extension.PRINTPREPA_Attr)?.Value ?? "0")) == true)
            {
                Base.Document.Cache.SetValue<ARRegisterExt.usrVATOutCode>(invoice, eGUICustomizations.DAC.TWGUIFormatCode.vATOutCode36);
                Base.Document.Cache.SetValue<ARRegisterExt.usrSummaryPrint>(invoice, null);
                Base.Document.Cache.SetValue<ARRegisterExt.usrGUISummary>(invoice, null);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Return the User-Defined Field value on Sales Order depend on differnt attribute ID.
        /// </summary>
        public virtual object GetSOrderUDFValue(SOOrder order, string SOrderUDF_Attr)
        {
            var order_State = Base.soorder.Cache.GetValueExt(order, $"{CS.Messages.Attribute}{SOrderUDF_Attr}") as PXFieldState;

            return order_State?.Value;
        }
        #endregion
    }
}