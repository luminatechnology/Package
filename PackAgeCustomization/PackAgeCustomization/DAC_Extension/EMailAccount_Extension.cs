using PX.Data;
using PX.SM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackAgeCustomization.DAC_Extension
{
    public class EMailAccount_Extension : PXCacheExtension<EMailAccount>
    {
        [PXDBString(100, IsUnicode = true)]
        [PXUIField(DisplayName = "Sender Name")]
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        public virtual string AccountDisplayName { get; set; }
    }
}
