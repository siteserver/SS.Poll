import * as React from 'react';
import Upload from 'rc-upload';
import { Api } from '../lib/api';
import * as utils from '../utils/utils';

interface P {
  imageUrl: string;
  onChange: (imageUrl: string) => void;
}

export default class ImageUpload extends React.Component<P, {
  imageUrl: string
  loading: boolean
}> {
  constructor(props: P) {
    super(props);

    this.state = {
      imageUrl: props.imageUrl || '',
      loading: false
    };
  }

  render() {
    var api: Api = new Api(false);
    const url = api.url('image');
    const uploadProps = utils.getUploadFilesProps(
      url,
      false,
      'image/png, image/jpeg, image/gif',
      (fileUrl: string) => {
        this.setState({
          loading: false,
          imageUrl: fileUrl
        });
        this.props.onChange(fileUrl);
      },
      (errorMessage: string) => {
        this.setState({
          loading: false
        });
        utils.Swal.warning(errorMessage);
      },
      () => {
        this.setState({
          loading: true
        });
      }
    );

    return (
      <div className="input-group m-t-10">
        <input value={this.state.imageUrl || ''} disabled={true} type="text" className="form-control" />
        <span className="input-group-btn">

          <button style={{ display: this.state.loading ? '' : 'none' }} type="button" className="btn btn-warning">图片上传中，请稍后...</button>
          <Upload {...uploadProps}>
            <button style={{ display: this.state.loading ? 'none' : '' }} type="button" className="btn btn-primary">上传...</button>
          </Upload>

        </span>
      </div>
    );
  }
}
