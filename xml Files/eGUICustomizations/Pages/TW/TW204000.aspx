<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW204000.aspx.cs" Inherits="Page_TW204000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="eGUICustomizations.Graph.TWNCreateNewGUIMaint" PrimaryView="Document">
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView SkinID="" ID="form" runat="server" DataSourceID="ds" DataMember="Document" Width="100%" Height="100px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule1" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector1" DataField="RequestID" CommitChanges="True" ></px:PXSelector>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask3" DataField="CustomerID" AllowEdit="True" CommitChanges="true" ></px:PXSegmentMask>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit4" DataField="NewGUIDate" ></px:PXDateTimeEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
<px:PXSplitContainer ID="SptCont1" runat="server" SkinID="Horizontal" SplitterPosition="300" Height="600px" Panel1MinSize="150" Panel2MinSize="150">
<AutoSize Container="Window" Enabled="true" MinHeight="300"></AutoSize>
<Template1>
	<px:PXGrid FilesIndicator="False" NoteIndicator="False" runat="server" ID="grdScanMaster" SyncPosition="True" Height="100%" SkinID="Primary" TabIndex="700" Width="100%" Caption="Invoice" CaptionVisible="True" ScrollBars="Always" DataSourceID="ds" AllowPaging="True" AdjustPageSize="Auto">
		<AutoSize Enabled="True" Container="Window" ></AutoSize>
		<Mode AllowAddNew="False" InitNewRow="True" ></Mode>
		<Levels>
			<px:PXGridLevel DataMember="GUIInvoice">
				<Columns>
					<px:PXGridColumn DataField="Selected" Width="60" Type="CheckBox" ></px:PXGridColumn>
					<px:PXGridColumn DataField="GUINbr" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="GUIFormatcode" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="GUIDirection" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="GUIDate" Width="90" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CustVend" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CustVendName" Width="280" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TaxNbr" Width="96" ></px:PXGridColumn>
					<px:PXGridColumn DataField="OurTaxNbr" Width="96" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TaxID" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TaxZoneID" Width="120" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TaxCategoryID" Width="120" ></px:PXGridColumn>
					<px:PXGridColumn DataField="OrderNbr" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TransDate" Width="90" ></px:PXGridColumn>
					<px:PXGridColumn DataField="NetAmount" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TaxAmount" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn DataField="GUITitle" Width="220" ></px:PXGridColumn>
					<px:PXGridColumn DataField="BatchNbr" Width="140" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels></px:PXGrid></Template1>
<Template2>
	<px:PXGrid NoteIndicator="False" FilesIndicator="False" runat="server" ID="grdScanDetail" SyncPosition="True" SkinID="Primary" Width="100%" Caption="Credit Note" CaptionVisible="True" ScrollBars="Always" DataSourceID="ds" AllowPaging="True" AdjustPageSize="Auto">
		<AutoSize Enabled="True" ></AutoSize>
		<Mode AllowAddNew="False" InitNewRow="True" ></Mode>
		<Levels>
			<px:PXGridLevel DataMember="GUICreditNote">
				<Columns>
					<px:PXGridColumn DataField="Selected" Width="60" Type="CheckBox" ></px:PXGridColumn>
					<px:PXGridColumn DataField="GUINbr" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="GUIFormatcode" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="GUIDirection" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="GUIDate" Width="90" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CustVend" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CustVendName" Width="280" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TaxNbr" Width="96" ></px:PXGridColumn>
					<px:PXGridColumn DataField="OurTaxNbr" Width="96" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TaxID" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TaxZoneID" Width="120" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TaxCategoryID" Width="120" ></px:PXGridColumn>
					<px:PXGridColumn DataField="OrderNbr" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="NetAmount" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TaxAmount" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn DataField="GUITitle" Width="220" ></px:PXGridColumn>
					<px:PXGridColumn DataField="BatchNbr" Width="140" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels></px:PXGrid></Template2>
</px:PXSplitContainer>
</asp:Content>