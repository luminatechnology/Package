<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW503010.aspx.cs" Inherits="Page_TW503010" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="eGUICustomizations.Graph.TWNExpOnlineStrGUIInv" PrimaryView="GUITranProc">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Inquire" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="GUITranProc">
			    <Columns>
				<px:PXGridColumn DataField="Selected" Width="30" Type="CheckBox" AllowCheckAll="True" TextAlign="Center" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GUIStatus" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GUIFormatcode" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GUINbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GUIDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TransDate" Width="90" />
				<px:PXGridColumn DataField="OurTaxNbr" Width="96" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxNbr" Width="96" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NetAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VATType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustVend" Width="140" />
				<px:PXGridColumn DataField="OrderNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BatchNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CarrierType" Width="72" />
				<px:PXGridColumn DataField="CarrierID" Width="220" />
				<px:PXGridColumn DataField="NPONbr" Width="120" />
				<px:PXGridColumn DataField="EGUIExported" Type="CheckBox" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EGUIExportedDateTime" Width="90" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>