<%@ Page Language="C#" Inherits="SS.Poll.Pages.ModalFieldAdd" %>
  <!DOCTYPE html>
  <html class="bg-light">

  <head>
    <meta charset="utf-8">
    <link href="assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/siteserver.css" rel="stylesheet" type="text/css" />
  </head>

  <body class="bg-light">

    <form runat="server" class="m-3 p-3">

      <asp:Literal id="LtlMessage" runat="server" />

      <div class="form-group row">
        <label class="col-2 col-form-label">
          字段名称
          <asp:RequiredFieldValidator ControlToValidate="TbAttributeName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
          />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAttributeName" ValidationExpression="[a-zA-Z0-9_]+" ErrorMessage="字段名称只允许包含字母、数字以及下划线"
            foreColor="red" Display="Dynamic" />
        </label>
        <div class="col-4">
          <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbAttributeName" runat="server" />
        </div>
        <label class="col-2 col-form-label">
          显示名称
          <asp:RequiredFieldValidator ControlToValidate="TbDisplayName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
          />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDisplayName" ValidationExpression="[^']+" errorMessage=" *"
            foreColor="red" display="Dynamic" />
        </label>
        <div class="col-4">
          <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbDisplayName" runat="server" />
        </div>
      </div>

      <div class="form-group row">
        <label class="col-2 col-form-label">提示信息</label>
        <div class="col-10">
          <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbPlaceHolder" runat="server" />
        </div>
      </div>

      <div class="form-group row">
        <label class="col-2 col-form-label">是否启用</label>
        <div class="col-10">
          <asp:DropDownList class="form-control" ID="DdlIsDisabled" runat="server">
            <asp:ListItem Text="启用" Value="False" Selected="True" />
            <asp:ListItem Text="禁用" Value="True" />
          </asp:DropDownList>
        </div>
      </div>

      <div class="form-group row">
        <label class="col-2 col-form-label">提交类型</label>
        <div class="col-10">
          <asp:DropDownList class="form-control" ID="DdlFieldType" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server"></asp:DropDownList>
        </div>
      </div>

      <asp:PlaceHolder ID="PhItemsType" runat="server">

        <div class="form-group row">
          <label class="col-2 col-form-label">设置选项</label>
          <div class="col-10">
            <asp:DropDownList class="form-control" ID="DdlItemType" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server">
              <asp:ListItem Text="快速设置" Value="True" Selected="True" />
              <asp:ListItem Text="详细设置" Value="False" />
            </asp:DropDownList>
          </div>
        </div>

        <asp:PlaceHolder ID="PhItemCount" runat="server">
          <div class="form-group row">
            <label class="col-2 col-form-label">
              共有选项
              <asp:RequiredFieldValidator ControlToValidate="TbItemCount" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator ControlToValidate="TbItemCount" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="此项必须为数字"
                foreColor="red" runat="server" />
            </label>
            <div class="col-7">
              <asp:TextBox class="form-control" id="TbItemCount" runat="server" />
            </div>
            <div class="col-3">

              <asp:Button class="btn" style="margin-bottom:0px;" id="SetCount" text="设 置" onclick="SetCount_OnClick" CausesValidation="false"
                runat="server" />

            </div>
          </div>
        </asp:PlaceHolder>

      </asp:PlaceHolder>

      <asp:PlaceHolder ID="PhItemsRapid" runat="server">
        <div class="form-group row">
          <label class="col-2 col-form-label">
            可选值
            <asp:RequiredFieldValidator ControlToValidate="TbItemValues" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
            />
          </label>
          <div class="col-10">
            <asp:TextBox TextMode="MultiLine" class="form-control" Columns="60" id="TbItemValues" runat="server" />
            <span class="help-block">英文","分隔，如：“选项1,选项2”</span>
          </div>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder ID="PhItems" runat="server">
        <div class="form-group row">
          <label class="col-2 col-form-label">可选值</label>
          <div class="col-md-10">
            <asp:Repeater ID="RptItems" runat="server">
              <itemtemplate>
                <table width="100%" border="0" cellspacing="2" cellpadding="2">
                  <tr>
                    <td class="center" style="width:20px;">
                      <strong>
                        <%# Container.ItemIndex + 1 %>
                      </strong>
                    </td>
                    <td>
                      <asp:TextBox ID="TbValue" class="form-control" Columns="40" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Value") %>'></asp:TextBox>
                      <asp:RequiredFieldValidator ControlToValidate="TbValue" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                      />
                    </td>
                    <td>
                      <asp:CheckBox ID="CbIsSelected" runat="server" Checked="False" Text="选定"></asp:CheckBox>
                    </td>
                  </tr>
                </table>
              </itemtemplate>
            </asp:Repeater>
          </div>
        </div>
      </asp:PlaceHolder>

      <hr />

      <div class="text-right mr-1">
        <asp:Button class="btn btn-primary" onclick="Add_OnClick" runat="server" Text="保 存"></asp:Button>
        <button type="button" class="btn btn-default" onClick="window.parent.layer.closeAll();return false;">取 消</button>
      </div>

    </form>
  </body>

  </html>
  <script src="assets/js/jquery.min.js"></script>
  <script src="assets/layer/layer.min.js" type="text/javascript"></script>