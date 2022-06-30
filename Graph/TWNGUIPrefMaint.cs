using System.Web;
using PX.Data;
using PX.Data.BQL.Fluent;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace eGUICustomizations.Graph
{
    public class TWNGUIPrefMaint : PXGraph<TWNGUIPrefMaint>
    {
        public PXSave<TWNGUIPreferences> Save;
        public PXCancel<TWNGUIPreferences> Cancel;

        public SelectFrom<TWNGUIPreferences>.View GUIPreferences;

        #region Override Methods
        public override void Persist()
        {
            TWNSiteMapControlHandler.UpdateMenuItem(new string[] { "TW402000", "TW505000" }, !(GUIPreferences.Current?.EnableWHT ?? false));

            base.Persist();

            PX.Data.Redirector.Refresh(HttpContext.Current);
        }
        #endregion
    }
}