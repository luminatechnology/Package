using PX.Data;
using PX.Objects.CA;
using PX.Objects.CS;
using PX.SM;

namespace PX.Objects.AR
{
	public class CustomerMaint_Extension : PXGraphExtension<PX.Objects.AR.CustomerMaint>
	{
		#region Cache Attached
		[PXMergeAttributes(Method = MergeMethod.Replace)]
		[PXDBString(8, InputMask = "CC.CC.CC.CC")]
		[PXUIField(DisplayName = "Report")]
		[PXDefault(typeof(Search<NotificationSetup.reportID, Where<NotificationSetup.setupID, Equal<Current<NotificationSource.setupID>>>>),
				   PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<SiteMap.screenID, Where<SiteMap.url, Like<Common.urlReports>,
														  And<Where<SiteMap.screenID, Like<PXModule.ar_>,
																	Or<SiteMap.screenID, Like<PXModule.so_>,
																	   Or<SiteMap.screenID, Like<PXModule.cr_>,
																		  Or<SiteMap.screenID, Like<PXModule2.tw_>>>>>>>,
													OrderBy<Asc<SiteMap.screenID>>>), 
					typeof(SiteMap.screenID), 
					typeof(SiteMap.title),
					Headers = new string[] { CA.Messages.ReportID, CA.Messages.ReportName },
					DescriptionField = typeof(SiteMap.title))]
		[PXFormula(typeof(Default<NotificationSource.setupID>))]
		protected void _(Events.CacheAttached<NotificationSource.reportID> e) { }
		#endregion
	}
}