import * as React from 'react';
import * as cx from 'classnames';
import * as utils from '../utils/utils';

interface P {
  redirect: (pageType: string) => void;
}

export default class Footer extends React.Component<P, {}> {
  constructor(props: P) {
    super(props);
  }

  render() {
    let ulEl = (
      <ul className="navigation-menu">
        <li className={cx({ 'has-submenu': true, 'active': true })}>
          <a href="javascript:;"><i className="ion-compose" />{utils.PageType.TEXT_EDIT_ITEMS}</a>
        </li>
        <li className={cx({ 'has-submenu': true })}>
          <a href="javascript:;" onClick={() => { this.props.redirect(utils.PageType.VALUE_EDIT_FIELDS); }}><i className="ion-compose" />{utils.PageType.TEXT_EDIT_FIELDS}</a>
        </li>
        <li className={cx({ 'has-submenu': true })}>
          <a href="javascript:;" onClick={() => { this.props.redirect(utils.PageType.VALUE_EDIT_SETTINGS); }}><i className="ion-compose" />{utils.PageType.TEXT_EDIT_SETTINGS}</a>
        </li>
        <li className="has-submenu">
          <a href="javascript:;" onClick={() => { this.props.redirect(''); }}><i className="ion-ios-undo" />返回列表</a>
        </li>
      </ul>
    );

    return (
      <header id="topnav">
        <div className="navbar-custom">
          <div className="container">
            <div id="navigation">
              {ulEl}
            </div>
          </div>
        </div>
      </header>
    );
  }
}
