var $api = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/" +
    utils.getQueryString("pluginId") +
    "/pages/templates/",
  params: {
    siteId: utils.getQueryInt('siteId')
  },
  withCredentials: true
});

var data = {
  siteId: utils.getQueryString('siteId'),
  pollId: utils.getQueryString('pollId'),
  apiUrl: utils.getQueryString('apiUrl'),
  type: utils.getQueryString('type'),
  pageLoad: false,
  pageConfig: null,
  pageAlert: {
    type: 'primary',
    html: '投票标签：<mark>&lt;stl:poll name="投票名称" type="模板文件夹"&gt;&lt;/stl:poll&gt;</mark>，如果希望自定义模板样式，可以点击代码编辑按钮然后修改模板代码。'
  },
  templateInfoList: null,
  name: null,
  templateHtml: null,
};

var methods = {
  getIconUrl: function (templateInfo) {
    return '../templates/' + templateInfo.name + '/' + templateInfo.icon;
  },

  loadTemplates: function () {
    var $this = this;

    if (this.pageLoad) {
      utils.loading(true);
    }

    $api.get('', {
      params: {
        type: this.type
      }
    }).then(function (response) {
      var res = response.data;

      $this.templateInfoList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
      $this.pageLoad = true;
    });
  },

  btnEditClick: function (name) {
    var url = 'templatesLayerEdit.html?siteId=' + this.siteId + '&type=' + this.type + '&name=' + name + '&apiUrl=' + encodeURIComponent(this.apiUrl);
    utils.openLayer({
      title: '模板设置',
      url: url
    });
  },

  btnHtmlClick: function (templateInfo) {
    utils.loading(true);
    var url = 'templateHtml.html?siteId=' + this.siteId + '&apiUrl=' + encodeURIComponent(this.apiUrl) + '&type=' + this.type + '&name=' + templateInfo.name;
    location.href = url;
  },

  btnPreviewClick: function (name) {
    var $this = this;
    utils.openLayer({
      title: '预览模板',
      url: 'templatesLayerPreview.html?siteId=' + $this.siteId + '&name=' + name + '&apiUrl=' + encodeURIComponent($this.apiUrl)
    });
  },

  btnDeleteClick: function (template) {
    var $this = this;
    utils.alertDelete({
      title: '删除模板',
      text: '此操作将删除模板' + template.name + '，确认吗？',
      callback: function () {
        utils.loading(true);
        $api.delete('', {
          params: {
            type: $this.type,
            name: template.name
          }
        }).then(function (response) {
          var res = response.data;

          $this.templateInfoList = res.value;
        }).catch(function (error) {
          $this.pageAlert = utils.getPageAlert(error);
        }).then(function () {
          utils.loading(false);
        });
      }
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    utils.loading(true);
    $api.post('', {
      name: this.name,
      templateHtml: this.templateHtml
    }).then(function (response) {
      var res = response.data;

      swal({
        toast: true,
        type: 'success',
        title: "模板编辑成功！",
        showConfirmButton: false,
        timer: 2000
      }).then(function () {
        $this.pageType = 'list';
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnNavClick: function(type) {
    utils.loading(true);
    location.href = utils.getPageUrl('templates.html') + '&type=' + type;
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.loadTemplates();
  }
});