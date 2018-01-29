import * as React from 'react';

export default class Loading extends React.Component<{}, {}> {
    render() {
        return (
            <div style={{ padding: '20px 0' }}>
                <div className="container">
                    <div id="form" className="form-horizontal">
                        <div className="row">
                            <div className="col-sm-12">
                                <div className="card-box">
                                    <div className="row">
                                        <p className="text-muted font-13">载入中，请稍后...</p>
                                        <div className="progress progress-lg m-b-5">
                                            <div className="progress-bar progress-bar-warning progress-bar-striped active" style={{ width: '67%' }}>
                                                <span className="sr-only">67% Complete</span>
                                            </div>
                                        </div>
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