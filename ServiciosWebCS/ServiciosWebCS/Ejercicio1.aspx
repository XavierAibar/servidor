<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Ejercicio1.aspx.cs" Inherits="ServiciosWebCS.Ejercicio1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="Estilos.css" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="contenido">
            <h2>CONSUMIR UN SERVICIO WEB YA EXISTENTE</h2>
            <h1>Titulaciones Oficiales en la Universidad de Alicante</h1>
            <br />
            <br />
            <p>
                Curso académico (formato aaaa-aa)&nbsp;&nbsp;
                <asp:TextBox ID="txtCurso" runat="server"></asp:TextBox>
                &nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnObtenerInformacion" runat="server" Text="Obtener Informacion" OnClick="btnObtenerInformacion_Click" />
            </p>
        </div>
        <div id="res">
            <asp:Label ID="lblResultado" runat="server"></asp:Label>
            <br />
            <asp:GridView ID="GridView1" runat="server">
        </asp:GridView>
        </div>
        
    </form>
</body>
</html>
