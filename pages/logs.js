var $api = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/" +
    utils.getQueryString("pluginId") +
    "/pages/logs/",
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
  fieldInfoList: null,
  allAttributeNames: [],
  listAttributeNames: [],
  page: 1,
  items: [],
  count: null,
  pages: null,
  pageOptions: null
};

var methods = {
  delete: function (logId) {
    var $this = this;

    utils.loading(true);
    $api.delete('', {
      params: {
        logId: logId
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
      $this.count = res.count;
      $this.pages = res.pages;
      $this.page = res.page;
      $this.pageOptions = [];
      for (var i = 1; i <= $this.pages; i++) {
        $this.pageOptions.push(i);
      }
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnEditClick: function (logId) {
    location.href = utils.getPageUrl('logAdd.html') + '&pollId=' + $pollId + '&logId=' + logId + '&returnUrl=' + encodeURIComponent(this.returnUrl);
  },

  btnDeleteClick: function (logId) {
    var $this = this;

    utils.alertDelete({
      title: '删除数据',
      text: '此操作将删除数据，确定吗？',
      callback: function () {
        $this.delete(logId);
      }
    });
  },

  btnExportClick: function () {
    utils.loading(true);

    $api.post('actions/export').then(function (response) {
      var res = response.data;

      swal2({
        toast: true,
        type: 'success',
        title: "数据导出成功",
        showConfirmButton: false,
        timer: 1000
      }).then(function () {
        window.open(res.value);
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnVisibleClick: function (attributeName) {
    var $this = this;
    event.stopPropagation();
    event.preventDefault();

    utils.loading(true);
    $api.post('actions/visible', {
      attributeName: attributeName
    }).then(function (response) {
      var res = response.data;

      $this.listAttributeNames = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
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

  getAttributeText: function (attributeName) {
    if (attributeName === 'Guid') {
      return '编号';
    } else if (attributeName === 'AddDate') {
      return '添加时间';
    }
    return attributeName;
  },

  getAttributeValue: function (item, attributeName) {
    return item[_.lowerFirst(attributeName)];
  },

  loadFirstPage: function () {
    if (this.page === 1) return;
    this.loadPage(1);
  },

  loadPrevPage: function () {
    if (this.page - 1 <= 0) return;
    this.loadPage(this.page - 1);
  },

  loadNextPage: function () {
    if (this.page + 1 > this.pages) return;
    this.loadPage(this.page + 1);
  },

  loadLastPage: function () {
    if (this.page + 1 > this.pages) return;
    this.loadPage(this.pages);
  },

  onPageSelect(option) {
    this.loadPage(option);
  },

  loadPage: function (page) {
    var $this = this;

    if ($this.pageLoad) {
      utils.loading(true);
    }

    $api.get('', {
      params: {
        page: page
      }
    }).then(function (response) {
      var res = response.data;

      if ($this.pageLoad) {
        utils.loading(false);
        utils.scrollToTop();
      } else {
        $this.pageLoad = true;
      }

      $this.fieldInfoList = res.fieldInfoList;
      $this.allAttributeNames = res.allAttributeNames;
      $this.listAttributeNames = res.listAttributeNames;

      $this.items = res.value;
      $this.count = res.count;
      $this.pages = res.pages;
      $this.page = res.page;
      $this.pageOptions = [];
      for (var i = 1; i <= $this.pages; i++) {
        $this.pageOptions.push(i);
      }
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  }
};

Vue.component("multiselect", window.VueMultiselect.default);

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.loadPage(1);
  }
});
