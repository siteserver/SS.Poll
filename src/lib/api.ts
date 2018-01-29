export class Api {
  isHttp: boolean;
  apiUrl: string;
  publishmentSystemId: number;
  channelId: number;
  contentId: number;
  returnUrl: string;

  constructor(isHttp: boolean) {
    this.isHttp = false;
    this.apiUrl = this.getQueryStringByName('apiUrl') + (isHttp ? '/http/' : '/json/') + 'ss-poll';
    this.publishmentSystemId = parseInt(this.getQueryStringByName('publishmentSystemId'), 10) || 0;
    this.channelId = parseInt(this.getQueryStringByName('channelId'), 10) || 0;
    this.contentId = parseInt(this.getQueryStringByName('contentId'), 10) || 0;
    this.returnUrl = this.getQueryStringByName('returnUrl');
  }

  getQueryStringByName(name: string) {
    var result = location.search.match(new RegExp('[\?\&]' + name + '=([^\&]+)', 'i'));
    if (!result || result.length < 1) {
      return '';
    }
    return decodeURIComponent(result[1]);
  }

  _getURL(url: string, data: object | null, method: string) {
    
    url += ((/\?/).test(url) ? '&' : '?');
    if ((typeof data === 'object') && method === 'GET') {
      var pairs = [];
      if (data) {
        for (var prop in data) {
          if (data.hasOwnProperty(prop)) {
            var k = encodeURIComponent(prop),
              v = encodeURIComponent(data[prop]);
            pairs.push(k + '=' + v);
          }
        }
      }
      url += '&' + pairs.join('&');
    }
    return url + '&' + (new Date()).getTime();
  }

  request(method: string, path: string, data: object | null, cb: (err: { status: number, message: string } | null, res: {} | null, status: number) => void) {
    var xhr = new XMLHttpRequest();
    xhr.open(method, this._getURL(path, data, method), true);
    xhr.withCredentials = true;
    if (cb) {
      xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
          if (xhr.status < 400) {
            cb(null, parse(xhr.responseText), xhr.status);
          } else {
            var err = parse(xhr.responseText);
            cb({
              status: xhr.status,
              message: err.message || errorCode(xhr.status)
            }, null, xhr.status);
          }
        }
      };
    }

    // xhr.dataType = 'json';
    xhr.setRequestHeader('Accept', 'application/vnd.siteserver+json; version=1');
    xhr.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
    if (data) {
      xhr.send(JSON.stringify(data));
    } else {
      xhr.send();
    }
  }

  url(name?: string, id?: number) {
    var url = this.apiUrl;
    if (name) {
      url += '/' + name;
      if (id) {
        url += '/' + id;
      }
    }
    if (this.publishmentSystemId) {
      url += '?publishmentSystemId=' + this.publishmentSystemId;
      if (this.channelId) {
        url += '&channelId=' + this.channelId;
      }
      if (this.contentId) {
        url += '&contentId=' + this.contentId;
      }
    }
    return this._getURL(url, null, 'GET');
  }

  get(data: object | null, cb: (err: { status: number, message: string } | null, res: {} | null, status: number) => void, name?: string, id?: number) {
    var url = this.apiUrl;
    if (name) {
      url += '/' + name;
      if (id) {
        url += '/' + id;
      }
    }
    if (this.publishmentSystemId) {
      url += '?publishmentSystemId=' + this.publishmentSystemId;
      if (this.channelId) {
        url += '&channelId=' + this.channelId;
      }
      if (this.contentId) {
        url += '&contentId=' + this.contentId;
      }
    }
    return this.request('GET', url, data, cb);
  }

  post(data: object | null, cb: (err: { status: number, message: string } | null, res: {} | null, status: number) => void, name?: string, id?: number) {
    var url = this.apiUrl;
    if (name) {
      url += '/' + name;
      if (id) {
        url += '/' + id;
      }
    }
    if (this.publishmentSystemId) {
      url += '?publishmentSystemId=' + this.publishmentSystemId;
      if (this.channelId) {
        url += '&channelId=' + this.channelId;
      }
      if (this.contentId) {
        url += '&contentId=' + this.contentId;
      }
    }
    return this.request('POST', url, data, cb);
  }

  put(data: object | null, cb: (err: { status: number, message: string } | null, res: {} | null, status: number) => void, name?: string, id?: number) {
    var url = this.apiUrl;
    if (name) {
      url += '/' + name;
      if (id) {
        url += '/' + id;
      }
    }
    if (this.publishmentSystemId) {
      url += '?publishmentSystemId=' + this.publishmentSystemId;
      if (this.channelId) {
        url += '&channelId=' + this.channelId;
      }
      if (this.contentId) {
        url += '&contentId=' + this.contentId;
      }
    }
    return this.request('PUT', url, data, cb);
  }

  delete(data: object | null, cb: (err: { status: number, message: string } | null, res: {} | null, status: number) => void, name?: string, id?: number) {
    var url = this.apiUrl;
    if (name) {
      url += '/' + name;
      if (id) {
        url += '/' + id;
      }
    }
    if (this.publishmentSystemId) {
      url += '?publishmentSystemId=' + this.publishmentSystemId;
      if (this.channelId) {
        url += '&channelId=' + this.channelId;
      }
      if (this.contentId) {
        url += '&contentId=' + this.contentId;
      }
    }
    return this.request('DELETE', url, data, cb);
  }

  patch(data: object | null, cb: (err: { status: number, message: string } | null, res: {} | null, status: number) => void, name?: string, id?: number) {
    var url = this.apiUrl;
    if (name) {
      url += '/' + name;
      if (id) {
        url += '/' + id;
      }
    }
    if (this.publishmentSystemId) {
      url += '?publishmentSystemId=' + this.publishmentSystemId;
      if (this.channelId) {
        url += '&channelId=' + this.channelId;
      }
      if (this.contentId) {
        url += '&contentId=' + this.contentId;
      }
    }
    return this.request('PATCH', url, data, cb);
  }
}

function parse(responseText: string) {
  try {
    return responseText ? JSON.parse(responseText) : {};
  } catch (e) {
    return {};
  }
}

function errorCode(status: number): string {
  switch (status) {
    case 400:
      return 'Bad Request';
    case 401:
      return 'Unauthorized';
    case 402:
      return 'Payment Required';
    case 403:
      return 'Forbidden';
    case 404:
      return 'Not Found';
    case 405:
      return 'Method Not Allowed';
    case 406:
      return 'Not Acceptable';
    case 407:
      return 'Proxy Authentication Required';
    case 408:
      return 'Request Timeout';
    case 409:
      return 'Conflict';
    case 410:
      return 'Gone';
    case 411:
      return 'Length Required';
    case 500:
      return 'Internal Server Error';
    default:
      return 'Unknown Error';
  }
}