using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackAgeCustomization.DAC;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.SO;

namespace PackAgeCustomization.DAC_Inheritance
{
    public class LUMSOOrder : IBqlTable
    {

        [PXSelector(typeof(SelectFrom<SOOrder>
                          .LeftJoin<SOOrderKvExt>.On<SOOrder.noteID.IsEqual<SOOrderKvExt.recordID>>
                          .LeftJoin<BAccount>.On<BAccount.acctCD.IsEqual<SOOrderKvExt.valueString>>
                          .Where<SOOrder.orderType.IsEqual<SOType_TOAttr>>
                          .SearchFor<SOOrder.orderNbr>),
            typeof(SOOrder.orderType),
            typeof(SOOrder.orderNbr),
            typeof(SOOrder.status),
            typeof(SOOrder.orderDate),
            typeof(SOOrder.orderDesc),
            typeof(BAccount.acctName),
           SubstituteKey = typeof(SOOrder.orderNbr))]
        public virtual String OrderNbr { get; set; }
    }

    public class SOType_TOAttr : PX.Data.BQL.BqlString.Constant<SOType_TOAttr>
    {
        public SOType_TOAttr() : base("TO") { }
    }

    public class CUSTOMERAttr : PX.Data.BQL.BqlString.Constant<CUSTOMERAttr>
    {
        public CUSTOMERAttr() : base("AttributeCUSTOMER") { }
    }
}
