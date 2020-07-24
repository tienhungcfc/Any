<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="2.aspx.cs" Inherits="cboCMS.Web._2" %>

<script runat="server">
    Exception ex = null;

    cboCMS.Core.JsonNet jn = new cboCMS.Core.JsonNet();

    protected void Page_Load(object sender, EventArgs e)
    {

        //var s = cboCMS.Core.Common.Authentication.EncodePassword("ceciliaspa", "admin");
        //
        try
        {
            dynamic a = new Any();
            a = Any.FromJsonString(cboCMS.Web.WebControl.ReadFromTxt("/Upload/1.json", ""));
            a.orders[0].items[1].title = "hhh";
            a.orders[0].items.Push(Any.FromJsonString(@"{""id"":103}"));
            a.orders.Push(Any.FromJsonString(@"{id:200, items:[]}"));
            a.orders[1].items.Push(Any.FromJsonString("{id:201, qty:909}"));
            RenderMess = jn.SerializeObject(Any.Export(Any.Select((Any)a, "orders.items", (Any x, int index)=> {
                dynamic z = x;
                z.hh = 1000 + (index + 1);
                z.ee = "" + index;
                z.ff = 1 != 0;
                return 0;
            })));

        }
        catch (Exception er)
        {
            ex = er;
            RenderMess = jn.SerializeObject(new
            {
                error = ex
            });
        }

        Response.ContentType = "text/json";

    }





</script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Any</title>
    <meta charset="utf-8" />
</head>
<body>
    

</body>
</html>
