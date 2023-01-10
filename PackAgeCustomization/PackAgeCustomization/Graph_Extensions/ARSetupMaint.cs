using PX.Data;
using PX.Objects.CA;
using PX.SM;

namespace PX.Objects.AR
{
  public class ARSetupMaint_Extension : PXGraphExtension<PX.Objects.AR.ARSetupMaint>
  {
        #region Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Replace)]
		[PXDBString(8, InputMask = "CC.CC.CC.CC")]
		[PXUIField(DisplayName = "Report")]
		[PXSelector(typeof(Search<SiteMap.screenID, Where<SiteMap.url, Like<Common.urlReports>,
														 And<Where<SiteMap.screenID, Like<PXModule.ar_>,
																   Or<SiteMap.screenID, Like<PXModule2.tw_>>>>>,
													OrderBy<Asc<SiteMap.screenID>>>), 
					typeof(SiteMap.screenID), 
					typeof(SiteMap.title),
					Headers = new string[] { CA.Messages.ReportID, CA.Messages.ReportName },
					DescriptionField = typeof(SiteMap.title))]
		protected void _(Events.CacheAttached<ARNotification.reportID> e) { }
        #endregion
    }

	public class PXModule2 : PXModule
    {
		public const string TW = "TW";

		public class tw_ : PX.Data.BQL.BqlString.Constant<tw_>
		{
			public tw_() : base($"{TW}%") { }
		}
	}
}