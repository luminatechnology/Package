using System;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;
using eGUICustomizations.Graph_Release;

namespace PX.Objects.AR
{
    public class ARInvoiceEntry_Extension : PXGraphExtension<ARInvoiceEntry>
    {
        public bool activateGUI = TWNGUIValidation.ActivateTWGUI(new PXGraph());

        public TWNReleaseProcess rp = PXGraph.CreateInstance<TWNReleaseProcess>();

        TWNGUIValidation tWNGUIValidation = new TWNGUIValidation();

        //public static bool IsActive() => TWNGUIValidation.ActivateTWGUI(new PXGraph());

        #region Delegate Methods
        public delegate void PersistDelgate();
        [PXOverride]
        public void Persist(PersistDelgate baseMethod)
        {
            foreach (ARRegister row in Base.CurrentDocument.Cache.Deleted)
            {
                rp.GenerateVoidedGUI(true, row);
            }

            baseMethod();
        }
        #endregion

        #region Actions
        public PXAction<ARInvoice> BuyPlasticBag;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Buy Plastic Bag", Visible = false)]
        protected void buyPlasticBag()
        {
            ARTran aRTran = Base.Transactions.Cache.CreateInstance() as ARTran;

            TWNGUIPreferences GUIPreferences = SelectFrom<TWNGUIPreferences>.View.Select(Base);

            if (GUIPreferences.PlasticBag == null)
            {
                throw new MissingMemberException(TWMessages.NoPlasticBag);
            }

            aRTran.InventoryID = GUIPreferences.PlasticBag;
            aRTran.Qty         = 1;

            Base.Transactions.Cache.Insert(aRTran);
        }
        #endregion

