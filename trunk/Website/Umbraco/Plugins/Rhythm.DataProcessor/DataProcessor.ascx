<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DataProcessor.ascx.cs" Inherits="Rhythm.DataProcessor.DataProcessorDashboard" %>
<%@ Import Namespace="Rhythm.DataProcessor" %>
<%@ Register TagPrefix="u" Namespace="umbraco.editorControls" Assembly="umbraco.editorControls" %>

<%-- Data processors drop down.  --%>
<asp:Label runat="server" AssociatedControlID="ddlProcessors" Text="Choose a Data Processor" />
<asp:DropDownList runat="server" ID="ddlProcessors" AutoPostBack="true"
    OnSelectedIndexChanged="ddlProcessors_Change" />

<%-- Once a processor has been selected, show the input fields. --%>
<% if(ddlProcessors.SelectedIndex > 0) { %>
    <h2><%= ddlProcessors.SelectedValue %></h2>

    <%-- Render each input. --%>
    <asp:Repeater runat="server" ID="rpInputs">
        <ItemTemplate>
            <h3><%# (Container.DataItem as ProcessorInput).Label %></h3>

            <%-- Single node. --%>
            <asp:PlaceHolder runat="server" ID="phNode"
                Visible="<%# (Container.DataItem as ProcessorInput).Kind == ProcessorKinds.Node %>">
                <u:MultiNodeTreePicker.MNTP_DataEditor runat="server" StartNodeId="-1"
                    StartNodeSelectionType="Picker" ControlHeight="200" MaxNodeCount="1"
                    MinNodeCount="1" ID="inputNode" />
            </asp:PlaceHolder>

            <%-- Multiple nodes. --%>
            <asp:PlaceHolder runat="server" Visible="<%# (Container.DataItem as ProcessorInput).Kind == ProcessorKinds.Nodes %>" ID="phNodes">
                <u:MultiNodeTreePicker.MNTP_DataEditor runat="server" StartNodeId="-1"
                    StartNodeSelectionType="Picker" ControlHeight="200" MaxNodeCount="-1"
                    MinNodeCount="1" ID="inputNodes" />
            </asp:PlaceHolder>

        </ItemTemplate>
        <SeparatorTemplate>
            <br />
        </SeparatorTemplate>
    </asp:Repeater>
    <asp:PlaceHolder runat="server" ID="phInputs" />
    <br />

    <%-- Button to begin processing data. --%>
    <asp:Button runat="server" ID="btnProcess" Text="Process" OnClick="btnProcess_Click" />

<% } %>
<br />

<%-- Show results. --%>
<asp:Literal runat="server" ID="litResults" />