using PX.Data;
using eGUICustomizations.Descriptor;

namespace PX.Objects.TX
{
    public class SalesTaxMaint_Extension : PXGraphExtension<SalesTaxMaint>
    {
        #region Event Handlers
        protected void _(Events.RowSelected<Tax> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            bool IsActivate = TWNGUIValidation.ActivateTWGUI(Base);

            PXUIFieldAttribute.SetVisible<TaxExt.usrGUIType>(e.Cache, null, IsActivate);
            PXUIFieldAttribute.SetVisible<TaxExt.usrTWNGUI> (e.Cache, null, IsActivate);
        }
        #endregion
    }
}