        #region Event Handlers
        protected void _(Events.RowSelected<ARInvoice> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            if (e.Row == null) { return; }

            ARRegisterExt registerExt = e.Row.GetExtension<ARRegisterExt>();

            //BuyPlasticBag.SetVisible(activateGUI);

            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrGUIDate>(e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrGUINbr>(e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrOurTaxNbr>(e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrTaxNbr>(e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrVATOutCode>(e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrB2CType>(e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrCarrierID>(e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrNPONbr>(e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<ARRegisterExt.usrCreditAction>(e.Cache, null, activateGUI &&
                                                                                        !string.IsNullOrEmpty(registerExt.UsrVATOutCode) &&
                                                                                        registerExt.UsrVATOutCode.IsIn(TWGUIFormatCode.vATOutCode33, TWGUIFormatCode.vATOutCode34));

            bool taxNbrBlank  = string.IsNullOrEmpty(registerExt.UsrTaxNbr);
            bool statusClosed = e.Row.Status.IsIn(ARDocStatus.Open, ARDocStatus.Closed);

            PXUIFieldAttribute.SetEnabled<ARRegisterExt.usrB2CType>(e.Cache, e.Row, !statusClosed && taxNbrBlank);
            PXUIFieldAttribute.SetEnabled<ARRegisterExt.usrCarrierID>(e.Cache, e.Row, !statusClosed && taxNbrBlank && registerExt.UsrB2CType == TWNStringList.TWNB2CType.MC);
            PXUIFieldAttribute.SetEnabled<ARRegisterExt.usrNPONbr>(e.Cache, e.Row, !statusClosed && taxNbrBlank && registerExt.UsrB2CType == TWNStringList.TWNB2CType.NPO);
            PXUIFieldAttribute.SetEnabled<ARRegisterExt.usrVATOutCode>(e.Cache, e.Row, string.IsNullOrEmpty(registerExt.UsrGUINbr));
            PXUIFieldAttribute.SetEnabled<ARRegisterExt.usrCreditAction>(e.Cache, e.Row, Base.GetDocumentState(e.Cache, e.Row).DocumentDescrEnabled);
        }

        protected void _(Events.RowPersisting<ARInvoice> e, PXRowPersisting baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
            {
                ARRegisterExt regisExt = e.Row.GetExtension<ARRegisterExt>();

                if (string.IsNullOrEmpty(regisExt.UsrGUINbr) && regisExt.UsrVATOutCode.IsIn(TWGUIFormatCode.vATOutCode31, TWGUIFormatCode.vATOutCode32, TWGUIFormatCode.vATOutCode35) )
                {
                    TWNGUIPreferences pref = SelectFrom<TWNGUIPreferences>.View.Select(Base);

                    regisExt.UsrGUINbr = ARGUINbrAutoNumAttribute.GetNextNumber(e.Cache, 
                                                                                e.Row, 
                                                                                regisExt.UsrVATOutCode == TWGUIFormatCode.vATOutCode32 ? pref.GUI2CopiesNumbering : pref.GUI3CopiesNumbering, 
                                                                                regisExt.UsrGUIDate);

                    tWNGUIValidation.CheckGUINbrExisted(Base, regisExt.UsrGUINbr, regisExt.UsrVATOutCode);
                }
            }
        }

        protected void _(Events.RowUpdated<ARInvoice> e, PXRowUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            /// When using the copy and paste feature, don't bring the source GUI number into the new one.
            if (Base.IsCopyPasteContext == true)
            {
                e.Row.GetExtension<ARRegisterExt>().UsrGUINbr = null;
            }
        }

        protected virtual void _(Events.RowInserting<ARInvoice> e)
        {
            if (activateGUI && e.Row.DocType == ARDocType.CreditMemo)
            {
                ARRegisterExt registerExt = e.Row.GetExtension<ARRegisterExt>();

                switch (registerExt.UsrVATOutCode)
                {
                    case TWGUIFormatCode.vATOutCode31:
                    case TWGUIFormatCode.vATOutCode35:
                        registerExt.UsrVATOutCode = TWGUIFormatCode.vATOutCode33;
                        break;

                    case TWGUIFormatCode.vATOutCode32:
                        registerExt.UsrVATOutCode = TWGUIFormatCode.vATOutCode34;
                        break;
                }

                registerExt.UsrCreditAction = TWNStringList.TWNCreditAction.CN;
            }
        }

        protected virtual void _(Events.FieldUpdated<ARInvoice.customerID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            if (activateGUI == true && e.Row is ARInvoice row)
            {
                string vATInCode = null;

                if (row.DocType == ARDocType.CreditMemo)
                {
                    vATInCode = TWGUIFormatCode.vATOutCode33;
                }
                else if (row.DocType.IsIn(ARDocType.Invoice, ARDocType.CashSale))
                {
                    vATInCode = CSAnswers.PK.Find(Base, Base.customer.Current.NoteID, ARRegisterExt.VATOUTFRMTName)?.Value;
                }

                e.Cache.SetValue<ARRegisterExt.usrVATOutCode>(row, vATInCode);
            }
        }

        protected virtual void _(Events.FieldVerifying<ARInvoice.hold> e)
        {
            if (activateGUI && (bool)e.NewValue == false && (bool)e.OldValue == true)
            {
                var row = e.Row as ARInvoice;

                if (row.CuryDocBal != decimal.Zero && string.IsNullOrEmpty(row.GetExtension<ARRegisterExt>().UsrVATOutCode))
                {
                    Base.Document.Ask(TWMessages.RemindHeader, TWMessages.ReminderMesg, MessageButtons.OKCancel);

                    e.Cancel = Base.Document.AskExt() == WebDialogResult.Cancel;
                }
            }
        }

        protected virtual void _(Events.FieldVerifying<ARRegisterExt.usrCreditAction> e)
        {
            string gUINbr = (e.Row as ARRegister)?.GetExtension<ARRegisterExt>().UsrGUINbr;

            if (!string.IsNullOrEmpty(gUINbr))
            {
                bool prepayGUI = SelectFrom<ARRegister>.InnerJoin<TWNGUITrans>.On<TWNGUITrans.orderNbr.IsEqual<ARRegister.refNbr>>
                                                       .Where<TWNGUITrans.gUINbr.IsEqual<@P.AsString>
                                                              .And<TWNGUITrans.gUIFormatcode.IsEqual<@P.AsString>>>.View
                                                       .Select(rp, gUINbr, TWGUIFormatCode.vATOutCode35).TopFirst?.DocType == ARDocType.Prepayment;

                if (prepayGUI == true && (string)e.NewValue == TWNStringList.TWNCreditAction.VG)
                {
                    throw new PXSetPropertyException(TWMessages.CantVoidPrepayGUI, PXErrorLevel.Error);
                }
            }
        }
        #endregion

        #region Static Methods
        public static string AmtInChinese(int amount)
        {
            try
            {
                string m_1, m_2, m_3, m_4, m_5, m_6, m_7, m_8, m_9, Num, type;

                Num  = amount.ToString();
                type = "";
                m_1  = Num;
                
                string numNum = "0123456789.";
                string numChina = "零壹貳參肆伍陸柒捌玖點";
                string numChinaWeigh = "個拾佰仟萬拾佰仟億拾佰仟萬";

                if (Num.Substring(0, 1) == "0") // 0123 -> 123
                {
                    Num = Num.Substring(1, Num.Length - 1);
                }
                if (!Num.Contains('.'))
                {
                    Num += ".00";
                }
                else//123.234  123.23 123.2
                {
                    Num = Num.Substring(0, Num.IndexOf('.') + 1 + (Num.Split('.')[1].Length > 2 ? 3 : Num.Split('.')[1].Length));
                }
                m_1 = Num;
                m_2 = m_1;
                m_3 = m_4 = "";
                //m_2:1234-> 壹貳叁肆
                for (int i = 0; i < 11; i++)
                {
                    m_2 = m_2.Replace(numNum.Substring(i, 1), numChina.Substring(i, 1));
                }
                // m_3:佰拾萬仟佰拾個

                int iLen = m_1.Length;
                if (m_1.IndexOf('.') > 0)
                {
                    iLen = m_1.IndexOf('.');//獲取整數位數
                }
                for (int j = iLen; j >= 1; j--)
                {
                    m_3 += numChinaWeigh.Substring(j - 1, 1);
                }
                //m_4:2行+3行
                for (int i = 0; i < m_3.Length; i++)
                {
                    m_4 += m_2.Substring(i, 1) + m_3.Substring(i, 1);
                }
                //m_5:4行去"0"後拾佰仟
                m_5 = m_4;
                m_5 = m_5.Replace("零拾", "零");
                m_5 = m_5.Replace("零佰", "零");
                m_5 = m_5.Replace("零仟", "零");
                //m_6:00-> 0,000-> 0
                m_6 = m_5;
                for (int i = 0; i < iLen; i++)
                {
                    m_6 = m_6.Replace("零零", "零");
                }
                //m_7:6行去億,萬,個位"0"
                m_7 = m_6;
                m_7 = m_7.Replace("億零萬零", "億零");
                m_7 = m_7.Replace("億零萬", "億零");
                m_7 = m_7.Replace("零億", "億");
                m_7 = m_7.Replace("零萬", "萬");

                if (m_7.Length > 2)
                {
                    m_7 = m_7.Replace("零個", "個");
                }
                //m_8:7行+2行小數-> 數目
                m_8 = m_7;
                m_8 = m_8.Replace("個", "");
                if (m_2.Substring(m_2.Length - 3, 3) != "點零零")
                {
                    m_8 += m_2.Substring(m_2.Length - 3, 3);
                }
                //m_9:7行+2行小數-> 價格
                m_9 = m_7;
                m_9 = m_9.Replace("個", "元");
                if (m_2.Substring(m_2.Length - 3, 3) != "點零零")
                {
                    m_9 += m_2.Substring(m_2.Length - 2, 2);
                    m_9 = m_9.Insert(m_9.Length - 1, "角");
                    m_9 += "分";
                }
                else
                {
                    m_9 += "整";
                }
                if (m_9 != "零元整")
                {
                    m_9 = m_9.Replace("零元", "");
                }
                m_9 = m_9.Replace("零分", "整");

                if (type == "數量")
                {
                    return m_8;
                }
                else
                {
                    return m_9;
                }
            }
            catch
            {
                throw;
            }
        }

        //public static bool CheckAppointmentAmt(ARInvoice invoice)
        //{
        //    FSAppointment appointment = SelectFrom<FSAppointment>.InnerJoin<FSPostDoc>.On<FSPostDoc.appointmentID.IsEqual<FSAppointment.appointmentID>>
        //                                                         .Where<FSPostDoc.postDocType.IsEqual<@P.AsString>
        //                                                                .And<FSPostDoc.postRefNbr.IsEqual<@P.AsString>>>
        //                                                         .View.Select(PXGraph.CreateInstance<ARInvoiceEntry>(), invoice.DocType, invoice.RefNbr);
            
        //    return appointment != null && appointment.CostTotal.Equals(decimal.Zero);
        //}
        #endregion
    }
}