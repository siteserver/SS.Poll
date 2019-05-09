var $api = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/" +
    utils.getQueryString("pluginId") +
    "/pages/fields/",
  params: {
    siteId: utils.getQueryInt('siteId'),
    channelId: utils.getQueryInt('channelId'),
    contentId: utils.getQueryInt('contentId'),
    pollId: utils.getQueryInt('pollId')
  },
  withCredentials: true
});

var data = {
  apiUrl: utils.getQueryString('apiUrl'),
  siteId: utils.getQueryString('siteId'),
  channelId: utils.getQueryString('channelId'),
  contentId: utils.getQueryString('contentId'),
  pollId: utils.getQueryString('pollId'),
  returnUrl: utils.getQueryString('returnUrl'),
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  items: null,
  uploadUrl: null,
  files: []
};

var methods = {
  getList: function () {
    var $this = this;

    $api.get('').then(function (response) {
      var res = response.data;

      $this.items = res.value;
      $this.uploadUrl = $api.defaults.baseURL + '/actions/import?adminToken=' + res.adminToken + '&siteId=' + this.siteId + '&pollId=' + this.pollId;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  delete: function (fieldId) {
    var $this = this;

    utils.loading(true);
    $api.delete('', {
      params: {
        fieldId: fieldId
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  inputFile(newFile, oldFile) {
    if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
      if (!this.$refs.import.active) {
        this.$refs.import.active = true
      }
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      swal2({
        title: '字段导入成功',
        type: 'success',
        confirmButtonText: '确 定',
        confirmButtonClass: 'btn btn-primary',
      }).then(function (result) {
        if (result.value) {
          location.reload(true);
        }
      });

    }
  },

  inputFilter: function (newFile, oldFile, prevent) {
    if (newFile && !oldFile) {
      if (!/\.(zip)$/i.test(newFile.name)) {
        swal2({
          title: '上传格式错误！',
          text: '请上传zip压缩包',
          type: 'error',
          confirmButtonText: '确 定',
          confirmButtonClass: 'btn btn-primary',
        });
        return prevent()
      }
    }
  },

  btnEditClick: function (fieldId) {
    utils.openLayer({
      title: '编辑字段',
      url: utils.getPageUrl('fieldsLayerModel.html') + '&pollId=' + this.pollId + '&fieldId=' + fieldId
    });
  },

  btnValidateClick: function (fieldId) {
    utils.openLayer({
      title: '设置验证规则',
      url: utils.getPageUrl('fieldsLayerValidate.html') + '&pollId=' + this.pollId + '&fieldId=' + fieldId
    });
  },

  btnAddClick: function () {
    utils.openLayer({
      title: '新增字段',
      url: utils.getPageUrl('fieldsLayerModel.html') + '&pollId=' + this.pollId
    });
  },

  btnExportClick: function () {
    var $this = this;

    utils.loading(true);
    $api.post('actions/export').then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnDeleteClick: function (title, fieldId) {
    var $this = this;

    utils.alertDelete({
      title: '删除字段',
      text: '此操作将删除字段 ' + title + '，确定吗？',
      callback: function () {
        $this.delete(fieldId);
      }
    });
  },

  btnNavClick: function (pageName) {
    utils.loading(true);
    location.href = utils.getPageUrl(pageName) + '&pollId=' + this.pollId + '&returnUrl=' + encodeURIComponent(this.returnUrl);
  },

  btnReturnClick: function() {
    utils.loading(true);
    location.href = this.returnUrl;
  },
};

new Vue({
  el: '#main',
  data: data,
  components: {
    FileUpload: VueUploadComponent
  },
  methods: methods,
  created: function () {
    this.getList();
  }
});
