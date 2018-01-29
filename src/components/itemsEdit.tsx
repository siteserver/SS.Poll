import * as React from 'react';
import { Poll, Item } from '../models';
import ModalItemEdit from './modalItemEdit';

const addpic = require('../images/plus.png');

interface P {
  poll: Poll;
  items: Array<Item>;
  onItemSave: (item: Item) => void;
  onItemDelete: (itemId: number) => void;
}

interface S {
  isImage: boolean;
  isUrl: boolean;
  items: Array<Item>;
  item: Item | null;
}

export default class ItemsEdit extends React.Component<P, S> {
  constructor(props: P) {
    super(props);

    this.state = {
      isImage: props.poll.isImage,
      isUrl: props.poll.isUrl,
      items: props.items,
      item: null
    };

    this.handleAdd = this.handleAdd.bind(this);
    this.handleEdit = this.handleEdit.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleDelete = this.handleDelete.bind(this);
    this.handleCancel = this.handleCancel.bind(this);
  }

  componentWillReceiveProps(nextProps: Readonly<P>) {
    this.state = {
      isImage: nextProps.poll.isImage,
      isUrl: nextProps.poll.isUrl,
      items: nextProps.items,
      item: null
    };
  }

  handleAdd() {
    var item = new Item();
    item.pollId = this.props.poll.id;

    this.setState({
      item
    });
  }

  handleEdit(item: Item) {
    this.setState({
      item
    });
  }

  handleSubmit(item: Item) {
    this.props.onItemSave(item);
    this.setState({
      item: null
    });
  }

  handleDelete(itemId: number) {
    this.props.onItemDelete(itemId);
    this.setState({
      item: null
    });
  }

  handleCancel() {
    this.setState({
      item: null
    });
  }

  render() {
    var modalEdit = this.state.item ? <ModalItemEdit isImage={this.state.isImage} isUrl={this.state.isUrl} item={this.state.item} handleSubmit={this.handleSubmit} handleDelete={this.handleDelete} handleCancel={this.handleCancel} /> : null;

    return (
      <div className="row">
        {modalEdit}
        <div className="col-sm-12">

          <div className="other_wrap">
            <div className="vote_list">
              <div className="w1060" style={{ marginTop: 20 }}>
                {/* <h2>候选人按姓氏笔画排序</h2>
                <h3>(请选择 1 - 10 个选项)</h3> */}
                <ul>
                  {this.state.items.map((item) => {
                    const imgEl = this.state.isImage ? <img src={item.imageUrl} width="163" height="163" /> : null;
                    return (
                      <li key={item.id}>
                        <label href="javascript:;" onClick={() => { this.handleEdit(item); }}>
                          {imgEl}
                          <p>{item.title}</p>
                          <p>{item.subTitle}</p>
                        </label>
                      </li>
                    );
                  })}
                  {/* <li>
                    <label>
                      <img src={pic} width="163" height="163" />
                      <p>新增投票项</p>
                    </label>
                  </li> */}
                  <li>
                    <label href="javascript:;" onClick={this.handleAdd}>
                      <img src={addpic} width="163" height="163" style={{display: this.state.isImage ? '' : 'none'}} />
                      <p>新增投票项</p>
                    </label>
                  </li>
                </ul>
              </div>
            </div>
            {/* <div className="vote_submit">
              <div className="w1060">
                <ul>
                  <li><b><i>*</i>姓名</b>
                    <input type="text" name="" id="" value="" />
                    <p>最长为5个汉字</p>
                  </li>
                  <li>
                    <b>
                      <i>*</i>手机号</b>
                    <input type="text" name="" id="" value="" />
                    <p>1位手机号码</p>
                  </li>
                  <li>
                    <b>
                      <i>*</i>验证码</b>
                    <input type="text" name="" id="" value="" className="w_input" />
                    <img src={verify} />
                    <p>(不清楚?请点击刷新)</p>
                  </li>
                </ul>
                <a href="#" className="vote_btn">提 交</a>
              </div>
            </div> */}
          </div>

        </div>
      </div>
    );
  }
}
