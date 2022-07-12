<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW505000.aspx.cs" Inherits="Page_TW505000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="eGUICustomizations.Graph.TWNGenWHTFile" PrimaryView="Filter">
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="70px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule2" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule6" Merge="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit3" CommitChanges="True" DataField="FromDate_Date" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit5" TimeMode="True" SuppressLabel="True" CommitChanges="True" DataField="FromDate_Time" ></px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule7" Merge="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit9" CommitChanges="True" DataField="ToDate_Date" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit10" TimeMode="True" SuppressLabel="True" CommitChanges="True" DataField="ToDate_Time" ></px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="PXDateTimeEdit1" CommitChanges="True" DataField="FromPaymDate" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="PXDateTimeEdit2" CommitChanges="True" DataField="ToPaymDate" ></px:PXDateTimeEdit>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Inquire" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="WHTTranProc">
			    <Columns>
				<px:PXGridColumn DataField="Selected" Width="30" Type="CheckBox" AllowCheckAll="True" TextAlign="Center" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BatchNbr" Width="140" />
				<px:PXGridColumn DataField="TranDate" Width="90" />
				<px:PXGridColumn DataField="PaymDate" Width="90" />
				<px:PXGridColumn DataField="PersonalID" Width="120" />
				<px:PXGridColumn DataField="PropertyID" Width="140" />
				<px:PXGridColumn DataField="TypeOfIn" Width="70" />
				<px:PXGridColumn DataField="WHTFmtCode" Width="70" />
				<px:PXGridColumn DataField="WHTFmtSub" Width="70" />
				<px:PXGridColumn DataField="PayeeName" Width="140" />
				<px:PXGridColumn DataField="PayeeAddr" Width="220" />
				<px:PXGridColumn DataField="SecNHICode" Width="70" />
				<px:PXGridColumn DataField="SecNHIPct" Width="100" />
				<px:PXGridColumn DataField="SecNHIAmt" Width="100" />
				<px:PXGridColumn DataField="WHTTaxPct" Width="100" />
				<px:PXGridColumn DataField="WHTAmt" Width="100" /></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>