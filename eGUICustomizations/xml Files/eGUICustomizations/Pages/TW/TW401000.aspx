<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW401000.aspx.cs" Inherits="Page_TW401000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="eGUICustomizations.Graph.TWNeGUIInquiry" PrimaryView="ViewGUITrans" >
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server"></asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid PreservePageIndex="true" KeepPosition="True" FastFilterFields="GUINbr,OrderNbr,CustVend" AdjustPageSize="Auto" AllowPaging="True" AllowSearch="True" Width="100%" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="ViewGUITrans">
			    <Columns>
				<px:PXGridColumn DataField="GUIStatus" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Branch" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Branch_Branch_acctName" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GUIDirection" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GUIFormatcode" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GUINbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GUIDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GUIDecPeriod" Width="150" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxZoneID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxCategoryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxID" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VATType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OurTaxNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NetAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NetAmtRemain" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxAmtRemain" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SequenceNo" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustVend" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustVendName" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DisplayMode="Hint" DataField="DeductionCode" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BatchNbr" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocType" Width="50"></px:PXGridColumn>
				<px:PXGridColumn DataField="OrderNbr" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TransDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PrintedDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ExportMethods" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ExportTicketType" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ExportTicketNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustomType" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ClearingDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Remark" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GUITitle" Width="200" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="EGUIExcluded" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="EGUIExported" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EGUIExportedDateTime" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CarrierType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CarrierID" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NPONbr" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="B2CPrinted" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PrintCount" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="QREncrypter" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CreatedDateTime" Width="90" ></px:PXGridColumn></Columns>
				<RowTemplate>
					<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector1" DataField="ExportMethods" ></px:PXSelector>
					<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector2" DataField="ExportTicketType" ></px:PXSelector>
					<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector3" DataField="NPONbr" ></px:PXSelector>
					<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector5" DataField="TaxCategoryID" ></px:PXSelector>
					<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector6" DataField="TaxID" ></px:PXSelector>
					<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector7" DataField="TaxZoneID" ></px:PXSelector>
					<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector8" DataField="Branch" ></px:PXSelector>
					<px:PXSelector runat="server" ID="CstPXSelector9" DataField="CustVend" AllowEdit="True" ></px:PXSelector>
					<px:PXSelector runat="server" ID="CstPXSelector10" DataField="BatchNbr" AllowEdit="True" ></px:PXSelector></RowTemplate></px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True"></AutoSize>
		<ActionBar PagerVisible="Bottom">
            <PagerSettings Mode="NumericCompact" />
        </ActionBar>
		<Mode InitNewRow="True" AllowUpload="True" AllowAddNew="False" ></Mode>
		<Mode AllowDelete="False" ></Mode></px:PXGrid>
</asp:Content>