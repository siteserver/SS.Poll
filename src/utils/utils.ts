import * as swal from 'sweetalert';

export class PageType {
  public static VALUE_EDIT_ITEMS: string = 'edit_items';
  public static VALUE_EDIT_FIELDS: string = 'edit_fields';
  public static VALUE_EDIT_SETTINGS: string = 'edit_settings';

  public static TEXT_EDIT_ITEMS: string = '投票项管理';
  public static TEXT_EDIT_FIELDS: string = '提交字段管理';
  public static TEXT_EDIT_SETTINGS: string = '投票选项';
}

export function getUploadFilesProps(url: string, multi: boolean, accept: string, success: (fileUrl: string) => void, error: (errorMessage: string) => void, progress: () => void) {
  return {
    action: url,
    multiple: multi,
    dataType: 'json',
    accept: accept,
    maxFileSize: 5000000, // 5 MB
    withCredentials: true,
    onSuccess(fileUrl: string) {
      success(fileUrl);
    },
    onError(err: { errorMessage: string }) {
      error(err.errorMessage);
    },
    onProgress() {
      progress();
    }
  };
}

export class Swal {
  static tip(title: string, text?: string, isTimer?: boolean) {
    if (isTimer) {
      swal({
        title: title,
        text: text,
        timer: 2000,
        confirmButtonText: '确认',
        cancelButtonText: '取消',
        html: true
      });
    } else {
      swal({
        title: title,
        text: text,
        confirmButtonText: '确认',
        cancelButtonText: '取消',
        html: true
      });
    }
  }

  static success(title: string, callback?: () => void) {
    swal(
      {
        title: title,
        text: '',
        type: 'success',
        confirmButtonText: '确认',
        html: true
      },
      callback);
  }

  static successConfirm(title: string, confirm: (isConfirm: boolean) => void) {
    swal(
      {
        title: title,
        type: 'success',
        showCancelButton: true,
        confirmButtonText: '确认',
        cancelButtonText: '取消',
        html: true
      },
      confirm);
  }

  static error(errorMessage: string, callback?: () => void) {
    swal(
      {
        title: errorMessage,
        text: '',
        type: 'error',
        confirmButtonText: '确认',
        cancelButtonText: '取消',
        html: true
      },
      callback);
  }

  static warning(title: string, text?: string) {
    swal({
      title: title,
      text: text,
      type: 'warning',
      confirmButtonText: '确认',
      cancelButtonText: '取消',
      html: true
    });
  }

  static delete(title: string, text: string, confirm: (isConfirm: boolean) => void) {
    swal(
      {
        title: title,
        text: text,
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#f36c60',
        confirmButtonText: '确认删除',
        cancelButtonText: '取消',
        closeOnConfirm: true,
        html: true
      },
      confirm);
  }

  static confirm(title: string, text: string, confirm: (isConfirm: boolean) => void) {
    swal(
      {
        title: title,
        text: text,
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#f36c60',
        confirmButtonText: '确认',
        cancelButtonText: '取消',
        closeOnConfirm: true,
        html: true
      },
      confirm
    );
  }
}
