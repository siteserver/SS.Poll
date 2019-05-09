var $api = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/" +
    utils.getQueryString("pluginId") +
    "/pages/settings/",
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
  pageConfig: null,
  pageLoad: false,
  pageAlert: null,
  pageType: 'list',
  pollInfo: null,
  fieldInfoList: [],
  attributeNames: null,
  administratorSmsNotifyKeys: null,
  userSmsNotifyKeys: null
};

var methods = {
  getAttributeText: function (attributeName) {
    if (attributeName === 'AddDate') {
      return '添加时间';
    }
    return attributeName;
  },

  getDisplaySettings: function () {
    var text = '';
    if (this.pollInfo.isImage) {
      text = '投票项包含图片';
      if (this.pollInfo.isUrl) {
        text += '及链接';
      }
    }
    else if (this.pollInfo.isUrl) {
      text = '投票项包含链接';
    }
    else {
      text = '投票项不含图片及链接';
    }
    return text;
  },

  getCheckSettings: function () {
    var text = '';
    if (this.pollInfo.isCheckbox) {
      text = '多选投票';
      if (this.pollInfo.checkboxMin) {
        text += '，最少选择 ' + this.pollInfo.checkboxMin + ' 项';
      }
      if (this.pollInfo.checkboxMax) {
        text += '，最多选择 ' + this.pollInfo.checkboxMax + ' 项';
      }
    }
    else {
      text = '单选投票';
    }
    return text;
  },

  load: function () {
    var $this = this;

    $api.get('').then(function (response) {
      var res = response.data;

      $this.pollInfo = res.value;
      $this.fieldInfoList = res.fieldInfoList;
      $this.attributeNames = res.attributeNames;
      $this.administratorSmsNotifyKeys = res.administratorSmsNotifyKeys;
      $this.userSmsNotifyKeys = res.userSmsNotifyKeys;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  submit: function () {
    var $this = this;

    var payload = {
      type: this.pageType
    };
    if (this.pageType === 'isClosed') {
      payload.isClosed = this.pollInfo.isClosed;
    } else if (this.pageType === 'title') {
      payload.title = this.pollInfo.title;
    } else if (this.pageType === 'description') {
      payload.description = this.pollInfo.description;
    } else if (this.pageType === 'isImage') {
      payload.isImage = this.pollInfo.isImage;
      payload.isUrl = this.pollInfo.isUrl;
    } else if (this.pageType === 'isCheckbox') {
      payload.isCheckbox = this.pollInfo.isCheckbox;
      payload.checkboxMin = this.pollInfo.checkboxMin;
      payload.checkboxMax = this.pollInfo.checkboxMax;
    } else if (this.pageType === 'isTimeout') {
      payload.isTimeout = this.pollInfo.isTimeout;
      payload.timeToStart = this.pollInfo.timeToStart;
      payload.timeToEnd = this.pollInfo.timeToEnd;
    } else if (this.pageType === 'isCaptcha') {
      payload.isCaptcha = this.pollInfo.isCaptcha;
    } else if (this.pageType === 'isAdministratorSmsNotify') {
      payload.isAdministratorSmsNotify = this.pollInfo.isAdministratorSmsNotify;
      payload.administratorSmsNotifyTplId = this.pollInfo.administratorSmsNotifyTplId;
      payload.administratorSmsNotifyKeys = this.administratorSmsNotifyKeys.join(',');
      payload.administratorSmsNotifyMobile = this.pollInfo.administratorSmsNotifyMobile;
    } else if (this.pageType === 'isAdministratorMailNotify') {
      payload.isAdministratorMailNotify = this.pollInfo.isAdministratorMailNotify;
      payload.administratorMailNotifyAddress = this.pollInfo.administratorMailNotifyAddress;
    } else if (this.pageType === 'isUserSmsNotify') {
      payload.isUserSmsNotify = this.pollInfo.isUserSmsNotify;
      payload.userSmsNotifyTplId = this.pollInfo.userSmsNotifyTplId;
      payload.userSmsNotifyKeys = this.userSmsNotifyKeys.join(',');
      payload.userSmsNotifyMobileName = this.pollInfo.userSmsNotifyMobileName;
    }

    utils.loading(true);
    $api.post('', payload).then(function (response) {
      var res = response.data;

      $this.pageType = 'list';
      swal2({
        toast: true,
        type: 'success',
        title: "设置保存成功",
        showConfirmButton: false,
        timer: 2000
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
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
