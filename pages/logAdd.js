var $api = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/" +
    utils.getQueryString("pluginId") +
    "/pages/logAdd/",
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
  logId: utils.getQueryInt('logId'),
  returnUrl: utils.getQueryString('returnUrl'),
  pageConfig: null,
  pageLoad: false,
  pageAlert: null,
  pageType: '',
  fieldInfoList: []
};

var methods = {
  load: function () {
    var $this = this;

    $api.get('', {
      params: {
        logId: this.logId
      }
    }).then(function (response) {
      var res = response.data;

      $this.fieldInfoList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  getValue: function (attributeName) {
    for (var i = 0; i < this.fieldInfoList.length; i++) {
      var style = this.fieldInfoList[i];
      if (style.attributeName === attributeName) {
        return style.value;
      }
    }
    return '';
  },

  setValue: function (attributeName, value) {
    for (var i = 0; i < this.fieldInfoList.length; i++) {
      var style = this.fieldInfoList[i];
      if (style.attributeName === attributeName) {
        style.value = value;
      }
    }
  },

  submit: function () {
    var $this = this;

    var payload = {
      logId: this.logId
    };
    for (var i = 0; i < this.fieldInfoList.length; i++) {
      var style = this.fieldInfoList[i];
      payload[style.title] = style.value;
    }

    utils.loading(true);
    $api.post('', payload).then(function (response) {
      var res = response.data;

      swal2({
        toast: true,
        type: 'success',
        title: "数据保存成功",
        showConfirmButton: false,
        timer: 2000
      }).then(function () {
        $this.btnNavClick("logs.html");
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnImageClick: function (imageUrl) {
    top.utils.openImagesLayer([imageUrl]);
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
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

Vue.component("date-picker", window.DatePicker.default);

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});