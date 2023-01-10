using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.CR;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace eGUICustomizations.Graph
{
    public class TWNGenWHTFile : PXGraph<TWNGenWHTFile>
    {
        #region Filter & Process & Features
        public PXCancel<WHTTranFilter> Cancel;
        public PXFilter<WHTTranFilter> Filter;

        public PXFilteredProcessing<TWNWHTTran, WHTTranFilter,
                                    Where<TWNWHTTran.tranDate, GreaterEqual<Current<WHTTranFilter.fromDate>>,
                                          And<TWNWHTTran.tranDate, LessEqual<Current<WHTTranFilter.toDate>>,
                                              And<TWNWHTTran.paymDate, GreaterEqual<Current<WHTTranFilter.fromPaymDate>>,
                                                  And<TWNWHTTran.paymDate, LessEqual<Current<WHTTranFilter.toPaymDate>>>>>>> WHTTranProc;

        public PXSetup<TWNGUIPreferences> GUISetup;
        #endregion

        #region Constructor
        public TWNGenWHTFile()
        {
            WHTTranProc.SetProcessCaption(ActionsMessages.Export);
            WHTTranProc.SetProcessAllCaption(TWMessages.ExportAll);
            ///<remarks> Because the file is downloaded using the throw exception, an error message may be displayed even if there is no error.///</remarks>
            WHTTranProc.SetProcessDelegate(Export);
            //WHTTranProc.SetProcessDelegate(delegate (List<TWNWHTTran> list)
            //{
            //    TWNGenWHTFile graph = CreateInstance<TWNGenWHTFile>();

            //    graph.Export(list);
            //});
        }
        #endregion

        #region Methods
        TWNGenGUIMediaFile mediaGraph = PXGraph.CreateInstance<TWNGenGUIMediaFile>();

        public void Export(List<TWNWHTTran> wHTTranList)
        {
            try
            {
                TWNGUIPreferences gUIPreferences = GUISetup.Current;

                int count = 1;
                string lines = "", fileName = "";

                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(stream, Encoding.ASCII))
                    {
                        fileName = this.Accessinfo.BusinessDate.Value.ToString("yyyyMMdd") + ".txt";

                        foreach (TWNWHTTran row in wHTTranList)
                        {
                            // 稽徵機關代號
                            lines = gUIPreferences?.WHTTaxAuthority;
                            // 流水號
                            lines += AutoNumberAttribute.GetNextNumber(WHTTranProc.Cache, row, gUIPreferences.WHTFileNumbering, Accessinfo.BusinessDate);
                            // 扣繳單位或營利事業統一編號
                            lines += gUIPreferences.OurTaxNbr;
                            // Remark
                            lines += mediaGraph.space;
                            // Format Code
                            lines += row.WHTFmtCode;
                            // Personal ID
                            lines += row.PersonalID.PadRight(10, ' ');
                            // Type Of Income
                            lines += row.TypeOfIn;
                            // Payment Budget
                            lines += PadAmt2FixedStrLength((row.WHTAmt + row.SecNHIAmt + row.NetAmt).Value);
                            // Tax Amount
                            lines += PadAmt2FixedStrLength(row.WHTAmt.Value);
                            // Net Amount
                            lines += PadAmt2FixedStrLength((row.SecNHIAmt + row.NetAmt).Value);
                            // 租賃房屋稅籍編號、執行業務者業別代號、所得人代號或帳號、外僑護照號碼
                            lines += GetFormatNbr(row);
                            // Blank
                            lines += mediaGraph.space;
                            // Error Remark
                            lines += mediaGraph.space;
                            // Income Year
                            lines += mediaGraph.GetGUILegal(row.PaymDate.Value);
                            // Income Chinese Name
                            lines += row.PayeeName;
                            // Income Chinese Address
                            lines += row.PayeeAddr;
                            // 所得所屬期間
                            // The format is YYYMMYYYMM (From payment date and To payment date), the Chinese year and month are retrieved from the payment date entered in the selection criteria. 
                            lines += mediaGraph.GetGUILegal(Filter.Current.FromPaymDate.Value) + mediaGraph.GetGUILegal(Filter.Current.ToPaymDate.Value);
                            // Blank
                            lines += mediaGraph.space;
                            // 是否滿183天
                            // Default is blank, if 證號別 = 5 or 6 or 7 or 8 or 9 then "N"
                            lines += (int.Parse(row.TypeOfIn) >= 5) ? 'N' : mediaGraph.space;
                            // Country
                            lines += GetVendCountryID(row.DocType, row.RefNbr);
                            // Tax Code
                            lines += mediaGraph.space;
                            // Blank
                            lines += mediaGraph.space;
                            // File Created Date
                            lines += this.Accessinfo.BusinessDate.Value.ToString("MMdd");

                            // Only the last line does not need to be broken.
                            if (count < wHTTranList.Count)
                            {
                                sw.WriteLine(lines);
                                count++;
                            }
                            else
                            {
                                sw.Write(lines);
                            }
                        }

                        sw.Close();

                        throw new PXRedirectToFileException(new PX.SM.FileInfo(Guid.NewGuid(), fileName,
                                                                               null, stream.ToArray(), string.Empty),
                                                            true);
                    }
                }
            }
            catch (Exception ex)
            {
                PXProcessing<TWNWHTTran>.SetError(ex);
                throw;
            }
        }

        protected string PadAmt2FixedStrLength(decimal amount)
        {
            mediaGraph.fixedLen  = 10;
            mediaGraph.combinStr = amount.ToString();
            
            return mediaGraph.combinStr.PadLeft(mediaGraph.fixedLen, mediaGraph.zero);
        }

        /// <summary>
        /// If TWN_WhtTrans.FormatCode ='9A', then TWN_WhtTrans.FormatSubCode 
        /// else(TWN_WhtTrans.PropertyID if TWN_WhtTrans.PropertyID is not blank)
        /// </summary>
        protected string GetFormatNbr(TWNWHTTran wHTTran)
        {
            if (wHTTran.WHTFmtCode == "9A")
            {
                return wHTTran.WHTFmtSub;
            }
            else
            {
                return string.IsNullOrEmpty(wHTTran.PropertyID) ? new string(mediaGraph.space, 12) : wHTTran.PropertyID;
            }
        }

        protected string GetVendCountryID(string docType, string refNbr)
        {
            Address address= SelectFrom<Address>.LeftJoin<BAccount>.On<BAccount.bAccountID.IsEqual<Address.bAccountID>
                                                                       .And<BAccount.defAddressID.IsEqual<Address.addressID>>>
                                                .LeftJoin<APRegister>.On<APRegister.vendorID.IsEqual<BAccount.bAccountID>>
                                                .Where<APRegister.docType.IsEqual<@P.AsString>
                                                      .And<APRegister.refNbr.IsEqual<@P.AsString>>>.View.ReadOnly.Select(this, docType, refNbr);

            return address != null ? address.CountryID : new string(mediaGraph.space, 2);
        }
        #endregion
    }

    #region Filter DAC
    [Serializable]
    [PXCacheName("Withholding Tax Trans Filter")]
    public class WHTTranFilter : GUITransFilter
    {
        #region FromDate
        public new abstract class fromDate : PX.Data.BQL.BqlDateTime.Field<fromDate> { }
        #endregion

        #region ToDate
        public new abstract class toDate : PX.Data.BQL.BqlDateTime.Field<toDate> { }
        #endregion

        #region FromPaymDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "From Payment Date", Visibility = PXUIVisibility.Visible)]
        public virtual DateTime? FromPaymDate { get; set; }
        public abstract class fromPaymDate : PX.Data.BQL.BqlDateTime.Field<fromPaymDate> { }
        #endregion

        #region ToPaymDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "To Payment Date", Visibility = PXUIVisibility.Visible)]
        public virtual DateTime? ToPaymDate { get; set; }
        public abstract class toPaymDate : PX.Data.BQL.BqlDateTime.Field<toPaymDate> { }
        #endregion
    }
    #endregion  
}