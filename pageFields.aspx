<%@ Page Language="C#" Inherits="SS.Poll.Pages.PageFields" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <link href="assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/siteserver.min.css" rel="stylesheet" type="text/css" />
  </head>

  <body>

    <form runat="server">

      <header id="topnav">
        <div class="navbar-custom">
          <div class="container">

            <div id="navigation">
              <!-- Navigation Menu-->
              <ul class="navigation-menu">

                <li class="has-submenu">
                  <a href="<%=PageItemsUrl%>">投票项管理</a>
                </li>

                <li class="has-submenu active">
                  <a href="<%=PageFieldsUrl%>">
                    提交字段管理
                  </a>
                </li>
                <li class="has-submenu">
                  <a href="<%=PageSettingsUrl%>">
                    投票选项
                  </a>
                </li>
                <li class="has-submenu">
                  <a href="<%=PageResultsUrl%>">
                    查看投票结果
                  </a>
                </li>
                <li class="has-submenu">
                  <a href="<%=PageLogsUrl%>">
                    投票提交记录
                  </a>
                </li>
                <li class="has-submenu">
                  <a href="<%=ReturnUrl%>">
                    <i class="ion-ios-undo"></i>
                    返回列表
                  </a>
                </li>

              </ul>
              <!-- End navigation menu -->
            </div>

          </div>
        </div>
      </header>

      <div class="container-fluid">

        <div class="card-box m-t-20">
          <div class="header-title">提交表单管理</div>
          <p class="text-muted m-b-25 font-13">在此对表单字段进行增删改等操作</p>

          <asp:Literal id="LtlMessage" runat="server" />

          <asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead" CssClass="table m-0"
            gridlines="none" runat="server">
            <Columns>
              <asp:TemplateColumn HeaderText="字段名">
                <ItemTemplate>
                  <asp:Literal ID="ltlAttributeName" runat="server"></asp:Literal>
                </ItemTemplate>
              </asp:TemplateColumn>
              <asp:TemplateColumn HeaderText="显示名称">
                <ItemTemplate>
                  <asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal>
                </ItemTemplate>
              </asp:TemplateColumn>
              <asp:TemplateColumn HeaderText="表单提交类型">
                <ItemTemplate>
                  <asp:Literal ID="ltlFieldType" runat="server"></asp:Literal>
                </ItemTemplate>
                <ItemStyle Width="120" />
              </asp:TemplateColumn>
              <asp:TemplateColumn HeaderText="是否启用">
                <ItemTemplate>
                  <asp:Literal ID="ltlIsDisabled" runat="server"></asp:Literal>
                </ItemTemplate>
                <ItemStyle Width="100" />
              </asp:TemplateColumn>
              <asp:TemplateColumn HeaderText="验证规则">
                <ItemTemplate>
                  <asp:Literal ID="ltlValidate" runat="server"></asp:Literal>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="left" />
              </asp:TemplateColumn>
              <asp:TemplateColumn HeaderText="操作">
                <ItemTemplate>
                  <asp:Literal ID="ltlActions" runat="server"></asp:Literal>
                </ItemTemplate>
                <ItemStyle Width="300" />
              </asp:TemplateColumn>
            </Columns>
          </asp:dataGrid>

          <div class="m-b-25"></div>

          <asp:Button class="btn btn-primary" id="BtnAddField" Text="新增字段" runat="server" />
          <asp:Button class="btn" id="BtnImport" Text="导 入" runat="server" />
          <asp:Button class="btn" id="BtnExport" Text="导 出" runat="server" />

        </div>

      </div>

    </form>
  </body>

  </html>
  <script src="assets/js/jquery.min.js"></script>
  <script src="assets/layer/layer.min.js" type="text/javascript"></script>
