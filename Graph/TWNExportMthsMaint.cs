using PX.Data;
using PX.Data.BQL.Fluent;
using eGUICustomizations.DAC;

namespace eGUICustomizations.Graph
{
    public class TWNExportMthsMaint : PXGraph<TWNExportMthsMaint>
    {
        public PXSavePerRow<TWNExportMethods> Save;
        public PXCancel<TWNExportMethods> Cancel;

        [PXImport(typeof(TWNExportMethods))]
        public SelectFrom<TWNExportMethods>.View ExportMethods;
    }
}