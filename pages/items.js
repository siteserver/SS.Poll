var $api = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/" +
    utils.getQueryString("pluginId") +
    "/pages/items/",
  params: {
    siteId: utils.getQueryInt('siteId'),
    channelId: utils.getQueryInt('channelId'),
    contentId: utils.getQueryInt('contentId'),
    pollId: utils.getQueryInt('pollId')
  },
  withCredentials: true
});

var data = {
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  contentId: utils.getQueryInt('contentId'),
  pollId: utils.getQueryInt('pollId'),
  returnUrl: utils.getQueryString('returnUrl'),
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  items: null,
  totalCount: null,
  pollInfo: null,
  itemId: null,
  title: null,
  subTitle: null,
  imageUrl: null,
  linkUrl: null,
  count: null,
  uploadUrl: null,
  files: []
};

var methods = {
  inputFile: function(newFile, oldFile) {
    if (newFile && oldFile) {
      if (newFile.progress !== oldFile.progress) {
        utils.loading(true);
      }
      if (newFile.error && !oldFile.error) {
        utils.loading(false);
      }
      if (newFile.success && !oldFile.success) {
        utils.loading(false);
      }
    }

    if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
      if (!this.$refs.upload.active) {
        this.$refs.upload.active = true
      }
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      this.imageUrl = newFile.response.value;
    }
  },

  inputFilter: function (newFile, oldFile, prevent) {
    if (newFile && !oldFile) {
      if (!/\.(gif|jpg|jpeg|png|webp)$/i.test(newFile.name)) {
        swal2({
          title: '上传格式错误！',
          text: '请上传图片',
          type: 'error',
          confirmButtonText: '确 定',
          confirmButtonClass: 'btn btn-primary',
        });
        return prevent()
      }
    }
  },

  getPercentage: function(item) {
    if (this.totalCount == 0) return '0.00';
    return (item.count / this.totalCount).toFixed(2);
  },

  apiGetItems: function() {
    var $this = this;

    $api
      .get("")
      .then(function(response) {
        var res = response.data;
        
        $this.items = res.value;
        $this.totalCount = res.totalCount;
        $this.pollInfo = res.pollInfo;
        $this.uploadUrl = $api.defaults.baseURL + 'actions/upload?adminToken=' + res.adminToken + '&siteId=' + $this.siteId;

        $this.pageType = "list";
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        utils.loading(false);
        $this.pageLoad = true;
      });
  },

  apiInsert: function() {
    var $this = this;

    utils.loading(true);
    $api
      .post("", {
        title: this.title,
        subTitle: this.subTitle,
        imageUrl: this.imageUrl,
        linkUrl: this.linkUrl,
        count: this.count,
      })
      .then(function(response) {
        var res = response.data;
        swal2.success("投票项添加成功！");
        $this.apiGetItems();
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        utils.loading(false);
      });
  },

  apiUpdate: function() {
    var $this = this;

    utils.loading(true);
    $api
      .put(this.itemId + '', {
        title: this.title,
        subTitle: this.subTitle,
        imageUrl: this.imageUrl,
        linkUrl: this.linkUrl,
        count: this.count,
      })
      .then(function(response) {
        var res = response.data;
        swal2.success("投票项修改成功！");
        $this.apiGetItems();
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        utils.loading(false);
      });
  },

  apiUp: function(item) {
    var $this = this;

    utils.loading(true);
    $api
      .post('actions/up', {
        itemId: item.id
      })
      .then(function(response) {
        var res = response.data;
        swal2.success("投票项排序成功！");
        $this.apiGetItems();
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        utils.loading(false);
      });
  },

  apiDown: function(item) {
    var $this = this;

    utils.loading(true);
    $api
      .post('actions/down', {
        itemId: item.id
      })
      .then(function(response) {
        var res = response.data;
        swal2.success("投票项排序成功！");
        $this.apiGetItems();
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        utils.loading(false);
      });
  },

  apiDelete: function(item) {
    var $this = this;

    utils.loading(true);
    $api
      .delete(item.id + '')
      .then(function(response) {
        var res = response.data;
        swal2.success("投票项删除成功！");
        $this.apiGetItems();
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        utils.loading(false);
      });
  },

  btnAddClick: function() {
    this.pageType = "add";
    this.pageAlert = null;
    this.itemId = 0;
    this.title = '';
    this.subTitle = '';
    this.imageUrl = '';
    this.linkUrl = '';
    this.count = 0;
  },

  btnExportClick: function() {
    var $this = this;

    utils.loading(true);
    $api
      .post('actions/export')
      .then(function(response) {
        var res = response.data;
        swal2.success("投票项导出成功！");
        window.open(res.value);
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        utils.loading(false);
      });
  },

  btnEditClick: function(item) {
    this.pageType = "add";
    this.pageAlert = null;
    this.itemId = item.id;
    this.title = item.title ? item.title : '';
    this.subTitle = item.subTitle ? item.subTitle : '';
    this.imageUrl = item.imageUrl ? item.imageUrl : '';
    this.linkUrl = item.linkUrl ? item.linkUrl : '';
    this.count = item.count;
  },

  btnDownClick: function(item) {
    var $this = this;

    utils.alertDelete({
      title: "删除投票项",
      text: "此操作将删除投票项 " + item.title + "，确定吗？",
      callback: function() {
        $this.apiDelete(item);
      }
    });
  },

  btnDeleteClick: function(item) {
    var $this = this;

    utils.alertDelete({
      title: "删除投票项",
      text: "此操作将删除投票项 " + item.title + "，确定吗？",
      callback: function() {
        $this.apiDelete(item);
      }
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        if ($this.itemId) {
          $this.apiUpdate();
        } else {
          $this.apiInsert();
        }
      }
    });
  },

  btnCancelClick: function() {
    this.pageType = "list";
    this.item = null;
  },

  btnReturnClick: function() {
    utils.loading(true);
    location.href = this.returnUrl;
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

var $vue = new Vue({
  el: "#main",
  data: data,
  components: {
    FileUpload: VueUploadComponent
  },
  methods: methods,
  created: function() {
    this.apiGetItems();
  }
});
