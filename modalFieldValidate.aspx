<%@ Page Language="C#" Inherits="SS.Poll.Pages.ModalFieldValidate" %>
  <!DOCTYPE html>
  <html class="bg-light">

  <head>
    <meta charset="utf-8">
    <link href="assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/siteserver.min.css" rel="stylesheet" type="text/css" />
  </head>

  <body class="bg-light">

    <form runat="server" class="m-3 p-3">

      <asp:Literal id="LtlMessage" runat="server" />

      <div class="form-group row">
        <label class="col-2 col-form-label">是否启用表单验证</label>
        <div class="col-10">
          <asp:DropDownList id="DdlIsValidate" class="form-control" OnSelectedIndexChanged="Validate_SelectedIndexChanged" AutoPostBack="true"
            runat="server">
            <asp:ListItem Value="True" Text="启用" />
            <asp:ListItem Value="False" Text="不启用" Selected="True" />
          </asp:DropDownList>
        </div>
      </div>

      <asp:PlaceHolder ID="PhValidate" runat="server">
        <div class="form-group row">
          <label class="col-2 col-form-label">是否为必填项</label>
          <div class="col-10">
            <asp:DropDownList id="DdlIsRequired" class="form-control" runat="server">
              <asp:ListItem Value="True" Text="是" />
              <asp:ListItem Value="False" Text="否" Selected="True" />
            </asp:DropDownList>
          </div>
        </div>
        <asp:PlaceHolder ID="PhNum" runat="server">
          <div class="form-group row">
            <label class="col-2 col-form-label">最小字符数</label>
            <div class="col-8">
              <asp:TextBox class="form-control" MaxLength="50" Text="0" id="TbMinNum" runat="server" />
            </div>
            <div class="col-2">
              个字符
              <asp:RegularExpressionValidator ControlToValidate="TbMinNum" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                foreColor="red" runat="server" /> （0代表不限制）
            </div>
          </div>
          <div class="form-group row">
            <label class="col-2 col-form-label">最大字符数</label>
            <div class="col-8">
              <asp:TextBox class="form-control" MaxLength="50" Text="0" id="TbMaxNum" runat="server" />
            </div>
            <div class="col-2">
              个字符
              <asp:RegularExpressionValidator ControlToValidate="TbMaxNum" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                foreColor="red" runat="server" /> （0代表不限制）
            </div>
          </div>
        </asp:PlaceHolder>
        <div class="form-group row">
          <label class="col-2 col-form-label">高级验证</label>
          <div class="col-10">
            <asp:DropDownList ID="DdlValidateType" class="form-control" OnSelectedIndexChanged="Validate_SelectedIndexChanged" AutoPostBack="true"
              runat="server"></asp:DropDownList>
          </div>
        </div>
        <div class="form-group row">
          <label class="col-2 col-form-label">验证失败提示信息</label>
          <div class="col-6">
            <asp:TextBox Columns="60" class="form-control" TextMode="MultiLine" id="TbErrorMessage" runat="server" />
          </div>
          <div class="col-4">
            不设置系统将使用默认提示
          </div>
        </div>
      </asp:PlaceHolder>

      <hr />

      <div class="text-right mr-1">
        <asp:Button class="btn btn-primary" onclick="BtnValidate_OnClick" runat="server" Text="保 存"></asp:Button>
        <button type="button" class="btn btn-default" onClick="window.parent.layer.closeAll();return false;">取 消</button>
      </div>

    </form>
  </body>

  </html>
  <script src="assets/js/jquery.min.js"></script>
  <script src="assets/layer/layer.min.js" type="text/javascript"></script>