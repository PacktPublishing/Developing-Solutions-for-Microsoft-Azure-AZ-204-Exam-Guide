<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TheCloudShopWebState._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


        <div>
            <table>
                <tr>
                    <td>Set Session Variable</td>
                    <td>
                        <asp:TextBox ID="txtinput" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:Button ID="btnPut" runat="server" Text="SET" OnClick="btnPut_Click" /></td>
                </tr>
                <tr>
                    <td>Retrieve Session Variable</td>
                    <td>
                        <asp:TextBox ID="txtoutput" ReadOnly="true" Enabled="false" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:Button ID="btnGet" runat="server" Text="GET" OnClick="btnGet_Click" /></td>
                </tr>
            </table>
        </div>


</asp:Content>
