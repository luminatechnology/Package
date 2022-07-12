using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.SiteMap.DAC;
using PX.SiteMap.Graph;

namespace eGUICustomizations.Descriptor
{
    public class TWNSiteMapControlHandler
    {
        #region Workspace Constant Strings 
        public const string Taxes = "3CA80FC2-2FA4-485A-8672-C8832DAA2ACC";
        public const string Finance = "B5EC7B62-D2E5-4234-999D-0C92A0B0B74D";
        public const string Inventory = "6557C1C6-747E-45BB-9072-54F096598D61";
        public const string SalesOrders = "E2C3849A-6280-41DF-81F3-552B91ADFAE5";
        #endregion

        #region Category Constant Strings 
        public const string Empty = "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA";
        public const string Reports = "0B491E12-C58D-4E47-8A0D-96DD3A8AB395";
        public const string Inquiries = "98E86774-69E3-41EA-B94F-EB2C7A8426D4";
        public const string Processes = "32130442-2305-4394-9389-CF915B191D2A";
        public const string Transactions = "38D13A6E-3076-42FB-9FCE-62FA33897DA6";
        public const string PrintedForms = "B731B346-4D68-4796-B797-3E035EF3497C";
        #endregion

        public static void UpdateMenuItem(string[] screenIDs, bool hide = false)
        {
            SiteMapMaint graph = PXGraph.CreateInstance<SiteMapMaint>();

            for (int i = 0; i < screenIDs.Length; i++)
            {
                foreach (SiteMap row in SelectFrom<SiteMap>.Where<SiteMap.screenID.IsEqual<@P.AsString>>.View.Select(graph, screenIDs[i]))
                {
                    string Workspace = Guid.Empty.ToString();
                    string Category = Empty;

                    if (hide == false)
                    {
                        Workspace = Taxes;

                        switch (screenIDs[i].Substring(2, 2))
                        {
                            case "40":
                                Category = Inquiries;
                                break;
                            case "50":
                                Category = Processes;
                                break;
                            case "64":
                                Category = PrintedForms;
                                break;
                        }
                    }

                    graph.SiteMap.Cache.SetValue<SiteMap.workspaces>(row, Workspace);
                    graph.SiteMap.Cache.SetValue<SiteMap.category>(row, Category);

                    graph.SiteMap.Update(row);
                }
            }

            graph.Save.Press();
        }
    }
}
