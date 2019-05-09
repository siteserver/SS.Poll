var $api = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/" +
    utils.getQueryString("pluginId") +
    "/pages/polls/",
  params: {
    siteId: utils.getQueryInt('siteId')
  },
  withCredentials: true
});

var $urlActionsUp = '/pages/polls/actions/up';
var $urlActionsDown = '/pages/polls/actions/down';
var $urlExport = '/pages/polls/actions/export';
var $urlImport = '/pages/polls/actions/import';

var data = {
  siteId: utils.getQueryInt('siteId'),
  pageLoad: false,
  pageAlert: null,
  pageType: 'list',
  pollInfoList: null,
  pollInfo: null,
  uploadUrl: null,
  files: []
};

var methods = {
  getList: function () {
    var $this = this;

    $api.get('').then(function (response) {
      var res = response.data;

      $this.pollInfoList = res.value;
      $this.uploadUrl = $api.defaults.baseURL + '/actions/import?adminToken=' + res.adminToken + '&siteId=' + this.siteId;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  delete: function (pollId) {
    var $this = this;

    utils.loading(true);
    $api.delete('', {
      params: {
        pollId: pollId
      }
    }).then(function (response) {
      var res = response.data;

      swal2({
        toast: true,
        type: 'success',
        title: "表单删除成功",
        showConfirmButton: false,
        timer: 2000
      });
      $this.pollInfoList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnViewClick: function (pollId) {
    location.href = 'logs.html?siteId=' + $siteId + '&pollId=' + pollId + '&apiUrl=' + encodeURIComponent($apiUrl) + '&returnUrl=' + encodeURIComponent(location.href);
  },

  btnUpClick: function (pollInfo) {
    var $this = this;

    utils.loading(true);
    $api.post($urlActionsUp, {
      siteId: $siteId,
      pollId: pollInfo.id
    }).then(function (response) {
      var res = response.data;

      swal2({
        toast: true,
        type: 'success',
        title: "表单排序成功",
        showConfirmButton: false,
        timer: 2000
      });
      $this.pollInfoList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnDownClick: function (pollInfo) {
    var $this = this;

    utils.loading(true);
    $api.post($urlActionsDown, {
      siteId: $siteId,
      pollId: pollInfo.id
    }).then(function (response) {
      var res = response.data;

      swal2({
        toast: true,
        type: 'success',
        title: "表单排序成功",
        showConfirmButton: false,
        timer: 2000
      });
      $this.pollInfoList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnEditClick: function (pollInfo) {
    this.pageType = 'edit';
    this.pollInfo = pollInfo;
  },

  btnAddClick: function () {
    this.pageType = 'add';
    this.pollInfo = {
      title: '',
      description: ''
    };
  },

  btnDeleteClick: function (pollInfo) {
    var $this = this;

    utils.alertDelete({
      title: '删除表单',
      text: '此操作将删除表单，确定吗？',
      callback: function () {
        $this.delete(pollInfo.id);
      }
    });
  },

  btnSubmitClick: function () {
    var $this = this;

    utils.loading(true);
    if (this.pollInfo.id) {
      $api.put('', {
        pollId: this.pollInfo.id,
        title: this.pollInfo.title,
        description: this.pollInfo.description
      }).then(function (response) {
        var res = response.data;

        swal2({
          toast: true,
          type: 'success',
          title: "表单修改成功",
          showConfirmButton: false,
          timer: 2000
        });
        $this.pollInfoList = res.value;
        $this.pageType = 'list';
      }).catch(function (error) {
        $this.pageAlert = utils.getPageAlert(error);
      }).then(function () {
        utils.loading(false);
      });
    } else {
      $api.post('', {
        title: this.pollInfo.title,
        description: this.pollInfo.description
      }).then(function (response) {
        var res = response.data;

        swal2({
          toast: true,
          type: 'success',
          title: "表单添加成功",
          showConfirmButton: false,
          timer: 2000
        });
        $this.pollInfoList = res.value;
        $this.pageType = 'list';
      }).catch(function (error) {
        $this.pageAlert = utils.getPageAlert(error);
      }).then(function () {
        utils.loading(false);
      });
    }
  },

  inputFile(newFile, oldFile) {
    if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
      if (!this.$refs.import.active) {
        this.$refs.import.active = true
      }
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      swal2({
        title: '表单导入成功',
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

  btnExportClick: function (pollInfo) {
    var $this = this;

    utils.loading(true);
    $api.post('/actions/export', {
      pollId: pollInfo.id
    }).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  }
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