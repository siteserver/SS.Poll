<%@ Page Language="C#" Inherits="SS.Poll.Pages.ModalItemAdd" %>
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
        <label class="col-2 col-form-label">
          标题
          <asp:RequiredFieldValidator ControlToValidate="TbTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
          />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTitle" ValidationExpression="[a-zA-Z0-9_]+" ErrorMessage="字段名称只允许包含字母、数字以及下划线"
            foreColor="red" Display="Dynamic" />
        </label>
        <div class="col-10">
          <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbTitle" runat="server" />
        </div>
      </div>

      <div class="form-group row">
        <label class="col-2 col-form-label">
          副标题
          <asp:RequiredFieldValidator ControlToValidate="TbSubTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
          />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSubTitle" ValidationExpression="[^']+" errorMessage=" *"
            foreColor="red" display="Dynamic" />
        </label>
        <div class="col-10">
          <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbSubTitle" runat="server" />
        </div>
      </div>

      <asp:PlaceHolder id="PhImageUrl" runat="server">
        <div class="form-group row">
          <label class="col-2 col-form-label">
            图片
          </label>
          <div class="col-10">
            <asp:Literal ID="LtlImageUrl" runat="server"></asp:Literal>
            <asp:HiddenField ID="HfImageUrl" runat="server"></asp:HiddenField>
            <div id="upload" class="btn btn-success">选 择</div>
            <span id="upload_txt" style="clear: both; font-size: 12px; color: #FF3737;"></span>
          </div>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhLinkUrl" runat="server">
        <div class="form-group row">
          <label class="col-2 col-form-label">
            链接地址
            <asp:RequiredFieldValidator ControlToValidate="TbLinkUrl" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
            />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbLinkUrl" ValidationExpression="[^']+" errorMessage=" *"
              foreColor="red" display="Dynamic" />
          </label>
          <div class="col-10">
            <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbLinkUrl" runat="server" />
          </div>
        </div>
      </asp:PlaceHolder>

      <div class="form-group row">
        <label class="col-2 col-form-label">
          票数
          <asp:RequiredFieldValidator ControlToValidate="TbCount" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
          />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbCount" ValidationExpression="[^']+" errorMessage=" *"
            foreColor="red" display="Dynamic" />
        </label>
        <div class="col-10">
          <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbCount" runat="server" />
        </div>
      </div>

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
  <script src="assets/js/ajaxUpload.js"></script>
  <script type="text/javascript" language="javascript">
    $(document).ready(function () {
      new AjaxUpload('upload', {
        action: "<%=UploadUrl%>",
        name: "Filedata",
        data: {},
        onSubmit: function (file, ext) {
          var reg = /^(gif|jpg|jpeg|png)$/i;
          if (ext && reg.test(ext)) {
            $('#upload_txt').text('上传中... ');
          } else {
            $('#upload_txt').text('系统不允许上传指定的格式');
            return false;
          }
        },
        onComplete: function (file, response) {
          console.log(response);
          if (response) {
            response = JSON.parse(response)
            if (response.success === 'true') {
              $('#upload_txt').text('');
              $('#imageUrl').attr('src', response.imageUrl + '?v=' + Math.random());
              $('#HfImageUrl').val(response.imageUrl);
            } else {
              $('#upload_txt').text(response.message);
            }
          }
        }
      });
    });
  </script>