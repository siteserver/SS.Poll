<%@ Page Language="C#" Inherits="SS.Poll.Pages.PageResults" %>
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
        <div class="navbar-custom active">
          <div class="container">

            <div id="navigation" class="active">
              <!-- Navigation Menu-->
              <ul class="navigation-menu">

                <li class="has-submenu">
                  <a href="<%=PageItemsUrl%>">投票项管理</a>
                </li>

                <li class="has-submenu">
                  <a href="<%=PageFieldsUrl%>">
                    提交字段管理
                  </a>
                </li>
                <li class="has-submenu">
                  <a href="<%=PageSettingsUrl%>">
                    投票选项
                  </a>
                </li>
                <li class="has-submenu active">
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


      <!-- container start -->
      <div class="container-fluid">

        <div class="card-box m-t-20">
          <h4 class="text-dark  header-title m-t-0">查看投票结果</h4>
          <p class="text-muted m-b-25 font-13"></p>
          <asp:Literal id="LtlMessage" runat="server" />

          <asp:repeater id="RptItems" runat="server">
            <itemtemplate>
              <div class="card-box widget-user">
                <div>
                  <asp:Literal ID="ltlImage" runat="server"></asp:Literal>
                  <div class="wid-u-info">
                    <h4 class="m-t-0 m-b-5">
                      <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
                    </h4>
                    <p class="text-muted m-b-5 font-13">
                      <asp:Literal ID="ltlSubTitle" runat="server"></asp:Literal>
                    </p>
                    <small class="text-warning">
                      <b>
                        <asp:Literal ID="ltlSummary" runat="server"></asp:Literal>
                      </b>
                    </small>
                    <div class="progress" style=" margin: 5px 0;">
                      <asp:Literal ID="ltlProgress" runat="server"></asp:Literal>
                    </div>
                  </div>
                </div>
              </div>
            </itemtemplate>
          </asp:repeater>

          <div class="m-b-25"></div>

          <asp:Button class="btn btn-success" id="BtnExport" onclick="BtnExport_Click" Text="导 出" runat="server" />

        </div>

      </div>
      <!-- container end -->

      <asp:Literal id="LtlScript" runat="server"></asp:Literal>
    </form>
  </body>

  </html>
  <script src="assets/js/jquery.min.js"></script>
  <script src="assets/layer/layer.min.js" type="text/javascript"></script>