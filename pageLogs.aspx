<%@ Page Language="C#" Inherits="SS.Poll.Pages.PageLogs" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <link href="assets/plugin-utils/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/plugin-utils.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/ionicons.min.css" rel="stylesheet" type="text/css" />
  </head>

  <body>

    <form runat="server">

      <header id="topnav">
        <div class="navbar-custom">
          <div class="container">
            <div id="navigation">
              <ul class="navigation-menu">
                <li class="has-submenu">
                  <a href="<%=UrlPageResults%>">
                    <i class="ion-compose"></i>
                    查看投票结果
                  </a>
                </li>
                <li class="has-submenu active">
                  <a href="javascript:;">
                    <i class="ion-compose"></i>
                    投票提交记录
                  </a>
                </li>
                <li class="has-submenu">
                  <a href="<%=UrlReturn%>">
                    <i class="ion-ios-undo"></i>
                    返回列表
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </header>


      <!-- container start -->
      <div class="container" style="margin-top: 70px;">
        <div class="m-b-25"></div>
        <div class="row">
          <div class="col-sm-12">
            <div class="card-box">
              <h4 class="text-dark  header-title m-t-0">投票提交记录</h4>
              <p class="text-muted m-b-25 font-13"></p>
              <asp:Literal id="LtlMessage" runat="server" />

              <table class="tablesaw m-t-20 table m-b-0 tablesaw-stack">
                <thead>
                  <tr>
                    <asp:Literal id="LtlFieldNames" runat="server" />
                    <th scope="col">提交时间</th>
                  </tr>
                </thead>
                <tbody>
                  <asp:repeater id="RptLogs" runat="server">
                    <itemtemplate>
                      <tr>
                        <asp:Literal id="ltlValues" runat="server" />
                        <td>
                          <asp:Literal id="ltlAddDate" runat="server" />
                        </td>
                      </tr>
                    </itemtemplate>
                  </asp:repeater>
                </tbody>
              </table>

              <div class="m-b-25"></div>

              <asp:Button class="btn btn-success" id="BtnExport" onclick="BtnExport_Click" Text="导 出" runat="server" />

            </div>
          </div>
        </div>
      </div>
      <!-- container end -->

      <asp:Literal id="LtlScript" runat="server"></asp:Literal>
    </form>
  </body>

  </html>
  <script src="assets/plugin-utils/js/jquery.min.js"></script>
  <script src="assets/plugin-utils/js/bootstrap.min.js"></script>