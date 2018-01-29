import * as React from 'react';
import { Poll, Item } from '../models';
import { Api } from '../lib/api';

interface P {
  poll: Poll;
  items: Array<Item>;
}

export default class View extends React.Component<P, {}> {
  constructor(props: P) {
    super(props);
  }

  getExportUrl() {
    var api: Api = new Api(true);
    return api.url('exportResults', this.props.poll.id);
  }

  render() {
    var listEl = null;
    let totalCount: number = 0;
    this.props.items.forEach((item: Item) => {
      totalCount += item.count;
    });
    if (this.props.poll.isImage) {
      listEl = (
        <div className="vote1_cont">
          <div className="vote1_title">总计：{totalCount}票</div>
          <ul>
            {
              this.props.items.map((item: Item) => {
                const percent = totalCount === 0 ? 0 : Math.round((item.count / totalCount) * 100);
                return (
                  <li key={item.id}>
                    <div className="vote1_img">
                      <img src={item.imageUrl} />
                      <p>{item.title}</p>
                    </div>
                    <div className="vote1_num">{item.count}票</div>
                    <div className="vote1_plan">
                      <span style={{ width: percent + '%' }} />
                    </div>
                    <div className="vote1_percent">{percent + '%'}</div>
                  </li>
                );
              })
            }
          </ul>
        </div>
      );
    } else {
      listEl = (
        <div className="vote1_cont vote1_cont1">
          <div className="vote1_title">总计：{totalCount}票</div>
          <ul>
            {
              this.props.items.map((item: Item) => {
                const percent = totalCount === 0 ? 0 : Math.round((item.count / totalCount) * 100);
                return (
                  <li key={item.id}>
                    <div className="vote1_text">{item.title}</div>
                    <div className="vote1_num">{item.count}票</div>
                    <div className="vote1_plan">
                      <span style={{ width: percent + '%' }} />
                    </div>
                    <div className="vote1_percent">{percent + '%'}</div>
                  </li>
                );
              })
            }
          </ul>
        </div>
      );
    }

    return (
      <div className="row">
        <div className="col-sm-12">
          <div className="card-box">

            <h4 className="m-t-0 header-title"><b>投票数统计</b></h4>

            <div className="results_wrap">
              {listEl}
            </div>

            <div className="m-t-10">
              <a className="btn btn-success btn-md" target="_blank" href={this.getExportUrl()}>导 出</a>
            </div>

          </div>
        </div>
      </div>
    );
  }
}