import * as React from 'react';
import Header from './components/header';
import ItemsEdit from './components/itemsEdit';
import Loading from './components/loading';
import { Poll, Item } from './models';
import { Api } from './lib/api';
import * as utils from './utils/utils';

import './css/bootstrap.min.css';
import './css/font-awesome.min.css';
import './css/ionicons.min.css';
import './css/plugin-utils.css';
import './css/image-poll.css';
import './css/results.css';
import './css/sweetalert.css';
import './css/react-datetime.css';

interface S {
  poll: Poll | null;
  items: Array<Item> | null;
}

class App extends React.Component<{}, S> {
  api: Api = new Api(false);

  constructor() {
    super();
    this.state = {
      poll: null,
      items: null
    };
    this._onPollSave = this._onPollSave.bind(this);
    this._onItemSave = this._onItemSave.bind(this);
    this._onItemDelete = this._onItemDelete.bind(this);
  }

  componentDidMount() {
    this.api.get(null, (err, res: {
      poll: Poll,
      items: Array<Item>
    }) => {
      this.setState({
        poll: res.poll,
        items: res.items
      });
    });
  }

  redirect(pageType: string) {
    const apiUrl = encodeURIComponent(this.api.getQueryStringByName('apiUrl'));
    const publishmentSystemId = encodeURIComponent(this.api.getQueryStringByName('publishmentSystemId'));
    const channelId = encodeURIComponent(this.api.getQueryStringByName('channelId'));
    const contentId = encodeURIComponent(this.api.getQueryStringByName('contentId'));
    const returnUrl = encodeURIComponent(this.api.getQueryStringByName('returnUrl'));
    if (pageType === utils.PageType.VALUE_EDIT_FIELDS) {
      location.href = `../pageFields.aspx?apiUrl=${apiUrl}&publishmentSystemId=${publishmentSystemId}&channelId=${channelId}&contentId=${contentId}&returnUrl=${returnUrl}`;
    } else if (pageType === utils.PageType.VALUE_EDIT_SETTINGS) {
      location.href = `../pageSettings.aspx?apiUrl=${apiUrl}&publishmentSystemId=${publishmentSystemId}&channelId=${channelId}&contentId=${contentId}&returnUrl=${returnUrl}`;
    } else {
      location.href = this.api.getQueryStringByName('returnUrl');
    }
  }

  _onPollSave(poll: Poll) {
    this.api.put(
      poll,
      (err, res: {}) => {
        utils.Swal.success('保存成功');
        this.setState({
          poll
        });
      },
      'poll',
      poll.id
    );
  }

  _onItemSave(item: Item) {
    if (item.id) {
      this.api.put(
        item,
        (err, res: {}) => {
          console.log(err);
          console.log(res);
        },
        'item',
        item.id
      );
    } else {
      this.api.post(
        item,
        (err, res: Item) => {
          var items = this.state.items || [];
          items.push(res);
          this.setState({
            items
          });
          console.log(err);
          console.log(res);
        },
        'item'
      );
    }
  }

  _onItemDelete(itemId: number) {
    var items: Array<Item> = [];
    (this.state.items || []).forEach((item: Item) => {
      if (item.id !== itemId) {
        items.push(item);
      }
    });
    this.setState({
      items
    });

    this.api.delete(
      null,
      (err, res: {}) => {
        console.log(err);
      },
      'item',
      itemId
    );
  }

  render() {
    if (!this.state.poll || !this.state.items) {
      return <Loading />;
    }

    let title = '';
    let containerEl = null;

    title = utils.PageType.TEXT_EDIT_ITEMS;
    containerEl = (
      <div>
        <ItemsEdit poll={this.state.poll} items={this.state.items} onItemSave={this._onItemSave} onItemDelete={this._onItemDelete} />
      </div>
    );

    return (
      <div className="wrapper">
        <Header redirect={(pageType: string) => { this.redirect(pageType); }} />

        <div className="container">
          <div className="row">
            <div className="col-sm-12">
              <h4 className="page-title">{title}</h4>
            </div>
          </div>

          {containerEl}
        </div>

      </div>
    );
  }
}

export default App;
