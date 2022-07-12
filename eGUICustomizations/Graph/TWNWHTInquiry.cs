using PX.Data;
using PX.Data.BQL.Fluent;
using eGUICustomizations.DAC;

namespace eGUICustomizations.Graph
{
    public class TWNWHTInquiry : PXGraph<TWNWHTInquiry>
    {
        #region Select & Features
        public PXSavePerRow<TWNWHTTran> Save;
        public PXCancel<TWNWHTTran> Cancel;

        [PXFilterable]
        public SelectFrom<TWNWHTTran>.View WHTTran;
        #endregion

        #region Event Handlers
        protected virtual void _(Events.RowSelected<TWNWHTTran> e)
        {
            PXUIFieldAttribute.SetEnabled<TWNWHTTran.docType>(e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNWHTTran.refNbr>(e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNWHTTran.batchNbr>(e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNWHTTran.tranDate>(e.Cache, e.Row, false);
        }
        #endregion
    }
}