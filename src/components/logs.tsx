import * as React from 'react';
import * as ReactPaginate from 'react-paginate';
import { Poll, Log } from '../models';
import { Api } from '../lib/api';

interface P {
  poll: Poll;
}

interface S {
  data: Array<Log>;
  pageCount: number;
  offset: number;
}

export default class Logs extends React.Component<P, S> {
  constructor(props: P) {
    super(props);

    this.state = {
      data: [],
      pageCount: 0,
      offset: 0,
    };

    this.handlePageClick = this.handlePageClick.bind(this);
  }

  componentDidMount() {
    this.load();
  }

  load() {
    var api: Api = new Api(false);
    api.get(
      { limit: 20, offset: this.state.offset },
      (err, res: {
        logs: Array<Log>;
        totalCount: number;
      }) => {
        this.setState({
          data: res.logs,
          pageCount: Math.ceil(res.totalCount / 20)
        });
      },
      'logs',
      this.props.poll.id);
  }

  handlePageClick(data: { selected: number }) {
    let selected = data.selected;
    let offset = Math.ceil(selected * 20);

    this.setState({ offset: offset }, () => {
      this.load();
    });
  }

  getExportUrl() {
    var api: Api = new Api(true);
    return api.url('exportLogs', this.props.poll.id);
  }

  render() {
    const listEl = this.state.data.map((log: Log) => {
      return (
        <tr>
          <td>{log.fullName}</td>
          <td>{log.mobile}</td>
          <td>{log.addDate}</td>
        </tr>
      );
    });

    return (
      <div className="row">
        <div className="col-sm-12">
          <div className="card-box">

            <h4 className="m-t-0 header-title"><b>投票清单</b></h4>

            <div className="tablesaw-bar mode-stack" />
            <table className="tablesaw m-t-20 table m-b-0 tablesaw-stack">
              <thead>
                <tr>
                  <th scope="col">姓名</th>
                  <th scope="col">手机号</th>
                  <th scope="col">日期</th>
                </tr>
              </thead>
              <tbody>
                {listEl}
              </tbody>
            </table>

            <ReactPaginate previousLabel={'上一页'} nextLabel={'下一页'} breakLabel={<a href="">...</a>} breakClassName={'break-me'} pageCount={this.state.pageCount} marginPagesDisplayed={2} pageRangeDisplayed={5} onPageChange={this.handlePageClick} containerClassName={'pagination'} subContainerClassName={'pages pagination'} activeClassName={'active'} />

            <div className="m-t-10">
              <a className="btn btn-success btn-md" target="_blank" href={this.getExportUrl()}>导 出</a>
            </div>

          </div>
        </div>
      </div>
    );
  }
}