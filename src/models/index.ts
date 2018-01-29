export class Poll {
  public id: number;
  public siteId: number;
  public contentId: number;
  public isImage: boolean;
  public isUrl: boolean;
  public isResult: boolean;
  public isTimeout: boolean;
  public timeToStart: Date;
  public timeToEnd: Date;
  public isCheckbox: boolean;
  public checkboxMin: number;
  public checkboxMax: number;
  public isProfile: boolean;
}

export class Item {
  public id: number;
  public pollId: number;
  public title: string;
  public subTitle: string;
  public imageUrl: string;
  public linkUrl: string;
  public count: number;
}

export class Log {
  public id: number;
  public pollId: number;
  public itemIds: string;
  public fullName: string;
  public mobile: string;
  public cookieSn: string;
  public wxOpenId: string;
  public addDate: Date;
}