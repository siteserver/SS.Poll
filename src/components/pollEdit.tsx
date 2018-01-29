import * as React from 'react';
import { Poll } from '../models';
var moment = require('moment');
require('moment/locale/zh-cn');
import * as Datetime from 'react-datetime';

interface P {
  poll: Poll;
  onSave: (poll: Poll) => void;
}

interface S {
  isImage: boolean;
  isUrl: boolean;
  isResult: boolean;
  isTimeout: boolean;
  timeToStart: Date;
  timeToEnd: Date;
  isCheckbox: boolean;
  checkboxMin: number;
  checkboxMax: number;
  isProfile: boolean;
}

export default class PollEdit extends React.Component<P, S> {
  constructor(props: P) {
    super(props);
    this.state = {
      isImage: props.poll.isImage,
      isUrl: props.poll.isUrl,
      isResult: props.poll.isResult,
      isTimeout: props.poll.isTimeout,
      timeToStart: props.poll.timeToStart,
      timeToEnd: props.poll.timeToEnd,
      isCheckbox: props.poll.isCheckbox,
      checkboxMin: props.poll.checkboxMin,
      checkboxMax: props.poll.checkboxMax,
      isProfile: props.poll.isProfile,
    };

    this.handleIsImageChange = this.handleIsImageChange.bind(this);
    this.handleIsUrlChange = this.handleIsUrlChange.bind(this);
    this.handleIsTimeoutChange = this.handleIsTimeoutChange.bind(this);
    this.handleTimeToStartChange = this.handleTimeToStartChange.bind(this);
    this.handleTimeToEndChange = this.handleTimeToEndChange.bind(this);
    this.handleIsCheckboxChange = this.handleIsCheckboxChange.bind(this);
    this.handleIsProfileChange = this.handleIsProfileChange.bind(this);
    this.handleIsResultChange = this.handleIsResultChange.bind(this);
    this.handleCheckboxMinChange = this.handleCheckboxMinChange.bind(this);
    this.handleCheckboxMaxChange = this.handleCheckboxMaxChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  handleIsImageChange() {
    this.setState({
      isImage: !this.state.isImage
    });
  }

  handleIsUrlChange() {
    this.setState({
      isUrl: !this.state.isUrl
    });
  }

  handleIsTimeoutChange() {
    this.setState({
      isTimeout: !this.state.isTimeout
    });
  }

  handleTimeToStartChange(date: any) {
    var timeToStart = date.toDate();
    this.setState({
      timeToStart: timeToStart
    });
  }

  handleTimeToEndChange(date: any) {
    var timeToEnd = date.toDate();
    this.setState({
      timeToEnd: timeToEnd
    });
  }

  handleIsCheckboxChange() {
    this.setState({
      isCheckbox: !this.state.isCheckbox
    });
  }

  handleIsResultChange() {
    this.setState({
      isResult: !this.state.isResult
    });
  }

  handleIsProfileChange() {
    this.setState({
      isProfile: !this.state.isProfile
    });
  }

  handleCheckboxMinChange(event: { target: { value: string } }) {
    this.setState({
      checkboxMin: parseInt(event.target.value, 10) || 0
    });
  }

  handleCheckboxMaxChange(event: { target: { value: string } }) {
    this.setState({
      checkboxMax: parseInt(event.target.value, 10) || 0
    });
  }

  handleSubmit() {
    var poll = this.props.poll;
    poll.isImage = this.state.isImage;
    poll.isUrl = this.state.isUrl;
    poll.isResult = this.state.isResult;
    poll.isTimeout = this.state.isTimeout;
    poll.timeToStart = this.state.timeToStart;
    poll.timeToEnd = this.state.timeToEnd;
    poll.isCheckbox = this.state.isCheckbox;
    poll.checkboxMin = this.state.checkboxMin;
    poll.checkboxMax = this.state.checkboxMax;
    poll.isProfile = this.state.isProfile;

    this.props.onSave(poll);
  }

  render() {

    return (
      <div className="row">
        <div className="col-sm-12">
          <div className="card-box">
            <h4 className="m-t-0 header-title">
              <b>投票设置</b>
            </h4>
            <p className="text-muted m-b-30 font-13">在此设置投票以及选项信息</p>
            <div className="row">
              <div className="form-horizontal" role="form">
                <div className="form-group">
                  <label className="col-md-2 control-label">投票选项</label>
                  <div className="col-md-10">
                    <div className="checkbox checkbox-primary">
                      <input id="isImage" type="checkbox" checked={this.state.isImage} onChange={this.handleIsImageChange} />
                      <label htmlFor="isImage">包含图片</label>
                      <input id="isUrl" type="checkbox" checked={this.state.isUrl} onChange={this.handleIsUrlChange} />
                      <label htmlFor="isUrl" style={{marginLeft: 30}}>包含链接</label>
                      <input id="isTimeout" type="checkbox" checked={this.state.isTimeout} onChange={this.handleIsTimeoutChange} />
                      <label htmlFor="isTimeout" style={{marginLeft: 30}}>限制时间段</label>
                      <input id="isCheckbox" type="checkbox" checked={this.state.isCheckbox} onChange={this.handleIsCheckboxChange} />
                      <label htmlFor="isCheckbox" style={{marginLeft: 30}}>可多选</label>
                    </div>
                  </div>
                </div>
                <div className="form-group" style={{ display: this.state.isTimeout ? '' : 'none' }}>
                  <label className="col-md-2 control-label" htmlFor="maxNum">开始时间</label>
                  <div className="col-md-3">
                    <Datetime value={(this.state.timeToStart + '').indexOf('1754') !== -1 ? moment(new Date()) : moment(this.state.timeToStart)} onChange={this.handleTimeToStartChange} locale="zh-cn" />
                  </div>
                  <span className="col-md-7">&nbsp;</span>
                </div>
                <div className="form-group" style={{ display: this.state.isTimeout ? '' : 'none' }}>
                <label className="col-md-2 control-label" htmlFor="maxNum">结束时间</label>
                <div className="col-md-3">
                  <Datetime value={(this.state.timeToEnd + '').indexOf('1754') !== -1 ? moment().add(3, 'month') : moment(this.state.timeToEnd)} onChange={this.handleTimeToEndChange} locale="zh-cn" />
                </div>
                <span className="col-md-7">&nbsp;</span>
              </div>
                <div className="form-group" style={{ display: this.state.isCheckbox ? '' : 'none' }}>
                  <label className="col-md-2 control-label" htmlFor="maxNum">最少选择</label>
                  <div className="col-md-1">
                    <input type="text" value={this.state.checkboxMin || ''} onChange={this.handleCheckboxMinChange} className="form-control" />
                  </div>
                  <label className="col-md-1 control-label" htmlFor="checkboxMax">最多选择</label>
                  <div className="col-md-1">
                    <input type="text" value={this.state.checkboxMax || ''} onChange={this.handleCheckboxMaxChange} className="form-control" />
                  </div>
                  <span className="col-md-4">&nbsp;</span>
                </div>
                <div className="form-group">
                  <label className="col-md-2 control-label">投票界面选项</label>
                  <div className="col-md-10">
                    <div className="checkbox checkbox-primary">
                      <input id="isProfile" type="checkbox" checked={this.state.isProfile} onChange={this.handleIsProfileChange} />
                      <label htmlFor="isProfile">需要提交投票者信息</label>
                      <input id="isResult" type="checkbox" checked={this.state.isResult} onChange={this.handleIsResultChange} />
                      <label htmlFor="isResult" style={{marginLeft: 30}}>投票结束后显示结果</label>
                    </div>
                  </div>
                </div>
                <div className="form-group m-b-0">
                  <div className="col-sm-offset-2 col-sm-10">
                    <button className="btn btn-success btn-md" onClick={this.handleSubmit}>保 存</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
