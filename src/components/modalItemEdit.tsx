import * as React from 'react';
import { Item } from '../models';
import ImageUpload from './imageUpload';
import * as utils from '../utils/utils';

interface P {
  isImage: boolean;
  isUrl: boolean;
  item: Item;
  handleSubmit: (item: Item) => void;
  handleDelete: (itemId: number) => void;
  handleCancel: () => void;
}

interface S {
  title: string;
  subTitle: string;
  imageUrl: string;
  linkUrl: string;
  count: number;
}

export default class ModalItemEdit extends React.Component<P, S> {
  constructor(props: P) {
    super(props);

    this.state = {
      title: props.item.title || '',
      subTitle: props.item.subTitle || '',
      imageUrl: props.item.imageUrl || '',
      linkUrl: props.item.linkUrl || '',
      count: props.item.count || 0,
    };

    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleDelete = this.handleDelete.bind(this);
    this.handleCancel = this.handleCancel.bind(this);
    this.handleTitleChange = this.handleTitleChange.bind(this);
    this.handleSubTitleChange = this.handleSubTitleChange.bind(this);
    this.handleImageUrlChange = this.handleImageUrlChange.bind(this);
    this.handleLinkUrlChange = this.handleLinkUrlChange.bind(this);
    this.handleCountChange = this.handleCountChange.bind(this);
  }

  handleSubmit() {
    var item: Item = this.props.item;
    item.title = this.state.title;
    item.subTitle = this.state.subTitle;
    item.imageUrl = this.state.imageUrl;
    item.linkUrl = this.state.linkUrl;
    item.count = this.state.count;
    if (!item.title) {
      return utils.Swal.error('请填写标题！');  
    }
    if (this.props.isImage && !item.imageUrl) {
      return utils.Swal.error('请上传图片！');  
    }
    if (this.props.isUrl && !item.linkUrl) {
      return utils.Swal.error('请填写链接！');  
    }
    this.props.handleSubmit(item);
  }

  handleDelete() {
    utils.Swal.confirm('删除投票项', '此操作将删除投票项，确定删除吗？', (isConfirm: boolean) => {
      if (isConfirm) {
        this.props.handleDelete(this.props.item.id);
      }
    });    
  }

  handleCancel() {
    this.props.handleCancel();
  }

  handleTitleChange(event: { target: { value: string } }) {
    this.setState({
      title: event.target.value
    });
  }

  handleSubTitleChange(event: { target: { value: string } }) {
    this.setState({
      subTitle: event.target.value
    });
  }

  handleImageUrlChange(imageUrl: string) {
    this.setState({
      imageUrl
    });
  }

  handleLinkUrlChange(event: { target: { value: string } }) {
    this.setState({
      linkUrl: event.target.value
    });
  }

  handleCountChange(event: { target: { value: string } }) {
    this.setState({
      count: parseInt(event.target.value, 10) || 0
    });
  }

  render() {
    var submitName = this.props.item.id ? '编辑' : '新增';
    var delEl = this.props.item.id ? <button type="button" className="btn btn-danger" onClick={this.handleDelete}>删除</button> : null;

    return (
      <div id="con-close-modal" className="modal" style={{ display: 'block' }}>
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <button type="button" className="close" onClick={this.handleCancel}>×</button>
              <h4 className="modal-title">{submitName + '投票选项'}</h4>
            </div>
            <div className="modal-body">
              <div className="row">
                <div className="col-md-12">
                  <div className="form-group">
                    <label className="control-label">标题</label>
                    <input value={this.state.title || ''} onChange={this.handleTitleChange} type="text" className="form-control" />
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col-md-12">
                  <div className="form-group">
                    <label className="control-label">副标题</label>
                    <input value={this.state.subTitle || ''} onChange={this.handleSubTitleChange} type="text" className="form-control" />
                  </div>
                </div>
              </div>

              <div className="row" style={{display: this.props.isImage ? '' : 'none'}}>
                <div className="col-md-12">
                  <div className="form-group no-margin">
                    <label className="control-label">图片</label>

                    <ImageUpload imageUrl={this.state.imageUrl} onChange={this.handleImageUrlChange} />

                  </div>
                </div>
              </div>

              <div className="row" style={{display: this.props.isUrl ? '' : 'none'}}>
                <div className="col-md-12">
                  <div className="form-group no-margin">
                    <label className="control-label">链接</label>
                    <input value={this.state.linkUrl || ''} onChange={this.handleLinkUrlChange} type="text" className="form-control" />
                  </div>
                </div>
              </div>

              <div className="row">
                <div className="col-md-4">
                  <div className="form-group">
                    <label className="control-label">票数</label>
                    <input value={this.state.count || 0} onChange={this.handleCountChange} type="text" className="form-control" />
                  </div>
                </div>
              </div>

            </div>
            <div className="modal-footer">
              <button type="button" className="btn btn-primary" onClick={this.handleSubmit}>确定</button>
              {delEl}
              <button type="button" className="btn btn-default" onClick={this.handleCancel}>取消</button>
            </div>
          </div>
        </div>
      </div>
    );
  }
}